using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarberShop;
using React;


namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop();
            Task generator = new Process(shop, shop.Generator);
            shop.Run(generator);
            Console.ReadKey();
        }
    }
}
