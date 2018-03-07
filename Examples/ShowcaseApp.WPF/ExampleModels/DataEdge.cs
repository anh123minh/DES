using System.ComponentModel;
using GraphX;
using System;
using GraphX.Measure;
using GraphX.PCL.Common.Models;
using YAXLib;
using System.Windows.Media;
using SimulationV1.WPF.FileSerialization;

namespace SimulationV1.WPF
{
    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        [YAXCustomSerializer(typeof(YAXPointArraySerializer))]
        public override Point[] RoutingPoints { get; set; }

        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
			: base(source, target, weight)
		{
            Angle = 90;
		}

        public DataEdge()
            : base(null, null, 1)
        {
            Angle = 90;
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        /// <summary>
        /// Node main description (header)
        /// </summary>
        private string _text;
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }
        public string ToolTipText {get; set; }
        private string _color="Green";
        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }
       
        public override string ToString()
        {
            return Text;
        }
        private double _capacity = 100;
        public double Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }
        private double _load = 0;
        public double Load
        {
            get { return _load; }
            set { _load = value; }
        }
        private double _alpha = 0;
        public double Alpha
        {
            get { return (Load/Capacity)*100; }
            set { _alpha = value; }
        }
        //AM
        public string TypeOfEdge { get; set; } = "";
        public int Probability { get; set; } = 0;
        public int Delay { get; set; } = 0;


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
