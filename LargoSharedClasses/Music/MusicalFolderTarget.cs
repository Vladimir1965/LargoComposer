// <copyright file="MusicalFolderTarget.cs" company="Traced-Ideas, Czech republic">
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
    /// Midi File Type.
    /// </summary>
    public enum MusicalFolderTarget {
        /// <summary> Musical Folder. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Musical Folder. </summary>
        [UsedImplicitly] Original = 1,

        /// <summary> Musical Folder. </summary>
        Generated = 2
    }
}
