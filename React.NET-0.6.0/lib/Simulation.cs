// Decompiled with JetBrains decompiler
// Type: React.Simulation
// Assembly: React.NET, Version=0.6.0.34129, Culture=neutral, PublicKeyToken=null
// MVID: 9585CC9B-8FA2-499B-8A36-9206AC3D9C94
// Assembly location: D:\CSharp\GraphX-PCL - 1506\Examples\ShowcaseApp.WPF\bin\Debug\React.NET.dll

using React.Queue;
using System;

namespace React
{
  /// <summary>A class for running discrete-event simulations.</summary>
  /// <remarks>
  /// <para>
  /// <see cref="T:React.Simulation" /> contains mechanisms for maintaining the
  /// current simulated time as well as a collection of
  /// <see cref="T:React.ActivationEvent" /> instances scheduled to occur at some
  /// time during the simulation run.  This collection is typically
  /// referred to as the the <em>event queue</em>, <em>event calendar</em>
  /// or <em>future event set</em>.
  /// </para>
  /// <para>
  /// Fundamentally, the operation of the <see cref="T:React.Simulation" /> is
  /// quite simple.  It starts with the scheduling of one or more <em>
  /// generator</em> <see cref="T:React.Task" /> instances.  Internally, the
  /// generators are scheduled with <see cref="T:React.ActivationEvent" />s, but since
  /// it's <see cref="T:React.Task" />s that perform the actual simulated processing,
  /// the generators are <see cref="T:React.Task" />s rather than
  /// <see cref="T:React.ActivationEvent" />s.
  /// </para>
  /// <para>
  /// The generator <see cref="T:React.Task" />s serve to "jumpstart" or "bootstrap"
  /// the simulation.  They perform some set of initialization actions, which
  /// probably result in additional <see cref="T:React.Task" /> <em>activations</em>
  /// and thus additional <see cref="T:React.ActivationEvent" />s being scheduled.  As
  /// each <see cref="T:React.ActivationEvent" /> is fired it runs a
  /// <see cref="T:React.Task" />, and each <see cref="T:React.Task" /> might activate other
  /// <see cref="T:React.Task" />s (or itself).  Each activation places an
  /// <see cref="T:React.ActivationEvent" /> on the event queue.  This process
  /// continues until the event queue is emptied or the
  /// <see cref="T:React.Simulation" /> is ordered to stop.
  /// </para>
  /// </remarks>
  public class Simulation
  {
    /// <summary>The time the simulation will stop.</summary>
    private long _stopTime = long.MaxValue;
    /// <summary>The current <see cref="T:React.SimulationState" />.</summary>
    private SimulationState _state = SimulationState.Ready;
    /// <summary>The current simulation time.</summary>
    private long _currentTime;
    /// <summary>The number of discardable Tasks scheduled.</summary>
    private int _nDiscardableTasks;
    /// <summary>The event calendar (future event set).</summary>
    private IQueue<ActivationEvent> _eventQueue;

    /// <summary>Gets the current simulation time.</summary>
    /// <remarks>
    /// The current time will never be less than zero (0).  The simulation
    /// time at the beginning of a simulation run is zero.
    /// </remarks>
    /// <value>
    /// The current simulation time as an <see cref="T:System.Int64" />.
    /// </value>
    public long Now
    {
      get
      {
        return this._currentTime;
      }
    }

    /// <summary>
    /// Gets or sets the current <see cref="T:React.SimulationState" />.
    /// </summary>
    /// <remarks>
    /// Setting the current simulation state will raise the
    /// <see cref="E:React.Simulation.StateChanged" /> event.
    /// </remarks>
    public SimulationState State
    {
      get
      {
        return this._state;
      }
      protected set
      {
        if (this._state == value)
          return;
        this._state = value;
        this.OnStateChanged();
      }
    }

    /// <summary>Gets the simulation time when the simulation stopped.</summary>
    /// <remarks>
    /// The <see cref="T:React.Simulation" /> will stop its run loop when
    /// the current simulation time equals the stop time.  Any events
    /// scheduled at the stop time will be fired.
    /// </remarks>
    /// <value>
    /// The simulation stop time as an <see cref="T:System.Int64" />.
    /// </value>
    public long StopTime
    {
      get
      {
        return this._stopTime;
      }
    }

