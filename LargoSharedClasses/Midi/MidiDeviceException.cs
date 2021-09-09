// <copyright file="MidiDeviceException.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Contains MidiError enumeration.</summary>

using System;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// The base class for all MIDI device exception classes.
    /// </summary>
    [Serializable]
    public class MidiDeviceException : Exception //// DeviceException
    {
        #region Error Codes

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MidiDeviceException class with the
        /// specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        protected MidiDeviceException(int errCode)  ////  : base(errCode)
        {
            this.ErrorCode = errCode;
        }

        /// <summary>
        /// Initializes a new instance of the MidiDeviceException class.
        /// </summary>
        protected MidiDeviceException() {
        }

        /// <summary>
        /// Initializes a new instance of the MidiDeviceException class .
        /// </summary>
        /// <param name="message">Midi message.</param>
        protected MidiDeviceException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the MidiDeviceException class.
        /// </summary>
        /// <param name="message">Midi message.</param>
        /// <param name="innerException">Inner Exception.</param>
        protected MidiDeviceException(string message, Exception innerException)
            : base(message, innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the MidiDeviceException class.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming Context.</param>
        protected MidiDeviceException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets Error Code.
        /// </summary>
        /// <value> General property.</value>
        private int ErrorCode { [UsedImplicitly] get; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("ErrorCode {0}", this.ErrorCode);

            return s.ToString();
        }
        #endregion

        #region Methods
        /* MidiDeviceException ?!
        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        // ReSharper disable once RedundantOverriddenMember
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        */
        #endregion
    }
}
