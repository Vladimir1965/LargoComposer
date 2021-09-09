// <copyright file="InstrumentGroupMelodic.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Type of band. </summary>
    public enum InstrumentGroupMelodic {
        /// <summary> Type of voice. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Keyboards = 1,  //// Or Pianos + Organs

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Strings = 2,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Woodwind = 3,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Brass = 4,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Guitars = 5,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Vocal = 6,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Synthetic = 7,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] MelodicDrums = 8,
    }
}