    /// <summary>
    /// Event raised when the <see cref="T:React.SimulationState" /> has changed.
    /// </summary>
    /// <remarks>
    /// The new <see cref="T:React.SimulationState" /> is not passed to the
    /// delegate method, rather each handler must query the sender (e.g.
    /// the <see cref="T:React.Simulation" />) to obtain its current state.
    /// </remarks>
    public event EventHandler StateChanged;

    /// <summary>
    /// Create and initialize a new <see cref="T:React.Simulation" />.
    /// </summary>
    public Simulation()
    {
      this._eventQueue = (IQueue<ActivationEvent>) new PriorityQueue<ActivationEvent>()
      {
        Prioritizer = (Comparison<ActivationEvent>) ((e1, e2) =>
        {
          if (e1.Time < e2.Time)
            return 1;
          if (e1.Time > e2.Time)
            return -1;
          if (e1.Priority > e2.Priority)
            return 1;
          return e1.Priority < e2.Priority ? -1 : 0;
        })
      };
    }

    /// <summary>
    /// Adds the specified <see cref="T:React.ActivationEvent" /> to the event queue
    /// (future event set).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Once <paramref name="evt" /> is scheduled, it cannot be removed from
    /// the event queue, but it may be canceled.  Canceled
    /// <see cref="T:React.ActivationEvent" />s remain in the queue, but are simply
    /// discarded rather than fired when they are removed from the queue.
    /// </para>
    /// <para>
    /// Most client code will not need to call this method, rather one of
    /// the <b>Activate</b> methods of the <see cref="T:React.Task" /> class should
    /// be used to schedule <see cref="T:React.Task" />s to run.
    /// </para>
    /// <para>
    /// <b>Important:</b> If the <see cref="T:React.Simulation" /> is in the
    /// <see cref="F:React.SimulationState.Stopping" /> state,
    /// <paramref name="evt" /> is silently ignored, <b>it is not
    /// scheduled</b>.
    /// </para>
    /// </remarks>
    /// <exception cref="T:React.BackClockingException">
    /// If <paramref name="evt" /> has an event time earlier than the
    /// current simulation time, <see cref="P:React.Simulation.Now" />.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// If the simulation <see cref="P:React.Simulation.State" /> is either
    /// <see cref="F:React.SimulationState.Completed" /> or
    /// <see cref="F:React.SimulationState.Failed" />.
    /// </exception>
    /// <param name="evt">
    /// The <see cref="T:React.ActivationEvent" /> to schedule (add to the event queue).
    /// </param>
    public virtual void ScheduleEvent(ActivationEvent evt)
    {
      if (evt.Time < this.Now)
        throw new BackClockingException(this, evt.Time);
      if (this.State == SimulationState.Completed || this.State == SimulationState.Failed)
        throw new InvalidOperationException("Simulation state is " + this.State.ToString());
      if (this.State == SimulationState.Stopping)
        return;
      this._eventQueue.Enqueue(evt);
      if (evt.Priority == TaskPriority.Discardable)
        ++this._nDiscardableTasks;
    }

    /// <overloads>Run the simulation.</overloads>
    /// <summary>
    /// Run the <see cref="T:React.Simulation" />.
    /// </summary>
    /// <remarks>
    /// When this version of <see cref="M:React.Simulation.Run" /> is used, one or more
    /// initial <see cref="T:React.Task" /> instance must have already been
    /// activated or the <see cref="T:React.Simulation" /> will immediately stop.
    /// </remarks>
    /// <returns>
    /// The number of <see cref="T:React.ActivationEvent" /> instances that remained
    /// in the event queue at the time the <see cref="T:React.Simulation" />
    /// stopped.
    /// </returns>
    public int Run()
    {
      return this.Run(new Task[0]);
    }

    /// <summary>
    /// Run the <see cref="T:React.Simulation" /> using the given generator
    /// <see cref="T:React.Task" /> instance.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="generator" /> is <see langword="null" />.
    /// </exception>
    /// <param name="generator">
    /// The <see cref="T:React.Task" /> which will serve as the sole generator.
    /// </param>
    /// <returns>
    /// The number of <see cref="T:React.ActivationEvent" /> instances that
    /// remained in the event queue at the time the
    /// <see cref="T:React.Simulation" /> stopped.
    /// </returns>
    public int Run(Task generator)
    {
      if (generator == null)
        throw new ArgumentNullException("generator");
      return this.Run(new Task[1]{ generator });
    }

