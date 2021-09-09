// <copyright file="OutputDeviceException.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// The exception that is thrown when a error occurs with the OutputDevice
    /// class.
    /// </summary>
    [Serializable]
    public sealed class OutputDeviceException : MidiDeviceException {
        #region Fields

        /// <summary>
        /// The error message.
        /// </summary>
        private readonly StringBuilder message = new StringBuilder(128);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class with the specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        public OutputDeviceException(int errCode)
            : base(errCode) {
            // Get error message.
                MidiOutGetErrorText(errCode, this.message, this.message.Capacity);
        }

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class.
        /// </summary>
        [UsedImplicitly]
        public OutputDeviceException() {
        }

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class.
        /// </summary>
        /// <param name="message">Midi message.</param>
        public OutputDeviceException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class.
        /// </summary>
        /// <param name="message">Midi message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public OutputDeviceException(string message, Exception innerException)
            : base(message, innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming Context.</param>
        private OutputDeviceException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value> General property.</value>
        public override string Message => this.message.ToString();

        #endregion

        #region Win32 Midi Output Error Function

        [DllImport("winmm.dll", EntryPoint = "midiOutGetErrorText")]
        private static extern int MidiOutGetErrorText(int errCode, StringBuilder message, int sizeOfMessage);

        #endregion
    }
}
