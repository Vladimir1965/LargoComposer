// <copyright file="MusicalToneCollection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Melodic Tone Collection.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public sealed class MusicalToneCollection : Collection<MusicalTone>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalToneCollection class.
        /// </summary>
        public MusicalToneCollection() {
        }

        /// <summary>
        /// Initializes a new instance of the MusicalToneCollection class.
        /// </summary>
        /// <param name="givenList">Given list.</param>
        /// <param name="setOrdinalIndex">Set Ordinal Index.</param>
        public MusicalToneCollection(IEnumerable<MusicalTone> givenList, bool setOrdinalIndex) {
            Contract.Requires(givenList != null);
            if (givenList == null) {
                return;
            }

            foreach (var tone in givenList) {
                this.AddTone(tone, setOrdinalIndex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MusicalToneCollection class.
        /// </summary>
        /// <param name="givenList">Given list.</param>
        public MusicalToneCollection(IList<MusicalTone> givenList)
            : base(givenList) {
        }

        /// <summary>
        /// Initializes a new instance of the MusicalToneCollection class.
        /// </summary>
        /// <param name="givenTones">Given tones.</param>
        /// <param name="range">Rhythmical bit-range of the tones.</param>
        /// <param name="setOrdinalIndex">Set Ordinal Index.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        public MusicalToneCollection(IEnumerable<IMusicalTone> givenTones, BitRange range, bool setOrdinalIndex) {
            if (givenTones == null) {
                return;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var mtone in givenTones) {
                var toneRange = mtone.BitRange; //// (barNumber);
                if (toneRange == null) {
                    continue;
                }

                if (range != null) { //// 2016/07
                    var interRange = toneRange.IntersectionWith(range);
                    if (interRange == null || interRange.IsEmpty) {
                        continue;
                    }
                }

                if (mtone is MusicalTone tone)
                {
                    this.AddTone(tone, setOrdinalIndex);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Tone Key.
        /// </summary>
        /// <value> General property.</value>
        public string ToneKey {
            get {
                var sb = new StringBuilder();
                foreach (var s in this.Select(mt => MusicalProperties.GetNoteName(mt.Pitch.Element) + mt.Pitch.Octave)) {
                    sb.Append(s);
                }

                return sb.ToString();
            }
        }

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public MusicalToneCollection FormalTones {
            get {
                var elements = (from mt in this select mt.Pitch.Element).Distinct();
                var firstTone = this.FirstOrDefault();
                if (firstTone?.Pitch == null) {
                    return null;
                }

                var harSystem = firstTone.Pitch.HarmonicSystem;
                var ftc = new MusicalToneCollection();
                foreach (var p in elements.Select(element => new MusicalPitch(harSystem, 0, element))) {
                    ftc.AddTone(new MusicalTone(p, firstTone.BitRange, firstTone.Loudness, firstTone.BarNumber), true); //// (firstTone.BarNumberFrom)
                }

                return ftc;
            }
        }

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public MusicalToneCollection ValidToneList { //// MusicalToneCollection
            get {
                //// 2011/11 added && mt.IsTrueTone
                var tones = from mt in this
                            where !mt.IsEmpty && mt.IsTrueTone
                            orderby mt.Pitch.SystemAltitude
                            select mt;
                var collection = new MusicalToneCollection(tones, false);
                return collection; //// new MusicalToneCollection(list.ToList());
            }
        }

        /// <summary> Gets String representation of the collection. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema {
            get {
                var str = new StringBuilder();
                byte i = 0;
                foreach (var s in from mt in this where mt != null && mt.IsTrueTone select mt.Pitch.ToString()) {
                    str.Append(s);
                    i++;
                    if (i != this.Count) {
                        str.Append(",");
                    }
                }

                return str.ToString();
            }
        }

        /// <summary> Gets For melodical tracks only. </summary>
        /// <value> Property description. </value>
        public MusicalOctave MeanOctave {
            get {
                const int limit = 100;
                var sumNotes = 0;
                var numNotes = 0;
                foreach (var mt in this.Where(mt => mt?.Pitch != null)) {
                    sumNotes += mt.Pitch.MidiKeyNumber;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanNote = numNotes != 0 ? (byte)((float)sumNotes / numNotes) : (byte)0;

                var firstTone = this.FirstOrDefault();
                MusicalPitch mp;
                if (firstTone?.Pitch != null) {
                    var harSystem = firstTone.Pitch.HarmonicSystem;
                    mp = harSystem.GetPitch(meanNote);
                }
                else {
                    mp = new MusicalPitch(meanNote);
                }

                return (mp != null) ? (MusicalOctave)mp.Octave : MusicalOctave.None;
            }
        }

        /// <summary>Gets BandType. </summary>
        /// <value> Property description. </value>
        public MusicalBand MeanBandType {
            get {
                const int limit = 100;
                var sumNotes = 0;
                var numNotes = 0;
                foreach (var mt in this.Where(mt => mt?.Pitch != null)) {
                    sumNotes += mt.Pitch.MidiKeyNumber;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanNote = numNotes != 0 ? (byte)((float)sumNotes / numNotes) : (byte)0;

                return MusicalProperties.BandOfPitch(meanNote);
            }
        }

        /// <summary>Gets Mean Duration. </summary>
        /// <value> Property description. </value>
        public float MeanDuration {
            get {
                const int limit = 100;
                float sumDur = 0;
                var numNotes = 0;
                foreach (var mt in
                    this.Where(mt => mt != null && !mt.IsEmpty && mt.RhythmicOrder != 0)) {
                    sumDur += (float)mt.Duration / mt.RhythmicOrder;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanDur = numNotes != 0 ? sumDur / numNotes : 0;

                return meanDur;
            }
        }

        /// <summary>Gets Mean Loudness. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness MeanLoudness {
            get {
                const int limit = 100;
                float sumLoudness = 0;
                var numNotes = 0;
                foreach (var mt in this.Where(mt => mt != null && mt.Loudness > 0)) {
                    sumLoudness += (short)mt.Loudness;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanLoudness = numNotes != 0 ? sumLoudness / numNotes : 0;

                return (MusicalLoudness)(byte)Math.Round(meanLoudness);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Contains Halftone.
        /// </summary>
        /// <value> Property description. </value>
        public bool ContainsHalftone {
            get {
                for (var i = 1; i < this.Count; i++) {
                    var t0 = this.ElementAt(i - 1);
                    var t1 = this.ElementAt(i);
                    //// Sequences containing halftones are considered to be melodic
                    if (t0 == null || t1 == null || t0.Pitch == null || t1.Pitch == null) {
                        continue;
                    }

                    if (Math.Abs(t0.Pitch.SystemAltitude - t1.Pitch.SystemAltitude) == 1) {
                        return true; //// Avoid multiple or conditional return statements.
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is is a simple stagnation.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsSimpleStagnation {
            get {
                if (this.Count != 2) {
                    return false;
                }

                var t0 = this.ElementAt(0);
                var t1 = this.ElementAt(1);
                if (t0 == null || t1 == null || t0.Pitch == null || t1.Pitch == null) {
                    return false;
                }

                return t0.Pitch.SystemAltitude == t1.Pitch.SystemAltitude;
            }
        }
        #endregion

        #region Static support

        /// <summary>
        /// Guess Mel Part Type.
        /// </summary>
        /// <param name="bandType">Band type.</param>
        /// <param name="melMotionAllowed">Melodic Motion Allowed.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static MelodicFunction GuessMelodicType(MusicalBand bandType, bool melMotionAllowed) {
            MelodicFunction mpt;
            switch (bandType) {
                case MusicalBand.HighTones: {
                        mpt = melMotionAllowed ? MelodicFunction.MelodicMotion : MelodicFunction.HarmonicMotion;
                        break;
                    }

                case MusicalBand.MiddleTones: {
                        mpt = MelodicFunction.HarmonicMotion; //// HarmonicFilling;
                        break;
                    }

                case MusicalBand.BassTones: {
                        mpt = MelodicFunction.HarmonicBass;
                        break;
                    }

                default: {
                        mpt = MelodicFunction.HarmonicFilling;
                        break;
                    }
            }

            return mpt;
        }

        /// <summary>
        /// Harmonic Mel Part Type.
        /// </summary>
        /// <param name="bandType">Band type.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static MelodicFunction HarmonicMelodicType(MusicalBand bandType) {
            var mpt = MelodicFunction.None;
            switch (bandType) {
                case MusicalBand.HighTones: {
                        mpt = MelodicFunction.HarmonicMotion;
                        break;
                    }

                case MusicalBand.MiddleTones: {
                        mpt = MelodicFunction.HarmonicFilling;
                        break;
                    }

                case MusicalBand.BassTones: {
                        mpt = MelodicFunction.HarmonicBass;
                        break;
                    }

                case MusicalBand.Any:
                    break;
                case MusicalBand.BassBeat:
                    break;
                case MusicalBand.MiddleBeat:
                    break;
                case MusicalBand.HighBeat:
                    break;
                //// resharper default: break;
            }

            return mpt;
        }

        #endregion

        #region Public methods
        /// <summary> Add on musical tone to the end of part. </summary>
        /// <param name="givenTone">Musical tone.</param>
        /// <param name="setOrdinalIndex">Set Ordinal Index.</param>
        public void AddTone(MusicalTone givenTone, bool setOrdinalIndex) {
            Contract.Requires(givenTone != null);
            //// if (givenTone == null) { return false;  }

            if (setOrdinalIndex) {
                givenTone.OrdinalIndex = this.Count;
            }

            this.Add(givenTone);
        }

        /// <summary>
        /// Add Collection.
        /// </summary>
        /// <param name="givenTones">Given tones.</param>
        /// <param name="setOrdinalIndex">Set Ordinal Index.</param>
        public void AddCollection(MusicalToneCollection givenTones, bool setOrdinalIndex) {
            Contract.Requires(givenTones != null);
            //// if (givenTone == null) { return false;  }

            givenTones.ForAll(musicalTone => this.AddTone(musicalTone, setOrdinalIndex));
        }

        /// <summary>
        /// Has Any Inconsistency With Harmony.
        /// </summary>
        /// <param name="harmonicStructure">Harmonic Structure.</param>
        /// <param name="harmonicRange">Harmonic Range.</param>
        /// <returns> Returns value. </returns>
        public bool HasAnyInconsistencyWithHarmony(BinaryStructure harmonicStructure, BitRange harmonicRange) {
            if (harmonicStructure == null || harmonicRange == null) {
                return false;
            }

            return this.Where(mt => mt != null && harmonicRange.CoverRange(mt.BitRange)).Any(mt => (mt.Pitch != null) //// (mt.BarNumberFrom)
                                        && !harmonicStructure.IsOn(mt.Pitch.Element));
        }

        /// <summary>
        /// Rhythmic Pattern.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string RhythmicPattern() {
            var sb = new StringBuilder();
            foreach (var mt in this.Where(mt => mt != null)) {
                sb.Append(mt.Duration.ToString(CultureInfo.CurrentCulture.NumberFormat));
                sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determine Harmonic Struct.
        /// </summary>
        /// <param name="harmonicModality">Harmonic modality.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public HarmonicStructure DetermineHarmonicStruct(BinarySchema harmonicModality) {
            Contract.Requires(harmonicModality != null);
            //// if (harmonicModality == null) { return null; }

            if (this.Count == 0) {
                return null;
            }

            var harSystem = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            var hstruct = new HarmonicStructure(harSystem, (string)null) { ToneSchema = null };
            var toneValue = this.DetermineToneValues(harSystem, harmonicModality);
            SortToneValues(hstruct, toneValue);

            hstruct.DetermineLevel();
            hstruct.DetermineBehavior();
            //// string s = hstruct.ToneSchema;
            return hstruct;
        }

        /// <summary>
        /// Export To Midi Events.
        /// </summary>
        /// <param name="events">Event Collection.</param>
        /// <param name="instrument">Midi Instrument.</param>
        /// <param name="barDivision">Bar division.</param>
        public void ExportToMidiEvents(MidiEventCollection events, byte instrument, int barDivision) {
            Contract.Requires(events != null);

            if (events == null) {
                return;
            }

            //// if (channel != MidiChannel.DrumChannel) {
            events.PutInstrument(0, instrument);
            //// }

            foreach (var mt in this.Where(mt => mt != null && mt.RhythmicOrder > 0))
            {
                //// RealTone rt = new RealTone(mt, instrument, channel);
                mt.InstrumentNumber = instrument;
                //// mt.Channel = channel;
                mt.WriteTo(events, barDivision);
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            this.ForAll(musicalTone => s.Append(musicalTone));

            return s.ToString();
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Sorts the tone values.
        /// </summary>
        /// <param name="hstruct">The harmonic structure.</param>
        /// <param name="toneValue">The tone value.</param>
        private static void SortToneValues(HarmonicStructure hstruct, Dictionary<int, float> toneValue) {
            Contract.Requires(toneValue != null);
            if (hstruct == null) {
                return;
            }

            ////  Sorts toneValues by weight 
            var values = toneValue.Values;
            var top = (from v in values select v).OrderByDescending(v => v).Take(3).ToList();
            var limit = 3; //// max number of tones in the structure
            foreach (var kvp in toneValue) {
                if (top.Contains(kvp.Value)) {
                    var elem = (byte)kvp.Key; // % harSystem.Order 
                    hstruct.On(elem);
                    limit--;
                }

                if (limit == 0) {
                    break;
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Determines the tone values.
        /// </summary>
        /// <param name="harSystem">The harmonic system.</param>
        /// <param name="harmonicModality">The harmonic modality.</param>
        /// <returns> Returns value. </returns>
        private Dictionary<int, float> DetermineToneValues(HarmonicSystem harSystem, BinarySchema harmonicModality) {
            Contract.Requires(harmonicModality != null);

            if (harSystem == null) {
                return null;
            }

            const float modalToneIncrease = 2.0f;
            const float startingToneIncrease = 0.1f;
            var toneValue = new Dictionary<int, float>();
            //// Assign weight value to any modalAltitude
            for (byte level = 0; level < harmonicModality.Level; level++) {
                int modalAltitude = harmonicModality.PlaceAtLevel(level);
                float value = 0;
                foreach (var mt in this) {
                    if (mt != null && mt.IsTrueTone && harSystem.Order != 0) {
                        var formalAltitude = mt.Pitch.SystemAltitude % harSystem.Order;
                        if (formalAltitude == modalAltitude) {
                            value += modalToneIncrease; //// 0.5
                            //// Long tones have higher values
                            //// value += mt.Duration - (mt.Pitch.Octave * 0.1f);
                            //// Starting tones have higher values
                            if (mt.BitFrom == 0) {
                                value += startingToneIncrease;
                            }
                        }

                        //// The tones consonant with other tones have higher values
                        Contract.Assume(formalAltitude - modalAltitude > short.MinValue);
                        var formalDistance = formalAltitude - modalAltitude;
                        var v = MusicalInterval.GuessSonanceValue(formalDistance);
                        value += v;
                    }

                    if (toneValue.ContainsKey(modalAltitude)) {
                        toneValue[modalAltitude] += value;
                    }
                    else {
                        toneValue.Add(modalAltitude, value);
                    }
                }
            }

            return toneValue;
        }
        #endregion
    }
}
