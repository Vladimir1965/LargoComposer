// <copyright file="HarmonicClusterExtent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Cluster extent. </summary>
    public enum HarmonicClusterExtent {
        /// <summary> Cluster extent. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Cluster extent. </summary>
        TightExtent = 1,

        /// <summary> Cluster extent. </summary>
        MiddleExtent = 2,

        /// <summary> Cluster extent. </summary>
        WideExtent = 3
    }
}
