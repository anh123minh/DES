using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using TrafficManagement.WPF.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using QuickGraph;
using TrafficManagement.WPF.FileSerialization;
using Rect = GraphX.Measure.Rect;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Threading;
using AForge;
using AForge.Genetic;

namespace TrafficManagement.WPF.Pages
{
    
    /// <summary>
    /// Interaction logic for windowParaEdge.xaml
    /// </summary>
    public partial class windowParaEdge : Window
    {
        
        public delegate void SetValueForm(DataEdge ed);
        public SetValueForm SetValueControl;
        private DataEdge edgeBefore;
        private DataEdge edgeAfter;
        
        public windowParaEdge()
        {
            InitializeComponent();
            SetValueControl = new SetValueForm(GetValueControl);
            
        }
        
    private void GetValueControl(DataEdge ed)
    {
            edgeBefore = new DataEdge();
            edgeAfter = new DataEdge();
            tBxName.Text = ed.Source.Text + "-" + ed.Target.Text;
            tBxCapacity.Text = ed.Capacity.ToString();
            tBxWeight.Text = ed.Weight.ToString();
            edgeBefore = ed;
            edgeAfter = ed;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateEgde(edgeAfter);
            TrafficManagement.WPF.Pages.EditorGraph graph = new Pages.EditorGraph();
            graph.edgeSelected = edgeAfter;         
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {            
            tBxCapacity.Text = edgeBefore.Capacity.ToString();
        }
        private void UpdateEgde(DataEdge ed)
        {
            ed.Capacity = double.Parse(tBxCapacity.Text);
            ed.Weight = double.Parse(tBxWeight.Text);
        }

       
    }
}
