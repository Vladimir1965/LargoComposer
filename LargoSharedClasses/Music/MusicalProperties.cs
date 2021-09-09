// <copyright file="MusicalProperties.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Notation;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Properties.
    /// </summary>
    public static class MusicalProperties
    {
        #region Fields
        /// <summary> Count of used MIDI channel. </summary>
        public const byte MidiCountChannels = 16; //// 8; ////31?

        /// <summary>
        /// Loudness quotient.
        /// </summary>
        private const byte LoudnessQuotient = 14;

        /// <summary>
        /// Central loudness.
        /// </summary>
        private const float CentralLoudness = 4.5f;

        /// <summary>
        /// Central velocity.
        /// </summary>
        private const float CentralVelocity = 63f;
        #endregion

        #region Older Static Interface
        /// <summary>
        /// For given part number returns number of channel.
        /// </summary>
        /// <param name="lineIndex">Line Number.</param>
        /// <returns> Returns value. </returns>
        public static MidiChannel ChannelForPartNumber(int lineIndex) {
            var num = lineIndex % MidiCountChannels;
            return (MidiChannel)(byte)((num < (byte)MidiChannel.DrumChannel) ? num : (num + 1) % MidiCountChannels);
        }

        #endregion

        #region Public static methods
        /// <summary>
        /// Rhythmic Order.
        /// </summary>
        /// <param name="division">Rhythmical division.</param>
        /// <param name="metricBeat">Metric Beat.</param>
        /// <param name="metricGround">Metric Ground.</param>
        /// <returns> Returns value. </returns>
        public static byte RhythmicOrder(int division, byte metricBeat, byte metricGround) {
            Contract.Requires(metricGround != 0);
            var barDivision = BarDivision(division, metricBeat, metricGround);
            var rhythmicOrder = (byte)Math.Min(barDivision, DefaultValue.MaximumRhythmicOrder); //// !!!!
            while (rhythmicOrder > 0 && barDivision % rhythmicOrder > 0) { //// was division % rhythmicOrder
                rhythmicOrder--;
            }

            return rhythmicOrder;
        }

        /// <summary>
        /// Determines the rhythmic order.
        /// </summary>
        /// <param name="division">The division.</param>
        /// <returns> Returns value. </returns>
        public static int DetermineRhythmicOrder(int division) {
            const int p2 = 2;
            const int p3 = 3;
            const int p5 = 5;

            while (division > DefaultValue.MaximumRhythmicOrder) {
                if (division % p2 == 0) {
                    division = division / p2;
                }
                else {
                    if (division % p3 == 0) {
                        division = division / p3;
                    }
                    else {
                        if (division % p5 == 0) {
                            division = division / p5;
                        }
                    }
                }
            }

            return division;
        }

        /// <summary> Converts the integer index to its formal system representative element. </summary>
        /// <param name="order">Order of the system.</param>        
        /// <param name="sysLength">Real system length.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static byte FormalLength(byte order, int sysLength) {
            if (order == 0) {
                return 0;
            }

            //// Time optimization
            if (sysLength < 0) {
                //// if (sysLength < -order) {  sysLength += order + order; }

                while (sysLength < 0) {
                    sysLength += order;
                }
            }
            else {
                if (sysLength < order) {
                    return (byte)sysLength;
                }

                while (sysLength >= order) {
                    sysLength -= order;
                }
            }

            //// int frmLength = sysLength % order;
            //// return (byte)((frmLength < 0) ? (frmLength + order) : frmLength);
            return (byte)sysLength;
        }

        /// <summary>
        /// Gets the letter.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <param name="uppercase">If set to <c>true</c> [uppercase].</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static string GetLetter(int givenNumber, bool uppercase) {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";   //// char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var count = alphabet.Length;
            var rest = givenNumber % count;
            var layer = givenNumber / count;
            var result = alphabet[rest].ToString();
            if (layer > 0) {
                result = result + layer;
            }

            if (uppercase) {
                result = result.ToUpper(CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the motive.
        /// </summary>
        /// <param name="givenPrefix">The given prefix.</param>
        /// <param name="givenNumber">The given number.</param>
        /// <param name="givenLength">Length of the given.</param>
        /// <returns> Returns value. </returns>
        public static string GetMotiveName(string givenPrefix, int givenNumber, int givenLength) {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";   //// char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var count = alphabet.Length;
            var rest = (givenNumber - 1) % count;
            var layer = (givenNumber - 1) / count;
            var result = alphabet[rest].ToString(CultureInfo.CurrentCulture);
            if (layer > 0) {
                result = result + (layer + 1);
            }

            result = givenPrefix + result.ToUpper() + "(" + givenLength.ToString() + ")"; //// "00", CultureInfo.CurrentCulture
            return result;
        }

        /// <summary>
        /// Gets the order value.
        /// </summary>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns> Returns value. </returns>
        public static string GetOrderValue(byte harmonicOrder, byte rhythmicOrder) {
            return string.Format(CultureInfo.CurrentCulture.NumberFormat, "H{0}/R{1}", harmonicOrder, rhythmicOrder);
        }

        /// <summary>
        /// Gets the tempo value.
        /// </summary>
        /// <param name="tempo">The tempo.</param>
        /// <returns> Returns value. </returns>
        public static string GetTempoValue(int tempo) {
            var s = string.Empty;
            var list = DataEnums.GetListMusicalTempo;
            var numbers = SupportCommon.GetEnumValues(typeof(MusicalTempo));
            if (numbers == null) {
                return s;
            }

            var idx = numbers.TakeWhile(val => tempo > val).Count();
            if (idx < list.Count && list[idx] != null) {
                s = list[idx].Value;
            }

            return s;
        }

        /// <summary>
        /// Gets the index of the tempo.
        /// </summary>
        /// <param name="tempo">The tempo.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static int GetTempoIndex(int tempo) {
            var list = DataEnums.GetListMusicalTempo;
            var numbers = SupportCommon.GetEnumValues(typeof(MusicalTempo));
            if (numbers == null) {
                return -1;
            }

            var idx = numbers.TakeWhile(val => tempo > val).Count(); ////>
            if (idx > 0 && idx < list.Count && list[idx] != null) {
                return idx - 1;
            }
            else {
                return -1;
            }
        }

        #endregion

        #region Conversion of notes
        /// <summary>
        /// Steps to step number.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static int StepToStepNumber(string step) {
            switch (step) {
                case "C":
                    return 0;
                case "D":
                    return 1;
                case "E":
                    return 2;
                case "F":
                    return 3;
                case "G":
                    return 4;
                case "A":
                    return 5;
                case "B":
                    return 6;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get Note Number.
        /// </summary>
        /// <param name="noteName">Single Note Name.</param>
        /// <param name="alterSign">Alter Sign.</param>
        /// <returns> Returns value. </returns>
        public static int GetNoteNumber(string noteName, short alterSign) {
            var number = GetNoteNumber(noteName);
            number += alterSign;
            return number;
        }

        /// <summary>Gets the name of a note given its numeric value.</summary>
        /// <param name="note">The numeric value of the note.</param>
        /// <returns>The name of the note.</returns>
        public static string GetNoteName(byte note) {
            //// Get the pitch within the octave
            var noteNumber = (MusicalNote)(note % DefaultValue.HarmonicOrder);
            var name = noteNumber.ToString().Replace("is", "#");
            name = name.Replace("Note", string.Empty);
            return name;
        }

        /// <summary>Gets the name of a note given its numeric value.</summary>
        /// <param name="note">The numeric value of the note.</param>
        /// <returns>The name of the note.</returns>
        public static string GetSingleNoteName(byte note) {
            //// Get the pitch within the octave
            var noteNumber = (MusicalNote)(note % DefaultValue.HarmonicOrder);
            var name = noteNumber.ToString().Substring(4, 1);
            return name;
        }

        /// <summary>Gets the name of a note given its numeric value.</summary>
        /// <param name="note">The numeric value of the note.</param>
        /// <returns>The name of the note.</returns>
        public static short GetAlterSign(byte note) {
            //// Get the pitch within the octave
            var noteNumber = (MusicalNote)(note % DefaultValue.HarmonicOrder);
            var sign = (short)(noteNumber.ToString().Contains("is") ? 1 : 0);
            return sign;
        }

        /// <summary>
        /// Gets the name of a note given its numeric value.
        /// </summary>
        /// <param name="note">The numeric value of the note.</param>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <returns>
        /// The name of the note.
        /// </returns>
        public static string GetNoteNameAndOctave(byte note, byte harmonicOrder) {
            const string rest = @" - ";
            if (note == 0) {
                return rest;
            }

            //// Get the octave and the pitch within the octave
            var octave = (harmonicOrder != 0) ? note / harmonicOrder : 0;
            //// Translate the pitch into a note name
            var name = GetNoteName(note);
            //// Append the octave onto the name
            return name.Trim() + MusicalPitch.RealOctaveNumber(octave).ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Get Note Type.
        /// </summary>
        /// <param name="quotient">Length quotient.</param>
        /// <returns> Returns value. </returns>
        public static string GetNoteType(float quotient) { //// whole, half, quarter, eighth, sixteenth
            string noteType;
            const float delta = 0.01f;
            if (quotient >= DefaultValue.Unit - delta) {
                noteType = "whole";
            }
            else {
                if (quotient >= DefaultValue.HalfUnit - delta) {
                    noteType = "half";
                }
                else {
                    noteType = quotient >= DefaultValue.QuarterUnit - delta ? "quarter"
                        : quotient >= DefaultValue.EightUnit - delta ? "eighth" : "sixteenth";
                }
            }

            return noteType;
        }

        #endregion

        #region Octave And Band
        /// <summary>
        /// Band Of Pitch.
        /// </summary>
        /// <param name="givenMidiNote">Midi note.</param>
        /// <returns> Returns value. </returns>
        public static MusicalBand BandOfPitch(byte givenMidiNote) {
            const byte bassToMiddleBreak = 52;
            const byte middleToHighBreak = 76;
            if (givenMidiNote < bassToMiddleBreak) {
                return MusicalBand.BassTones;
            }

            return givenMidiNote < middleToHighBreak ? MusicalBand.MiddleTones : MusicalBand.HighTones;
        }

        /// <summary>
        /// Band TypeFromOctave.
        /// </summary>
        /// <param name="givenOctave">Given Octave.</param>
        /// <returns> Returns value. </returns>
        public static MusicalBand BandTypeFromOctave(MusicalOctave givenOctave) {
            var bt = (byte)givenOctave <= (byte)MusicalOctave.Small ? MusicalBand.BassTones
                : (byte)givenOctave >= (byte)MusicalOctave.TwoLine ? MusicalBand.HighTones : MusicalBand.MiddleTones;

            return bt;
        }

        #endregion

        #region Acoustic ...
        /// <summary>
        /// Midis the key of frequency.
        /// </summary>
        /// <param name="givenFrequency">The given frequency.</param>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        public static byte MidiKeyOfFrequency(double givenFrequency) {
            const double halfToneShift = 3.0313597; //// for 440 Hz = midi 69 
            byte midiKeyNumber = (byte)Math.Round(((Math.Log(givenFrequency) / Math.Log(2)) - halfToneShift) * 12, 0);
            return midiKeyNumber;
        }

        /// <summary>
        /// Velocity Of Loudness.
        /// </summary>
        /// <param name="givenLoudness">Loudness value.</param>
        /// <returns> Returns value. </returns>
        public static byte VelocityOfLoudness(byte givenLoudness) {
            const byte maxVelocity = (byte)CentralVelocity * 2;
            if (givenLoudness > (byte)MusicalLoudness.MaxLoudness) {
                givenLoudness = (byte)MusicalLoudness.MaxLoudness;
            }

            var loudness = givenLoudness - CentralLoudness; //// 6.3f;
            var velocity = CentralVelocity + (loudness * LoudnessQuotient);
            byte v;
            checked {
                v = (byte)Math.Round(velocity);
            }

            return v > maxVelocity ? maxVelocity : v;
        }

        /// <summary>
        /// Loudness Of Velocity.
        /// </summary>
        /// <param name="velocity">Velocity value.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalLoudness LoudnessOfVelocity(byte velocity) {
            const byte maxLoudness = (byte)CentralLoudness * 2;
            var deltaVelocity = velocity - CentralVelocity;
            var loudness = CentralLoudness + (deltaVelocity / LoudnessQuotient);
            byte value;
            checked {
                value = (byte)Math.Round(loudness);
            }

            return (MusicalLoudness)(value > maxLoudness ? maxLoudness : value);
        }

        /// <summary>
        /// Models the rhythmic order.
        /// </summary>
        /// <param name="realRhythmicOrder">The real rhythmic order.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static byte ModelRhythmicOrder(byte realRhythmicOrder) {
            byte rhythmicOrder = 6;
            if (realRhythmicOrder % 9 == 0) {
                rhythmicOrder = 9;
            }
            else {
                if (realRhythmicOrder % 8 == 0) {
                    rhythmicOrder = 8;
                }
            }

            return rhythmicOrder;
        }

        /// <summary>
        /// Bar Division.
        /// </summary>
        /// <param name="division">Division value.</param>
        /// <param name="metricBeat">Metric beat.</param>
        /// <param name="metricGround">Metric ground.</param>
        /// <returns> Returns value. </returns>
        public static int BarDivision(int division, byte metricBeat, byte metricGround) {
            Contract.Requires(metricGround != 0);
            if (metricBeat == 0 || metricGround == 0) {
                return 0;
            }

            int wholeBarDivision;
            checked {
                wholeBarDivision = division * 4;
            }

            var particularBarDivision = wholeBarDivision * metricBeat / metricGround;
            return particularBarDivision;
        }

        /// <summary>
        /// For given part number returns number of channel.
        /// </summary>
        /// <param name="sysOrder">System order.</param>
        /// <param name="length">Formal length.</param>
        /// <param name="barDivision">Bar division.</param>
        /// <returns> Returns value. </returns>
        public static int MidiDuration(byte sysOrder, int length, int barDivision) {
            Contract.Requires(sysOrder != 0);
            if (length == 0) {
                return 0;
            }

            if (sysOrder == 0) {
                throw new InvalidOperationException("Invalid Rhythmical Order.");
            }

            if (barDivision * length < sysOrder) {
                throw new InvalidOperationException("Invalid Midi Duration.");
            }

            var midiDuration = (int)Math.Round((double)(barDivision / sysOrder * length));  //// checked {}
            return midiDuration;
        }

        /// <summary>
        /// Midis the time to ticks quotient.
        /// </summary>
        /// <param name="tempo">The tempo.</param>
        /// <param name="division">The division.</param>
        /// <returns> Returns value. </returns>
        public static float MidiTimeToTicksQuotient(int tempo, int division) { ///// byte metricGround,
                                                                               //// http://devmaster.net/forums/topic/7090-midi-time-to-milliseconds/
                                                                               //// tempo / (division * 1000). Since tempo is in microseconds per quarter-note, 
                                                                               //// and division is in ticks per quarter-note, you should divide them to get 
                                                                               //// the microseconds per tick, then divide by 1000 to get milliseconds per tick

            var quotient = division * tempo / 60000.0f;

            //// experimentally set (see metronome click for bar, ... ?!)
            //// var quotient = metricGround / 48f * tempo / 12f * division / 384f;
            return quotient;
        }

        /// <summary>
        /// Bars the duration of the midi.
        /// </summary>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <param name="metricBeat">The metric beat.</param>
        /// <param name="metricGround">The metric ground.</param>
        /// <param name="division">The division.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static int BarMidiDuration(byte rhythmicOrder, byte metricBeat, byte metricGround, int division) {
            Contract.Requires(rhythmicOrder != 0);
            Contract.Requires(metricGround != 0);
            var barDivision = BarDivision(division, metricBeat, metricGround);
            var barMidiDuration = MidiDuration(rhythmicOrder, rhythmicOrder, barDivision);
            return barMidiDuration;
        }

        #endregion

        #region Metric
        /// <summary>
        /// Gets the metric base.
        /// </summary>
        /// <param name="metricGround">The metric ground.</param>
        /// <returns> Returns value. </returns>
        public static byte GetMetricBase(byte metricGround) {
            var metricBase = Math.Log(metricGround) / Math.Log(2.0);
            return (byte)(int)Math.Round(metricBase, 0);
        }

        /// <summary>
        /// Gets the metric ground.
        /// </summary>
        /// <param name="metricBase">The metric base.</param>
        /// <returns> Returns value. </returns>
        public static byte GetMetricGround(byte metricBase) {
            Contract.Ensures(Contract.Result<byte>() > 0);
            return (byte)(int)Math.Pow(2, metricBase);
        }

        /// <summary>
        /// Gets the metric value.
        /// </summary>
        /// <param name="metricBeat">The metric beat.</param>
        /// <param name="metricGround">The metric ground.</param>
        /// <returns> Returns value. </returns>
        public static string GetMetricValue(byte metricBeat, byte metricGround) {
            return string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0}/{1}", metricBeat, metricGround);
        }
        #endregion

        #region Values

        /// <summary>
        /// Percentage from value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static byte PercentageFromValue(MusicalValue value) {
            const int extent = (byte)MusicalValue.VeryHigh - (byte)MusicalValue.VeryLow;
            var percent = (byte)((byte)value * 100.0f / extent);
            return percent;
        }

        #endregion

        #region Fixed names of structures
        /// <summary>
        /// Chords the names.
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string ChordNames(long classNumber) {
            string s = string.Empty;
            switch (classNumber) {
                case 1: s = @"Unison"; break;
                case 3: s = @"Semitone"; break;
                case 5: s = @"Whole-tone"; break;
                case 9: s = @"Minor Third"; break;
                case 17: s = @"Major Third"; break;
                case 33: s = @"Perfect Fourth"; break;
                case 37: s = @"Incomplete Minor-seventh Chord"; break;
                case 41: s = @"Incomplete Dominant-seventh Chord.2"; break;
                case 65: s = @"Tritone"; break;
                case 67: s = @"Rite chord.2, Tritone-fourth.1"; break;
                case 69: s = @"Incomplete Dominant-seventh Chord.1"; break;
                case 73: s = @"Diminished Chord"; break;
                case 81: s = @"Incomplete Half-dim-seventh Chord"; break;
                case 97: s = @"Rite chord.1, Tritone-fourth.2"; break;
                case 145: s = @"Major Chord"; break;
                case 273: s = @"Augmented Chord"; break;
                case 725: s = @"Dominant-11th"; break;
                case 283: s = @"Minor-major Ninth Chord"; break;
                case 291: s = @"Major-seventh Chord"; break;
                case 293: s = @"Half-diminished Seventh Chord"; break;
                case 297: s = @"Minor-seventh Chord Raga Bhavani"; break;
                case 299: s = @"Major-Ninth Chord"; break;
                case 301: s = @"Diminished-major Ninth Chord"; break;
                case 307: s = @"Major-augmented Ninth Chord"; break;
                case 309: s = @"Diminished-augmented Ninth Chord"; break;
                case 325: s = @"French-sixth Chord"; break;
                case 329: s = @"Dominant-seventh/German-sixth Chord"; break;
                case 345: s = @"Augmented-diminished Ninth Chord"; break;
                case 361: s = @"Minor-diminished Ninth Chord"; break;
                case 409: s = @"Augmented-minor Chord"; break;
                case 425: s = @"Minor Ninth Chord"; break;
                case 585: s = @"Diminished-seventh Chord"; break;
                case 587: s = @"Diminished Minor-Ninth Chord"; break;
                case 619: s = @"Double-Phrygian Hexatonic/chord"; break;
                case 621: s = @"Pyramid Hexatonic/chord"; break;
                case 845: s = @"Petrushka chord"; break;
                case 877: s = @"Petrushka chord"; break;
            }

            return s;
        }

        /// <summary>
        /// Modalities the names.
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string ModalityNames(long classNumber) {
            string s = string.Empty;
            switch (classNumber) {
                case 5: s = @"Warao ditonic (South America)"; break;
                case 7: s = @"Bach/Chromatic Trimirror"; break;
                case 11: s = @"Phrygian Trichord"; break;
                case 13: s = @"Minor Trichord"; break;
                case 15: s = @"BACH/Chromatic Tetramirror"; break;
                case 19: s = @"Major-minor Trichord.1"; break;
                case 21: s = @"Whole-tone Trichord"; break;
                case 23: s = @"Major-second Tetracluster.2"; break;
                case 25: s = @"Major-minor Trichord.2"; break;
                case 27: s = @"Alternating Tetramirror"; break;
                case 29: s = @"Major-second Tetracluster.1"; break;
                case 31: s = @"Chromatic Pentamirror"; break;
                case 33: s = @"Honchoshi,Niagari (Japan)"; break;
                case 37: s = @"Ute tritonic"; break;
                case 39: s = @"Minor-Third Tetracluster.2"; break;
                case 43: s = @"Dorian Tetrachord, Phrygian Tetrachord"; break;
                case 45: s = @"Phrygian Tetrachord, Minor Tetramirror"; break;
                case 47: s = @"Major-second Pentacluster.2"; break;
                case 51: s = @"Chromatic Mezotetrachord, Arabian Tetramirror"; break;
                case 53: s = @"Lydian Tetrachord, Major Tetrachord Warao tetratonic: South America"; break;
                case 55: s = @"Minor-second Major Pentachord"; break;
                case 57: s = @"Minor-Third Tetracluster.1"; break;
                case 59: s = @"Spanish Pentacluster"; break;
                case 61: s = @"Major-second Pentacluster.1"; break;
                case 63: s = @"Chromatic Hexamirror"; break;
                case 71: s = @"Major-Third Tetracluster.2"; break;
                case 75: s = @"Minor-second Diminished Tetrachord"; break;
                case 77: s = @"Harmonic-minor Tetrachord"; break;
                case 79: s = @"Blues Pentacluster"; break;
                case 83: s = @"All-interval Tetrachord.1"; break;
                case 85: s = @"Whole-tone Tetramirror"; break;
                case 87: s = @"Tritone-Expanding Pentachord"; break;
                case 89: s = @"Major-third Diminished Tetrachord"; break;
                case 91: s = @"Alternating Pentachord.1"; break;
                case 93: s = @"Tritone-Symmetric Pentamirror"; break;
                case 99: s = @"Double Fourth Tetramirror"; break;
                case 101: s = @"All-interval Tetrachord.2"; break;
                case 103: s = @"Oriental Pentacluster.1"; break;
                case 105: s = @"Perfect-fourth Diminished Tetrachord"; break;
                case 107: s = @"Locrian Pentamirror"; break;
                case 109: s = @"Alternating Pentachord.2"; break;
                case 113: s = @"Major-Third Tetracluster.1"; break;
                case 115: s = @"Oriental Pentacluster.2"; break;
                case 117: s = @"Tritone-Contracting Pentachord"; break;
                case 121: s = @"Minor-third Pentacluster"; break;
                case 127: s = @"Chromatic Heptamirror"; break;
                case 133: s = @"Quartal Trichord, Warao tritonic (South America), Sansagari (Japan)"; break;
                case 409: s = @"Lebanese Pentachord"; break;
                case 135: s = @"Perfect Fourth Tetramirror"; break;
                case 137: s = @"Minor Chord Peruvian tritonic 2"; break;
                case 139: s = @"All-interval Tetrachord.3"; break;
                case 141: s = @"Major-second Minor Tetrachord"; break;
                case 143: s = @"Major-third Pentacluster.2"; break;
                case 147: s = @"Major-diminished Tetrachord"; break;
                case 149: s = @"Eskimo tetratonic (Alaska), Major-second Major Tetrachord"; break;
                case 151: s = @"Major-seventh Pentacluster.2"; break;
                case 153: s = @"Major-minor Tetramirror"; break;
                case 155: s = @"Major-minor-dim Pentachord.1"; break;
                case 157: s = @"Center-cluster Pentachord.1"; break;
                case 163: s = @"Minor-second Quartal Tetrachord"; break;
                case 165: s = @"Quartal Tetramirror, Genus primum"; break;
                case 167: s = @"Double-seconds Triple-fourth Pentachord.1"; break;
                case 169: s = @"Perfect-fourth Minor Tetrachord"; break;
                case 171: s = @"Phrygian Pentachord"; break;
                case 173: s = @"Dorian/Minor Pentachord, Jewish"; break;
                case 177: s = @"Perfect-fourth Major Tetrachord"; break;
                case 179: s = @"Gypsy/semiditonic Pentachord.1"; break;
                case 181: s = @"Major/Ionic Pentachord"; break;
                case 185: s = @"Center-cluster Pentachord.2"; break;
                case 195: s = @"Messiaen's truncated 5, Lendvai's, Double Tritone Tetramirror"; break;
                case 197: s = @"Tritone Quartal Tetrachord"; break;
                case 199: s = @"Double Pentacluster1"; break;
                case 201: s = @"Minor-diminished Tetrachord"; break;
                case 203: s = @"Javanese Pentachord"; break;
                case 205: s = @"Gypsy/semiditonic Pentachord.2"; break;
                case 209: s = @"All-interval Tetrachord.4"; break;
                case 211: s = @"Balinese Pentachord"; break;
                case 213: s = @"Lydian Pentachord"; break;
                case 217: s = @"Major-minor-dim Pentachord.2"; break;
                case 219: s = @"Alternating Hexamirror"; break;
                case 227: s = @"Double Pentacluster.2"; break;
                case 229: s = @"Double-seconds Triple-fourth Pentachord.2"; break;
                case 231: s = @"Double-cluster Hexamirror"; break;
                case 233: s = @"Major-seventh Pentacluster.1"; break;
                case 241: s = @"Major-third Pentacluster.2"; break;
                case 255: s = @"Chromatic Octamirror"; break;
                case 275: s = @"Minor-augmented Tetrachord"; break;
                case 279: s = @"Augmented Pentacluster.1"; break;
                case 281: s = @"Augmented-major Tetrachord"; break;
                case 285: s = @"Augmented Pentacluster.2"; break;
                case 295: s = @"Diminished Pentacluster.1"; break;
                case 307: s = @"Syrian pentatonic"; break;
                case 313: s = @"Center-cluster Pentamirror"; break;
                case 315: s = @"Indian Sharavati"; break;
                case 325: s = @"Messiaen's truncated 6"; break;
                case 327: s = @"Bardos's, Asymmetric Pentamirror, Indian Gauri"; break;
                case 331: s = @"Kumoi Pentachord.2, Mixolydian Pentatonic"; break;
                case 333: s = @"Augmented-sixth Pentachord.1, Indian Marga Hindola"; break;
                case 339: s = @"Enigmatic Pentachord.1, Indian Nata"; break;
                case 341: s = @"Whole-tone Pentamirror"; break;
                case 343: s = @"Arabian Major Locrian"; break;
                case 347: s = @"Indian Sharasvati"; break;
                case 355: s = @"Balinese Pelog Pentatonic, Korean"; break;
                case 357: s = @"Javan Pentatonic, Augmented-sixth Pentachord.2, Indian Hindola"; break;
                case 363: s = @"Locrian Hexachord"; break;
                case 365: s = @"Super-Locrian Hexamirror"; break;
                case 371: s = @"Indian Malarani"; break;
                case 397: s = @"Indian-Japan Pentatonic"; break;
                case 403: s = @"Persian Pentamirror"; break;
                case 405: s = @"Enigmatic Pentachord.2, Altered Pentatonic"; break;
                case 407: s = @"Indian Vijayasri"; break;
                case 411: s = @"Indian Paraju"; break;
                case 413: s = @"Megha or Cloud"; break;
                case 421: s = @"Korean, Kumoi Pentachord.1"; break;
                case 427: s = @"Phrygian Hexamirror"; break;
                case 429: s = @"Minor Hexachord"; break;
                case 435: s = @"Gypsy hexatonic"; break;
                case 437: s = @"Hawaiian, Melodic-minor Hexachord"; break;
                case 455: s = @"Messiaen's 5"; break;
                case 457: s = @"Diminished Pentacluster.2"; break;
                case 511: s = @"Chromatic Nonamirror"; break;
                case 589: s = @"Flat-Ninth Pentachord"; break;
                case 595: s = @"Neapolitan Pentachord.1"; break;
                case 597: s = @"Major-minor, Prometheus Pentamirror, Korean"; break;
                case 603: s = @"Indian Ghantana"; break;
                case 613: s = @"Neapolitan Pentachord.2, Scriabin"; break;
                case 615: s = @"Schoenberg Anagram Hexachord"; break;
                case 623: s = @"Debussy's Heptatonic"; break;
                case 661: s = @"Natural/Genuine/Black Key Pentatonic, Slendro, Kausika, Mehga"; break;
                case 667: s = @"Indian Dipaka, Prometheus Neapolitan"; break;
                case 669: s = @"Blues scale I, Indian Marva"; break;
                case 683: s = @"Scriabin's Mystic, Prometheus Hexachord, Eskimo (Alaska)"; break;
                case 685: s = @"Dorian Hexachord"; break;
                case 693: s = @"Guidon/Arezzo/Natural/Genuine/Persian Hexachord"; break;
                case 715: s = @"Messiaen's truncated 2, Minor-bitonal Hexachord"; break;
                case 725: s = @"Natural/Genuine/Lydian Hexachord, Ancient Chinese"; break;
                case 727: s = @"Bluesy R&R"; break;
                case 731: s = @"Gypsy, Moravian Pistalkova (Whistle), Alternating Heptachord.1, Hungarian Major"; break;
                case 735: s = @"Blues Octatonic"; break;
                case 743: s = @"Indian, Chromatic inverse"; break;
                case 757: s = @"Tritone Major Heptachord"; break;
                case 819: s = @"Augmented, Messiaen's truncated 3, Lendvai's, Genus tertium"; break;
                case 827: s = @"Gipsy Hexatonic"; break;
                case 829: s = @"Verdi's Enigmatic"; break;
                case 845: s = @"Messiaen's truncated 2, Major-bitonal Hexachord"; break;
                case 847: s = @"Indian Shyamalam"; break;
                case 853: s = @"Harmonic Hexachord, Augmented-11th, Indian Sviraga"; break;
                case 855: s = @"Neapolitan"; break;
                case 859: s = @"Harmonic Minor, Spanish Gypsy"; break;
                case 861: s = @"Indian Narmada"; break;
                case 863: s = @"Indian Cintamani"; break;
                case 871: s = @"Persian, Gypsy, Hungarian, Double Harmonic, Indian Bhairava, Turkish, Oriental"; break;
                case 875: s = @"Harmonic Major"; break;
                case 877: s = @"Diminished,  Alternating Heptachord.2"; break;
                case 925: s = @"Greek Chromatic, Indian"; break;
                case 949: s = @"Modified Blues"; break;
                case 955: s = @"Indian Saurastra"; break;
                case 975: s = @"Messiaen's 4"; break;
                case 981: s = @"Enigmatic Heptatonic, Verdi's Enigmatic"; break;
                case 983: s = @"Verdi's Enigmatic, Free-constructed"; break;
                case 987: s = @"Algerian"; break;
                case 1013: s = @"Blues Octatonic"; break;
                case 1023: s = @"Chromatic Decamirror"; break;
                case 1365: s = @"Whole-tone,  Messiaen's mode 1, Anhemitonic Hexatonic"; break;
                case 1367: s = @"Neapolitan, Leading Whole-tone, Combined"; break;
                case 1371: s = @"Jazz Minor,  Bartok's, Acoustic, Plane-altered, Moravian Podhalska"; break;
                case 1387: s = @"Natural/Genuine, Medieval, Greek"; break;
                case 1403: s = @"Spanish Octatonic, Espla's scale, Jewish"; break;
                case 1455: s = @"Greek Complete, Egyptian, Blues"; break;
                case 1463: s = @"Arabic Zirafkend"; break;
                case 1467: s = @"Spanish, Major-Minor, Blues"; break;
                case 1471: s = @"Nonatonic Blues"; break;
                case 1495: s = @"Messiaen's 6"; break;
                case 1519: s = @"Major-minor Nonatonic"; break;
                case 1755: s = @"Diminished, Messiaen's 2, Lendvai's, Half-Whole step"; break;
                case 1775: s = @"Moorish Phrygian"; break;
                case 1783: s = @"Youlan scale (China), Diminished Nonachord"; break;
                case 1791: s = @"Indian Sindhi-Bhairavi"; break;
                case 1911: s = @"Messiaen's 3, Tsjerepnin, Genus chromaticum"; break;
                case 1983: s = @"Major-minor mixed, Minor Pentatonic with leading tones"; break;
                case 2015: s = @"Messiaen's 7, Symmetrical Decatonic"; break;
                case 2047: s = @"Chromatic Undecamirror"; break;
                case 4095: s = @"Twelve-tone Chromatic, Dodecamirror"; break;
            }

            return s;
        }

        #endregion

        #region Private static methods
        /// <summary>
        /// Note Number.
        /// </summary>
        /// <param name="noteName">Name of the note.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static int GetNoteNumber(string noteName) {
            if (noteName == null) {
                return 0;
            }

            const string noteNumberPrefix = "Note";
            if (string.CompareOrdinal(noteName, "B") == 0) {   //// American
                noteName = "H"; //// Italian
            }

            noteName = noteName.Replace("#", "is");
            noteName = noteNumberPrefix + noteName;
            var num = (MusicalNote)Enum.Parse(typeof(MusicalNote), noteName);
            return (int)num;
        }

        /// <summary>
        /// Frequencies to midi pitch.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private static int FrequencyToMidiPitch(double frequency) {
            double i = 0;
            //// 27,5 Hz is the frequency of note A sub contra (A0) (the lowest in MIDI standard)
            while (true) {
                if ((frequency < 27.5f * Math.Pow(2, 1.0f / 36) * Math.Pow(2, i / 12)) &&
                    (frequency >= 27.5f * Math.Pow(2, -1.0f / 18) * Math.Pow(2, i / 12))) {
                    break;
                }

                i++;
                if (i > 100) {
                    break;
                }
            }

            return (int)i + 21;
        }
        #endregion
    }
}
