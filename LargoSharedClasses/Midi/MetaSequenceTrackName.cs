﻿// <copyright file="MetaSequenceTrackName.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;

namespace LargoSharedClasses.Midi {
    /// <summary>A sequence/track name meta event.</summary>
    [Serializable]
    public sealed class MetaSequenceTrackName : MetaAbstractText {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x3;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaSequenceTrackName class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="text">The text associated with the event.</param>
        public MetaSequenceTrackName(long deltaTime, string text)
            : base(deltaTime, EventMetaId, text) {
        }
        #endregion
    }
}
