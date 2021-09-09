// <copyright file="MusicalChangeType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Models
{
    /// <summary> Types of changes possible in music block. </summary>
    public enum MusicalChangeType {
        /// <summary> Musical change type. </summary>
        None = 0,

        /// <summary> Musical change type. </summary>
        Harmonic = 1,

        /// <summary> Musical change type. </summary>
        Melodic = 2,

        /// <summary> Musical change type. </summary>
        Rhythmic = 3,

        /// <summary> Musical change type. </summary>
        Instrument = 4,

        /// <summary> Musical change type. </summary>
        Octave = 5,

        /// <summary> Musical change type. </summary>
        Loudness = 6,

        /// <summary> Musical change type. </summary>
        Staff = 7,

        /// <summary> Musical change type. </summary>
        Tempo = 8,

        /// <summary> Musical change type. </summary>
        Energy = 9,

        /// <summary> Musical change type. </summary>
        Tonality = 10,

        /// <summary> Musical change type. </summary>
        All = 11
    }
}
