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
using DevExpress;

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AMResult.xaml
    /// </summary>
    public partial class AMResult : Window
    {
        
        public AMResult(PointCollection aaa)
        {
            InitializeComponent();
            //PointCollection aaa = new PointCollection()
            //{   new Point(1,1),
            //    //new Point(2,1),
            //    new Point(2,2),
            //    //new Point(3,2),
            //    new Point(3,3),
            //    //new Point(4,3),
            //    new Point(4,4),
            //    //new Point(5,4),
            //    new Point(5,5),
            //    //new Point(6,5),
            //    new Point(6,6),
            //    //new Point(7,6),
            //    new Point(7,7),
            //    //new Point(8,7),
            //    new Point(8,8),
            //    //new Point(9,8),
            //    new Point(9,9),
            //    //new Point(10,9),
            //    new Point(10,10)

            //};

            BieuDo.ItemsSource = aaa; 
        }

        //private void WindowLoad(object sender, EventArgs e)
        //{ItemsSource="{Binding}" Title="" IsSelectionEnabled="True"

        //}


        
    }
}
