// <copyright file="TimerMode.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Defines constants for the simple (multimedia) Timer's event types.
    /// </summary>
    public enum TimerMode {
        /// <summary>
        /// Timer event occurs once.
        /// </summary>
        [UsedImplicitly] OneShot,

        /// <summary>
        /// Timer event occurs periodically.
        /// </summary>
        Periodic
    }
}
