// <copyright file="RawTempo.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary> Musical tempo. </summary>
    public enum RawTempo {
        /// <summary> No Raw tempo. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Raw tempo - Very Slow. </summary>
        [UsedImplicitly] VerySlow = 1,

        /// <summary> Raw tempo - Slow. </summary>
        [UsedImplicitly] Slow = 2,

        /// <summary> Raw tempo - Middle. </summary>
        [UsedImplicitly] Middle = 3,

        /// <summary> Raw tempo - Fast. </summary>
        [UsedImplicitly] Fast = 4,

        /// <summary> Raw tempo - Very Fast. </summary>
        [UsedImplicitly] VeryFast = 5,
    }
}
