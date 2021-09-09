// <copyright file="InstrumentGenus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Instrument Genus.
    /// </summary>
    public enum InstrumentGenus
    {
        /// <summary>
        /// Instrument Genus.
        /// </summary>
        None = 0,

        /// <summary>
        /// Instrument Genus.
        /// </summary>
        [UsedImplicitly] Rhythmical = 1,

        /// <summary>
        /// Instrument Genus.
        /// </summary>
        Melodical = 2
    }
}