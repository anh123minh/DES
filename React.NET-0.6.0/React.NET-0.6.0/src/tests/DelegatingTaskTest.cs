//=============================================================================
//=  $Id: DelegatingTaskTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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

namespace React.Tasking
{
	/// <summary>
	/// </summary>
    [TestFixture]
	public class DelegatingTaskTest
	{
		private Simulation sim = null;
		private object temp = null;
		
		public DelegatingTaskTest()
		{
		}

		[SetUp]
		public void InitTest()
		{
			sim = new Simulation();
			temp = null;
		}

        [Test]
		public void RunAndGetMessage()
		{
			Delegating task = new Delegating(sim,
				new DelegatingTaskHandler(SetMessageViaTemp));
			sim.Run(new Task [] {task});
			Assert.AreEqual("Hello", temp);
			Assert.AreEqual(0L, sim.Now);
		}

        [Test]
		public void SimpleCounter()
		{
			temp = 0;
			Delegating task = new Delegating(sim,
				new DelegatingTaskHandler(CountUsingTemp));
			sim.Run(new Task [] {task});
			Assert.AreEqual(10, (int) temp);
		}
		
		private bool SetMessageViaTemp(Delegating task, object activator,
			object data)
		{
			temp = "Hello";
			return true;
		}

		private bool CountUsingTemp(Delegating task, object activator,
			object data)
		{
			int i = (int) temp;
			i++;
			temp = i;
			if (i < 10)
				task.Activate(null, i * 100);

			return i >= 10;
		}
	}
}
