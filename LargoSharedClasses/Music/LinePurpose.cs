// <copyright file="LinePurpose.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Outline of all musical properties used to evaluate. </summary>
    public enum LinePurpose {
        /// <summary> Line purpose. Line has no tones. </summary>
        None = 0,

        /// <summary> Line purpose. Line is filled with tones influencing sound.  </summary>
        Fixed = 1,

        /// <summary> Line purpose. Line tones are expected to be composed. </summary>
        Composed = 2,

        /// <summary> Line purpose. Line is filled with tones, but these are mute now. </summary>
        Mute = 3,

        /// <summary> Line purpose. Line is filled with tones that are considered to be no influence. </summary>
        [UsedImplicitly] Free = 4
    }
}
