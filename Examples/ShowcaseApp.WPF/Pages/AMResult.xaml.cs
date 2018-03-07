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
    /// Interaction logic for AMResult.xaml
    /// </summary>
    public partial class AMResult : Window
    {
        public AMResult()
        {
            InitializeComponent();
            int x = 0;int y=0;
            var m = new Random();
            var points = new List<Point>();
            for (int i = 0; i < 100; i++)
            {
                x++;
                y++;
                points.Add(new Point(x+m.Next(0,3),y+m.Next(0,3)));
            }
            LineSeries.ItemsSource = points;

        }
    }
}
