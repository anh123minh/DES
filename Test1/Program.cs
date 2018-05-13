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
            //Shop shop = new Shop();
            //Task generator = new Process(shop, shop.Generator);
            //shop.Run(generator);
            Shop shop1 = new Shop();
            var nut = new Nut();
            Task[] genTasks = new Task[2];
            //for (int i = 0; i < 2; i++)
            //{
            //    genTasks[i] = new Process(shop1, shop1.Generator,nut);
            //}
            genTasks[0] = new Process(shop1, shop1.SinhCus, new Nut() { Interval = 5, Name = "0", TypeDistribuion = Nut.Distribution.NormalDis});
            genTasks[1] = new Process(shop1, shop1.SinhCus, new Nut() { Interval = 10, Name = "1", TypeDistribuion = Nut.Distribution.ExponentialDis});
            shop1.Run(genTasks);
            Console.ReadKey();
        }
    }
}
