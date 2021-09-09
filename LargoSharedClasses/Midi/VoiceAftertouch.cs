// <copyright file="VoiceAftertouch.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>MIDI event to modify a note according to the aftertouch of a key.</summary>
    [Serializable]
    public sealed class VoiceAftertouch : VoiceAbstractNote {
        #region Fields
        /// <summary>The category status byte for Aftertouch messages.</summary>
        private const byte CategoryStatusByte = 0xA;
        
        /// <summary>The pressure of the note (0x0 to 0x7F).</summary>
        private byte pressure;

        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the VoiceAftertouch class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        /// <param name="note">The MIDI note to modify (0x0 to 0x7F).</param>
        /// <param name="givenPressure">The pressure of the note (0x0 to 0x7F).</param>
        public VoiceAftertouch(long deltaTime, MidiChannel channel, byte note, byte givenPressure) :
            base(deltaTime, CategoryStatusByte, channel, note) {
            this.Pressure = givenPressure;
        }
        #endregion

        #region Properties
        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => this.pressure;

        /// <summary>Gets or sets the pressure of the note (0x0 to 0x7F).</summary>
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
            //// Contract.Requires(outputStream != null);
            if (outputStream == null) {
                return;
            }
            //// Write out the base event information
            base.Write(outputStream);

            //// Write out the data
            outputStream.WriteByte(this.pressure);
        }
        #endregion
    }
}
