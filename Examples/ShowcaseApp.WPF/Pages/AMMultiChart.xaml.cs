using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AMMultiChart.xaml
    /// </summary>
    public partial class AMMultiChart : Window
    {
        public ChartData ChartData { get; set; }
        public List<ChartData> ChartDataList { get; set; }
        private List<int> listtimenowgraph;
        private List<List<int>> listblockcountgraph;
        private readonly List<int> _listtimenowtable;
        private readonly List<List<int>> _listblockcounttable = new List<List<int>>();

        private readonly Lines _listtimenowtableline;
        private readonly List<Lines> _listblockcounttableline = new List<Lines>();
        private string _namegraph;
        public AMMultiChart(List<int> listtimenowtb, List<List<int>> listblockcounttb, List<int> listtimenowgr, List<List<int>> listblockcountgr)
        {
            _listtimenowtable = listtimenowtb;
            _listblockcounttable = listblockcounttb;

            listtimenowgraph = listtimenowgr;
            listblockcountgraph = listblockcountgr;           

            InitializeComponent();
            btnMultiGraph_Click(null, null);
        }

        public AMMultiChart(List<int> listtimenowtb, List<List<int>> listblockcounttb)
        {
            _listtimenowtable = listtimenowtb;
            _listblockcounttable = listblockcounttb;

            InitializeComponent();
            btnMultiGraph_Click(null, null);
        }

        public AMMultiChart(List<int> listtimenowtb, List<List<int>> listblockcounttb1, List<List<int>> listblockcounttb2)
        {
            _listtimenowtable = listtimenowtb;
            for (int i = 0; i < listblockcounttb1.Count; i++)
            {
                var listtemp = new List<int>();
                for (int j = 0; j < listblockcounttb1[0].Count; j++)
                {
                    listtemp.Add(listblockcounttb1[i][j]);
                }
                for (int j = 0; j < listblockcounttb2[0].Count; j++)
                {
                    listtemp.Add(listblockcounttb2[i][j]);
                }
                _listblockcounttable.Add(listtemp);
            }
            InitializeComponent();
            btnMultiGraph_Click(null, null);
        }

        public AMMultiChart(Lines listtimenowtbline, List<Lines> listblockcounttbline, string namegraph)
        {
            _listtimenowtableline = listtimenowtbline;
            _listblockcounttableline = listblockcounttbline;
            _namegraph = namegraph;
            InitializeComponent();
            btnMultiGraph_Click(null, null);
        }
        public AMMultiChart(Lines listtimenowtbline, List<Lines> listblockcounttblinein, List<Lines> listblockcounttblineout, string namegraph)
        {
            _namegraph = namegraph;
            _listtimenowtableline = listtimenowtbline;
            foreach (Lines t in listblockcounttblinein)
            {
                _listblockcounttableline.Add(t);
            }
            foreach (Lines t in listblockcounttblineout)
            {
                _listblockcounttableline.Add(t);
            }
            InitializeComponent();
            btnMultiGraph_Click(null, null);
        }
        private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
        {
            TableData.Visibility = Visibility.Collapsed;
            btnTable.IsEnabled = true;
            Graph.Visibility = Visibility.Visible;
            btnMultiGraph.IsEnabled = false;

            ChartData = new ChartData();
            ChartData.Title = "Количество разметки " + _namegraph;
            ChartData.DataSeriesList = new List<ChartDataSerie>();

            if (_listblockcounttableline.Count != 0)
            {
                for (int i = 0; i < _listblockcounttableline.Count; i++)
                {
                    var dataSeries = new Dictionary<int, int>();
                    for (int j = 0; j < _listblockcounttableline[0].LineData.Count; j++)
                    {
                        dataSeries.Add(_listtimenowtableline.LineData[j], _listblockcounttableline[i].LineData[j]);
                    }
                    ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = _listblockcounttableline[i].LineName, Data = dataSeries });
                }
            }

            //quan trong mo rong
            //for (int i = 0; i < listblockcounttable[0].Count; i++)
            //{
            //    var dataSeries = new List<ChartPoint>();
            //    for (int j = 0; j < listtimenowtable.Count; j++)
            //    {
            //        dataSeries.Add(new ChartPoint(){ Time = listtimenowtable[j], Value = listblockcounttable[j][i]});
            //    }
            //    ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
            //}

            ChartDataList = new List<ChartData>();
            ChartDataList.Add(ChartData);

            this.DataContext = this;
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            btnMultiGraph.IsEnabled = true;
            btnTable.IsEnabled = false;
            Graph.Visibility = Visibility.Collapsed;
            TableData.Visibility = Visibility.Visible;

            var dara = new List<Lines>();
            dara.Add(_listtimenowtableline);
            foreach (var a in _listblockcounttableline)
            {
                dara.Add(a);
            }

            var dataTable = new DataTable();
            for (int i = 0; i < dara.Count; i++)
            {
                var cl = new DataColumn(dara[i].LineName, typeof(int));
                dataTable.Columns.Add(cl);
            }
            var result = Cot2Hang(dara);
            for (int i = 0; i < result.Count; i++)
            {
                var row = dataTable.NewRow();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    row[j] = result[i][j];
                }
                dataTable.Rows.Add(row);
            }
            TableData.ItemsSource = dataTable.DefaultView;
        }

        List<List<int>> Cot2Hang(List<Lines> listline)
        {

            var socot = listline.Count;
            var sohang = listline.FirstOrDefault().LineData.Count;
            var list1 = new List<List<int>>();

            for (int i = 0; i < sohang; i++)
            {
                var lis = new List<int>();
                for (int j = 0; j < socot; j++)
                {

                    lis.Add(listline[j].LineData[i]);
                }
                list1.Add(lis);
            }
            return list1;
        }

        #region ban den nay da chay
        //private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
        //{
        //    TableData.Visibility = Visibility.Collapsed;
        //    btnTable.IsEnabled = true;
        //    Graph.Visibility = Visibility.Visible;
        //    btnMultiGraph.IsEnabled = false;

        //    ChartData = new ChartData();
        //    ChartData.Title = "Количество разметки в системе";
        //    ChartData.DataSeriesList = new List<ChartDataSerie>();

        //    //for (int i = 0; i < listblockcounttable.Count; i++)
        //    //{
        //    //    var dataSeries = new Dictionary<int, int>();
        //    //    for (int j = 0; j < listtimenowtable.Count; j++)
        //    //    {
        //    //        dataSeries.Add(listtimenowtable[j], listblockcounttable[i][j]);
        //    //    }
        //    //    ChartData.DataSeriesList.Add(dataSeries);
        //    //}
        //    if (_listblockcounttable[0].Count != 0)
        //    {
        //        for (int i = 0; i < _listblockcounttable[0].Count; i++)
        //        {
        //            var dataSeries = new Dictionary<int, int>();
        //            for (int j = 0; j < _listtimenowtable.Count; j++)
        //            {
        //                dataSeries.Add(_listtimenowtable[j], _listblockcounttable[j][i]);
        //            }
        //            ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
        //        }
        //    }

        //    //quan trong mo rong
        //    //for (int i = 0; i < listblockcounttable[0].Count; i++)
        //    //{
        //    //    var dataSeries = new List<ChartPoint>();
        //    //    for (int j = 0; j < listtimenowtable.Count; j++)
        //    //    {
        //    //        dataSeries.Add(new ChartPoint(){ Time = listtimenowtable[j], Value = listblockcounttable[j][i]});
        //    //    }
        //    //    ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
        //    //}




        //    ChartDataList = new List<ChartData>();
        //    ChartDataList.Add(ChartData);

        //    this.DataContext = this;
        //}
        #endregion



        //private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
        //{
        //    Bang.Visibility = Visibility.Collapsed;
        //    //Bang.LastChildFill = false;
        //    Graph.Visibility = Visibility.Visible;
        //    //Graph.LastChildFill = true;

        //    ChartData = new ChartData();
        //    ChartData.Title = "Chart Title";
        //    ChartData.DataSeriesList = new List<List<int>>();

        //    for (int i = 0; i < listblockcountgraph.Count; i++)
        //    {
        //        var dataSeries = new List<int>();
        //        for (int j = 0; j < listtimenowgraph.Count; j++)
        //        {
        //            dataSeries.Add(listtimenowgraph[j]);
        //            dataSeries.Add(listblockcountgraph[i][j]);
        //        }
        //        ChartData.DataSeriesList.Add(dataSeries);
        //    }


        //    ChartDataList = new List<ChartData>();
        //    ChartDataList.Add(ChartData);

        //    this.DataContext = this;
        //}

        //private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
        //{
        //    Bang.Visibility = Visibility.Collapsed;
        //    //Bang.LastChildFill = false;
        //    Graph.Visibility = Visibility.Visible;
        //    //Graph.LastChildFill = true;

        //    ChartData = new ChartData();
        //    ChartData.Title = "Chart Title";
        //    ChartData.DataSeriesList = new List<Dictionary<int, int>>();

        //    for (int i = 0; i < listblockcountgraph.Count; i++)
        //    {
        //        var dataSeries = new Dictionary<int, int>();
        //        for (int j = 0; j < listtimenowgraph.Count; j++)
        //        {
        //            dataSeries.Add(listtimenowgraph[j], listblockcountgraph[i][j]);
        //        }
        //        ChartData.DataSeriesList.Add(dataSeries);
        //    }


        //    ChartDataList = new List<ChartData>();
        //    ChartDataList.Add(ChartData);

        //    this.DataContext = this;
        //}

        #region den day chay on
        //private void btnTable_Click(object sender, RoutedEventArgs e)
        //{
        //    btnMultiGraph.IsEnabled = true;
        //    btnTable.IsEnabled = false;
        //    Graph.Visibility = Visibility.Collapsed;
        //    TableData.Visibility = Visibility.Visible;

        //    var dara = new List<List<int>>();
        //    dara.Add(_listtimenowtable);

        //    for (int i = 0; i < _listblockcounttable[0].Count; i++)
        //    {
        //        var temp = new List<int>();
        //        for (int j = 0; j < _listblockcounttable.Count; j++)
        //        {
        //            temp.Add(_listblockcounttable[j][i]);
        //        }
        //        dara.Add(temp);
        //    }

        //    var result = Cot2Hang(dara);
        //    var dataTable = new DataTable();
        //    for (int i = 0; i < result[0].Count; i++)
        //    {
        //        var cl = new DataColumn($"column {i}", typeof(int));
        //        dataTable.Columns.Add(cl);
        //    }
        //    for (int i = 0; i < result.Count; i++)
        //    {
        //        var row = dataTable.NewRow();
        //        for (int j = 0; j < dataTable.Columns.Count; j++)
        //        {
        //            row[j] = result[i][j];
        //        }

        //        dataTable.Rows.Add(row);
        //    }

        //    TableData.ItemsSource = dataTable.DefaultView;


        //    //Graph.Visibility = Visibility.Collapsed;
        //    ////Graph.LastChildFill = false;
        //    //Bang.Visibility = Visibility.Visible;
        //    ////Bang.LastChildFill = true;

        //    ////var dara = new ObservableCollection<List<int>>();
        //    //var dara = new List<List<int>>();
        //    //dara.Add(listtimenowtable);
        //    //foreach (var sd in listblockcounttable)
        //    //{
        //    //    dara.Add(sd);
        //    //}
        //    //TableData.ItemsSource = Cot2Hang(dara);
        //    ////var ns = new List<List<int>>();
        //    ////var s1 = new List<int>(){1,2,3,4,5};
        //    ////var s2 = new List<int>() { 2, 4, 6, 8, 10 };
        //    ////var s3 = new List<int>() { 10, 20, 30, 40, 50 };
        //    ////ns.Add(s1);
        //    ////ns.Add(s2);
        //    ////ns.Add(s3);
        //    ////TableData.ItemsSource = s1;
        //}
        #endregion


        List<List<int>> Cot2Hang(List<List<int>> list)
        {
            
            var sodang = list.Count;
            var socot = list.FirstOrDefault().Count;
            var list1 = new List<List<int>>();

            for (int i = 0; i < socot; i++)
            {
                var lis = new List<int>();
                for (int j = 0; j < sodang; j++)
                {
                    
                    lis.Add(list[j][i]);
                    //list1[i, j] = list[j][i];
                }
                list1.Add(lis);
            }
            return list1;
        }
    }

    //public class ChartData
    //{
    //    public string Title { get; set; }
    //    public List<Dictionary<string, int>> DataSeriesList { get; set; }
    //}
    public class ChartData
    {
        public string Title { get; set; }
        public List<ChartDataSerie> DataSeriesList { get; set; }
    }

    public class ChartDataSerie
    {
        public string Name { get; set; }

        public Dictionary<int, int> Data { get; set; }
    }

    public class Lines
    {
        public string LineName { get; set; }
        public List<int> LineData { get; set; }
    }
