//=============================================================================
//=  $Id: Shop.cs 128 2005-12-04 20:12:00Z Eric Roe $
//=
//=  React.NET: A discrete-event simulation library for the .NET Framework.
//=  Copyright (c) 2005, Eric K. Roe.  All rights reserved.
//=
//=  React.NET is free software; you can redistribute it and/or modify it
//=  under the terms of the GNU General Public License as published by the
//=  Free Software Foundation; either version 2 of the License, or (at your
//=  option) any later version.
//=
//=  React.NET is distributed in the hope that it will be useful, but WITHOUT
//=  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//=  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//=  more details.
//=
//=  You should have received a copy of the GNU General Public License along
//=  with React.NET; if not, write to the Free Software Foundation, Inc.,
//=  51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//=============================================================================

using System;
using System.Collections.Generic;
using React;
using React.Distribution;

namespace TestDES
{
    /// <summary>
    /// The Barber Shop demonstration simulation.
    /// </summary>
    /// <remarks>
    /// The simulation demonstrates using <see cref="TrackedResources"/> as well
    /// as having one <see cref="Process"/> block on another.
    /// <para>
    /// The simulation is kicked off via the <see cref="Generator"/> method,
    /// which serves as a <see cref="ProcessSteps"/> delegate for the generator
    /// <see cref="Process"/>.  <see cref="Generator"/> begins by creating a
    /// <see cref="TrackedResource"/> containing four <see cref="Barber"/>
    /// processes.  It then creates a new <see cref="Customer"/> about once
    /// every five minutes and passes the resource (i.e. the barbers) to the
    /// client as <em>activation data</em>.  After eight hours (8 * 60min),
    /// the barber shop closes for the day.  Of course, the barbers finish
    /// with those customers who have been waiting.
    /// </para>
    /// </remarks>
    public class Shop : Simulation
    {


        public int NumberShop { get; set; } = 1;

        public bool[] AllIsReady;
        public bool[] AllAllReady;
        //Bien dung trong tinh toan

        public bool AllReady { get; set; } = false;//San sang de thuc thi hay chua

        public Shop()
        {
        }

        public Shop(int numshop)
        {
            NumberShop = numshop;
            bool[] aIsReady = new bool[numshop];
            for (int i = 0; i < numshop; i++)
            {
                aIsReady[i] = false;
            }
            AllIsReady = aIsReady;
            AllAllReady = aIsReady;
        }
        public IEnumerator<Task> SinhCus(Process p, object data)
        {
            Nut nut = data as Nut;
            Console.WriteLine(@"The barber shop " + nut.Name + " is opening for business...");
            var barbers1 = nut.SubTracked(this);
            AllAllReady[Int32.Parse(nut.Name)] = barbers1.AllReady;

            int i = 0;
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

            do
            {
                //long d;
                //do
                //{
                //    d = (long)nut.TypeDis.NextDouble();
                //} while (d <= 0L);
                long d = nut.Hang;
                if (nut.FirstTime != 0 && Now == 0)
                {
                    yield return p.Delay(nut.FirstTime);
                    i++;
                    Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers);
                    //c.Activate(null, 0L, barbers);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop " + nut.Name);
                }
                else
                {
                    AllIsReady[Int32.Parse(nut.Name)] = barbers1.IsReady;
                    var aa = true;
                    foreach (var a in AllIsReady)
                    {
                        aa = aa && a;
                    }
                    if (aa)
                    {
                        barbers1.AllReady = true;

                    }
                    else { barbers1.AllReady = false; }
                    i++;
                    Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers, this);
                    c.Activate(null, d, barbers1);
                    yield return p.Delay(d);
                    AllIsReady[Int32.Parse(nut.Name)] = barbers1.IsReady;
                    var bb = true;
                    foreach (var a in AllIsReady)
                    {
                        bb = bb && a;
                    }
                    if (bb)
                    {
                        barbers1.AllReady = true;

                    }

                    //i++;
                    //Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers, this);
                    //c.Activate(nut.Acti, 0L, barbers1);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop " + nut.Name);
                }

            } while (Now < Nut.ClosingTime);

