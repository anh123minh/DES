using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimulationV1.WPF.ExampleModels;
namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AMResultTransitions.xaml
    /// </summary>
    public partial class AMResultTransitions : Window
    {
        private Transition Transition;
        public DataVertex[] ArrayTransitions;
        private DataVertex _vertexTransition;



        public AMResultTransitions(DataVertex[] arrayTransitions)
        {
            InitializeComponent();
            ArrayTransitions = arrayTransitions;
            CbbTransitions.ItemsSource = ArrayTransitions;
        }
        public AMResultTransitions(Transition transition)
        {
            InitializeComponent();
            Transition = transition;
            ArrayTransitions = transition.ArrayTransitions1;
            CbbTransitions.ItemsSource = ArrayTransitions;

            txtTime.Text = transition.LastTime1.ToString();
            txtSumfire.Text = transition.Count.ToString();
        }
        private void btnPlacesIn_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = _vertexTransition.ListTimeNow;
            var pttb1 = _vertexTransition.ListTimePlaceIn;
            var asm = new AMMultiChart(tntb1, pttb1);
            asm.Show();
            //btnPlacesIn.IsEnabled = false;
            //btnPlacesOut.IsEnabled = true;
            //btnPlacesCommon.IsEnabled = true;
        }

        private void btnPlacesOut_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = _vertexTransition.ListTimeNow;
            var pttb1 = _vertexTransition.ListTimePlaceOut;
            var asm = new AMMultiChart(tntb1, pttb1);
            asm.Show();
            //btnPlacesIn.IsEnabled = true;
            //btnPlacesOut.IsEnabled = false;
            //btnPlacesCommon.IsEnabled = true;
        }

        private void btnPlacesCommon_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = _vertexTransition.ListTimeNow;
            var pttb1 = _vertexTransition.ListTimePlaceIn;
            var pttb2 = _vertexTransition.ListTimePlaceOut;
            var asm = new AMMultiChart(tntb1, pttb1, pttb2);
            asm.Show();
            //btnPlacesIn.IsEnabled = true;
            //btnPlacesOut.IsEnabled = true;
            //btnPlacesCommon.IsEnabled = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vertexTransition = (DataVertex)CbbTransitions.SelectedItem;
        }
       
        private void BtnCommon_Click(object sender, RoutedEventArgs e)
        {
            Pies.Visibility = Visibility.Collapsed;
            Analis.Visibility = Visibility.Visible;
            BtnPies.IsEnabled = true;
            BtnCommon.IsEnabled = false;            
        }

        private void BtnPies_Click(object sender, RoutedEventArgs e)
        {
            Pies.Visibility = Visibility.Visible;
            Analis.Visibility = Visibility.Collapsed;
            BtnPies.IsEnabled = false;
            BtnCommon.IsEnabled = true;
        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = Transition.ListTimeNowTable1;
            var pttb1 = Transition.PhantichTable1;
            var asm = new AMMultiChart(tntb1, pttb1);
            asm.Show();
        }
    }
}



//namespace SimulationV1.WPF.Pages
//{
//    /// <summary>
//    /// Interaction logic for AMResultTransitions.xaml
//    /// </summary>
//    public partial class AMResultTransitions : Window
//    {
//        private Transition Transition;
//        public DataVertex[] ArrayTransitions;
//        private DataVertex _vertexTransition;

//        public ChartData ChartData { get; set; }
//        public List<ChartData> ChartDataList { get; set; }

//        public AMResultTransitions(DataVertex[] arrayTransitions)
//        {
//            InitializeComponent();
//            ArrayTransitions = arrayTransitions;
//            CbbTransitions.ItemsSource = ArrayTransitions;
//        }
//        public AMResultTransitions(Transition transition)
//        {
//            InitializeComponent();
//            Transition = transition;
//            ArrayTransitions = transition.ArrayTransitions1;
//            CbbTransitions.ItemsSource = ArrayTransitions;

