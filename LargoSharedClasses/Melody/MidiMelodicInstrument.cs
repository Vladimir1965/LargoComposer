// <copyright file="MidiMelodicInstrument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Melody
{
    /// <summary> Midi Melodic instruments - General MIDI Instrument Patch Map. </summary>
    [Serializable]
    public enum MidiMelodicInstrument {  //// GeneralMidiInstrument
        #region Pianos
        /// <summary>Acoustic Grand.</summary>
        [UsedImplicitly] AcousticGrandPiano = 0,

        /// <summary>Bright Acoustic.</summary>
        [UsedImplicitly] BrightAcousticPiano = 1,

        /// <summary>Electric Grand.</summary>
        [UsedImplicitly] ElectricGrandPiano = 2,

        /// <summary>Honky Tonk.</summary>
        [UsedImplicitly] HonkyTonkPiano = 3,

        /// <summary>Electric Piano 1.</summary>
        [UsedImplicitly] ElectricPiano1 = 4,

        /// <summary>Electric Piano 2.</summary>
        [UsedImplicitly] ElectricPiano2 = 5,

        /// <summary>Harpsichord keyboard.</summary>
        [UsedImplicitly] Harpsichord = 6,

        /// <summary>Clavichord (Clavinet) keyboard.</summary>
        [UsedImplicitly] Clavichord = 7,
        #endregion

        #region Chrome Percussion
        /// <summary>Idiophone Celesta.</summary>
        [UsedImplicitly] Celesta = 8,

        /// <summary>Glockenspiel steel bars.</summary>
        [UsedImplicitly] Glockenspiel = 9,

        /// <summary>Music Box.</summary>
        [UsedImplicitly] MusicBox = 10,

        /// <summary>Vibraphone aluminum bars.</summary>
        [UsedImplicitly] Vibraphone = 11,

        /// <summary>African Marimba.</summary>
        [UsedImplicitly] Marimba = 12,

        /// <summary>Xylophone hardwood bars .</summary>
        [UsedImplicitly] Xylophone = 13,

        /// <summary>Tubular Bells.</summary>
        [UsedImplicitly] TubularBells = 14,

        /// <summary>Appalachian dulcimer.</summary>
        [UsedImplicitly] Dulcimer = 15,

        #endregion

        #region Organ

        /// <summary>Drawbar Organ.</summary>
        [UsedImplicitly] DrawbarOrgan = 16,

        /// <summary>Percussive Organ.</summary>
        [UsedImplicitly] PercussiveOrgan = 17,

        /// <summary>Rock Organ.</summary>
        [UsedImplicitly] RockOrgan = 18,

        /// <summary>Church Organ.</summary>
        [UsedImplicitly] ChurchOrgan = 19,

        /// <summary>Reed Organ.</summary>
        [UsedImplicitly] ReedOrgan = 20,

        /// <summary>Box-shaped Accordian (Accordion).</summary>
        [UsedImplicitly] FrenchAccordian = 21,

        /// <summary>Reed wind harmonica.</summary>
        [UsedImplicitly] Harmonica = 22,

        /// <summary>Tango Accordian (Accordion).</summary>
        [UsedImplicitly] TangoAccordian = 23,
        #endregion

        #region Guitar
        /// <summary>Nylon Acoustic Guitar.</summary>
        [UsedImplicitly] NylonAcousticGuitar = 24,

        /// <summary>Steel Acoustic Guitar.</summary>
        [UsedImplicitly] SteelAcousticGuitar = 25,

        /// <summary>Jazz Electric Guitar.</summary>
        [UsedImplicitly] JazzElectricGuitar = 26,

        /// <summary>Clean Electric Guitar.</summary>
        [UsedImplicitly] CleanElectricGuitar = 27,

        /// <summary>Muted Electric Guitar.</summary>
        [UsedImplicitly] MutedElectricGuitar = 28,

        /// <summary>Overdriven Guitar.</summary>
        [UsedImplicitly] OverdrivenGuitar = 29,

        /// <summary>Distortion Guitar.</summary>
        [UsedImplicitly] DistortionGuitar = 30,

        /// <summary>Guitar Harmonics.</summary>
        [UsedImplicitly] GuitarHarmonics = 31,
        #endregion

        #region Bass
        /// <summary>Acoustic Bass.</summary>
        [UsedImplicitly] AcousticBass = 32,

        /// <summary>Finger Electric Bass.</summary>
        [UsedImplicitly] FingerElectricBass = 33,

        /// <summary>Pick Electric Bass.</summary>
        [UsedImplicitly] PickElectricBass = 34,

        /// <summary>Fretless Bass.</summary>
        [UsedImplicitly] FretlessBass = 35,

        /// <summary>Slap Bass 1.</summary>
        [UsedImplicitly] SlapBass1 = 36,

        /// <summary>Slap Bass 2.</summary>
        [UsedImplicitly] SlapBass2 = 37,

        /// <summary>Synth Bass 1.</summary>
        [UsedImplicitly] SynthBass1 = 38,

        /// <summary>Synth Bass 2.</summary>
        [UsedImplicitly] SynthBass2 = 39,
        #endregion

        #region Strings
        /// <summary>String Violin.</summary>
        [UsedImplicitly] Violin = 40,

        /// <summary>String Viola.</summary>
        [UsedImplicitly] Viola = 41,

        /// <summary>String Cello.</summary>
        [UsedImplicitly] Cello = 42,

        /// <summary>String Contrabass.</summary>
        [UsedImplicitly] Contrabass = 43,

        /// <summary>Tremolo Strings.</summary>
        [UsedImplicitly] TremoloStrings = 44,

        /// <summary>Pizzicato Strings.</summary>
        [UsedImplicitly] PizzicatoStrings = 45,

        /// <summary>Orchestral Strings (Harp).</summary>
        [UsedImplicitly] OrchestralStrings = 46,

        /// <summary>Orchestral Timpani.</summary>
        [UsedImplicitly] Timpani = 47,
        #endregion

        #region Ensemble
        /// <summary>String Ensemble 1.</summary>
        [UsedImplicitly] StringEnsemble1 = 48,

        /// <summary>String Ensemble 2 (Slow Strings).</summary>
        [UsedImplicitly] StringEnsemble2 = 49,

        /// <summary>Synth Strings 1.</summary>
        [UsedImplicitly] SynthStrings1 = 50,

        /// <summary>Synth Strings 2.</summary>
        [UsedImplicitly] SynthStrings2 = 51,

        /// <summary>Choir Aahs.</summary>
        [UsedImplicitly] ChoirAahs = 52,

        /// <summary>Voice Oohs.</summary>
        [UsedImplicitly] VoiceOohs = 53,

        /// <summary>Synth Voice (Synth Choir).</summary>
        [UsedImplicitly] SynthVoice = 54,

        /// <summary>Orchestra Hit.</summary>
        [UsedImplicitly] OrchestraHit = 55,
        #endregion

        #region Brass
        /// <summary>Brass trumpet.</summary>
        [UsedImplicitly] Trumpet = 56,

        /// <summary>Brass trombone.</summary>
        [UsedImplicitly] Trombone = 57,

        /// <summary>Brass tuba.</summary>
        [UsedImplicitly] Tuba = 58,

        /// <summary>Muted Trumpet.</summary>
        [UsedImplicitly] MutedTrumpet = 59,

        /// <summary>French Horn.</summary>
        [UsedImplicitly] FrenchHorn = 60,

        /// <summary>Brass Section.</summary>
        [UsedImplicitly] BrassSection = 61,

        /// <summary>Synth Brass 1.</summary>
        [UsedImplicitly] SynthBrass1 = 62,

        /// <summary>Synth Brass 2.</summary>
        [UsedImplicitly] SynthBrass2 = 63,
        #endregion

        #region Reed
        /// <summary>Soprano Saxophone.</summary>
        [UsedImplicitly] SopranoSax = 64,

        /// <summary>Alto Saxophone.</summary>
        [UsedImplicitly] AltoSax = 65,

        /// <summary>Tenor Saxophone.</summary>
        [UsedImplicitly] TenorSax = 66,

        /// <summary>Baritone Saxophone.</summary>
        [UsedImplicitly] BaritoneSax = 67,

        /// <summary>Woodwind Oboe.</summary>
        [UsedImplicitly] Oboe = 68,

        /// <summary>English Horn.</summary>
        [UsedImplicitly] EnglishHorn = 69,

        /// <summary>Reed Bassoon.</summary>
        [UsedImplicitly] Bassoon = 70,

        /// <summary>Reed Clarinet.</summary>
        [UsedImplicitly] Clarinet = 71,
        #endregion

        #region Pipe
        /// <summary>Woodwind Piccolo.</summary>
        [UsedImplicitly] Piccolo = 72,

        /// <summary>Woodwind Flute.</summary>
        [UsedImplicitly] Flute = 73,

        /// <summary>Woodwind Recorder.</summary>
        [UsedImplicitly] Recorder = 74,

        /// <summary>Pan Flute.</summary>
        [UsedImplicitly] PanFlute = 75,

        /// <summary>Blown Bottle.</summary>
        [UsedImplicitly] BlownBottle = 76,

        /// <summary>Woodwind Shakuhachi.</summary>
        [UsedImplicitly] Shakuhachi = 77,

        /// <summary>Woodwind Whistle.</summary>
        [UsedImplicitly] Whistle = 78,

        /// <summary>Woodwind Ocarina.</summary>
        [UsedImplicitly] Ocarina = 79,
        #endregion

        #region Synth Lead
        /// <summary>Square Lead (Square Wave).</summary>
        [UsedImplicitly] SquareLead = 80,

        /// <summary>Sawtooth Lead (Saw Wave).</summary>
        [UsedImplicitly] SawtoothLead = 81,

        /// <summary>Calliope Lead (Synth Calliope).</summary>
        [UsedImplicitly] CalliopeLead = 82,

        /// <summary>Chiff Lead (Chiffer Lead).</summary>
        [UsedImplicitly] ChiffLead = 83,

        /// <summary>Charango Lead.</summary>
        [UsedImplicitly] CharangoLead = 84,

        /// <summary>Voice Lead (Solo Vox).</summary>
        [UsedImplicitly] VoiceLead = 85,

        /// <summary>Fifths Lead (Fifth Saw Wave).</summary>
        [UsedImplicitly] FifthsLead = 86,

        /// <summary>Base Lead (Bass And Lead).</summary>
        [UsedImplicitly] BaseLead = 87,
        #endregion

        #region Synth Pad
        /// <summary>NewAge Synth Pad.</summary>
        [UsedImplicitly] NewAgePad = 88,

        /// <summary>Warm Synth Pad.</summary>
        [UsedImplicitly] WarmPad = 89,

        /// <summary>Polysynth Pad.</summary>
        [UsedImplicitly] PolysynthPad = 90,

        /// <summary>Choir Synth Pad (Space Voice).</summary>
        [UsedImplicitly] ChoirPad = 91,

        /// <summary>Bowed Synth Pad (BowedGlass).</summary>
        [UsedImplicitly] BowedPad = 92,

        /// <summary>Metallic Synth Pad (Metal Pad).</summary>
        [UsedImplicitly] MetallicPad = 93,

        /// <summary>Halo Synth Pad.</summary>
        [UsedImplicitly] HaloPad = 94,

        /// <summary>Sweep Synth Pad.</summary>
        [UsedImplicitly] SweepPad = 95,
        #endregion

        #region Synth Effects
        /// <summary>Rain effect (Ice Rain).</summary>
        [UsedImplicitly] Rain = 96,

        /// <summary>Soundtrack effect.</summary>
        [UsedImplicitly] Soundtrack = 97,

        /// <summary>Crystal effect.</summary>
        [UsedImplicitly] Crystal = 98,

        /// <summary>Atmosphere effect.</summary>
        [UsedImplicitly] Atmosphere = 99,

        /// <summary>Brightness effect.</summary>
        [UsedImplicitly] Brightness = 100,

        /// <summary>Goblin effect.</summary>
        [UsedImplicitly] Goblin = 101,

        /// <summary>Echoes effect (Echo Drops).</summary>
        [UsedImplicitly] Echoes = 102,

        /// <summary>Sci-fi effect (Star Theme).</summary>
        [UsedImplicitly] Scifi = 103,
        #endregion

        #region Ethnic
        /// <summary>Indian Sitar.</summary>
        [UsedImplicitly] Sitar = 104,

        /// <summary>Stringed Banjo.</summary>
        [UsedImplicitly] Banjo = 105,

        /// <summary>Japanese stringed Shamisen.</summary>
        [UsedImplicitly] Shamisen = 106,

        /// <summary>Japanese stringed Koto.</summary>
        [UsedImplicitly] Koto = 107,

        /// <summary>African Kalimba.</summary>
        [UsedImplicitly] Kalimba = 108,

        /// <summary>Aero-phonic Bagpipe.</summary>
        [UsedImplicitly] Bagpipe = 109,

        /// <summary>Bowed String Fiddle.</summary>
        [UsedImplicitly] Fiddle = 110,

        /// <summary>Woodwind Shanai.</summary>
        [UsedImplicitly] Shanai = 111,
        #endregion

        #region Percussive
        /// <summary>Tinkle Bell.</summary>
        [UsedImplicitly] TinkleBell = 112,

        /// <summary>Percussive Agogo.</summary>
        [UsedImplicitly] Agogo = 113,

        /// <summary>Steel Drums.</summary>
        [UsedImplicitly] SteelDrums = 114,

        /// <summary>Percussive Woodblock.</summary>
        [UsedImplicitly] Woodblock = 115,

        /// <summary>Taiko Drum.</summary>
        [UsedImplicitly] TaikoDrum = 116,

        /// <summary>Melodic Tom.</summary>
        [UsedImplicitly] MelodicTom = 117,

        /// <summary>Synth Drum.</summary>
        [UsedImplicitly] SynthDrum = 118,

        /// <summary>Reverse Cymbal.</summary>
        [UsedImplicitly] ReverseCymbal = 119,
        #endregion

        #region Sound Effects
        /// <summary>Guitar Fret Noise.</summary>
        [UsedImplicitly] GuitarFretNoise = 120,

        /// <summary>Breath Noise.</summary>
        [UsedImplicitly] BreathNoise = 121,

        /// <summary>Seashore effect.</summary>
        [UsedImplicitly] Seashore = 122,

        /// <summary>Bird Tweet.</summary>
        [UsedImplicitly] BirdTweet = 123,

        /// <summary>Telephone Ring.</summary>
        [UsedImplicitly] TelephoneRing = 124,

        /// <summary>Helicopter effect.</summary>
        [UsedImplicitly] Helicopter = 125,

        /// <summary>Applause effect.</summary>
        [UsedImplicitly] Applause = 126,

        /// <summary>Gunshot effect.</summary>
        [UsedImplicitly]
        None = 127  //// Gunshot
        #endregion
    }
}
