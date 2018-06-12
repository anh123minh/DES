using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using DevExpress.Mvvm.Native;

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for WindowParaVertex.xaml
    /// </summary>
    public partial class WindowParaVertex : Window
    {
        public delegate void SetValueForm(DataVertex vtx);
        public SetValueForm SetValueControl;
        public DataVertex VertexBefore;
        public DataVertex VertexAfter;

        public WindowParaVertex()
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

                        tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();

                    tBxName.Text = VertexBefore.Text;
                    tBxTraffic.Text = VertexBefore.Traffic.ToString();
                    DP1.Visibility = Visibility.Visible;
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.FirstMark.ToString();
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
                    //Label1.Content = "Ёмкость очереди:";
                    //tBx1.Text = VertexBefore.PlaceType.QueueCapacity.ToString();
                    //tBx1.IsEnabled = false;//
                    Label1.Content = "Начало:";
                    tBx1.Text = VertexBefore.FirstMark.ToString();
                    Label2.Content = "Приоритет:";
                    tBx2.Text = VertexBefore.PlaceType.Priority.ToString();
                    tBx2.IsEnabled = false;//
                    DP3.Visibility = Visibility.Visible;
                    Label3.Content = "Тип файла:";
                    tBx3.Text = VertexBefore.PlaceType.FileType.ToString();
                    tBx3.IsEnabled = false;//
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
                    cbbDistribution.Visibility = Visibility.Visible;
                    cbbDistribution.SelectedIndex = (int)VertexBefore.TransitionType.TypeDistribuion;
                    tBxPara.Text = VertexBefore.TransitionType.Para.ToString();
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
                    if (VertexBefore.TransitionType.PathFullFile != "" && File.Exists(VertexBefore.TransitionType.PathFullFile))
                    {
                        info.IsEnabled = false;
                        LbLoadfromfile.Visibility = Visibility.Visible;
                        btnFromWindow.IsEnabled = true;
                    }
                    break;
                default:
                    MessageBox.Show("Тип узлы не определен!");
                    break;
            }


        }
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateVertex();
            EditorGraph graph = new Pages.EditorGraph();
            graph.vertexSelected = VertexAfter;
            MessageBox.Show("Сохранены!", "Заявление", MessageBoxButton.OK, MessageBoxImage.Information );
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tBxName.Text = VertexBefore.Text;
            tBxTraffic.Text = VertexBefore.Traffic.ToString();

            switch (VertexBefore.TypeOfVertex)
            {
                case "AMGenerator":
                    tBx1.Text = VertexBefore.FirstMark.ToString();
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
                            tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();
                            break;
                        default:
                            break;
                    }
                    break;
                case "AMPlace":
                    tBx1.Text = VertexBefore.FirstMark.ToString();
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
                            tBxPara.Text = VertexBefore.GeneratorType.Para.ToString();
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
        private void UpdateVertex(bool fromfile = false, List<List<double>> lispdf = null, List<List<double>> liscdf = null, string namefile = "")
        {
            try
            {
                VertexAfter.Text = tBxName.Text;
                VertexAfter.Traffic = double.Parse(tBxTraffic.Text);
                switch (VertexBefore.TypeOfVertex)
                {
                    case "AMGenerator":
                        VertexAfter.FirstMark = int.Parse(tBx1.Text);
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
                                VertexAfter.GeneratorType.Para = double.Parse(tBxPara.Text);
                                break;
                            default:
                                break;
                        }
                        //if (fromfile)
                        //{
                        //    VertexAfter.TransitionType.TListPointsPDF = lispdf;
                        //    VertexAfter.TransitionType.TListPointsCDF = liscdf;
                        //}
                        break;
                    case "AMPlace":
                        VertexAfter.FirstMark = int.Parse(tBx1.Text);
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
                                VertexAfter.TransitionType.Para = double.Parse(tBxPara.Text);
                                break;
                            default:
                                break;
                        }
                        if (fromfile)
                        {
                            VertexAfter.TransitionType.TListPointsPDF = lispdf;
                            VertexAfter.TransitionType.TListPointsCDF = liscdf;
                            VertexAfter.TransitionType.PathFullFile = System.IO.Path.GetFullPath(namefile);
                            //VertexAfter.TransitionType.PathFullFile = System.IO.Path.GetFileName(namefile);
                        }
                        else
                        {
                            VertexAfter.TransitionType.TListPointsPDF.Clear();
                            VertexAfter.TransitionType.TListPointsCDF.Clear();
                            VertexAfter.TransitionType.PathFullFile = "";
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VertexAfter.TypeOfVertex == "AMTransition" && VertexAfter.TransitionType.PathFullFile != "" && File.Exists(VertexAfter.TransitionType.PathFullFile))
                {
                    var pdf = "[Probability_density]";
                    var cdf = "[Distribution_function]";
                    var filename = VertexAfter.TransitionType.PathFullFile;
                    var temppdf = SetDistributionFromFile(VertexAfter, pdf, filename);
                    var tempcdf = SetDistributionFromFile(VertexAfter, cdf, filename);
                    UpdateVertex(true, temppdf, tempcdf, filename);
                    //SetFromFile(VertexBefore);
                }
                else
                {
                    UpdateVertex();
                }
                var amGraph = new AMGraph(VertexAfter);
                amGraph.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                //UpdateVertex();
            }
        }

        private void cbbDistribution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////LabelPara = new Label();
            //if (cbbDistribution.SelectedIndex == 0)
            //{
            //    LabelPara1.Text = "Дисперсия:";
            //}
            //if (cbbDistribution.SelectedIndex == 1)
            //{
            //    LabelPara1.Text = "Лямбда:";
            //}
            ////MessageBox.Show("aaa");
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                            
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "GPD files (*.gpd)|*.gpd|All files (*.*)|*.*";                           

                if (openFileDialog.ShowDialog() == true)
                {
                    info.IsEnabled = false;
                    LbLoadfromfile.Visibility = Visibility.Visible;
                    btnFromWindow.IsEnabled = true;

                    var pdf = "[Probability_density]";
                    var cdf = "[Distribution_function]";
                    var filename = VertexBefore.TransitionType.PathFullFile != "" && VertexBefore.TransitionType.PathFullFile == openFileDialog.FileName
                        ? VertexBefore.TransitionType.PathFullFile
                        : openFileDialog.FileName;
                    var temppdf = SetDistributionFromFile(VertexBefore, pdf, filename);
                    var tempcdf = SetDistributionFromFile(VertexBefore, cdf, filename);
                    UpdateVertex(true, temppdf, tempcdf, filename);
                    //SetFromFile(VertexBefore, openFileDialog.FileName);
                }                
            }
            catch (Exception exception)
            {
                MessageBox.Show("Проверите файл!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                //MessageBox.Show(exception.ToString());
            }            
        }

        public static List<List<double>> SetDistributionFromFile(DataVertex vertex, string dis, string filename)
        {
            List<double> listdistriX = new List<double>();
            List<double> listdistriY = new List<double>();
            string patternX = "X=";
            string patternY = "Y=";
            string rpatternX = @"^Point_\d{1,3}_X=";
            string rpatternY = @"^Point_\d{1,3}_Y=";

            //tim dong PointCount -> Spilt -> lay phan tu thu 2 -> Chuyen sang int => so diem
            var ms = Int32.Parse(Regex.Split(File.ReadAllLines(filename).First(x => x.Contains("PointCount")), "PointCount=")[1]);
            //doc toan bo file <- bo qua cac dong ma tu do bat dap dis
            foreach (var line in File.ReadAllLines(filename).Skip(File.ReadAllLines(filename).IndexOf(x=>x == dis)))
            {
                if (line.Contains(patternX))
                {
                    var ss = Regex.Split(line, rpatternX);
                    listdistriX.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));
                }
                if (line.Contains(patternY))
                {
                    var ss = Regex.Split(line, rpatternY);
                    listdistriY.Add(Double.Parse(ss[1], System.Globalization.NumberStyles.Float));
                }
                if(listdistriY.Count == ms) { break;}// neu da du so diem thi k doc tiep maf thoat
            }
            return SetListPoint(listdistriX, listdistriY);
        }
        public void SetFromFile(DataVertex vertex, string openFileDialog = "")
        {
            List<double> listxpdf = new List<double>();
            List<double> listypdf = new List<double>();
            List<double> listxcdf = new List<double>();
            List<double> listycdf = new List<double>();
            string s = "";
            string dis = "[Distribution_function]";
            string patternX = "X=";
            string patternY = "Y=";
            string rpatternX = @"^Point_\d{1,3}_X=";
            string rpatternY = @"^Point_\d{1,3}_Y=";
            string filename = "";
            filename = vertex.TransitionType.PathFullFile != ""
                ? vertex.TransitionType.PathFullFile
                : openFileDialog;
            var ms = Regex.Split(File.ReadAllLines(filename).First(x => x.Contains("PointCount")), "PointCount=");
            var mss = Int32.Parse(ms[1]);
            foreach (var line in File.ReadAllLines(filename))
            {
                var flagdis = listypdf.Count == mss;
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
            var temppdf = SetListPoint(listxpdf, listypdf);
            var tempcdf = SetListPoint(listxcdf, listycdf);
            UpdateVertex(true, temppdf, tempcdf, filename);
        }
        public static List<List<double>> SetListPoint(List<double> listx, List<double> listy)
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
            info.IsEnabled = true;
            LbLoadfromfile.Visibility = Visibility.Collapsed;
            UpdateVertex();
        }
        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}
        //Regex rx = new Regex("^Point_[0-9]{1,3}_X=(-)?[0-9]{1,10}(,.)?[a-zA-Z0-9]{1,20}$");
        //Regex ry = new Regex("^Point_[0-9]{1,3}_Y=(-)?[0-9]{1,10}(,.)?[a-zA-Z0-9]{1,20}$");
    }
}
