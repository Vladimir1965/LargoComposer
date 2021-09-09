// <copyright file="MidiMetaType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// MIDI MetaEvent Type.
    /// </summary>
    public enum MidiMetaType {
        /// <summary>Track sequence number.</summary>
        TrackSequenceNumber = 0x00,

        /// <summary>Text event.</summary>
        TextEvent = 0x01,

        /// <summary>Copyright meta event.</summary>
        CopyrightNotice = 0x02,

        /// <summary>Sequence track name.</summary>
        SoundtrackName = 0x03,

        /// <summary>Track instrument name.</summary>
        TrackInstrumentName = 0x04,

        /// <summary>Lyric in MIDI.</summary>
        Lyric = 0x05,

        /// <summary>Midi Marker.</summary>
        Marker = 0x06,

        /// <summary>Cue point.</summary>
        CuePoint = 0x07,

        /// <summary>Program (patch) name.</summary>
        ProgramName = 0x08,

        /// <summary>Device (port) name.</summary>
        DeviceName = 0x09,

        /// <summary>MIDI Channel (not official?).</summary>
        MidiChannelPrefix = 0x20,

        /// <summary>MIDI Port (not official?).</summary>
        MidiPort = 0x21,

        /// <summary>End track.</summary>
        MetaEndOfTrack = 0x2F,

        /// <summary>Set tempo.</summary>
        SetTempo = 0x51,

        /// <summary>Time Code offset.</summary>
        TimeCodeOffset = 0x54,

        /// <summary>Time signature.</summary>
        TimeSignature = 0x58,

        /// <summary>Key signature.</summary>
        KeySignature = 0x59,

        /// <summary>Sequencer specific.</summary>
        SequencerSpecific = 0x7F
    }
}
