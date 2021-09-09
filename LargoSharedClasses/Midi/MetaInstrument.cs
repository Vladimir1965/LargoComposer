// <copyright file="MetaInstrument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>A instrument name meta event.</summary>
    [Serializable]
    public sealed class MetaInstrument : MetaAbstractText {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x4;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaInstrument class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="text">The text associated with the event.</param>
        public MetaInstrument(long deltaTime, string text)
            : base(deltaTime, EventMetaId, text) {
        }

        /// <summary>Initializes a new instance of the MetaInstrument class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The ID of the meta event.</param>
        /// <param name="text">The text associated with the event.</param>
        [UsedImplicitly]
        public MetaInstrument(long deltaTime, byte givenMetaEventId, string text)
            : base(deltaTime, givenMetaEventId, text) {
        }
         
        #endregion
    }
}
