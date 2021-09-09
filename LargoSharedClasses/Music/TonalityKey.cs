// <copyright file="TonalityKey.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary>The number of sharps or flats in the key signature.</summary>
    [Serializable]
    public enum TonalityKey //// This should be sbyte, but sbyte is not CLS-compliant, so we'll leave it as int
    {
        /// <summary>No flat defined.</summary>
        None = -1,

        #region Defined Keys
        /// <summary>Key has no sharps or flats.</summary>
        [UsedImplicitly] TonalityC = 0,

        /// <summary>Key has 1 flat.</summary>
        [UsedImplicitly] TonalityF = -1,

        /// <summary>Key has 2 flats.</summary>
        [UsedImplicitly] TonalityB = -2,

        /// <summary>Key has 3 flats.</summary>
        [UsedImplicitly] TonalityEb = -3,

        /// <summary>Key has 4 flats.</summary>
        [UsedImplicitly] TonalityAb = -4,

        /// <summary>Key has 5 flats.</summary>
        [UsedImplicitly] TonalityDb = -5,

        /// <summary>Key has 6 flats.</summary>
        [UsedImplicitly] TonalityGb = -6,

        /// <summary>Key has 7 flats.</summary>
        [UsedImplicitly] TonalityCb = -7,

        /// <summary>Key has 1 sharp.</summary>
        [UsedImplicitly] TonalityG = 1,

        /// <summary>Key has 2 sharps.</summary>
        [UsedImplicitly] TonalityD = 2,

        /// <summary>Key has 3 sharps.</summary>
        [UsedImplicitly] TonalityA = 3,

        /// <summary>Key has 4 sharps.</summary>
        [UsedImplicitly] TonalityE = 4,

        /// <summary>Key has 5 sharps.</summary>
        [UsedImplicitly] TonalityH = 5,

        /// <summary>Key has 6 sharps.</summary>
        [UsedImplicitly] TonalityFs = 6,

        /// <summary>Key has 7 sharps.</summary>
        [UsedImplicitly] TonalityCs = 7
        #endregion
    }
}
