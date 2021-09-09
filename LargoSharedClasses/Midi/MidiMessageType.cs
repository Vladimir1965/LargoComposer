// <copyright file="MidiMessageType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// Defines constants representing MIDI message types.
    /// </summary>
    [UsedImplicitly]
    public enum MidiMessageType {
        /// <summary>
        /// Midi Message Type.
        /// </summary>
        [UsedImplicitly] Channel = 0,

        /// <summary>
        /// Midi Message Type.
        /// </summary>
        [UsedImplicitly] SystemExclusive = 1,

        /// <summary>
        /// Midi Message Type.
        /// </summary>
        [UsedImplicitly] SystemCommon = 2,

        /// <summary>
        /// Midi Message Type.
        /// </summary>
        [UsedImplicitly] SystemRealTime = 3,

        /// <summary>
        /// Midi Message Type.
        /// </summary>
        [UsedImplicitly] Meta = 4
    }
}
