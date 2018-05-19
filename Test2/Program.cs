using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using React.Distribution;

namespace Test2
{
    class Program
    {
        static void Main(string[] args)
        {
            var trans = new Transition(50);

            trans.Run();

            Console.ReadKey();
        }
    }
}
