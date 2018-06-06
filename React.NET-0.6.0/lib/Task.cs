// Decompiled with JetBrains decompiler
// Type: React.Task
// Assembly: React.NET, Version=0.6.0.34129, Culture=neutral, PublicKeyToken=null
// MVID: 9585CC9B-8FA2-499B-8A36-9206AC3D9C94
// Assembly location: D:\CSharp\GraphX-PCL - 1506\Examples\ShowcaseApp.WPF\bin\Debug\React.NET.dll

using React.Queue;
using System;
using System.Collections.Generic;

namespace React
{
  /// <summary>
  /// An object that carries out some processing during the course of
  /// running a <see cref="P:React.Task.Simulation" />.
  /// <seealso cref="T:React.Process" />
  /// </summary>
  public abstract class Task : Blocking<Task>
  {
    /// <summary>The task priority.</summary>
    private int _priority = TaskPriority.Normal;
    /// <summary>The temporary elevated task priority.</summary>
    private int _elevated = TaskPriority.Normal;
    /// <summary>
    /// Schedule time returned when the <see cref="T:React.Task" /> is not
    /// scheduled to execute.
    /// </summary>
    /// <remarks>
    /// This value is identical to
    /// <see cref="F:React.ActivationEvent.NotScheduled" />.
    /// </remarks>
    public const long NotScheduled = -1;
    /// <summary>
    /// The simulation context under which the <see cref="T:React.Task" /> runs.
    /// </summary>
    private Simulation _sim;
    /// <summary>
    /// The wait queue used to block <see cref="T:React.Task" />s.
    /// </summary>
    private IQueue<Task> _waitQ;
    /// <summary>The Task instances upon which this task is blocked.</summary>
    private IList<Task> _blockedOn;
    /// <summary>Flag indicating the task has been canceled.</summary>
    private bool _cancelFlag;
    /// <summary>Flag indicating the task has been interrupted.</summary>
    private bool _intFlag;
    /// <summary>
    /// The <see cref="T:React.ActivationEvent" /> which invoked the task.
    /// </summary>
    private ActivationEvent _actevt;

    /// <summary>
    /// Gets the simulation context under which the <see cref="T:React.Task" /> is
    /// running.
    /// </summary>
    /// <value>
    /// The simulation context as a <see cref="P:React.Task.Simulation" />.
    /// </value>
    public Simulation Simulation
    {
      get
      {
        return this._sim;
      }
    }

    /// <summary>Gets the current simulation time.</summary>
    /// <remarks>
    /// This is really just a shortcut for <c>task.Simulation.Now</c>.
    /// </remarks>
    /// <value>
    /// The current simulation time as an <see cref="T:System.Int64" />.
    /// </value>
    public long Now
    {
      get
      {
        return this._sim.Now;
      }
    }

    /// <summary>
    /// Gets whether or not the <see cref="T:React.Task" /> has been scheduled to
    /// run.
    /// </summary>
    /// <remarks>
    /// A <see cref="T:React.Task" /> that has been activated using one of the
    /// <b>Activate</b> methods will be scheduled to run.  Therefore after
    /// calling <b>Activate</b>, <see cref="P:React.Task.IsScheduled" /> should always
    /// return <b>true</b>.
    /// </remarks>
    /// <value>
    /// <b>true</b> if the <see cref="T:React.Task" /> has been scheduled.
    /// </value>
    public bool IsScheduled
    {
      get
      {
        return this._actevt != null && this._actevt.IsPending;
      }
    }

    /// <summary>
    /// Gets the time the <see cref="T:React.Task" /> is scheduled to run.
    /// </summary>
    /// <remarks>
    /// If the <see cref="T:React.Task" /> is not scheduled to run, this
    /// property will be <see cref="F:React.Task.NotScheduled" />.
    /// </remarks>
    /// <value>
    /// The simulation time the <see cref="T:React.Task" /> will run as an
    /// <see cref="T:System.Int64" />.
    /// </value>
    public long ScheduledTime
    {
      get
      {
        if (this.IsScheduled)
          return this._actevt.Time;
        return -1;
      }
    }

