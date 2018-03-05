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
    /// Interaction logic for windowDiagramAlpha.xaml
    /// </summary>
    public partial class windowDiagramAlpha : Window
    {
        List<KeyValuePair<string, double>> valueList;
        List<KeyValuePair<string, double>> valueListFull;
        public windowDiagramAlpha(IEnumerable<DataEdge> edgelist)
        {
            InitializeComponent();
            valueList = new List<KeyValuePair<string, double>>();
            valueListFull = new List<KeyValuePair<string, double>>();
            foreach (DataEdge ed in edgelist)
            {
                if (ed.Alpha != 0)
                    valueList.Add(new KeyValuePair<string, double>(ed.Source.Text + "-" + ed.Target.Text, ed.Alpha));
                valueListFull.Add(new KeyValuePair<string, double>(ed.Source.Text + "-" + ed.Target.Text, ed.Alpha));
            }


            barChart.DataContext = valueList;
            cbxmode.Checked += Cbxmode_Change;
            cbxmode.Unchecked += Cbxmode_Change;
        }

        private void Cbxmode_Change(object sender, RoutedEventArgs e)
        {
            if (cbxmode.IsChecked == true) barChart.DataContext = valueListFull;
            else barChart.DataContext = valueList;
        }
    }
}