//quan trong mo rong
    //public class ChartDataSerie
    //{
    //    public string Name { get; set; }

    //    public List<ChartPoint> Data { get; set; }
    //}

    //public class ChartPoint
    //{
    //    public int Time { get; set; }
    //    public int Value { get; set; }
    //}
    ////public class ChartData
    ////{
    ////    public string Title { get; set; }
    ////    public List<List<int>> DataSeriesList { get; set; }
    ////}
}

//var dataSeries = new Dictionary<string, int>();
//dataSeries.Add("Jan", 5);
//dataSeries.Add("Feb", 7);
//dataSeries.Add("Mar", 3);

//ChartData = new ChartData();
//ChartData.Title = "Chart Title";
//ChartData.DataSeriesList = new List<Dictionary<string, int>>();
//ChartData.DataSeriesList.Add(dataSeries);

//var dataSeries1 = new Dictionary<string, int>();
//dataSeries1.Add("Jan", 2);
//dataSeries1.Add("Feb", 4);
//dataSeries1.Add("Mar", 6);
//ChartData.DataSeriesList.Add(dataSeries1); 

//var dataSeries2 = new Dictionary<string, int>();
//dataSeries2.Add("Jan", 7);
//dataSeries2.Add("Feb", 8);
//dataSeries2.Add("Mar", 9);
//ChartData.DataSeriesList.Add(dataSeries2);
//ChartDataList = new List<ChartData>();
//ChartDataList.Add(ChartData);

//InitializeComponent();

//this.DataContext = this;