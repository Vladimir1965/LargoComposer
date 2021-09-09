// <copyright file="MetaTimeCodeOffset.cs" company="Traced-Ideas, Czech republic">
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
    /// <summary>An SMPTE offset meta event message.</summary>
    [Serializable]
    public sealed class MetaTimeCodeOffset : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x54;

        /// <summary>The status byte for TimeCodeOffset (?!).</summary>
        private const byte StatusByte = 0x05;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaTimeCodeOffset class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenHours">Hours for the event.</param>
        /// <param name="givenMinutes">Minutes for the event.</param>
        /// <param name="givenSeconds">Seconds for the event.</param>
        /// <param name="givenFrames">Frames for the event.</param>
        /// <param name="givenFractionalFrames">Fractional frames for the event.</param>
        public MetaTimeCodeOffset(
                        long deltaTime,
                        byte givenHours,
                        byte givenMinutes,
                        byte givenSeconds,
                        byte givenFrames,
                        byte givenFractionalFrames) :
            base(deltaTime, EventMetaId) {
            this.Hours = givenHours;
            this.Minutes = givenMinutes;
            this.Seconds = givenSeconds;
            this.Frames = givenFrames;
            this.FractionalFrames = givenFractionalFrames;
        }

        /// <summary>Initializes a new instance of the MetaTimeCodeOffset class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The ID of the meta event.</param>
        [UsedImplicitly]
        public MetaTimeCodeOffset(long deltaTime, byte givenMetaEventId)
            : base(deltaTime, givenMetaEventId) {
        }
         
        #endregion

        #region Properties
        /// <summary>Gets the hours for the event.</summary>
        /// <value> General musical property.</value>
        private byte Hours { get; }

        /// <summary>Gets the minutes for the event.</summary>
        /// <value> General musical property.</value>
        private byte Minutes { get; }

        /// <summary>Gets the seconds for the event.</summary>
        /// <value> General musical property.</value>
        private byte Seconds { get; }

        /// <summary>Gets the frames for the event.</summary>
        /// <value> General musical property.</value>
        private byte Frames { get;  }

        /// <summary>Gets the fractional frames for the event.</summary>
        /// <value> General musical property.</value>
        private byte FractionalFrames { get; }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Hours.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Minutes.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Seconds.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.Frames.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            sb.Append("\t");
            sb.Append("0x");
            sb.Append(this.FractionalFrames.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(StatusByte);
            outputStream.WriteByte(this.Hours);
            outputStream.WriteByte(this.Minutes);
            outputStream.WriteByte(this.Seconds);
            outputStream.WriteByte(this.Frames);
            outputStream.WriteByte(this.FractionalFrames);
        }
        #endregion
    }
}
