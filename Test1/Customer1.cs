//=============================================================================
//=  $Id: Customer.cs 128 2005-12-04 20:12:00Z Eric Roe $
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
using System.Windows;
using System.Windows.Media;
using React;

namespace BarberShop
{
    /// <summary>
    /// The customer <see cref="Process"/>.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Customer"/> waits to acquire a <see cref="Barber"/> from
    /// a <see cref="TrackedResource"/>.  Once having obtained a
    /// <see cref="Barber"/>, they activate the <see cref="Barber"/> process
    /// to simulate cutting hair.
    /// </remarks>
    internal class Customer1 : Process
    {
        public long TimeCome { get; set; }
        public long TimeIn { get; set; }
        public long TimeOut { get; set; }
        public int QueueCapacity { get; set; } = 500;
        public PointCollection Points { get; set; } = new PointCollection() { new Point(0, 0) };

        //Bien dung trong tinh toan
        public bool IsReady { get; set; } = false;//San sang de thuc thi hay chua

        internal Customer1(Simulation sim) : base(sim)
        {
        }

        internal Customer1(Simulation sim, string name, long timecome) : base(sim)
        {
            this.Name = name;
            this.TimeCome = timecome;
        }
        internal Customer1(Simulation sim, string name, long timecome, int maxque) : base(sim)
        {
            this.Name = name;
            this.TimeCome = timecome;
            this.QueueCapacity = maxque;
        }

        private long Condition { get; set; } = 4;

        protected override IEnumerator<Task> GetProcessSteps()
        {
            TrackedResource barbers = (TrackedResource)ActivationData;
            if (barbers.BlockCount < 1)
            {
                if (barbers.Free != 0)
                {
                    //Console.WriteLine("Now - " + Now + " 1         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                    barbers.OutOfService = 2;
                    //Console.WriteLine("Now - " + Now + " 2         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                }
            }
            else
            {
                IsReady = true;
                if (barbers.OutOfService != 0)
                {
                    //Console.WriteLine("Now - " + Now + " 3         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                    barbers.OutOfService = 0;
                    //Console.WriteLine("Now - " + Now + " 4         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                }
            }

            //Console.WriteLine("Now - " + Now + " M         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
            if (barbers.BlockCount < QueueCapacity)//max so Cus trong hang doi
            {
                //Console.WriteLine("Now - " + Now + " 5         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
                yield return barbers.Acquire(this);//?o?n n?y s? nh?y sang Barber ?? th?c hi?n, khi th?c hi?n xong s? nh?y v? 2//Sau doan nay Cus se luu vao hang doi// busy or not?//chiem lay cus moi
                //Console.WriteLine("Now - " + Now + " 6         BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);
            }
            else
            {
                yield break;
            }

            System.Diagnostics.Debug.Assert(barbers == Activator);
            System.Diagnostics.Debug.Assert(ActivationData != null);

            Barber1 barber = (Barber1)ActivationData;
            Points.Add(new Point(Now, barber.BlockCount));
            //Console.WriteLine("Now - " + Now + " H  H      BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);

            TimeIn = this.Now;
            Console.WriteLine(this.Now + " CusIn  Customer " + this.Name + " Shop 1 " + barber.Name + " begins cutting hair of");

            WaitOnTask(barber);
            yield return Suspend();
            // HINT: The above two lines of code can be shortened to
            //          yield return barber;

            //Console.WriteLine("Now - " + Now + " NN  NN    BlockCount - " + barbers.BlockCount + "- OutOfService - " + barbers.OutOfService + "- Reserved - " + barbers.Reserved);

            TimeOut = this.Now;
            Console.Write(this.Now + " CusOut Customer " + Name + " Shop 1 pays {0} for the haircut.",
                barber.Name);
            Console.WriteLine($"   thoi gian trong hang doi {TimeIn - this.TimeCome}" +
                              $" --- thoi gian trong he thong {TimeOut - this.TimeCome}");

            yield return barbers.Release(this);//giai phong bo nho
        }
    }
}
