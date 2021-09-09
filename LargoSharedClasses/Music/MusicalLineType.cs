// <copyright file="MusicalLineType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> General type of line. </summary>
    public enum MusicalLineType {
        /// <summary> Type of line. </summary>
        None = 0,

        /// <summary> Type of line. </summary>
        [UsedImplicitly] Empty = 1,

        /// <summary> Type of line. </summary>
        Rhythmic = 2,

        /// <summary> Type of line. </summary>
        Melodic = 3,

        /// <summary> Type of line. </summary>
        Harmonic = 4
    }
}
