// <copyright file="RhythmicElement.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Rhythm
{
    /// <summary> Type of melodic part. </summary>
    public enum RhythmicElement {
        /// <summary> Type of part. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Type of part. </summary>
        ContinuePrevious = 0,

        /// <summary> Type of part. </summary>
        StartTone = 1,

        /// <summary> Type of part. </summary>
        StartRest = 2
    }
}
