// <copyright file="MidiRhythmicSection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Rhythm
{
    /// <summary> Rhythmical sections. </summary>
    [Serializable]
    public enum MidiRhythmicSection {
        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Drums = 0,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cymbals = 1,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Snare = 2,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Toms = 3,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Hihat = 4,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Triangle = 5,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Woodblock = 6,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Conga = 7,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Bonga = 8,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Guiro = 9,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cuica = 10,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Timbale = 11,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Agogo = 12,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Taiko = 13,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Metronome = 14,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Others = 15,

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly]
        None = 255
    }
}
