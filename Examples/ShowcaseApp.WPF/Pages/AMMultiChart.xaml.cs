using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<int> listtimenowtable;
        private List<List<int>> listblockcounttable;
        public AMMultiChart(List<int> listtimenowtb, List<List<int>> listblockcounttb, List<int> listtimenowgr, List<List<int>> listblockcountgr)
        {
            listtimenowtable = listtimenowtb;
            listblockcounttable = listblockcounttb;

            listtimenowgraph = listtimenowgr;
            listblockcountgraph = listblockcountgr;           

            InitializeComponent();
            btnMultiGraph_Click(null, null);


        }

        public AMMultiChart(List<int> listtimenowtb, List<List<int>> listblockcounttb)
        {
            listtimenowtable = listtimenowtb;
            listblockcounttable = listblockcounttb;

            InitializeComponent();
            btnMultiGraph_Click(null, null);


        }
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
        private void btnMultiGraph_Click(object sender, RoutedEventArgs e)
        {
            Bang.Visibility = Visibility.Collapsed;
            //Bang.LastChildFill = false;
            Graph.Visibility = Visibility.Visible;
            //Graph.LastChildFill = true;

            ChartData = new ChartData();
            ChartData.Title = "Chart Title";
            ChartData.DataSeriesList = new List<Dictionary<int, int>>();

            for (int i = 0; i < listblockcounttable.Count; i++)
            {
                var dataSeries = new Dictionary<int, int>();
                for (int j = 0; j < listtimenowtable.Count; j++)
                {
                    dataSeries.Add(listtimenowtable[j], listblockcounttable[i][j]);
                }
                ChartData.DataSeriesList.Add(dataSeries);
            }


            ChartDataList = new List<ChartData>();
            ChartDataList.Add(ChartData);

            this.DataContext = this;
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            Graph.Visibility = Visibility.Collapsed;
            //Graph.LastChildFill = false;
            Bang.Visibility = Visibility.Visible;
            //Bang.LastChildFill = true;
            
            //var dara = new ObservableCollection<List<int>>();
            var dara = new List<List<int>>();
            dara.Add(listtimenowtable);
            foreach (var sd in listblockcounttable)
            {
                dara.Add(sd);
            }
            TableData.ItemsSource = Cot2Hang(dara);
            //var ns = new List<List<int>>();
            //var s1 = new List<int>(){1,2,3,4,5};
            //var s2 = new List<int>() { 2, 4, 6, 8, 10 };
            //var s3 = new List<int>() { 10, 20, 30, 40, 50 };
            //ns.Add(s1);
            //ns.Add(s2);
            //ns.Add(s3);
            //TableData.ItemsSource = s1;
        }

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
        public List<Dictionary<int, int>> DataSeriesList { get; set; }
    }

    //public class ChartData
    //{
    //    public string Title { get; set; }
    //    public List<List<int>> DataSeriesList { get; set; }
    //}
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