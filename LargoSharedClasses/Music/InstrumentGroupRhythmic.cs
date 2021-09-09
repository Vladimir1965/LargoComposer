// <copyright file="InstrumentGroupRhythmic.cs" company="Traced-Ideas, Czech republic">
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
    public enum InstrumentGroupRhythmic {
        /// <summary> Type of voice. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Drums = 9,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cymbals = 10,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Snare = 11,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Toms = 12,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Hihat = 13,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Triangle = 14,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Woodblock = 15,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Conga = 16,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Bonga = 17,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Guiro = 18,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cuica = 19,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Timbale = 20,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Agogo = 21,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Taiko = 22,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Metronome = 23,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Others = 24
    }
}
