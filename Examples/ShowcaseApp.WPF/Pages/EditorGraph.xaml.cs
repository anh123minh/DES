using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using Microsoft.Win32;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Threading;
using AForge.Genetic;
using QuickGraph;
using React;
using SimulationV1.WPF.FileSerialization;
using SimulationV1.WPF.Models;
using SimulationV1.WPF.ExampleModels;

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for DynamicGraph.xaml
    /// </summary>
    public partial class EditorGraph : IDisposable
    {   // Глобальные перемены     
        public int maxValuePath = 5, IPCout = 0, ChannelCout = 0, ElementCout = 0;
        private EditorOperationMode _opMode = EditorOperationMode.Select;
        private VertexControl _ecFrom;
        private readonly EditorObjectManager _editorManager;
        public List<IEnumerable<DataEdge>> _allRoute;
        private IEnumerable<IEnumerable<DataEdge>> PathEnum;//Tập hợp các tập hợp của các Edge
        private IEnumerable<IEnumerable<TaggedEquatableEdge<DataVertex, double>>> PathYen;
        public IEnumerable<DataEdge> EdgeStore;
        public IEnumerable<DataVertex> VertexStore;
        public List<DataVertex> ListVertex;
        public DataEdge edgeSelected;
        public DataVertex vertexSelected;
        private DataVertex vertexBefore;
        private bool overLoad = false;
        //Khai bao tham so
        private enum VertexType//các kiểu của các element
        {
            AMCreate = 0,
            AMQueue,
            AMTerminate,
            AMAnd,
            IP
        }
        private enum EdgeType
        {
            AMArc = 0,
            AMDirection
        }
        private VertexType _vertextype = VertexType.AMCreate;
        private EdgeType _edgetype = EdgeType.AMArc;

        private Resource _ambarbers;
        //private List<Barber> _ambarbers = new List<Barber>();

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
        public volatile bool needToStop = false;//volatile - từ khóa khai báo biến được dùng cho nhiều Thread 
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
            BtnCreate.Checked += ToolbarButton_Checked;
            BtnQueue.Checked += ToolbarButton_Checked;
            BtnTerminate.Checked += ToolbarButton_Checked;
            BtnAccumulate.Checked += ToolbarButton_Checked;
            butDraw.Checked += ToolbarButton_Checked;
            butAMDraw.Checked += ToolbarButton_Checked;
            butSelect.IsChecked = true;
            Loaded += GG_Loaded;
            selectionBox.SelectedIndex = selectionMethod;
            cbxHybridAlgorithm.SelectedIndex = hybridAlgorithm;
            Title.Text = "noname.xml";

            //Moi them vao 22/02/2018
            graphArea.ShowAllEdgesLabels();
            //vertexSelected.Cnew.Cre1
        }

        #region ToolbarButton_Checked - Sự kiện khi nhấn chuột vào 1 biểu tượng bất kì và thiết lập các tham số: _opMode - các mode xử lý,_vertextype - mặc định là AMCreate
        // Обработка событий щелчка мышкой на кнопке главного панела
        void ToolbarButton_Checked(object sender, RoutedEventArgs e)
        {
            if (butDelete.IsChecked == true && sender == butDelete)
            {
                BtnCreate.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butSelect.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Arrow;
                _opMode = EditorOperationMode.Delete;
                ClearEditMode();
                ClearSelectMode();
                return;
            }
            if (BtnCreate.IsChecked == true && sender == BtnCreate)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butDraw.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.AMCreate;
                ClearSelectMode();
                return;
            }
            if (BtnQueue.IsChecked == true && sender == BtnQueue)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnCreate.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butDraw.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.AMQueue;
                ClearSelectMode();
                return;
            }
            if (BtnTerminate.IsChecked == true && sender == BtnTerminate)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnCreate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butDraw.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.AMTerminate;
                ClearSelectMode();
                return;
            }
            if (BtnAccumulate.IsChecked == true && sender == BtnAccumulate)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnCreate.IsChecked = false;
                butDraw.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.AddVertex;
                _vertextype = VertexType.AMAnd;
                ClearSelectMode();
                return;
            }
            if (butSelect.IsChecked == true && sender == butSelect)
            {
                BtnCreate.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butDelete.IsChecked = false;
                butDraw.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Hand;
                _opMode = EditorOperationMode.Select;
                ClearEditMode();
                graphArea.SetVerticesDrag(true, true);//Khi biểu tượng lá Select thì có thể kéo thả Vertex
                return;
            }
            if (butDraw.IsChecked == true && sender == butDraw)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnCreate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butAMDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _edgetype = EdgeType.AMArc;
                _opMode = EditorOperationMode.AddEdge;
                ClearSelectMode();
                return;
            }
            if (butAMDraw.IsChecked == true && sender == butAMDraw)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                BtnQueue.IsChecked = false;
                BtnTerminate.IsChecked = false;
                BtnCreate.IsChecked = false;
                BtnAccumulate.IsChecked = false;
                butDraw.IsChecked = false;
                zoomCtrl.Cursor = Cursors.Pen;
                _edgetype = EdgeType.AMDirection;
                _opMode = EditorOperationMode.AddEdge;
                ClearSelectMode();
                return;
            }
        }
        #endregion

        #region zoomCtrl_MouseDown - Sự kiện khi nhấn chuột xuống
        // Обработка событий щелчка мышкой
        void zoomCtrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)//Khi nhấn chuột trái xuống
            {
                if (_opMode == EditorOperationMode.AddVertex)//Nếu là mode thêm Vertex thì sinh 1 position rồi gọi method tạo Vertex
                {
                    var pos = zoomCtrl.TranslatePoint(e.GetPosition(zoomCtrl), graphArea);
                    pos.Offset(-22.5, -22.5);
                    var vc = CreateVertexControl(pos);//Tao cac Vertex
                }
                if (_opMode == EditorOperationMode.Select)//Nếu là mode chọn 1 Vertex trên khung làm việc thì call method xóa mode chọn
                {
                    ClearSelectMode(true);
                }
            }
            if (e.RightButton == MouseButtonState.Pressed)//Khi nhấn chuột phải xuống
            {
                if (_opMode == EditorOperationMode.AddEdge)
                    ClearEditMode();
            }
        }
        #endregion

        #region CreateVertexControl - Method tạo mới Vertex(vị trí)
        // Метод для создания элемента сети передачи данных       
        private VertexControl CreateVertexControl(System.Windows.Point position)
        {
            var data = new DataVertex();//Tạo mới 1 Vertex 
            switch (_vertextype)
            {
                case VertexType.AMCreate:
                    //Set mới 1 Vertex với pros: Text+số thứ tự, Type bằng Constructor và pro ImageId
                    data = new DataVertex("Генератор " + (CountElement("AMCreate") + 1), "AMCreate") { ImageId = 0, CreateType = new CreateClass()};
                    break;
                case VertexType.AMQueue:
                    data = new DataVertex("Очередь " + (CountElement("AMQueue") + 1), "AMQueue") { ImageId = 1, QueueType = new QueueClass()};
                    break;
                case VertexType.AMTerminate:
                    data = new DataVertex("Терминатор " + (CountElement("AMTerminate") + 1), "AMTerminate") { ImageId = 2, TerminateType = new TerminateClass()};
                    break;
                case VertexType.AMAnd:
                    data = new DataVertex("Конъюнкция " + (CountElement("AMAnd") + 1), "AMAnd") { ImageId = 3, AndType = new AndClass()};
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен!");
                    break;
            }
            var vc = new VertexControl(data);//Tạo mới 1 Control
            vc.SetPosition(position);//Set position cho Control tren màn hình
            graphArea.AddVertexAndData(data, vc, true);//Thêm data Vertex vào graph + thể hiện Control lên màn hình
            return vc;
        }

        #region CountElement - Method đếm số lượng Element
        // Метод для вычисления количества элементов (Очередь, Терминатор, ИП, Генератор) сети передачи данных
        private int CountElement(string Type)
        {
            if (graphArea.LogicCore.Graph == null) return 0;
            int cout = 0;
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
                if (vtx.TypeOfVertex == Type) cout++;
            return cout;
        }
        #endregion
        #endregion

        #region ClearSelectMode + ClearEditMode - Methods xử lý các sự kiện thoát các chế độ chọn
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
        #endregion

        #region graphArea_VertexSelected - Sự kiện khi 1 Vertex được chọn bằng chuột trái hoặc phải
        // Обработчик события выбора элемента
        void graphArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)//Chon bang chuot Left
            {
                //So sánh với các mode được thiết lập tại ToolbarButton_Checked
                if (_opMode == EditorOperationMode.AddEdge)//Che do them Edge moi
                    CreateEdgeControl(args.VertexControl);//Tao Edge moi
                else if (_opMode == EditorOperationMode.Delete)//Che do Delete
                    SafeRemoveVertex(args.VertexControl);//Xoa Vertex duoc chon
                else if (_opMode == EditorOperationMode.Select && args.Modifiers == ModifierKeys.Control)//Che do Select va giu phim Control de chon cung luc nhieu icon
                    SelectVertex(args.VertexControl);//Lam noi bat or lam chim icon duoc chon (khi ket hop Control chon cung luc nhieu icon)
            }

            if (args.MouseArgs.RightButton == System.Windows.Input.MouseButtonState.Pressed)//Chon bang chuot Right
            {
                //Tao 1 menu chuột phải để chọn
                vertexSelected = (DataVertex)args.VertexControl.Vertex;//Lưu Vertex được chọn vào 2 biến,
                vertexBefore = (DataVertex)args.VertexControl.Vertex;// để sau nếu có thay đổi tham số nào thì có thể lấy cái mới(selected) update vào cái cũ(before)
                args.VertexControl.ContextMenu = new System.Windows.Controls.ContextMenu();//Tạo mới 1 menu chuột phải
                var miVertex = new MenuItem { Header = "Параметры", Tag = args.VertexControl };//Tạo mới 1 MenuItem
                miVertex.Click += MiVertex_Click;//call method -> tạo mới 1 windowParaVertex 
                args.VertexControl.ContextMenu.Items.Add(miVertex);//Thêm vào 1 item
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        #region SafeRemoveVertex - Xóa Vertex được chọn
        // Метод для удаления элемента из сети передачи данных
        private void SafeRemoveVertex(VertexControl vc)
        {
            //remove vertex and all adjacent edges from layout and data graph
            graphArea.RemoveVertexAndEdges(vc.Vertex as DataVertex);
        }
        #endregion

        #region MiVertex_Click - Sự kiện sau chuột phải vào Vertex và nhấn chọn mở windowParaVertex mới
        // Обработчик события выбора меню "Параметр" элемента
        private void MiVertex_Click(object sender, RoutedEventArgs e)
        {
            windowParaVertex frmVertex = new windowParaVertex();//Tạo mới một window
            frmVertex.SetValueControl(vertexSelected);//Thể hiện các giá trị của Vertex được chọn lên của sổ
            frmVertex.Closed += FrmVertex_Closed;
            frmVertex.Show();
        }

        #region FrmVertex_Closed - Sự kiện đóng của sổ chuột phải Vertex thì lưu lại giá trị mới
        // Обработчик события закрытия окна настройки параметров элемента
        private void FrmVertex_Closed(object sender, EventArgs e)
        {
            GetPameterVertex(vertexSelected);
        }

        #region GetPameterVertex - Method xử lý Sự kiện đóng của sổ chuột phải Vertex thì lưu lại giá trị mới
        // Получение параметров элемента из окна настройки 
        private void GetPameterVertex(DataVertex vertex)
        {
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)//Duyệt qua tất cả Vertex 
            {
                if (vtx.Text == vertexBefore.Text)//So sánh với chính nó các giá trị củ và update các giá trị mới
                {
                    vtx.Text = vertex.Text;
                    vtx.Traffic = vertex.Traffic;
                }
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");//Save or Update lại toạn bộ giá trị của các Element
            graphArea.StateStorage.LoadState("exampleState");//Load lại trạng thái/giá trị của các Element ra màn hình
        }
        #endregion
        #endregion
        #endregion
        #endregion        

        #region CreateEdgeControl - Method tạo Edge mới
        // Метод для создания канала сети передачи данных       
        private void CreateEdgeControl(VertexControl vc)
        {
            if (_ecFrom == null)
            {
                _editorManager.CreateVirtualEdge(vc, vc.GetPosition());
                _ecFrom = vc;
                HighlightBehaviour.SetHighlighted(_ecFrom, true);
                return;
            }
            if (_ecFrom == vc) return;
            var data = new DataEdge();
            switch (_edgetype)
            {
                case EdgeType.AMArc:
                    data = new DataEdge((DataVertex) _ecFrom.Vertex, (DataVertex) vc.Vertex, 1, "Orange"); //{ ArcType = new ArcClass()};
                    //data.Color = "Orange";
                    break;
                case EdgeType.AMDirection:
                    data = new DataEdge((DataVertex) _ecFrom.Vertex, (DataVertex) vc.Vertex, 1, "Red"); //{ DirectionType = new DirectionClass()};
                    //data.Color = "Red";
                    break;
                default:
                    MessageBox.Show("k tim thay edge!");
                    break;
            }            
            var ec = new EdgeControl(_ecFrom, vc, data);
            graphArea.InsertEdgeAndData(data, ec, 0, true);//Chèn mới 1 Edge tại vị trí nhất định và thêm data Edge vào CSDL
            HighlightBehaviour.SetHighlighted(_ecFrom, false);
            _ecFrom = null;
            _editorManager.DestroyVirtualEdge();
        }
        #endregion

        #region graphArea_EdgeSelected - Sự kiện khi 1 Edge được chọn bằng chuột trái hoặc phải
        // Обработчик события выбора канала
        void graphArea_EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            HighlightBehaviour.SetHighlighted(args.EdgeControl, true);
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && _opMode == EditorOperationMode.Delete)//Chuột trái và mode Delete
                graphArea.RemoveEdge(args.EdgeControl.Edge as DataEdge, true);//Call method remove
            if (args.MouseArgs.RightButton == System.Windows.Input.MouseButtonState.Pressed)//Chuột phải thì gọi menu chuột phải
            {
                edgeSelected = (DataEdge)args.EdgeControl.Edge;//Lưu Edge được chọn
                args.EdgeControl.ContextMenu = new System.Windows.Controls.ContextMenu();//tạo mới 1 menu chuột phải
                var miEdge = new System.Windows.Controls.MenuItem() { Header = "Параметры", Tag = args.EdgeControl };//Tạo mói 1 MenuItem
                miEdge.Click += MiEdge_Click;//call method -> tạo mới 1 windowParaEdge
                args.EdgeControl.ContextMenu.Items.Add(miEdge);
                args.EdgeControl.ContextMenu.IsOpen = true;
            }
        }

        #region MiEdge_Click - Sự kiện sau chuột phải vào Edge và nhấn chọn mở windowParaEdge mới
        // Обработчик события выбора меню "Параметр" канала
        private void MiEdge_Click(object sender, RoutedEventArgs e)
        {
            windowParaEdge frmChanel = new windowParaEdge();//Tạo mới một window
            frmChanel.SetValueControl(edgeSelected);//Thể hiện các giá trị của Edge được chọn lên của sổ
            frmChanel.Closed += FrmChanel_Closed;
            frmChanel.Show();
        }

        #region FrmChanel_Closed - Sự kiện khi đóng cửa sổ chuột phải Edge thì lưu giá trị mới
        // Обработчик события закрытия окна настройки параметров канала
        private void FrmChanel_Closed(object sender, EventArgs e)
        {
            GetParameterEdge(edgeSelected);
        }

        #region GetParameterEdge - Method xử lý Sự kiện đóng của sổ chuột phải Edge thì lưu lại giá trị mới
        // Метод для получения и обновления параметров канала из окна настройки
        public void GetParameterEdge(DataEdge edge)
        {
            foreach (DataEdge edg in graphArea.LogicCore.Graph.Edges)//Duyệt qua tất cả Edge
            {//So sánh với chính nó các giá trị cũ và update giá trị mới
                if ((edg.Source == edge.Target && edg.Target == edge.Source) || (edg.Source == edge.Source && edg.Target == edge.Target))
                {
                    edg.Capacity = edge.Capacity;
                    edg.Weight = edge.Weight;
                }
            }
            graphArea.StateStorage.SaveOrUpdateState("exampleState", "My example state");//Save or Update lại toạn bộ giá trị của các Element
            graphArea.StateStorage.LoadState("exampleState");//Load lại trạng thái/giá trị của các Element ra màn hình
        }
        #endregion
        #endregion
        #endregion
        #endregion

        #region SelectVertex - Method làm nổi bật or làm chìm Element được chọn (khi kết hợp Control chọn cùng lúc nhiều Element)
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
        #endregion


        #region GG_Loaded - Sự kiện Load lại App
        void GG_Loaded(object sender, RoutedEventArgs e)
        {
            GG_RegisterCommands();
        }

        #region Commands
        //Các lệnh Commands
        void GG_RegisterCommands()
        {//CommandBinding constructor : 1 object ICommand, 1 method thực thi, 1 method xác thực cho thực thi
            gg_saveState.Command = SaveStateCommand;//Khi nhấn nút Save trong khung Xuất, Nhập, In
            CommandBindings.Add(new CommandBinding(SaveStateCommand, SaveStateCommandExecute, SaveStateCommandCanExecute));
            CommandBindings.Add(new CommandBinding(LoadStateCommand, LoadStateCommandExecute, LoadStateCommandCanExecute));
            gg_loadState.Command = LoadStateCommand;//Khi nhấn nút Load trong khung Xuất, Nhập, In

            CommandBindings.Add(new CommandBinding(SaveLayoutCommand, SaveLayoutCommandExecute, SaveLayoutCommandCanExecute));
            GgSaveLayout.Command = SaveLayoutCommand;//Khi nhấn nút Save trong khung File
            btnSaveGA.Command = SaveLayoutCommand;//Khi nhấn nút Save bên tab 3
            CommandBindings.Add(new CommandBinding(LoadLayoutCommand, LoadLayoutCommandExecute, LoadLayoutCommandCanExecute));
            GgLoadLayout.Command = LoadLayoutCommand;//Khi nhấn nút Open trong khung File

        }

        // Сохранение данных элементов в память//Lưu data Element vào bộ nhớ
        #region SaveStateCommand
        private static readonly RoutedCommand SaveStateCommand = new RoutedCommand();
        private void SaveStateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = graphArea.LogicCore.Graph != null && graphArea.VertexList.Count > 0;//Khi có Element trên màn hình thì có thể Save
        }

        private void SaveStateCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (graphArea.StateStorage.ContainsState("exampleState"))//Kiểm tra xem có chứa ID cho trước k
                graphArea.StateStorage.RemoveState("exampleState");//Đúng thì xóa ID đó
            graphArea.StateStorage.SaveState("exampleState", "My example state");//Sau đó Lưu ID hiện tại vào danh sách
        }
        #endregion
        // Получение состояния сети из памяти //Lấy data/trạng thái of mạng/hệ thống từ bộ nhớ
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
        // Сохранение схемы компоновки элементов//Lưu sơ đồ bố trí các Element vào file XML
        #region SaveLayoutCommand
        private static readonly RoutedCommand SaveLayoutCommand = new RoutedCommand();
        private void SaveLayoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)//Method xác thực cho thực thi
        {
            e.CanExecute = graphArea.LogicCore.Graph != null && graphArea.VertexList.Count > 0;
        }

        private void SaveLayoutCommandExecute(object sender, ExecutedRoutedEventArgs e)//Method thực thi
        {
            foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)//Duyệt qua all Vertex trong danh sách Vertex 
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
        // Получение схемы компоновки элементов//Mở sơ đồ/file có sẵn
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

        #region GGRelayoutCommand

        private bool GGRelayoutCommandCanExecute(object sender)
        {
            return true;
        }

        #endregion

        #endregion
        #endregion


        // Объявления делегатов для включения асинхронного вызова к установке свойств элементов управления

        #region MyRegion
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
                UpdateGraph(graphArea, ListVertex, chromosome);
            }
            else
            {
                SetGraphColor d = new SetGraphColor(SetColor);
                Dispatcher.Invoke(d, new object[] { graph, chromosome });
            }
        }


        #endregion
        private delegate void UpdatewindowResults(windowResults windowResult, string chromosome, int iteration, double alpha);
        private void UpdateResults(windowResults windowResult, string chromosome, int iteration, double alpha)
        {
            string str = "Итерация № " + iteration.ToString();
            if (Dispatcher.CheckAccess())//Xác định xem tiểu trình đang gọi là gắn liền với Dispatcher này.
            {//Dispatcher - Provides services for managing the queue of work items for a thread.
                windowResult.progressBar.Value = iteration;
                windowResult.ListBestChromosome.Items.Add(str);
                windowResult.ListBestChromosome.Items.Add(chromosome);
                windowResult.UpdateLineSeries(iteration, alpha);
            }
            else
            {
                UpdatewindowResults d = new UpdatewindowResults(UpdateResults);
                Dispatcher.Invoke(d, new object[] { windowResult, chromosome, iteration, alpha });
            }
        }



        #region ResetGraph_Click - Sự kiện nhấn nút Clear
        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
            RefreshGraph();
        }

        #region RefreshGraph - Method đưa màu sắc Edge trở lại mặc định
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
        #endregion
        #endregion

        #region gg_saveAsPngImage_Click - Sự kiện nhấn nút Save dạng PNG
        // Сохрание схемы компоновки элементов сети в виде изображения 
        private void gg_saveAsPngImage_Click(object sender, RoutedEventArgs e)
        {
            graphArea.ExportAsImageDialog(ImageType.PNG, true, 96D, 100);
        }
        #endregion

        #region gg_printlay_Click - Sự kiện nhấn nút In
        // Печать схему компоновки элементов сети передачи данных
        private void gg_printlay_Click(object sender, RoutedEventArgs e)
        {
            graphArea.PrintDialog("Печать");
        }
        #endregion

        #region BtnNew_Click - Sự kiện tạo dự án mới
        // Создание нового проекта
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить файл ?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
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
                    //graphArea.LogicCore.Graph.Clear();
                    graphArea.LogicCore.Graph?.Clear();
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
        #endregion

        #region TabControl_SelectionChanged - Sự kiện thay đổi tab
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
                    cbxRoot.SelectedIndex = cbxRoot.Items.Count - 1;
                    cbxGoal.SelectedIndex = 0;
                }
                if (TabControl.SelectedItem == TabGA)
                {
                    cbxCenter.Items.Clear();
                    foreach (DataVertex vtx in graphArea.LogicCore.Graph.Vertices)
                    {
                        if (vtx.TypeOfVertex == "AMCreate")
                            cbxCenter.Items.Add(vtx.Text);

                    }
                    cbxCenter.SelectedIndex = 0;
                }
            }

        }
        #endregion

        #region btnFindPath_Click - Sự kiện nhấn nút Tìm kiếm
        // Обработчик события нажатия на кнопке "Поиск"
        private void btnFindPath_Click(object sender, RoutedEventArgs e)
        {

            bool flaggoal = false;
            bool flagroot = false;
            int iPathCount;
            string goalID = null, rootID = null;
            PathEnum = null;
            PathList.Items.Clear();//Xóa sạch các items cũ
            PathList.Items.Add("Список маршрутов:");//Thêm vào dòng mới
            iPathCount = int.Parse(_tBxPathCount.Text);//Truyền vào tham số từ giao diện và lưu vào biến iPathCount
            try
            {
                goalID = cbxGoal.SelectedItem.ToString();//Truyền data
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Назначение не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                rootID = cbxRoot.SelectedItem.ToString();//Truyền data
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Источник не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Duyệt qua toàn bộ danh sách các Vertex 
            foreach (DataVertex vt in graphArea.LogicCore.Graph.Vertices)
            {
                if (vt.Text == goalID)
                {
                    flaggoal = true;//Nếu có Vertex trùng khớp thì set true
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

            //Khi flaggoal=true && flagroot=true thì tiến hành thực hiện theo algorithm được chọn
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
                var Yen = new YenAlgorithm(graphArea.LogicCore.Graph, root, vertexToFind, iPathCount);
                PathEnum = Yen.Execute();
                if (PathEnum == null) PathList.Items.Add("Маршрут не найден !");
                else
                {
                    foreach (IEnumerable<DataEdge> path in PathEnum)
                    {
                        PathList.Items.Add(PathToString(path));
                    }
                }
            }

        }

        #region Method nối tên các Vertex từ root tới goal
        // Метод для получения названия маршрута
        private string PathToString(IEnumerable<DataEdge> NameOfPath)
        {
            string path = "";
            foreach (DataEdge edge in NameOfPath)
            {
                path = edge.Source.ToString();
                break;
            }
            foreach (DataEdge edge in NameOfPath)
                path = path + "->" + edge.Target.ToString();
            return path;
        }
        #endregion
        #endregion
       
        #region startButton_Click_1 - Sự kiện nhấn nút Start
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
            UpdateSettings();// Thể hiện giá trị mới lên màn hình
            selectionMethod = selectionBox.SelectedIndex;//Thiết lập method được chọn
            hybridAlgorithm = cbxHybridAlgorithm.SelectedIndex;//Thiết lập algorit được chọn
            // Генерация данных о возможных вариантах построения сети
            string center = null;
            try
            {
                center = cbxCenter.SelectedItem.ToString();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Генератор не существует !", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region Method làm mới lại các thể hiện trong khung 
        // Обновление элемента формы
        private void UpdateSettings()
        {
            populationSizeBox.Text = populationSize.ToString();
            iterationsBox.Text = iterations.ToString();
        }
        #endregion

        #region Method tìm tất cả các đường từ IP về AMCreate đã chọn
        // Метод для поиска всех виртуальных каналов для измерителных пунктов
        private void FindAllPath(string Center, int hybridAlgorithm)
        {
            IPCout = 0;
            bool flag = false;
            maxValuePath = int.Parse(tbxMaxValue.Text);
            _allRoute = null;
            var goal = new DataVertex();
            // поиск Генератор в графе - Tìm AMCreate trùng với AMCreate trong combobox
            VertexStore = graphArea.LogicCore.Graph.Vertices;
            ListVertex = graphArea.LogicCore.Graph.Vertices.ToList();
            foreach (DataVertex vt in ListVertex)
            {
                if (vt.Text == Center)//so sánh xem trùng k, trùng thì thoát
                {
                    goal = vt;
                    flag = true;
                    break;
                }
            }
            if (flag != true) MessageBox.Show("Назначение не найден", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            // поиск ИП и все маршрут из этого ИП в Генератор - tìm IP và tất cả các đường từ IP đến AMCreate ở trên
            foreach (DataVertex vt in ListVertex)
                if (vt.TypeOfVertex == "IP")
                {
                    IPCout++;
                    FindPath(vt, goal, maxValuePath, ref _allRoute, hybridAlgorithm);//Gọi method FindPath với vt là từng IP trong danh sách các Vertex, goal - AMCreate được chọn, maxValuePath -tham số, _allRoute - , hybridA - Algorithm được chọn
                    vt.ListPath = _allRoute;//gán tâp hợp các Edge tạo thành đường ngắn nhất cho từng IP
                }

            ChannelCout = graphArea.LogicCore.Graph.EdgeCount;
            ElementCout = graphArea.LogicCore.Graph.VertexCount;
            EdgeStore = graphArea.LogicCore.Graph.Edges;
        }

        #region FindPath - Method trả ra tâp hợp các Edge tạo thành đường ngắn nhất
        // Метод для поиска маршрутов для определенного источника и сохранения результата в двумерный массив
        private void FindPath(DataVertex root, DataVertex goal, int numberOfPath, ref List<IEnumerable<DataEdge>> allpath, int hybridAlgorithm)
        {
            bool flaggoal = false;
            bool flagroot = false;
            foreach (DataVertex vt in graphArea.LogicCore.Graph.Vertices)
            {
                if (vt.Text == goal.Text)//so sánh với tên AMCreate truyền vào, nếu dúng set 2 cờ true, sai - xuất thông báo
                {
                    flaggoal = true;
                }
                if (vt.Text == root.Text)
                {
                    flagroot = true;
                }
            }
            if (flaggoal != true) MessageBox.Show("Не правильно задавать параметр в поле: Назначение", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (flagroot != true) MessageBox.Show("Не правильно задавать параметр в поле: Источник", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //Khi số Vertex khác 0 và 2 cờ đều true, băt đầu chọn Algorithm để tính toán
            if (graphArea.LogicCore.Graph.VertexCount != 0 && flaggoal == true && flagroot == true)
            {
                if (hybridAlgorithm == 0)
                {
                    Func<DataEdge, double> edgeWeights = E => E.Weight;
                    var rank = new HoffmanPavleyRankedShortestPathAlgorithm<DataVertex, DataEdge>(graphArea.LogicCore.Graph, edgeWeights);
                    rank.ShortestPathCount = numberOfPath;
                    rank.SetRootVertex(root);
                    rank.Compute(root, goal);
                    allpath = rank.ComputedShortestPaths.ToList();//Method trả ra tâp hợp các Edge tạo thành đường ngắn nhất
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
        #endregion
        #endregion



        #endregion

        #region Events trên của sổ Result 
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
            windowResult.TabDiagram.IsSelected = true;
            btnSaveGA.IsEnabled = true;
            UpdateGraph(graphArea, ListVertex, bestchromosome);
            if (overLoad == true) MessageBox.Show("Сеть перегружена !", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            windowResult.ListBestRoutes.Items.Clear();
            for (int i = 0; i < bestchromosome.Length; i++)
            {
                string nameIP = "ИП " + (i + 1).ToString();
                foreach (DataVertex vtx in ListVertex)
                    if (vtx.Text == nameIP)
                    {
                        windowResult.ListBestRoutes.Items.Add(PathToString(vtx.ListPath[bestchromosome[i]]));
                    }
            }

        }

        #region Sự kiện bắt buộc dừng khi nhấn Stop
        // Обработчик события нажатия на кнопке "Стоп" в окне windowResult
        private void windowResult_btnStop_click(object sender, RoutedEventArgs e)
        {
            if (workerThread != null)
            {
                needToStop = true;
                workerThread.Join(100);//Join - method khóa luồng hiện tại tới khi đối tượng luồng chấm dứt hoạt động
                workerThread = null;
            }
        }
        #endregion
        #endregion        

        #region cbxHybridAlgorithm_SelectionChanged - Sự kiện chọn thuật toán để giải
        private void cbxHybridAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (cbxHybridAlgorithm.SelectedIndex == 2) tbxMaxValue.IsEnabled = false;
                else tbxMaxValue.IsEnabled = true;
            }

        }
        #endregion

        #region ListViewItem_PreviewMouseLeftButtonDown - Sự kiện xem trước chuột trái thả xuống(làm nỗi bật các đường được chọn theo kết quả sau khi nhấn nút Tìm kiếm)
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
        #endregion        



        #region SearchSolution- Method tìm kết quả tối ưu với sự hỗ trợ của Giải thuật di truyền
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
                if (IPCout == 1) str = (bestValue[0] + 1).ToString();
                else
                    for (int j = 0; j < bestValue.Length; j++)
                    {
                        str += (bestValue[j] + 1).ToString();
                    }
                double alpha = Math.Round(fitnessFunction.Alpha(population.BestChromosome), 2);
                overLoad = (alpha == 1) ? true : false;
                UpdateResults(windowResult, str, i, alpha);
                i++;
                if ((iterations != 0) && (i > iterations))
                    break;
            }
        }
        #endregion

        #region UpdateGraph - Method làm thay đổi màu sắc Edge
        //Метод для изменения цвета канала 
        private void UpdateGraph(GraphAreaExample graphArea, List<DataVertex> ListVertex, ushort[] path)
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
        #endregion       

        #region Dispose - Method gỡ bỏ và đặt lại các tài nguyên không được quản lý
        // Метод для удаления и сброса неуправляемых ресурсов
        public void Dispose()
        {
            if (_editorManager != null)
                _editorManager.Dispose();
            if (graphArea != null)
                graphArea.Dispose();
        }
        #endregion



        #region khong dung
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
        private AdjacencyGraph<DataVertex, TaggedEquatableEdge<DataVertex, double>> ToAdjacencyGraph(BidirectionalGraph<DataVertex, DataEdge> BiGraph)
        {
            var adjGraph = new AdjacencyGraph<DataVertex, TaggedEquatableEdge<DataVertex, double>>();
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


        #endregion

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            //var create = new CreateClass();
            //Task generator = new Process(create, create.Generator);//Khai báo 1 nhiệm vụ
            //create.Run(generator);
            ////var amresult = new AMResult(create.Points);
            ////amresult.Show();
            ////Console.ReadKey();
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            //var amresult = new AMResult();
            //amresult.Show();
            var vertexf = new DataVertex();
            var aa = vertexf.CreateType;
        }
    }
}



