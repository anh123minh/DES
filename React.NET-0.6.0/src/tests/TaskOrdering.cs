//=============================================================================
//=  $Id: TaskOrdering.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
using NUnit.Framework;
using React.Queue;

namespace React
{
    /// <summary>
    /// Summary description for TaskOrdering
    /// </summary>
    [TestFixture]
    public class TaskOrdering
    {
        public TaskOrdering()
        {
        }

        [Test]
        public void OrderedByTime()
        {
            IQueue<int> fifo = new FifoQueue<int>();
            Simulation sim = new Simulation();
            for (int i = 0; i < 100; i++)
            {
                Task t = new OrderingTask(sim, i);
                t.Activate(null, i * 100, fifo);
            }
            int tasksNotRun = sim.Run();
            Assert.AreEqual(0, tasksNotRun);
            Assert.AreEqual(100, fifo.Count);

            for (int v = 0; fifo.Count > 0; v++)
            {
                Assert.AreEqual(v, fifo.Dequeue());
            }
        }

        [Test]
        public void OrderedByTimeReverse()
        {
            IQueue<int> fifo = new FifoQueue<int>();
            Simulation sim = new Simulation();
            for (int i = 0; i < 100; i++)
            {
                Task t = new OrderingTask(sim, 100 - i - 1);
                t.Activate(null, 1000 - i, fifo);
            }
            int tasksNotRun = sim.Run();
            Assert.AreEqual(0, tasksNotRun);
            Assert.AreEqual(100, fifo.Count);

            for (int v = 0; fifo.Count > 0; v++)
            {
                Assert.AreEqual(v, fifo.Dequeue());
            }
        }

        [Test]
        public void OrderedByPriority()
        {
            IQueue<int> fifo = new FifoQueue<int>();
            Simulation sim = new Simulation();
            for (int i = 0; i < 100; i++)
            {
                Task t = new OrderingTask(sim, i);
                t.Activate(null, 123L, fifo, 100 - i - 1);
            }
            int tasksNotRun = sim.Run();
            Assert.AreEqual(0, tasksNotRun);
            Assert.AreEqual(100, fifo.Count);

            for (int v = 0; fifo.Count > 0; v++)
            {
                Assert.AreEqual(v, fifo.Dequeue());
            }
        }

        [Test]
        public void OrderedByPriorityReversed()
        {
            IQueue<int> fifo = new FifoQueue<int>();
            Simulation sim = new Simulation();
            for (int i = 0; i < 100; i++)
            {
                Task t = new OrderingTask(sim, 100 - i - 1);
                t.Activate(null, 123L, fifo, 1000 + i);
            }
            int tasksNotRun = sim.Run();
            Assert.AreEqual(0, tasksNotRun);
            Assert.AreEqual(100, fifo.Count);

            for (int v = 0; fifo.Count > 0; v++)
            {
                Assert.AreEqual(v, fifo.Dequeue());
            }
        }

        class OrderingTask : Task
        {
            private int value;
            public OrderingTask(Simulation sim, int v) : base(sim)
            {
                value = v;
            }
            protected override void ExecuteTask(object activator, object data)
            {
                IQueue<int> q = data as IQueue<int>;
                q.Enqueue(value);
            }
        }
    }
}
