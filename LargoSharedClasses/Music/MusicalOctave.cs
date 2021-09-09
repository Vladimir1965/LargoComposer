// <copyright file="MusicalOctave.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Musical octave. </summary>
    public enum MusicalOctave {
        /// <summary> Musical octave. </summary>
        FiveLine = 9,

        /// <summary> Musical octave. </summary>
        [UsedImplicitly] FourLine = 8,

        /// <summary> Musical octave. </summary>
        [UsedImplicitly] ThreeLine = 7,

        /// <summary> Musical octave. </summary>
        TwoLine = 6,

        /// <summary> Musical octave. </summary>
        OneLine = 5,

        /// <summary> Musical octave. </summary>
        Small = 4,

        /// <summary> Musical octave. </summary>
        [UsedImplicitly] Great = 3,

        /// <summary> Musical octave. </summary>
        Contra = 2,

        /// <summary> Musical octave. </summary>
        SubContra = 1,

        /// <summary> Musical octave. </summary>
        None = 0
    }
}
