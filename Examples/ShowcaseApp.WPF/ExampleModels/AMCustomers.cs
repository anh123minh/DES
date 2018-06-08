namespace SimulationV1.WPF.ExampleModels
{
    public class Customers
    {
        public string Name { get; set; } = "";
        public string FromType { get; set; } = "";
        public int TimePlan { get; set; } = 0;// = TimeCome
        public int TimeIn { get; set; } = 0;
        public int TimeOut { get; set; } = 0;

        public int TimeStayQueue => TimeOut - TimeIn > 0 ? TimeOut - TimeIn : 0;
        public int TimeStaySystem => TimeOut - TimePlan > 0 ? TimeOut - TimePlan : 0;

        public Customers()
        {
            
        }

        public Customers(int timeplan)
        {
            TimePlan = timeplan;
        }

        public Customers(string name, int timeplan)
        {
            Name = name;
            TimePlan = timeplan;
        }
        public Customers(string name, string fromtype, int timeplan)
        {
            Name = name;
            FromType = fromtype;
            TimePlan = timeplan;
        }

        public Customers(int timeplan, int timein, int timeout)
        {
            TimePlan = timeplan;
            TimeIn = timein;
            TimeOut = timeout;
        }

        public override string ToString()
        {
            return "Name " + Name + "- FromType " + FromType + "- TimePlan " + TimePlan + "- TimeIn " + TimeIn + "- TimeOut " + TimeOut + "- TimeStayQueue " + TimeStayQueue + "- TimeStaySystem " + TimeStaySystem;
        }
    }
}
