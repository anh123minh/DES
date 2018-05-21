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
                case "AMGenerator":
                    //DPDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.SelectedIndex = (int)VertexBefore.GeneratorType.TypeDistribuion;
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    DP1.Visibility = Visibility.Visible;
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.GeneratorType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.GeneratorType.Interval.ToString();
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Длина файла:";
                    tBx3.Text = VertexBefore.GeneratorType.LengthOfFile.ToString();
                    btnGraph.Visibility = Visibility.Visible;
                    break;
                case "AMPlace":
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    DP1.Visibility = Visibility.Visible;
                    Label1.Content = "Ёмкость очереди:";
                    tBx1.Text = VertexBefore.PlaceType.QueueCapacity.ToString();
                    Label2.Content = "Приоритет:";
                    tBx2.Text = VertexBefore.PlaceType.Priority.ToString();
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Тип файла:";
                    tBx3.Text = VertexBefore.PlaceType.FileType.ToString();
                    break;
                case "AMTerminate":
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    Label1.Content = "Выходной счетчик:";
                    tBx1.Text = VertexBefore.TerminateType.OutputCounter.ToString();
                    Label2.Content = "Момент остоновки:";
                    tBx2.Text = VertexBefore.TerminateType.StoppingTime.ToString();
                    break;
                case "AMTransition":
                    //DPDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.SelectedIndex = (int)VertexBefore.TransitionType.TypeDistribuion;
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    //Label1.Content = "Начало:";
                    //tBx1.Text = VertexBefore.TransitionType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.TransitionType.Interval.ToString();
                    //DP3.Visibility = Visibility.Visible;
                    //Label3.Content = "Длина файла:";
                    //tBx3.Text = VertexBefore.TransitionType.LengthOfFile.ToString();
                    btnGraph.Visibility = Visibility.Visible;
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
            //MessageBox.Show("Saved!");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tBxName.Text = VertexBefore.Text;
            tBxTraffic.Text = VertexBefore.Traffic.ToString();

            switch (VertexBefore.TypeOfVertex)
            {
                case "AMGenerator":
                    tBx1.Text = VertexBefore.GeneratorType.FirstTime.ToString();
                    tBx2.Text = VertexBefore.GeneratorType.Interval.ToString();
                    tBx3.Text = VertexBefore.GeneratorType.LengthOfFile.ToString();
                    switch (VertexBefore.GeneratorType.TypeDistribuion)
                    {
                        case GeneratorClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            break;
                        case GeneratorClass.Distribution.ExponentialDis:
                            cbbDistribution.SelectedIndex = 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case "AMPlace":
                    tBx1.Text = VertexBefore.PlaceType.QueueCapacity.ToString();
                    tBx2.Text = VertexBefore.PlaceType.Priority.ToString();
                    tBx3.Text = VertexBefore.PlaceType.FileType.ToString();
                    break;
                case "AMTerminate":
                    tBx1.Text = VertexBefore.TerminateType.OutputCounter.ToString();
                    tBx2.Text = VertexBefore.TerminateType.StoppingTime.ToString();
                    break;
                case "AMTransition":
                    tBx1.Text = VertexBefore.TransitionType.FirstTime.ToString();
                    tBx2.Text = VertexBefore.TransitionType.Interval.ToString();
                    tBx3.Text = VertexBefore.TransitionType.LengthOfFile.ToString();
                    switch (VertexBefore.TransitionType.TypeDistribuion)
                    {
                        case GeneratorClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            break;
                        case GeneratorClass.Distribution.ExponentialDis:
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
            try
            {
                VertexAfter.Text = tBxName.Text;
                VertexAfter.Traffic = double.Parse(tBxTraffic.Text);
                switch (VertexBefore.TypeOfVertex)
                {
                    case "AMGenerator":
                        VertexAfter.GeneratorType.FirstTime = int.Parse(tBx1.Text);
                        VertexAfter.GeneratorType.Interval = double.Parse(tBx2.Text);
                        VertexAfter.GeneratorType.LengthOfFile = int.Parse(tBx3.Text);
                        switch (cbbDistribution.SelectedIndex)
                        {
                            case 0:
                                VertexAfter.GeneratorType.TypeDistribuion = GeneratorClass.Distribution.NormalDis;
                                break;
                            case 1:
                                VertexAfter.GeneratorType.TypeDistribuion = GeneratorClass.Distribution.ExponentialDis;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "AMPlace":
                        VertexAfter.PlaceType.QueueCapacity = int.Parse(tBx1.Text);
                        VertexAfter.PlaceType.Priority = int.Parse(tBx2.Text);
                        VertexAfter.PlaceType.FileType = int.Parse(tBx3.Text);
                        break;
                    case "AMTerminate":
                        VertexAfter.TerminateType.OutputCounter = int.Parse(tBx1.Text);
                        VertexAfter.TerminateType.StoppingTime = int.Parse(tBx2.Text);
                        break;
                    case "AMTransition":
                        //VertexAfter.TransitionType.FirstTime = int.Parse(tBx1.Text);
                        //VertexAfter.TransitionType.Interval = double.Parse(tBx2.Text);
                        VertexAfter.TransitionType.LengthOfFile = int.Parse(tBx3.Text);
                        switch (cbbDistribution.SelectedIndex)
                        {
                            case 0:
                                VertexAfter.TransitionType.TypeDistribuion = GeneratorClass.Distribution.NormalDis;
                                break;
                            case 1:
                                VertexAfter.TransitionType.TypeDistribuion = GeneratorClass.Distribution.ExponentialDis;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                MessageBox.Show("Saved!");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            var amGraph = new AMGraph(VertexAfter);
            amGraph.Show();
        }

        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}
       
    }
}
