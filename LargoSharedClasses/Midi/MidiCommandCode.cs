// <copyright file="MidiCommandCode.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// Midi Message. MIDI command codes. 
    /// </summary>
    public enum MidiCommandCode {
        /// <summary>Empty midi command. </summary>
        [UsedImplicitly] None = 0x00,

        /// <summary>Note Off. Key Release. </summary>
        VoiceNoteOff = 0x80,

        /// <summary>Note On. Key Press. </summary>
        VoiceNoteOn = 0x90,

        /// <summary>Key After-touch.</summary>
        [UsedImplicitly] KeyAftertouch = 0xA0,

        /// <summary>Control change.</summary>
        ControlChange = 0xB0,

        /// <summary>Patch change.</summary>
        [UsedImplicitly] PatchChange = 0xC0,

        /// <summary>Channel after-touch.</summary>
        [UsedImplicitly] ChannelAftertouch = 0xD0,

        /// <summary>Pitch wheel change.</summary>
        [UsedImplicitly] PitchWheelChange = 0xE0,

        /// <summary>System exclusive message.</summary>
        SystemExclusive = 0xF0,

        /// <summary>End Of System Exclusive (comes at end of a System Exclusive message).</summary>
        EndOfSystemExclusive = 0xF7,

        /// <summary>Timing clock (used when synchronization is required).</summary>
        [UsedImplicitly] TimingClock = 0xF8,

        /// <summary>Start sequence.</summary>
        [UsedImplicitly] StartSequence = 0xFA,

        /// <summary>Continue sequence.</summary>
        [UsedImplicitly] ContinueSequence = 0xFB,

        /// <summary>Stop sequence.</summary>
        [UsedImplicitly] StopSequence = 0xFC,

        /// <summary>Auto Sensing.</summary>
        [UsedImplicitly] AutoSensing = 0xFE,

        /// <summary>Meta event.</summary>
        MetaEvent = 0xFF
    }
}
