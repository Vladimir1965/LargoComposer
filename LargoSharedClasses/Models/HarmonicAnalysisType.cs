// <copyright file="HarmonicAnalysisType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Models
{
    /// <summary> Type of harmonic analysis. </summary>
    public enum HarmonicAnalysisType {
        /// <summary> Harmonic Analyze Type. 
        /// </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Harmonic Analyze Type. 
        /// </summary>
        [UsedImplicitly] BarDivision = 1,

        /// <summary> Harmonic Analyze Type. 
        /// </summary>
        [UsedImplicitly] HalfBarDivision = 2,

        /// <summary> Harmonic Analyze Type. 
        /// </summary>
        DivisionByTicks = 3
    }
}
