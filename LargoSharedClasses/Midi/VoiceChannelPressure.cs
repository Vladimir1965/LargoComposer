// <copyright file="VoiceChannelPressure.cs" company="Traced-Ideas, Czech republic">
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
    
    /// <summary>MIDI event to apply pressure to a channel's currently playing notes.</summary>
    [Serializable]
    public sealed class VoiceChannelPressure : VoiceEvent {
        #region Fields
        /// <summary>The category status byte for ChannelPressure messages.</summary>
        private const byte CategoryStatusByte = 0xD;

        /// <summary>The amount of pressure to be applied (0x0 to 0x7F).</summary>
        private byte pressure;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the VoiceChannelPressure class.</summary>
        /// <param name="deltaTime">The delta-time since the previous message.</param>
        /// <param name="channel">The channel to which to write the message (0 through 15).</param>
        /// <param name="givenPressure">The pressure to be applied.</param>
        public VoiceChannelPressure(long deltaTime, MidiChannel channel, byte givenPressure) :
            base(deltaTime, CategoryStatusByte, channel) {
            this.Pressure = givenPressure;
        }
        #endregion
        #region Properties
        /// <summary> Gets The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter1 => this.pressure;

        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => 0;

        /// <summary>Gets or sets the amount pressure to be applied (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        private byte Pressure {
            get => this.pressure;

            set {
                if (value > 127) {
                    this.pressure = 127;
                    return;
                    //// throw new ArgumentOutOfRangeException("value", value, "The pressure must be in the range from 0 to 127.");
                }

                this.pressure = value;
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
            sb.Append("0x");
            sb.Append(this.Pressure.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(this.pressure);
        }
        #endregion
    }
}
