﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for windowParaVertex.xaml
    /// </summary>
    public partial class windowParaVertex : Window
    {
        public delegate void SetValueForm(DataVertex vtx);
        public SetValueForm SetValueControl;
        public DataVertex VertexBefore;
        public DataVertex VertexAfter;       
        public windowParaVertex()
        {
            InitializeComponent();
            SetValueControl = new SetValueForm(GetValue);
        }

        public void GetValue(DataVertex vtx)
        {
            VertexBefore= vtx;
            VertexAfter = vtx;
            switch (VertexBefore.TypeOfVertex)
            {
                case "AMCreate":              
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.CreateType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.CreateType.Interval.ToString();
                    Label3.Content = "Длина файла:";
                    tBx3.Text = VertexBefore.CreateType.LengthOfFile.ToString();
                    break;
                case "AMQueue":
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Ёмкость очереди:";
                    tBx1.Text = VertexBefore.QueueType.QueueCapacity.ToString();
                    Label2.Content = "Приоритет:";
                    tBx2.Text = VertexBefore.QueueType.Priority.ToString();
                    Label3.Content = "Тип файла:";
                    tBx3.Text = VertexBefore.QueueType.FileType.ToString();
                    break;
                case "AMTerminate":
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Выходной счетчик:";
                    tBx1.Text = VertexBefore.TerminateType.OutputCounter.ToString();
                    Label2.Content = "Момент остоновки:";
                    tBx2.Text = VertexBefore.TerminateType.StoppingTime.ToString();
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен!");
                    break;
            }

            //VertexBefore = vtx;
            //VertexAfter = vtx;
            //tBxName.Text = VertexBefore.Text;
            //tBxTraffic.Text = VertexBefore.Traffic.ToString();

        }
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateVertex();
            EditorGraph graph = new Pages.EditorGraph();
            graph.vertexSelected = VertexAfter;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tBxName.Text = VertexBefore.Text;
            tBxTraffic.Text = VertexBefore.Traffic.ToString();
        }
        private void UpdateVertex()
        {
            VertexAfter.Text = tBxName.Text;
            VertexAfter.Traffic = double.Parse(tBxTraffic.Text);
        }
    }
}
