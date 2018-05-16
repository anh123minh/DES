using System;
using React;

namespace TestDES
{
    class Program
    {
        static void Main(string[] args)
        {
            //Shop shop = new Shop();
            //Task generator = new Process(shop, shop.Generator);
            //shop.Run(generator);
            Shop shop1 = new Shop(2);
            var nut = new Nut();
            Task[] genTasks = new Task[2];
            //for (int i = 0; i < 2; i++)
            //{
            //    genTasks[i] = new Process(shop1, shop1.Generator,nut);
            //}
            genTasks[0] = new Process(shop1, shop1.SinhCus, new Nut() { Interval = 5, Name = "0", TypeDistribuion = Nut.Distribution.NormalDis, NumBarbers = 3 });
            genTasks[1] = new Process(shop1, shop1.SinhCus, new Nut() { Interval = 10, Name = "1", TypeDistribuion = Nut.Distribution.ExponentialDis, NumBarbers = 2 });
            shop1.Run(genTasks);

            //Shop shop2 = new Shop();
            //Task gen = new Process(shop2, shop2.SinhCus, new Nut() { Interval = 5, Name = "1", TypeDistribuion = Nut.Distribution.NormalDis, NumBarbers = 3, Sim = shop2 });
            //shop2.Run(gen);
            Console.ReadKey();
        }
    }
}
