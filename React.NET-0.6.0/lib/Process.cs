// Decompiled with JetBrains decompiler
// Type: React.Process
// Assembly: React.NET, Version=0.6.0.34129, Culture=neutral, PublicKeyToken=null
// MVID: 9585CC9B-8FA2-499B-8A36-9206AC3D9C94
// Assembly location: D:\CSharp\GraphX-PCL - 1506\Examples\ShowcaseApp.WPF\bin\Debug\React.NET.dll

using React.Tasking;
using System;
using System.Collections.Generic;

namespace React
{
  /// <summary>
  /// A <see cref="T:React.Task" /> implementation that uses an iterator method to
  /// support simulating complex or long-running processes.
  /// </summary>
  public class Process : Task
  {
    /// <summary>
    /// The delegate which can create the process step generator.
    /// </summary>
    private ProcessSteps _stepsfunc;
    /// <summary>
    /// Data passed to the <see cref="T:React.Process" /> when using a
    /// <see cref="T:React.ProcessSteps" /> delegate.
    /// </summary>
    private object _stepsdata;
    /// <summary>
    /// The active generator which yields each processing step.
    /// </summary>
    private IEnumerator<Task> _steps;
    /// <summary>
    /// The object that activated this <see cref="T:React.Process" />.
    /// </summary>
    private object _activator;
    /// <summary>Per-activation event data.</summary>
    private object _activationData;

    /// <summary>
    /// Gets the object that activated this <see cref="T:React.Process" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the <see cref="T:React.Process" /> is activated by an
    /// <see cref="T:React.IResource" />, <see cref="T:React.ICondition" />, or other
    /// blocking object, this property should always be non-null.
    /// </para>
    /// <para>
    /// The property is <see langword="null" /> except during the
    /// execution of the <see cref="M:React.Process.ExecuteTask(System.Object,System.Object)" /> method.
    /// </para>
    /// </remarks>
    /// <value>
    /// The <see cref="T:System.Object" /> that activated this <see cref="T:React.Process" />
    /// or <see langword="null" /> if self-activated or anonymously
    /// activated.
    /// </value>
    public object Activator
    {
      get
      {
        return this._activator;
      }
    }

    /// <summary>
    /// Gets the per-activation event data specified when this
    /// <see cref="T:React.Process" /> was activated.
    /// </summary>
    /// <remarks>
    /// The property is <see langword="null" /> except during the
    /// execution of the <see cref="M:React.Process.ExecuteTask(System.Object,System.Object)" /> method.
    /// </remarks>
    /// <value>
    /// The per-activation event data <see cref="T:System.Object" /> or
    /// <see langword="null" /> if the <see cref="P:React.Process.Activator" /> did not
    /// specify any activation data.
    /// </value>
    public object ActivationData
    {
      get
      {
        return this._activationData;
      }
    }

    /// <summary>
    /// Create a new <see cref="T:React.Process" /> instance.
    /// </summary>
    /// <remarks>
    /// This constructor is only available to derived classes.
    /// </remarks>
    /// <param name="sim">The simulation context.</param>
    protected Process(Simulation sim)
      : base(sim)
    {
    }

    /// <summary>
    /// Create a new <see cref="T:React.Process" /> that obtains its processing
    /// steps generator from the given delegate.
    /// </summary>
    /// <param name="sim">The simulation context.</param>
    /// <param name="steps">
    /// The <see cref="T:React.ProcessSteps" /> delegate that can create the
    /// generator which supplies the processing steps for the
    /// <see cref="T:React.Process" />.
    /// </param>
    public Process(Simulation sim, ProcessSteps steps)
      : this(sim, steps, (object) null)
    {
    }

