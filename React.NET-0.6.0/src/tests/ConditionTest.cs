//=============================================================================
//=  $Id: ConditionTest.cs 133 2006-01-21 17:32:13Z Eric Roe $
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

using System.Collections.Generic;
using React;
using React.Tasking;
using NUnit.Framework;

namespace React
{
    /// <summary>
    ///This is a test class for React.Condition and is intended
    ///to contain all React.Condition Unit Tests
    ///</summary>
    [TestFixture]
    public class ConditionTest
    {
        /// <summary>
        ///A test case for Condition ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            Condition target = new Condition();
            Assert.AreEqual(null, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(true, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);
        }

        /// <summary>
        ///A test case for Condition (bool)
        ///</summary>
        [Test]
        public void ConstructorTest1()
        {
            bool resumeAll = false;
            Condition target = new Condition(resumeAll);
            Assert.AreEqual(null, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(false, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);

            resumeAll = true;
            target = new Condition(resumeAll);
            Assert.AreEqual(null, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(true, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);
        }

        /// <summary>
        ///A test case for Condition (string)
        ///</summary>
        [Test]
        public void ConstructorTest2()
        {
            string name = "TEST NAME";
            Condition target = new Condition(name);
            Assert.AreEqual(name, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(true, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);
        }

        /// <summary>
        ///A test case for Condition (string, bool)
        ///</summary>
        [Test]
        public void ConstructorTest3()
        {
            string name = "TEST NAME";
            bool resumeAll = false;
            Condition target = new Condition(name, resumeAll);
            Assert.AreEqual(name, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(false, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);

            resumeAll = true;
            target = new Condition(name, resumeAll);
            Assert.AreEqual(name, target.Name);
            Assert.AreEqual(true, target.AutoReset);
            Assert.AreEqual(true, target.ResumeAllOnSignal);
            Assert.IsFalse(target.Signalled);
        }

        [Test]
        public void BlockedWhenNotSignalled()
        {
            Condition target = new Condition();
            Simulation sim = new Simulation();
            Task nop = new NoOperation(sim);
            Task blk = target.Block(nop);
            sim.Run(blk);
            Assert.AreEqual(1, target.BlockCount);
        }

        [Test]
        public void NotBlockedWhenSignalled()
        {
            Condition target = new Condition();
            target.AutoReset = false;
            target.Signal();
            Simulation sim = new Simulation();
            Task nop = new NoOperation(sim);
            Task blk = target.Block(nop);
            nop.WaitOnTask(blk);
            Assert.IsTrue(target.Signalled);
            sim.Run(blk);
            Assert.AreEqual(0, target.BlockCount);
        }

        [Test]
        public void ResumeOnSignal()
        {
            Condition target = new Condition();
            Simulation sim = new Simulation();
            Task unblock = new Process(sim, SignalCondition, target);
            Task nop = new NoOperation(sim);
            Task block = target.Block(nop);
            nop.WaitOnTask(block);
            sim.Run(new Task[] { block, unblock });
            Assert.IsFalse(target.Signalled);
            Assert.AreEqual(0, target.BlockCount);
            Assert.AreEqual(10L, sim.StopTime);
        }

        //====================================================================
        //====                   Private Implementation                   ====
        //====================================================================

        private IEnumerator<Task> SignalCondition(Process p, object data)
        {
            Condition c = (Condition)data;
            yield return p.Delay(10);
            Assert.AreEqual(1, c.BlockCount);
            c.Signal();
            yield break;
        }
    }
}