//            BtnCommon_Click(null, null);
//        }
//        private void btnPlacesIn_Click(object sender, RoutedEventArgs e)
//        {
//            var tntb1 = _vertexTransition.ListTimeNow;
//            var pttb1 = _vertexTransition.ListTimePlaceIn;
//            var asm = new AMMultiChart(tntb1, pttb1);
//            asm.Show();
//            btnPlacesIn.IsEnabled = false;
//            btnPlacesOut.IsEnabled = true;
//            btnPlacesCommon.IsEnabled = true;
//            btnMultiGraph_Click(null, null);
//        }

//        private void btnPlacesOut_Click(object sender, RoutedEventArgs e)
//        {
//            var tntb1 = _vertexTransition.ListTimeNow;
//            var pttb1 = _vertexTransition.ListTimePlaceOut;
//            var asm = new AMMultiChart(tntb1, pttb1);
//            asm.Show();
//            btnPlacesIn.IsEnabled = true;
//            btnPlacesOut.IsEnabled = false;
//            btnPlacesCommon.IsEnabled = true;
//            btnMultiGraph_Click(null, null);
//        }

//        private void btnPlacesCommon_Click(object sender, RoutedEventArgs e)
//        {
//            var tntb1 = _vertexTransition.ListTimeNow;
//            var pttb1 = _vertexTransition.ListTimePlaceIn;
//            var pttb2 = _vertexTransition.ListTimePlaceOut;
//            var asm = new AMMultiChart(tntb1, pttb1,pttb2);
//            asm.Show();
//            btnPlacesIn.IsEnabled = true;
//            btnPlacesOut.IsEnabled = true;
//            btnPlacesCommon.IsEnabled = false;
//            btnMultiGraph_Click(null, null);
//        }

//        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            _vertexTransition = (DataVertex) CbbTransitions.SelectedItem;
//        }
//        private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
//        {
//            Graph.Visibility = Visibility.Visible;
//            TableData.Visibility = Visibility.Collapsed;

//            List<int> listtimenowtable;
//            List<List<int>> listblockcounttable = new List<List<int>>();
//            if (!BtnCommon.IsEnabled)
//            {
//                listtimenowtable = Transition.ListTimeNowTable1;
//                listblockcounttable = Transition.PhantichTable1;
//            }
//            else
//            {
//                listtimenowtable = _vertexTransition.ListTimeNow;
//                if (!btnPlacesIn.IsEnabled)
//                {
//                    listblockcounttable = _vertexTransition.ListTimePlaceIn;
//                }
//                else if (!btnPlacesOut.IsEnabled)
//                {
//                    listblockcounttable = _vertexTransition.ListTimePlaceOut;
//                }
//                else
//                {
//                    for (int i = 0; i < _vertexTransition.ListTimePlaceIn.Count; i++)
//                    {
//                        var listtemp = new List<int>();
//                        for (int j = 0; j < _vertexTransition.ListTimePlaceIn[0].Count; j++)
//                        {
//                            listtemp.Add(_vertexTransition.ListTimePlaceIn[i][j]);
//                        }
//                        for (int j = 0; j < _vertexTransition.ListTimePlaceOut[0].Count; j++)
//                        {
//                            listtemp.Add(_vertexTransition.ListTimePlaceOut[i][j]);
//                        }
//                        listblockcounttable.Add(listtemp);
//                    }
//                }

//            }

//            ChartData = new ChartData();
//            ChartData.Title = "Количество разметки в системе";
//            ChartData.DataSeriesList = new List<ChartDataSerie>();

//            for (int i = 0; i < listblockcounttable[0].Count; i++)
//            {
//                var dataSeries = new Dictionary<int, int>();
//                for (int j = 0; j < listtimenowtable.Count; j++)
//                {
//                    dataSeries.Add(listtimenowtable[j], listblockcounttable[j][i]);
//                }
//                ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
//            }
//            //quan trong mo rong
//            //for (int i = 0; i < listblockcounttable[0].Count; i++)
//            //{
//            //    var dataSeries = new List<ChartPoint>();
//            //    for (int j = 0; j < listtimenowtable.Count; j++)
//            //    {
//            //        dataSeries.Add(new ChartPoint(){ Time = listtimenowtable[j], Value = listblockcounttable[j][i]});
//            //    }
//            //    ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
//            //}

