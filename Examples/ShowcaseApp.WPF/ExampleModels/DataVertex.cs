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

        

        public string Name { get; set; }
        public string Profession { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int ImageId { get; set; }
        
        public bool IsBlue { get; set; }

        public CreateClass CreateType { get; set; }
        public QueueClass QueueType { get; set; }
        public TerminateClass TerminateType { get; set; }
        public AccumulateClass Anew { get; set; }

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

    public class CreateClass : Simulation
    {
        private NonUniform TypeDis { get; set; }//Kieu Distribution
        public enum Distribution
        {
            NormalDis,
            ExponentialDis
        }
        //Nhung tham so set tu giao dien duoc
        public int FirstTime { get; set; } = 0;//Thời điểm bắt đầu mô phỏng
        public double Interval { get; set; } = 5;//Khoang lamda
        public int LengthOfFile { get; set; } = 15;//số Customer tối đa       
        public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;

        public PointCollection Points { get; set; } = new PointCollection(){new Point(0,0)};

        //public Point point { get; set; }
                        
        public int Priority { get; set; } = 0;//Thứ tự ưu tiên
        public int FileType { get; set; } = 0;//dạng ưu tiên
        public Resource ABarbers { get; set; }//Khai báo các máy phục vụ
        
        //Nhung tham so set tu cac Vertex khac
        public int QueueCapacity { get; set; } = 500;// = QueueCapacity
        public long EndingTime { get; set; } = 240;//Thời gian kết thúc, sau sẽ được truyện vào từ Terminate 
        
        //Method sinh Cus
        public IEnumerator<Task> Generator(Process p, object data)
        {
            Console.WriteLine(this.Now + @" The barber shop is opening for business...");
            //Resource barbers = CreateBarbers();
            ABarbers = Resource.Create(new List<Barber>() { new Barber(this, "Minh"), new Barber(this, "Anh") });
            int i = 0;
            switch (TypeDistribuion)
            {
                case Distribution.NormalDis:
                    TypeDis = new Normal(Interval, 1.0);
                    break;
                case Distribution.ExponentialDis:
                    TypeDis = new Exponential(Interval);
                    break;
                default:
                    Console.WriteLine("k tim thay");
                    break;
            }

            do
            {
                long d;
                do
                {
                    d = (long)TypeDis.NextDouble();
                } while (d <= 0L);
                if (FirstTime != 0 && Now == 0)
                {
                    yield return p.Delay(FirstTime);
                    i++;
                    //Console.WriteLine(@"xxx         so Cus trong hang doi = " + ABarbers.BlockCount + " " + Now);
                    Customer c = new Customer(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, ABarbers);
                    Console.WriteLine(this.Now + " The customer " + c.Name + " come");
                }
                else
                {
                    yield return p.Delay(d);                   
                    Console.WriteLine(@"xxx         BlockCount - " + ABarbers.BlockCount + "- OutOfService - " + ABarbers.OutOfService + "- Reserved - " + ABarbers.Reserved + "- Now - " + Now);
                    i++;
                    Customer c = new Customer(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, ABarbers);
                    Console.WriteLine(this.Now + " The customer " + c.Name + " come");
                    Console.WriteLine(@"yyy         BlockCount - " + ABarbers.BlockCount + "- OutOfService - " + ABarbers.OutOfService + "- Reserved - " + ABarbers.Reserved + "- Now - " + Now);
                    //point = new Point(Now, ABarbers.BlockCount);
                    Points.Add(new Point(Now, ABarbers.BlockCount));
                }

            } while (Now < EndingTime && i < LengthOfFile);

            Console.WriteLine(@"======================================================");
            Console.WriteLine(@"The barber shop is closed for the day.");

            if (ABarbers.BlockCount > 0)
            {
                Console.WriteLine(@"The barbers have to work late today.");
            }

            yield break;
        }

    }
    public class QueueClass
    {
        public int QueueCapacity { get; set; } = 500;//Cần tìm thuộc tính liên quan đến số Customer có thể chứa trong hàng đợi
        public int Priority { get; set; } = 0;
        public int FileType { get; set; } = 0;
    }
    public class TerminateClass
    {
        public int OutputCounter { get; set; } = 100;//Số Customer xử lý được là dừng, trường hợp này chưa xem xét tới
        public int StoppingTime { get; set; }//bằng với EndingTime trong CreateClass
    }
    public class AccumulateClass
    {
        public int Acc1 { get; set; }
        public int Acc2 { get; set; }
    }

}
