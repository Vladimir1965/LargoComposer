// <copyright file="MetaEvent.cs" company="Traced-Ideas, Czech republic">
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
    /// <summary>Represents a meta event message.</summary>
    [Serializable]
    public abstract class MetaEvent : MidiEvent {
        #region Fields
        /// <summary>The ID of the meta event.</summary>
        private readonly byte metaEventId;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaEvent class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The ID of the meta event.</param>
        protected MetaEvent(long deltaTime, byte givenMetaEventId)
            : base(deltaTime) {
            this.metaEventId = givenMetaEventId;
        }
        #endregion

        #region Properties
        /// <summary>Gets the ID of this meta event.</summary>
        /// <value> General musical property.</value>
        private byte MetaEventId => this.metaEventId;

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append(" MetaId=0x");
            sb.Append(this.MetaEventId.ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
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
            outputStream.WriteByte(0xFF);
            outputStream.WriteByte(this.metaEventId);
        }
        #endregion
    }
}