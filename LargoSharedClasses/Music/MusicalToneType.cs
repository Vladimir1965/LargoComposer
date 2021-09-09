// <copyright file="MusicalToneType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    /// <summary>  Type of musical tone.  </summary>
    public enum MusicalToneType {
        /// <summary> Tone type. </summary>
        Empty = 0,

        /// <summary> Tone type. </summary>
        Rhythmic = 1,

        /// <summary> Tone type. </summary>
        Melodic = 2 // or Harmonic, Ornament,...?
    }
}
