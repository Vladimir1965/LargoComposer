// <copyright file="MetaSequenceNumber.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Midi {
    /// <summary>A sequence number meta event message.</summary>
    [Serializable]
    public sealed class MetaSequenceNumber : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x0;

        /// <summary>The sequence number for the event.</summary>
        private int number;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaSequenceNumber class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="number">The sequence number for the event.</param>
        public MetaSequenceNumber(long deltaTime, int number)
            : base(deltaTime, EventMetaId) {
            this.Number = number;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the sequence number for the event.</summary>
        /// <value> General musical property.</value>
        private int Number {
            get => this.number;

            set {
                if (value < 0 || value > 0xFFFF) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The number must be in the range from 0x0 to 0xFFFF.");
                }

                this.number = value;
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
            sb.Append(this.Number.ToString(CultureInfo.CurrentCulture.NumberFormat));
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

            // Special meta event marker and the id of the event
            outputStream.WriteByte(0x02);
            outputStream.WriteByte((byte)((this.number & DefaultValue.MaskFirstByte) >> 8));
            outputStream.WriteByte((byte)(this.number & 0xFF));
        }
        #endregion
    }
}
