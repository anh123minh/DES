//=============================================================================
//=  $Id: DelayTaskTest.cs 163 2006-09-03 15:15:33Z eroe $
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
using System.Collections.Generic;
using NUnit.Framework;

namespace React
{
	/// <summary>
	/// 
	/// </summary>
    [TestFixture]
	public class DelayTaskTest
	{
		private Simulation _sim = null;
		
		public DelayTaskTest()
		{
		}

		[SetUp]
		public void InitTest()
		{
			_sim = new Simulation();
		}

		[TearDown]
		public void UninitTest()
		{
			_sim = null;
		}

        [Test]
		public void CountToTen()
		{
			TestProcess p = new TestProcess(_sim, 10);
			_sim.Run(new Task [] {p});
			Assert.AreEqual(10, p.CurrentIteration);
			Assert.AreEqual(100L, _sim.Now);
		}

		[
            Test,
			ExpectedException(typeof(ArgumentException))
		]
		public void InvalidDelay()
		{
			TestProcess p = new TestProcess(_sim, 10);
			p.DelayTime = -1L;
			_sim.Run(new Task [] {p});
		}

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void DisallowTimeChange()
        {
            Task delay = new React.Tasking.Delay(_sim, 10);
            delay.Activate(null);
            ((React.Tasking.Delay)delay).Time += 1;
        }

        private class TestProcess : CounterProcess
		{
			public TestProcess(Simulation sim, int count) : base(sim, count)
			{
			}

			protected override IEnumerator<Task> GetProcessSteps()
			{
				while (CurrentIteration < TotalIterations)
				{
					IncrementIteration();
					yield return new React.Tasking.Delay(Simulation, DelayTime);
				}

				yield break;
			}
			
		}
	}
}
