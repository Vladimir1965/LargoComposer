// <copyright file="VoiceController.cs" company="Traced-Ideas, Czech republic">
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

    /// <summary>
    /// MIDI event to modify the tone with data from a pedal, lever, or other device; 
    /// also used for miscellaneous controls such as volume and bank select.
    /// </summary>
    [Serializable]
    public sealed class VoiceController : VoiceEvent
    {
        #region Fields
        /// <summary>The category status byte for Controller messages.</summary>
        private const byte CategoryStatusByte = 0xB;

        /// <summary>The type of controller message (0x0 to 0x7F).</summary>
        private byte number;

        /// <summary>The value of the controller message (0x0 to 0x7F).</summary>
        private byte controlValue;
        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the VoiceController class.</summary>
        /// <param name="deltaTime">The delta-time since the previous message.</param>
        /// <param name="channel">The channel to which to write the message (0 through 15).</param>
        /// <param name="number">The type of controller message to be written.</param>
        /// <param name="value">The value of the controller message.</param>
        public VoiceController(long deltaTime, MidiChannel channel, byte number, byte value) :
            base(deltaTime, CategoryStatusByte, channel) {
            this.Number = number;
            this.Value = value;
        }
        #endregion

        #region Properties
        /// <summary> Gets The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter1 => this.number;

        /// <summary> Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public override byte Parameter2 => this.controlValue;

        /// <summary>Gets or sets the value of the controller message (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        private byte Value {
            get => this.controlValue;

            set {
                if (value > 127) {
                    value = (byte)(value % 128);
                    //// throw new ArgumentOutOfRangeException("value", value, "The value must be in the range from 0 to 127.");
                }

                this.controlValue = value;
            }
        }

        /// <summary>Gets or sets type of controller message to be written (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        private byte Number {
            get => this.number;

            set => this.number = value;
        }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            if (Enum.IsDefined(typeof(MidiController), (int)this.number)) {
                sb.Append((MidiController)this.number);
            }
            else {
                sb.Append("0x");
                sb.Append(this.Number.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            }

            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Value.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(this.controlValue);
        }
        #endregion
    }
}
