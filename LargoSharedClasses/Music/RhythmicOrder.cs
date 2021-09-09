// <copyright file="RhythmicOrder.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Orders of rhythmical systems. </summary>
    public enum RhythmicOrder {
        /// <summary> Order of system. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Order of system. </summary>
        R24 = 24
    }
}
