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
        public MainWindow()
        {
            InitializeComponent();
            PointCollection aaa = new PointCollection()
            {   new Point(1,1),
                //new Point(2,1),
                new Point(2,2),
                //new Point(3,2),
                new Point(3,3),
                //new Point(5,3),
                //new Point(3,1),
                new Point(6,1),
                new Point(8,3),
                //new Point(5,4),
                new Point(9,5),
                //new Point(6,5),
                //new Point(6,6),
                ////new Point(7,6),
                //new Point(7,7),
                ////new Point(8,7),
                //new Point(8,8),
                ////new Point(9,8),
                //new Point(9,9),
                ////new Point(10,9),
                //new Point(10,10)

            };
            //var a = new StepLineSeries {Points = aaa};
            BieuDo.ItemsSource = aaa; //.Points;
        }
    }
}
