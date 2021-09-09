// <copyright file="MetaUnknown.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace LargoSharedClasses.Midi {
    /// <summary>An unknown meta event message.</summary>
    [Serializable]
    public sealed class MetaUnknown : MetaEvent {
        #region Fields
        /// <summary>
        /// Event data.
        /// </summary>
        private byte[] data;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaUnknown class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The event ID for this meta event.</param>
        /// <param name="givenData">The data associated with the event.</param>
        public MetaUnknown(long deltaTime, byte givenMetaEventId, byte[] givenData) :
            base(deltaTime, givenMetaEventId) {
            this.SetData(givenData);
        }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            if (this.GetData() != null) {
                sb.Append("\t");
            }

            sb.Append(MidiEvent.DataToString(this.GetData()));
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
            MidiEvent.WriteVariableLength(outputStream, this.GetData() != null ? this.GetData().Length : 0);
            if (this.GetData() != null) {
                outputStream.Write(this.GetData(), 0, this.GetData().Length);
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// Gets or sets the data for the event.
        /// </summary>
        /// <returns> Returns value. </returns>
        /// <value> General musical property.</value>
        private byte[] GetData() {
            Contract.Ensures(Contract.Result<byte[]>() != null);
            if (this.data == null) {
                throw new InvalidOperationException("Midi event data can not be null.");
            }

            return this.data;
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetData(byte[] value) {
            this.data = value;
        }

        #endregion
    }
}
