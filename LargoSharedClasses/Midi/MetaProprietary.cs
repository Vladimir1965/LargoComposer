// <copyright file="MetaProprietary.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;
using System.Text;

namespace LargoSharedClasses.Midi {
    /// <summary>A proprietary meta event message.</summary>
    [Serializable]
    public sealed class MetaProprietary : MetaEvent {
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x7F;

        #region Fields
        /// <summary>
        /// Event data.
        /// </summary>
        private byte[] data;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaProprietary class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenData">The data associated with the event.</param>
        public MetaProprietary(long deltaTime, byte[] givenData) :
            base(deltaTime, EventMetaId) {
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
            //// Contract.Requires(outputStream != null);
            if (outputStream == null) {
                return;
            }
            //// Write out the base event information
            base.Write(outputStream);

            // Event data
            var data1 = this.GetData();
            MidiEvent.WriteVariableLength(outputStream, data1?.Length ?? 0);
            if (data1 != null) {
                outputStream.Write(data1, 0, data1.Length);
            }
        }
        #endregion

        #region Private data
        /// <summary>
        /// Gets or sets the data for the event.
        /// </summary>
        /// <returns> Returns value. </returns>
        /// <value> General musical property.</value>
        private byte[] GetData() {
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
