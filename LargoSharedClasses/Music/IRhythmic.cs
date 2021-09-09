// <copyright file="IRhythmic.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Rhythmic Interface.
    /// </summary>
    public interface IRhythmic {
        #region Properties
        /// <summary>
        /// Gets the rhythmic system.
        /// </summary>
        /// <value>
        /// The rhythmic system.
        /// </value>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        RhythmicSystem RhythmicSystem { get; }
        #endregion
    }
}
