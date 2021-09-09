// <copyright file="MusicalVariety.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Composer
{
    using Abstract;
    using Interfaces;
    using JetBrains.Annotations;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Settings;
    using Localization;
    using Music;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary> Variety of melodic tones. </summary>
    /// <remarks> Variety of tones. </remarks>
    [Serializable]
    [XmlInclude(typeof(MusicalTone))]
    public sealed class MusicalVariety : IMusicalVariety
    {
        #region Fields
        /// <summary>
        /// Musical rules.
        /// </summary>
        private readonly MusicalRules musicalRules;

        /// <summary>
        /// Musical Bar.
        /// </summary>
        [NonSerialized]
        private MusicalBar musicalBar;

        /// <summary>
        /// Musical Element.
        /// </summary>
        [NonSerialized]
        private MusicalElement musicalElement;

        /// <summary>
        /// Line rules.
        /// </summary>
        private ILineRules lineRules;

        /// <summary>
        /// Melodic Tone.
        /// </summary>
        private MusicalTone melTone;

        /// <summary>
        /// Harmonic Structure.
        /// </summary>
        private HarmonicStructure harmonicStructure;

        /// <summary>
        /// Single Harmony.
        /// </summary>
        private bool singleHarmony;

        /// <summary>
        /// Formal Evaluators.
        /// </summary>
        [NonSerialized]
        private DeterminateValue formalEvaluators;

        /// <summary>
        /// Real Evaluators.
        /// </summary>
        [NonSerialized]
        private DeterminateValue realEvaluators;

        /// <summary>
        /// List of melodic tones.
        /// </summary>
        private List<MusicalTone> toneList;

        /// <summary>
        /// Tone clusters.
        /// </summary>
        private List<HarmonicCluster> toneClusters;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalVariety class.  Serializable.
        /// </summary>
        /// <param name="givenSettings">The given settings.</param>
        public MusicalVariety(MusicalSettings givenSettings) {
            //// LimitCount = DefRhythmicShapeVariety.LimitCount;
            this.musicalRules = givenSettings.SettingsComposition.Rules;
            this.HasTraceValues = givenSettings.SettingsProgram.HasTraceValues;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MusicalVariety" /> class from being created.
        /// </summary>
        private MusicalVariety() {
        }

        #endregion

        #region Public properties - Rules and requests
        /// <summary>
        /// Gets Line Rules.
        /// </summary>
        /// <value> Property description. </value>
        public ILineRules LineRules {
            get {
                Contract.Ensures(Contract.Result<ILineRules>() != null);
                if (this.lineRules == null) {
                    throw new InvalidOperationException("Line rules are null.");
                }

                return this.lineRules;
            }

            private set => this.lineRules = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }
        #endregion

        #region Public properties - Status
        /// <summary> Gets Musical Element. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public MusicalElement Element {
            get {
                Contract.Ensures(Contract.Result<MusicalElement>() != null);
                if (this.musicalElement == null) {
                    throw new InvalidOperationException("Music element is null.");
                }

                return this.musicalElement;
            }

            private set => this.musicalElement = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public MusicalLine Line => (MusicalLine)this.Element.Line;

        /// <summary>
        /// Gets or sets a value indicating whether [trace values].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [trace values]; otherwise, <c>false</c>.
        /// </value>
        public bool HasTraceValues { get; set; }

        /// <summary>
        /// Gets or sets the melodic sequence.
        /// </summary>
        /// <value>
        /// The melodic sequence.
        /// </value>
        private MusicalSequence MelodicSequence { get; set; }

        /// <summary> Gets or sets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        //// [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures")]
        private MusicalBar Bar {
            get {
                Contract.Ensures(Contract.Result<MusicalBar>() != null);
                if (this.musicalBar == null) {
                    throw new InvalidOperationException("Musical bar is null.");
                }

                return this.musicalBar;
            }

            set => this.musicalBar = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets the musical rules.
        /// </summary>
        /// <value>
        /// The musical rules.
        /// </value>
        private MusicalRules MusicalRules {
            get {
                Contract.Ensures(Contract.Result<MusicalRules>() != null);
                if (this.musicalRules == null) {
                    throw new InvalidOperationException("Musical rules are null.");
                }

                return this.musicalRules;
            }
        }
        #endregion

        #region Private properties
        /// <summary> Gets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private MusicalTone MusicalTone {
            get {
                Contract.Ensures(Contract.Result<MusicalTone>() != null);
                if (this.melTone == null) {
                    throw new InvalidOperationException("Musical tone is null.");
                }

                return this.melTone;
            }
        }

        /// <summary> Gets list of tones. </summary>
        /// <value> Property description. </value> 
        private List<MusicalTone> ToneList {
            get {
                Contract.Ensures(Contract.Result<List<MusicalTone>>() != null);
                if (this.toneList == null) {
                    throw new ArgumentException("ToneList is null.");
                }

                return this.toneList;
            }
        }

        /// <summary> Gets or sets the recent cluster. </summary>
        /// <value> Property description. </value> 
        private HarmonicCluster RecentCluster { get; set; }

        /// <summary> Gets or sets Main Tone Cluster. </summary>
        /// <value> Property description. </value> 
        private HarmonicCluster MainToneCluster { get; set; }

        /// <summary> Gets or sets list of clusters. </summary>
        /// <value> Property description. </value> 
        private List<HarmonicCluster> ToneClusters {
            get {
                Contract.Ensures(Contract.Result<List<HarmonicCluster>>() != null);
                if (this.toneClusters == null) {
                    throw new InvalidOperationException("Tone clusters are null.");
                }

                return this.toneClusters;
            }

            set => this.toneClusters = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public override string ToString() {
            var s = new StringBuilder();
            s.Append("Melodic variety:"); ////\r\n
            s.Append(this.ToneList.Count.ToString(CultureInfo.CurrentCulture.NumberFormat) + " tones");
            ////s.Append(base.ToString());
            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Prepares the variety.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenRules">The given rules.</param>
        public void PrepareVariety(MusicalElement givenLine, MusicalBar givenBar, ILineRules givenRules) {
            this.Element = givenLine;
            this.Bar = givenBar;
            this.LineRules = givenRules;
            this.MelodicSequence = new MusicalSequence(this);
            this.PrepareEvaluators();
        }

        /// <summary> Generates possible tones. </summary>
        /// <param name="givenTone">Melodic tone.</param>
        /// <param name="harmonicModality">Harmonic modality.</param>
        /// <param name="harmonicStruct">Harmonic structure.</param>
        /// <param name="harmonyOnly">Indicate harmonic tones.</param>
        /// <returns> Returns value. </returns>
        public bool GeneratePossibilities(MusicalTone givenTone, BinarySchema harmonicModality, BinaryStructure harmonicStruct, bool harmonyOnly) {
            Contract.Requires(givenTone != null);
            Contract.Requires(harmonicModality != null);
            //// if (givenTone == null) { return false;  }
            //// if (harmonicModality == null) { return false; }

            this.toneList = new List<MusicalTone>();
            var modalityLevel = harmonicModality.Level;
            for (byte ml = 0; ml < modalityLevel; ml++) {
                var e = harmonicModality.PlaceAtLevel(ml);
                if (harmonyOnly && harmonicStruct != null && harmonicStruct.IsOff(e)) {
                    continue;
                }

                //// 2011/11 Now from lower octaves,because higher octaves are preferred by engine (weighed impulse ...]

                var requestedOctave = this.Element.Status.Octave;
                var octaveRequest = (short)requestedOctave; //// this.Element.Line.MusicalOctave;
                octaveRequest = (short)((requestedOctave == MusicalOctave.SubContra) ? octaveRequest + 2 : octaveRequest);
                octaveRequest = (short)((requestedOctave == MusicalOctave.Contra) ? octaveRequest + 1 : octaveRequest);
                //// octaveRequest = (short)((octaveRequest == (short)MusicalOctave.FiveLine) ? octaveRequest - 1 : octaveRequest);
                //// 2016/12 octaveRequest+1
                for (var octave = (short)(octaveRequest - 2); octave <= octaveRequest + 1; octave++) {
                    MusicalTone mt = givenTone.Clone() as MusicalTone;
                    if (mt == null) {
                        continue;
                    }

                    mt.Pitch.SetValues(octave, e);
                    mt.ModalityIndex = (short)(ml + (octave * modalityLevel));
                    if (harmonicStruct != null) {
                        mt.HarmonicIndex = harmonicStruct.LevelOfBit(e);
                    }

                    this.ToneList.Add(mt);
                }
            }

            return this.ToneList.Any();
        }

        /// <summary> Returns the next optimal melodic tone. </summary>
        /// <param name="givenTone">Melodic tone.</param>
        /// <returns> Returns value. </returns>
        public MusicalTone OptimalNextMelTone(MusicalTone givenTone) {
            Contract.Requires(givenTone != null);
            Contract.Requires(this.Bar.CommonRhythmicShape != null);

            this.melTone = givenTone;
            this.PrepareToneClusters();

            this.singleHarmony = false;
            this.harmonicStructure = this.Bar.HarmonicBar?.PrevailingHarmonicStructure(givenTone.BitRange, out this.singleHarmony); //// (this.Bar.Number)
            var extremeTotal = -10000000f;
            MusicalTone bestMelTone = null;
            short lastToneElement = -1;
            float formalValue = 0;
            //// givenTone = mt; not possible! pointer to givenTone is in clusters!

            foreach (var mt in this.ToneList.Where(mt => mt != null)) {
                givenTone.SetMelTone(mt);
                this.Element.PrepareMelInterval();
                if (lastToneElement != mt.Pitch.Element) {
                    formalValue = TotalValue(this.formalEvaluators);
                    lastToneElement = givenTone.Pitch.Element;
                }

                if (this.Bar.CommonRhythmicShape != null) {
                    var br = givenTone.BitRange; //// (this.Bar.Number)
                    if (br != null) {
                        this.Bar.RecomputeHarmonicClusters(br);
                    }

                    givenTone.OrdinalIndex = mt.OrdinalIndex; //// if lost somewhere ?
                }

                float value = TotalValue(this.realEvaluators);
                var total = formalValue + value;
                if (this.HasTraceValues) {
                    this.TraceValues(formalValue, mt, value, total);
                } 

                if (total <= extremeTotal) {
                    continue;
                }

                extremeTotal = total;
                bestMelTone = mt;
            }

            if (bestMelTone != null) {
                ////  if (this.TraceValues) { ToneTracer.Singleton.MarkBestTone(bestMelTone, this.Element.Status.LineIndex); } 
                return bestMelTone;
            }

            var msg = string.Format(
                CultureInfo.InvariantCulture,
                "Requested tone not found in line {0}. Check harmonic modality of the line.", 
                this.Element.Line.LineIndex.ToString(CultureInfo.CurrentCulture.NumberFormat));
            throw new InvalidOperationException(msg);
        }

        #region Private Static Methods
        /// <summary>
        /// Total Value.
        /// </summary>
        /// <param name="evaluators">Set of evaluators.</param>
        /// <returns> Returns value. </returns>
        private static int TotalValue(DeterminateValue evaluators) {
            if (evaluators == null) {
                return 0;
            }

            var delegates = evaluators.GetInvocationList();
            var values = from DeterminateValue member in delegates
                            where member != null
                            select member.Invoke();
            return values.Sum();
        }
        #endregion

        /// <summary>
        /// Traces the values.
        /// </summary>
        /// <param name="formalValue">The formal value.</param>
        /// <param name="givenTone">The given tone.</param>
        /// <param name="value">The value.</param>
        /// <param name="total">The total.</param>
        private void TraceValues(float formalValue, MusicalTone givenTone, float value, float total) {
            Contract.Requires(givenTone != null);

            if (this.Element.TonePacket == null) {
                return;
            }

            var intendedTone = new IntendedTone {
                LineIndex = this.Element.Line.LineIndex,
                MusicalTone = givenTone,
                HarmonicCoverValue = this.HarmonicCoverValue(),
                HarmonicValue = this.HarmonicValue(),
                EasySingValue = this.EasySingValue(),
                FreeBandValue = this.FreeBandValue(),
                ImpulseCollisionsValue = this.ImpulseCollisionsValue(),
                MelodicCollisionsValue = this.MelodicCollisionsValue(),
                AmbitChangeValue = this.AmbitChangeValue(),
                OctaveValue = this.OctaveValue(),
                FiguralValue = this.FiguralValue(),
                VariabilityValue = this.VariabilityValue(),
                SequenceValue = this.SequenceValue(),
                TotalFormalValue = formalValue,
                TotalRealValue = value,
                TotalValue = total
            };

            this.Element.TonePacket.IntendedTones.Add(intendedTone);
        }
        #endregion

        #region Bar related values

        /// <summary> Evaluates value of harmonic cover.  </summary>
        /// <returns> Returns value. </returns>
        private int HarmonicCoverValue() {
            Contract.Requires(this.Bar.HarmonicBar.RhythmicShape != null);
            if (this.Bar.HarmonicBar.RhythmicShape == null) {
                return MusicalQuantity.NeutralValue;
            }

            var level = this.Bar.HarmonicBar.RhythmicShape.Level;
            var total = MusicalQuantity.NeutralValue;

            for (byte i = 0; i < level; i++) {
                var harStr = this.Bar.HarmonicBar.HarmonicStructureAtRhythmicLevel(i);
                if (harStr == null || this.Bar.HarmonicBar.RhythmicShape == null) {
                    continue;
                }

                var harRange = this.Bar.HarmonicBar.RhythmicShape.RangeForLevel(i);
                var harRatio = 1.0f * harRange.Order / this.Bar.Header.System.RhythmicOrder;
                var stopRange = harRange.StopAt(this.melTone.BitRange.BitTo); //// (this.Bar.Number)
                if (stopRange == null || stopRange.IsEmpty || this.Element.Tones == null) {
                    continue;
                }

                //// 2016/09 Here were melodicBits = 0!? (error)
                int melodicBits = this.Element.Tones.NumberOfMelodicBits(harStr, stopRange);
                var allowedBits = stopRange.Length * DefaultValue.HalfUnit;
                var isSatisfied = !(melodicBits > allowedBits);

                //// The harmonic values are more important then free band (10*)
                if (isSatisfied) { ///// this.harmonicStructure.IsOn(this.melTone.Pitch.Element)) {
                    var quotient = (float)melodicBits / harRange.Order;
                    total += (int)(10 * MusicalQuantity.ForcedValue * (1 - quotient) * harRatio); //// ForcedValue
                }
                else {
                    var quotient = (float)melodicBits / harRange.Order;
                    total += (int)(10 * MusicalQuantity.HarmfulValue * quotient * harRatio); //// HarmfulValue
                }
            }

            total /= level + 1; //// 2014/01
            return total;
            //// if (!isSatisfied) { return MusicalQuantity.PoorValue; }
            //// return MusicalQuantity.NeutralValue; 
        }

        /// <summary> Evaluates value of free harmonic bands. </summary>
        /// <returns> Returns value. </returns>
        private int FreeBandValue() {
            Contract.Requires(this.melTone != null);
            //// Contract.Requires(this.MusicalTone.BitRange != null);
            if (this.melTone.Duration == 0) {
                return MusicalQuantity.NeutralValue;
            }

            var rangeMelodic = 0;
            var rangeMelodicExists = 0;
            var rangeTones = 0;
            //// int rangeTotal = 0;

            foreach (var harCluster in this.ToneClusters) {
                if (harCluster == null) {
                    continue;
                }

                var bitTotal = harCluster.ToneList.Count;
                var bitTones = harCluster.NumberOfTrueTones();
                var bitEqualTones = harCluster.NumberOfEqualTones(this.melTone.Pitch.Element);
                if (bitTotal > 1) { //// 2008/07  
                    if (bitEqualTones > 1) { //// 2013 (test >0)
                        rangeMelodicExists += harCluster.CurrentEffectiveLength;
                    }
                }

                //// rangeTotal += bitTotal * harCluster.CurrentEffectiveLength;
                rangeTones += bitTones * harCluster.CurrentEffectiveLength;
                rangeMelodic += bitEqualTones * harCluster.CurrentEffectiveLength;
            }

            //// pitches not existing in sound are preferred
            var existRatio = this.MusicalTone.Duration > 0 ? (float)rangeMelodicExists / this.MusicalTone.Duration : 1.0f;
            //// values increased because of importance
            if (existRatio < DefaultValue.QuarterUnit) {
                return MusicalQuantity.VeryNiceValue;
            }

            //// pitches existing, but rare are preferred 2008/12
            //// float equalRatio = rangeTotal > 0 ? (float)rangeMelodic / rangeTotal : DefaultValue.HalfUnit;
            var equalRatio = rangeTones > 0 ? (float)rangeMelodic / rangeTones : DefaultValue.HalfUnit;

            //// if (existRatio > DefaultValue.HalfUnit) { //// 2013  && occurenceRatio > DefaultValue.HalfUnit
            //// For strict 4-part harmonization - quotient>1  needed !?
            const int quotient = 3; //// 2016/08 was 1, 2015 was 4
            return (int)(quotient * MusicalQuantity.PoorValue * (existRatio + equalRatio));
            //// }
            //// return (int)(MusicalQuantity.GoodValue * (1 - occurenceRatio));   //// 2013 test, MusicalQuantity.GoodValue
        }

        /// <summary> Compute value of impulse collisions of the given tone.  </summary>
        /// <returns> Returns value. </returns>
        private int ImpulseCollisionsValue() {
            //// to favor - if the dissonance is prepared by the previous chord
            //// var test = this.ToneClusters.FirstOrDefault();
            //// if (test!=null && test.ValidToneList.Count >=2) {
            //// float c = test.RealConsonance;    float i = test.RealImpulse;      float j = test.RealImpulse }
            var value = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var harCluster in this.ToneClusters) {
                if (harCluster == null || !harCluster.IsChord) {
                    continue;
                }

                var r = harCluster.RealEnergy.Impulse - DefaultValue.Fifty;
                if (r > 0) {
                    value += (int)(r * MusicalQuantity.PoorValue * harCluster.CurrentEffectiveLength); //// BadValue
                }
            }

            //// 2016 value = value / this.Bar.Header.System.RhythmicOrder;
            return value;
            //// (harCluster.RealConsonance < DefaultValue.Fifty) //// PoorValue
        }

        /// <summary> Compute value of non-harmonic collisions of the given tone. </summary>
        /// <returns> Returns value. </returns>
        private int MelodicCollisionsValue() {
            //// return 0;
            //// +to favor - if the dissonance is prepared by the previous one
            var total = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var harCluster in this.ToneClusters) {
                if (harCluster == null || !harCluster.IsChord) {
                    continue;
                }

                var melodicTones = harCluster.MelodicTones();
                if (melodicTones == null || melodicTones.Count <= 1) {
                    continue;
                }

                total += harCluster.CurrentEffectiveLength;
            }

            return total * MusicalQuantity.PoorValue; //// 2016 / this.Bar.Header.System.RhythmicOrder;
        }

        /// <summary> Compute value of cluster Ambit change. </summary>
        /// <returns> Returns value. </returns>
        private int AmbitChangeValue() {
            const float epsilon = 0.001f;
            var ambitChange = MusicalQuantity.NeutralValue;
            if (this.RecentCluster == null) {
                return ambitChange;
            }

            float lastAmbit = this.RecentCluster.FormalAmbit;
            float ambit = this.MainToneCluster.FormalAmbit;
            if (Math.Abs(ambit - lastAmbit) > epsilon) {
                ambitChange += MusicalQuantity.GoodValue;
            }

            return ambitChange;
        }

        #endregion

        #region Tone related values

        /// <summary> Determine value of tone according to correspondence with harmony. </summary>
        /// <returns> Returns value.</returns>
        private int HarmonicValue() {
            Contract.Requires(this.Element.Status != null);
            if (this.harmonicStructure == null) { //// || !this.Element.Status.IsHarmonic
                return MusicalQuantity.NeutralValue;
            }

            var total = MusicalQuantity.NeutralValue;
            if (this.singleHarmony) {
                total = !this.harmonicStructure.IsOn(this.MusicalTone.Pitch.Element) ? MusicalQuantity.HarmfulValue : MusicalQuantity.VeryNiceValue;
            }
            else {
                var level = this.Bar.HarmonicBar.RhythmicShape.Level;
                for (byte i = 0; i < level; i++) {
                    var harStr = this.Bar.HarmonicBar.HarmonicStructureAtRhythmicLevel(i);
                    if (harStr == null || this.Bar.HarmonicBar.RhythmicShape == null) {
                        continue;
                    }

                    var harRange = this.Bar.HarmonicBar.RhythmicShape.RangeForLevel(i);
                    var harRatio = 1.0f * harRange.Order / this.Bar.Header.System.RhythmicOrder;
                    var interRange = harRange.IntersectionWith(this.melTone.BitRange); //// (this.Bar.Number)
                    if (interRange == null || interRange.IsEmpty || this.Element.Tones == null) {
                        continue;
                    }

                    int melodicBits = this.Element.Tones.NumberOfMelodicBits(harStr, harRange);
                    float neededHarmonicBits = harRange.Length;

                    //// The harmonic values are more important then free band (10*)
                    if (melodicBits == 0) { ///// this.harmonicStructure.IsOn(this.melTone.Pitch.Element)) {
                        var quotient = neededHarmonicBits / harRange.Order;
                        total += (int)(10 * MusicalQuantity.ForcedValue * quotient * harRatio);
                    }
                    else {
                        var quotient = (float)melodicBits / harRange.Order;
                        total += (int)(10 * MusicalQuantity.HarmfulValue * quotient * harRatio);
                    }
                }
            }

            //// 2006/06 increased 20 times because of free band increase
            return total; //// > 0.0f ? MusicalQuantity.ForcedValue : total;
        }
        #endregion

        #region Interval related values

        /// <summary> Property evaluating how easy it is to sing this interval. </summary>
        /// <returns> Returns value. </returns>
        private int EasySingValue() {
            if (this.Element.Status.CurrentMelInterval == null) {
                return MusicalQuantity.NeutralValue;
            }

            var val = this.Element.Status.CurrentMelInterval.EasySing;
            return val > 0.0f ? MusicalQuantity.NiceValue : MusicalQuantity.PoorValue;
        }

        #endregion

        #region Sequence values
        /// <summary>
        /// Sequences the value.
        /// </summary>
        /// <returns> Returns value. </returns>
        private int SequenceValue() {
            return this.MelodicSequence.SequenceValue();
        }

        /// <summary> Value of variability. </summary>
        /// <returns> Returns value. </returns>
        private int VariabilityValue() {
            //// to favor tones that differ from previous ones
            //// just simple test, cycle is time consuming ...
            //// p += Bar.CountOfUsedPitch(CurrentTone.Pitch);   n += Bar.NumberOfTones();
            if (this.Line.LastTone == null || this.Line.PenultTone == null
                || this.Line.CurrentTone == null
                || this.Line.LastTone.Pitch == null || this.Line.PenultTone.Pitch == null) {
                return MusicalQuantity.NeutralValue;
            }

            var i1 = this.Line.LastTone.Pitch.IntervalFrom(this.Line.PenultTone.Pitch);
            var i2 = this.Line.CurrentTone.Pitch.IntervalFrom(this.Line.LastTone.Pitch);

            int v = MusicalQuantity.NeutralValue;
            var fi1 = this.Element.Bar.Header.System.HarmonicSystem.FormalMedianLength(i1);
            var fi2 = this.Element.Bar.Header.System.HarmonicSystem.FormalMedianLength(i2);
            if (fi1 == 0)
            {
                v += MusicalQuantity.PoorValue;
            }
            else
            {
                v += MusicalQuantity.VeryNiceValue;                
            }

            if (fi2 == 0) {
                v += MusicalQuantity.PoorValue;
            }
            else {
                v += MusicalQuantity.VeryNiceValue;
            }

            return v;
        }

        #endregion

        #region Other Values
        /// <summary> Determine tone value. </summary>
        /// <returns> Returns value. </returns>
        private int FiguralValue() {
            Contract.Requires(this.Element.Status != null);
            //// 2016/10  Contract.Requires(this.Element.Status.PlannedTones != null);
            Contract.Requires(this.Element.MusicalLine.CurrentTone != null);
            //// const int limitDiff = 4; //// 4;
            var tones = this.Element.Status.MelodicPlan.PlannedTones;
            if (tones == null || tones.Count == 0) {
                return MusicalQuantity.NeutralValue;
            }

            //// if (!this.Element.Status.HasMelodicMotive) {  return MusicalQuantity.NeutralValue; }
            //// int cnt = this.Element.Line.MusicalTones.Count;
            //// int plannedCount = this.Element.Line.PlannedTones.Count;
            //// if (cnt == plannedCount) { 
            var mt1 = this.Line.CurrentTone; //// MusicalTones[cnt - 1] as MusicalTone;
            if (mt1 == null) {
                return MusicalQuantity.NeutralValue;
            }

            var mt0 = this.Line.LastTone; //// this.Element.Tones.Last(); //// 2016 Status.PreviousTone(mt1); //// MusicalTones[cnt - 1] as MusicalTone;
            if (mt0 == null) {
                return MusicalQuantity.NeutralValue;
            }

            int idx0 = 0, idx1 = 0;
            if (this.Element.Tones != null) {
                idx0 = mt0.OrdinalIndex; //// this.Element.Tones.IndexOf(mt0);
                idx1 = mt1.OrdinalIndex; //// this.Element.Tones.IndexOf(mt1);
            }

            if (idx0 < 0 || idx0 >= tones.Count || idx1 < 0 || idx1 >= tones.Count) {
                return MusicalQuantity.NeutralValue;
            }

            return this.EvaluateFiguralValue(mt1, mt0, idx0, idx1);
        }

        /// <summary>
        /// Evaluates the figural value.
        /// </summary>
        /// <param name="mt1">The MT1.</param>
        /// <param name="mt0">The MT0.</param>
        /// <param name="idx0">The index 0.</param>
        /// <param name="idx1">The index 1.</param>
        /// <returns> Returns value. </returns>
        private int EvaluateFiguralValue(MusicalTone mt1, MusicalTone mt0, int idx0, int idx1) {
            Contract.Requires(mt0 != null);
            Contract.Requires(mt1 != null);
            if (!(this.Element.Status.MelodicPlan.PlannedTones[idx0] is MusicalTone pmt0) || mt0.IsEmpty || pmt0.IsEmpty 
                    || mt1 == null || !(this.Element.Status.MelodicPlan.PlannedTones[idx1] is MusicalTone pmt1) || mt1.IsEmpty ||
                pmt1.IsEmpty) {
                return MusicalQuantity.NeutralValue;
            }

            var distance = mt1.Pitch.SystemAltitude - mt0.Pitch.SystemAltitude;
            var plannedDistance = pmt1.Pitch.SystemAltitude - pmt0.Pitch.SystemAltitude;
            var diff = Math.Abs(distance - plannedDistance);
            //// if (diff > limitDiff) { return MusicalQuantity.NeutralValue;  } 

            var value = MusicalQuantity.VeryNiceValue; //// NiceValue
            value += Math.Sign(distance) == Math.Sign(plannedDistance) ? MusicalQuantity.VeryNiceValue : MusicalQuantity.NiceValue;

            if (diff > 0) {
                value -= diff; //// * 10; //// Avoid multiple or conditional return statements.
            }

            //// 2016/12 multiply by 5 //// 2017/03 by 10
            return value * 10;
        }

        /// <summary> Determine tone value. </summary>
        /// <returns> Returns value. </returns>
        private int OctaveValue() {
            var octave = this.Element.Status.Octave;
            var requestedOctave = (short)octave;
            var mt1 = this.Element.MusicalLine.CurrentTone; //// this.MusicalTone
            if (mt1 == null) {
                return MusicalQuantity.NeutralValue;
            }

            if (mt1.Pitch.Octave == requestedOctave) {
                return MusicalQuantity.VeryNiceValue; //// NiceValue;
            }

            int order = mt1.Pitch.HarmonicSystem.Order;
            var requestedAltitude = (requestedOctave + 0.5f) * order;
            var altitudeDiff = Math.Abs(this.MusicalTone.Pitch.SystemAltitude - requestedAltitude);

            var mt0 = this.Line.LastTone;
            if (mt0?.Pitch == null) {
                return altitudeDiff > order ? MusicalQuantity.PoorValue : MusicalQuantity.NeutralValue;
            }

            altitudeDiff = Math.Abs(mt1.Pitch.SystemAltitude - mt0.Pitch.SystemAltitude);
            if (altitudeDiff > order) {
                return MusicalQuantity.PoorValue;
            }

            return altitudeDiff > order ? MusicalQuantity.PoorValue : MusicalQuantity.NeutralValue;
        }

        //// melodic, harmonic, rhythmic variability ... 
        #endregion

        #region Prepare Evaluators
        /// <summary>
        /// Prepare Formal Evaluators.
        /// </summary>
        /// <returns> Returns value. </returns>
        private DeterminateValue PrepareFormalEvaluators() {
            DeterminateValue dv = null;
            var harmonicRule = false;

            //// Combined rules
            if (this.MusicalRules.RuleSimpleHarmony > 0 || this.LineRules.RuleToneHarmonic > 0) {
                dv += this.HarmonicValue;
                harmonicRule = true;
            }

            //// Engine rules
            if (!harmonicRule && this.MusicalRules.RuleHarmonicCover > 0) { //// (formalRequest.HasItem(GenProperty.ToneHarmonicCover)) {
                dv += this.HarmonicCoverValue;
            }

            //// Line type rules

            if (this.LineRules.RuleIntervalEasySing > 0) {
                dv += this.EasySingValue;
            }

            if (this.MusicalRules.RuleFreeBand > 0) { //// (realRequest.HasItem(GenProperty.ClusterFreeBand)) {
                dv += this.FreeBandValue;
            }

            //// if (this.LineRules.RuleToneRoot > 0) {  dv += this.RootValue;  } 
            return dv;
        }

        /// <summary>
        /// Prepare Real Evaluators.
        /// </summary>
        /// <returns> Returns value. </returns>
        private DeterminateValue PrepareRealEvaluators() {
            DeterminateValue dv = null;

            //// Engine rules

            if (this.MusicalRules.RuleImpulseCollisions > 0) { ////  (realRequest.HasItem(GenProperty.ClusterImpulseCollisions)) {
                dv += this.ImpulseCollisionsValue;
            }

            //// Moved to formals - RuleFreeBand and RuleParallelConnections
            if (this.MusicalRules.RuleMelodicCollisions > 0) { ////  (realRequest.HasItem(GenProperty.ClusterMelodicCollisions)) {
                dv += this.MelodicCollisionsValue;
            } 

            if (this.MusicalRules.RuleAmbitChange > 0) { //// (realRequest.HasItem(GenProperty.ClusterAmbitChange)) {
                dv += this.AmbitChangeValue;
            }

            //// Line type rules
            dv += this.OctaveValue;
            //// dv += new DeterminateValue(this.AltitudeValue);
            if (this.LineRules.RuleMelodicVariability > 0) {
                dv += this.VariabilityValue;
            }

            if (this.LineRules.RuleMelodicFigural > 0) {
                dv += this.FiguralValue;
            }

            if (this.LineRules.MelodicShape != MelodicShape.None) {
                dv += this.SequenceValue;
            }

            return dv;
        }

        #endregion

        #region Other private methods
        /// <summary>
        /// Prepare Tone Clusters.
        /// </summary>
        private void PrepareToneClusters() {
            this.ToneClusters = new List<HarmonicCluster>();
            var bitFrom = this.MusicalTone.BitFrom;
            if (bitFrom > 0) {
                var recentBit = (byte)(bitFrom - 1);
                this.RecentCluster = this.Bar.HarmonicClusterAtTick(recentBit);
            }

            this.MainToneCluster = this.Bar.HarmonicClusterAtTick(bitFrom);
            var bitTo = this.MusicalTone.BitTo;
            HarmonicCluster lastHarCluster = null;
            for (var t = bitFrom; t <= bitTo; t++) {
                HarmonicCluster harCluster;
                if (lastHarCluster != null && t < (lastHarCluster.Tick + lastHarCluster.Duration)) {
                    harCluster = lastHarCluster;
                    harCluster.CurrentEffectiveLength++;
                }
                else {
                    harCluster = this.Bar.HarmonicClusterAtTick(t);
                    if (harCluster == null) {
                        continue;
                    }

                    harCluster.CurrentEffectiveLength = 1;
                    this.ToneClusters.Add(harCluster);
                    lastHarCluster = harCluster;
                }
            }
        }

        /// <summary>
        /// Prepare Evaluators.
        /// </summary>
        private void PrepareEvaluators() {
            this.formalEvaluators = this.PrepareFormalEvaluators();
            this.realEvaluators = this.PrepareRealEvaluators();
        }
        #endregion
    }
}
