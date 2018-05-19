using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using React.Distribution;

namespace Test2
{
    class Transition
    {

        public int SoCungVao { get; set; } = 3;
        public int SoCungRa { get; set; } = 3;
        public int EndingTime { get; set; }
        public List<List<int>> Phantich { get; set; }
        //public List<Point> Points { get; set; }

        public Transition(int endingtime)
        {
            EndingTime = endingtime;
        }

        public void Run()
        {
            try
            {
                var TimeKH = 0;
                var TimeNow = 0;
                var TimeNowNext = 0;

                List<Queue<int>> analis = new List<Queue<int>>();

                var SumCung = SoCungVao + SoCungRa;

                int[] arrayDKCungVao = new int[SoCungVao];
                arrayDKCungVao[0] = 3;
                arrayDKCungVao[1] = 2;
                arrayDKCungVao[2] = 2;
                Queue<Customer>[] arrayHDVaoRa = new Queue<Customer>[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHDVaoRa[i] = new Queue<Customer>();
                }

                int[] arrayDKCungRa = new int[SoCungRa];
                arrayDKCungRa[0] = 5;
                arrayDKCungRa[1] = 2;
                arrayDKCungRa[2] = 3;
                var listKH = new List<Customer>();
                var random = new Random();
                Nut[] arrayNuts = new Nut[SoCungVao];
                arrayNuts[0] = new Nut() { TypeDistribuion = Nut.Distribution.ExponentialDis, Interval = 1 };
                arrayNuts[1] = new Nut() { TypeDistribuion = Nut.Distribution.NormalDis, Interval = 2 };
                arrayNuts[2] = new Nut() { TypeDistribuion = Nut.Distribution.NormalDis, Interval = 3 };

                Queue<Customer>[] arrayHD = new Queue<Customer>[SumCung];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHD[i] = new Queue<Customer>();
                }
                for (int i = SoCungVao; i < SumCung; i++)
                {
                    arrayHD[i] = new Queue<Customer>();
                }
                int[] arrayDKCung = new int[SumCung];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayDKCung[i] = arrayDKCungVao[i];
                }
                for (int i = SoCungVao; i < SumCung; i++)
                {
                    arrayDKCung[i] = arrayDKCungRa[i - SoCungVao];
                }
                int[] arrayTimeKH = new int[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayTimeKH[i] = 0;
                }

                do
                {
                    //TimeNowNext = FindMinNextTimePlan(listKH);
                    for (int i = 0; i < SoCungVao; i++)
                    {
                        var cus = SinhMotCusAndName1(i.ToString(), arrayNuts[i], arrayTimeKH[i]);
                            listKH.Add(cus);
                            arrayTimeKH[i] = cus.TimePlan;
                    }
                    listKH = listKH.FindAll(x => x.TimePlan < EndingTime);
                    if (listKH.Count != 0)
                    {
                        TimeNow = FindMinTimePlan(listKH);
                        //Console.WriteLine(TimeNow);
                    }
                    else
                    {
                        break;
                        //TimeNow+=EndingTime;
                        //Console.WriteLine(TimeNow);
                    }
                    var listequaltimenow = listKH.FindAll(x => x.TimePlan == TimeNow);
                    listKH = listKH.FindAll(x => x.TimePlan != TimeNow);

                    foreach (var a in listequaltimenow)
                    {
                        var cus = a;
                        var aa = cus.Name;
                        arrayHD[Int32.Parse(aa)].Enqueue(new Customer(){Name = cus.Name, TimePlan = cus.TimePlan});
                    }
                    Console.WriteLine("timenow " + TimeNow + " " + BLockCount(arrayHD));
                    var lis = new Queue<int>();
                    foreach (var arr in arrayHD)
                    {
                        lis.Enqueue(arr.Count);
                    }
                    analis.Add(lis);
                    
                    if (AlReady(arrayHD, arrayDKCung))
                    {
                        for (int i = 0; i < SoCungVao; i++)
                        {
                            for (int j = 0; j < arrayDKCung[i]; j++)
                            {
                                var cus1 = arrayHD[i].Dequeue();
                                arrayHDVaoRa[i].Enqueue(new Customer() { Name = cus1.Name, TimeIn = TimeNow, TimeOut = TimeNow, TimePlan = cus1.TimePlan });
                            }
                        }
                        for (int i = SoCungVao; i < SumCung; i++)
                        {
                            var ran1 = random.Next(4, 10);
                            for (int j = 0; j < arrayDKCung[i]; j++)
                            {
                                listKH.Add(SinhMotCusWithName(i.ToString(), TimeNow, ran1));
                            }
                        }
                    }
                    //Console.WriteLine("timenow " + TimeNow + " " + BLockCount(arrayHD));
                } while (TimeNow < EndingTime);

