// <copyright file="MidiMetaMessageType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// Represents MetaMessage types.
    /// </summary>
    [UsedImplicitly]
    public enum MidiMetaMessageType {
        /// <summary>
        /// Represents sequencer number type.
        /// </summary>
        [UsedImplicitly] SequenceNumber,

        /// <summary>
        /// Represents the text type.
        /// </summary>
        [UsedImplicitly] Text,

        /// <summary>
        /// Represents the copyright type.
        /// </summary>
        [UsedImplicitly] Copyright,

        /// <summary>
        /// Represents the track name type.
        /// </summary>
        [UsedImplicitly] TrackName,

        /// <summary>
        /// Represents the instrument name type.
        /// </summary>
        [UsedImplicitly] InstrumentName,

        /// <summary>
        /// Represents the lyric type.
        /// </summary>
        [UsedImplicitly] Lyric,

        /// <summary>
        /// Represents the marker type.
        /// </summary>
        [UsedImplicitly] Marker,

        /// <summary>
        /// Represents the cue point type.
        /// </summary>
        [UsedImplicitly] CuePoint,

        /// <summary>
        /// Represents the program name type.
        /// </summary>
        [UsedImplicitly] ProgramName,

        /// <summary>
        /// Represents the device name type.
        /// </summary>
        [UsedImplicitly] DeviceName,

        /// <summary>
        /// Represents then end of track type.
        /// </summary>
        [UsedImplicitly] MetaEndOfTrack = 0x2F,

        /// <summary>
        /// Represents the tempo type.
        /// </summary>
        [UsedImplicitly] Tempo = 0x51,

        /// <summary>
        /// Represents the time code offset type.
        /// </summary>
        [UsedImplicitly] SmpteOffset = 0x54,

        /// <summary>
        /// Represents the time signature type.
        /// </summary>
        [UsedImplicitly] TimeSignature = 0x58,

        /// <summary>
        /// Represents the key signature type.
        /// </summary>
        [UsedImplicitly] KeySignature,

        /// <summary>
        /// Represents the proprietary event type.
        /// </summary>
        [UsedImplicitly] ProprietaryEvent = 0x7F
    }
}
