// <copyright file="IHarmonic.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Harmony;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// Harmonic Interface.
    /// </summary>
    public interface IHarmonic {
        #region Properties
        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        HarmonicSystem HarmonicSystem { get; }

        /// <summary>
        /// Gets the formal energy.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        HarmonicBehavior FormalEnergy { get; }

        #endregion
    }
}
