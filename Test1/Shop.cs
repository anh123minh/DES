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
using System.Text;
using React;
using React.Distribution;

namespace BarberShop
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
        private const long ClosingTime = 4 * 60;
        private NonUniform TypeDis { get; set; }//Kieu Distribution
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

        public Shop()
        {
        }

        public IEnumerator<Task> Generator(Process p, object data)
        {

            Console.WriteLine(@"The barber shop is opening for business...");
            Resource barbers = CreateBarbers();
            int i = 0;
            //switch (TypeDistribuion)
            //{
            //    case Distribution.NormalDis:
            //        TypeDis = new Normal(Interval, 1.0);
            //        break;
            //    case Distribution.ExponentialDis:
            //        TypeDis = new Exponential(Interval);
            //        break;
            //    default:
            //        Console.WriteLine("k tim thay");
            //        break;
            //}
            TypeDis = new Normal(Interval, 0.0);
            do
            {
                long d;
                do
                {
                    d = (long)TypeDis.NextDouble();
                } while (d <= 0L);
                if (FirstTime != 0 && Now == 0)
                {
                    yield return p.Delay(FirstTime);
                    i++;
                    //Console.WriteLine(@"xxx         so Cus trong hang doi = " + ABarbers.BlockCount + " " + Now);
                    Customer c = new Customer(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, barbers);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop 0");
                }
                else
                {
                    yield return p.Delay(d);
                    //Console.WriteLine("Now - " + Now + " xxx         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                    i++;
                    Customer c = new Customer(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, barbers);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop 0");
                    //Console.WriteLine("Now - " + Now + " yyy         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);

                }

            } while (Now < ClosingTime);

            Console.WriteLine(@"======================================================");
            Console.WriteLine(@"The barber shop is closed for the day.");

            if (barbers.BlockCount > 0)
            {
                Console.WriteLine(@"The barbers have to work late today.");
            }

            yield break;
        }
        public IEnumerator<Task> Generator1(Process p, object data)
        {

            Console.WriteLine(@"1The barber shop is opening for business...");
            Resource barbers1 = CreateBarbers1();
            int i = 0;
            //switch (TypeDistribuion)
            //{
            //    case Distribution.NormalDis:
            //        TypeDis = new Normal(Interval, 1.0);
            //        break;
            //    case Distribution.ExponentialDis:
            //        TypeDis = new Exponential(Interval);
            //        break;
            //    default:
            //        Console.WriteLine("k tim thay");
            //        break;
            //}
            TypeDis = new Exponential(Interval);
            do
            {
                long d;
                do
                {
                    d = (long)TypeDis.NextDouble();
                } while (d <= 0L);
                if (FirstTime != 0 && Now == 0)
                {
                    yield return p.Delay(FirstTime);
                    i++;
                    //Console.WriteLine(@"xxx         so Cus trong hang doi = " + ABarbers.BlockCount + " " + Now);
                    Customer1 c = new Customer1(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, barbers1);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop 1");
                }
                else
                {
                    yield return p.Delay(d);
                    //Console.WriteLine("1Now - " + Now + " xxx         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                    i++;
                    Customer1 c = new Customer1(this, i.ToString(), this.Now, QueueCapacity);
                    c.Activate(null, 0L, barbers1);
                    Console.WriteLine(this.Now + " CusCome customer " + c.Name + " Shop 1");
                    //Console.WriteLine("1Now - " + Now + " yyy         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);

                }

            } while (Now < ClosingTime);

            Console.WriteLine(@"1======================================================");
            Console.WriteLine(@"1The barber shop is closed for the day.");

            if (barbers1.BlockCount > 0)
            {
                Console.WriteLine(@"1The barbers have to work late today.");
            }

            yield break;
        }

        //private IEnumerable<Task> SinhCus(Process p, object data, Nut nut)
        //{
        //    Console.WriteLine(@"The barber shop is opening for business...");
        //    Resource barbers = CreateBarbers();
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
        //        long d;
        //        do
        //        {
        //            d = (long)nut.TypeDis.NextDouble();
        //        } while (d <= 0L);
        //        if (nut.FirstTime != 0 && Now == 0)
        //        {
        //            yield return p.Delay(nut.FirstTime);
        //            i++;
        //            //Console.WriteLine(@"xxx         so Cus trong hang doi = " + ABarbers.BlockCount + " " + Now);
        //            Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity);
        //            c.Activate(null, 0L, barbers);
        //            Console.WriteLine(this.Now + " The customer " + c.Name + " come");
        //        }
        //        else
        //        {
        //            yield return p.Delay(d);
        //            Console.WriteLine("Now - " + Now + " xxx         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
        //            i++;
        //            Customer c = new Customer(this, i.ToString(), this.Now, nut.QueueCapacity);
        //            c.Activate(null, 0L, barbers);
        //            Console.WriteLine("Now - " + this.Now + " The customer " + c.Name + " come");
        //            Console.WriteLine("Now - " + Now + " yyy         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);

        //        }

        //    } while (Now < Nut.ClosingTime);

        //    Console.WriteLine(@"======================================================");
        //    Console.WriteLine(@"The barber shop is closed for the day.");

        //    if (barbers.BlockCount > 0)
        //    {
        //        Console.WriteLine(@"The barbers have to work late today.");
        //    }

        //    yield break;
        //}
        //public class Nut
        //{
        //    public const long ClosingTime = 4 * 60;
        //    public NonUniform TypeDis { get; set; }//Kieu Distribution
        //    public enum Distribution
        //    {
        //        NormalDis,
        //        ExponentialDis
        //    }
        //    //Nhung Bien set tu giao dien duoc
        //    public int FirstTime { get; set; } = 0;//Thời điểm bắt đầu mô phỏng
        //    public double Interval { get; set; } = 5;//Khoang lamda
        //    public int LengthOfFile { get; set; } = 15;//số Customer tối đa       
        //    public Distribution TypeDistribuion { get; set; } = Distribution.NormalDis;

        //    public int QueueCapacity { get; set; } = 500;

        //    //Bien dung trong tinh toan
        //    public bool IsReady { get; set; } = false;//San sang de thuc thi hay chua
        //}
        private Resource CreateBarbers()
        {
            //Barber[] barbers = new Barber[4];
            //barbers[0] = new Barber(this, "Frank");
            //barbers[1] = new Barber(this, "Tom");
            //barbers[2] = new Barber(this, "Bill");
            //barbers[3] = new Barber(this, "Joe");
            List<Barber> barbers = new List<Barber>();
            barbers.Add(new Barber(this, "Shop 0 A"));
            barbers.Add(new Barber(this, "Shop 0 B"));

            return Resource.Create(barbers);
        }

        private Resource CreateBarbers1()
        {
            List<Barber1> barbers = new List<Barber1>();
            barbers.Add(new Barber1(this, "Shop 1 A"));
            barbers.Add(new Barber1(this, "Shop 1 B"));

            return Resource.Create(barbers);
        }
        //private const long ClosingTime = 8 * 60;

        //public Shop()
        //{
        //}

        //public IEnumerator<Task> Generator(Process p, object data)
        //{
        //    Console.WriteLine("The barber shop is opening for business...");
        //    Resource barbers = CreateBarbers();

        //    Normal n = new Normal(5.0, 1.0);

        //    do
        //    {
        //        long d;
        //        do
        //        {
        //            d = (long)n.NextDouble();
        //        } while (d <= 0L);

        //        yield return p.Delay(d);

        //        Customer c = new Customer(this);
        //        c.Activate(null, 0L, barbers);

        //    } while (Now < ClosingTime);

        //    Console.WriteLine("The barber shop is closed for the day.");

        //    if (barbers.BlockCount > 0)
        //    {
        //        Console.WriteLine("The barbers have to work late today.");
        //    }

        //    yield break;
        //}

        //private Resource CreateBarbers()
        //{
        //    Barber[] barbers = new Barber[4];
        //    barbers[0] = new Barber(this, "Frank");
        //    barbers[1] = new Barber(this, "Tom");
        //    barbers[2] = new Barber(this, "Bill");
        //    barbers[3] = new Barber(this, "Joe");

        //    return Resource.Create(barbers);
        //}

        //static void Main(string[] args)
        //{
        //    Shop shop = new Shop();
        //    Task generator = new Process(shop, shop.Generator);
        //    shop.Run(generator);
        //}
    }
}
