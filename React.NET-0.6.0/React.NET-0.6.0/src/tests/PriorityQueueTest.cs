//=============================================================================
//=  $Id: PriorityQueueTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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

namespace React.Queue
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
    [TestFixture]
	public class PriorityQueueTest : CommonQueueTests
	{
		public PriorityQueueTest()
		{
		}

        protected override IQueue<int> GetQueueInstance()
        {
            return new PriorityQueue<int>();
        }


        [Test]
		public void DefaultPrioritizer()
		{
			// Does the priority queue default prioritizer work
			// as expected?
			Comparison<int> c = PriorityQueue<int>.DefaultPrioritizer;
			int result = c(0, 1);
			Assert.AreEqual(-1, result);
			result = c(1, 0);
			Assert.AreEqual(1, result);
			result = c(0, 0);
			Assert.AreEqual(0, result);
			result = c(1234, 1234);
			Assert.AreEqual(0, result);
			result = c(-1234, 1234);
			Assert.AreEqual(-1, result);

			// Does a new priority queue use the default prioritizer?
			PriorityQueue<int> q = new PriorityQueue<int>();
			Assert.AreEqual(c, q.Prioritizer);
		}

        [Test]
		public void OrderedPriorityOrdering()
		{
			int i;
			IQueue<int> q = new PriorityQueue<int>();
			for (i = 0; i < 100; i++)
			{
				q.Enqueue(i);
			}

			Assert.AreEqual(100, q.Count);

			for (i = 99; i >=0; i--)
			{
				int val = q.Dequeue();
				Assert.AreEqual(i, val);
			}

			Assert.AreEqual(0, q.Count);
		}

        [Test]
		public void RandomPriorityOrdering()
		{
		}

        [Test]
		public void PeekRepeats()
		{
			IQueue<int> q = new PriorityQueue<int>();
			q.Enqueue(123);
			int val = q.Peek();
			Assert.AreEqual(123, val);
			Assert.AreEqual(1, q.Count);
			val = q.Peek();
			Assert.AreEqual(123, val);
			Assert.AreEqual(1, q.Count);
			q.Enqueue(456);
			val = q.Peek();
			Assert.AreEqual(456, val);
			Assert.AreEqual(2, q.Count);
		}

        [Test]
		public void OrderedEnumeration()
		{
			Random prng = new Random();
			IQueue<int> q = new PriorityQueue<int>();
			for (int i = 0; i < 10000; i++)
			{
				int val = prng.Next(-1000000, 1000000);
				q.Enqueue(val);
			}

			int prev = Int32.MaxValue;
			foreach (int j in q)
			{
				Assert.IsTrue(prev >= j);
				prev = j;
			}
		}
	}
}
