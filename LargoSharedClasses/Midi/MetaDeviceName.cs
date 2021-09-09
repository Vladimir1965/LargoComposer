// <copyright file="MetaDeviceName.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;

namespace LargoSharedClasses.Midi {
    /// <summary>A device name meta event.</summary>
    [Serializable]
    public sealed class MetaDeviceName : MetaAbstractText {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x9;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaDeviceName class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="text">The text associated with the event.</param>
        public MetaDeviceName(long deltaTime, string text)
            : base(deltaTime, EventMetaId, text) {
        }
        #endregion
    }
}
