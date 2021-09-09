// <copyright file="MelodicShape.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Melody
{
    /// <summary> Type of melodic part. </summary>
    public enum MelodicShape {
        /// <summary> Type of part. </summary>
        None = 0,

        /// <summary> Type of part. </summary>
        MinimumMotion = 1,

        /// <summary> Type of part. </summary>
        Scales = 2,

        /// <summary> Type of part. </summary>
        Sinusoids = 3,

        /// <summary> Type of part. </summary>
        BreakingLine = 4,

        /// <summary> Type of part. </summary>
        Original = 5
    }
}
