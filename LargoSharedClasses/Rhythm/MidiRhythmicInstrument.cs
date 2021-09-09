// <copyright file="MidiRhythmicInstrument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Rhythm
{
    /// <summary> Rhythmical instruments - General MIDI Percussion Patch Map. </summary>
    [Serializable]
    public enum MidiRhythmicInstrument {
        /// <summary>Rhythmic instrument.</summary>
        [UsedImplicitly]
        None = 0, //// Any

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly] ScratchPush = 29,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly] ScratchPull = 30,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        Stick = 31,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        LowMetronome = 32,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        MidMetronome = 33,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        HighMetronome = 34,

        /// <summary>Bass Drum (Acoustic Bass Drum).</summary>
        [UsedImplicitly]
        BassDrum = 35,

        /// <summary>Bass Drum 1.</summary>
        [UsedImplicitly]
        BassDrum1 = 36,

        /// <summary>Side Stick.</summary>
        [UsedImplicitly]
        SideStick = 37,

        /// <summary>Acoustic Snare.</summary>
        [UsedImplicitly]
        AcousticSnare = 38,

        /// <summary>Hand Clap.</summary>
        [UsedImplicitly]
        Handclap = 39,

        /// <summary>Electric Snare.</summary>
        [UsedImplicitly]
        ElectricSnare = 40,

        /// <summary>Low Floor Tom.</summary>
        [UsedImplicitly]
        LowFloorTom = 41,

        /// <summary>Closed Hi Hat.</summary>
        [UsedImplicitly]
        ClosedHighHat = 42,

        /// <summary>High Floor Tom.</summary>
        [UsedImplicitly]
        HighFloorTom = 43,

        /// <summary>Pedal Hi Hat.</summary>
        [UsedImplicitly]
        PedalHighHat = 44,

        /// <summary>Low Tom drum.</summary>
        [UsedImplicitly]
        LowTom = 45,

        /// <summary>Open Hi Hat.</summary>
        [UsedImplicitly]
        OpenHighHat = 46,

        /// <summary>Low Mid Tom.</summary>
        [UsedImplicitly]
        LowMidTom = 47,

        /// <summary>Hi Mid Tom.</summary>
        [UsedImplicitly]
        HighMidTom = 48,

        /// <summary>Crash Cymbal 1.</summary>
        [UsedImplicitly]
        CrashCymbal1 = 49,

        /// <summary>High Tom drum.</summary>
        [UsedImplicitly]
        HighTom = 50,

        /// <summary>Ride Cymbal.</summary>
        [UsedImplicitly]
        RideCymbal = 51,

        /// <summary>Chinese Cymbal.</summary>
        [UsedImplicitly]
        ChineseCymbal = 52,

        /// <summary>Ride Bell.</summary>
        [UsedImplicitly]
        RideBell = 53,

        /// <summary>Circular Tambourine.</summary>
        [UsedImplicitly]
        Tambourine = 54,

        /// <summary>Splash Cymbal.</summary>
        [UsedImplicitly]
        SplashCymbal = 55,

        /// <summary>Idiophone Cowbell.</summary>
        [UsedImplicitly]
        Cowbell = 56,

        /// <summary>Crash Cymbal 2.</summary>
        [UsedImplicitly]
        CrashCymbal2 = 57,

        /// <summary>Resonating Vibraslap.</summary>
        [UsedImplicitly]
        Vibraslap = 58,

        /// <summary>Ride Cymbal 2.</summary>
        [UsedImplicitly]
        RideCymbal2 = 59,

        /// <summary>Cuban High Bongo.</summary>
        [UsedImplicitly]
        HighBongo = 60,

        /// <summary>Cuban Low Bongo.</summary>
        [UsedImplicitly]
        LowBongo = 61,

        /// <summary>Mute Hi Conga.</summary>
        [UsedImplicitly]
        MuteHighConga = 62,

        /// <summary>Open Hi Conga.</summary>
        [UsedImplicitly]
        OpenHighConga = 63,

        /// <summary>Low Conga.</summary>
        [UsedImplicitly]
        LowConga = 64,

        /// <summary>High Timbale.</summary>
        [UsedImplicitly]
        HighTimbale = 65,

        /// <summary>Low Timbale.</summary>
        [UsedImplicitly]
        LowTimbale = 66,

        /// <summary>High Agogo.</summary>
        [UsedImplicitly]
        HighAgogo = 67,

        /// <summary>Low Agogo.</summary>
        [UsedImplicitly]
        LowAgogo = 68,

        /// <summary>African Cabasa.</summary>
        [UsedImplicitly]
        Cabasa = 69,

        /// <summary>Latin American Maracas.</summary>
        [UsedImplicitly]
        Maracas = 70,

        /// <summary>Short Whistle.</summary>
        [UsedImplicitly]
        ShortWhistle = 71,

        /// <summary>Long Whistle (Decimal Whistle).</summary>
        [UsedImplicitly]
        LongWhistle = 72,

        /// <summary>Short Guiro.</summary>
        [UsedImplicitly]
        ShortGuiro = 73,

        /// <summary>Long Guiro (Decimal Guiro).</summary>
        [UsedImplicitly]
        LongGuiro = 74,

        /// <summary>Idiophone Claves.</summary>
        [UsedImplicitly]
        Claves = 75,

        /// <summary>Hi Wood Block.</summary>
        [UsedImplicitly]
        HighWoodblock = 76,

        /// <summary>Low Wood Block.</summary>
        [UsedImplicitly]
        LowWoodblock = 77,

        /// <summary>Mute Cuica.</summary>
        [UsedImplicitly]
        MuteCuica = 78,

        /// <summary>Open Cuica.</summary>
        [UsedImplicitly]
        OpenCuica = 79,

        /// <summary>Mute Triangle.</summary>
        [UsedImplicitly]
        MuteTriangle = 80,

        /// <summary>Open Triangle.</summary>
        [UsedImplicitly]
        OpenTriangle = 81,

        /// <summary> Castanet percussion. </summary>
        [UsedImplicitly]
        Castanets = 85,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        HighTaiko = 86,

        /// <summary> Rhythmic instrument. </summary>
        [UsedImplicitly]
        LowTaiko = 87
    }
}
