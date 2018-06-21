namespace SimulationV1.WPF.ExampleModels
{
    public class Chips
    {
        public string Name { get; set; } = "";//Имя вершины, которая создала метку
        public string FromType { get; set; } = "";//Тип вершины, которая создала метку
        public int TimePlan { get; set; } = 0;//Момент времени, в который метка поступает в систему
        public int TimeIn { get; set; } = 0;//Момент времени, в который метка поступает в обслуживание
        public int TimeOut { get; set; } = 0;//Момент времени, в который метка покидает систему
        public int TimeStayQueue => TimeOut - TimeIn > 0 ? TimeOut - TimeIn : 0;//время пребывания метки в очереди
        public int TimeStaySystem => TimeOut - TimePlan > 0 ? TimeOut - TimePlan : 0;//время пребывания метки в системе

        public Chips()
        {           
        }
        public Chips(int timeplan)
        {
            TimePlan = timeplan;
        }

        public Chips(string name, int timeplan)
        {
            Name = name;
            TimePlan = timeplan;
        }
        public Chips(string name, string fromtype, int timeplan)
        {
            Name = name;
            FromType = fromtype;
            TimePlan = timeplan;
        }

        public Chips(int timeplan, int timein, int timeout)
        {
            TimePlan = timeplan;
            TimeIn = timein;
            TimeOut = timeout;
        }

        public override string ToString()
        {
            return "TimePlan " + TimePlan + " - Name " + Name + "- FromType " + FromType
                +  "- TimeIn " + TimeIn + "- TimeOut " + TimeOut + "- TimeStayQueue " + TimeStayQueue + "- TimeStaySystem " + TimeStaySystem;
        }
    }
}
