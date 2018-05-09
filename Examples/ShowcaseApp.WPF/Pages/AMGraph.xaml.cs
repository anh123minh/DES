﻿using System;
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

        public AMGraph(DataVertex vertex0)
        {
            InitializeComponent();
            vertex = vertex0;
                     
        }

        public PointCollection ExponentialDistribution_PDF(double m)
        {
            PointCollection pointcoll = new PointCollection();
            for (double i = 0.001; i < 5; i+=0.1)
            {
                var y = m * Math.Exp(-m * i);
                pointcoll.Add(new Point(i, y));
            }           
            return pointcoll;
        }

        public PointCollection ExponentialDistribution_CDF(double m)
        {
            PointCollection pointcoll = new PointCollection();
            for (double i = 0.001; i < 5; i += 0.1)
            {
                var y = 1 - Math.Exp(-m * i);
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }

        public PointCollection NormallDistribution_PDF(double deviation, double m = 0)
        {
            PointCollection pointcoll = new PointCollection();
            for (double i = -15; i < 15; i += 0.1)
            {
                var y = 1/(Math.Pow(2*Math.PI*Math.Pow(deviation,2),0.5))*Math.Exp(-(Math.Pow(i-m,2))/(2* Math.Pow(deviation, 2)));
                pointcoll.Add(new Point(i, y));
            }
            return pointcoll;
        }

        public PointCollection NormalDistribution_CDF(double m)
        {
            PointCollection pointcoll = new PointCollection();
            //for (double i = 0.5; i < 5; i += 0.25)
            //{
            //    var y = 1 - Math.Exp(-m * i);
            //    pointcoll.Add(new Point(i, y));
            //}
            return pointcoll;
        }

        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            if (vertex.TypeOfVertex == "AMCreate")
            {
                if (vertex.CreateType.TypeDistribuion == CreateClass.Distribution.ExponentialDis)
                {
                    var mean = vertex.CreateType.Interval;
                    result = ExponentialDistribution_PDF(mean);
                }
                else if(vertex.CreateType.TypeDistribuion == CreateClass.Distribution.NormalDis)
                {
                    var mean = vertex.CreateType.Interval;
                    result = NormallDistribution_PDF(mean);
                }
            }
            else if (vertex.TypeOfVertex == "AMAnd")
            {
                if (vertex.AndType.TypeDistribuion == CreateClass.Distribution.ExponentialDis)
                {
                    var mean = vertex.AndType.Interval;
                    result = ExponentialDistribution_PDF(mean);
                }
                else if (vertex.AndType.TypeDistribuion == CreateClass.Distribution.NormalDis)
                {
                    var mean = vertex.AndType.Interval;
                    result = NormallDistribution_PDF(mean);
                }
            }
            DoThi.ItemsSource = result;
        }

        private void btnCDF_Click(object sender, RoutedEventArgs e)
        {
            if (vertex.TypeOfVertex == "AMCreate")
            {
                if (vertex.CreateType.TypeDistribuion == CreateClass.Distribution.ExponentialDis)
                {
                    var mean = vertex.CreateType.Interval;
                    result = ExponentialDistribution_CDF(mean);
                }
                if (vertex.CreateType.TypeDistribuion == CreateClass.Distribution.NormalDis)
                {
                    var mean = vertex.CreateType.Interval;
                    result = NormalDistribution_CDF(mean);
                }

            }
            else if (vertex.TypeOfVertex == "AMAnd")
            {
                if (vertex.AndType.TypeDistribuion == CreateClass.Distribution.ExponentialDis)
                {
                    var mean = vertex.AndType.Interval;
                    result = ExponentialDistribution_CDF(mean);
                }
                if (vertex.AndType.TypeDistribuion == CreateClass.Distribution.NormalDis)
                {
                    var mean = vertex.AndType.Interval;
                    result = NormalDistribution_CDF(mean);
                }
            }
            DoThi.ItemsSource = result;
        }

        
    }
}