                Phantich = ChuyenHang2Cot(analis);
                foreach (var dc in Phantich)
                {
                    foreach (var vf in dc)
                    {
                        Console.Write(vf + " ");
                    }
                    Console.WriteLine();
                }
                foreach (var a in arrayHD)
                {
                    Console.WriteLine(a.Count);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public List<List<int>> ChuyenHang2Cot(List<Queue<int>> list)
        {
            int mm = list.FirstOrDefault().Count;
            
            var aa = new List<List<int>>();
            for (int i = 0; i < mm; i++)
            {
                var nn = new List<int>();
                foreach (var m in list)
                {
                    nn.Add(m.Dequeue());
                }
                aa.Add(nn);
            }            
            return aa;
        }
        private string BLockCount(Queue<Customer>[] listCustomers)
        {
            var str = "";
            foreach (var c in listCustomers)
            {
                str = str + c.Count + "-";
            }
            return str;
        }
        private string BLockCount(List<List<int>> anaList)
        {
            var str = "";
            foreach (var c in anaList)
            {
                foreach (var b in c)
                {
                    str += b.ToString();
                }                
            }
            return str;
        }


        public Customer SinhMotCus(int timenow)
        {
            var ran = new Random();
            var c = new Customer(timenow + ran.Next(1, 5));
            return c;
        }

        public Customer SinhMotCusWithName(string name, int timenow, int ranmax)
        {
            var ran = new Random();
            var c = new Customer(name, timenow + ran.Next(1, ranmax));
            return c;
        }

        public Queue<Customer> SinhCus(int endingTime)
        {
            Queue<Customer> queueCustomers = new Queue<Customer>();
            int stt = 1;
            var now = 0;
            var ran = new Random();
            do
            {

                now += ran.Next(1, 5);
                var c = new Customer(stt.ToString(), now);
                queueCustomers.Enqueue(c);
                stt++;
            } while (now < endingTime);
            return queueCustomers;
        }

        public bool AlReady(Queue<Customer> a, Queue<Customer> b, int min1, int min2)
        {
            return a.Count >= min1 && b.Count >= min2;
        }

        public int FindMinTimePlan(List<Customer> listcus)
        {
            return listcus.Min(c => c.TimePlan);
        }

        public static int FindMinNextTimePlan(List<Customer> listcus)
        {
            var listemp = listcus;
            listemp.RemoveAll(a => a.TimePlan == listemp.Min(c => c.TimePlan));
            return listemp.Min(c => c.TimePlan);
        }

        public Customer CusHasEqualPlanTimeNow(List<Customer> listcus)
        {
            var cus = new Customer();

            return cus;
        }

        public bool AlReady(Queue<Customer>[] listqueuecus, int[] listminmachine)
        {
            var already = true;
            for (int i = 0; i < SoCungVao; i++)
            {
                already = already && listqueuecus[i].Count >= listminmachine[i];
            }
            return already;
        }
        public bool AlReady1(Queue<Customer>[] listqueuecus, int[] listminmachine)
        {
            var already = true;
            for (int i = 0; i < SoCungVao; i++)
            {
                already = already && listqueuecus[i].Count >= listminmachine[i];
            }
            return already;
        }

        public long SinhMotCusAndName(string name, Nut nut)
        {
            switch (nut.TypeDistribuion)
            {
                case Nut.Distribution.NormalDis:
                    nut.TypeDis = new Normal(nut.Interval, 1.0);
                    break;
                case Nut.Distribution.ExponentialDis:
                    nut.TypeDis = new Exponential(nut.Interval);
                    break;
                default:
                    Console.WriteLine("k tim thay");
                    break;
            }

            long d;
            do
            {
                d = (long)nut.TypeDis.NextDouble();
            } while (d <= 0L);
            return d;
        }
        public Customer SinhMotCusAndName1(string name, Nut nut, int timenow)
        {
            switch (nut.TypeDistribuion)
            {
                case Nut.Distribution.NormalDis:
                    nut.TypeDis = new Normal(nut.Interval, 1.0);
                    break;
                case Nut.Distribution.ExponentialDis:
                    nut.TypeDis = new Exponential(nut.Interval);
                    break;
                default:
                    Console.WriteLine("k tim thay");
                    break;
            }

            long d;
            do
            {
                d = (long)nut.TypeDis.NextDouble();
            } while (d <= 0L);
            var c = new Customer(name, timenow + (int)d);
            return c;
        }

        private static bool EpKieuDuoc(Customer cus)
        {
            int number;
            bool result = Int32.TryParse(cus.Name, out number);
            return result;
        }
    }