            Console.WriteLine(@"======================================================");
            Console.WriteLine(@"The barber shop is closed for the day.");

            if (nut.CreateResource(this).BlockCount > 0)//
            {
                Console.WriteLine(@"The barbers have to work late today.");
            }

            yield break;
        }

        //public IEnumerator<Task> SinhCus(Process p, object data)
        //{
        //    Nut nut = data as Nut;
        //    Console.WriteLine(@"The barber shop " + nut.Name + " is opening for business...");
        //    var barbers = nut.SubTracked(this);

        //    int i = 0;
        //    switch (nut.TypeDistribuion)
        //    {
        //        case Nut.Distribution.NormalDis:
        //            nut.TypeDis = new Normal(nut.Interval, 1.0);
        //            break;
        //        case Nut.Distribution.ExponentialDis:
        //            nut.TypeDis = new Exponential(nut.Interval);
        //            break;
        //        default:
        //            Console.WriteLine("k tim thay");
        //            break;
        //    }

        //    do
        //    {
        //        //long d;
        //        //do
        //        //{
        //        //    d = (long)nut.TypeDis.NextDouble();
        //        //} while (d <= 0L);
        //        long d = nut.Hang;
        //        if (nut.FirstTime != 0 && Now == 0)
        //        {
        //            yield return p.Delay(nut.FirstTime);
        //            i++;
        //            Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers);
        //            //c.Activate(null, 0L, barbers);
        //            Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop " + nut.Name);
        //        }
        //        else
        //        {
        //            i++;
        //            Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers, this);
        //            c.Activate(p, d, barbers);
        //            p.WaitOnTask(c);
        //            yield return p.Suspend();

        //            //yield return p.Delay(0);

        //            //i++;
        //            //Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity, nut.Name, nut.NumBarbers, this);
        //            //c.Activate(nut.Acti, 0L, barbers);
        //            Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop " + nut.Name);
        //        }

        //    } while (Now < Nut.ClosingTime);

        //    Console.WriteLine(@"======================================================");
        //    Console.WriteLine(@"The barber shop is closed for the day.");

        //    if (nut.CreateResource(this).BlockCount > 0)//
        //    {
        //        Console.WriteLine(@"The barbers have to work late today.");
        //    }

        //    yield break;
        //}


    }

    public class Nut
    {
        public long Hang { get; set; } = 1;//Hang so 
        public string Name { get; set; } = "";//Name of Shop
        public const long ClosingTime = 4 * 60;
        public NonUniform TypeDis { get; set; }//Kieu Distribution
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
        public int NumBarbers { get; set; } = 1;//So luong may phuc vu

        public int QueueCapacity { get; set; } = 500;

        //Bien dung trong tinh toan
        public bool IsReady { get; set; } = false;//San sang de thuc thi hay chua

        public Resource CreateResource(Simulation sim)//Create list barbers
        {
            List<Barber> barbers = new List<Barber>();
            for (int i = 0; i < NumBarbers; i++)
            {
                barbers.Add(new Barber(sim, "Shop " + Name + " Barber " + i.ToString()));
            }
            return Resource.Create(barbers);
        }

        public Resource Barbers
        {
            get
            {
                List<Barber> barbers = new List<Barber>();
                for (int i = 0; i < NumBarbers; i++)
                {
                    barbers.Add(new Barber(Sim, "Shop " + Name + " Barber " + i.ToString()));
                }
                return Resource.Create(barbers);
            }
        }

        public SubTrackedResource SubTracked(Simulation sim)
        {
            List<Barber> barbers = new List<Barber>();
            for (int i = 0; i < NumBarbers; i++)
            {
                barbers.Add(new Barber(sim, "Shop " + Name + " Barber " + i.ToString()));
            }
            return new SubTrackedResource(barbers);
        }
        public Simulation Sim { get; set; }
        public object Acti { get; set; }
    }

    
}
