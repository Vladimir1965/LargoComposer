// <copyright file="VoiceProgramChange.cs" company="Traced-Ideas, Czech republic">
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
    using LargoSharedClasses.Melody;
    using Music;

    /// <summary>MIDI event to select an instrument for the channel by assigning a patch number.</summary>
    [Serializable]
    public sealed class VoiceProgramChange : VoiceEvent
    {
        #region Fields
        /// <summary>The category status byte for ProgramChange messages.</summary>
        private const byte CategoryStatusByte = 0xC;

        /// <summary>The number of the program to which to change (0x0 to 0x7F).</summary>
        private byte number;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the VoiceProgramChange class.</summary>
        /// <param name="deltaTime">The delta-time since the previous message.</param>
        /// <param name="channel">The channel to which to write the message (0 through 15).</param>
        /// <param name="number">The instrument to which to change.</param>
        public VoiceProgramChange(long deltaTime, MidiChannel channel, MidiMelodicInstrument number) :
            this(deltaTime, channel, (byte)number) {
        }

        /// <summary>Initializes a new instance of the VoiceProgramChange class.</summary>
        /// <param name="deltaTime">The delta-time since the previous message.</param>
        /// <param name="channel">The channel to which to write the message (0 through 15).</param>
        /// <param name="number">The number of the program to which to change.</param>
        public VoiceProgramChange(long deltaTime, MidiChannel channel, byte number) :
            base(deltaTime, CategoryStatusByte, channel) {
            this.Number = number;
        }
        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">value - The number must be in the range from 0 to 127.</exception>
        public byte Number {
            get => this.number;

            set { //// 2020/10 private
                if (value > 127) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The number must be in the range from 0 to 127.");
                }

                this.number = value;
            }
        }

        /// <summary> Gets The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter1 => this.number;

        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => 0;

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            if (Enum.IsDefined(typeof(MidiMelodicInstrument), (int)this.number)) {
                sb.Append((MidiMelodicInstrument)this.number);
            }
            else {
                sb.Append("0x");
                sb.Append(this.number.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            }

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
            outputStream.WriteByte(this.number);
        }
        #endregion
    }
}
