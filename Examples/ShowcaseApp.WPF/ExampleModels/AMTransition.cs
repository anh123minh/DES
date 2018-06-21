using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using React.Distribution;
using SimulationV1.WPF.Pages;

namespace SimulationV1.WPF.ExampleModels
{
    public class Transition
    {
        public int SoCungVao { get; set; } = 3;
        public int SoCungRa { get; set; } = 3;
        public int EndingTime { get; set; }
        public List<int> ListTimeNowTable { get; set; }
        public List<List<int>> PhantichTable { get; set; }
        public List<int> ListTimeNowGraph { get; set; }//dung cho ve do thi
        public List<List<int>> PhantichGraph { get; set; }
        public List<List<int>> ListptTable = new List<List<int>>();//can xem lai

        public int[] ArrayDkCungVao;
        public int[] ArrayDkCungRa;
        public GeneratorClass[] ArrayNguon;
        public TransitionClass NutTransition { get; set; }

        //moi
        public int EndingTime2 { get; set; }
        public GeneratorClass[] ArrayNguon2;
        public TransitionClass[] ArrayTransitions2;
        public int TimeNow2 = 0;
        public int TimeKh2 = 0;
        public int TimeNowNext2 = 0;
        public int LastTime2 = 0;//la thoi diem cuoi cung trong mo phong khi co bat cu Gen nao sinh het cus va time du kien < timeend
        public List<int> ListTimeNowTable2 { get; set; }
        public List<List<int>> PhantichTable2 { get; set; }
        //moi moi
        public int EndingTime1 { get; set; }
        public DataVertex[] ArrayNguon1;
        public DataVertex[] ArrayTransitions1;
        public int TimeNow1 = 0;
        public int TimeKh1 = 0;
        public int TimeNowNext1 = 0;
        public int LastTime1 = 0;//la thoi diem cuoi cung trong mo phong khi co bat cu Gen nao sinh het cus va time du kien < timeend
        public List<int> ListTimeNowTable1 { get; set; }
        public Lines LineTimeNowTable1 { get; set; }
        public List<List<int>> PhantichTable1 { get; set; }
        public List<Lines> LinePhanTichTable1 { get; set; }
        public List<Chips> ListKh1 = new List<Chips>();//list ke hoach chua cac cus se vao he thong 
        public int Count = 0;//dem so lan kich khoat trong toan bo he thong

        //public Transition(int endingtime, int socungvao, int[] arraycungvao, int socungra, int[] arraycungra, GeneratorClass[] mangnguon, TransitionClass nutchuyen)
        //{
        //    EndingTime = endingtime;
        //    SoCungVao = socungvao;
        //    ArrayDkCungVao = arraycungvao;
        //    SoCungRa = socungra;
        //    ArrayDkCungRa = arraycungra;
        //    ArrayNguon = mangnguon;
        //    NutTransition = nutchuyen;
        //}

