// <copyright file="LineRhythm.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Templates
{
    /// <summary>
    /// Line Rhythm.
    /// </summary>
    public enum LineRhythm
    {
        /// <summary>
        /// Line Rhythm.
        /// </summary>
        [UsedImplicitly] None = 0,

        /// <summary>
        /// Line Rhythm.
        /// </summary>
        [UsedImplicitly] SimpleOneTone = 1,

        /// <summary>
        /// Line Rhythm.
        /// </summary>
        HarmonicShape = 2,

        /// <summary>
        /// Line Rhythm.
        /// </summary>
        HarmonicStructure = 3
    }
}