//=============================================================================
//=  $Id: FifoQueueTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
	public class FifoQueueTest : CommonQueueTests
	{
		public FifoQueueTest()
		{
		}

        protected override IQueue<int> GetQueueInstance()
        {
            return new FifoQueue<int>();
        }


        [Test]
		public void FifoOrdering()
		{
			int i;
			IQueue<int> q = new FifoQueue<int>();
			for (i = 0; i < 100; i++)
			{
				q.Enqueue(i);
			}

			Assert.AreEqual(100, q.Count);

			for (i = 0; i < 100; i++)
			{
				int val = q.Dequeue();
				Assert.AreEqual(i, val);
			}

			Assert.AreEqual(0, q.Count);
		}

        [Test]
		public void PeekRepeats()
		{
			IQueue<int> q = new FifoQueue<int>();
			q.Enqueue(123);
			int val = q.Peek();
			Assert.AreEqual(123, val);
			Assert.AreEqual(1, q.Count);
			val = q.Peek();
			Assert.AreEqual(123, val);
			Assert.AreEqual(1, q.Count);
			q.Enqueue(456);
			val = q.Peek();
			Assert.AreEqual(123, val);
			Assert.AreEqual(2, q.Count);
		}

        [Test]
		public void EnumeratorIsFifo()
		{
			int i;
			IQueue<int> q = new FifoQueue<int>();
			for (i = 0; i < 100; i++)
			{
				q.Enqueue(i);
			}

			i = 0;
			foreach (int j in q) {
				Assert.AreEqual(i, j);
				i++;
			}
		}
	}
}
