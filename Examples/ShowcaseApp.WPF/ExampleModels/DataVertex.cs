using GraphX.PCL.Common.Models;
using System.Collections.Generic;
namespace Simulation.WPF
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

        //  private int[,] _matrixload ;
      //  public int[,] MatrixLoad { get; set; }
      //  {
     //      get { return _matrixload; }
      //     set { _matrixload = value; }
      //  }
    }

    public class CreateClass
    {
        public int FirstTime { get; set; }
        public int Interval { get; set; }
        public int LengthOfFile { get; set; }
        public int Priority { get; set; }
        public int FileType { get; set; }

    }
    public class QueueClass
    {
        public int QueueCapacity { get; set; }
        public int Priority { get; set; }
        public int FileType { get; set; }
    }
    public class TerminateClass
    {
        public int OutputCounter { get; set; }
        public int StoppingTime { get; set; }
    }
    public class AccumulateClass
    {
        public int Acc1 { get; set; }
        public int Acc2 { get; set; }
    }

}
