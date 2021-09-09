// <copyright file="TimedPlayer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LargoSharedClasses.Midi;
using LargoSharedClasses.MidiFile;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Timed Player.
    /// </summary>
    public sealed class TimedPlayer
    {
        #region Fields

        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly TimedPlayer InternalSingleton = new TimedPlayer();

        /// <summary>
        /// The last played bar number
        /// </summary>
        private int lastPlayedBarNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the TimedPlayer class from being created.
        /// </summary>
        private TimedPlayer()
        {
            this.lastPlayedBarNumber = -1;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [editor skip to bar].
        /// </summary>
        public event EventHandler<SkipToBarEventArgs> SkipToBar;

        #endregion

        #region Static properties

        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static TimedPlayer Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<TimedPlayer>() != null);
                if (InternalSingleton == null)
                {
                    throw new InvalidOperationException("Singleton Timed Player is null.");
                }

                return InternalSingleton;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether is Playing.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pause.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pause; otherwise, <c>false</c>.
        /// </value>
        public bool IsPause { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Plays the block.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public void PlayBlock(CompactMidiBlock givenBlock)
        {
            var section = new MusicalSection(1, 100, string.Empty);
            CompactMidiFeeder.Singleton.LoadBlock(givenBlock, section);
            MidiClock.Singleton.Tick += this.TimerTick; //// TimedPlayer.Singleton.TimerTick;
            MidiClock.Singleton.Start();
            this.IsPlaying = true;
            this.IsPause = false;
        }

        //// private string s;
        
        /// <summary>
        /// Timer tick.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        public void TimerTick(object sender, EventArgs e)
        {
            var block = CompactMidiFeeder.Singleton.MidiBlock;
            if (block == null || block.BarDuration == 0 || block.MidiBars.Count == 0) {
                return;
            }

            var deltaTime = (int)Math.Round(MidiClock.Singleton.Ticks * block.MidiTimeToTicksQuotient);

            //// There are events with deltaTime = order in some bars ..., now (deltaTime-1), was deltaTime
            //// var dt = (deltaTime == this.BarMidiDuration) ? deltaTime - 1 : deltaTime;
            var dt = deltaTime - 1;
            var barNumber = 1 + (int)Math.Floor((double)dt / block.BarDuration);

            ////  if (this.s == null) { this.s = "";  }
            ////  this.s = s + string.Format("{0}   bar {1}\n", dt, barNumber);
            ////  SupportFiles.StringToFile(s,@"d:\temp\log.txt");
            
            if (barNumber != this.lastPlayedBarNumber) {
                //// MessageBox.Show(barNumber.ToString());

                this.SendEditorSkipToBarEvent(barNumber);
                this.lastPlayedBarNumber = barNumber;
            }

            if (barNumber <= block.NumberOfBars) {
                while (true) {
                    var midiEvent = CompactMidiFeeder.Singleton.GetNextEvent(barNumber, deltaTime);
                    if (midiEvent == null) {
                        break;
                    }

                    DirectPlayer.PlayEvent(midiEvent);
                }
            }
            else {
                this.StopPlaying();
            }
        }

        /// <summary>
        /// Sends the editor skip to bar event.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        public void SendEditorSkipToBarEvent(int givenBarNumber)
        {
            var command = this.SkipToBar;
            command?.Invoke(this, new SkipToBarEventArgs(givenBarNumber));
        }

        #endregion

        #region Pause/Stop Playing

        /// <summary>
        /// Pause Playing.
        /// </summary>
        public void PausePlaying()
        {
            this.IsPause = true;
            MidiClock.Singleton.Stop();
            if (this.IsPlaying) {
                DirectPlayer.StopPlaying();
            }

            this.IsPlaying = false;
        }

        /// <summary>
        /// Continues the playing.
        /// </summary>
        public void ContinuePlaying()
        {
            this.IsPlaying = true;
            this.IsPause = false;
            MidiClock.Singleton.Continue();
            //// DirectPlayer.StopPlaying();
        }

        /// <summary>
        /// Stops the playing.
        /// </summary>
        public void StopPlaying()
        {
            this.IsPlaying = false;
            MidiClock.Singleton.Stop();
            DirectPlayer.StopPlaying();
        }

        /// <summary>
        /// Rewinds this instance.
        /// </summary>
        [UsedImplicitly]
        public void Rewind()
        {
            MidiClock.Singleton.Rewind();
        }

        /// <summary>
        /// Skips the forward.
        /// </summary>
        /// <param name="quotient">The quotient.</param>
        [UsedImplicitly]
        public void SkipBackward(int quotient)
        {
            MidiClock.Singleton.SkipBackward(quotient);
        }

        /// <summary>
        /// Skips the forward.
        /// </summary>
        /// <param name="quotient">The quotient.</param>
        [UsedImplicitly]
        public void SkipForward(int quotient)
        {
            MidiClock.Singleton.SkipForward(quotient);
        }

        #endregion
    }
}
