//=============================================================================
//=  $Id: SimulationTest.cs 176 2006-10-07 15:43:17Z eroe $
//=
//=  React.NET: A discrete-event simulation library for the .NET Framework.
//=  Copyright (c) 2004, Eric K. Roe.  All rights reserved.
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
using NUnit.Framework;
using React.Tasking;

namespace React
{
    [TestFixture]
	public class SimulationTest
	{
		public SimulationTest()
		{
		}

		[Test]
		public void CreateSimulation()
		{
			Simulation sim = new Simulation();
			Assert.AreEqual(0, sim.Run());
			Assert.AreEqual(0L, sim.Now);
			Assert.AreEqual(0L, sim.StopTime);
		}

        [Test]
		public void ScheduleStop()
		{
			long stopIn = 1000L;
			Simulation sim = new Simulation();
			StopSimulation.Stop(sim, stopIn);
			Assert.AreEqual(0, sim.Run());
			Assert.AreEqual(stopIn, sim.Now);
			Assert.AreEqual(stopIn, sim.StopTime);
		}

        [Test]
		public void ScheduleMultipleStops()
		{
			Simulation sim = new Simulation();

			for (int i = 0; i < 10; i++)
			{
				long stopIn = 1000L * i + 1000L;
				StopSimulation.Stop(sim, stopIn);
			}

			Assert.AreEqual(9, sim.Run());
			Assert.AreEqual(1000L, sim.Now);
			Assert.AreEqual(1000L, sim.StopTime);
		}

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void ScheduleAfterCompleted()
        {
            Simulation sim = new Simulation();
            Delay delay = new Delay(sim, 100L);
            sim.Run(delay);
            Assert.AreEqual(SimulationState.Completed, sim.State);

            delay = new Delay(sim, 200L);
            // Simulation complete, should throw exception.
            delay.Activate(null);
        }
    }
}
