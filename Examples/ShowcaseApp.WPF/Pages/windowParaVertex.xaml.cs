using System;
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

namespace SimulationV1.WPF.Pages
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
                    //DPDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.SelectedIndex = (int)VertexBefore.CreateType.TypeDistribuion;
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.CreateType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.CreateType.Interval.ToString();
                    DP3.Visibility = Visibility.Visible;
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
                    DP3.Visibility = Visibility.Visible;
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
                case "AMAnd":
                    //DPDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.SelectedIndex = (int)VertexBefore.AndType.TypeDistribuion;
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.AndType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.AndType.Interval.ToString();
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Длина файла:";
                    tBx3.Text = VertexBefore.AndType.LengthOfFile.ToString();
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

            switch (VertexBefore.TypeOfVertex)
            {
                case "AMCreate":
                    tBx1.Text = VertexBefore.CreateType.FirstTime.ToString();
                    tBx2.Text = VertexBefore.CreateType.Interval.ToString();
                    tBx3.Text = VertexBefore.CreateType.LengthOfFile.ToString();
                    switch (VertexBefore.CreateType.TypeDistribuion)
                    {
                        case CreateClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            break;
                        case CreateClass.Distribution.ExponentialDis:
                            cbbDistribution.SelectedIndex = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case "AMQueue":
                    tBx1.Text = VertexBefore.QueueType.QueueCapacity.ToString();
                    tBx2.Text = VertexBefore.QueueType.Priority.ToString();
                    tBx3.Text = VertexBefore.QueueType.FileType.ToString();
                    break;
                case "AMTerminate":
                    tBx1.Text = VertexBefore.TerminateType.OutputCounter.ToString();
                    tBx2.Text = VertexBefore.TerminateType.StoppingTime.ToString();
                    break;
                case "AMAnd":
                    tBx1.Text = VertexBefore.AndType.FirstTime.ToString();
                    tBx2.Text = VertexBefore.AndType.Interval.ToString();
                    tBx3.Text = VertexBefore.AndType.LengthOfFile.ToString();
                    switch (VertexBefore.AndType.TypeDistribuion)
                    {
                        case CreateClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            break;
                        case CreateClass.Distribution.ExponentialDis:
                            cbbDistribution.SelectedIndex = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            Close();
        }
        private void UpdateVertex()
        {
            VertexAfter.Text = tBxName.Text;
            VertexAfter.Traffic = double.Parse(tBxTraffic.Text);
            switch (VertexBefore.TypeOfVertex)
            {
                case "AMCreate":                                      
                    VertexAfter.CreateType.FirstTime = int.Parse(tBx1.Text);
                    VertexAfter.CreateType.Interval = int.Parse(tBx2.Text);
                    VertexAfter.CreateType.LengthOfFile = int.Parse(tBx3.Text);
                    switch (cbbDistribution.SelectedIndex)
                    {
                        case 0:
                            VertexAfter.CreateType.TypeDistribuion = CreateClass.Distribution.NormalDis;
                            break;
                        case 1:
                            VertexAfter.CreateType.TypeDistribuion = CreateClass.Distribution.ExponentialDis;
                            break;
                        default:
                            break;
                    }
                    break;
                case "AMQueue":
                    VertexAfter.QueueType.QueueCapacity = int.Parse(tBx1.Text);
                    VertexAfter.QueueType.Priority = int.Parse(tBx2.Text);
                    VertexAfter.QueueType.FileType = int.Parse(tBx3.Text);
                    break;
                case "AMTerminate":
                    VertexAfter.TerminateType.OutputCounter = int.Parse(tBx1.Text);
                    VertexAfter.TerminateType.StoppingTime = int.Parse(tBx2.Text);
                    break;
                case "AMAnd":
                    VertexAfter.AndType.FirstTime = int.Parse(tBx1.Text);
                    VertexAfter.AndType.Interval = int.Parse(tBx2.Text);
                    VertexAfter.AndType.LengthOfFile = int.Parse(tBx3.Text);
                    switch (cbbDistribution.SelectedIndex)
                    {
                        case 0:
                            VertexAfter.AndType.TypeDistribuion = CreateClass.Distribution.NormalDis;
                            break;
                        case 1:
                            VertexAfter.AndType.TypeDistribuion = CreateClass.Distribution.ExponentialDis;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            

        }
    }
}
