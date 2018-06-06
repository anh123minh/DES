//=============================================================================
//=  $Id: TimeWeightedVarianceTest.cs 164 2006-09-03 22:12:23Z eroe $
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
    public class TimeWeightedVarianceTest
    {
        private double _valueForTesting;

        public event EventHandler<ValueChangedEventArgs> TestValueChanged;

        public TimeWeightedVarianceTest()
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
            TimeWeightedVariance twv = new TimeWeightedVariance(null);
        }

        [Test]
        public void DefaultConstructor()
        {
            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());
            Assert.AreEqual(0, twv.Observations);
            Assert.IsTrue(Double.IsNaN(twv.Value));

            TimeValue lastObservation = twv.LastObservation;
            Assert.IsFalse(lastObservation.IsValid);
        }

        [Test]
        public void VarianceOfSameObservations()
        {
            double d = 321.0;
            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());

            for (int i = 0; i < 1000; i++)
            {
                twv.Observe(d, i);
            }

            Assert.AreEqual(1000, twv.Observations);
            Assert.AreEqual(0.0, twv.Value);

            TimeValue lastObservation = twv.LastObservation;
            Assert.IsTrue(lastObservation.IsValid);
            Assert.AreEqual(d, lastObservation.Value);
            Assert.AreEqual(999, lastObservation.Time);
        }

        [Test]
        public void VarianceOfZeroWeights()
        {
            double d = 321.0;
            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());

            for (int i = 0; i < 1000; i++)
            {
                twv.Observe(d, 0L);
            }

            Assert.AreEqual(Double.NaN, twv.Value);
        }

        [Test]
        public void VarianceOfFewObservations()
        {
            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());
            twv.Observe(3.0, 0L);
            twv.Observe(2.0, 4L);
            twv.Observe(6.0, 7L);
            twv.Observe(4.0, 8L);
            twv.Observe(2.0, 10L);
            Assert.AreEqual(1.36, twv.Value,0.00001);
        }

        [Test]
        public void VarianceOfManyObservations()
        {
            IUniform valGen = UniformStreams.DefaultStreams.GetUniform();
            IUniform timeGen = UniformStreams.DefaultStreams.GetUniform();
            TimeValue lastObservation = new TimeValue(0L, valGen.NextDouble() * 100.0);
            double[] weights = new double[1000];
            double[] values = new double[1000];

            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());
            TimeWeightedMean twm = new TimeWeightedMean(new Simulation());

            twm.Observe(lastObservation.Value, lastObservation.Time);
            twv.Observe(lastObservation.Value, lastObservation.Time);

            for (int i = 0; i < 1000; i++)
            {
                long newTime = lastObservation.Time + (long)(timeGen.NextDouble() * 10.0);
                TimeValue newObservation = new TimeValue(newTime, valGen.NextDouble() * 100.0);
                double weight = newObservation.Time - lastObservation.Time;

                twm.Observe(newObservation.Value, newObservation.Time);
                twv.Observe(newObservation.Value, newObservation.Time);

                weights[i] = weight;
                values[i] = lastObservation.Value;

                lastObservation = newObservation;
            }

            double sumW = 0.0;
            double sumWVals = 0.0;
            for (int i = 0; i < 1000; i++)
            {
                sumW += weights[i];
                sumWVals += weights[i] * Math.Pow(values[i] - twm.Value, 2.0);
            }

            double variance = sumWVals / sumW;
            Assert.AreEqual(variance, twv.Value, 0.00001);
        }

        [Test]
        public void AttachAndDetach()
        {
            TimeWeightedVariance twv = new TimeWeightedVariance(new Simulation());
            twv.ObserveOnAttach = true;
            twv.ObserveOnDetach = true;

            _valueForTesting = 5.5;
            twv.Attach(this, "ValueForTesting");
            TimeValue lastObservation = twv.LastObservation;
            Assert.AreEqual(1, twv.Observations);
            Assert.AreEqual(5.5, lastObservation.Value);

            _valueForTesting = 10.0;
            twv.Detach(this, "ValueForTesting");
            lastObservation = twv.LastObservation;
            Assert.AreEqual(2, twv.Observations);
            Assert.AreEqual(10.0, lastObservation.Value);
        }
    }
}
