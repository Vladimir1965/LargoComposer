// <copyright file="MidiClock.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Leslie Sanford</author>
// <email>jabberdabber@hotmail.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Free license.

using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// Provides basic functionality for generating tick events with pulses per period.
    /// (originally used for quarter musical note resolution, here for bars...)
    /// Generates clock events internally.
    /// </summary>
    public sealed class MidiClock : IDisposable {  //// IClock,
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly MidiClock InternalSingleton = new MidiClock(1);

        /// <summary>
        /// Used for generating tick events.
        /// </summary>
        private readonly SimpleTimer timer; //// = new SimpleTimer();

        /// <summary>
        /// Is disposed.
        /// </summary>
        private bool disposed;
        #endregion
    
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiClock"/> class.
        /// </summary>
        [UsedImplicitly]
        public MidiClock() {
        }

        /// <summary>
        /// Initializes a new instance of the MidiClock class.
        /// </summary>
        /// <param name="givenPeriod">Timer Period - period in milliseconds.</param>
        public MidiClock(int givenPeriod) {
            if (givenPeriod < 1) {
                throw new ArgumentOutOfRangeException(nameof(givenPeriod), givenPeriod, "Timer period cannot be less than one.");
            }

            /* Description of Midi Clock
            Notes are measured in pulses rather than in time. 
            The note start time and duration will be a discrete number of pulses. 
            Pulses per quarter note can be found in the header part of the MIDI file and specifies how many midi pulses make up one quarter note (=1/4). 
            This is useful for determining note values - if the current pulse is 96 and a note's duration is 48 we know that it's a quaver (=1/8). 
            It's also vital for determining the rate at which the MIDI file should be played. 
            The tempo tells us how long each pulse should last. 
            The BPM measures how many quarter notes happen in a minute. 
            To work out the length of each pulse we can use the following formula: Pulse Length = 60/(BPM * pulse)
    
            MidiTempoBaseNumber = 60000000;
            MidiMaxTempoValue = 0xFFFFFF
            int Tempo [BPM] = MidiTempoBaseNumber / value [0 to MidiMaxTempoValue]  //// The default tempo in microseconds: 120bpm, value = 500000
            The minimum pulses per quarter note value.
            PulsesMinValue = 24;
            Pulse Length = 60/(Tempo[BPM] * PulsesMinValue[pulses])
            MicrosecondsPerMillisecond = 1000; //// The number of microseconds per millisecond.
            */

            this.timer = new SimpleTimer { Mode = TimerMode.Periodic, Period = givenPeriod, Resolution = 1 }; //// Period = 1700
            this.timer.Tick += this.TimerTick;
            //// 2019/02 this.timer.Start();
        }

        #endregion
        
        #region Events
        /// <summary>
        /// EventHandler Tick.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// EventHandler Started.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler Started;

        /// <summary>
        /// EventHandler Continued.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler Continued;

        /// <summary>
        /// EventHandler Stopped.
        /// </summary>
        [UsedImplicitly]
        public event EventHandler Stopped;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MidiClock Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static MidiClock Singleton {
            get {
                Contract.Ensures(Contract.Result<MidiClock>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Midi Clock is null.");
                }

                return InternalSingleton; 
            }
        }

        /// <summary>
        /// Gets Tick accumulator.
        /// </summary>
        /// <value> Property description. </value>
        public int Ticks { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Is Running. IClock Members.
        /// </summary>
        /// <value> General property.</value>
        public bool IsRunning { get; private set; }

        #endregion

        #region Public methods
        /* MIDIClock - tempo !?
        /// <summary>
        /// Sets the tempo.
        /// </summary>
        /// <param name="givenTempo">The given tempo.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Tempo out of range.</exception>
        public void SetTempo(int givenTempo) {
            //// Require
            if (tempo < 1) {
                throw new ArgumentOutOfRangeException(
                    "Tempo out of range.");
            }

            this.tempo = givenTempo;
        } */

        /// <summary>
        /// Starts the MidiInternalClock.
        /// </summary>
        public void Start() {
            if (this.IsRunning) {
                return;
            }

            this.Ticks = 0;
            this.OnStarted(EventArgs.Empty);

            // Start the multimedia timer in order to start generating ticks.
            this.timer.Start();

            // Indicate that the clock is now running.
            this.IsRunning = true;
        }

        /// <summary>
        /// Resumes tick generation from the current position.
        /// </summary>
        [UsedImplicitly]
        public void Continue() {
            if (this.IsRunning) {
                return;
            }

            //// Raise Continued event.
            this.OnContinued(EventArgs.Empty);
            //// Start multimedia timer in order to start generating ticks.
            this.timer.Start();
            //// Indicate that the clock is now running.
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops the MidiInternalClock.
        /// </summary>
        public void Stop() {
            if (!this.IsRunning) {
                return;
            }

            //// Stop the multimedia timer.
            this.timer.Stop();
            //// Indicate that the clock is not running.
            this.IsRunning = false;
            this.OnStopped(EventArgs.Empty);
        }

        /// <summary>
        /// Rewinds this instance.
        /// </summary>
        public void Rewind() {
            this.Ticks = this.timer.Period;
            this.OnTick(EventArgs.Empty);
        }

        /// <summary>
        /// Skips the backward.
        /// </summary>
        /// <param name="quotient">The quotient.</param>
        public void SkipBackward(int quotient) {
            this.Ticks -= quotient * this.timer.Period;
            this.Ticks = Math.Max(this.Ticks, this.timer.Period);
            this.OnTick(EventArgs.Empty);
        }

        /// <summary>
        /// Skips the forward.
        /// </summary>
        /// <param name="quotient">The quotient.</param>
        public void SkipForward(int quotient) {
            this.Ticks += quotient * this.timer.Period;
            this.OnTick(EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            //// Unregister object for finalization.
            //// GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method On Continued.
        /// </summary>
        /// <param name="e">Event Args.</param>
        [UsedImplicitly]
        private void OnContinued(EventArgs e) {
            var handler = this.Continued;

            handler?.Invoke(this, e);
        }

        #region IDisposable Implementation

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">Disposing <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing) {
            lock (this) {
                //// Do nothing if the object has already been disposed of.
                if (this.disposed) {
                    return;
                }

                if (disposing) {
                    //// Release disposable objects used by this instance here.
                    this.timer?.Dispose();

                    InternalSingleton?.Dispose();
                }

                // Release unmanaged resources here. Don't access reference type fields.

                // Remember that the object has been disposed of.
                this.disposed = true;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Method On Tick.
        /// </summary>
        /// <param name="e">Event Args.</param>
        [UsedImplicitly]
        private void OnTick(EventArgs e) {
            var handler = this.Tick;
            //// handler.GetInvocationList().Length;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method On Started.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnStarted(EventArgs e) {
            var handler = this.Started;

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Method On Stopped.
        /// </summary>
        /// <param name="e">Event Args.</param>
        private void OnStopped(EventArgs e) {
            var handler = this.Stopped;

            handler?.Invoke(this, e);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Timer Tick.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event Arguments.</param>
        private void TimerTick(object sender, EventArgs e) { //// virtual
            Contract.Requires(this.timer != null);
            this.Ticks += this.timer.Period;
            this.OnTick(e);
        }
        #endregion
    }
}
