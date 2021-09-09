// <copyright file="VoiceNoteOn.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Music;

    /// <summary>Midi event to play a note.</summary>
    [Serializable]
    public sealed class VoiceNoteOn : VoiceAbstractNote {
        #region Fields
        /// <summary>The category status byte for VoiceNoteOn messages.</summary>
        private const byte CategoryStatusByte = 0x9;

        /// <summary>The velocity of the note (0x0 to 0x7F).</summary>
        private byte velocity;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the VoiceNoteOn class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        /// <param name="note">The MIDI note to sound (0x0 to 0x7F).</param>
        /// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
        public VoiceNoteOn(long deltaTime, MidiChannel channel, byte note, byte velocity) :
            base(deltaTime, CategoryStatusByte, channel, note) {
            this.Velocity = velocity;
        }
        #endregion

        #region Properties
        /// <summary>Gets the velocity of the note (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public byte Velocity {
            get => this.velocity;

            private set {
                if (value > 127) {
                    this.velocity = 127;
                    //// throw new ArgumentOutOfRangeException("value", value, "The velocity must be in the range from 0 to 127.");
                }

                this.velocity = value;
            }
        }

        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => this.velocity;

        #endregion

        #region Static Methods

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append(" v=");
            sb.Append(this.velocity.ToString(CultureInfo.CurrentCulture.NumberFormat)); //// ToString("X2"
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

            //// Write out the data
            outputStream.WriteByte(this.velocity);
        }
        #endregion
    }
}
