// <copyright file="MusicalNoteAlter.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Notation
{
    /// <summary> Note Number. </summary>
    public enum MusicalNoteAlter {
        /// <summary> Note alter. </summary>
        [UsedImplicitly]
        Natural = 0,

        /// <summary> Note alter. </summary>
        [UsedImplicitly]
        DoubleFlat = -2,

        /// <summary> Note alter. </summary>
        [UsedImplicitly]
        Flat = -1,

        /// <summary> Note alter. </summary>
        [UsedImplicitly]
        Sharp = 1,

        /// <summary> Note alter. </summary>
        [UsedImplicitly]
        DoubleSharp = 2
    }
}