//=============================================================================
//=  $Id: MaximumTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
    /// Summary description for MaximumTest
    /// </summary>
    [TestFixture]
    public class MaximumTest
    {
        public MaximumTest()
        {
        }

        [Test]
        public void DefaultConstructor()
        {
            Maximum max = new Maximum();
            Assert.AreEqual(0, max.Observations);
            Assert.IsTrue(Double.IsNaN(max.Value));
        }

        [Test]
        public void InitialValueConstructor()
        {
            Maximum max = new Maximum(100.0);
            Assert.AreEqual(1, max.Observations);
            Assert.AreEqual(100.0, max.Value);
        }

        [Test]
        public void MaximumValue()
        {
            Maximum max = new Maximum();

            for (int i = 0; i < 1000; i++)
            {
                max.Observe((double)i);
            }

            Assert.AreEqual(1000, max.Observations);
            Assert.AreEqual(999.0, max.Value);

            max.Observe(-80000.6);
            Assert.AreEqual(1001, max.Observations);
            Assert.AreEqual(999.0, max.Value);

            max.Observe(8000.5);
            Assert.AreEqual(1002, max.Observations);
            Assert.AreEqual(8000.5, max.Value);
        }
    }
}
