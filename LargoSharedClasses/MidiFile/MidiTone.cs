// <copyright file="MidiTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to represent one MIDI tone.

using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Midi Tone.
    /// </summary>
    public sealed class MidiTone : IMidiTone
    {
        /// <summary>
        /// Initializes a new instance of the MidiTone class.
        /// </summary>
        /// <param name="startTime">Start time.</param>
        /// <param name="noteNumber">The note number.</param>
        /// <param name="velocity">Midi Velocity.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="channel">The channel.</param>
        public MidiTone(long startTime, byte noteNumber, byte velocity, byte instrument, MidiChannel channel) {
            this.StartTime = startTime;
            this.NoteNumber = noteNumber;
            this.Note = MusicalProperties.GetNoteNameAndOctave(this.NoteNumber, DefaultValue.HarmonicOrder);
            this.Velocity = velocity;
            this.InstrumentNumber = instrument;
            this.Channel = channel;
            this.FirstInBar = false;
            //// this.IsReady = false;
            this.Loudness = MusicalProperties.LoudnessOfVelocity(this.Velocity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTone"/> class.
        /// </summary>
        [UsedImplicitly]
        public MidiTone() {
        }

        #region Properties
        /// <summary> Gets The amount of time before tone.</summary>
        /// <value> General musical property.</value>
        public long StartTime { get; }

        /// <summary>Gets The MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public byte NoteNumber { get; }

        /// <summary>Gets The Instrument (0x0 to 0x7F).</summary>
        /// <value> Property description. </value>
        public byte InstrumentNumber { get; }

        /// <summary>Gets The Channel (0x0 to 0x7F).</summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; }

        /// <summary>Gets or sets The amount of time.</summary>
        /// <value> Property description. </value>
        public long Duration { get; set; }

        /// <summary>Gets or sets Number of bar.</summary>
        /// <value> Property description. </value>
        public int BarNumberFrom { get; set; }

        /// <summary>Gets or sets Number of bar.</summary>
        /// <value> Property description. </value>
        public int BarNumberTo { get; set; }

        /// <summary>Gets or sets a value indicating whether Number of bar.</summary>
        /// <value> Property description. </value>
        public bool FirstInBar { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets the loudness.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalLoudness Loudness { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [sound through].
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public bool SoundThrough { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is rhythmic.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is rhythmic; otherwise, <c>false</c>.
        /// </value>
        private bool IsRhythmic => this.Channel == MidiChannel.DrumChannel;

        /// <summary>
        /// Gets the name of the instrument.
        /// </summary>
        /// <value>
        /// The name of the instrument.
        /// </value>
        private string InstrumentName {
            get {
                var s = this.IsRhythmic ? ((MidiRhythmicInstrument)this.InstrumentNumber).ToString() : ((MidiMelodicInstrument)this.InstrumentNumber).ToString();
                return s;
            }
        }

        /// <summary>Gets The velocity of the note (0x0 to 0x7F).</summary>
        /// <value> Property description. </value>
        private byte Velocity { get; }
        #endregion

        #region Private Properties

        /// <summary>Gets The MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        private string Note { get; }
        #endregion

        /// <summary>
        /// Complete Duration.
        /// </summary>
        /// <param name="endDeltaTime">End Delta Time.</param>
        /// <param name="fullLength">Full length.</param>
        /// <param name="givenDivision">Given division.</param>
        public void CompleteDuration(long endDeltaTime, bool fullLength, int givenDivision) {
            //// when note off events are missing 
            if (this.Duration != 0) {
                return;
            }

            this.Duration = fullLength ? endDeltaTime - this.StartTime : endDeltaTime - this.StartTime - 1;
            //// 2019/02 - subtracted 1 from endDeltaTime - e.g. the case endDeltaTime=1440, division=720 gives bar-to 3 instead of 2 ... 
            var quotient = (double)(endDeltaTime - 1) / givenDivision;
            this.BarNumberTo = (int)Math.Floor(quotient) + 1;
        }

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            //// sb.Append(this.DeltaTime.ToString(CultureInfo.CurrentCulture));
            //// sb.Append(" ");
            sb.AppendFormat("Note {0}", this.Note);
            sb.Append(" (");
            sb.Append(this.StartTime.ToString(CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("* ");
            sb.Append(this.Duration.ToString(CultureInfo.CurrentCulture.NumberFormat));
            //// sb.Append(", ");
            sb.Append(string.Format(CultureInfo.CurrentCulture, "Bar {0}-{1} ", this.BarNumberFrom, this.BarNumberTo));
            //// sb.Append(this.Velocity.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat));
            sb.Append(" Instrument:");
            sb.Append(this.InstrumentName);
            sb.Append(")");
            return sb.ToString();
        }
        #endregion
    }
}
