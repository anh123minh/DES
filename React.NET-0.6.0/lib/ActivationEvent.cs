// Decompiled with JetBrains decompiler
// Type: React.ActivationEvent
// Assembly: React.NET, Version=0.6.0.34129, Culture=neutral, PublicKeyToken=null
// MVID: 9585CC9B-8FA2-499B-8A36-9206AC3D9C94
// Assembly location: D:\CSharp\GraphX-PCL - 1506\Examples\ShowcaseApp.WPF\bin\Debug\React.NET.dll

using System;

namespace React
{
  /// <summary>
  /// Object used by a <see cref="T:React.Simulation" /> to schedule and run
  /// <see cref="P:React.ActivationEvent.Task" /> instances.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="T:React.ActivationEvent" /> is part of the React.NET tasking system.  Each
  /// <see cref="T:React.ActivationEvent" /> specifies <em>when</em> a particular
  /// <see cref="P:React.ActivationEvent.Task" /> should be executed.  Each <see cref="P:React.ActivationEvent.Task" />, on
  /// the other hand, describes <em>what</em> happens.
  /// </para>
  /// <para>
  /// Normally <see cref="P:React.ActivationEvent.Task" /> and <see cref="T:React.Process" /> implementors will
  /// not need to work directly with <see cref="T:React.ActivationEvent" />s. Instead,
  /// they can simply use the various <b>Activate</b> methods of the
  /// <see cref="P:React.ActivationEvent.Task" /> class, which will create and schedule an
  /// <see cref="T:React.ActivationEvent" /> on behalf of the <see cref="P:React.ActivationEvent.Task" />.
  /// </para>
  /// </remarks>
  public class ActivationEvent
  {
    /// <summary>The time the event is scheduled to occur.</summary>
    private long _evtTime = -1;
    /// <summary>
    /// The priority of <see cref="F:React.ActivationEvent._task" /> at the time this event was created.
    /// </summary>
    private int _priority = TaskPriority.Normal;
    /// <summary>Time which indicates the event is not scheduled.</summary>
    public const long NotScheduled = -1;
    /// <summary>Client-specific event data.</summary>
    private object _evtData;
    /// <summary>Flag indicating whether or not the event is canceled.</summary>
    private bool _cancelFlag;
    /// <summary>The <see cref="P:React.ActivationEvent.Task" /> to execute.</summary>
    private Task _task;
    /// <summary>The object that activated <see cref="F:React.ActivationEvent._task" />.</summary>
    private object _activator;

    /// <summary>
    /// Gets the <see cref="P:React.ActivationEvent.Task" /> this event will execute.
    /// </summary>
    /// <value>
    /// The <see cref="P:React.ActivationEvent.Task" /> to execute.
    /// </value>
    public Task Task
    {
      get
      {
        return this._task;
      }
    }

    /// <summary>
    /// Gets the object that is activating the <see cref="P:React.ActivationEvent.Task" />
    /// associated with this <see cref="T:React.ActivationEvent" />.
    /// </summary>
    /// <value>
    /// The task activator as an <see cref="T:System.Object" /> or
    /// <see langword="null" /> if the activation is anonymous (i.e. it
    /// did not specify an activator).
    /// </value>
    public object Activator
    {
      get
      {
        return this._activator;
      }
    }

