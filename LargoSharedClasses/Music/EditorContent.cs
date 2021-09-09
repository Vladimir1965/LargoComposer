// <copyright file="EditorContent.cs" company="Traced-Ideas, Czech republic">
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
    /// Editor Content.
    /// </summary>
    public enum EditorContent {
        /// <summary> No content. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Raster content. </summary>
        Raster = 1,

        /// <summary> Cell content. </summary>
        Cell = 2,

        /// <summary> Instrument And Loudness. </summary>
        InstrumentAndLoudness = 3,

        /// <summary> Octave And Band. </summary>
        OctaveAndBand = 4,

        /// <summary> Tone And Beat Level. </summary>
        ToneAndBeatLevel = 5,

        /// <summary> Melodic Function And Shape. </summary>
        MelodicFunctionAndShape = 6,

        /// <summary> Rhythmic Structure. </summary>
        RhythmicStructure = 7,

        /// <summary> Melodic Structure. </summary>
        MelodicStructure = 8,

        /// <summary> Rhythmic motive. </summary>
        RhythmicMotive = 9,

        /// <summary> Melodic motive. </summary>
        MelodicMotive = 10
    }
}
