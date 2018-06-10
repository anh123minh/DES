using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using React.Distribution;

namespace SimulationV1.WPF.ExampleModels
{
    class Transition
    {

        public int SoCungVao { get; set; } = 3;
        public int SoCungRa { get; set; } = 3;
        public int EndingTime { get; set; }
        public List<int> ListTimeNowTable { get; set; }
        public List<List<int>> PhantichTable { get; set; }
        public List<int> ListTimeNowGraph { get; set; }//dung cho ve do thi
        public List<List<int>> PhantichGraph { get; set; }
        public List<List<int>> listptTable = new List<List<int>>();//can xem lai

        public int[] ArrayDKCungVao;
        public int[] ArrayDKCungRa;
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
        public List<List<int>> PhantichTable1 { get; set; }
        public List<Chips> listKH1 = new List<Chips>();//list ke hoach chua cac cus se vao he thong 

        public Transition(int endingtime, int socungvao, int[] arraycungvao, int socungra, int[] arraycungra, GeneratorClass[] mangnguon, TransitionClass nutchuyen)
        {
            EndingTime = endingtime;
            SoCungVao = socungvao;
            ArrayDKCungVao = arraycungvao;
            SoCungRa = socungra;
            ArrayDKCungRa = arraycungra;
            ArrayNguon = mangnguon;
            NutTransition = nutchuyen;
        }