    /// <summary>
    /// Run the <see cref="T:React.Simulation" /> using the provided generator
    /// <see cref="T:React.Task" /> instances.
    /// </summary>
    /// <remarks>
    /// The array of generators must contain zero (0) or more
    /// <see cref="T:React.Task" /> instances; it cannot be <see langword="null" />.
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="generators" /> is <see langword="null" />.
    /// </exception>
    /// <param name="generators">
    /// An array of generator <see cref="T:React.Task" /> instances.
    /// </param>
    /// <returns>
    /// The number of <see cref="T:React.ActivationEvent" /> instances that
    /// remained in the event queue at the time the
    /// <see cref="T:React.Simulation" /> stopped.
    /// </returns>
    public virtual int Run(Task[] generators)
    {
      this._currentTime = 0L;
      this._stopTime = long.MaxValue;
      this.ActivateGenerators(generators);
      this.State = SimulationState.Running;
      while (this._stopTime >= this._currentTime && this._eventQueue.Count > 0 && this._nDiscardableTasks < this._eventQueue.Count)
      {
        ActivationEvent activationEvent = this._eventQueue.Dequeue();
        this._currentTime = activationEvent.Time;
        if (activationEvent.Priority == TaskPriority.Discardable)
          --this._nDiscardableTasks;
        if (this._currentTime > this._stopTime)
          this._eventQueue.Enqueue(activationEvent);
        else if (activationEvent.IsPending)
          activationEvent.Fire(this);
      }
      if (this._currentTime < this._stopTime)
        this._stopTime = this._currentTime;
      else
        this._currentTime = this._stopTime;
      int count = this._eventQueue.Count;
      this._eventQueue.Clear();
      this.State = SimulationState.Completed;
      return count;
    }

    /// <summary>
    /// Stop the <see cref="T:React.Simulation" /> at the current simulation time.
    /// </summary>
    /// <remarks>
    /// This method is the equivalent of <c>sim.Stop(sim.Now)</c>.  If the
    /// <see cref="T:React.Simulation" /> is not running, invoking this method has
    /// no effect.
    /// </remarks>
    public void Stop()
    {
      this.Stop(this.Now);
    }

    /// <summary>
    /// Stop the <see cref="T:React.Simulation" /> at the specified simulation time.
    /// </summary>
    /// <remarks>
    /// If the <see cref="T:React.Simulation" /> is not running, this method has no
    /// effect.
    /// </remarks>
    /// <param name="absTime">
    /// The absolute simulation time when the <see cref="T:React.Simulation" />
    /// should stop running.  If <paramref name="absTime" /> is less than
    /// <see cref="P:React.Simulation.Now" />, the simulation will stop at the current time.
    /// </param>
    public virtual void Stop(long absTime)
    {
      this.State = SimulationState.Stopping;
      if (absTime < this.Now)
        this._stopTime = this.Now;
      else
        this._stopTime = absTime;
    }

    /// <summary>
    /// Invoked after the <see cref="T:React.SimulationState" /> has changed.
    /// </summary>
    /// <remarks>
    /// The default implementation raises the <see cref="E:React.Simulation.StateChanged" />
    /// event.
    /// </remarks>
    protected virtual void OnStateChanged()
    {
      if (this.StateChanged == null)
        return;
      this.StateChanged((object) this, EventArgs.Empty);
    }

    /// <summary>
    /// Activate all the generator <see cref="T:React.Task" /> instances in the
    /// given array.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// If <paramref name="generators" /> is <see langword="null" />.
    /// </exception>
    /// <param name="generators">
    /// An array containing zero or more <see cref="T:React.Task" />s that will
    /// serve as generators for the <see cref="T:React.Simulation" />.
    /// </param>
    private void ActivateGenerators(Task[] generators)
    {
      if (generators == null)
        throw new ArgumentNullException("'generators' cannot be null.");
      this.State = SimulationState.Initializing;
      foreach (Task generator in generators)
        generator.Activate((object) this);
    }
  }
}
