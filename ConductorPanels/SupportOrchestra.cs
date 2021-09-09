// <copyright file="SupportOrchestra.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;

namespace ConductorPanels
{
    /// <summary>
    /// Support Orchestra.
    /// </summary>
    public class SupportOrchestra
    {
        /// <summary>
        /// Introduction the woods.
        /// </summary>
        /// <param name="variant">The variant.</param>
        /// <returns> Returns value. </returns>
        public static OrchestraUnit IntroWoods(int variant) {
            //// bar 14-17, 
            var voice1 = new MusicalVoice();
            voice1.Instrument = new MusicalInstrument(MidiMelodicInstrument.FrenchHorn);
            voice1.Octave = MusicalOctave.Small;
            voice1.Loudness = MusicalLoudness.MeanLoudness;

            var voice2 = new MusicalVoice();
            voice2.Instrument = new MusicalInstrument(MidiMelodicInstrument.Trombone);
            voice2.Octave = MusicalOctave.Small; //// or OneLine
            voice2.Loudness = MusicalLoudness.MeanLoudness;

            var voice3 = new MusicalVoice();
            voice3.Instrument = new MusicalInstrument(MidiMelodicInstrument.Bassoon);
            voice3.Octave = MusicalOctave.OneLine;
            voice3.Loudness = MusicalLoudness.MeanLoudness;

            var voice4 = new MusicalVoice();
            voice4.Instrument = new MusicalInstrument(MidiMelodicInstrument.Trombone);
            voice4.Octave = MusicalOctave.OneLine;
            voice4.Loudness = MusicalLoudness.MeanLoudness;

            var voice5 = new MusicalVoice();
            voice5.Instrument = new MusicalInstrument(MidiMelodicInstrument.Trombone);
            voice5.Octave = MusicalOctave.OneLine;
            voice5.Loudness = MusicalLoudness.MeanLoudness;

            var voice6 = new MusicalVoice();
            voice6.Instrument = new MusicalInstrument(MidiMelodicInstrument.Flute);
            voice6.Octave = MusicalOctave.OneLine;
            voice6.Loudness = MusicalLoudness.MeanLoudness;

            var voice7 = new MusicalVoice();
            voice7.Instrument = new MusicalInstrument(MidiMelodicInstrument.Flute);
            voice7.Octave = MusicalOctave.TwoLine;
            voice7.Loudness = MusicalLoudness.MeanLoudness;

            MusicalVoice voice8 = null;
            if (variant == 1) { //// bar 18
                voice7 = null;
            }

            if (variant == 2) { //// bar 19
                voice1.Octave = MusicalOctave.Great;
                voice2.Octave = MusicalOctave.Great;
                voice3.Octave = MusicalOctave.Small;
                voice7.Octave = MusicalOctave.OneLine;
                voice8 = new MusicalVoice();
                voice8.Instrument = new MusicalInstrument(MidiMelodicInstrument.Bassoon);
                voice8.Octave = MusicalOctave.OneLine;
                voice8.Loudness = MusicalLoudness.MeanLoudness;
            }

            if (variant == 3) { //// bar 98, 100
                voice8 = new MusicalVoice();
                voice8.Instrument = new MusicalInstrument(MidiMelodicInstrument.Clarinet);
                voice8.Octave = MusicalOctave.TwoLine;
                voice8.Loudness = MusicalLoudness.MeanLoudness;
            }

            var orchestra = new OrchestraUnit(
                    "Intro Woods " + variant, 
                    "Requiem / Intro", 
                    "Mozart W.A.",
                     voice1, 
                     voice2, 
                     voice3, 
                     voice4);
            orchestra.AddVoices(voice5, voice6, voice7, voice8);
            return orchestra;
        }

        /// <summary>
        /// Introduction the strings.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static OrchestraUnit IntroStrings() {
            //// bar 14-17, 
            var voice1 = new MusicalVoice();
            voice1.Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1);
            voice1.Octave = MusicalOctave.Great; //// or Contra
            voice1.Loudness = MusicalLoudness.MeanLoudness;

            var voice2 = new MusicalVoice();
            voice2.Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1);
            voice2.Octave = MusicalOctave.Small; //// or OneLine
            voice2.Loudness = MusicalLoudness.MeanLoudness;

            var voice3 = new MusicalVoice();
            voice3.Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1);
            voice3.Octave = MusicalOctave.OneLine;
            voice3.Loudness = MusicalLoudness.MeanLoudness;

            var voice4 = new MusicalVoice();
            voice4.Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1);
            voice4.Octave = MusicalOctave.OneLine;
            voice4.Loudness = MusicalLoudness.MeanLoudness;

            var orchestra = new OrchestraUnit("Intro Strings", "Requiem / Intro", "Mozart W.A.", voice1, voice2, voice3, voice4);
            return orchestra;
        }

        /// <summary>
        /// Definition of the choir.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static OrchestraUnit AgnusChoir() {
            var voice1 = new MusicalVoice();
            voice1.Instrument = new MusicalInstrument(MidiMelodicInstrument.ChoirAahs);
            voice1.Octave = MusicalOctave.Small;
            voice1.Loudness = MusicalLoudness.MeanLoudness;

            var voice2 = new MusicalVoice();
            voice2.Instrument = new MusicalInstrument(MidiMelodicInstrument.ChoirAahs);
            voice2.Octave = MusicalOctave.OneLine;
            voice2.Loudness = MusicalLoudness.MeanLoudness;

            var voice3 = new MusicalVoice();
            voice3.Instrument = new MusicalInstrument(MidiMelodicInstrument.VoiceOohs);
            voice3.Octave = MusicalOctave.OneLine;
            voice3.Loudness = MusicalLoudness.MeanLoudness;

            var voice4 = new MusicalVoice();
            voice4.Instrument = new MusicalInstrument(MidiMelodicInstrument.VoiceOohs);
            voice4.Octave = MusicalOctave.TwoLine;
            voice4.Loudness = MusicalLoudness.MeanLoudness;

            var orchestra = new OrchestraUnit("Agnus Choir", "Requiem / Agnus Dei", "Mozart W.A.", voice1, voice2, voice3, voice4);
            return orchestra;
        }

        /// <summary>
        /// Definition of the piano right.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static OrchestraUnit AgnusPianoRight() {
            var voice1 = new MusicalVoice();
            voice1.Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano);  //// Trumpet
            voice1.Octave = MusicalOctave.TwoLine;
            voice1.Loudness = MusicalLoudness.MeanLoudness;

            var orchestra = new OrchestraUnit("Agnus Piano Right", "Requiem / Agnus Dei", "Mozart W.A.", voice1, null, null, null);
            return orchestra;
        }

        /// <summary>
        /// Definition of the piano left.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static OrchestraUnit AgnusPianoLeft() {
            var voice1 = new MusicalVoice();
            voice1.Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano);  //// Cello
            voice1.Octave = MusicalOctave.Small;
            voice1.Loudness = MusicalLoudness.MeanLoudness;

            var voice2 = new MusicalVoice();
            voice2.Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano); //// Cello
            voice2.Octave = MusicalOctave.OneLine;
            voice2.Loudness = MusicalLoudness.MeanLoudness;

            var voice3 = new MusicalVoice();
            voice3.Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano); //// Cello
            voice3.Octave = MusicalOctave.OneLine;
            voice3.Loudness = MusicalLoudness.MeanLoudness;

            var orchestra = new OrchestraUnit("Agnus Piano Left", "Requiem / Agnus Dei", "Mozart W.A.", voice1, voice2, voice3, null);
            return orchestra;
        }
    }
}
