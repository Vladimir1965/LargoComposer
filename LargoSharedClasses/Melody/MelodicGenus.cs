// <copyright file="MelodicGenus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Melody
{
    /// <summary> Class of melodic part. </summary>
    public enum MelodicGenus {
        /// <summary> Class of part. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Class of part. </summary>
        Melodic = 1,

        /// <summary> Class of part. </summary>
        Harmonic = 2
    }
}
