//=============================================================================
//=  $Id: CountTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
using React.Monitoring;

namespace React
{
    /// <summary>
    /// Summary description for CountTest
    /// </summary>
    [TestFixture]
    public class CountTest
    {
        public CountTest()
        {
        }

        [Test]
        public void DefaultConstructor()
        {
            Count count = new Count();
            Assert.AreEqual(0, count.Observations);
            Assert.AreEqual(0.0, count.Value);
            Assert.AreEqual(1, count.IncrementBy);
        }

        [Test]
        public void InitialValueConstructor()
        {
            Count count = new Count(5);
            Assert.AreEqual(0, count.Observations);
            Assert.AreEqual(5.0, count.Value);
            Assert.AreEqual(1, count.IncrementBy);

            count = new Count(-30);
            Assert.AreEqual(0, count.Observations);
            Assert.AreEqual(-30.0, count.Value);
            Assert.AreEqual(1, count.IncrementBy);
        }

        [Test]
        public void IncrementValueConstructor()
        {
            Count count = new Count(0, 2);
            Assert.AreEqual(0, count.Observations);
            Assert.AreEqual(0.0, count.Value);
            Assert.AreEqual(2, count.IncrementBy);
        }

        [Test]
        public void CountDefault()
        {
            Count count = new Count();
            count.Observe(100);
            count.Observe(200);
            count.Observe(-100);
            count.Observe(0);

            Assert.AreEqual(4, count.Observations);
            Assert.AreEqual(4.0, count.Value);
        }

        [Test]
        public void CountByFive()
        {
            Count count = new Count(0, 5);
            count.Observe(100);
            count.Observe(200);
            count.Observe(-100);
            count.Observe(0);
            count.Observe(0);
            count.Observe(0);

            Assert.AreEqual(6, count.Observations);
            Assert.AreEqual(30.0, count.Value);
        }

        [Test]
        public void CountBy2From10()
        {
            Count count = new Count(10, 2);
            count.Observe(0);
            count.Observe(0);
            count.Observe(0);

            Assert.AreEqual(3, count.Observations);
            Assert.AreEqual(16.0, count.Value);
        }

        [Test]
        public void CountBackwards()
        {
            Count count = new Count(10, -1);
            count.Observe(0);
            count.Observe(0);
            count.Observe(0);
            count.Observe(0);
            count.Observe(0);

            Assert.AreEqual(5, count.Observations);
            Assert.AreEqual(5.0, count.Value);
        }
    }
}
