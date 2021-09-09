// <copyright file="MidiChannel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> MIDI channel. Channel 9 is for drums, channel 15 is for internal use (empty notes). </summary>
    public enum MidiChannel {
        /// <summary> Number of MIDI drum channel. </summary>
        /// <summary>General MIDI percussion channel.</summary>
        DrumChannel = 9, //// Channel 10 (1-based) is reserved for the percussion map

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C00 = 0,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C01 = 1,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C02 = 2,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C03 = 3,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C04 = 4,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C05 = 5,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C06 = 6,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C07 = 7,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C08 = 8,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C10 = 10,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C11 = 11,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C12 = 12,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C13 = 13,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C14 = 14,

        /// <summary> MIDI channel. </summary>
        [UsedImplicitly] C15 = 15,

        /// <summary>
        /// Unknown channel.
        /// </summary>
        Unknown = 255
    }
}
