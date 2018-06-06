//=============================================================================
//=  $Id: ProcessTest.cs 148 2006-06-10 17:18:29Z eroe $
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
	public class ProcessTest
	{
		private Simulation _sim = null;
		private int _count = 0;
		
		public ProcessTest()
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
			CounterProcess cp = new CounterProcess(_sim, 10);
			_sim.Run(new Task [] {cp});
			Assert.AreEqual(10, cp.CurrentIteration);
			Assert.AreEqual(100L, _sim.Now);
		}

        [Test]
		public void DelegateCountToTen()
		{
			ProcessSteps func = new ProcessSteps(CountProcess);
			Process p = new Process(_sim, func);
			_sim.Run(new Task [] {p});
			Assert.AreEqual(10, _count);
			Assert.AreEqual(100L, _sim.Now);
		}

		private IEnumerator<Task> CountProcess(Process process, object data)
		{
			while (_count < 10)
			{
				_count++;
				yield return process.Delay(10);
			}

			yield break;
		}

		[
			Test,
			ExpectedException(typeof(ArgumentException))
		]
		public void InvalidDelay()
		{
			CounterProcess cp = new CounterProcess(_sim, 10);
			cp.DelayTime = -1L;
			_sim.Run(new Task [] {cp});
		}

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void DoubleInterrupt()
        {
            Process interruptee = new Process(_sim, Interruptee, false);
            interruptee.Activate(null, 10L);
            Process p = new Process(_sim, Interruptor, interruptee);
            p.Activate(null, 5L, TaskPriority.Elevated);
            p = new Process(_sim, Interruptor, interruptee);
            p.Activate(null, 5L, TaskPriority.Elevated);
            _sim.Run();
        }

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void ActivateWithoutClear()
        {
            Process p = new Process(_sim, Interruptee, false);
            p.Activate(null, 10L);
            p = new Process(_sim, Interruptor, p);
            p.Activate(null, 5L);
            _sim.Run();
        }

        [Test]
        public void ActivateWithClear()
        {
            Process p = new Process(_sim, Interruptee, true);
            p.Activate(null, 10L);
            p = new Process(_sim, Interruptor, p);
            p.Activate(null, 5L);
            _sim.Run();
        }

        private IEnumerator<Task> Interruptor(Process process, object data)
        {
            Task task = (Task)data;
            task.Interrupt(process);
            yield break;
        }

        private IEnumerator<Task> Interruptee(Process process, object data)
        {
            Assert.IsTrue(process.Interrupted);
            if ((bool)data)
                process.ClearInterrupt();

            process.Activate(null, 10L);
            yield return process;
            yield break;
        }
	}

	class CounterProcess : React.Process
	{
		private int _count = 0;
		private int _desiredCount;
		private long _delay = 10;
		
		public CounterProcess(Simulation sim, int count) : base(sim)
		{
			_desiredCount = count;
		}

		public int CurrentIteration
		{
			get {return _count;}
		}

		public int TotalIterations
		{
			get {return _desiredCount;}
		}

		public long DelayTime
		{
			get {return _delay;}
			set {_delay = value;}
		}

		protected void IncrementIteration()
		{
			_count++;
		}

		protected override IEnumerator<Task> GetProcessSteps()
		{
			while (CurrentIteration < TotalIterations)
			{
				IncrementIteration();
				yield return Delay(DelayTime);
			}

			yield break;
		}
	}
}
