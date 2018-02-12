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
            tBxName.Text = VertexBefore.Text;
            tBxTraffic.Text = VertexBefore.Traffic.ToString();
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
