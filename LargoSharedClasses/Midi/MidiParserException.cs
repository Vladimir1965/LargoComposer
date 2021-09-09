// <copyright file="MidiParserException.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes for parsing track data into actual MIDI track and event objects

using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Security.Permissions;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>Exception thrown when an error is encountered during the parsing of a MIDI file.</summary>
    [Serializable]
    public sealed class MidiParserException : Exception { ////
        #region Fields
        /// <summary>Position in the data stream that caused the exception.</summary>
        private readonly long position;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiParserException class.</summary>
        [UsedImplicitly]
        public MidiParserException() {
        }

        /// <summary>Initializes a new instance of the MidiParserException class.</summary>
        /// <param name="message">The message for the exception.</param>
        /// <param name="position">Position in the data stream that caused the exception.</param>
        public MidiParserException(string message, long position) :
            base(message) {
            this.position = position;
        }

        /// <summary>Initializes a new instance of the MidiParserException class.</summary>
        /// <param name="message">The message for the exception.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public MidiParserException(string message, Exception innerException) :
            base(message, innerException) {
        } 

        /// <summary>Initializes a new instance of the MidiParserException class.</summary>
        /// <param name="message">The message for the exception.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        /// <param name="position">Position in the data stream that caused the exception.</param>
        public MidiParserException(string message, Exception innerException, long position) :
            base(message, innerException) {
            this.position = position;
        }

        /// <summary>Initializes a new instance of the MidiParserException class.</summary>
        /// <param name="info">Serialization information to rebuild this exception.</param>
        /// <param name="context">Serialization context used to rebuild this exception.</param>
        private MidiParserException(SerializationInfo info, StreamingContext context) :
            base(info, context) {
                Contract.Requires(info != null);
                this.position = info.GetInt64("position");
        }

        #endregion

        #region Properties
        /* MIDI - Position that caused the exception.
        /// <summary>Gets the byte position that caused the exception.</summary>
        /// <value> General musical property.</value>
        [UsedImplicitly]
        private long Position { get { return this.position; } } */ 
        #endregion

        /// <summary>Serialize the exception information.</summary>
        /// <param name="info">Serialization information in which to store the exception data.</param>
        /// <param name="context">Serialization context in which to store the exception data.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            //// if (info == null) {  return; }
            //// Add data to serialization info
            info.AddValue("position", this.position);

            // Add all base data
            base.GetObjectData(info, context);
        }
    }
}
