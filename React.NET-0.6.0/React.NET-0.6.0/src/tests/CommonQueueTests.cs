//=============================================================================
//=  $Id: CommonQueueTests.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
	public abstract class CommonQueueTests
	{
		public CommonQueueTests()
		{
		}

        protected abstract IQueue<int> GetQueueInstance();

        [Test]
        public void CreatedEmpty()
		{
            IQueue<int> q = GetQueueInstance();
			Assert.AreEqual(0, q.Count);
		}

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void DequeueEmpty()
		{
            IQueue<int> q = GetQueueInstance();
			int val = q.Dequeue();
		}

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void PeekEmpty()
		{
            IQueue<int> q = GetQueueInstance();
			int val = q.Dequeue();
		}
	}
}