    public class Nut
    {
        public string Name { get; set; } = ""; //Name of Shop
        public const long ClosingTime = 4 * 60;
        public NonUniform TypeDis { get; set; } //Kieu Distribution

        public enum Distribution
        {
            NormalDis,
            ExponentialDis
        }

        //Nhung Bien set tu giao dien duoc
        public int FirstTime { get; set; } = 0; //Thời điểm bắt đầu mô phỏng

        public double Interval { get; set; } = 5; //Khoang lamda
        public double StdDev { get; set; } = 1;// danh cho normal distribuion
        public int LengthOfFile { get; set; } = 15; //số Customer tối đa       
        public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;
        public int NumBarbers { get; set; } = 1; //So luong may phuc vu

        public int QueueCapacity { get; set; } = 500;
    }
}
//private int DoSomeThing1(Queue<Customer> kehoach1, int TimeNow, Queue<Customer> kehoach2,
//Queue<Customer> hangdoi1, Queue<Customer> hangdoi2, int Min1,
//int Min2, Queue<Customer> thuthap1, Queue<Customer> thuthap2, Queue<Customer> hangdoi3, int EndingTime)
//{
//do
//{
//kehoach1.Enqueue(SinhMotCus(TimeNow));
//kehoach2.Enqueue(SinhMotCus(TimeNow));
//var a = kehoach1.Peek();
//var b = kehoach2.Peek();
//    //TimeNow = Math.Min(a.TimePlan, b.TimePlan);
//    if (a.TimePlan < b.TimePlan)
//{
//    TimeNow = a.TimePlan;
//    kehoach1.Enqueue(SinhMotCus(TimeNow));
//    hangdoi1.Enqueue(kehoach1.Dequeue());
//}
//else if (b.TimePlan < a.TimePlan)
//{
//    TimeNow = b.TimePlan;
//    kehoach2.Enqueue(SinhMotCus(TimeNow));
//    hangdoi2.Enqueue(kehoach2.Dequeue());
//}
//else
//{
//    TimeNow = a.TimePlan;
//    hangdoi1.Enqueue(kehoach1.Dequeue());
//    hangdoi2.Enqueue(kehoach2.Dequeue());
//}
//if (AlReady(hangdoi1, hangdoi2, Min1, Min2))
//{
//HangDoi2ThuThap(Min1, thuthap1, hangdoi1);
//HangDoi2ThuThap(Min2, thuthap2, hangdoi2);

//hangdoi3.Enqueue(SinhMotCus(TimeNow));
//}
//} while (TimeNow < EndingTime);
//return TimeNow;
//}
