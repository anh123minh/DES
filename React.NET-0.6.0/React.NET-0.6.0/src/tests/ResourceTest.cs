//=============================================================================
//=  $Id: ResourceTest.cs 149 2006-06-10 17:23:26Z eroe $
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
using System.Collections.Generic;
using NUnit.Framework;
using React;
using React.Tasking;

namespace React
{
	[TestFixture]
	public class ResourceTest
	{
		private static readonly string [] Names = {
	        "Frodo", "Merry", "Pippen", "Sam"
		};

		public ResourceTest()
		{
		}

        [Test]
		public void CreateAnonymousResource()
		{
			IResource resource = Resource.Create(5);
			Assert.IsTrue(resource is AnonymousResource);
			Assert.AreEqual(5, resource.Count);
			Assert.AreEqual(5, resource.Free);
			Assert.AreEqual(0, resource.OutOfService);
			Assert.AreEqual(0, resource.InUse);
		}

        [Test]
		public void CreateTrackedResource()
		{
			IResource resource = Resource.Create(Names);
			Assert.IsTrue(resource is TrackedResource);
			Assert.AreEqual(Names.Length, resource.Count);
			Assert.AreEqual(Names.Length, resource.Free);
			Assert.AreEqual(0, resource.OutOfService);
			Assert.AreEqual(0, resource.InUse);

			resource = Resource.Create(new int [] {5000});
			Assert.IsTrue(resource is TrackedResource);
			Assert.AreEqual(1, resource.Count);
			Assert.AreEqual(1, resource.Free);
			Assert.AreEqual(0, resource.OutOfService);
			Assert.AreEqual(0, resource.InUse);
		}

        [Test]
		public void AnonymousOutOfService()
		{
			IResource resource = Resource.Create(5);
			int free = resource.Free;
			int oos = 0;
			while (resource.Free > 0)
			{
				free--;
				oos++;
				resource.OutOfService++;
				Assert.AreEqual(oos, resource.OutOfService);
				Assert.AreEqual(free, resource.Free);
			}

			resource.OutOfService = 3;
			Assert.AreEqual(resource.Count - resource.OutOfService, resource.Free);
		}

        [Test]
		public void TrackedOutOfService()
		{
			IResource resource = Resource.Create(Names);
			int free = resource.Free;
			int oos = 0;
			while (resource.Free > 0)
			{
				free--;
				oos++;
				resource.OutOfService++;
				Assert.AreEqual(oos, resource.OutOfService);
				Assert.AreEqual(free, resource.Free);
			}

			resource.OutOfService = 3;
			Assert.AreEqual(resource.Count - resource.OutOfService, resource.Free);
		}

        [Test]
        public void AcquireAllResources()
        {
            Resource resource = Resource.Create(10);
            resource.AllowOwnMany = true;
            Simulation sim = new Simulation();
            Process process = new Process(sim, AcquireAll, resource);

            Assert.AreEqual(resource.Count, resource.Free);
            sim.Run(process);
            Assert.AreEqual(0, resource.Free);
            Assert.AreEqual(resource.Count, resource.InUse);
        }

        [
            Test,
            ExpectedException(typeof(InvalidOperationException))
        ]
        public void AcquireAllDisallowed()
        {
            Resource resource = Resource.Create(10);
            resource.AllowOwnMany = false;
            Simulation sim = new Simulation();
            Process process = new Process(sim, AcquireAll, resource);
            sim.Run(process);
        }

        [Test]
        public void InterruptedAcquire()
        {
            IResource resource = Resource.Create(3);
            Simulation sim = new Simulation();
            Process process = new Process(sim, AcquireOneInterrupted, resource);
            process.Activate(null);
            Assert.AreEqual(resource.Count, resource.Free);
            sim.Run();
            Assert.AreEqual(resource.Count, resource.Free);
        }

        [Test]
        public void SetOutOfServiceAnonymous()
        {
            IResource resource = Resource.Create(7);
            Simulation sim = new Simulation();
            resource.OutOfService = 2;
            Task[] tasks = new Task[resource.Count];
            for (int i = 0; i < resource.Count; i++)
            {
                Task nop = new NoOperation(sim);
                tasks[i] = resource.Acquire(nop);
                nop.WaitOnTask(tasks[i]);
            }
            sim.Run(tasks);
            Assert.AreEqual(resource.Count - resource.OutOfService,
                resource.InUse);
        }

        [Test]
        public void SetOutOfServiceTracked()
        {
            IResource resource = Resource.Create(Names);
            Simulation sim = new Simulation();
            resource.OutOfService = 2;
            Task[] tasks = new Task[resource.Count];
            for (int i = 0; i < resource.Count; i++)
            {
                Task nop = new NoOperation(sim);
                tasks[i] = resource.Acquire(nop);
                nop.WaitOnTask(tasks[i]);
            }
            sim.Run(tasks);
            Assert.AreEqual(Names.Length - resource.OutOfService,
                resource.InUse);
        }

