using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text.RegularExpressions;

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
                    //if (VertexBefore.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    //{
                    //    DPPara.Visibility = Visibility.Visible;
                        tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();
                    //}
                    //if (VertexBefore.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    //{
                    //    DPPara.Visibility = Visibility.Collapsed;
                    //}
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    DP1.Visibility = Visibility.Visible;
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.GeneratorType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.GeneratorType.Mean.ToString();                    
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Длина файла:";
                    tBx3.Text = VertexBefore.GeneratorType.LengthOfFile.ToString();
                    btnGraph.Visibility = Visibility.Visible;
                    FromFile.Visibility = Visibility.Collapsed;                    
                    break;
                case "AMPlace":
                    DPPara.Visibility = Visibility.Collapsed;
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
                    FromFile.Visibility = Visibility.Collapsed;
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
                    //if (VertexBefore.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.NormalDis)
                    //{
                    //    DPPara.Visibility = Visibility.Visible;
                        tBxPara.Text = VertexBefore.TransitionType.Para.ToString();
                    //}
                    //if (VertexBefore.GeneratorType.TypeDistribuion == GeneratorClass.Distribution.ExponentialDis)
                    //{
                    //    DPPara.Visibility = Visibility.Collapsed;
                    //}
                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    //Label1.Content = "Начало:";
                    //tBx1.Text = VertexBefore.TransitionType.FirstTime.ToString();
                    Label2.Content = "Интервал:";
                    tBx2.Text = VertexBefore.TransitionType.Mean.ToString();
                    //DP3.Visibility = Visibility.Visible;
                    //Label3.Content = "Длина файла:";
                    //tBx3.Text = VertexBefore.TransitionType.LengthOfFile.ToString();
                    btnGraph.Visibility = Visibility.Visible;
                    FromFile.Visibility = Visibility.Visible;
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
            UpdateVertex(false);
            EditorGraph graph = new Pages.EditorGraph();
            graph.vertexSelected = VertexAfter;
            MessageBox.Show("Saved!");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tBxName.Text = VertexBefore.Text;
            tBxTraffic.Text = VertexBefore.Traffic.ToString();

            switch (VertexBefore.TypeOfVertex)
            {
                case "AMGenerator":
                    tBx1.Text = VertexBefore.GeneratorType.FirstTime.ToString();
                    tBx2.Text = VertexBefore.GeneratorType.Mean.ToString();
                    tBx3.Text = VertexBefore.GeneratorType.LengthOfFile.ToString();
                    switch (VertexBefore.GeneratorType.TypeDistribuion)
                    {
                        case GeneratorClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();
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
                    tBx2.Text = VertexBefore.TransitionType.Mean.ToString();
                    tBx3.Text = VertexBefore.TransitionType.LengthOfFile.ToString();
                    switch (VertexBefore.TransitionType.TypeDistribuion)
                    {
                        case GeneratorClass.Distribution.NormalDis:
                            cbbDistribution.SelectedIndex = 0;
                            tBxPara.Text = VertexBefore.TransitionType.Para.ToString();
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
        private void UpdateVertex(bool fromfile, List<List<double>> lispdf = null, List<List<double>> liscdf = null)
        {
            try
            {
                VertexAfter.Text = tBxName.Text;
                VertexAfter.Traffic = double.Parse(tBxTraffic.Text);
                switch (VertexBefore.TypeOfVertex)
                {
                    case "AMGenerator":
                        VertexAfter.GeneratorType.FirstTime = int.Parse(tBx1.Text);
                        VertexAfter.GeneratorType.Mean = double.Parse(tBx2.Text);
                        VertexAfter.GeneratorType.LengthOfFile = int.Parse(tBx3.Text);
                        switch (cbbDistribution.SelectedIndex)
                        {
                            case 0:
                                VertexAfter.GeneratorType.TypeDistribuion = GeneratorClass.Distribution.NormalDis;
                                VertexAfter.GeneratorType.Para = double.Parse(tBxPara.Text);
                                break;
                            case 1:
                                VertexAfter.GeneratorType.TypeDistribuion = GeneratorClass.Distribution.ExponentialDis;
                                break;
                            default:
                                break;
                        }
                        //if (fromfile)
                        //{
                        //    VertexAfter.ListPointsPDF = lispdf;
                        //    VertexAfter.ListPointsCDF = liscdf;
                        //}
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
                        VertexAfter.TransitionType.Mean = double.Parse(tBx2.Text);
                        //VertexAfter.TransitionType.LengthOfFile = int.Parse(tBx3.Text);
                        switch (cbbDistribution.SelectedIndex)
                        {
                            case 0:
                                VertexAfter.TransitionType.TypeDistribuion = GeneratorClass.Distribution.NormalDis;
                                VertexAfter.TransitionType.Para = double.Parse(tBxPara.Text);
                                break;
                            case 1:
                                VertexAfter.TransitionType.TypeDistribuion = GeneratorClass.Distribution.ExponentialDis;
                                break;
                            default:
                                break;
                        }
                        if (fromfile)
                        {
                            VertexAfter.ListPointsPDF = lispdf;
                            VertexAfter.ListPointsCDF = liscdf;
                        }
                        else
                        {
                            VertexAfter.ListPointsPDF = null;
                            VertexAfter.ListPointsCDF = null;
                        }
                        break;
                    default:
                        break;
                }
                //MessageBox.Show("Saved!");
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

        private void cbbDistribution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cbbDistribution.SelectedIndex == 0)
            //{
            //    DPPara.Visibility = Visibility.Visible;
            //    tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();
            //}
            //if (cbbDistribution.SelectedIndex == 1)
            //{
            //    DPPara.Visibility = Visibility.Collapsed;
            //}
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            info.Visibility = Visibility.Hidden;
            loadfromfile.Visibility = Visibility.Visible;
            btnFromWindow.IsEnabled = true;
            List<double> listxpdf = new List<double>();
            List<double> listypdf = new List<double>();
            List<double> listxcdf = new List<double>();
            List<double> listycdf = new List<double>();
            bool flagdis = false;
            string s="";
            string dis = "[Distribution_function]";
            string patternX = "X=";
            string patternY = "Y=";
            string rpatternX = @"^Point_\d{1,3}_X=";            
            string rpatternY = @"^Point_\d{1,3}_Y=";
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";           

            if (openFileDialog.ShowDialog() == true)
            {
                var ms = Regex.Split(File.ReadAllLines(openFileDialog.FileName).First(x => x.Contains("PointCount")), "PointCount=");
                var mss = Int32.Parse(ms[1]);
                //StreamReader file = File.OpenText(openFileDialog.FileName);
                foreach (var line in File.ReadAllLines(openFileDialog.FileName))
                {
                    flagdis = listypdf.Count == mss;
                    if (!flagdis)
                    {
                        if (line.Contains(patternX))
                        {
                            var ss = Regex.Split(line, rpatternX);
                            listxpdf.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));

                        }

                        if (line.Contains(patternY))
                        {
                            var ss = Regex.Split(line, rpatternY);
                            listypdf.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));

                        }
                    }
                    else
                    {
                        if (line.Contains(patternX))
                        {
                            var ss = Regex.Split(line, rpatternX);
                            listxcdf.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));

                        }

                        if (line.Contains(patternY))
                        {
                            var ss = Regex.Split(line, rpatternY);
                            listycdf.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));

                        }
                    }

                }
            }
            var temppdf = SetListPoint(listxpdf, listypdf);
            var tempcdf = SetListPoint(listxcdf, listycdf);
            UpdateVertex(true, temppdf, tempcdf);
        }

        public List<List<double>> SetListPoint(List<double> listx, List<double> listy)
        {
            var result = new List<List<double>>();
            for (int i = 0; i < listx.Count; i++)
            {
                var temp = new List<double>();
                temp.Add(listx[i]);
                temp.Add(listy[i]);
                result.Add(temp);
            }
            return result;
        }

        private void btnFromWindow_Click(object sender, RoutedEventArgs e)
        {
            info.Visibility = Visibility.Visible;
            loadfromfile.Visibility = Visibility.Collapsed;
            UpdateVertex(false);
        }
        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}
        //Regex rx = new Regex("^Point_[0-9]{1,3}_X=(-)?[0-9]{1,10}(,.)?[a-zA-Z0-9]{1,20}$");
        //Regex ry = new Regex("^Point_[0-9]{1,3}_Y=(-)?[0-9]{1,10}(,.)?[a-zA-Z0-9]{1,20}$");
    }
}
