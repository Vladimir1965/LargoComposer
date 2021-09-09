// <copyright file="MusicalValue.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Value of musical properties. </summary>
    public enum MusicalValue {
        /// <summary> Value of property. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Value of property. </summary>
        VeryLow = 1,

        /// <summary> Value of property. </summary>
        Lower = 2,

        /// <summary> Value of property. </summary>
        NotHigh = 3,

        /// <summary> Value of property. </summary>
        Middle = 4,

        /// <summary> Value of property. </summary>
        NotLow = 5,

        /// <summary> Value of property. </summary>
        Higher = 6,

        /// <summary> Value of property. </summary>
        VeryHigh = 7
    }
}
