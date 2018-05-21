using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    class Customers
    {
        public string Name { get; set; } = "";
        public int TimePlan { get; set; } = 0;// = TimeCome
        public int TimeIn { get; set; } = 0;
        public int TimeOut { get; set; } = 0;

        public int TimeStayQueue => TimeOut - TimeIn;
        public int TimeStaySystem => TimeOut - TimePlan;

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

        public Customers(int timeplan, int timein, int timeout)
        {
            TimePlan = timeplan;
            TimeIn = timein;
            TimeOut = timeout;
        }

        public override string ToString()
        {
            return "Name " + Name + "- TimePlan " + TimePlan + "- TimeIn " + TimeIn + "- TimeOut " + TimeOut + "- TimeStayQueue " + TimeStayQueue + "- TimeStaySystem " + TimeStaySystem;
        }
    }
}
