// <copyright file="MusicalDenominator.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Denominator.
    /// </summary>
    public enum MusicalDenominator
    {
        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Musical Denominator. </summary>
        Whole = 1,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] Half = 2,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] Quarter = 4,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] Eighth = 8,

        /// <summary> Musical Denominator. </summary>
        Sixteenth = 16,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] D32Nd = 32,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] D64Th = 64,

        /// <summary> Musical Denominator. </summary>
        D128Th = 128,

        /// <summary> Musical Denominator. </summary>
        [UsedImplicitly] Unknown = 6
    }
}
