// <copyright file="VoiceEvent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi
{
    using System;
    using System.IO;
    using System.Text;
    using JetBrains.Annotations;
    using Music;

    /// <summary>Represents a voice category message.</summary>
    [Serializable]
    public abstract class VoiceEvent : MidiEvent
    {
        #region Fields
        /// <summary>The status identifier (0x0 through 0xF) for this voice event.</summary>
        private readonly byte category;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the VoiceEvent class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenCategory">The category identifier (0x0 through 0xF) for this voice event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        protected VoiceEvent(long deltaTime, byte givenCategory, MidiChannel channel)
            : base(deltaTime) {
            // Validate the parameters
            if (givenCategory > 0xF) {
                throw new ArgumentOutOfRangeException(nameof(givenCategory), givenCategory, "Category values must be in the range from 0x0 to 0xF.");
            }

            // Store the data
            this.category = givenCategory;
            this.Channel = channel;
        }

        /// <summary>Initializes a new instance of the VoiceEvent class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        protected VoiceEvent(long deltaTime)
            : base(deltaTime) {
        }

        #endregion

        #region Properties
        /// <summary>Gets The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public abstract byte Parameter1 { get; }

        /// <summary>Gets The second parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public abstract byte Parameter2 { get; }

        /// <summary>Gets the status identifier (0x0 through 0xF) for this voice event.</summary>
        /// <value> General musical property.</value>
        [UsedImplicitly]
        public byte Category => this.category;

        /// <summary>Gets or sets the channel (0x0 through 0xF) for this voice event.</summary>
        /// <value> General musical property.</value>
        public MidiChannel Channel { get; set; }  //// virtual (11/2010)

        /// <summary>Gets the status byte for the event message (combination of category and channel).</summary>
        /// <value> General musical property.</value>
        public byte Status => this.GetStatusByte();

        /// <summary>Gets the word that represents this event as a MIDI event message.</summary>
        [UsedImplicitly]
        internal int Message => this.Status | (this.Parameter1 << 8) | (this.Parameter2 << 16);

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" \t");
            sb.Append("C=" + this.Channel);
            //// sb.Append("\t");
            //// sb.Append("0x");
            //// sb.Append(this.Channel.ToString("X1", System.Globalization.CultureInfo.CurrentCulture.NumberFormat));
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

            // Write out the status byte
            outputStream.WriteByte(this.GetStatusByte());
        }

        /// <summary>Gets the status byte for the message event.</summary>
        /// <returns>The status byte (combination of category and channel) for the message event.</returns>
        private byte GetStatusByte() {
            // The command is the upper 4 bits and the channel is the lower 4.
            return (byte)((this.category << 4) | (byte)this.Channel);  //// was cast to int before this.category
        }
        #endregion
    }
}
