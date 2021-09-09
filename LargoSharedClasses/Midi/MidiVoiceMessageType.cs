// <copyright file="MidiVoiceMessageType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// Midi Voice Message Type.
    /// </summary>
    public enum MidiVoiceMessageType {
        /// <summary>Message type.</summary>
        [UsedImplicitly] None = 0,        
        
        /// <summary>Message type.</summary>
        VoiceNoteOff = 0x08,

        /// <summary>Message type.</summary>
        VoiceNoteOn = 0x09,

        /// <summary>Message type.</summary>
        PolyphonicKeyPressure = 0x0A, //// AFTERTOUCH

        /// <summary>Message type.</summary>
        ControllerChange = 0x0B,

        /// <summary>Message type.</summary>
        ProgramChange = 0x0C,

        /// <summary>Message type.</summary>
        ChannelKeyPressure = 0x0D,

        /// <summary>Message type.</summary>
        PitchBend = 0x0E //// PITCH WHEEL
    }
}
