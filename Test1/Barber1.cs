//=============================================================================
//=  $Id: Barber.cs 128 2005-12-04 20:12:00Z Eric Roe $
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

using System.Collections.Generic;
using React;

namespace Test1
{
    /// <summary>
    /// The barber <see cref="Process"/>.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Barber"/> is a member of a <see cref="TrackedResource"/>
    /// and, when activated, cuts a <see cref="Customer"/> object's hair.
    /// </remarks>
    internal class Barber1 : Process
    {
        internal Barber1(Simulation sim, string name) : base(sim)
        {
            this.Name = name;
        }

        protected override IEnumerator<Task> GetProcessSteps()
        {
            //Console.WriteLine(Name + " begins cutting customer's hair ...");
            //yield return Delay(22);
            //Console.WriteLine(Name + " finishes cutting customer's hair.");
            //yield break;

            //Console.WriteLine(this.Now + " - " + Name + " Shop 1 begins cutting customer's hair ... Shop 1");
            yield return Delay(0);
            //Console.WriteLine(this.Now + "-" + Name + " finishes cutting customer's hair.");
            yield break;
        }
    }
}
