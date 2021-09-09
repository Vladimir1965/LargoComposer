// <copyright file="TonalityGenus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary>The tonality of the key signature (major or minor).</summary>
    [Serializable]
    public enum TonalityGenus {
        #region Major/Minor
        /// <summary>Key is major.</summary>
        Major = 0,

        /// <summary>Key is minor.</summary>
        [UsedImplicitly] Minor = 1
        #endregion
    }
}