        [Test]
        public void SetInServiceAnonymous()
        {
            IResource resource = Resource.Create(6);
            Simulation sim = new Simulation();
            resource.OutOfService = resource.Count;
            Task[] tasks = new Task[resource.Count + 1];
            for (int i = 0; i < resource.Count; i++)
            {
                Task nop = new NoOperation(sim);
                tasks[i] = resource.Acquire(nop);
                nop.WaitOnTask(tasks[i]);
            }
            tasks[tasks.Length - 1] = new Process(sim, SetInService, resource);
            sim.Run(tasks);
            Assert.AreEqual(resource.Count, resource.InUse);
        }

        [Test]
        public void SetInServiceTracked()
        {
            IResource resource = Resource.Create(Names);
            Simulation sim = new Simulation();
            resource.OutOfService = resource.Count;
            Task[] tasks = new Task[resource.Count + 1];
            for (int i = 0; i < resource.Count; i++)
            {
                Task nop = new NoOperation(sim);
                tasks[i] = resource.Acquire(nop);
                nop.WaitOnTask(tasks[i]);
            }
            tasks[tasks.Length - 1] = new Process(sim, SetInService, resource);
            sim.Run(tasks);
            Assert.AreEqual(Names.Length, resource.InUse);
        }

        [Test]
        public void TransferAnonymous()
        {
            IResource resource = Resource.Create(2);
            Simulation sim = new Simulation();
            Task task = new Process(sim, TransferTask, resource);
            task.Activate(null);
            sim.Run();
            Assert.AreEqual(2, resource.Free);
        }

        [Test]
        public void TransferTracked()
        {
            IResource resource = Resource.Create(Names);
            Simulation sim = new Simulation();
            Task task = new Process(sim, TransferTask, resource);
            task.Activate(null);
            sim.Run();
            Assert.AreEqual(Names.Length, resource.Free);
        }

        [Test]
        public void TransferUsingInterrupt()
        {
            IResource resource = Resource.Create(1);
            Simulation sim = new Simulation();
            Task task1 = new Process(sim, TransferAndInterrupt, resource);
            task1.Name = "Task #1";
            Task task2 = new Process(sim, TransferAndInterrupt, resource);
            task2.Name = "Task #2";
            task1.Activate(null, 0L, task2);
            task2.Activate(null, 0L, task1);
            sim.Run();
            Assert.AreEqual(1, resource.Free);
        }

        //====================================================================
        //====                   Private Implementation                   ====
        //====================================================================

        private IEnumerator<Task> AcquireAll(Process p, object data)
        {
            Resource r = (Resource)data;
            int ownCount = 0;
            while (r.Free > 0)
            {
                int oldFree = r.Free;
                yield return r.Acquire(p);
                ownCount++;
                Assert.AreEqual(oldFree - 1, r.Free);
            }

            Assert.AreEqual(r.Count, ownCount);

            yield break;
        }

        private IEnumerator<Task> AcquireOneInterrupted(Process p, object data)
        {
            IResource r = (IResource)data;
            Task t = new InterruptTask(p.Simulation);
            p.WaitOnTask(t, t.Priority + 10);
            Assert.AreEqual(1, t.BlockCount);

            yield return r.Acquire(p);
            // The InterruptTask should interrupt us here.
            Assert.IsTrue(p.Interrupted);
            yield return p.Delay(3);
            // After a delay, the interrupt flag should be cleared.
            Assert.IsTrue(!p.Interrupted);
            yield break;
        }

        private IEnumerator<Task> SetInService(Process p, object data)
        {
            Resource r = (Resource)data;
            yield return p.Delay(10);
            Assert.AreEqual(r.Count, r.BlockCount);
            while (r.OutOfService > 0)
            {
                r.OutOfService--;
                yield return p.Delay(10);
            }
            Assert.AreEqual(0, r.BlockCount);
            yield break;
        }

        private IEnumerator<Task> TransferTask(Process p, object data)
        {
            Resource r = (Resource)data;
            yield return r.Acquire(p);
            Assert.IsTrue(r.IsOwner(p));
            Process receiver = new Process(p.Simulation, ReceiveTransfer);
            yield return r.Transfer(p, receiver);
            receiver.Activate(p, 10L, r);
            Assert.IsFalse(r.IsOwner(p));
            yield break;
        }

        private IEnumerator<Task> ReceiveTransfer(Process p, object data)
        {
            Resource r = (Resource)p.ActivationData;
            Assert.IsTrue(r.IsOwner(p));
            yield return r.Release(p);
            Assert.IsFalse(r.IsOwner(p));
            yield break;
        }

        private IEnumerator<Task> TransferAndInterrupt(Process p, object data)
        {
            Resource r = (Resource)data;
            Task task = (Task)p.ActivationData;
            yield return r.Acquire(p);
            Assert.IsTrue(r.IsOwner(p));
            if (p.Interrupted)
            {
                yield return p.Delay(1000L);
                yield return r.Release(p);
                Assert.AreEqual(r.Count, r.Free);
            }
            else
            {
                yield return p.Delay(1000L);
                yield return r.Transfer(p, task);
                task.Interrupt(p);
            }
            Assert.IsFalse(r.IsOwner(p));
            yield break;
        }
    }
}
