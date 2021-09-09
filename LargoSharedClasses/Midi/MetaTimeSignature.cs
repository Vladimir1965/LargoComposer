// <copyright file="MetaTimeSignature.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>A time signature meta event message.</summary>
    [Serializable]
    public sealed class MetaTimeSignature : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x58;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaTimeSignature class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="numerator">Numerator of the time signature.</param>
        /// <param name="denominator">Negative power of two, denominator of time signature.</param>
        /// <param name="midiClocksPerClick">The number of MIDI clocks per metronome click.</param>
        /// <param name="numberOfNotated32">The number of notated 32-nd notes per MIDI quarter note.</param>
        public MetaTimeSignature(
                    long deltaTime,
                    byte numerator, 
                    byte denominator, 
                    byte midiClocksPerClick, 
                    byte numberOfNotated32) :
            base(deltaTime, EventMetaId) {
            this.Numerator = numerator;
            this.Denominator = denominator;
            this.MidiClocksPerClick = midiClocksPerClick;
            this.NumberOfNotated32 = numberOfNotated32;
        }

        /// <summary>Initializes a new instance of the MetaTimeSignature class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The ID of the meta event.</param>
        [UsedImplicitly]
        public MetaTimeSignature(long deltaTime, byte givenMetaEventId)
            : base(deltaTime, givenMetaEventId) {
        }
         
        #endregion

        #region Properties
        /// <summary>Gets the numerator for the event.</summary>
        /// <value> General musical property.</value>
        public byte Numerator { get; }

        /// <summary>Gets the denominator for the event.</summary>
        /// <value> General musical property.</value>
        public byte Denominator { get; }

        /// <summary>Gets the MIDI clocks per click for the event.</summary>
        /// <value> General musical property.</value>
        private byte MidiClocksPerClick { get; }

        /// <summary>Gets the number of notated 32 notes per MIDI quarter note for the event.</summary>
        /// <value> General musical property.</value>
        private byte NumberOfNotated32 { get; }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Numerator.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Denominator.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.MidiClocksPerClick.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.NumberOfNotated32.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>Write the event to the output stream.</summary>
        /// <param name="outputStream">The stream to which the event should be written.</param>
        public override void Write(Stream outputStream) {
            if (outputStream == null) {
                return;
            }
            //// Write out the base event information
            base.Write(outputStream);
            //// Event data
            outputStream.WriteByte(0x04);
            outputStream.WriteByte(this.Numerator);
            outputStream.WriteByte(this.Denominator);
            outputStream.WriteByte(this.MidiClocksPerClick);
            outputStream.WriteByte(this.NumberOfNotated32);
        }
        #endregion
    }
}