    /// <summary>
    /// Gets whether or not the <see cref="T:React.Task" /> is blocked (that is,
    /// waiting on other <see cref="T:React.Task" />s)
    /// </summary>
    /// <remarks>
    /// <para>
    /// Immediately after a call to one of the <b>Activate</b> methods,
    /// this property will normally be <b>false</b> as <b>Activate</b>
    /// invokes <see cref="M:React.Task.ClearBlocks" />.  Subsequent calls to
    /// <see cref="M:React.Task.WaitOnTask(React.Task)" /> or <see cref="M:React.Task.WaitOnTask(React.Task,System.Int32)" />
    /// will cause this property to be <b>true</b>.
    /// </para>
    /// <para>
    /// Remember <see cref="P:React.Task.IsBlocked" /> is used to check if this
    /// <see cref="T:React.Task" /> is waiting on other <see cref="T:React.Task" />s
    /// <b>not</b> to check if this <see cref="T:React.Task" /> is blocking other
    /// <see cref="T:React.Task" />s (e.g. other <see cref="T:React.Task" />s are waiting on
    /// this <see cref="T:React.Task" />).
    /// </para>
    /// </remarks>
    /// <value>
    /// <b>true</b> if this <see cref="T:React.Task" /> is blocking on one or more
    /// <see cref="T:React.Task" />s.
    /// </value>
    public bool IsBlocked
    {
      get
      {
        return this._blockedOn != null && this._blockedOn.Count > 0;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:React.Queue.IQueue`1" /> that contains all the
    /// <see cref="T:React.Task" />s which are blocking on this <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// The wait queue is created on demand when this property is first
    /// accessed. The <see cref="M:React.Blocking`1.CreateBlockingQueue(System.Int32)" />
    /// method is used to create the wait queue.
    /// </remarks>
    /// <value>
    /// The <see cref="T:React.Queue.IQueue`1" /> that contains the
    /// <see cref="T:React.Task" />s blocking on this <see cref="T:React.Task" />.
    /// </value>
    protected IQueue<Task> WaitQueue
    {
      get
      {
        if (this._waitQ == null)
          this._waitQ = this.CreateBlockingQueue(0);
        return this._waitQ;
      }
    }

    /// <summary>Gets the current task priority.</summary>
    /// <remarks>
    /// <para>
    /// If the priority was elevated using <see cref="M:React.Task.ElevatePriority(System.Int32)" />,
    /// then <see cref="P:React.Task.Priority" /> will return the elevated task priority.
    /// The only way to get the task's default (non-elevated) priority is
    /// as follows.
    /// </para>
    /// <para>
    /// <code>
    /// // Get the current (possibly elevated) priority.
    /// int currpriority = task.Priority;
    /// // Restore the default priority which also returns the default priority.
    /// int defpriority = task.RestorePriority();
    /// // Return the priority to it's possibly elevated level.
    /// task.ElevatePriority(currpriority);</code></para>
    /// </remarks>
    /// <value>
    /// The current task priority as an <see cref="T:System.Int32" />.
    /// </value>
    public int Priority
    {
      get
      {
        return this._elevated;
      }
    }

    /// <summary>
    /// Gets whether or not the <see cref="T:React.Task" /> was canceled.
    /// </summary>
    /// <remarks>
    /// This property will be <b>true</b> after the <see cref="M:React.Task.Cancel" />
    /// method is invoked.
    /// </remarks>
    /// <value>
    /// <b>true</b> if the <see cref="T:React.Task" /> was canceled.
    /// </value>
    public bool Canceled
    {
      get
      {
        return this._cancelFlag;
      }
    }

    /// <summary>
    /// Gets whether or not the <see cref="T:React.Task" /> was interrupted.
    /// </summary>
    /// <remarks>
    /// This value is automatically reset to <b>false</b> after the
    /// <see cref="T:React.Task" /> executes.
    /// </remarks>
    /// <value>
    /// <b>true</b> if the <see cref="T:React.Task" /> was interrupted.
    /// </value>
    public bool Interrupted
    {
      get
      {
        return this._intFlag;
      }
    }

    /// <summary>
    /// Create a new <see cref="T:React.Task" /> instance that will run under under
    /// the given simulation context.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="sim" /> is <see langword="null" />.
    /// </exception>
    /// <param name="sim">The simulation context.</param>
    protected Task(Simulation sim)
    {
      if (sim == null)
        throw new ArgumentNullException("sim");
      this._sim = sim;
    }

    /// <summary>
    /// Get the number of <see cref="T:React.Task" /> instances blocked on the
    /// specified wait queue.
    /// </summary>
    /// <exception cref="T:System.ArgumentException">
    /// If <paramref name="queueId" /> is not a valid queue identifier.
    /// </exception>
    /// <param name="queueId">The queue identifier.</param>
    /// <returns>
    /// The number of <see cref="T:React.Task" /> instances blocked on the
    /// queue identified by <paramref name="queueId" />.
    /// </returns>
    public override int GetBlockCount(int queueId)
    {
      if (queueId == 0 || queueId == -1)
        return this._waitQ != null ? this._waitQ.Count : 0;
      throw new ArgumentException("Invalid queue id: " + (object) queueId);
    }

    /// <summary>
    /// Gets the <see cref="T:React.Task" /> instances blocking on the
    /// wait queue identified by a queue id.
    /// </summary>
    /// <exception cref="T:System.ArgumentException">
    /// If <paramref name="queueId" /> is not a valid queue identifier.
    /// </exception>
    /// <param name="queueId">The queue identifier.</param>
    /// <returns>
    /// An array of <see cref="T:React.Task" /> instances that are currently
    /// contained in the wait queue identified by
    /// <paramref name="queueId" />.  The returned array will never
    /// by <see langword="null" />.
    /// </returns>
    public override Task[] GetBlockedTasks(int queueId)
    {
      if (queueId == 0 || queueId == -1)
        return Blocking<Task>.GetBlockedTasks(this._waitQ);
      throw new ArgumentException("Invalid queue id: " + (object) queueId);
    }

    /// <summary>Clear the interrupt state.</summary>
    /// <remarks>
    /// The <see cref="T:React.Task" /> will automatically invoke this method after
    /// it's <see cref="M:React.Task.ExecuteTask(System.Object,System.Object)" /> method runs.
    /// </remarks>
    public void ClearInterrupt()
    {
      this._intFlag = false;
    }

    /// <summary>
    /// Temporarily elevate the <see cref="T:React.Task" />'s priority.
    /// </summary>
    /// <remarks>
    /// This method can both elevate (raise) the priority or reduce (lower)
    /// the priority.  If <paramref name="newPriority" /> is greater than
    /// <see cref="P:React.Task.Priority" />, the task prioritiy is raised; if
    /// <paramref name="newPriority" /> is lower than
    /// <see cref="P:React.Task.Priority" />, the task prioritiy is lowered.
    /// </remarks>
    /// <param name="newPriority">The new task priority.</param>
    public void ElevatePriority(int newPriority)
    {
      this._elevated = newPriority;
    }

    /// <summary>
    /// Restores the <see cref="T:React.Task" />'s priority to its non-elevated
    /// level.
    /// </summary>
    /// <returns>The non-elevated task priority.</returns>
    public int RestorePriority()
    {
      this._elevated = this._priority;
      return this._priority;
    }

    /// <summary>
    /// Wait the given <see cref="T:React.Task" /> while it executes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <paramref name="task" /> must not already be scheduled because this
    /// method will invoke its <see cref="M:React.Task.Activate(System.Object,System.Int64)" />
    /// method.
    /// </para>
    /// <para>
    /// The method is simply shorthand for
    /// <code>
    /// task.Activate(this, 0L);
    /// task.Block(this);</code>
    /// </para>
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="task" /> is <see langword="null" />.
    /// </exception>
    /// <param name="task">
    /// The <see cref="T:React.Task" /> to wait upon while it runs.
    /// </param>
    public void WaitOnTask(Task task)
    {
      if (task == null)
        throw new ArgumentNullException("task", "cannot be null");
      task.Activate((object) this, 0L);
      task.Block(this);
    }

    /// <summary>
    /// Wait the given <see cref="T:React.Task" /> while it executes at the
    /// specified priority.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <paramref name="task" /> must not already be scheduled because this
    /// method will invoke its <see cref="M:React.Task.Activate(System.Object,System.Int64,System.Int32)" />
    /// method.
    /// </para>
    /// <para>
    /// The method is simply shorthand for
    /// <code>
    /// task.Activate(this, 0L, priority);
    /// task.Block(this);</code>
    /// </para>
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="task" /> is <see langword="null" />.
    /// </exception>
    /// <param name="task">
    /// The <see cref="T:React.Task" /> to wait upon while it runs.
    /// </param>
    /// <param name="priority">
    /// The priority to activate <paramref name="task" />.
    /// </param>
    public void WaitOnTask(Task task, int priority)
    {
      if (task == null)
        throw new ArgumentNullException("task", "cannot be null");
      task.Activate((object) this, 0L, priority);
      task.Block(this);
    }

    /// <overloads>Activates (schedules) the Task to run.</overloads>
    /// <summary>
    /// Activates the <see cref="T:React.Task" /> at the current simulation time.
    /// </summary>
    /// <param name="activator">
    /// The object that is activating the <see cref="T:React.Task" />.  May be
    /// <see langword="null" />
    /// </param>
    public void Activate(object activator)
    {
      this.Activate(activator, 0L, (object) null, this.Priority);
    }

    /// <summary>
    /// Activates the <see cref="T:React.Task" /> at some time in the future.
    /// </summary>
    /// <param name="activator">
    /// The object that is activating the <see cref="T:React.Task" />.  May be
    /// <see langword="null" />
    /// </param>
    /// <param name="relTime">
    /// The time relative to the current time when the <see cref="T:React.Task" />
    /// should be scheduled to run.  If this value is zero (0), this
    /// method is the same as <see cref="M:React.Task.Activate(System.Object)" />.
    /// </param>
    public void Activate(object activator, long relTime)
    {
      this.Activate(activator, relTime, (object) null, this.Priority);
    }

    /// <summary>
    /// Activates the <see cref="T:React.Task" /> at some time in the future and
    /// with the given priority.
    /// </summary>
    /// <param name="activator">
    /// The object that is activating the <see cref="T:React.Task" />.  May be
    /// <see langword="null" />.
    /// </param>
    /// <param name="relTime">
    /// The time relative to the current time when the <see cref="T:React.Task" />
    /// should be scheduled to run.
    /// </param>
    /// <param name="priority">
    /// The task priority.  Higher values indicate higher priorities.
    /// </param>
    public void Activate(object activator, long relTime, int priority)
    {
      this.Activate(activator, relTime, (object) null, priority);
    }

    /// <summary>
    /// Activates the <see cref="T:React.Task" /> at some time in the future and
    /// with the given client-specific data.
    /// </summary>
    /// <param name="activator">
    /// The object that is activating the <see cref="T:React.Task" />.  May be
    /// <see langword="null" />
    /// </param>
    /// <param name="relTime">
    /// The time relative to the current time when the <see cref="T:React.Task" />
    /// should be scheduled to run.
    /// </param>
    /// <param name="data">
    /// An object containing client-specific data for the
    /// <see cref="T:React.Task" />.
    /// </param>
    public void Activate(object activator, long relTime, object data)
    {
      this.Activate(activator, relTime, data, this.Priority);
    }

    /// <summary>
    /// Activates the <see cref="T:React.Task" /> at some time in the future and
    /// specifying the task priority and client-specific task data.
    /// </summary>
    /// <remarks>
    /// <see cref="T:React.Task" /> implementations can normally treat this method
    /// as the "designated" version of the <b>Activate</b> method, which
    /// all other versions of <b>Activate</b> invoke.  That, in fact, is
    /// how the <see cref="T:React.Task" /> class implements <b>Activate</b>.
    /// </remarks>
    /// <exception cref="T:System.InvalidOperationException">
    /// If <see cref="P:React.Task.Interrupted" /> is <b>true</b>.  Before calling
    /// this method, ensure that the <see cref="T:React.Task" /> is no longer
    /// in an interrupted state.
    /// </exception>
    /// <param name="activator">
    /// The object that is activating the <see cref="T:React.Task" />.  May be
    /// <see langword="null" />.
    /// </param>
    /// <param name="relTime">
    /// The time relative to the current time when the <see cref="T:React.Task" />
    /// should be scheduled to run.
    /// </param>
    /// <param name="data">
    /// An object containing client-specific data for the
    /// <see cref="T:React.Task" />.
    /// </param>
    /// <param name="priority">
    /// The task priority.  Higher values indicate higher priorities.
    /// </param>
    public virtual void Activate(object activator, long relTime, object data, int priority)
    {
      if (this.Interrupted)
        throw new InvalidOperationException("Task is in an interrupted state.  Clear interrupt flag.");
      this.CancelPending(this._actevt);
      this.ClearBlocks();
      this.ElevatePriority(priority);
      ActivationEvent evt = new ActivationEvent(this, activator, relTime);
      evt.Data = data;
      this.Simulation.ScheduleEvent(evt);
      this._actevt = evt;
    }

    /// <summary>
    /// Resume the next waiting <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The next waiting <see cref="T:React.Task" /> is resumed with <c>this</c>
    /// as the activator and <see langword="null" /> for the activation
    /// data.
    /// </para>
    /// <para>
    /// Calling this method is identical to calling
    /// <code>ResumeNext(this, null);</code>
    /// </para>
    /// </remarks>
    protected void ResumeNext()
    {
      this.ResumeNext((object) this, (object) null);
    }

    /// <summary>
    /// Resume the next waiting <see cref="T:React.Task" /> with the specified
    /// activation data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The next waiting <see cref="T:React.Task" /> is resume with <c>this</c>
    /// as the activator.
    /// </para>
    /// <para>
    /// Calling this method is identical to calling
    /// <code>ResumeNext(this, data);</code>
    /// </para>
    /// </remarks>
    /// <param name="data">The activation data.</param>
    protected void ResumeNext(object data)
    {
      this.ResumeNext((object) this, data);
    }

    /// <summary>
    /// Resume the next waiting <see cref="T:React.Task" /> specifying the
    /// activator and activation data.
    /// </summary>
    /// <param name="activator">The activator.</param>
    /// <param name="data">The activation data.</param>
    protected virtual void ResumeNext(object activator, object data)
    {
      if (this.BlockCount <= 0)
        return;
      bool canceled;
      do
      {
        Task task = this.WaitQueue.Dequeue();
        canceled = task.Canceled;
        if (!canceled)
          this.ResumeTask(task, activator, data);
      }
      while (canceled && this.BlockCount > 0);
    }

    /// <summary>
    /// Resume all waiting <see cref="T:React.Task" />s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All waiting <see cref="T:React.Task" />s are resumed with <c>this</c> as the
    /// activator and <see langword="null" />as the activation data.
    /// </para>
    /// <para>
    /// Calling this method is identical to calling
    /// <code>ResumeAll(this, null);</code>
    /// </para>
    /// </remarks>
    protected void ResumeAll()
    {
      this.ResumeAll((object) this, (object) null);
    }

    /// <summary>
    /// Resume all waiting <see cref="T:React.Task" />s passing each the specified
    /// activation data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All waiting <see cref="T:React.Task" />s are resumed with <c>this</c> as the
    /// activator.
    /// </para>
    /// <para>
    /// Calling this method is identical to calling
    /// <code>ResumeAll(this, data);</code>
    /// </para>
    /// </remarks>
    /// <param name="data">The activation data.</param>
    protected void ResumeAll(object data)
    {
      this.ResumeAll((object) this, data);
    }

    /// <summary>
    /// Resume all waiting <see cref="T:React.Task" />s specifying the activator and
    /// the activation data.
    /// </summary>
    /// <param name="activator">The activator.</param>
    /// <param name="data">The activation data.</param>
    protected virtual void ResumeAll(object activator, object data)
    {
      int blockCount = this.BlockCount;
      if (blockCount > 1)
      {
        Task[] array = new Task[blockCount];
        this.WaitQueue.CopyTo(array, 0);
        this.WaitQueue.Clear();
        for (int index = 0; index < blockCount; ++index)
        {
          Task task = array[index];
          if (!task.Canceled)
            this.ResumeTask(task, activator, data);
        }
      }
      else
      {
        if (blockCount != 1)
          return;
        this.ResumeNext(activator, data);
      }
    }

    /// <summary>
    /// Resume the specified <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is invoked for each blocked <see cref="T:React.Task" /> that is
    /// to be resumed by calling one of the <b>ResumeNext</b> or
    /// <b>ResumeAll</b> methods.  Subclasses may override this method to
    /// alter the way <paramref name="task" /> is activated.
    /// </para>
    /// <para>
    /// The default implementation simply performs
    /// </para>
    /// <para><code>task.Activate(activator, 0L, data);</code></para>
    /// <para>
    /// Client code should normally never need to invoke this method
    /// directly.
    /// </para>
    /// <para>
    /// <b>By the time this method is called, <paramref name="task" /> has
    /// already been removed from <see cref="P:React.Task.WaitQueue" />.</b>
    /// </para>
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="task" /> is <see langword="null" />.
    /// </exception>
    /// <param name="task">
    /// The <see cref="T:React.Task" /> to resume (activate).
    /// </param>
    /// <param name="activator">
    /// The activator that will be passed to <paramref name="task" /> upon
    /// its activation.
    /// </param>
    /// <param name="data">
    /// Optional activation data passed to <paramref name="task" />.
    /// </param>
    protected virtual void ResumeTask(Task task, object activator, object data)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      task.Activate(activator, 0L, data);
    }

    /// <summary>
    /// Block the specified <see cref="T:React.Task" /> instance.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="task" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// If <paramref name="task" /> attempts to block itself.  For
    /// example, if code like <c>this.Block(this);</c> is executed.
    /// </exception>
    /// <param name="task">
    /// The <see cref="T:React.Task" /> to block.
    /// </param>
    public virtual void Block(Task task)
    {
      if (task == null)
        throw new ArgumentNullException("'task' cannot be null.");
      if (task == this)
        throw new ArgumentException("Task cannot block on itself.");
      task.UpdateBlockingLinks(this, true);
      this.WaitQueue.Enqueue(task);
    }

    /// <summary>
    /// Unblock, but do not resume, the specified <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is used to remove <paramref name="task" /> from the
    /// <see cref="T:React.Task" /> instance's wait list without resuming the
    /// execution of <paramref name="task" />.  The most common use for
    /// invoking <see cref="M:React.Task.Unblock(React.Task)" /> is to stop a <see cref="T:React.Task" />
    /// from waiting after it has been resumed by another means (e.g. a
    /// different simulation object has resumed <paramref name="task" />).
    /// </para>
    /// <para>
    /// Again, it's very important to realize that <see cref="M:React.Task.Unblock(React.Task)" />
    /// does <b>not</b> activate <paramref name="task" />.
    /// </para>
    /// <para>
    /// This method does nothing if <paramref name="task" /> equals
    /// <c>this</c> or is <see langword="null" />.
    /// </para>
    /// </remarks>
    /// <param name="task">
    /// The <see cref="T:React.Task" /> which will stop blocking on this
    /// <see cref="T:React.Task" /> instance.
    /// </param>
    public virtual void Unblock(Task task)
    {
      if (task == null || task == this || this.BlockCount <= 0)
        return;
      this.WaitQueue.Remove(task);
      task.UpdateBlockingLinks(this, false);
    }

    /// <summary>
    /// Stop blocking on all <see cref="T:React.Task" />s currently being blocked
    /// upon.
    /// </summary>
    protected virtual void ClearBlocks()
    {
      if (this._blockedOn == null)
        return;
      IList<Task> blockedOn = this._blockedOn;
      this._blockedOn = (IList<Task>) null;
      foreach (Task task in (IEnumerable<Task>) blockedOn)
        task.Unblock(this);
      if (this._blockedOn == null)
      {
        blockedOn.Clear();
        this._blockedOn = blockedOn;
      }
    }

    /// <summary>
    /// Update the association between this <see cref="T:React.Task" /> and the
    /// <see cref="T:React.Task" /> upon which it is blocking.
    /// </summary>
    /// <param name="blocker">
    /// The <see cref="T:React.Task" /> upon which this <see cref="T:React.Task" /> is
    /// blocking.
    /// </param>
    /// <param name="blocked">
    /// <b>true</b> if <paramref name="blocker" /> is blocking this
    /// <see cref="T:React.Task" />; or <b>false</b> if <paramref name="blocker" />
    /// is unblocking this <see cref="T:React.Task" />.
    /// </param>
    private void UpdateBlockingLinks(Task blocker, bool blocked)
    {
      if (blocked)
      {
        if (this._blockedOn == null)
          this._blockedOn = (IList<Task>) new List<Task>();
        if (this._blockedOn.Contains(blocker))
          throw new InvalidOperationException("Already blocking on specified task.");
        this._blockedOn.Add(blocker);
      }
      else
      {
        if (this._blockedOn == null)
          return;
        this._blockedOn.Remove(blocker);
      }
    }

    /// <summary>
    /// Cancel the <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A canceled task will not be executed.  The associated
    /// <see cref="T:React.ActivationEvent" /> (if any) is also canceled.
    /// </para>
    /// <para>
    /// Callers should note that once a <see cref="T:React.Task" /> is canceled
    /// it cannot be un-canceled, and therefore can never be
    /// re-activated.
    /// </para>
    /// </remarks>
    public void Cancel()
    {
      this._cancelFlag = true;
      this.CancelPending(this._actevt);
    }

    /// <summary>
    /// Interrupt a blocked <see cref="T:React.Task" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an blocked <see cref="T:React.Task" /> is interrupted, it should be
    /// activated at <see cref="P:React.Simulation.Now" />.  When the
    /// <see cref="T:React.Task" /> resumes running, it can check the
    /// <see cref="P:React.Task.Interrupted" /> property to determine how to proceed.
    /// The <paramref name="interruptor" /> is available to the
    /// <see cref="T:React.Task" /> as the <em>activator</em> parameter when
    /// <see cref="M:React.Task.ExecuteTask(System.Object,System.Object)" /> is invoked.
    /// </para>
    /// <para>
    /// The <see cref="T:React.Task" /> must handle the interrupt and clear the
    /// interrupt flag by calling <see cref="M:React.Task.ClearInterrupt" /> before
    /// <see cref="M:React.Task.Interrupt(System.Object)" /> or
    /// <see cref="M:React.Task.Activate(System.Object,System.Int64,System.Object,System.Int32)" /> (or any of the
    /// other <b>Activate</b> methods) may be called again.
    /// </para>
    /// </remarks>
    /// <exception cref="T:System.ArgumentException">
    /// If an <see cref="T:React.Task" /> attempts to interrupt itself.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="interruptor" /> is <see langword="null" />.
    /// </exception>
    /// <param name="interruptor">
    /// The object which caused the interrupt.  This should normally be
    /// the object that is invoking this method.
    /// </param>
    public void Interrupt(object interruptor)
    {
      if (interruptor == this)
        throw new ArgumentException("Task cannot interrupt itself.", "interruptor");
      if (interruptor == null)
        throw new ArgumentNullException("interruptor");
      this.Activate(interruptor, 0L);
      this._intFlag = true;
    }

    /// <summary>Perform the task actions.</summary>
    /// <remarks>
    /// This method is invoked by the <see cref="T:React.Task" />'s associated
    /// <see cref="T:React.ActivationEvent" /> when the
    /// <see cref="T:React.ActivationEvent" /> is fired.  Normally this method
    /// should not be called by client code.
    /// </remarks>
    /// <param name="activator">
    /// The object that activated this <see cref="T:React.Task" />.
    /// </param>
    /// <param name="data">
    /// Optional data for the <see cref="T:React.Task" />.
    /// </param>
    protected abstract void ExecuteTask(object activator, object data);

    /// <summary>
    /// Cancel the pending <see cref="T:React.ActivationEvent" />.
    /// </summary>
    /// <param name="evt">
    /// The <see cref="T:React.ActivationEvent" /> to cancel.
    /// </param>
    internal void CancelPending(ActivationEvent evt)
    {
      if (this._actevt == null)
        return;
      if (this._actevt != evt)
        throw new InvalidOperationException("Event mis-match.");
      this._actevt = (ActivationEvent) null;
      evt.Cancel();
    }

    /// <summary>
    /// Invoked by an <see cref="T:React.ActivationEvent" /> to execute the
    /// <see cref="T:React.Task" />.
    /// </summary>
    /// <param name="evt">
    /// The <see cref="T:React.ActivationEvent" /> that triggered this
    /// <see cref="T:React.Task" /> to execute.
    /// </param>
    internal void RunFromActivationEvent(ActivationEvent evt)
    {
      if (this._actevt != evt)
        throw new InvalidOperationException("Event mis-match.");
      this.RestorePriority();
      this._actevt = (ActivationEvent) null;
      this.ExecuteTask(evt.Activator, evt.Data);
      this.ClearInterrupt();
    }
  }
}