//            ChartDataList = new List<ChartData>();
//            ChartDataList.Add(ChartData);
//            DataTableAndGrap.Visibility = Visibility.Visible;
//            Graph.Visibility = Visibility.Visible;

//            this.DataContext = this;
//        }

//        private void btnTable_Click(object sender, RoutedEventArgs e)
//        {
//            Graph.Visibility = Visibility.Collapsed;
//            TableData.Visibility = Visibility.Visible;
//            List<int> listtimenowtable;
//            List<List<int>> listblockcounttable = new List<List<int>>();
//            if (!BtnCommon.IsEnabled)
//            {
//                listtimenowtable = Transition.ListTimeNowTable1;
//                listblockcounttable = Transition.PhantichTable1;
//            }
//            else
//            {
//                listtimenowtable = _vertexTransition.ListTimeNow;
//                if (!btnPlacesIn.IsEnabled)
//                {
//                    listblockcounttable = _vertexTransition.ListTimePlaceIn;
//                }
//                else if (!btnPlacesOut.IsEnabled)
//                {
//                    listblockcounttable = _vertexTransition.ListTimePlaceOut;
//                }
//                else
//                {
//                    for (int i = 0; i < _vertexTransition.ListTimePlaceIn.Count; i++)
//                    {
//                        var listtemp = new List<int>();
//                        for (int j = 0; j < _vertexTransition.ListTimePlaceIn[0].Count; j++)
//                        {
//                            listtemp.Add(_vertexTransition.ListTimePlaceIn[i][j]);
//                        }
//                        for (int j = 0; j < _vertexTransition.ListTimePlaceOut[0].Count; j++)
//                        {
//                            listtemp.Add(_vertexTransition.ListTimePlaceOut[i][j]);
//                        }
//                        listblockcounttable.Add(listtemp);
//                    }
//                }

//            }

//            var dara = new List<List<int>>();
//            dara.Add(listtimenowtable);

//            for (int i = 0; i < listblockcounttable[0].Count; i++)
//            {
//                var temp = new List<int>();
//                for (int j = 0; j < listblockcounttable.Count; j++)
//                {
//                    temp.Add(listblockcounttable[j][i]);
//                }
//                dara.Add(temp);
//            }

//            var result = Cot2Hang(dara);
//            var dataTable = new DataTable();
//            for (int i = 0; i < result[0].Count; i++)
//            {
//                var cl = new DataColumn($"column {i}", typeof(int));
//                dataTable.Columns.Add(cl);
//            }
//            for (int i = 0; i < result.Count; i++)
//            {
//                var row = dataTable.NewRow();
//                for (int j = 0; j < dataTable.Columns.Count; j++)
//                {
//                    row[j] = result[i][j];
//                }

//                dataTable.Rows.Add(row);
//            }

//            TableData.ItemsSource = dataTable.DefaultView;
//        }

//        List<List<int>> Cot2Hang(List<List<int>> list)
//        {

//            var sodang = list.Count;
//            var socot = list.FirstOrDefault().Count;
//            var list1 = new List<List<int>>();
//            for (int i = 0; i < socot; i++)
//            {
//                var lis = new List<int>();
//                for (int j = 0; j < sodang; j++)
//                {
//                    lis.Add(list[j][i]);
//                }
//                list1.Add(lis);
//            }
//            return list1;
//        }
//        private void BtnCommon_Click(object sender, RoutedEventArgs e)
//        {
//            Pies.Visibility = Visibility.Collapsed;
//            BtnPies.IsEnabled = true;
//            BtnCommon.IsEnabled = false;
//            btnMultiGraph_Click(null, null);
//        }

//        private void BtnPies_Click(object sender, RoutedEventArgs e)
//        {
//            Pies.Visibility = Visibility.Visible;
//            BtnPies.IsEnabled = false;
//            BtnCommon.IsEnabled = true;
//            btnPlacesCommon_Click(null,null);

//        }
//    }
//}
