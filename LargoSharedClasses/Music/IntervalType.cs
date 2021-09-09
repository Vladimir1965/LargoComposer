// <copyright file="IntervalType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Interval type. </summary>
    public enum IntervalType {
            /// <summary> Interval type. </summary>
            [UsedImplicitly] Unison = 0,

            /// <summary> Interval type. </summary>
            Halftone = 1,  ////  Semitone

            /// <summary> Interval type. </summary>
            Second = 2,  //// WholeTone

            /// <summary> Interval type. </summary>
            MinorThird = 3,

            /// <summary> Interval type. </summary>
            MajorThird = 4,

            /// <summary> Interval type. </summary>
            Fourth = 5,

            /// <summary> Interval type. </summary>
            [UsedImplicitly] Tritone = 6,

            /// <summary> Interval type. </summary>
            Fifth = 7,

            /// <summary> Interval type. </summary>
            [UsedImplicitly] MinorSixth = 8,

            /// <summary> Interval type. </summary>
            [UsedImplicitly] MajorSixth = 9,

            /// <summary> Interval type. </summary>
            [UsedImplicitly] Seventh = 10,

            /// <summary> Interval type. </summary>
            [UsedImplicitly] AugmentedSeventh = 11
    }
}
