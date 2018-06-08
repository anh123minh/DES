using GraphX.PCL.Common.Models;
using System.Collections.Generic;
using React;
using React.Distribution;
using SimulationV1.WPF.ExampleModels;
using System;
using System.Windows;
using System.Windows.Media;
using AForge.Math.Geometry;

namespace SimulationV1.WPF
{
    public class DataVertex: VertexBase
    {
        private string _type = "";
        public string TypeOfVertex // Center or VLB or Router or IP
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Text { get; set; }//Tên của Vertex 
        public int Number { get; set; }// Порядковый номер элемента
        public List<IEnumerable<DataEdge>> ListPath { get; set; } = null;//Danh sách gán theo Vertex, có thế là danh sách các Edge
        public double Traffic { get; set; } = 0;//Traffic của Vertex 

        public List<DataEdge> ListEdgesTarget = new List<DataEdge>();//Danh sach cac edge ket thuc la vertex nay
        public List<DataEdge> ListEdgesSorce = new List<DataEdge>();//Danh sach cac edge bat dau la vertex nay
        public List<DataVertex> ListEdgesTargetVertex = new List<DataVertex>();//Danh sach cac vertex bat dau cua edge ket thuc la vertex nay
        public List<DataVertex> ListEdgesSorceVertex = new List<DataVertex>();//Danh sach cac vertex bat dau cua edge bat dau la vertex nay
        public int[] Mangdkcungvao;
        public int[] Mangdkcungra;
        public Queue<Customers> HdCustomerses = new Queue<Customers>();//hang doi de them cus vao
        public Queue<Customers> HdCustomersesPhantich = new Queue<Customers>();// de sau dung phan tich// co le k can thiet

        public List<List<double>> ListPointsPDF = new List<List<double>>();//f(x)
        public List<List<double>> ListPointsCDF = new List<List<double>>();//F(x)

        public string Name { get; set; }
        public string Profession { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int ImageId { get; set; }
        
        public bool IsBlue { get; set; }

        public GeneratorClass GeneratorType { get; set; } //= new GeneratorClass();
        public PlaceClass PlaceType { get; set; }
        public TerminateClass TerminateType { get; set; }
        public TransitionClass TransitionType { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }

        #endregion

        /// <summary>
        /// Default constructor for this class
        /// (required for serialization).
        /// </summary>
        public DataVertex():this(string.Empty)
        {
        }
        public DataVertex(string text = "")
        {
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;            
        }
        public DataVertex(string text = "",string type="")
        {
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;
            TypeOfVertex = string.IsNullOrEmpty(type) ? "No type" : type;
        }


    }

    public class GeneratorClass //: Simulation
    {
        public NonUniform TypeDis { get; set; }//Kieu Distribution
        public enum Distribution
        {
            NormalDis,
            ExponentialDis
        }
        //Nhung Bien set tu giao dien duoc
        public int FirstTime { get; set; } = 0;//Thời điểm bắt đầu mô phỏng
        public double Mean { get; set; } = 0;//mean --Khoang lamda
        public double Para { get; set; } = 2;//Lamda or Variance  --Normal dung Standard Deviation nen nho can 2
        public int LengthOfFile { get; set; } = 20;//số Customer tối đa       
        public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;
        //---------------------------


    }
    public class PlaceClass
    {
        //Bien truyen tu ngoai vao
        public int QueueCapacity { get; set; } = 500;//Cần tìm thuộc tính liên quan đến số Customer có thể chứa trong hàng đợi
        public int Priority { get; set; } = 0;
        public int FileType { get; set; } = 0;

        //Bien dung trong tinh toan
        public bool IsReady { get; set; } = false;//San sang de thuc thi hay chua
    }
    public class TerminateClass
    {
        public int OutputCounter { get; set; } = 100;//Số Customer xử lý được là dừng, trường hợp này chưa xem xét tới
        public int StoppingTime { get; set; }//bằng với EndingTime trong GeneratorClass
    }
    public class TransitionClass : GeneratorClass
    {
        public Queue<int> ListRandomCome { get; set; } = new Queue<int>();//Danh sach random nhan vao tu bang ngoai
        public int NumberEdgesIn { get; set; } = 0;
        public int NumberEdgesOut { get; set; } = 0;
        public bool AllReady { get; set; } = false;
        public string PathFullFile { get; set; } = "";//ten file load data
        public List<List<double>> TListPointsPDF = new List<List<double>>();//f(x)
        public List<List<double>> TListPointsCDF = new List<List<double>>();//F(x)
    }

    public interface IQueueClass
    {
        //Bien truyen tu ngoai vao
        int QueueCapacity { get; set; } //Cần tìm thuộc tính liên quan đến số Customer có thể chứa trong hàng đợi
        int Priority { get; set; }
        int FileType { get; set; }
    }
    public interface ITerminateClass
    {
        int OutputCounter { get; set; }//Số Customer xử lý được là dừng, trường hợp này chưa xem xét tới
        int StoppingTime { get; set; }//bằng với EndingTime trong GeneratorClass
    }
    public interface IAndClass : ICreateClass
    {
        int NumberEdgesIn { get; set; }
        int NumberEdgesOut { get; set; }
        bool AllReady { get; set; }
    }
    public interface ICreateClass
    {
        NonUniform TypeDis { get; set; }//Kieu Distribution
        Distribution TypeDistribuion { get; set; }
        //Nhung Bien set tu giao dien duoc
        int FirstTime { get; set; }//Thời điểm bắt đầu mô phỏng
        double Interval { get; set; }//Khoang lamda
        int LengthOfFile { get; set; }//số Customer tối đa       
    }
    public enum Distribution
    {
        NormalDis,
        ExponentialDis
    }
}
