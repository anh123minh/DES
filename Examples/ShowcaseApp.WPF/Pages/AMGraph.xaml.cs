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
        readonly DataVertex _vertex;
        private PointCollection _result = new PointCollection();
        List<TablePoint> _tablePoint = new List<TablePoint>();

        public class TablePoint
        {
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
        }
        public AMGraph(DataVertex vertex0)
        {
            InitializeComponent();
            _vertex = vertex0;
            btnTable_Click(null, null);
        }
        public List<TablePoint> Table(List<List<double>> listpdf, List<List<double>> listcdf)
        {
            List<TablePoint> pointcoll = new List<TablePoint>();

            for (int i = 0; i < listpdf.Count; i++)
            {
                pointcoll.Add(new TablePoint { x = listpdf[i][0], y = listpdf[i][1], z = listcdf[i][1] });
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
            for (double i = men - Math.Sqrt(variance)*4; i < Math.Sqrt(variance) * 4 + men; i += Math.Sqrt(variance)/20)
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
            for (double i = men - Math.Sqrt(variance) * 4; i < Math.Sqrt(variance) * 4 + men; i += Math.Sqrt(variance) / 20)
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
            for (double i = men - Math.Sqrt(variance) * 4; i < Math.Sqrt(variance) * 4 + men; i += Math.Sqrt(variance) / 20)
            {
                var y = dist.CDF(i);
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }
        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            ChartIn.Title = "Плотность вероятности";
            btnPDF.IsEnabled = false;
            btnCDF.IsEnabled = true;
            btnTable.IsEnabled = true;
            Bang.Visibility = Visibility.Collapsed;
            Chart.Visibility = Visibility.Visible;
            Bang.LastChildFill = false;
            Chart.LastChildFill = true;
            if (_vertex.TypeOfVertex == "AMGenerator")
            {
                if (_vertex.GeneratorType.TListPointsPDF == null || _vertex.GeneratorType.TListPointsPDF.Count == 0)
                {
                    if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.GeneratorType.Para;
                        _result = ExponentialDistribution_PDF(lamda);
                    }
                    else if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.GeneratorType.Para;
                        var mean = _vertex.GeneratorType.Mean;
                        _result = NormallDistribution_PDF(varian, mean);
                    }
                }
                else
                {
                    _result = PDF(_vertex.GeneratorType.TListPointsPDF);
                }                
            }
            else if (_vertex.TypeOfVertex == "AMTransition")
            {
                if (_vertex.TransitionType.TListPointsPDF == null || _vertex.TransitionType.TListPointsPDF.Count == 0)
                {
                    if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.TransitionType.Para;
                        _result = ExponentialDistribution_PDF(lamda);
                    }
                    else if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.TransitionType.Para;
                        var mean = _vertex.TransitionType.Mean;
                        _result = NormallDistribution_PDF(varian, mean);
                    }
                }
                else
                {
                    _result = PDF(_vertex.TransitionType.TListPointsPDF);
                }
            }
            DoThi.ItemsSource = _result;
        }
        private void btnCDF_Click(object sender, RoutedEventArgs e)
        {
            ChartIn.Title = "Функция распределения";
            btnPDF.IsEnabled = true;
            btnCDF.IsEnabled = false;
            btnTable.IsEnabled = true;
            Bang.Visibility = Visibility.Collapsed;
            Chart.Visibility = Visibility.Visible;
            Bang.LastChildFill = false;
            Chart.LastChildFill = true;
            if (_vertex.TypeOfVertex == "AMGenerator")
            {
                if (_vertex.GeneratorType.TListPointsCDF == null || _vertex.GeneratorType.TListPointsPDF.Count == 0)
                {
                    if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.GeneratorType.Para;
                        _result = ExponentialDistribution_CDF(lamda);
                    }
                    if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.GeneratorType.Para;
                        var mean = _vertex.GeneratorType.Mean;
                        _result = NormalDistribution_CDF(varian, mean);
                    }
                }
                else
                {
                    _result = CDF(_vertex.GeneratorType.TListPointsCDF);
                }
                
            }
            else if (_vertex.TypeOfVertex == "AMTransition")
            {
                if (_vertex.TransitionType.TListPointsCDF == null || _vertex.TransitionType.TListPointsPDF.Count == 0)
                {
                    if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.TransitionType.Para;
                        _result = ExponentialDistribution_CDF(lamda);
                    }
                    if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.TransitionType.Para;
                        var mean = _vertex.TransitionType.Mean;
                        _result = NormalDistribution_CDF(varian, mean);
                    }
                }
                else
                {
                    _result = CDF(_vertex.TransitionType.TListPointsCDF);
                }
            }
            DoThi.ItemsSource = _result;
        }
        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            btnPDF.IsEnabled = true;
            btnCDF.IsEnabled = true;
            btnTable.IsEnabled = false;
            Bang.Visibility = Visibility.Visible;
            Chart.Visibility = Visibility.Collapsed;
            Bang.LastChildFill = true;
            Chart.LastChildFill = false;
            if (_vertex.TypeOfVertex == "AMGenerator")
            {
                if (_vertex.GeneratorType.TListPointsPDF == null || _vertex.GeneratorType.TListPointsPDF.Count == 0)//xem lai xem co can check null nua k?
                {
                    if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.GeneratorType.Para;
                        _tablePoint = Table_Exp(lamda);
                    }
                    else if (_vertex.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.GeneratorType.Para;
                        var mean = _vertex.GeneratorType.Mean;
                        _tablePoint = Table_Normal(varian, mean);
                    }
                }
                else
                {
                    _tablePoint = Table(_vertex.GeneratorType.TListPointsPDF, _vertex.GeneratorType.TListPointsCDF);
                }
                
            }
            else if (_vertex.TypeOfVertex == "AMTransition")
            {
                if (_vertex.TransitionType.TListPointsPDF == null || _vertex.TransitionType.TListPointsPDF.Count == 0)//xem lai xem co can check null nua k?
                {
                    if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    {
                        var lamda = _vertex.TransitionType.Para;
                        _tablePoint = Table_Exp(lamda);
                    }
                    else if (_vertex.TransitionType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    {
                        var varian = _vertex.TransitionType.Para;
                        var mean = _vertex.TransitionType.Mean;
                        _tablePoint = Table_Normal(varian, mean);
                    }
                }
                else
                {
                    _tablePoint = Table(_vertex.TransitionType.TListPointsPDF, _vertex.TransitionType.TListPointsCDF);
                }                
            }
            TableData.ItemsSource = _tablePoint;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
