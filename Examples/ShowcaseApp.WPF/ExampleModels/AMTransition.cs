using System;
using System.Collections.Generic;
using System.Linq;
using React.Distribution;

namespace SimulationV1.WPF.ExampleModels
{
    class Transition
    {

        public int SoCungVao { get; set; } = 3;
        public int SoCungRa { get; set; } = 3;
        public int EndingTime { get; set; }
        public List<int> ListTimeNow { get; set; }
        public List<List<int>> Phantich { get; set; }

        public int[] ArrayDKCungVao;
        public int[] ArrayDKCungRa;
        public GeneratorClass[] ArrayNuts;
        public GeneratorClass NutTransition { get; set; }
        //public List<Point> Points { get; set; }

        public Transition(int endingtime, int socungvao, int[] arraycungvao, int socungra, int[] arraycungra, GeneratorClass[] mangnguon, GeneratorClass nutchuyen)
        {
            EndingTime = endingtime;
            SoCungVao = socungvao;
            ArrayDKCungVao = arraycungvao;
            SoCungRa = socungra;
            ArrayDKCungRa = arraycungra;
            ArrayNuts = mangnguon;
            NutTransition = nutchuyen;
        }

        public void Run()
        {
            try
            {
                var TimeKH = 0;
                var TimeNow = 0;
                var TimeNowNext = 0;
                var SumCung = SoCungVao + SoCungRa;
                ListTimeNow = new List<int>();
                Phantich = new List<List<int>>();
                List<Queue<int>> analis = new List<Queue<int>>();

                Queue<Customers>[] arrayHDVaoRa = new Queue<Customers>[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHDVaoRa[i] = new Queue<Customers>();
                }
                //list ke hoach chua cac cus se vao he thong 
                var listKH = new List<Customers>();

                #region Tao hang doi cho cac Places -> sau co the tao thanh field arrayHD cho class va tien hanh khoi tao trong Contructor
                Queue<Customers>[] arrayHD = new Queue<Customers>[SumCung];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHD[i] = new Queue<Customers>();
                }
                for (int i = SoCungVao; i < SumCung; i++)
                {
                    arrayHD[i] = new Queue<Customers>();
                }
                #endregion

                #region Tao mang chung dieu kien cung vao, ra -> sau tao thanhf field arrayDKCung cho class va tien hanh khoi tao trong Contructor
                var arrayDKCung = new int[SumCung];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayDKCung[i] = ArrayDKCungVao[i];
                }
                for (int i = SoCungVao; i < SumCung; i++)
                {
                    arrayDKCung[i] = ArrayDKCungRa[i - SoCungVao];
                }
                #endregion

                #region Tao mang timeKh de tim ra thoi diem tiep theo can sinh cus, tranh truong hop 2 cus den cung 1 thoi diem -> tien hanh khoi tao trong Contructor
                int[] arrayTimeKH = new int[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayTimeKH[i] = 0;
                }
                #endregion

                #region Tao mang co cho cac generator
                var arrayboolLengthOfFile = new bool[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayboolLengthOfFile[i] = false;
                }
                #endregion

                #region Tao mang lengoffile tu cac Generator dau vao
                var arrayLengthOfFile = new int[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayLengthOfFile[i] = ArrayNuts[i].LengthOfFile;
                }
                #endregion
                do
                {
                    //TimeNowNext = FindMinNextTimePlan(listKH);
                    if (!AllCusGenerated(arrayboolLengthOfFile))
                    {
                        for (int i = 0; i < SoCungVao; i++)
                        {
                            var cus = SinhMotCusAndName2(i.ToString(), ArrayNuts[i], arrayTimeKH[i]);
                            arrayLengthOfFile[i]--;
                            if (arrayLengthOfFile[i] > 0)
                            {
                                arrayboolLengthOfFile[i] = false;
                                listKH.Add(cus);
                                arrayTimeKH[i] = cus.TimePlan;
                            }
                            else
                            {
                                arrayboolLengthOfFile[i] = true;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    //for (int i = 0; i < SoCungVao; i++)
                    //{
                    //    var cus = SinhMotCusAndName2(i.ToString(), ArrayNuts[i], arrayTimeKH[i]);
                    //    listKH.Add(cus);
                    //    arrayTimeKH[i] = cus.TimePlan;
                    //}
                    listKH = listKH.FindAll(x => x.TimePlan < EndingTime);
                    if (listKH.Count != 0)
                    {
                        TimeNow = FindMinTimePlan(listKH);
                        ListTimeNow.Add(TimeNow);
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
                        arrayHD[Int32.Parse(aa)].Enqueue(new Customers() { Name = cus.Name, TimePlan = cus.TimePlan });
                    }
                    //Console.WriteLine("timenow " + TimeNow + " " + BLockCount(arrayHD));
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
                                arrayHDVaoRa[i].Enqueue(new Customers() { Name = cus1.Name, TimeIn = TimeNow, TimeOut = TimeNow, TimePlan = cus1.TimePlan });
                            }
                        }
                        var d = RandomNumberFromTransition(NutTransition);
                        for (int i = SoCungVao; i < SumCung; i++)
                        {
                            for (int j = 0; j < arrayDKCung[i]; j++)
                            {
                                ////listKH.Add(SinhMotCusWithName(i.ToString(), TimeNow, ran1));
                                //listKH.Add(SinhMotCusAndName2(i.ToString(), NutTransition, TimeNow));
                                listKH.Add(Sinh1Customer(i.ToString(), TimeNow + (int)d));
                            }
                        }
                    }
                    //Console.WriteLine("timenow " + TimeNow + " " + BLockCount(arrayHD));
                } while (TimeNow < EndingTime);

