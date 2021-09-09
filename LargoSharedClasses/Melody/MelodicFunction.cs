// <copyright file="MelodicFunction.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Melody
{
    /// <summary> Type of melodic part. </summary>
    public enum MelodicFunction {
        /// <summary> Type of part. </summary>
        None = 0,

        /// <summary> Type of part. </summary>
        MelodicMotion = 1,

        /// <summary> Type of part. </summary>
        MelodicFilling = 2,

        /// <summary> Type of part. </summary>
        HarmonicMotion = 3,

        /// <summary> Type of part. </summary>
        HarmonicFilling = 4,

        /// <summary> Type of part. </summary>
        HarmonicBass = 5,

        /// <summary> Type of part. </summary>
        MelodicBass = 6
    }
}