        public Transition(int endingtime, GeneratorClass[] mangnguon, TransitionClass[] mangchuyen)
        {
            EndingTime2 = endingtime;
            ArrayNguon2 = mangnguon;
            ArrayTransitions2 = mangchuyen;
        }
        public Transition(int endingtime, DataVertex[] mangnguon, DataVertex[] mangchuyen)
        {
            EndingTime1 = endingtime;
            ArrayNguon1 = mangnguon;
            ArrayTransitions1 = mangchuyen;
            ListTimeNowTable1 = new List<int>();
        }
        public void Run1()
        {
            try
            {
     
                var listptTable1 = new List<List<int>>();
                //listqueue1 - danh sach cac hang doi ca truoc va sau Chuyen
                var listQueueBeforAndAfter = ArrayTransitions1.SelectMany(x => x.ListEdgesTargetVertex).Concat(ArrayTransitions1.SelectMany(x => x.ListEdgesSorceVertex)).Distinct().ToList();
                //listQueueAfter - danh sach cac hang doi phia sau Chuyen
                var listQueueAfter = ArrayTransitions1.SelectMany(x => x.ListEdgesSorceVertex).Distinct().ToList();
                //listQueueBefor - danh sach cac hang doi phia sau Chuyen
                var listQueueBefor = ArrayTransitions1.SelectMany(x => x.ListEdgesTargetVertex).Distinct().ToList();
                ListTimeNowTable1.Add(TimeNow1);
                var firsttimenow = new List<int>();
                foreach (var a in listQueueBeforAndAfter)
                {
                    firsttimenow.Add(a.HdCustomerses.Count);
                }
                listptTable1.Add(firsttimenow);
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
                var arrayTimeKh1 = new int[ArrayNguon1.Count()];
                var listnumcusnguon = new int[ArrayNguon1.Count()];
                do
                {
                    for (int i = 0; i < ArrayNguon1.Length; i++)
                    {
                        if (ArrayNguon1.Select(x => x.GeneratorType.LengthOfFile).ToArray()[i] > listnumcusnguon[i])
                        {
                            var cus = SinhMotCusAndName11(ArrayNguon1[i], arrayTimeKh1[i]);
                            listKH1.Add(cus);
                            listnumcusnguon[i]++;
                            arrayTimeKh1[i] = cus.TimePlan;
                        }
                    }
                    listKH1 = listKH1.FindAll(x => !(x.TimePlan > EndingTime1 && x.FromType == "AMGenerator"));
                    if (listKH1.Count == 0)
                    {
                        break;
                    }
                    TimeNow1 = FindMinTimePlan(listKH1);//tim timeplan nho nhat trong listKH de set TimeNow moi
                    ListTimeNowTable1.Add(TimeNow1);
                    var listequaltimenow1 = listKH1.FindAll(x => x.TimePlan == TimeNow1);//loc ra cac Cus co timeplan = timenow duoc chon
                    listKH1 = listKH1.FindAll(x => x.TimePlan != TimeNow1);//xoa all Cus co timeplan = timenow

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

                    //    //Co Generator nao sinh het chua && TimeNow == LastTime thi dung -> neu co cai chua het thi co sinh them nua cung k de lam gif vi 1 cai da dung roi nen k the kich hoat bat cu Chuyen nao duoc nua ;
                    //    if (IsAnyGenEnd(arrayboolLengthOfFile) && TimeNow1 == LastTime)
                    //    {
                    //        break;
                    //    }
                    foreach (var a in ArrayTransitions1)
                    {
                        var mm = true;
                        for (int i = 0; i < a.Mangdkcungvao.Length; i++)
                        {
                            mm = mm && a.ListEdgesTargetVertex.Select(x => x.HdCustomerses.Count).ToArray()[i] >= a.Mangdkcungvao[i];
                        }
                        if (mm)//Kich hoat
                        {
                            for (int i = 0; i < a.Mangdkcungvao.Length; i++)
                            {
                                for (int j = 0; j < a.Mangdkcungvao[i]; j++)
                                {
                                    var cus1 = a.ListEdgesTargetVertex[i].HdCustomerses.Dequeue();
                                    a.ListEdgesTargetVertex[i].HdCustomersesPhantich.Enqueue(new Chips() { Name = cus1.Name, FromType = cus1.FromType, TimeIn = TimeNow1, TimeOut = TimeNow1, TimePlan = cus1.TimePlan });
                                }
                            }
                            //------ghi lai tai thoi diem kich hoat
                            //foreach (var c in ArrayTransitions1)
                            //{
                            //    c.ListTimeNow.Add(TimeNow1);
                            //    var befor = new List<int>();
                            //    foreach (var b in c.ListEdgesTargetVertex)
                            //    {
                            //        befor.Add(b.HdCustomerses.Count);
                            //    }
                            //    c.ListTimePlaceIn.Add(befor);
                            //    var after = new List<int>();
                            //    foreach (var b in c.ListEdgesSorceVertex)
                            //    {
                            //        befor.Add(b.HdCustomerses.Count);
                            //    }
                            //    c.ListTimePlaceOut.Add(after);      

                            double d;
                            if (a.TransitionType.TListPointsCDF.Count != 0)
                            {
                                Random rand = new Random();
                                var s = rand.Next(0, a.TransitionType.TListPointsCDF.Count);
                                d = Math.Abs(Math.Round(a.TransitionType.TListPointsCDF[s][0]));
                            }
                            else//lay du lieu tu window
                            {
                                d = RandomNumberFromTransition1(a);
                            }
                            for (int i = 0; i < a.Mangdkcungra.Length; i++)
                            {
                                for (int j = 0; j < a.Mangdkcungra[i]; j++)
                                {//de tranh truong hop cus sinh sau nhung co timeplan < timeplan cus sinh truoc
                                    listKH1.Add(TimeNow1 > LastTime1 ? Sinh1Customer1(a.ListEdgesSorceVertex[i], TimeNow1 + (int)d) : Sinh1Customer1(a.ListEdgesSorceVertex[i], LastTime1 + (int)d));
                                }
                            }
                            if (TimeNow1 > LastTime1)
                            {
                                LastTime1 = TimeNow1 + (int)d;
                            }
                            else
                            {
                                LastTime1 += (int)d;
                            }
                        }
                    }

                } while (TimeNow1 < EndingTime1);

                if (LastTime1 > EndingTime1) //lay nhung cus sinh ra do kich hoat tu listKH cho vao hang doi
                {                   
                    
                    //
                    var lcuslasttime = new List<Chips>();
                    //lay ra tat ca cac cus sinh ra do kich hoat tu listKH1
                    foreach (var a in listQueueAfter)
                    {
                        foreach (var b in listKH1.Where(x => x.Name == a.Text).ToList())
                        {
                            lcuslasttime.Add(b);
                        }
                    }
                    while (lcuslasttime.Count != 0)
                    {
                        //tim timeplan nho nhat trong cac cus duoc sinh do kich hoat -> lau nhu TimeNow -> lay ra cac cus do -> phan ve hang tuong ung
                        ListTimeNowTable1.Add(lcuslasttime.Min(x => x.TimePlan));
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
                                befor.Add(b.HdCustomerses.Count);
                            }
                            a.ListTimePlaceOut.Add(after);
                        }
                    }
                    
                }
                PhantichTable1 = ChuyenHang2Cot1(listptTable1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
            //int mm = list.FirstOrDefault().Count;
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


        public void Run()
        {
            try
            {
                var TimeKH = 0;
                var TimeNow = 0;
                var TimeNowNext = 0;
                var SumCung = SoCungVao + SoCungRa;
                var LastTime = 0;//la thoi diem cuoi cung trong mo phong khi co bat cu Gen nao sinh het cus va time du kien < timeend
                ListTimeNowTable = new List<int>();
                PhantichTable = new List<List<int>>();
                ListTimeNowGraph = new List<int>();
                PhantichGraph = new List<List<int>>();

                //List<List<int>> listptTable = new List<List<int>>();//can xem lai
                var ana1 = new List<List<int>>();

                Queue<Chips>[] arrayHDVaoRa = new Queue<Chips>[SoCungVao];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHDVaoRa[i] = new Queue<Chips>();
                }
                //list ke hoach chua cac cus se vao he thong 
                var listKH = new List<Chips>();

                #region Tao hang doi cho cac Places -> sau co the tao thanh field arrayHD cho class va tien hanh khoi tao trong Contructor
                Queue<Chips>[] arrayHD = new Queue<Chips>[SumCung];
                for (int i = 0; i < SoCungVao; i++)
                {
                    arrayHD[i] = new Queue<Chips>();
                }
                for (int i = SoCungVao; i < SumCung; i++)
                {
                    arrayHD[i] = new Queue<Chips>();
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
                    arrayLengthOfFile[i] = ArrayNguon[i].LengthOfFile;
                }
                #endregion

                #region Thoi diem timenow = 0

                ListTimeNowTable.Add(TimeNow);
                var firsttimenow = new List<int>();
                foreach (var sss in arrayHD)
                {
                    firsttimenow.Add(sss.Count);
                }
                listptTable.Add(firsttimenow);
                #endregion
                var liststringnameout = new List<string>();//danh sach cac ten 
                for (int i = SoCungRa; i < SumCung; i++)
                {
                    liststringnameout.Add(i.ToString());
                }
                var liststringnamein = new List<string>();//danh sach cac ten 
                for (int i = 0; i < SoCungVao; i++)
                {
                    liststringnamein.Add(i.ToString());
                }
                do
                {
                    for (int i = 0; i < SoCungVao; i++)
                    {
                        var cus = SinhMotCusAndName2(i.ToString(), ArrayNguon[i], arrayTimeKH[i]);
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

                    #region neu trong listKH co cus sinh ra tu Gen vaf co timeplan > ending thi xoa khoi danh sach
                    var listmove = new List<Chips>();
                    foreach (var a in listKH)
                    {
                        foreach (var b in liststringnamein)
                        {
                            if (a.Name == b && a.TimePlan > EndingTime)
                            {
                                listmove.Add(a);
                            }
                        }
                    }
                    foreach (var a in listmove)
                    {
                        listKH = listKH.FindAll(x => x != a);
                    }
                    #endregion

                    if (listKH.Count != 0)
                    {
                        TimeNow = FindMinTimePlan(listKH);//tim timeplan nho nhat trong listKH de set TimeNow moi
                        ListTimeNowTable.Add(TimeNow);
                        ListTimeNowGraph.Add(TimeNow);
                    }
                    else
                    {
                        break;
                    }
                    var listequaltimenow = listKH.FindAll(x => x.TimePlan == TimeNow);//loc ra cac Cus co timeplan = timenow duoc chon
                    listKH = listKH.FindAll(x => x.TimePlan != TimeNow);//xoa all Cus co timeplan = timenow

                    foreach (var a in listequaltimenow)//tu trong danh sach phan ve cac hang doi tuong ung
                    {
                        var cus = a;
                        var aa = cus.Name;
                        arrayHD[Int32.Parse(aa)].Enqueue(new Chips() { Name = cus.Name, TimePlan = cus.TimePlan });
                    }
                    var al = new List<int>();
                    foreach (var sss in arrayHD)//thuc hien dem so Cus trong hang -> dua ra graph
                    {
                        al.Add(sss.Count);
                    }
                    listptTable.Add(al);
                    ana1.Add(al);
                    //Co Generator nao sinh het chua && TimeNow == LastTime ? break : ;
                    if (IsAnyGenEnd(arrayboolLengthOfFile) && TimeNow == LastTime)
                    {
                        break;
                    }
                    if (AlReady(arrayHD, arrayDKCung))//xac dinh da thoa man dieu kien chua
                    {
                        for (int i = 0; i < SoCungVao; i++)
                        {
                            for (int j = 0; j < arrayDKCung[i]; j++)
                            {
                                var cus1 = arrayHD[i].Dequeue();
                                arrayHDVaoRa[i].Enqueue(new Chips() { Name = cus1.Name, TimeIn = TimeNow, TimeOut = TimeNow, TimePlan = cus1.TimePlan });
                            }
                        }
                        //------ghi lai tai thoi diem kich hoat
                        ListTimeNowGraph.Add(TimeNow);
                        var al1 = new List<int>();
                        foreach (var sss in arrayHD)
                        {
                            al1.Add(sss.Count);
                        }
                        ana1.Add(al1);
                        //------
                        double d;
                        if (NutTransition.TListPointsCDF.Count != 0)//neu lay du lieu tu file
                        {
                            Random rand = new Random();
                            var s = rand.Next(0, NutTransition.TListPointsCDF.Count);
                            d = Math.Abs(Math.Round(NutTransition.TListPointsCDF[s][0]));
                        }
                        else//lay du lieu tu window
                        {
                            d = RandomNumberFromTransition(NutTransition);
                        }
                        for (int i = SoCungVao; i < SumCung; i++)
                        {
                            for (int j = 0; j < arrayDKCung[i]; j++)
                            {//de tranh truong hop cus sinh sau nhung co timeplan < timeplan cus sinh truoc
                                listKH.Add(TimeNow > LastTime
                                    ? Sinh1Customer(i.ToString(), TimeNow + (int)d)
                                    : Sinh1Customer(i.ToString(), LastTime + (int)d));
                            }
                        }
                        if (TimeNow > LastTime)
                        {
                            LastTime = TimeNow + (int)d;
                        }
                        else
                        {
                            LastTime += (int)d;
                        }
                    }
                } while (TimeNow < EndingTime);//while (TimeNow < EndingTime);

                if (LastTime > EndingTime)//lay nhung cus sinh ra do kich hoat tu listKH cho vao hang doi
                {
                    //lay ra tat ca cac cus sinh ra do kich hoat tu listKH
                    var lcuslasttime = new List<Chips>();
                    foreach (var a in liststringnameout)
                    {
                        foreach (var b in listKH)
                        {
                            if (b.Name == a)
                            {
                                lcuslasttime.Add(b);
                            }
                        }
                    }
                    var listsametimeplan1 = from n in lcuslasttime group n by n.TimePlan into g select new { g.Key, Cus = from o in g group o by o.Name };
                    foreach (var t in listsametimeplan1)
                    {
                        var k = t.Key;
                        foreach (var h in t.Cus)
                        {
                            foreach (var a in h)
                            {
                                arrayHD[Int32.Parse(a.Name)].Enqueue(new Chips() { Name = a.Name, TimePlan = a.TimePlan });
                            }
                        }
                        ListTimeNowTable.Add(k);
                        var al1 = new List<int>();
                        foreach (var sss in arrayHD)
                        {
                            al1.Add(sss.Count);
                        }
                        listptTable.Add(al1);
                    }
                }
                PhantichTable = ChuyenHang2Cot1(listptTable);
                PhantichGraph = ChuyenHang2Cot1(ana1);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }        
        private bool IsAnyGenEnd(bool[] arrayboolLengthOfFile)
        {
            foreach (var a in arrayboolLengthOfFile)
            {
                if (a)
                {
                    return true;
                }
            }
            return false;
        }
        public Chips Sinh1Customer(string name, int plantime)
        {
            var c = new Chips(name, plantime);
            return c;
        }
        public List<List<int>> ChuyenHang2Cot(List<Queue<int>> list)
        {
            var dem = list;
            int mm = list.FirstOrDefault().Count;

            var aa = new List<List<int>>();
            for (int i = 0; i < mm; i++)
            {
                var nn = new List<int>();
                foreach (var m in dem)
                {
                    nn.Add(m.Dequeue());
                }
                aa.Add(nn);
            }
            return aa;
        }     
        public Chips SinhMotCus(int timenow)
        {
            var ran = new Random();
            var c = new Chips(timenow + ran.Next(1, 5));
            return c;
        }
        public Chips SinhMotCusWithName(string name, int timenow, int ranmax)
        {
            var ran = new Random();
            var c = new Chips(name, timenow + ran.Next(1, ranmax));
            return c;
        }
        public Queue<Chips> SinhCus(int endingTime)
        {
            Queue<Chips> queueCustomerss = new Queue<Chips>();
            int stt = 1;
            var now = 0;
            var ran = new Random();
            do
            {

                now += ran.Next(1, 5);
                var c = new Chips(stt.ToString(), now);
                queueCustomerss.Enqueue(c);
                stt++;
            } while (now < endingTime);
            return queueCustomerss;
        }
        public bool AlReady(Queue<Chips> a, Queue<Chips> b, int min1, int min2)
        {
            return a.Count >= min1 && b.Count >= min2;
        }
        public int FindMinTimePlan(List<Chips> listcus)
        {
            return listcus.Min(c => c.TimePlan);
        }
        public static int FindMinNextTimePlan(List<Chips> listcus)
        {
            var listemp = listcus;
            listemp.RemoveAll(a => a.TimePlan == listemp.Min(c => c.TimePlan));
            return listemp.Min(c => c.TimePlan);
        }
        public Chips CusHasEqualPlanTimeNow(List<Chips> listcus)
        {
            var cus = new Chips();

            return cus;
        }
        public bool AlReady(Queue<Chips>[] listqueuecus, int[] listminmachine)
        {
            var already = true;
            for (int i = 0; i < SoCungVao; i++)
            {
                already = already && listqueuecus[i].Count >= listminmachine[i];
            }
            return already;
        }
        public bool AlReady1(Queue<Chips>[] listqueuecus, int[] listminmachine)
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
        public Chips SinhMotCusAndName1(string name, Nut nut, int timenow)
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
            var c = new Chips(name, timenow + (int)d);
            return c;
        }
        public Chips SinhMotCusAndName2(string name, GeneratorClass nut, int timeplanmin)
        {
            var d = RandomNumberFromTransition(nut);
            var c = new Chips(name, timeplanmin + (int)d);
            return c;
        }
        private static long RandomNumberFromTransition(GeneratorClass nut)
        {
            switch (nut.TypeDistribuion)
            {
                case GeneratorClass.Distribution.NormalDis:
                    nut.TypeDis = new Normal(nut.Mean, Math.Sqrt(nut.Para));
                    break;
                case GeneratorClass.Distribution.ExponentialDis:
                    nut.TypeDis = new Exponential(nut.Para);
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
        public int LengthOfFile { get; set; } = 30; //số Chips tối đa       
        public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;
        public int NumBarbers { get; set; } = 1; //So luong may phuc vu

        public int QueueCapacity { get; set; } = 500;
    }
}

