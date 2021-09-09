// <copyright file="MidiController.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// MidiController enumeration.
    /// List of defined controllers. All descriptions come from MidiRef4.
    /// </summary>
    [Serializable]
    public enum MidiController {
        #region Controller Operations
        /// <summary>Switches between groups of sounds when more than 128 programs are in use.</summary>
        [UsedImplicitly] BankSelectCourse = 0,

        /// <summary> Modulation. Sets the modulation wheel to a particular value.</summary>
        [UsedImplicitly] ModulationWheelCourse = 1, //// ModulationWheel

        /// <summary>Often used to control after-touch.</summary>
        [UsedImplicitly] BreathControllerCourse = 2,

        /// <summary>Often used to control after-touch.</summary>
        [UsedImplicitly] FootPedalCourse = 4,

        /// <summary>The rate at which the pitch slides between two notes.</summary>
        [UsedImplicitly]
        PortamentoTimeCourse = 5,

        /// <summary>Data entry course.</summary>
        [UsedImplicitly]
        DataEntryCourse = 6, //// DataEntryMSB

        /// <summary>Main volume. Volume level for a given channel.</summary>
        [UsedImplicitly]
        VolumeCourse = 7, //// Volume

        /// <summary>Controls stereo-balance.</summary>
        [UsedImplicitly]
        BalanceCourse = 8,

        /// <summary> Pan. Where the stereo sound should be placed within the sound field.</summary>
        [UsedImplicitly]
        PanPositionCourse = 10, //// Pan

        /// <summary> Expression. Percentage of volume.</summary>
        [UsedImplicitly]
        ExpressionCourse = 11,  //// Expression

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        EffectControl1Course = 12,

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        EffectControl2Course = 13,

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        GeneralPurposeSlider1 = 16,

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        GeneralPurposeSlider2 = 17,

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        GeneralPurposeSlider3 = 18,

        /// <summary>Various effects.</summary>
        [UsedImplicitly]
        GeneralPurposeSlider4 = 19,

        /// <summary>Bank Select (LSB). Switches between groups of sounds when more than 128 programs are in use. Changes to a different bank of instruments if available. </summary>
        [UsedImplicitly]
        BankSelectFine = 32,

        /// <summary>Modulation Wheel (LSB). Sets the modulation wheel to a particular value. Sends a command to add modulation (vibrato) to the current sound. </summary>
        [UsedImplicitly]
        ModulationWheelFine = 33,

        /// <summary>Breath Controller (LSB). Often used to control after-touch. Much like modulation, but it is triggered by a device that you blow into. 
        /// The device measures the pressure of the wind, and adds modulation accordingly. </summary>
        [UsedImplicitly]
        BreathControllerFine = 34,

        /// <summary>Foot Controller (LSB). Often used to control after-touch.  Sends a control from a foot pedal device. 
        /// The effect is usually programmable by the user on a particular synth. </summary>
        [UsedImplicitly]
        FootPedalFine = 36,

        /// <summary>Pitch sliding Time (LSB). The rate at which the pitch slides between two notes. Used to change the time of 
        /// on the current channel (the time it takes to slide in pitch from one note to the next). </summary>
        [UsedImplicitly]
        PortamentoTimeFine = 37,

        /// <summary>Data Entry (LSB). Various effects. Common on many MIDI devices. The effect varies from unit to unit. </summary>
        [UsedImplicitly]
        DataEntryFine = 38, //// DataEntryLSB

        /// <summary>Channel Volume (LSB). Controls the main volume of a channel. Volume level for a given channel.</summary>
        [UsedImplicitly]
        VolumeFine = 39,

        /// <summary>Balance (LSB). Controls the stereo-balance.</summary>
        [UsedImplicitly]
        BalanceFine = 40,

        /// <summary>Pan (LSB). Where the stereo sound should be placed within the sound field.</summary>
        [UsedImplicitly]
        PanPositionFine = 42,

        /// <summary>Expression (LSB). Percentage of volume.  Like the Channel Volume controller, 
        /// however the Controller 7 is used to set the overall volume on a particular channel, 
        /// while Controller 11 is used to adjust the volume dynamics within that channel to a percentage of the overall volume. </summary>
        [UsedImplicitly]
        ExpressionFine = 43,

        /// <summary>Effect Control 1 (LSB). Controls some aspect of the MIDI unit's effects. </summary>
        [UsedImplicitly]
        EffectControl1Fine = 44,

        /// <summary>Effect Control 2 (LSB). Controls an alternate aspect of the unit's effects. </summary>
        [UsedImplicitly]
        EffectControl2Fine = 45,

        /// <summary>General Purpose Controller. Programmable on most units. </summary>
        [UsedImplicitly]
        GeneralPurpose1 = 48,

        /// <summary>General Purpose Controller. Programmable on most units. </summary>
        [UsedImplicitly]
        GeneralPurpose2 = 49,
        
        /// <summary>General Purpose Controller. Programmable on most units. </summary>
        [UsedImplicitly]
        GeneralPurpose3 = 50,
        
        /// <summary>General Purpose Controller. Programmable on most units. </summary>
        [UsedImplicitly]
        GeneralPurpose4 = 51,
        
        /// <summary>Sustain. Lengthens release time of playing notes. Sustains any notes that are playing (on/off). </summary>
        [UsedImplicitly]
        HoldPedalOnOff = 64,    //// SustainPedal

        /// <summary>The rate at which the pitch slides between two notes. Changes the state of the pitch sliding to on or off. </summary>
        [UsedImplicitly]
        PortamentoOnOff = 65,

        /// <summary>Sustains notes that are already on. Controls the sostenuto of the current instrument. Like the Sustain Pedal, but only sustains notes that are already ON when the message is sent (on/off). </summary>
        [UsedImplicitly]
        SustenutoPedalOnOff = 66,

        /// <summary>Softens volume of any notes played. Lowers the volume the current instrument (on/off). </summary>
        [UsedImplicitly]
        SoftPedalOnOff = 67,

        /// <summary>Legato effect between notes. Applies or removes legato (on/off). </summary>
        [UsedImplicitly]
        LegatoPedalOnOff = 68,

        /// <summary>Lengthens release time of playing notes. Lengthens the time that it takes for a note to fade-out when it's released (on/off). </summary>
        [UsedImplicitly]
        Hold2PedalOnOff = 69,

        /// <summary>Programmable, the default is 'Sound Variation'. Sound variation.</summary>
        [UsedImplicitly]
        SoundVariation = 70,

        /// <summary>Programmable, the default is 'Timbre'. Controls envelope levels.</summary>
        [UsedImplicitly]
        SoundTimbre = 71,

        /// <summary>Programmable, the default is 'Release Time'. Controls envelope release times.</summary>
        [UsedImplicitly]
        SoundReleaseTime = 72,

        /// <summary>Programmable, the default is 'Attack Time'. Controls envelope attack time.</summary>
        [UsedImplicitly]
        SoundAttackTime = 73,

        /// <summary>Programmable, the default is 'Brightness'. Controls filter's cutoff frequency.</summary>
        [UsedImplicitly]
        SoundBrightness = 74,

        /// <summary>Various Controls. Programmable, no default. </summary>
        [UsedImplicitly]
        SoundControl6 = 75,

        /// <summary>Various Controls. Programmable, no default. </summary>
        [UsedImplicitly]
        SoundControl7 = 76,

        /// <summary>Various Controls. Programmable, no default. </summary>
        [UsedImplicitly]
        SoundControl8 = 77,

        /// <summary>Various Controls. Programmable, no default. </summary>
        [UsedImplicitly]
        SoundControl9 = 78,

        /// <summary>Various Controls. Programmable, no default. </summary>
        [UsedImplicitly]
        SoundControl10 = 79,

        /// <summary>Various Controls. Programmable on most units (on/off). </summary>
        [UsedImplicitly]
        GeneralPurposeButton1OnOff = 80,

        /// <summary>Various Controls. Programmable on most units (on/off). </summary>
        [UsedImplicitly]
        GeneralPurposeButton2OnOff = 81,

        /// <summary>Various Controls. Programmable on most units (on/off). </summary>
        [UsedImplicitly]
        GeneralPurposeButton3OnOff = 82,

        /// <summary>Various Controls. Programmable on most units (on/off). </summary>
        [UsedImplicitly]
        GeneralPurposeButton4OnOff = 83,

        /// <summary>Changes the pitch sliding if available..</summary>
        [UsedImplicitly]
        PortamentoControl = 84,
     
        /// <summary>Controls level of reverb effect  - 0 (no effect) to 127.</summary> 
        [UsedImplicitly]
        EffectsLevel = 91,  //// ReverbLevel

        /// <summary>Controls level of tremolo effect - 0 (no effect) to 127.</summary>
        [UsedImplicitly]
        TremoloLevel = 92,

        /// <summary>Controls level of chorus effect - 0 (no effect) to 127.</summary>
        [UsedImplicitly]
        ChorusLevel = 93,

        /// <summary>Controls level for celeste (detune) effect - 0 (no effect) to 127.</summary>
        [UsedImplicitly]
        CelesteLevel = 94,

        /// <summary>Controls level of phase effect - 0 (no effect) to 127. </summary>
        [UsedImplicitly]
        PhaserLevel = 95,

        /// <summary>Causes data button's value to increment.</summary>
        [UsedImplicitly]
        DataButtonIncrement = 96,

        /// <summary>Causes data button's value to decrement.</summary>
        [UsedImplicitly]
        DataButtonDecrement = 97,

        /// <summary>Controls which parameter the button and data entry controls affect.</summary>
        [UsedImplicitly]
        NonRegisteredParameterFine = 98, //// NonRegisteredParameterLSB

        /// <summary>Controls which parameter the button and data entry controls affect.</summary>
        [UsedImplicitly]
        NonRegisteredParameterCourse = 99, //// NonRegisteredParameterMSB

        /// <summary>Controls which parameter the button and data entry controls affect.</summary>
        [UsedImplicitly]
        RegisteredParameterFine = 100, //// RegisteredParameterNumberLSB

        /// <summary>Controls which parameter the button and data entry controls affect.</summary>
        [UsedImplicitly]
        RegisteredParameterCourse = 101, //// RegisteredParameterNumberMSB

        /// <summary>Mutes all sounding notes, mutes all sound. </summary>
        [UsedImplicitly]
        AllSoundOff = 120,

        /// <summary>Reset all controllers. Resets controllers to default states. Turns off controllers or sets them to default, usually 0. </summary>
        [UsedImplicitly]
        AllControllersOff = 121,

        /// <summary>Turns on or off local keyboard. If set to off, a keyboard won't generate sound internally. </summary>
        [UsedImplicitly]
        LocalKeyboardOnOff = 122,

        /// <summary>Turns all notes off. Mutes all sounding notes.</summary>
        AllNotesOff = 123,

        /// <summary>Turns Omni mode off.</summary>
        [UsedImplicitly]
        OmniModeOff = 124,

        /// <summary>Turns Omni mode on.</summary>
        [UsedImplicitly]
        OmniModeOn = 125,

        /// <summary>Enables Monophonic operation, turns Monophonic operation on. </summary>
        [UsedImplicitly]
        MonoOperation = 126,

        /// <summary>Enables Polyphonic operation, turns Polyphonic operation on. </summary>
        [UsedImplicitly]
        PolyOperation = 127
        #endregion
    }
}
