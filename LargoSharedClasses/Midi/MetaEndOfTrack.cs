// <copyright file="MetaEndOfTrack.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;

namespace LargoSharedClasses.Midi {
    /// <summary>An end of track meta event message.</summary>
    [Serializable]
    public sealed class MetaEndOfTrack : MetaEvent {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x2F;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaEndOfTrack class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        public MetaEndOfTrack(long deltaTime)
            : base(deltaTime, EventMetaId) {
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
            //// End of track
            outputStream.WriteByte(0x00);
        }
        #endregion
    }
}
