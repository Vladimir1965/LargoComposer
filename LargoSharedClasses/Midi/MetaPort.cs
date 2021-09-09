// <copyright file="MetaPort.cs" company="Traced-Ideas, Czech republic">
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
    /// <summary>A MIDI port meta event message.</summary>
    [Serializable]
    public sealed class MetaPort : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x21;

        /// <summary>The port for the event.</summary>
        private byte port;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaPort"/> class.
        /// </summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenPort">The port for the event.</param>
        public MetaPort(long deltaTime, byte givenPort)
            : base(deltaTime, EventMetaId) {
            this.Port = givenPort;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the port for the event.</summary>
        /// <value> General musical property.</value>
        private byte Port {
            get => this.port;

            set {
                if (value > 0x7F) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The port must be in the range from 0x0 to 0x7F.");
                }

                this.port = value;
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
            sb.Append(this.Port.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(this.port);
        }
        #endregion
    }
}
