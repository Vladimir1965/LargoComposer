// <copyright file="MidiEventSystemExclusive.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;
using System.Text;

namespace LargoSharedClasses.Midi
{
    /// <summary>A system exclusive MIDI event.</summary>
    [Serializable]
    public sealed class MidiEventSystemExclusive : MidiEvent {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiEventSystemExclusive class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        private MidiEventSystemExclusive(long deltaTime) //// , byte[] givenData
            : base(deltaTime) {
            //// this.Data = givenData;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the data for this event.</summary>
        /// <value> General musical property.</value>
        private byte[] Data { get; set; } //// virtual
        #endregion

        /// <summary>
        /// News the midi event system exclusive.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="givenData">The given data.</param>
        /// <returns> Returns value. </returns>
        public static MidiEventSystemExclusive NewMidiEventSystemExclusive(long deltaTime, byte[] givenData) {
            var ev = new MidiEventSystemExclusive(deltaTime) { Data = givenData };
            return ev;
        }

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            if (this.Data != null) {
                sb.Append("\t");
            }

            sb.Append(MidiEvent.DataToString(this.Data));
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
            outputStream.WriteByte((byte)MidiCommandCode.SystemExclusive);
            MidiEvent.WriteVariableLength(outputStream, 1 + (this.Data?.Length ?? 0)); // "1+" for the F7 at the end
            if (this.Data != null) {
                outputStream.Write(this.Data, 0, this.Data.Length);
            }

            outputStream.WriteByte((byte)MidiCommandCode.EndOfSystemExclusive);
        }
        #endregion
    }
}
