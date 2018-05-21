using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using SimulationV1.WPF.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using QuickGraph;
using SimulationV1.WPF.FileSerialization;
using Rect = GraphX.Measure.Rect;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Threading;
using AForge;
using AForge.Genetic;

namespace SimulationV1.WPF.Pages
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
            edgeBefore = ed;
            edgeAfter = ed;
            tBxName.Text = ed.Source.Text + "-" + ed.Target.Text;
            tBxCapacity.Text = ed.Capacity.ToString();
            tBxWeight.Text = ed.Weight.ToString();
            Label1.Content = "Вероятность выполнения:";
            tBx1.Text = ed.Probability.ToString();
            switch (edgeBefore.Color)
            {
                case "Orange":
                    ////DPDistribution.Visibility = Visibility.Visible;
                    //DP2.Visibility = Visibility.Visible;
                    Label2.Content = "Задержка:";
                    tBx2.Text = ed.Delay.ToString();
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Number of edge";
                    tBx3.Text = ed.Number.ToString();
                    break;
                case "Red":                   
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен1!");
                    break;
            }
            //edgeBefore = new DataEdge();
            //edgeAfter = new DataEdge();
            //tBxName.Text = ed.Source.Text + "-" + ed.Target.Text;
            //tBxCapacity.Text = ed.Capacity.ToString();
            //tBxWeight.Text = ed.Weight.ToString();
            //edgeBefore = ed;
            //edgeAfter = ed;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateEgde(edgeAfter);
            EditorGraph graph = new Pages.EditorGraph();
            graph.edgeSelected = edgeAfter;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tBxCapacity.Text = edgeBefore.Capacity.ToString();

            tBx1.Text = edgeBefore.Probability.ToString();
            switch (edgeBefore.Color)
            {
                case "Orange":
                    tBx2.Text = edgeBefore.Delay.ToString();
                    tBx3.Text = edgeBefore.Number.ToString();
                    break;
                case "Red":
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен2!");
                    break;
            }
            Close();
        }
        private void UpdateEgde(DataEdge ed)
        {
            //ed.Capacity = double.Parse(tBxCapacity.Text);
            //ed.Weight = double.Parse(tBxWeight.Text);
            try
            {

                edgeAfter.Text = tBxName.Text;
                edgeAfter.Capacity = double.Parse(tBxCapacity.Text);
                edgeAfter.Weight = double.Parse(tBxWeight.Text);
                edgeAfter.Probability = double.Parse(tBx1.Text);
                switch (edgeBefore.Color)
                {
                    case "Orange":
                        edgeAfter.Delay = double.Parse(tBx2.Text);
                        edgeAfter.Number = int.Parse(tBx3.Text);
                        break;
                    case "Red":
                        break;
                    default:
                        MessageBox.Show("Тип узлы не определен3!");
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


    }
}