    /// <summary>Gets the simulation time the event should occur.</summary>
    /// <remarks>
    /// <para>
    /// Note that the event will not actually occur unless it is first
    /// scheduled with the simulation using the
    /// <see cref="M:React.Simulation.ScheduleEvent(React.ActivationEvent)" /> method.
    /// </para>
    /// <para>
    /// <see cref="T:React.ActivationEvent" />s with lower <see cref="P:React.ActivationEvent.Time" /> values
    /// always get fired before those with greater <see cref="P:React.ActivationEvent.Time" />s.
    /// For <see cref="T:React.ActivationEvent" /> occurring at the same
    /// <see cref="P:React.ActivationEvent.Time" />, their <see cref="P:React.ActivationEvent.Priority" /> values are used to
    /// determine which <see cref="T:React.ActivationEvent" /> should be fired
    /// first.
    /// </para>
    /// </remarks>
    /// <value>
    /// The simulation time the event should occur as an
    /// <see cref="T:System.Int64" />.
    /// </value>
    public long Time
    {
      get
      {
        return this._evtTime;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:React.ActivationEvent" />'s priority.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For <see cref="T:React.ActivationEvent" />s scheduled to occur at the same
    /// <see cref="P:React.ActivationEvent.Time" />, the <see cref="P:React.ActivationEvent.Priority" /> can be used as a
    /// tie-breaker with those <see cref="T:React.ActivationEvent" />s having higher
    /// priorities getting fired before those of lower priorities.
    /// </para>
    /// <para>
    /// <see cref="P:React.ActivationEvent.Priority" /> is not used to compare
    /// <see cref="T:React.ActivationEvent" />s occurring at different times.  In
    /// those cases, the earlier <see cref="T:React.ActivationEvent" /> always takes
    /// place before the later <see cref="T:React.ActivationEvent" />.
    /// </para>
    /// </remarks>
    /// <value>
    /// The priority as an <see cref="T:System.Int32" />.
    /// </value>
    public int Priority
    {
      get
      {
        return this._priority;
      }
    }

    /// <summary>
    /// Gets whether the <see cref="T:React.ActivationEvent" /> is pending.
    /// </summary>
    /// <remarks>
    /// This property will be <b>true</b> if the event is currently
    /// pending (waiting to be fired).  It is false if:
    /// <list type="bullet">
    ///     <item><description>
    ///         The <see cref="T:React.ActivationEvent" /> was never scheduled with
    ///         a <see cref="T:React.Simulation" />; or
    ///     </description></item>
    ///     <item><description>
    ///         It was cancelled via the <see cref="M:React.ActivationEvent.Cancel" /> method; or
    ///     </description></item>
    ///     <item><description>
    ///         Its associated <see cref="P:React.ActivationEvent.Task" /> was cancelled; or
    ///     </description></item>
    ///     <item><description>
    ///         It has already been fired.
    ///     </description></item>
    /// </list>
    /// </remarks>
    /// <value>
    /// <b>true</b> if the <see cref="T:React.ActivationEvent" /> is pending.
    /// </value>
    public bool IsPending
    {
      get
      {
        return this._evtTime >= 0L && !this._cancelFlag && !this.Task.Canceled;
      }
    }

    /// <summary>
    /// Gets any optional data associated with this
    /// <see cref="T:React.ActivationEvent" />.
    /// </summary>
    /// <value>
    /// The event data or <see langword="null" /> if there is no optional
    /// data.
    /// </value>
    public virtual object Data
    {
      get
      {
        return this._evtData;
      }
      set
      {
        this._evtData = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:React.ActivationEvent" /> that will run the specified
    /// <see cref="P:React.ActivationEvent.Task" />.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="task" /> is <see langword="null" />.
    /// </exception>
    /// <param name="task">
    /// The <see cref="P:React.ActivationEvent.Task" /> this <see cref="T:React.ActivationEvent" /> will run
    /// when it is fired.
    /// </param>
    /// <param name="relTime">
    /// The time relative to the current simulation time when the
    /// <see cref="T:React.ActivationEvent" /> should be fired.
    /// </param>
    public ActivationEvent(Task task, long relTime)
      : this(task, (object) null, relTime)
    {
    }

    /// <summary>
    /// Create a new <see cref="T:React.ActivationEvent" /> that will run the
    /// specified <see cref="P:React.ActivationEvent.Task" /> on behalf of the given activator.
    /// </summary>
    /// <param name="task">
    /// The <see cref="P:React.ActivationEvent.Task" /> this <see cref="T:React.ActivationEvent" /> will run
    /// when it is fired.
    /// </param>
    /// <param name="activator">
    /// The object which is activating <paramref name="task" />.
    /// </param>
    /// <param name="relTime">
    /// The relative time when <paramref name="task" /> should be executed.
    /// </param>
    public ActivationEvent(Task task, object activator, long relTime)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (task.Canceled)
        throw new ArgumentException("Cannot be canceled.", "task");
      this._evtTime = task.Simulation.Now + relTime;
      this._task = task;
      this._activator = activator;
      this._priority = task.Priority;
    }

    /// <summary>
    /// Cancels the <see cref="T:React.ActivationEvent" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// After cancelling the <see cref="T:React.ActivationEvent" />,
    /// <see cref="P:React.ActivationEvent.IsPending" /> will be <b>false</b> and the event will
    /// not be executed (fired) by the <see cref="T:React.Simulation" />.
    /// </para>
    /// <para>
    /// Note that cancelling an <see cref="T:React.ActivationEvent" /> does not cancel
    /// its associated <see cref="P:React.ActivationEvent.Task" />.
    /// </para>
    /// </remarks>
    public void Cancel()
    {
      this._cancelFlag = true;
      this.Task.CancelPending(this);
      this.PrepareDeferredData();
    }

    /// <summary>
    /// Invoked by the <see cref="T:React.Simulation" /> to fire the
    /// <see cref="T:React.ActivationEvent" />.
    /// </summary>
    /// <remarks>
    /// Only a <see cref="T:React.Simulation" /> should invoke this method and then
    /// only after scheduling the <see cref="T:React.ActivationEvent" />.
    /// </remarks>
    /// <param name="sim">
    /// The <see cref="T:React.Simulation" /> firing the
    /// <see cref="T:React.ActivationEvent" />.
    /// </param>
    internal void Fire(Simulation sim)
    {
      if (!this.IsPending)
        throw new InvalidOperationException("Event was not pending.");
      this.PrepareDeferredData();
      this._evtTime = -1L;
      this.Task.RunFromActivationEvent(this);
    }

    /// <summary>Obtains the deferred activation data if any.</summary>
    /// <remarks>
    /// After this method is invoked, the <see cref="P:React.ActivationEvent.Data" /> property
    /// will reflect the data obtained from the
    /// <see cref="T:React.DeferredDataCallback" /> delegate.
    /// </remarks>
    private void PrepareDeferredData()
    {
      DeferredDataCallback data = this.Data as DeferredDataCallback;
      if (data == null)
        return;
      this.Data = data(this);
    }
  }
}
