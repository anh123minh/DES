using System;
using System.Collections.Generic;
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
    /// Interaction logic for AMGraph.xaml
    /// </summary>
    public partial class AMGraph : Window
    {
        readonly DataVertex vertex;
        PointCollection result = new PointCollection();
        List<List<double>> tableList = new List<List<double>>();
        List<TablePoint> tablePoint = new List<TablePoint>();

        public class TablePoint
        {
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
        }

        public AMGraph(DataVertex vertex0)
        {
            InitializeComponent();
            vertex = vertex0;
            btnTable_Click(null, null);
                     
        }

        public List<TablePoint> Table(List<List<double>> listpdf, List<List<double>> listcdf)
        {
            List<TablePoint> pointcoll = new List<TablePoint>();

            for (int i = 0; i < listpdf.Count; i++)
            {
                pointcoll.Add(new TablePoint() { x = listpdf[i][0], y = listpdf[i][1], z = listcdf[i][1] });
            }
            return pointcoll;
        }
        public List<TablePoint> Table_Exp(double lamda)
        {
            List<TablePoint> pointcoll = new List<TablePoint>();
            for (double i = 0.001; i < 5; i += 0.01)
            {
                var pdf = lamda * Math.Exp(-lamda * i);
                var cdf = 1 - Math.Exp(-lamda * i);
                pointcoll.Add(new TablePoint(){ x = Math.Round(i, 3), y = pdf, z = cdf});
            }
            return pointcoll;
        }        
        public List<TablePoint> Table_Normal(double variance, double men)
        {
            List<TablePoint> pointcoll = new List<TablePoint>();
            NormalDist dist = new NormalDist(men, variance);
            for (double i = 0 - Math.Sqrt(variance)*4; i < Math.Sqrt(variance) * 4; i += Math.Sqrt(variance)/20)
            {
                var pdf = dist.PDF(i);
                var cdf = dist.CDF(i);
                pointcoll.Add(new TablePoint() { x = Math.Round(i,2), y = pdf, z = cdf });
            }
            return pointcoll;
        }

        public PointCollection PDF(List<List<double>> listpdf)
        {
            PointCollection pointcoll = new PointCollection();
            for (int i = 0; i < listpdf.Count; i++)
            {
                pointcoll.Add(new Point(listpdf[i][0], listpdf[i][1]));
            }
            return pointcoll;
        }
        public PointCollection ExponentialDistribution_PDF(double m)
        {
            PointCollection pointcoll = new PointCollection();
            for (double i = 0.001; i < 5; i+=0.01)
            {
                var y = m * Math.Exp(-m * i);
                pointcoll.Add(new Point(i, y));
            }           
            return pointcoll;
        }
        public PointCollection ExponentialDistribution_CDF(double m)
        {
            PointCollection pointcoll = new PointCollection();
            for (double i = 0.001; i < 5; i += 0.01)
            {
                var y = 1 - Math.Exp(-m * i);
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }

        public PointCollection CDF(List<List<double>> listcdf)
        {
            PointCollection pointcoll = new PointCollection();
            for (int i = 0; i < listcdf.Count; i++)
            {
                pointcoll.Add(new Point(listcdf[i][0], listcdf[i][1]));
            }
            return pointcoll;
        }
        public PointCollection NormallDistribution_PDF(double variance, double men)
        {
            PointCollection pointcoll = new PointCollection();
            NormalDist dist = new NormalDist(men, variance);
            for (double i = 0 - Math.Sqrt(variance) * 4; i < Math.Sqrt(variance) * 4; i += Math.Sqrt(variance) / 20)
            {
                var y = dist.PDF(i);
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }
        public PointCollection NormalDistribution_CDF(double variance, double men)
        {
            PointCollection pointcoll = new PointCollection();
            NormalDist dist = new NormalDist(men, variance);
            for (double i = 0 - Math.Sqrt(variance) * 4; i < Math.Sqrt(variance) * 4; i += Math.Sqrt(variance) / 20)
            {
                var y = dist.CDF(i);
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }

        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            Bang.Visibility = Visibility.Collapsed;
            Chart.Visibility = Visibility.Visible;
            if (vertex.TypeOfVertex == "AMGenerator")
            {
                if (vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                {
                    var lamda = vertex.GeneratorType.Para;
                    result = ExponentialDistribution_PDF(lamda);
                }
                else if(vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                {
                    var varian = vertex.GeneratorType.Para;
                    var mean = vertex.GeneratorType.Mean;
                    result = NormallDistribution_PDF(varian, mean);
                }
            }
            else if (vertex.TypeOfVertex == "AMTransition")
            {
                if (vertex.ListPointsPDF == null || vertex.ListPointsPDF.Count == 0)
                {
                    if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = vertex.TransitionType.Para;
                        result = ExponentialDistribution_PDF(lamda);
                    }
                    else if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = vertex.TransitionType.Para;
                        var mean = vertex.TransitionType.Mean;
                        result = NormallDistribution_PDF(varian, mean);
                    }
                }
                else
                {
                    result = PDF(vertex.ListPointsPDF);
                }
            }
            DoThi.ItemsSource = result;
        }
        private void btnCDF_Click(object sender, RoutedEventArgs e)
        {
            Bang.Visibility = Visibility.Collapsed;
            Chart.Visibility = Visibility.Visible;
            if (vertex.TypeOfVertex == "AMGenerator")
            {
                if (vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                {
                    var lamda = vertex.GeneratorType.Para;
                    result = ExponentialDistribution_CDF(lamda);
                }
                if (vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                {
                    var varian = vertex.GeneratorType.Para;
                    var mean = vertex.GeneratorType.Mean;
                    result = NormalDistribution_CDF(varian, mean);
                }

            }
            else if (vertex.TypeOfVertex == "AMTransition")
            {
                if (vertex.ListPointsCDF == null || vertex.ListPointsPDF.Count == 0)
                {
                    if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = vertex.TransitionType.Para;
                        result = ExponentialDistribution_CDF(lamda);
                    }
                    if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = vertex.TransitionType.Para;
                        var mean = vertex.TransitionType.Mean;
                        result = NormalDistribution_CDF(varian, mean);
                    }
                }
                else
                {
                    result = CDF(vertex.ListPointsCDF);
                }
            }
            DoThi.ItemsSource = result;
        }
        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            Bang.Visibility = Visibility.Visible;
            Chart.Visibility = Visibility.Collapsed;
            if (vertex.TypeOfVertex == "AMGenerator")
            {
                if (vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                {
                    var lamda = vertex.GeneratorType.Para;
                    tablePoint = Table_Exp(lamda);
                }
                else if (vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                {
                    var varian = vertex.GeneratorType.Para;
                    var mean = vertex.GeneratorType.Mean;
                    tablePoint = Table_Normal(varian, mean);
                }
            }
            else if (vertex.TypeOfVertex == "AMTransition")
            {
                if (vertex.ListPointsPDF == null || vertex.ListPointsPDF.Count == 0)
                {
                    if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = vertex.TransitionType.Para;
                        tablePoint = Table_Exp(lamda);
                    }
                    else if (vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = vertex.TransitionType.Para;
                        var mean = vertex.TransitionType.Mean;
                        tablePoint = Table_Normal(varian, mean);
                    }
                }
                else
                {
                    tablePoint = Table(vertex.ListPointsPDF, vertex.ListPointsCDF);
                }                
            }
            TableData.ItemsSource = tablePoint;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
