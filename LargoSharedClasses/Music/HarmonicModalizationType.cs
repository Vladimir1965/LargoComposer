// <copyright file="HarmonicModalizationType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    /// <summary> Type of harmonic modalization. </summary>
    public enum HarmonicModalizationType {
        /// <summary> Modalization type - No modality, full chromatic. </summary>
        Chromatic = 0,

        /// <summary> Modalization type - Modality from structures in the bar after it disappears. </summary>
        Consecutive = 1,

        /// <summary> Modalization type - Forced from above. </summary>
        Forced = 2,
    }
}
