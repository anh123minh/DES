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
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for windowResults.xaml
    /// </summary>
    public partial class windowResults : Window
    {
        private Stopwatch t;
        public int iterations;
        private double speed;
        private double remainingTime;
        public  windowResults()
        {
            InitializeComponent();
            t = new Stopwatch();
            t.Start();
            TabDiagram.Visibility = Visibility.Collapsed;
            TabRoutes.Visibility = Visibility.Collapsed;
            TabControlResult.SelectedIndex = 2;
        }
        ObservableCollection<Point> points = new ObservableCollection<Point>();

      
        public void UpdateLineSeries(int iteration, double alpha)
        {
            
            points.Add(new Point(iteration, alpha));
            LineSeries.ItemsSource = points;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbElapsedTime.Content = Math.Round(t.Elapsed.TotalMilliseconds).ToString() ;
            if (progressBar.Value == progressBar.Maximum)
            {
                t.Stop();
                percentProgressBar.Content = "Процесс завершен";
                btnResult.IsEnabled = true;               
                btnStop.IsEnabled=false ;
            }
               
            percentProgressBar.Content = Math.Round(progressBar.Value * 100 / progressBar.Maximum).ToString() + " %";
            speed = Math.Round( progressBar.Value / t.Elapsed.TotalSeconds);
           
            lbSpeed.Content = speed.ToString();
            remainingTime = Math.Round((iterations - progressBar.Value) * (t.Elapsed.TotalMilliseconds/progressBar.Value));
            lbRemainingTime.Content = remainingTime.ToString();
        }
    }
}
