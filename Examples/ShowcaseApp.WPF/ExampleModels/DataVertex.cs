using GraphX.PCL.Common.Models;
using System.Collections.Generic;
namespace Simulation.WPF
{
    public class DataVertex: VertexBase
    {
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        private string _text;
        public string Name { get; set; }
        public string Profession { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int ImageId { get; set; }
        private string _type="";
        public string TypeOfVertex // Center or VLB or Router or IP
        {
            get { return _type; }
            set { _type = value; }
        }
        public bool IsBlue { get; set; }
        private int _number;
        public int Number  // Порядковый номер элемента
        {
            get { return _number ; }
            set { _number = value; }
        }

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
        private List<IEnumerable<DataEdge>> _listpath = null;
        public List<IEnumerable<DataEdge>> ListPath
        {
            get { return _listpath; }
            set { _listpath = value; }
        }
        private double _traffic=0;
        public double Traffic
        {
            get { return _traffic; }
            set { _traffic = value; }
        }
        
      //  private int[,] _matrixload ;
      //  public int[,] MatrixLoad { get; set; }
      //  {
     //      get { return _matrixload; }
      //     set { _matrixload = value; }
      //  }
    }
}
