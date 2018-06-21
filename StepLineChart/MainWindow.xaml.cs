using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StepLineChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ChartData ChartData { get; set; }
        public List<ChartData> ChartDataList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            PointCollection aaa = new PointCollection()
            {
                new Point(0,0),
                new Point(2,1),
                new Point(2,0),
                new Point(3,1),              
                //new Point(30,6),
                //new Point(30,4),
                //new Point(30,2),
                //new Point(31,5),
                //new Point(3,1),
                //new Point(10,3),
                //new Point(12,1),
                ////new Point(5,4),
                //new Point(15,3),
                //new Point(16,30),
                ////new Point(6,6),
                //////new Point(7,6),
                ////new Point(7,7),
                //////new Point(8,7),
                ////new Point(8,8),
                //////new Point(9,8),
                ////new Point(9,9),
                //////new Point(10,9),
                ////new Point(10,10)

            };
            //var a = new StepLineSeries {Points = aaa};
            BieuDo.ItemsSource = aaa; //.Points;

            var listtime = new List<int> {0, 1, 1, 3, 4, 5,5, 6, 7, 8, 9, 10};
            var listblockcount = new List<List<int>>();
            var liscount1 = new List<int> {0, 1, 0, 1, 8, 10,5, 7, 9, 11, 18, 5};
            var liscount2 = new List<int> { 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0 };
            listblockcount.Add(liscount1);
            listblockcount.Add(liscount2);


            ChartData = new ChartData();
            ChartData.Title = "Количество разметки ";
            ChartData.DataSeriesList = new List<ChartDataSerie>();
            //for (int i = 0; i < listblockcount.Count; i++)
            //{
            //    var dataSeries = new Dictionary<int, int>();
            //    for (int j = 0; j < listblockcount[0].Count; j++)
            //    {
            //        dataSeries.Add(listtime[j], listblockcount[i][j]);
            //    }
            //    ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"aaa + {i}", Data = dataSeries });
            //}

            //quan trong mo rong
            for (int i = 0; i < listblockcount.Count; i++)
            {
                var dataSeries = new List<ChartPoint>();
                for (int j = 0; j < listblockcount[0].Count; j++)
                {
                    dataSeries.Add(new ChartPoint() { Time = listtime[j], Value = listblockcount[i][j] });
                }
                ChartData.DataSeriesList.Add(new ChartDataSerie() { Name = $"Serie {i}", Data = dataSeries });
            }

            ChartDataList = new List<ChartData>();
            ChartDataList.Add(ChartData);

            this.DataContext = this;
        }
    }
    public class ChartData
    {
        public string Title { get; set; }
        public List<ChartDataSerie> DataSeriesList { get; set; }
    }

    //public class ChartDataSerie
    //{
    //    public string Name { get; set; }

    //    public Dictionary<int, int> Data { get; set; }
    //}

    //public class Lines
    //{
    //    public string LineName { get; set; }
    //    public List<int> LineData { get; set; }
    //}
    //quan trong mo rong
    public class ChartDataSerie
    {
        public string Name { get; set; }
        public List<ChartPoint> Data { get; set; }
    }

    public class ChartPoint
    {
        public int Time { get; set; }
        public int Value { get; set; }
    }
    //public class ChartData
    //{
    //    public string Title { get; set; }
    //    public List<List<int>> DataSeriesList { get; set; }
    //}
}
