// <copyright file="RhythmicDegree.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Rhythm
{
    /// <summary> Degree of rhythmical systems. </summary>
    public enum RhythmicDegree {
        /// <summary> Degree of system. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Degree of system. </summary>
        Shape = 2,

        /// <summary> Degree of system. </summary>
        Structure = 3
    }
}
