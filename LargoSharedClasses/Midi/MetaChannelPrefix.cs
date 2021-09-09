// <copyright file="MetaChannelPrefix.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Midi {
    /// <summary>A channel prefix meta event message.</summary>
    [Serializable]
    public sealed class MetaChannelPrefix : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x20;

        /// <summary>The prefix for the event.</summary>
        private byte prefix;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaChannelPrefix class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenPrefix">The prefix for the event.</param>
        public MetaChannelPrefix(long deltaTime, byte givenPrefix)
            : base(deltaTime, EventMetaId) {
            this.Prefix = givenPrefix;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the prefix for the event.</summary>
        /// <value> General musical property.</value>
        private byte Prefix {
            get => this.prefix;

            set {
                if (value > 0x7F) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The prefix must be in the range from 0x0 to 0x7F.");
                }

                this.prefix = value;
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
            sb.Append(this.Prefix.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(0x01);
            outputStream.WriteByte(this.prefix);
        }
        #endregion
    }
}
