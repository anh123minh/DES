using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using TrafficManagement.WPF.Models;
using Microsoft.Win32;
using TrafficManagement.WPF.FileSerialization;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Threading;
using AForge.Genetic;
using QuickGraph;

namespace TrafficManagement.WPF.Pages
{
    /// <summary>
    /// Interaction logic for DynamicGraph.xaml
    /// </summary>
    public partial class EditorGraph: IDisposable
    {   // Глобальные перемены     
        public int maxValuePath = 5, IPCout = 0, ChannelCout = 0, ElementCout = 0;            
        private EditorOperationMode _opMode = EditorOperationMode.Select;
        private VertexControl _ecFrom;
        private readonly EditorObjectManager _editorManager;       
        public List<IEnumerable<DataEdge>> _allRoute;         
        private IEnumerable<IEnumerable<DataEdge>> PathEnum;
        private IEnumerable<IEnumerable<TaggedEquatableEdge<DataVertex, double>>> PathYen;
        public IEnumerable<DataEdge> EdgeStore;
        public IEnumerable<DataVertex> VertexStore;
        public List<DataVertex> ListVertex;
        public DataEdge edgeSelected;
        public DataVertex vertexSelected;
        private DataVertex vertexBefore;
        private bool overLoad=false;
        private enum VertexType
        {
            Center=0,
            VLB,
            Router,
            IP
        }
        private VertexType _vertextype = VertexType.Center;
        // Параметры генетического алгоритма
        private int populationSize = 40;
        private int iterations = 100;
        private int selectionMethod = 0;
        private int hybridAlgorithm = 0;
        private double mutationRate;
        private double crossoverRate;
        public int maxValue = 0;     
        public ushort[] bestchromosome = null;
        public Thread workerThread = null;
        public volatile bool needToStop = false;
        private windowResults windowResult;
        private windowDiagramAlpha _windowDiagramAlpha;
        private windowDiagramLoad _windowDiagramLoad;        
        public EditorGraph()
        {
            InitializeComponent();
            _editorManager = new EditorObjectManager(graphArea, zoomCtrl);
            var dgLogic = new LogicCoreExample();
            graphArea.LogicCore = dgLogic;
            graphArea.VertexSelected += graphArea_VertexSelected;
            graphArea.EdgeSelected += graphArea_EdgeSelected;
            graphArea.SetVerticesMathShape(VertexShape.Circle);
            graphArea.VertexLabelFactory = new DefaultVertexlabelFactory();
            dgLogic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Custom;
            dgLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.None;
            dgLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            dgLogic.EdgeCurvingEnabled = true;
            zoomCtrl.IsAnimationDisabled = true;
            ZoomControl.SetViewFinderVisibility(zoomCtrl, Visibility.Visible);
            zoomCtrl.Zoom = 2;
            zoomCtrl.MinZoom = .5;
            zoomCtrl.MaxZoom = 50;
            zoomCtrl.ZoomSensitivity = 25;
            zoomCtrl.MouseDown += zoomCtrl_MouseDown;
            butDelete.Checked += ToolbarButton_Checked;
            butSelect.Checked += ToolbarButton_Checked;
            butCenter.Checked += ToolbarButton_Checked;
            butVLB.Checked += ToolbarButton_Checked;
            butRouter.Checked += ToolbarButton_Checked;
            butIP.Checked += ToolbarButton_Checked;
            butDraw.Checked += ToolbarButton_Checked;
            butSelect.IsChecked = true;                       
            Loaded += GG_Loaded;            
            selectionBox.SelectedIndex = selectionMethod;
            cbxHybridAlgorithm.SelectedIndex = hybridAlgorithm;
            Title.Text = "noname.xml";
        }

        // Метод для получения и обновления параметров канала из окна настройки
      public  void GetParameterEdge(DataEdge edge)
        {           
            foreach (DataEdge edg in graphArea.LogicCore.Graph.Edges)
            {
              if ((edg.Source == edge.Target && edg.Target == edge.Source)||(edg.Source== edge.Source && edg.Target== edge.Target))
                {
                    edg.Capacity = edge.Capacity;
                    edg.Weight = edge.Weight;
                }              
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
            graphArea.StateStorage.LoadState("exampleState");
        }

       // Обработчик события выбора канала
        void graphArea_EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            HighlightBehaviour.SetHighlighted(args.EdgeControl, true);
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && _opMode == EditorOperationMode.Delete)
                graphArea.RemoveEdge(args.EdgeControl.Edge as DataEdge, true);
            if (args.MouseArgs.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                edgeSelected = (DataEdge)args.EdgeControl.Edge;
                args.EdgeControl.ContextMenu = new System.Windows.Controls.ContextMenu();                
                var miEdge = new System.Windows.Controls.MenuItem() { Header = "Параметры", Tag = args.EdgeControl};               
                miEdge.Click += MiEdge_Click;               
                args.EdgeControl.ContextMenu.Items.Add(miEdge);                
                args.EdgeControl.ContextMenu.IsOpen = true;
            } 
        }
        // Обработчик события выбора меню "Параметр" канала
        private void MiEdge_Click(object sender, RoutedEventArgs e)
        {
            windowParaEdge frmChanel = new windowParaEdge();
            frmChanel.SetValueControl(edgeSelected);    
            frmChanel.Closed += FrmChanel_Closed;
            frmChanel.Show();

        }
        // Обработчик события закрытия окна настройки параметров канала
        private void FrmChanel_Closed(object sender, EventArgs e)
        {
            GetParameterEdge(edgeSelected);
        }
        // Обработчик события выбора элемента
        void graphArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
             if(args.MouseArgs.LeftButton == MouseButtonState.Pressed)            
             {
                 if (_opMode == EditorOperationMode.AddEdge)
                    CreateEdgeControl(args.VertexControl);                
                 else if(_opMode == EditorOperationMode.Delete)
                     SafeRemoveVertex(args.VertexControl);
                 else if (_opMode == EditorOperationMode.Select && args.Modifiers == ModifierKeys.Control)
                     SelectVertex(args.VertexControl);
             }
             
