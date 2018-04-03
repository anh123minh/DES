//=============================================================================
//=  $Id: RecorderTest.cs 164 2006-09-03 22:12:23Z eroe $
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
    /// Summary description for RecorderTest
    /// </summary>
    [TestFixture]
    public class RecorderTest
    {
        public event EventHandler<ValueChangedEventArgs> SampleChanged;
        public event EventHandler<ValueChangedEventArgs> TimedSampleChanged;

        private double _sample;
        private long _time;

        public RecorderTest()
        {
        }

        [MonitorUsing("SampleChanged")]
        public double SampleProperty
        {
            get { return _sample; }
            set
            {
                double old = _sample;
                _sample = value;
                if (SampleChanged != null)
                {
                    SampleChanged(this,
                        new ValueChangedEventArgs(old, _sample));
                }
            }
        }

        [MonitorUsing("TimedSampleChanged")]
        public double TimedSampleProperty
        {
            get { return _sample; }
            set
            {
                double old = _sample;
                _sample = value;
                if (TimedSampleChanged != null)
                {
                    TimedSampleChanged(this,
                        new ValueChangedEventArgs(old, _sample, _time));
                }
            }
        }

        [Test]
        public void DefaultConstructor()
        {
            Recorder r = new Recorder();
            Assert.AreEqual(0, r.Count);
        }

        [Test]
        public void RecordDataSameTime()
        {
            Recorder r = new Recorder();
            r.Attach(this, "SampleProperty");

            for (int i = 0; i < 1000; i++)
            {
                SampleProperty = (double) i;
            }

            Assert.AreEqual(1000, r.Count);

            int j = 0;
            foreach (TimeValue tv in r)
            {
                Assert.AreEqual(j++, tv.Value);
                Assert.AreEqual(-1L, tv.Time);
            }
        }

        [Test]
        public void RecordDataVaryingTimes()
        {
            Recorder r = new Recorder();
            r.Attach(this, "TimedSampleProperty");

            for (int i = 0; i < 1000; i++)
            {
                if (i % 10 == 0)
                    _time++;

                TimedSampleProperty = (double)i;
            }

            Assert.AreEqual(1000, r.Count);

            int j = 0;
            long tm = 0;
            foreach (TimeValue tv in r)
            {
                if (j % 10 == 0)
                    tm++;

                Assert.AreEqual(j++, tv.Value);
                Assert.AreEqual(tm, tv.Time);
            }
        }
    }
}
