// <copyright file="InspectorType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Abstract
{
    using JetBrains.Annotations;

    /// <summary>
    /// Editor Content.
    /// </summary>
    public enum InspectorType
    {
        /// <summary> No content. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Number of tones. </summary>
        Bar = 1,

        /// <summary> Number of tones. </summary>
        Line = 2,

        /// <summary> Number of tones. </summary>
        Element = 3,

        /// <summary> Rhythmic motives. </summary>
        RhythmicMotive = 4,

        /// <summary> Melodic motives. </summary>
        MelodicMotive = 5,

        /// <summary> Number of tones. </summary>
        Tones = 6
    }
}