        //public Transition(int endingtime, GeneratorClass[] mangnguon, TransitionClass[] mangchuyen)
        //{
        //    EndingTime2 = endingtime;
        //    ArrayNguon2 = mangnguon;
        //    ArrayTransitions2 = mangchuyen;
        //}
        public Transition(int endingtime, DataVertex[] mangnguon, DataVertex[] mangchuyen)
        {
            EndingTime1 = endingtime;
            ArrayNguon1 = mangnguon;
            ArrayTransitions1 = mangchuyen;
            ListTimeNowTable1 = new List<int>();
            LineTimeNowTable1 = new Lines(){LineName = "Время", LineData = new List<int>()};
        }
        public void Run1()
        {
            try
            {

                PhantichTable1 = new List<List<int>>();
                LinePhanTichTable1 = new List<Lines>();
                var listptTable1 = new List<List<int>>();
                //listqueue1 - danh sach cac hang doi ca truoc va sau Chuyen
                var listQueueBeforAndAfter = ArrayTransitions1.SelectMany(x => x.ListEdgesTargetVertex).Concat(ArrayTransitions1.SelectMany(x => x.ListEdgesSorceVertex)).Distinct().ToList();
                //listQueueAfter - danh sach cac hang doi phia sau Chuyen
                var listQueueAfter = ArrayTransitions1.SelectMany(x => x.ListEdgesSorceVertex).Distinct().ToList();
                //listQueueBefor - danh sach cac hang doi phia sau Chuyen
                var listQueueBefor = ArrayTransitions1.SelectMany(x => x.ListEdgesTargetVertex).Distinct().ToList();
                ListTimeNowTable1.Add(TimeNow1);
                LineTimeNowTable1.LineData.Add(TimeNow1);
                var firsttimenow = new List<int>();
                foreach (var a in listQueueBeforAndAfter)
                {
                    firsttimenow.Add(a.HdCustomerses.Count);
                }
                listptTable1.Add(firsttimenow);
                
                foreach (var a in listQueueBeforAndAfter)
                {
                    var first = new Lines();
                    first.LineName = a.Text;
                    LinePhanTichTable1.Add(first);
                }
                CountAndSetNumberCusInQueue();
                var arrayTimeKh1 = new int[ArrayNguon1.Count()];
                var listnumcusnguon = new int[ArrayNguon1.Count()];
                InspectTrigger();
                do
                {
                    
                    for (int i = 0; i < ArrayNguon1.Length; i++)
                    {
                        if (ArrayNguon1.Select(x => x.GeneratorType.LengthOfFile).ToArray()[i] > listnumcusnguon[i])
                        {
                            Chips cus;
                            if (ArrayNguon1[i].GeneratorType.TListPointsCDF.Count != 0)
                            {
                                Random rand = new Random();
                                var s = rand.Next(0, ArrayNguon1[i].GeneratorType.TListPointsCDF.Count);
                                double d = Math.Abs(Math.Round(ArrayNguon1[i].GeneratorType.TListPointsCDF[s][0]));
                                cus = new Chips(ArrayNguon1[i].Text, ArrayNguon1[i].TypeOfVertex, arrayTimeKh1[i] + (int)d);
                            }
                            else //lay du lieu tu window
                            {
                                cus = SinhMotCusAndName11(ArrayNguon1[i], arrayTimeKh1[i]);
                            }
                            ListKh1.Add(cus);
                            listnumcusnguon[i]++;
                            arrayTimeKh1[i] = cus.TimePlan;
                        }
                    }
                    ListKh1 = ListKh1.FindAll(x => !(x.TimePlan > EndingTime1 && x.FromType == "AMGenerator"));
                    if (ListKh1.Count == 0)
                    {
                        break;
                    }
                    TimeNow1 = FindMinTimePlan(ListKh1);//tim timeplan nho nhat trong listKH de set TimeNow moi
                    ListTimeNowTable1.Add(TimeNow1);
                    LineTimeNowTable1.LineData.Add(TimeNow1);
                    var listequaltimenow1 = ListKh1.FindAll(x => x.TimePlan == TimeNow1);//loc ra cac Cus co timeplan = timenow duoc chon
                    ListKh1 = ListKh1.FindAll(x => x.TimePlan != TimeNow1);//xoa all Cus co timeplan = timenow

                    foreach (var a in listequaltimenow1)//tu trong danh sach phan ve cac hang doi tuong ung
                    {
                        foreach (var b in listQueueBeforAndAfter)
                        {
                            if (b.Text == a.Name)
                            {
                                b.HdCustomerses.Enqueue(a);
                                break;
                            }
                        }
                    }
                    var al = new List<int>();
                    foreach (var a in listQueueBeforAndAfter)//thuc hien dem so Cus trong hang -> dua ra graph
                    {
                        al.Add(a.HdCustomerses.Count);
                    }
                    listptTable1.Add(al);
                    CountAndSetNumberCusInQueue();

                    //    //Co Generator nao sinh het chua && TimeNow == LastTime thi dung -> neu co cai chua het thi co sinh them nua cung k de lam gif vi 1 cai da dung roi nen k the kich hoat bat cu Chuyen nao duoc nua ;
                    //    if (IsAnyGenEnd(arrayboolLengthOfFile) && TimeNow1 == LastTime)
                    //    {
                    //        break;
                    //    }
                    InspectTrigger();

                } while (TimeNow1 < EndingTime1);
                
                LastTime1 = TimeNow1;
                InspectTrigger();
                //lay nhung cus sinh ra do kich hoat tu listKH cho vao hang doi tuong ung
                var lcuslasttime = new List<Chips>();
                //lay ra tat ca cac cus sinh ra do kich hoat tu listKH1
                foreach (var a in listQueueAfter)
                {
                    foreach (var b in ListKh1.Where(x => x.Name == a.Text).ToList())
                    {
                        lcuslasttime.Add(b);
                    }
                }                
                if (lcuslasttime.Count != 0)
                {
                    LastTime1 = lcuslasttime.Max(x => x.TimePlan);
                    while (lcuslasttime.Count != 0)
                    {
                        //tim timeplan nho nhat trong cac cus duoc sinh do kich hoat -> lau nhu TimeNow -> lay ra cac cus do -> phan ve hang tuong ung
                        ListTimeNowTable1.Add(lcuslasttime.Min(x => x.TimePlan));
                        LineTimeNowTable1.LineData.Add(lcuslasttime.Min(x => x.TimePlan));
                        var list = lcuslasttime.Where(x => x.TimePlan == lcuslasttime.Min(y => y.TimePlan)).ToList();
                        lcuslasttime = lcuslasttime.FindAll(x => x.TimePlan != lcuslasttime.Min(y => y.TimePlan));//xoa all Cus co timeplan = timenow
                        foreach (var a in list)//tu trong danh sach phan ve cac hang doi tuong ung
                        {
                            foreach (var b in listQueueAfter)
                            {
                                if (b.Text == a.Name)
                                {
                                    b.HdCustomerses.Enqueue(a);
                                    break;
                                }
                            }
                        }
                        var al1 = new List<int>();
                        foreach (var sss in listQueueBeforAndAfter)
                        {
                            al1.Add(sss.HdCustomerses.Count);
                        }
                        listptTable1.Add(al1);
                        CountAndSetNumberCusInQueue();
                        InspectTrigger();
                    }
                }
                PhantichTable1 = listptTable1;
                var ss = ChuyenHang2Cot1(listptTable1);
                for (int i = 0; i < ss.Count; i++)
                {
                    LinePhanTichTable1[i].LineData = ss[i];
                }
                CountAndSetNumberCusInQueueWithName();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void InspectTrigger()
        {
            foreach (var a in ArrayTransitions1)
            {
                while (KiemtradieukienkichhoatMm(a)) //Kich hoat
                {
                    Count++;
                    for (int i = 0; i < a.Mangdkcungvao.Length; i++)
                    {
                        for (int j = 0; j < a.Mangdkcungvao[i]; j++)
                        {
                            var cus1 = a.ListEdgesTargetVertex[i].HdCustomerses.Dequeue();
                            a.ListEdgesTargetVertex[i].HdCustomersesPhantich.Enqueue
                            (new Chips()
                            {
                                Name = cus1.Name,
                                FromType = cus1.FromType,
                                TimeIn = TimeNow1,
                                TimeOut = TimeNow1,
                                TimePlan = cus1.TimePlan
                            });
                        }
                    }
                    //------ghi lai tai thoi diem kich hoat------//
                    double d;
                    if (a.TransitionType.TListPointsCDF.Count != 0)
                    {
                        Random rand = new Random();
                        var s = rand.Next(0, a.TransitionType.TListPointsCDF.Count);
                        d = Math.Abs(Math.Round(a.TransitionType.TListPointsCDF[s][0]));
                    }
                    else //lay du lieu tu window
                    {
                        d = RandomNumberFromTransition1(a);
                    }
                    for (int i = 0; i < a.Mangdkcungra.Length; i++)
                    {
                        //gan LastTime cua tung vertex 
                        a.ListEdgesSorceVertex[i].LastTime = TimeNow1 + d;
                        for (int j = 0; j < a.Mangdkcungra[i]; j++)
                        {
                            //tranh truong hop cai den sau lai duoc tao truoc       
                            ListKh1.Add(Sinh1Customer1(a.ListEdgesSorceVertex[i], (int) a.
                                ListEdgesSorceVertex[i].LastTime));
                        }
                    }
                }
            }
        }
        private static bool KiemtradieukienkichhoatMm(DataVertex a)
        {
            var mm = true;
            for (int i = 0; i < a.Mangdkcungvao.Length; i++)
            {
                mm = mm && a.ListEdgesTargetVertex.Select(x => x.HdCustomerses.Count).ToArray()[i] >= a.Mangdkcungvao[i];
            }
            return mm;
        }
        private void CountAndSetNumberCusInQueue()//dem nhung chua co ten
        {
            foreach (var a in ArrayTransitions1)
            {
                a.ListTimeNow.Add(TimeNow1);
                var befor = new List<int>();
                foreach (var b in a.ListEdgesTargetVertex)
                {
                    befor.Add(b.HdCustomerses.Count);
                }
                a.ListTimePlaceIn.Add(befor);
                var after = new List<int>();
                foreach (var b in a.ListEdgesSorceVertex)
                {
                    after.Add(b.HdCustomerses.Count);
                }
                a.ListTimePlaceOut.Add(after);
            }
        }
        private void CountAndSetNumberCusInQueueWithName()//dem co ten =>dung de xuat ra ngoai
        {
            foreach (var a in ArrayTransitions1)
            {
                a.LineTimeNow.LineName = a.Text;
                a.LineTimeNow.LineData = a.ListTimeNow;
                a.ListLinePlaceIn = new List<Lines>();
                foreach (var b in a.ListEdgesTargetVertex)
                {
                    var linein = new Lines();
                    linein.LineName = b.Text;
                    a.ListLinePlaceIn.Add(linein);
                }
                for (int i = 0; i < a.ListLinePlaceIn.Count; i++)
                {
                    var lin = new List<int>();
                    for (int j = 0; j < a.ListTimePlaceIn.Count; j++)
                    {
                        lin.Add(a.ListTimePlaceIn[j][i]);
                    }
                    a.ListLinePlaceIn[i].LineData = lin;
                }
                a.ListLinePlaceOut = new List<Lines>();
                foreach (var b in a.ListEdgesSorceVertex)
                {
                    var lineout = new Lines() {LineName = b.Text, LineData = new List<int>()};
                    a.ListLinePlaceOut.Add(lineout);
                    
                }
                for (int i = 0; i < a.ListLinePlaceOut.Count; i++)
                {
                    var lin = new List<int>();
                    for (int j = 0; j < a.ListTimePlaceOut.Count; j++)
                    {
                        lin.Add(a.ListTimePlaceOut[j][i]);
                    }
                    a.ListLinePlaceOut[i].LineData = lin;
                }
            }
        }
        private static long RandomNumberFromTransition1(DataVertex vertex)
        {
            switch (vertex.TransitionType.TypeDistribuion)
            {
                case GeneratorClass.Distribution.NormalDis:
                    vertex.TransitionType.TypeDis = new Normal(vertex.TransitionType.Mean, Math.Sqrt(vertex.TransitionType.Para));
                    break;
                case GeneratorClass.Distribution.ExponentialDis:
                    vertex.TransitionType.TypeDis = new Exponential(vertex.TransitionType.Para);
                    break;
                default:
                    Console.WriteLine("k tim thay");
                    break;
            }

            long d;
            do
            {
                d = (long)vertex.TransitionType.TypeDis.NextDouble();
            } while (d <= 0L);
            return d;
        }
        public Chips Sinh1Customer1(DataVertex vertex, int plantime)
        {
            var c = new Chips(vertex.Text, vertex.TypeOfVertex, plantime);
            return c;
        }
        public Chips SinhMotCusAndName11(DataVertex vertex, int timeplanmin)
        {
            var d = RandomNumberFromTransition11(vertex);
            var c = new Chips(vertex.Text, vertex.TypeOfVertex, timeplanmin + (int)d);
            return c;
        }
        private static long RandomNumberFromTransition11(DataVertex vertex)
        {
            switch (vertex.GeneratorType.TypeDistribuion)
            {
                case GeneratorClass.Distribution.NormalDis:
                    vertex.GeneratorType.TypeDis = new Normal(vertex.GeneratorType.Mean, Math.Sqrt(vertex.GeneratorType.Para));
                    break;
                case GeneratorClass.Distribution.ExponentialDis:
                    vertex.GeneratorType.TypeDis = new Exponential(vertex.GeneratorType.Para);
                    break;
                default:
                    Console.WriteLine("k tim thay");
                    break;
            }

            long d;
            do
            {
                d = (long)vertex.GeneratorType.TypeDis.NextDouble();
            } while (d <= 0L);
            return d;
        }
        public List<List<int>> ChuyenHang2Cot1(List<List<int>> list)
        {
            var dem = list.Count;
            int mm = list[0].Count;
            var aa = new List<List<int>>();
            for (int i = 0; i < mm; i++)
            {
                var nn = new List<int>();
                for (int j = 0; j < dem; j++)
                {
                    nn.Add(list[j][i]);
                }
                aa.Add(nn);
            }
            return aa;
        }
        public Chips Sinh1Customer(string name, int plantime)
        {
            var c = new Chips(name, plantime);
            return c;
        }
        public int FindMinTimePlan(List<Chips> listcus)
        {
            return listcus.Min(c => c.TimePlan);
        }
    }
}