                Phantich = ChuyenHang2Cot(analis);
                //foreach (var dc in Phantich)
                //{
                //    foreach (var vf in dc)
                //    {
                //        Console.Write(vf + " ");
                //    }
                //    Console.WriteLine();
                //}
                //foreach (var a in arrayHD)
                //{
                //    Console.WriteLine(a.Count);
                //}

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private bool AllCusGenerated(bool[] arraylengthoffile)
        {
            var allgenerated = true;
            foreach (var s in arraylengthoffile)
            {
                allgenerated = allgenerated && s;
            }
            return allgenerated;
        }

        public Customers Sinh1Customer(string name, int plantime)
        {
            var c = new Customers(name, plantime);
            return c;
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
        private string BLockCount(Queue<Customers>[] listCustomerss)
        {
            var str = "";
            foreach (var c in listCustomerss)
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

        public Customers SinhMotCus(int timenow)
        {
            var ran = new Random();
            var c = new Customers(timenow + ran.Next(1, 5));
            return c;
        }

        public Customers SinhMotCusWithName(string name, int timenow, int ranmax)
        {
            var ran = new Random();
            var c = new Customers(name, timenow + ran.Next(1, ranmax));
            return c;
        }

        public Queue<Customers> SinhCus(int endingTime)
        {
            Queue<Customers> queueCustomerss = new Queue<Customers>();
            int stt = 1;
            var now = 0;
            var ran = new Random();
            do
            {

                now += ran.Next(1, 5);
                var c = new Customers(stt.ToString(), now);
                queueCustomerss.Enqueue(c);
                stt++;
            } while (now < endingTime);
            return queueCustomerss;
        }

        public bool AlReady(Queue<Customers> a, Queue<Customers> b, int min1, int min2)
        {
            return a.Count >= min1 && b.Count >= min2;
        }

        public int FindMinTimePlan(List<Customers> listcus)
        {
            return listcus.Min(c => c.TimePlan);
        }

        public static int FindMinNextTimePlan(List<Customers> listcus)
        {
            var listemp = listcus;
            listemp.RemoveAll(a => a.TimePlan == listemp.Min(c => c.TimePlan));
            return listemp.Min(c => c.TimePlan);
        }

        public Customers CusHasEqualPlanTimeNow(List<Customers> listcus)
        {
            var cus = new Customers();

            return cus;
        }

        public bool AlReady(Queue<Customers>[] listqueuecus, int[] listminmachine)
        {
            var already = true;
            for (int i = 0; i < SoCungVao; i++)
            {
                already = already && listqueuecus[i].Count >= listminmachine[i];
            }
            return already;
        }
        public bool AlReady1(Queue<Customers>[] listqueuecus, int[] listminmachine)
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
        public Customers SinhMotCusAndName1(string name, Nut nut, int timenow)
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
            var c = new Customers(name, timenow + (int)d);
            return c;
        }

        public Customers SinhMotCusAndName2(string name, GeneratorClass nut, int timenow)
        {
            var d = RandomNumberFromTransition(nut);
            var c = new Customers(name, timenow + (int)d);
            return c;
        }

        private static long RandomNumberFromTransition(GeneratorClass nut)
        {
            switch (nut.TypeDistribuion)
            {
                case GeneratorClass.Distribution.NormalDis:
                    nut.TypeDis = new Normal(nut.Interval, 1.0);
                    break;
                case GeneratorClass.Distribution.ExponentialDis:
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

        private static bool EpKieuDuoc(Customers cus)
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
        public int LengthOfFile { get; set; } = 30; //số Customers tối đa       
        public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;
        public int NumBarbers { get; set; } = 1; //So luong may phuc vu

        public int QueueCapacity { get; set; } = 500;
    }
}
//private int DoSomeThing1(Queue<Customers> kehoach1, int TimeNow, Queue<Customers> kehoach2,
//Queue<Customers> hangdoi1, Queue<Customers> hangdoi2, int Min1,
//int Min2, Queue<Customers> thuthap1, Queue<Customers> thuthap2, Queue<Customers> hangdoi3, int EndingTime)
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
