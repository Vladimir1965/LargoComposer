// <copyright file="SimpleTimer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Leslie Sanford</author>
// <email>jabberdabber@hotmail.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Free license.

namespace LargoSharedClasses.Abstract
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Music;

    /// <summary>
    /// Represents the Windows multimedia timer.
    /// </summary>
    public sealed class SimpleTimer : IComponent
    {
        #region Fields
        /// <summary>
        /// Timer identifier.
        /// </summary>
        private int timerId;

        /// <summary>
        /// Timer mode.
        /// </summary>
        private volatile TimerMode mode;

        /// <summary>
        /// Period between timer events in milliseconds.
        /// </summary>
        private volatile int period;

        /// <summary>
        /// Timer resolution in milliseconds.
        /// </summary>
        private volatile int resolution;

        /// <summary>
        /// Called by Windows when a timer periodic event occurs.
        /// </summary>
        private TimeProc timeProcPeriodic;

        /// <summary>
        /// Called by Windows when a timer one shot event occurs.
        /// </summary>
        private TimeProc timeProcOneShot;

        /// <summary>
        /// Indicates whether or not the timer has been disposed.
        /// </summary>
        private volatile bool disposed;

        #endregion

        #region Constructors
        //// <summary>
        //// Initializes static members of the SimpleTimer class.
        //// </summary>
        //// static SimpleTimer() { }

        /// <summary>
        /// Initializes a new instance of the SimpleTimer class.
        /// </summary>
        public SimpleTimer() {
            this.Initialize();
        }

        /// <summary>
        /// Finalizes an instance of the SimpleTimer class.
        /// </summary>
        ~SimpleTimer() {
            if (this.IsRunning) {
                // Stop and destroy timer.
                NativeMethods.TimeKillEvent(this.timerId);
            }
        }
        #endregion

        #region Delegates

        /// <summary>
        /// Represents the method that is called by Windows when a timer event occurs.
        /// </summary>
        /// <param name="id">Timer Procedure Id.</param>
        /// <param name="msg">Timer Message.</param>
        /// <param name="user">Timer User.</param>
        /// <param name="param1">Parameter 1.</param>
        /// <param name="param2">Parameter 2.</param>
        private delegate void TimeProc(int id, int msg, int user, int param1, int param2);

        #endregion

        #region Events
        /// <summary>
        /// Occurs when the Timer has started.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler Started;

        /// <summary>
        /// Occurs when the Timer has stopped.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when the time period has elapsed.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// EventHandler Disposed. IComponent Members.
        /// </summary>
        public event EventHandler Disposed;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Site.
        /// </summary>
        /// <value> General property.</value>
        public ISite Site { get; set; }

        /// <summary>
        /// Gets or sets the time between Tick events -  in milliseconds.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>   
        /// <value> General property.</value>
        [System.Diagnostics.Contracts.Pure]
        public int Period {
            get {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                return this.period;
            }

            set {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                this.period = value;

                if (!this.IsRunning) {
                    return;
                }

                this.Stop();
                this.Start();
            }
        }

        /// <summary>
        /// Gets or sets the timer resolution.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>        
        /// <remarks>
        /// The resolution is in milliseconds. The resolution increases 
        /// with smaller values; a resolution of 0 indicates periodic events 
        /// should occur with the greatest possible accuracy. To reduce system 
        /// overhead, however, you should use the maximum value appropriate 
        /// for your application.
        /// </remarks>
        /// <value> General property.</value>
        public int Resolution {
            private get {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                return this.resolution;
            }

            set {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, Localization.LocalizedMusic.String("Multimedia timer resolution out of range."));
                }

                this.resolution = value;

                if (!this.IsRunning) {
                    return;
                }

                this.Stop();
                this.Start();
            }
        }

        /// <summary>
        /// Gets or sets the timer mode.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        /// <value> General property.</value>
        public TimerMode Mode {
            private get {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                return this.mode;
            }

            set {
                if (this.disposed) {
                    throw new ObjectDisposedException("Timer");
                }

                this.mode = value;

                if (!this.IsRunning) {
                    return;
                }

                this.Stop();
                this.Start();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Timer is running.
        /// </summary>
        /// <value> General property.</value>
        private bool IsRunning { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The timer has already been disposed.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// The timer failed to start.
        /// </exception>
        public void Start() {
            if (this.disposed) {
                throw new ObjectDisposedException("Timer");
            }

            if (this.IsRunning) {
                return;
            }

            //// If the periodic event callback should be used.
            var timerProc = (this.Mode == TimerMode.Periodic) ? this.timeProcPeriodic : this.timeProcOneShot;
            //// Create and start timer, period in milliseconds
            this.timerId = NativeMethods.TimeSetEvent(this.Period, this.Resolution, timerProc, 0, (int)this.Mode);

            //// If the timer was created successfully.
            if (this.timerId == 0) {
                throw new InvalidOperationException("Unable to start multimedia Timer.");
            }

            this.IsRunning = true;

            this.OnStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public void Stop() {
            if (this.disposed) {
                throw new ObjectDisposedException("Timer");
            }

            if (!this.IsRunning) {
                return;
            }

            // Stop and destroy timer.
            //// int result = 
            NativeMethods.TimeKillEvent(this.timerId);
            this.IsRunning = false;

            this.OnStopped(EventArgs.Empty);
        }

        /// <summary>
        /// Frees timer resources. IDisposable Members.
        /// </summary>
        public void Dispose() {
            if (this.disposed) {
                return;
            }

            if (this.IsRunning) {
                this.Stop();
            }

            this.disposed = true;

            this.OnDisposed(EventArgs.Empty);
            //// Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Timer Periodic Event Callback - Callback method called by the Win32 multimedia timer when a timer
        /// periodic event occurs.
        /// </summary>
        /// <param name="id">Timer Identifier.</param>
        /// <param name="msg">Timer Message.</param>
        /// <param name="user">Timer User.</param>
        /// <param name="param1">Parameter 1.</param>
        /// <param name="param2">Parameter 2.</param>
        private void TimerPeriodicEventCallback(int id, int msg, int user, int param1, int param2) {
            this.OnTick(EventArgs.Empty);
        }

        /// <summary>
        /// Callback method called by the Win32 multimedia timer when a timer one shot event occurs.
        /// </summary>
        /// <param name="id">Timer Identifier.</param>
        /// <param name="msg">Timer Message.</param>
        /// <param name="user">Timer User.</param>
        /// <param name="param1">Parameter 1.</param>
        /// <param name="param2">Parameter 2.</param>
        private void TimerOneShotEventCallback(int id, int msg, int user, int param1, int param2) {
            this.OnTick(EventArgs.Empty);
            this.Stop();
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initialize timer with default values.
        /// </summary>
        private void Initialize() {
            this.mode = TimerMode.Periodic;
            this.period = 60; //// Capabilities.PeriodMin;
            this.resolution = 1;

            this.IsRunning = false;

            this.timeProcPeriodic = this.TimerPeriodicEventCallback;
            this.timeProcOneShot = this.TimerOneShotEventCallback;
            //// this.tickRaiser = this.OnTick;
        }

        #endregion

        #region Event Raiser Methods

        /// <summary>
        /// On Disposed. Raises the Disposed event.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnDisposed(EventArgs e) {
            var handler = this.Disposed;

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// On Started. Raises the Started event.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnStarted(EventArgs e) {
            var handler = this.Started;

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// On Stopped. Raises the Stopped event.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnStopped(EventArgs e) {
            var handler = this.Stopped;

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// On Tick. Raises the Tick event.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnTick(EventArgs e) {
            var handler = this.Tick;

            handler?.Invoke(this, e);
        }
        #endregion

        #region Native class (Win32 Multimedia Timer Functions)
        /// <summary>
        /// Native Methods.
        /// </summary>
        private static class NativeMethods
        {
            //// <summary>
            //// Prevents a default instance of the NativeMethods class from being created.
            //// </summary>
            //// private NativeMethods() {}

            /// <summary>
            /// Times the set event. - Creates and starts the timer.
            /// </summary>
            /// <param name="delay">The event delay, in milliseconds. .</param>
            /// <param name="resolution">The resolution.</param>
            /// <param name="proc">The timer procedure.</param>
            /// <param name="user">The timer user.</param>
            /// <param name="mode">The timer mode.</param>
            /// <returns> Returns value. </returns> 
            [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
            public static extern int TimeSetEvent(int delay, int resolution, TimeProc proc, int user, int mode);

            /// <summary>
            /// Stops and destroys the timer.
            /// </summary>
            /// <param name="id">Given timer id.</param>
            /// <returns> Returns value. </returns> 
            [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
            public static extern int TimeKillEvent(int id);
        }
        #endregion
    }
}