    /// <summary>
    /// Create a new <see cref="T:React.Process" /> that obtains its processing
    /// steps generator from the given delegate and which can pass client
    /// data to the delegate.
    /// </summary>
    /// <param name="sim">The simulation context.</param>
    /// <param name="steps">
    /// The <see cref="T:React.ProcessSteps" /> delegate that can create the
    /// generator which supplies the processing steps for the
    /// <see cref="T:React.Process" />.
    /// </param>
    /// <param name="data">
    /// Client data passed to <paramref name="steps" /> when it is invoked.
    /// May be <see langword="null" />.
    /// </param>
    public Process(Simulation sim, ProcessSteps steps, object data)
      : base(sim)
    {
      if (steps == null)
        throw new ArgumentNullException("steps");
      this._stepsfunc = steps;
      this._stepsdata = data;
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.IEnumerator`1" /> that yields the processing
    /// steps.
    /// </summary>
    /// <remarks>
    /// This is a generator method that must be overridden by subclasses.  It
    /// must <c>yield</c> one or more <see cref="T:React.Task" /> instances which will
    /// perform each processing step.
    /// </remarks>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerator`1" /> (iterator) capable of
    /// yielding one or more <see cref="T:React.Task" /> instances that will
    /// perform actions on behalf of the <see cref="T:React.Process" />.
    /// </returns>
    protected virtual IEnumerator<Task> GetProcessSteps()
    {
      if (this._stepsfunc == null)
        throw new InvalidOperationException("No process steps defined.");
      return this._stepsfunc(this, this._stepsdata);
    }

    /// <summary>
    /// Executes each process step <see cref="T:React.Task" /> obtained from the
    /// generator created by <see cref="M:React.Process.GetProcessSteps" />.
    /// </summary>
    /// <remarks>
    /// Normally, <see cref="T:React.Process" /> implementors will not need to
    /// override this method; override <see cref="M:React.Process.GetProcessSteps" />
    /// instead.
    /// </remarks>
    /// <param name="activator">
    /// The object that activated this <see cref="T:React.Process" />.
    /// </param>
    /// <param name="data">Per-activation event data.</param>
    protected override void ExecuteTask(object activator, object data)
    {
      if (this._steps == null)
        this._steps = this.GetProcessSteps();
      this._activator = activator;
      this._activationData = data;
      if (this._steps.MoveNext())
      {
        Task current = this._steps.Current;
        if (current == null)
          throw new SimulationException("Process task step was null.");
        if (current != this)
        {
          this.ClearInterrupt();
          this.WaitOnTask(current);
        }
      }
      else
        this._steps = (IEnumerator<Task>) null;
      this._activationData = (object) null;
      this._activator = (object) null;
      if (this._steps != null)
        return;
      this.ResumeAll();
    }

    /// <summary>
    /// Defer processing to allow another <see cref="T:React.Task" /> to run.
    /// </summary>
    /// <remarks>
    /// This method is used to temporarily suspend the current
    /// <see cref="T:React.Process" /> and allow another <see cref="T:React.Task" /> to
    /// run.  It performs the same function as <c>Delay(0L)</c>.
    /// </remarks>
    /// <returns>
    /// A reference to the current <see cref="T:React.Process" />, <c>this</c>.
    /// </returns>
    [BlockingMethod]
    public Task Defer()
    {
      return this.Delay(0L);
    }

    /// <summary>Delay for a period of time.</summary>
    /// <exception cref="T:System.ArgumentException">
    /// If <paramref name="relTime" /> is less than zero (0).
    /// </exception>
    /// <param name="relTime">
    /// The delay time relative to the current simulation time.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="T:React.Process" />, <c>this</c>.
    /// </returns>
    [BlockingMethod]
    public Task Delay(long relTime)
    {
      if (relTime < 0L)
        throw new ArgumentException("'relTime' cannot be negative.");
      Task task;
      if (this.IsBlocked)
      {
        task = (Task) new Delay(this.Simulation, relTime);
      }
      else
      {
        this.ClearInterrupt();
        this.Activate((object) null, relTime);
        task = (Task) this;
      }
      return task;
    }

    /// <summary>
    /// Suspends or passivates the <see cref="T:React.Process" />.
    /// </summary>
    /// <remarks>
    /// Suspending a <see cref="T:React.Process" /> is different from defering a
    /// <see cref="T:React.Process" /> (see <see cref="M:React.Process.Defer" />).  When a
    /// <see cref="T:React.Process" /> is suspended, it requires another
    /// <see cref="T:React.Task" /> to re-activate it; a deferred
    /// <see cref="T:React.Process" /> will automatically re-activate.
    /// </remarks>
    /// <returns>
    /// A reference to the current <see cref="T:React.Process" />, <c>this</c>.
    /// </returns>
    [BlockingMethod]
    public Task Suspend()
    {
      return (Task) this;
    }
  }
}
