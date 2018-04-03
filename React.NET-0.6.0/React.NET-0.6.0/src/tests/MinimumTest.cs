//=============================================================================
//=  $Id: MinimumTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
    /// Summary description for MinimumTest
    /// </summary>
    [TestFixture]
    public class MinimumTest
    {
        public MinimumTest()
        {
        }

        [Test]
        public void DefaultConstructor()
        {
            Minimum min = new Minimum();
            Assert.AreEqual(0, min.Observations);
            Assert.IsTrue(Double.IsNaN(min.Value));
        }

        [Test]
        public void InitialValueConstructor()
        {
            Minimum min = new Minimum(100.0);
            Assert.AreEqual(1, min.Observations);
            Assert.AreEqual(100.0, min.Value);
        }

        [Test]
        public void MinimumValue()
        {
            Minimum min = new Minimum();

            for (int i = 0; i < 1000; i++)
            {
                min.Observe((double)i);
            }

            Assert.AreEqual(1000, min.Observations);
            Assert.AreEqual(0.0, min.Value);

            min.Observe(-80000.6);
            Assert.AreEqual(1001, min.Observations);
            Assert.AreEqual(-80000.6, min.Value);

            min.Observe(8000.5);
            Assert.AreEqual(1002, min.Observations);
            Assert.AreEqual(-80000.6, min.Value);
        }
    }
}
