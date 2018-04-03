//=============================================================================
//=  $Id: MeanTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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
    /// Summary description for MeanTest
    /// </summary>
    [TestFixture]
    public class MeanTest
    {
        public MeanTest()
        {
        }

        [Test]
        public void DefaultConstructor()
        {
            Mean avg = new Mean();
            Assert.AreEqual(0, avg.Observations);
            Assert.IsTrue(Double.IsNaN(avg.Value));
        }

        [Test]
        public void InitialValueConstructor()
        {
            Mean avg = new Mean(100.0);
            Assert.AreEqual(1, avg.Observations);
            Assert.AreEqual(100.0, avg.Value);
        }

        [Test]
        public void MeanOfSameValues()
        {
            double d = 344.5;
            Mean avg = new Mean();

            for (int i = 0; i < 1000; i++)
            {
                avg.Observe(d);
            }

            Assert.AreEqual(1000, avg.Observations);
            Assert.AreEqual(d, avg.Value);
        }

        [Test]
        public void MeanOfDifferingValues()
        {
            Random r = new Random();
            double sum = 0.0;
            Mean avg = new Mean();

            for (int i = 0; i < 1000; i++)
            {
                double d = r.NextDouble() * 100.0;
                avg.Observe(d);
                sum += d;
            }

            Assert.AreEqual(1000, avg.Observations);
            Assert.AreEqual(sum / 1000.0, avg.Value);
        }
    }
}
