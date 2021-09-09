// <copyright file="CompactMidiFeeder.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.MidiFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using LargoSharedClasses.Midi;
    using Music;

    /// <summary>
    /// Music Midi Feeder.
    /// </summary>
    public sealed class CompactMidiFeeder
    {
        #region Fields

        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly CompactMidiFeeder InternalSingleton = new CompactMidiFeeder();

        #endregion

        #region Constructors
        #endregion

        #region Static properties

        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static CompactMidiFeeder Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<CompactMidiFeeder>() != null);
                if (InternalSingleton == null)
                {
                    throw new InvalidOperationException("Singleton CompactMidiFeeder is null.");
                }

                return InternalSingleton;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the midi block.
        /// </summary>
        /// <value>
        /// The midi block.
        /// </value>
        public CompactMidiBlock MidiBlock { get; set; }

        /// <summary>
        /// Gets or sets the given section.
        /// </summary>
        /// <value>
        /// The given section.
        /// </value>
        public MusicalSection MusicalSection { get; set; }

        #endregion

        #region Loaders

        /// <summary>
        /// Write Tones To Bars.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="givenSection">The given section.</param>
        public void LoadBlock(CompactMidiBlock givenBlock, MusicalSection givenSection)
        {
            if (givenBlock == null || givenSection == null) {
                return;
            }

            this.MidiBlock = givenBlock;
            this.MusicalSection = givenSection;
            //// givenBlock.CollectMidiEvents();
        }

        #endregion

        #region Public interface
        /// <summary>
        /// Get Next Event.
        /// </summary>
        /// <param name="barNumber">Bar Number.</param>
        /// <param name="midiTime">The midi time.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IMidiEvent GetNextEvent(int barNumber, long midiTime)
        {
            if (this.MidiBlock == null || barNumber < 1 || barNumber > this.MidiBlock.MidiBars.Count) {
                return null;
            }

            CompactMidiBar mbar;
            checked {
                mbar = this.MidiBlock.MidiBars[barNumber - 1];
            }

            var midiEvent = mbar?.EventQueue?.FirstOrDefault(); //// Peek();
            if (midiEvent == null || midiEvent.StartTime > midiTime) {
                return null;
            }

            if (mbar.EventQueue is Queue<IMidiEvent> events) {
                midiEvent = events.Dequeue();
            }

            return midiEvent;
        }

        #endregion
    }
}
