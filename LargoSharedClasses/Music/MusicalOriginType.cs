// <copyright file="MusicalOriginType.cs" company="Traced-Ideas, Czech republic">
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
    /// Musical Model Type.
    /// </summary>
    public enum MusicalOriginType {
        /// <summary> Musical Origin Type. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Musical Origin Type. </summary>
        Original = 1,

        /// <summary> Musical Origin Type. </summary>
        Generated = 3,
    }
}
