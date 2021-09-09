// <copyright file="MusicalLoudness.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Musical loudness. </summary>
    public enum MusicalLoudness {
        /// <summary> Musical loudness. </summary>
        None = 0,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] P3 = 1,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] P2 = 2,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] P1 = 3,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Mp = 4,

        /// <summary> Musical loudness. </summary>
        MeanLoudness = 5,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Mf = 6,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] F1 = 7,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] F2 = 8,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] F3 = 8,

        /// <summary> Musical loudness. </summary>
        MaxLoudness = 9,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Ppp = 1,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Pp = 2,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] P = 3,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] F = 7,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Ff = 8,

        /// <summary> Musical loudness. </summary>
        [UsedImplicitly] Fff = 9
    }
}
