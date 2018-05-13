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
            genTasks[0] = new Process(shop1, shop1.Generator);
            genTasks[1] = new Process(shop1, shop1.Generator1);
            shop1.Run(genTasks);
            Console.ReadKey();
        }
        public class Nut
        {
            public const long ClosingTime = 4 * 60;
            //public NonUniform TypeDis { get; set; }//Kieu Distribution
            public enum Distribution
            {
                NormalDis,
                ExponentialDis
            }
            //Nhung Bien set tu giao dien duoc
            public int FirstTime { get; set; } = 0;//Thời điểm bắt đầu mô phỏng
            public double Interval { get; set; } = 5;//Khoang lamda
            public int LengthOfFile { get; set; } = 15;//số Customer tối đa       
            public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;

            public int QueueCapacity { get; set; } = 500;

            //Bien dung trong tinh toan
            public bool IsReady { get; set; } = false;//San sang de thuc thi hay chua
        }
    }
}
