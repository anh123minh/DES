//=============================================================================
//=  $Id: TimeValueTest.cs 142 2006-04-09 20:51:42Z Eric Roe $
//=
//=  React.NET: A discrete-event simulation library for the .NET Framework.
//=  Copyright (c) 2006, Eric K. Roe.  All rights reserved.
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
using TimeValue = React.Monitoring.TimeValue;

namespace React
{
    [TestFixture]
    public class TimeValueTest
    {
        public TimeValueTest()
        {
        }

        /*=========================  TEST METHODS  =========================*/

        #region Test Methods

        [Test]
        public void DefaultConstructor()
        {
            TimeValue tv = new TimeValue();
            Assert.IsTrue(tv.IsValid);
            Assert.AreEqual(0.0, tv.Value);
            Assert.AreEqual(0L, tv.Time);
        }

        [Test]
        public void InvalidTimeValue()
        {
            TimeValue invalid = TimeValue.Invalid;
            Assert.IsFalse(invalid.IsValid);
            Assert.AreEqual(Double.NaN, invalid.Value);
            Assert.AreEqual(-1L, invalid.Time);
        }

        [Test]
        public void TestEquals()
        {
            TimeValue tv1 = new TimeValue();
            TimeValue tv2 = new TimeValue();
            Assert.AreEqual(tv1, tv2);

            tv2 = new TimeValue(1L, 3.0);
            Assert.AreNotEqual(tv1, tv2);

            tv1 = new TimeValue(1L, 3.0);
            Assert.AreEqual(tv1, tv2);
        }

        [Test]
        public void TestOperators()
        {
            TimeValue tv1 = new TimeValue();
            TimeValue tv2 = new TimeValue();
            Assert.IsTrue(tv1 == tv2);
            Assert.IsFalse(tv1 != tv2);

            tv2 = new TimeValue(1L, 3.0);
            Assert.IsFalse(tv1 == tv2);
            Assert.IsTrue(tv1 != tv2);

            tv1 = new TimeValue(1L, 3.0);
            Assert.IsTrue(tv1 == tv2);
            Assert.IsFalse(tv1 != tv2);
        }

        #endregion
    }
}
