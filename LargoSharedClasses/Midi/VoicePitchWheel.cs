// <copyright file="VoicePitchWheel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi
{
    using Abstract;
    using JetBrains.Annotations;
    using Music;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>MIDI event to modify the pitch of all notes played on the channel.</summary>
    [Serializable]
    public sealed class VoicePitchWheel : VoiceEvent {
        #region Fields
        /// <summary>The category status byte for PitchWheel messages.</summary>
        private const byte CategoryStatusByte = 0xE;

        /// <summary>The upper 7-bits of the wheel position..</summary>
        private byte upperBits;

        /// <summary>The lower 7-bits of the wheel position..</summary>
        private byte lowerBits;
        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the VoicePitchWheel class.</summary>
        /// <param name="deltaTime">The delta-time since the previous message.</param>
        /// <param name="channel">The channel to which to write the message (0 through 15).</param>
        /// <param name="upperBits">The upper 7 bits of the position.</param>
        /// <param name="lowerBits">The lower 7 bits of the position.</param>
        public VoicePitchWheel(long deltaTime, MidiChannel channel, byte upperBits, byte lowerBits) :
            base(deltaTime, CategoryStatusByte, channel) {
            this.UpperBits = upperBits;
            this.LowerBits = lowerBits;
        }

        /// <summary>Initializes a new instance of the VoicePitchWheel class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenCategory">The category identifier (0x0 through 0xF) for this voice event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        public VoicePitchWheel(long deltaTime, byte givenCategory, MidiChannel channel)
            : base(deltaTime, givenCategory, channel) {
        }
        #endregion
        #region Properties
        /// <summary> Gets The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter1 => (byte)((this.Position & DefaultValue.MaskFirstByte) >> 8);

        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => (byte)(this.Position & 0xFF);

        /// <summary>Gets or sets the upper 7 bits of the position.</summary>
        /// <value> General musical property.</value>
        private byte UpperBits {
            get => this.upperBits;

            set {
                if (this.upperBits > 0x7F) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be in the range from 0x0 to 0x7F.");
                }

                this.upperBits = value;
            }
        }

        /// <summary>Gets or sets the lower 7 bits of the position.</summary>
        /// <value> General musical property.</value>
        private byte LowerBits {
            get => this.lowerBits;

            set {
                if (this.lowerBits > 0x7F) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be in the range from 0x0 to 0x7F.");
                }

                this.lowerBits = value;
            }
        }

        /// <summary>Gets or sets the wheel position.</summary>
        /// <value> General musical property.</value>
        private int Position {
            get => MidiEvent.CombineBytesTo14Bits(this.upperBits, this.lowerBits);

            [UsedImplicitly]
            set {
                if (value < 0 || value > 0x3FFF) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Pitch wheel position must be in the range from 0x0 to 0x3FFF.");
                }

                MidiEvent.Split14BitsToBytes(value, out this.upperBits, out this.lowerBits);
            }
        }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append(this.Position.ToString(CultureInfo.CurrentCulture.NumberFormat));
            sb.Append(" (");
            sb.Append(this.LowerBits.ToString(CultureInfo.CurrentCulture.NumberFormat));
            sb.Append(",");
            sb.Append(this.UpperBits.ToString(CultureInfo.CurrentCulture.NumberFormat));
            sb.Append(")");
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

            // Write out the data
            //// int position = this.Position;
            outputStream.WriteByte(this.Parameter1);
            outputStream.WriteByte(this.Parameter2);
        }
        #endregion
    }
}
