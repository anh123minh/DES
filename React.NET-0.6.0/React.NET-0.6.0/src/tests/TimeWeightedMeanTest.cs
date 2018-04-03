//=============================================================================
//=  $Id: TimeWeightedMeanTest.cs 164 2006-09-03 22:12:23Z eroe $
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
using React.Monitoring;
using React.Distribution;

namespace React
{
    [TestFixture]
    public class TimeWeightedMeanTest
    {
        private double _valueForTesting;

        public event EventHandler<ValueChangedEventArgs> TestValueChanged;

        public TimeWeightedMeanTest()
        {
        }

        [MonitorUsing("TestValueChanged")]
        public double ValueForTesting
        {
            get { return _valueForTesting; }
            set
            {
                double oldValue = _valueForTesting;
                _valueForTesting = value;
                if (TestValueChanged != null)
                {
                    TestValueChanged(this, new ValueChangedEventArgs(oldValue, value));
                }
            }
        }

        [
            Test,
            ExpectedException(typeof(ArgumentNullException))
        ]
        public void RequiresNonNullSimulation()
        {
            TimeWeightedMean twm = new TimeWeightedMean(null);
        }

        [Test]
        public void DefaultConstructor()
        {
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());
            Assert.AreEqual(0, twm.Observations);
            Assert.IsTrue(Double.IsNaN(twm.Value));

            TimeValue lastObservation = twm.LastObservation;
            Assert.IsFalse(lastObservation.IsValid);
        }

        [Test]
        public void MeanOfSameObservations()
        {
            double d = 123.0;
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());

            for (int i = 0; i < 1000; i++)
            {
                twm.Observe(d, i);
            }

            Assert.AreEqual(1000, twm.Observations);
            Assert.AreEqual(d, twm.Value);

            TimeValue lastObservation = twm.LastObservation;
            Assert.IsTrue(lastObservation.IsValid);
            Assert.AreEqual(d, lastObservation.Value);
            Assert.AreEqual(999, lastObservation.Time);
        }

        [Test]
        public void MeanOfZeroWeights()
        {
            double d = 123.0;
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());

            for (int i = 0; i < 1000; i++)
            {
                twm.Observe(d, 0L);
            }

            Assert.AreEqual(Double.NaN, twm.Value);
        }

        [Test]
        public void MeanOfFewObservations()
        {
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());
            twm.Observe(3.0, 0L);
            twm.Observe(2.0, 4L);
            twm.Observe(6.0, 7L);
            twm.Observe(4.0, 8L);
            twm.Observe(2.0, 10L);
            Assert.AreEqual(3.2, twm.Value);
        }

        [Test]
        public void MeanOfManyObservations()
        {
            IUniform valGen = UniformStreams.DefaultStreams.GetUniform();
            IUniform timeGen = UniformStreams.DefaultStreams.GetUniform();
            double weightedSum = 0.0;
            double sumOfWeights = 0.0;
            TimeValue lastObservation = new TimeValue(0L, valGen.NextDouble() * 100.0);

            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());

            twm.Observe(lastObservation.Value, lastObservation.Time);

            for (int i = 0; i < 1000; i++)
            {
                long newTime = lastObservation.Time + (long)(timeGen.NextDouble() * 10.0);
                TimeValue newObservation = new TimeValue(newTime, valGen.NextDouble() * 100.0);
                double weight = newObservation.Time - lastObservation.Time;
                weightedSum += weight * lastObservation.Value;
                sumOfWeights += weight;

                twm.Observe(newObservation.Value, newObservation.Time);
                lastObservation = newObservation;
            }

            double mean = weightedSum / sumOfWeights;
            Assert.AreEqual(mean, twm.Value);
        }

        [Test]
        public void AttachAndDetach()
        {
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());
            twm.ObserveOnAttach = true;
            twm.ObserveOnDetach = true;

            _valueForTesting = 5.5;
            twm.Attach(this, "ValueForTesting");
            TimeValue lastObservation = twm.LastObservation;
            Assert.AreEqual(1, twm.Observations);
            Assert.AreEqual(5.5, lastObservation.Value);

            _valueForTesting = 10.0;
            twm.Detach(this, "ValueForTesting");
            lastObservation = twm.LastObservation;
            Assert.AreEqual(2, twm.Observations);
            Assert.AreEqual(10.0, lastObservation.Value);
        }
    }
}
