using GraphX.Controls;
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

namespace Simulation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for ShowPathFind.xaml
    /// </summary>
    public partial class ShowPathFind : Window
    {
        public ShowPathFind()
        {
            InitializeComponent();
        }

    /*    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            { 
                foreach (DataEdge ed in graphArea.LogicCore.Graph.Edges)
                    foreach (IEnumerable<DataEdge> path in rank.ComputedShortestPaths)
                        foreach (DataEdge edge in path)
                        {
                            if ((edge == ed) || (edge.Source == ed.Target && edge.Target == ed.Source)) ed.Color = "Gold";
                        }
                graphArea.StateStorage.SaveState("exampleState");
                graphArea.StateStorage.LoadState("exampleState");
            }
        } */
    }
}