            if (args.MouseArgs.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                vertexSelected = (DataVertex)args.VertexControl.Vertex;
                vertexBefore = (DataVertex)args.VertexControl.Vertex;
                args.VertexControl.ContextMenu = new System.Windows.Controls.ContextMenu();              
                var miVertex = new MenuItem { Header = "Параметры", Tag = args.VertexControl};             
                miVertex.Click += MiVertex_Click;
                args.VertexControl.ContextMenu.Items.Add(miVertex);
                args.VertexControl.ContextMenu.IsOpen = true;        
            }            
        }
        // Обработчик события выбора меню "Параметр" элемента
        private void MiVertex_Click(object sender, RoutedEventArgs e)
        {            
            windowParaVertex frmVertex = new windowParaVertex();
            frmVertex.SetValueControl(vertexSelected);
            frmVertex.Closed += FrmVertex_Closed;            
            frmVertex.Show();

        }
        // Обработчик события закрытия окна настройки параметров элемента
        private void FrmVertex_Closed(object sender, EventArgs e)
        {
            GetPameterVertex(vertexSelected);
        }
        // Получение параметров элемента из окна настройки 
        private void GetPameterVertex(DataVertex vertex)
        {
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
            {
                if (vtx.Text==vertexBefore.Text)
                {
                    vtx.Text = vertex.Text;
                    vtx.Traffic = vertex.Traffic;
                }
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
            graphArea.StateStorage.LoadState("exampleState");
        }
        // Метод для создание подсветки элемента 
        private static void SelectVertex(DependencyObject vc)
        {
            if (DragBehaviour.GetIsTagged(vc))
            {
                HighlightBehaviour.SetHighlighted(vc, false);
                DragBehaviour.SetIsTagged(vc, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vc, true);
                DragBehaviour.SetIsTagged(vc, true);
            }
        }
        // Обработка событий щелчка мышкой
        void zoomCtrl_MouseDown(object sender, MouseButtonEventArgs e)
        {           
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_opMode == EditorOperationMode.AddVertex)
                {                    
                    var pos = zoomCtrl.TranslatePoint(e.GetPosition(zoomCtrl), graphArea);
                    pos.Offset(-22.5,-22.5);
                    var vc = CreateVertexControl(pos);                
                }
                if(_opMode == EditorOperationMode.Select)
                {
                    ClearSelectMode(true);
                }               
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (_opMode == EditorOperationMode.AddEdge)
                    ClearEditMode();
            }
        }
        // Обработка событий щелчка мышкой на кнопке главного панела
        void ToolbarButton_Checked(object sender, RoutedEventArgs e)
        {
            if(butDelete.IsChecked == true && sender == butDelete)
            {
                butCenter.IsChecked = false;
                butVLB.IsChecked = false;
                butRouter.IsChecked = false;
                butIP.IsChecked = false;
                butSelect.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Arrow;
                _opMode = EditorOperationMode.Delete;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (butCenter.IsChecked == true && sender == butCenter)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butVLB.IsChecked = false;
                butRouter.IsChecked = false;
                butIP.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.Center;
                ClearSelectMode();
                return;
            }
            if (butVLB.IsChecked == true && sender == butVLB)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butCenter.IsChecked = false;
                butRouter.IsChecked = false;
                butIP.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.VLB;
                ClearSelectMode();
                return;
            }
            if (butRouter.IsChecked == true && sender == butRouter)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butVLB.IsChecked = false;
                butCenter.IsChecked = false;
                butIP.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.Router;
                ClearSelectMode();
                return;
            }
            if (butIP.IsChecked == true && sender == butIP)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butVLB.IsChecked = false;
                butRouter.IsChecked = false;
                butCenter.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.IP;
                ClearSelectMode();
                return;
            }
            if (butSelect.IsChecked == true && sender == butSelect)
            {
                butCenter.IsChecked = false;
                butVLB.IsChecked = false;
                butRouter.IsChecked = false;
                butIP.IsChecked = false;
                butDelete.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Hand;
                _opMode = EditorOperationMode.Select;
                ClearEditMode();
                graphArea.SetVerticesDrag(true, true);
                return;
            }
            if (butDraw.IsChecked == true && sender == butDraw)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                butVLB.IsChecked = false;
                butRouter.IsChecked = false;
                butCenter.IsChecked = false;
                butIP.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddEdge;              
                ClearSelectMode();
                return;
            }
        }
        // Метод для снятия режима выбора 
        private void ClearSelectMode(bool soft = false)
        {
            graphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    HighlightBehaviour.SetHighlighted(a, false);
                    DragBehaviour.SetIsTagged(a, false);
                });
            if (!soft)
                graphArea.SetVerticesDrag(false);
        }
        // Метод для снятия режима редактирования
        private void ClearEditMode()
        {
            if (_ecFrom != null) HighlightBehaviour.SetHighlighted(_ecFrom, false);
            _editorManager.DestroyVirtualEdge();
            _ecFrom = null;
        }
        // Метод для создания элемента сети передачи данных
        private VertexControl CreateVertexControl(System.Windows.Point position)
        {          
            var data = new DataVertex();
            switch(_vertextype)
            {
                case VertexType.Center:

                    data = new DataVertex("Центр сбора "+ (CountElement("Center")+1),"Center") { ImageId = 0 };
                    break;
                case VertexType.VLB:
                    data = new DataVertex("VLB "+ (CountElement("VLB") + 1), "VLB") { ImageId = 1};
                    break;
                case VertexType.Router:
                    data = new DataVertex("Маршрутизатор "+ (CountElement("Router") + 1), "Router") { ImageId = 2};
                    break;
                case VertexType.IP:
                    data = new DataVertex("ИП "+ (CountElement("IP") + 1), "IP") { ImageId = 3 ,Traffic=20};
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен!");
                    break;
            }           
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            graphArea.AddVertexAndData(data, vc, true);            
            return vc;
        }
        // Метод для создания канала сети передачи данных
        private void CreateEdgeControl(VertexControl vc)
        {
            if(_ecFrom == null)
            {
                _editorManager.CreateVirtualEdge(vc, vc.GetPosition());
                _ecFrom = vc;
                HighlightBehaviour.SetHighlighted(_ecFrom, true);
                return;
            }
            if(_ecFrom == vc) return;           
            var data = new DataEdge((DataVertex)_ecFrom.Vertex, (DataVertex)vc.Vertex);
            var ec = new EdgeControl(_ecFrom, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);
            HighlightBehaviour.SetHighlighted(_ecFrom, false);
            _ecFrom = null;
            _editorManager.DestroyVirtualEdge();      
            
        }
        // Метод для удаления элемента из сети передачи данных
        private void SafeRemoveVertex(VertexControl vc)
        {            
            graphArea.RemoveVertexAndEdges(vc.Vertex as DataVertex);
        }
        // Метод для удаления и сброса неуправляемых ресурсов
        public void Dispose()
        {
            if(_editorManager != null)
                _editorManager.Dispose();
            if(graphArea != null)
                graphArea.Dispose();                       
        }       
       
        void GG_Loaded(object sender, RoutedEventArgs e)
        {
            GG_RegisterCommands();
        }

        #region Commands

        #region GGRelayoutCommand

        private bool GGRelayoutCommandCanExecute(object sender)
        {
            return true;
        }

        #endregion
        // Сохранение данных элементов в память
        #region SaveStateCommand
        private static readonly RoutedCommand SaveStateCommand = new RoutedCommand();
        private void SaveStateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = graphArea.LogicCore.Graph != null && graphArea.VertexList.Count > 0;
        }

        private void SaveStateCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (graphArea.StateStorage.ContainsState("exampleState"))
                graphArea.StateStorage.RemoveState("exampleState");
            graphArea.StateStorage.SaveState("exampleState", "My example state");
        }
        #endregion
        // Получение состояния сети из памяти 
        #region LoadStateCommand
        private static readonly RoutedCommand LoadStateCommand = new RoutedCommand();
        private void LoadStateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = graphArea.StateStorage.ContainsState("exampleState");
        }

        private void LoadStateCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (graphArea.StateStorage.ContainsState("exampleState"))
                graphArea.StateStorage.LoadState("exampleState");
        }
        #endregion
        // Сохранение схемы компоновки элементов
        #region SaveLayoutCommand
        private static readonly RoutedCommand SaveLayoutCommand = new RoutedCommand();
        private void SaveLayoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = graphArea.LogicCore.Graph != null && graphArea.VertexList.Count > 0;
        }

        private void SaveLayoutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {            
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
            {
                vtx.ListPath = null;
            }
            var dlg = new SaveFileDialog { Filter = "Файл проекта|*.xml", Title = "Сохранить проект", FileName = ".xml" };
            if (dlg.ShowDialog() == true)
            {
                FileServiceProviderWpf.SerializeDataToFile(dlg.FileName, graphArea.ExtractSerializationData());
            }
            Title.Text = dlg.FileName;
        }
        #endregion
        // Получение схемы компоновки элементов
        #region LoadLayoutCommand

        private static readonly RoutedCommand LoadLayoutCommand = new RoutedCommand();      

        private static void LoadLayoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadLayoutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Все файлы|*.*", Title = "Открыть", FileName = "project1.xml" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                graphArea.RebuildFromSerializationData(FileServiceProviderWpf.DeserializeDataFromFile(dlg.FileName));
                graphArea.SetVerticesDrag(true, true);
                graphArea.UpdateAllEdges();
                graphArea.ShowAllVerticesLabels(true);
                zoomCtrl.ZoomToFill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Не может загрузить файл:\n {0}", ex));
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
            graphArea.StateStorage.LoadState("exampleState");
            Title.Text = dlg.FileName;
        }
        #endregion

        void GG_RegisterCommands()
        {
            CommandBindings.Add(new CommandBinding(SaveStateCommand, SaveStateCommandExecute, SaveStateCommandCanExecute));
            gg_saveState.Command = SaveStateCommand;
            CommandBindings.Add(new CommandBinding(LoadStateCommand, LoadStateCommandExecute, LoadStateCommandCanExecute));
            gg_loadState.Command = LoadStateCommand;

            CommandBindings.Add(new CommandBinding(SaveLayoutCommand, SaveLayoutCommandExecute, SaveLayoutCommandCanExecute));
            gg_saveLayout.Command = SaveLayoutCommand;
            btnSaveGA.Command = SaveLayoutCommand;           
            CommandBindings.Add(new CommandBinding(LoadLayoutCommand, LoadLayoutCommandExecute, LoadLayoutCommandCanExecute));
            gg_loadLayout.Command = LoadLayoutCommand;
                      
        }
        
        #endregion
               
        // Обработчик события нажатия на кнопке "Поиск"
        private void btnFindPath_Click(object sender, RoutedEventArgs e)
        {

            bool flaggoal = false;
            bool flagroot = false;
            int iPathCount;
            string goalID=null, rootID=null;
            PathEnum = null;
            PathList.Items.Clear();
            PathList.Items.Add("Список маршрутов:");            
            iPathCount = int.Parse(_tBxPathCount.Text);
            try
            {
                goalID = cbxGoal.SelectedItem.ToString();
            }
           catch(NullReferenceException)
            {
                MessageBox.Show("Назначение не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                rootID = cbxRoot.SelectedItem.ToString();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Источник не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            foreach (DataVertex vt in graphArea.LogicCore.Graph.Vertices)
            {
                if (vt.Text == goalID)
                {
                    flaggoal = true;
                }
                if (vt.Text == rootID)
                {
                    flagroot = true;
                }
            }
            if (flaggoal != true)
            {
                MessageBox.Show("Назначение не найден", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }

            if (flagroot != true)
            {
                MessageBox.Show("Источник не найден", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
             // Поиск по алгоритму Гоффман и Павлеи
                if (cbxAlgorithm.SelectedIndex == 0)
                {
                    // Реализация алгоритма поиска кратчайших маршрутов
                    Func<DataEdge, double> edgeWeights = E => E.Weight;
                    DataVertex root = new DataVertex();
                    DataVertex vertexToFind = new DataVertex();
                    foreach (DataVertex vertex in graphArea.LogicCore.Graph.Vertices)
                    {
                        if (vertex.Text == rootID) root = vertex;
                        if (vertex.Text == goalID) vertexToFind = vertex;
                    }
                    var rank = new HoffmanPavleyRankedShortestPathAlgorithm<DataVertex, DataEdge>(graphArea.LogicCore.Graph, edgeWeights);

                    DataVertex source = root;
                    DataVertex target = vertexToFind;
                    rank.ShortestPathCount = iPathCount;
                    rank.SetRootVertex(root);
                    rank.Compute(root, vertexToFind);
                    PathEnum = rank.ComputedShortestPaths;
                    if (rank.ComputedShortestPathCount == 0) PathList.Items.Add("Маршрут не найден !");
                    else
                    {
                        foreach (IEnumerable<DataEdge> path in PathEnum)
                        {
                            PathList.Items.Add(PathToString(path));
                        }
                    }
                }
            // Поиск по алгоритму Гоффман и Павлеи
            if (cbxAlgorithm.SelectedIndex == 1)
            {
                Func<DataEdge, double> edgeWeights = E => E.Weight;
                DataVertex root = new DataVertex();
                DataVertex vertexToFind = new DataVertex();
                foreach (DataVertex vertex in graphArea.LogicCore.Graph.Vertices)
                {
                    if (vertex.Text == rootID) root = vertex;
                    if (vertex.Text == goalID) vertexToFind = vertex;
                }              
                var Yen = new YenAlgorithm(graphArea.LogicCore.Graph,root, vertexToFind,iPathCount);
                PathEnum = Yen.Execute();
                if (PathEnum==null) PathList.Items.Add("Маршрут не найден !");
                else
                {
                    foreach (IEnumerable<DataEdge> path in PathEnum)
                    {
                        PathList.Items.Add(PathToString(path));
                    }
                }
            }

        }

        // Метод для получения названия маршрута по алгоритму Йена
        private string PathToString(IEnumerable<TaggedEquatableEdge<DataVertex, double>> NameOfPath)
        {
            string path = "";
            foreach (TaggedEquatableEdge<DataVertex, double> edge in NameOfPath)
            {
                path = edge.Source.ToString();
                break;
            }
            foreach (TaggedEquatableEdge<DataVertex, double> edge in NameOfPath)
                path = path + "->" + edge.Target.ToString();
            return path;
        }
        // Метод преобразования BidirectionalGraph в AdjacencyGraph
        private AdjacencyGraph<DataVertex, TaggedEquatableEdge<DataVertex, double>> ToAdjacencyGraph(BidirectionalGraph<DataVertex,DataEdge> BiGraph)
        {
            var adjGraph= new AdjacencyGraph<DataVertex, TaggedEquatableEdge<DataVertex, double>>();
            foreach (DataVertex vtx in BiGraph.Vertices)
            {
                adjGraph.AddVertex(vtx);
            }
            foreach (DataEdge edge in BiGraph.Edges)
            {
                var ed = new TaggedEquatableEdge<DataVertex, double>(edge.Source, edge.Target, edge.Weight);
                adjGraph.AddEdge(ed);
            }
            return adjGraph;
        }
        // Метод преобразования AdjacencyGraph в BidirectionalGraph 
        private BidirectionalGraph<DataVertex, DataEdge> ToBidirectionalGraph(AdjacencyGraph<DataVertex, TaggedEquatableEdge<DataVertex, double>> AdjGraph)
        {
            var BiGraph = new BidirectionalGraph<DataVertex, DataEdge>();
            foreach (DataVertex vtx in AdjGraph.Vertices)
            {
                BiGraph.AddVertex(vtx);
            }
            foreach (TaggedEquatableEdge<DataVertex, double> edge in AdjGraph.Edges)
            {
                var ed = new DataEdge(edge.Source, edge.Target, edge.Tag);
                BiGraph.AddEdge(ed);
            }
            return BiGraph;
        }
        // Метод для получения названия маршрута
        private string PathToString (IEnumerable<DataEdge> NameOfPath)
        {            
            string path ="";
            foreach (DataEdge edge in NameOfPath)
            {
                path = edge.Source.ToString();
                break;
            }
            foreach (DataEdge edge in NameOfPath)
                path = path + "->" + edge.Target.ToString();            
            return path;
        } 
        // Сохрание схемы компоновки элементов сети в виде изображения 
        private void gg_saveAsPngImage_Click(object sender, RoutedEventArgs e)
        {
            graphArea.ExportAsImageDialog(ImageType.PNG, true, 96D, 100);
        }
        // Создание нового проекта
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить файл ?", "", MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (graphArea.LogicCore.Graph != null && graphArea.VertexList.Count > 0)
                    {
                        var dlg = new SaveFileDialog { Filter = "Файл проекта|*.xml", Title = "Сохранить проект", FileName = ".xml" };
                        if (dlg.ShowDialog() == true)
                        {
                            FileServiceProviderWpf.SerializeDataToFile(dlg.FileName, graphArea.ExtractSerializationData());
                        }
                        Title.Text = dlg.FileName;
                    }
                    graphArea.LogicCore.Graph.Clear();
                    graphArea.ClearLayout();
                    break;
                case MessageBoxResult.No:                   
                    graphArea.LogicCore.Graph.Clear();
                    graphArea.ClearLayout();
                    Title.Text = "noname.xml";
                    break;
                case MessageBoxResult.Cancel:                    
                    break;
            }            
        }      
        // Печать схему компоновки элементов сети передачи данных
        private void gg_printlay_Click(object sender, RoutedEventArgs e)
        {
            graphArea.PrintDialog("Печать");
        }
        // Отображение выбранного канала на экране     
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            IEnumerable<DataEdge> listpath = null;
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                foreach (IEnumerable<DataEdge> path in PathEnum)
                {
                    string NameOfPath = PathToString(path);

                    if (NameOfPath == item.Content.ToString())
                    {
                        listpath = path;
                        break;
                    }
                }
                foreach (DataEdge ed in graphArea.LogicCore.Graph.Edges)
                {
                    ed.Color = "Green";
                    foreach (DataEdge edge in listpath)
                        if ((edge == ed) || (edge.Source == ed.Target && edge.Target == ed.Source)) ed.Color = "Gold";
                }
                graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
                graphArea.StateStorage.LoadState("exampleState");
            }
        }
 
        // Метод для поиска всех виртуальных каналов для измерителных пунктов
        private void FindAllPath(string Center,int hybridAlgorithm)
        {
            IPCout = 0;
            bool flag = false;
            maxValuePath = int.Parse(tbxMaxValue.Text);            
            _allRoute = null;
            var goal = new DataVertex();
            // поиск центр сбора в графе
            VertexStore = graphArea.LogicCore.Graph.Vertices;
            ListVertex = graphArea.LogicCore.Graph.Vertices.ToList();
            foreach (DataVertex vt in ListVertex)
            {
                if (vt.Text == Center)
                {
                    goal = vt;
                    flag = true;
                    break;
                }                
            }
            if (flag != true) MessageBox.Show("Назначение не найден","Внимание",MessageBoxButton.OKCancel,MessageBoxImage.Error);
            // поиск ИП и все маршрут из этого ИП в центр сбора
            foreach (DataVertex vt in ListVertex)
                if (vt.TypeOfVertex == "IP")
                {
                    IPCout++;
                    FindPath(vt, goal, maxValuePath, ref _allRoute,hybridAlgorithm);
                    vt.ListPath = _allRoute;
                }                  
                     
            ChannelCout = graphArea.LogicCore.Graph.EdgeCount;
            ElementCout = graphArea.LogicCore.Graph.VertexCount;
            EdgeStore = graphArea.LogicCore.Graph.Edges;
            
        }

        // Метод для поиска маршрутов для определенного источника и сохранения результата в двумерный массив
        private void FindPath(DataVertex root, DataVertex goal,int numberOfPath, ref List<IEnumerable<DataEdge>> allpath, int hybridAlgorithm)
        {
            bool flaggoal = false;
            bool flagroot = false;
            foreach (DataVertex vt in graphArea.LogicCore.Graph.Vertices)
            {
                if (vt.Text == goal.Text)
                {
                    flaggoal =true ;                    
                }
                if (vt.Text == root.Text)
                {
                    flagroot = true;
                }
            }
            if (flaggoal != true) MessageBox.Show("Не правильно задавать параметр в поле: Назначение", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (flagroot != true) MessageBox.Show("Не правильно задавать параметр в поле: Источник", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);

            if (graphArea.LogicCore.Graph.VertexCount != 0 && flaggoal == true && flagroot == true)
            {
                if (hybridAlgorithm == 0)
                {
                    Func<DataEdge, double> edgeWeights = E => E.Weight;
                    var rank = new HoffmanPavleyRankedShortestPathAlgorithm<DataVertex, DataEdge>(graphArea.LogicCore.Graph, edgeWeights);
                    rank.ShortestPathCount = numberOfPath;
                    rank.SetRootVertex(root);
                    rank.Compute(root, goal);
                    allpath = rank.ComputedShortestPaths.ToList();
                    // В случае, если заданное число кратчайших маршрутов больше чем фактическое
                    // то нужно добавлять еще маршруты
                    if (numberOfPath > allpath.Count)
                    {
                        int i = numberOfPath - allpath.Count;
                        var shortestPath = allpath[0];
                        while (i != 0)
                        {
                            allpath.Add(shortestPath);
                            i--;
                        }

                    }
                }

                if (hybridAlgorithm == 1)
                {
                    Func<DataEdge, double> edgeWeights = E => E.Weight;
                    var Yen = new YenAlgorithm(graphArea.LogicCore.Graph, root, goal, numberOfPath);
                    allpath = Yen.Execute().ToList();
                    // В случае, если заданное число кратчайших маршрутов больше чем фактическое
                    // то нужно добавлять еще маршруты
                    if (numberOfPath > allpath.Count)
                    {
                        int i = numberOfPath - allpath.Count;
                        var shortestPath = allpath[0];
                        while (i != 0)
                        {
                            allpath.Add(shortestPath);
                            i--;
                        }

                    }
                }
                if (hybridAlgorithm == 2)
                {
                    numberOfPath = 200;
                    Func<DataEdge, double> edgeWeights = E => E.Weight;
                    var rank = new HoffmanPavleyRankedShortestPathAlgorithm<DataVertex, DataEdge>(graphArea.LogicCore.Graph, edgeWeights);
                    rank.ShortestPathCount = numberOfPath;
                    rank.SetRootVertex(root);
                    rank.Compute(root, goal);
                    allpath = rank.ComputedShortestPaths.ToList();
                    // В случае, если заданное число кратчайших маршрутов больше чем фактическое
                    // то нужно добавлять еще маршруты
                    if (numberOfPath > allpath.Count)
                    {
                        int i = numberOfPath - allpath.Count;
                        var shortestPath = allpath[0];
                        while (i != 0)
                        {
                            allpath.Add(shortestPath);
                            i--;
                        }

                    }
                }

            }
            else MessageBox.Show("Маршрут не найден!");
        }
        // Объявления делегатов для включения асинхронного вызова к установке свойств элементов управления
        private delegate void SetTextCallback(System.Windows.Controls.TextBox control, string text);        
        private void SetText(System.Windows.Controls.TextBox control, string text)
        {
            if (Dispatcher.CheckAccess())
            {
                control.Text = text;
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Dispatcher.Invoke(d, new object[] { control, text });
            }
        }        
        private delegate void SetTextCallbackListView(System.Windows.Controls.ListView control, string text);       
        private void SetTextListView(System.Windows.Controls.ListView control, string text)
        {
            if (Dispatcher.CheckAccess())
            {
                control.Items.Add(text);
            }
            else
            {
                SetTextCallbackListView d = new SetTextCallbackListView(SetTextListView);
                Dispatcher.Invoke(d, new object[] { control, text });
            }
        }       
        private delegate void SetGraphColor(GraphAreaExample graph, ushort[] chromosome);        
        private void SetColor(GraphAreaExample graph, ushort[] chromosome)
        {
            if (Dispatcher.CheckAccess())
            {
                UpdateGraph(graphArea,ListVertex, chromosome);
            }
            else
            {
                SetGraphColor d = new SetGraphColor(SetColor);
                Dispatcher.Invoke(d, new object[] { graph, chromosome });
            }
        }       
        private delegate void UpdatewindowResults(windowResults windowResult,string chromosome, int iteration, double alpha);
        private void UpdateResults(windowResults windowResult,string chromosome, int iteration, double alpha)
        {
            string str = "Итерация № " + iteration.ToString();
            if (Dispatcher.CheckAccess())
            {
                windowResult.progressBar.Value = iteration;
                windowResult.ListBestChromosome.Items.Add(str);
                windowResult.ListBestChromosome.Items.Add(chromosome);
                windowResult.UpdateLineSeries(iteration, alpha);
            }
            else
            {
                UpdatewindowResults d = new UpdatewindowResults(UpdateResults);
                Dispatcher.Invoke(d, new object[] { windowResult,chromosome, iteration,alpha });
            }
        }

        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
            RefreshGraph();
        }
        // Обработчик события смены Tab
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                zoomCtrl.Cursor = Cursors.Arrow;
                _opMode = EditorOperationMode.Select;
                ClearEditMode();
                graphArea.SetVerticesDrag(true, true);
                if (TabControl.SelectedItem == TabFindPath)
                {
                    cbxGoal.Items.Clear();
                    cbxRoot.Items.Clear();
                    foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
                    {
                        cbxRoot.Items.Add(vtx.Text);
                        cbxGoal.Items.Add(vtx.Text);
                    }
                    cbxRoot.SelectedIndex = cbxRoot.Items.Count-1;
                    cbxGoal.SelectedIndex = 0;
                }
                if (TabControl.SelectedItem == TabGA)
                {
                    cbxCenter.Items.Clear();
                    foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
                    {
                        if (vtx.TypeOfVertex == "Center")
                            cbxCenter.Items.Add(vtx.Text);
                        
                    }
                    cbxCenter.SelectedIndex = 0;
                }
            }
           
        }
               
        // Обновление элемента формы
        private void UpdateSettings()
        {
            populationSizeBox.Text = populationSize.ToString();
            iterationsBox.Text = iterations.ToString();
        }
        // Обрабочик события нажатия на кнопке "Старт"
        private void startButton_Click_1(object sender, RoutedEventArgs e)
        {
            bool flag = false;  
            // Получение размер популяции          
            try
            {
                populationSize = Math.Max(10, Math.Min(500, int.Parse(populationSizeBox.Text)));
            }
            catch
            {
                populationSize = 40;
            }   
            // Получение числа итерации        
            try
            {
                iterations = Math.Max(0, int.Parse(iterationsBox.Text));
            }
            catch
            {
                iterations = 100;
            }
            // Получение числа кратчайших маршрутов         
            try
            {
                maxValue = Math.Max(0, int.Parse(tbxMaxValue.Text));
            }
            catch
            {
                maxValue = 8;
            }            
            // Получение коэффициента мутации
            try
            {
                mutationRate = Math.Max(0, double.Parse(tBxMutationRate.Text));
            }
            catch
            {
                mutationRate = 0.1;
            }
            // Получение коэффициент скрещивания
            try
            {
                crossoverRate = Math.Max(0, double.Parse(tBxCrossoverRate.Text));
            }
            catch
            {
                mutationRate = 0.1;
            }
            // Обновление элемента формы
            UpdateSettings();            
            selectionMethod = selectionBox.SelectedIndex;
            hybridAlgorithm = cbxHybridAlgorithm.SelectedIndex;
            // Генерация данных о возможных вариантах построения сети
            string center = null;
            try
            {
                center = cbxCenter.SelectedItem.ToString();
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Центр сбора не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (DataVertex vt in graphArea.LogicCore.Graph.Vertices)
            {
                if (vt.Text == center)
                {                   
                    flag = true;
                    break;
                }
            }
            if (flag != true) MessageBox.Show("Не правильно задавать параметр: Центр приема", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            else
            {
               // FindAllPath(center,hybridAlgorithm);             
                // Генерация рабочего потока
                needToStop = false;
                workerThread = new Thread(new ThreadStart(SearchSolution));
                windowResult = new windowResults();
                windowResult.btnStop.Click += windowResult_btnStop_click;
                windowResult.btnResult.Click += windowResult_BtnResult_Click;
                windowResult.btnSaveResult.Click += BtnSaveResult_Click;
                windowResult.btnDiagramAlpha.Click += BtnDiagramAlpha_Click;
                windowResult.btnDiagramLoad.Click += BtnDiagramLoad_Click;
                windowResult.Show();                   
                windowResult.progressBar.Maximum = iterations;
                windowResult.progressBar.Minimum = 1;
                windowResult.iterations = iterations;
                windowResult.ListBestChromosome.Items.Clear();
                windowResult.ListBestChromosome.Items.Add("Лучшие хромосомы:");
                FindAllPath(center, hybridAlgorithm);
                workerThread.Start();
                
            }            
        }
           
        // Открывает окно диаграммы нагрузки канала
        private void BtnDiagramLoad_Click(object sender, RoutedEventArgs e)
        {
            _windowDiagramLoad = new windowDiagramLoad(graphArea.LogicCore.Graph.Edges);
            _windowDiagramLoad.Show();
        }
        // Открывает окно диаграммы уровни использования сети
        private void BtnDiagramAlpha_Click(object sender, RoutedEventArgs e)
        {
            _windowDiagramAlpha = new windowDiagramAlpha(graphArea.LogicCore.Graph.Edges);
            _windowDiagramAlpha.Show();
        }
        // Сохраняет результаты в файл при нажатии на кнопке "Результат"        
        private void BtnSaveResult_Click(object sender, RoutedEventArgs e)
        {
            windowResult.btnResult.IsEnabled = false;
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
            {
                vtx.ListPath = null;
            }
            var dlg = new SaveFileDialog { Filter = "Все файлы|*.*", Title = "Сохранить результат", FileName = ".xml" };
            if (dlg.ShowDialog() == true)
            {
                FileServiceProviderWpf.SerializeDataToFile(dlg.FileName, graphArea.ExtractSerializationData());
            }
            Title.Text = dlg.FileName;
        }
        // Отображает на экране результата моделирования
        private void windowResult_BtnResult_Click(object sender, RoutedEventArgs e)
        {
            windowResult.btnResult.IsEnabled = false;
            windowResult.btnSaveResult.IsEnabled = true;
            windowResult.TabRoutes.Visibility = Visibility.Visible;
            windowResult.TabDiagram.Visibility = Visibility.Visible;
            windowResult.TabDiagram.IsSelected=true;
            btnSaveGA.IsEnabled = true;
            UpdateGraph(graphArea, ListVertex, bestchromosome);                   
            if (overLoad == true) MessageBox.Show("Сеть перегружена !", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            windowResult.ListBestRoutes.Items.Clear();           
            for (int i = 0; i < bestchromosome.Length; i++)
            {
                string nameIP = "ИП " + (i + 1).ToString();
                foreach (DataVertex vtx in ListVertex)
                    if (vtx.Text==nameIP)
                    {
                        windowResult.ListBestRoutes.Items.Add(PathToString(vtx.ListPath[bestchromosome[i]]));
                    }
                }
                
        }
        // Обработчик события нажатия на кнопке "Стоп" в окне windowResult
        private void windowResult_btnStop_click(object sender, RoutedEventArgs e)
        {
            if (workerThread != null)
            {
                needToStop = true;
                workerThread.Join(100);
                workerThread = null;
            }
        }
      
        // Метод для поиска оптимального результата с помощью генетического алгоритма
        void SearchSolution()
        {     
                FitnessFunction fitnessFunction = new FitnessFunction(EdgeStore, ListVertex);                      
                Population population = new Population(populationSize,
                    new ShortArrayChromosome(IPCout, maxValuePath - 1), fitnessFunction,
                    (selectionMethod == 0) ? (ISelectionMethod)new EliteSelection() :
                    (selectionMethod == 1) ? (ISelectionMethod)new RankSelection() :
                    (ISelectionMethod)new RouletteWheelSelection()
                    );
                population.MutationRate = mutationRate;
                population.CrossoverRate = crossoverRate;                
                int i = 1;
                string str;                
                ushort[] path = new ushort[IPCout];            
                while (!needToStop)
                {
                    population.RunEpoch();                    
                    ushort[] bestValue = ((ShortArrayChromosome)population.BestChromosome).Value;
                    bestchromosome = bestValue;
                    str = null;
                    if (IPCout == 1) str = (bestValue[0]+1).ToString();
                    else
                      for (int j = 0; j < bestValue.Length; j++)
                       {
                          str += (bestValue[j] + 1).ToString();
                       }                   
                    double alpha = Math.Round(fitnessFunction.Alpha(population.BestChromosome), 2);
                    overLoad = (alpha == 1) ? true : false;                    
                    UpdateResults(windowResult,str, i,alpha);                
                    i++;
                    if ((iterations != 0) && (i > iterations))
                        break;
                }       
        }

        private void cbxHybridAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (cbxHybridAlgorithm.SelectedIndex == 2) tbxMaxValue.IsEnabled = false;
                else tbxMaxValue.IsEnabled = true;
            }
                
        }




        //Метод для изменения цвета канала 
        private void UpdateGraph(GraphAreaExample graphArea,List<DataVertex> ListVertex, ushort[] path)
        {
            foreach (DataEdge ed in graphArea.LogicCore.Graph.Edges)
            {
                ed.Load = 0;
                ed.Alpha = 0;
                ed.Color = "Green";
            }
            for (int i = 0; i < path.Length; i++)
            {
                string nameIP = "ИП " + (i + 1).ToString();
                foreach (DataVertex vertex in ListVertex)
                    if (vertex.Text == nameIP)  
                    {                       
                        foreach (DataEdge ed in vertex.ListPath[path[i]])
                            foreach (DataEdge channel in graphArea.LogicCore.Graph.Edges)                             
                                if ((ed.Source == channel.Source && ed.Target == channel.Target) || (ed.Source == channel.Target && ed.Target == channel.Source))
                                {
                                    channel.Load = channel.Load + vertex.Traffic;                                  
                                }
                    }
            }
            foreach (DataEdge channel in graphArea.LogicCore.Graph.Edges)
            {
                if (channel.Load == 0) channel.Color = "LightBlue";
                else
                {
                    if (channel.Load <= (0.2 * channel.Capacity)) channel.Color = "LightGreen";
                    else
                    if (channel.Load <= (0.4 * channel.Capacity)) channel.Color = "Yellow";
                    else
                    if (channel.Load <= (0.6 * channel.Capacity)) channel.Color = "Gold";
                    else
                    if (channel.Load <= (0.8 * channel.Capacity)) channel.Color = "Orange";
                    else
                    if (channel.Load <= channel.Capacity) channel.Color = "Red";
                    else channel.Color = "DarkRed";                    
                }               
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
            graphArea.StateStorage.LoadState("exampleState");
        }
      // Метод для получения цвета каналов по умольчанию
           private void RefreshGraph()
        {            
            PathList.Items.Clear();
            foreach (DataEdge ed in graphArea.LogicCore.Graph.Edges)
            {                
                ed.Load = 0;
                ed.Alpha = 0;
                ed.Color = "Green";
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");
            graphArea.StateStorage.LoadState("exampleState");
        }
        // Метод для вычисления количества элементов (VLB, маршрутизатор, ИП, центр сбора) сети передачи данных
        private int CountElement(string Type)
        {
            if (graphArea.LogicCore.Graph == null) return 0;
            int cout=0;
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
                if (vtx.TypeOfVertex == Type) cout++;
            return cout;
        }
    }
}



