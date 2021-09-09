// <copyright file="HarmonicSystem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Music
{
    /// <summary> Harmonic system. </summary>
    /// <remarks> Harmonic system is subclass of binary GSystem. It is defined by its Order, that represents number
    /// of tones in one octave. In addition to that, array of symbols (c,c#,d,..)
    /// and array  of (formal) intervals (0,1,2,..) are created for given Order.  </remarks>  
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicSystem : GeneralSystem {
        #region Fields
        /// <summary>
        /// Naming base 24.
        /// </summary>
        public const byte NamingBase24 = 24;

        /// <summary>
        /// Naming base 48.
        /// </summary>
        public const byte NamingBase48 = 48;

        /// <summary> Maximal Continuity Quantum (3). </summary>
        public const float C1 = 6.0f;

        /// <summary> Continuity Quantum (5). </summary>
        public const float C2 = 3.0f;

        /// <summary> Continuity Quantum (7). </summary>
        public const float C3 = 1.0f;

        /// <summary> Maximal Impulse Quantum (halftone). </summary>
        public const float I1 = 12.0f;

        /// <summary> Consonance quotient. </summary>
        public const float ConsonanceRatio = 4.0f;

        /// <summary> Constant determining size of interval store. This optimization is switched off now. </summary>
        public const int MaximumLengthOfStoredRealInterval = 0;

        /// <summary> Used Systems. </summary>
        private static readonly Dictionary<byte, HarmonicSystem> UsedSystems = new Dictionary<byte, HarmonicSystem>();

        /// <summary> Consonance quotient. </summary>
        private readonly Dictionary<int, float> intervalRatios;

        /// <summary> Musical sharp symbols. </summary>
        private readonly string[] sharpSymbols;

        /// <summary> Musical sharp symbols. </summary>
        private readonly string[] flatSymbols;

        /// <summary> List of intervals. </summary>
        private Collection<MusicalPitch> pitches;

        /// <summary> List of intervals. </summary>
        private Collection<HarmonicInterval> intervals;

        //// <summary> Shortcuts of harmonic structures. </summary>
        //// private Dictionary<string, string> harStructShortcuts;

        //// <summary> Shortcuts of harmonic structures. </summary>
        //// private Dictionary<string, string> harStructShortcutsMoot;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicSystem class.  Serializable. </summary>
        public HarmonicSystem() {
        }

        /// <summary> Initializes a new instance of the HarmonicSystem class. </summary>
        /// <param name="order">Order of system.</param>
        public HarmonicSystem(byte order)
            : base(2, order) {
                if (order == 0) {
                    throw new ArgumentException("Order of system must not be 0.");
                }

                this.intervalRatios = new Dictionary<int, float>();
                this.sharpSymbols = this.MakeSymbolArray(true);
                this.flatSymbols = this.MakeSymbolArray(false);
                this.StringOfSharpSymbols = SymbolsToString(this.sharpSymbols);
                this.StringOfFlatSymbols = SymbolsToString(this.flatSymbols);
                this.MakePitches();
                this.MakeIntervals();
        }

        /// <summary> Initializes a new instance of the HarmonicSystem class. </summary>
        /// <param name="degree">Degree of system.</param>
        /// <param name="order">Order of system.</param>
        public HarmonicSystem(byte degree, byte order)
            : base(degree, order) {
        }

        #endregion

        #region Properties
        /// <summary> Gets list of pitches. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<MusicalPitch> Pitches {
            get {
                Contract.Ensures(Contract.Result<Collection<MusicalPitch>>() != null);
                if (this.pitches == null) {
                    throw new InvalidOperationException("List of pitches is null.");
                }

                return this.pitches;
            }
        }

        /// <summary> Gets list of intervals. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<HarmonicInterval> Intervals {
            get {
                Contract.Ensures(Contract.Result<Collection<HarmonicInterval>>() != null);
                if (this.intervals == null) {
                    throw new InvalidOperationException("List of intervals is null.");
                }

                return this.intervals;
            }
        }

        /// <summary> Gets or sets string of musical symbols. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public string StringOfSharpSymbols { get; set; }

        /// <summary> Gets or sets string of musical symbols. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public string StringOfFlatSymbols { get; set; }

        /// <summary>
        /// Gets the chromatic modality.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicModality ChromaticModality {
            get {
                //// long chromatic = musicalBar.FileModel.MusicalBlock.HarmonicSystem.LongSize() - 1; // (long)(Math.Pow(2, this.Order) - 1);
                var order = this.Order;
                var chromaticCode = "0";  //// new byte[order + 2]; // chromaticCode[0] = 0; chromaticCode[1] = (byte)'#';
                for (var i = 0; i < order; i++) {
                    chromaticCode += ",1";
                }

                var modality = HarmonicModality.GetNewHarmonicModality(this, chromaticCode.ToString(CultureInfo.CurrentCulture));
                return modality;
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Gets Harmonic System.
        /// </summary>
        /// <param name="order">System order.</param>
        /// <returns> Returns value. </returns>
        public static HarmonicSystem GetHarmonicSystem(byte order) {
            Contract.Ensures(Contract.Result<HarmonicSystem>() != null);

            var hm = UsedSystems.ContainsKey(order) ? UsedSystems[order] : null;
            if (hm != null) {
                return hm;
            }

            hm = new HarmonicSystem(order);
            UsedSystems[order] = hm;

            return hm;
        }

        /// <summary>
        /// Formal measure of dissonance, higher for consonant structures.
        /// </summary>
        /// <param name="continuity">Harmonic Continuity.</param>
        /// <param name="impulse">Harmonic Impulse.</param>
        /// <returns> Returns value. </returns>
        public static float Consonance(float continuity, float impulse) {
            // float formalConsonance = aFContinuity-HarmonicSystem.ciSonanceRatio*aFImpulse;
            // float num = formalConsonance+HarmonicSystem.ciSonanceRatio*HarmonicSystem.i1;
            // float denominator = aFContinuity+HarmonicSystem.ciSonanceRatio*HarmonicSystem.i1;
            // formalConsonance = num/denominator*100f; // normalized to 0..100
            var formalConsonance = (Math.Abs(continuity) + (ConsonanceRatio * (100 - impulse))) / (ConsonanceRatio + 1);
            return formalConsonance;
        }

        #endregion

        #region Public methods
        /// <summary> Returns ratio of the interval. </summary>
        /// <param name="sysLength">Real system length.</param>
        /// <returns> Returns value. </returns>
        public float RatioForInterval(int sysLength) {
            float ratio;
            if (sysLength == 0) {
                return 1.0f;
            }

            if (sysLength == this.Order) {
                return 2.0f;
            }

            if (sysLength == -this.Order) {
                return DefaultValue.HalfUnit;
            }

            if (this.intervalRatios.ContainsKey(sysLength)) {
                ratio = this.intervalRatios[sysLength];
            }
            else {
                ratio = (float)Math.Pow(2.0, ((float)sysLength) / this.Order);
                this.intervalRatios[sysLength] = ratio;
            }

            return ratio;
        }

        /// <summary>
        /// Gets musical pitch.
        /// </summary>
        /// <param name="systemAltitude">System altitude.</param>
        /// <returns> Returns value. </returns>
        public MusicalPitch GetPitch(int systemAltitude) {
            if (systemAltitude < 0) {
                return null;
            }

            return systemAltitude < this.Pitches.Count ? this.Pitches[systemAltitude] : null;
        }
        #endregion

        #region MIDI support
        /// <summary> Returns interval Size in halftones, because of MIDI. </summary>
        /// <param name="sysLength">Real system length.</param>
        /// <returns> Returns value. </returns>
        public float HalftonesForInterval(int sysLength) {
            if (this.Order == DefaultValue.HarmonicOrder) {
                return sysLength;
            }

            var ratio = this.RatioForInterval(sysLength);
            var r1 = (float)Math.Pow(2.0, 1.0 / DefaultValue.HarmonicOrder);
            var lgr1 = (float)Math.Log(r1);
            var halftones = lgr1 >= DefaultValue.AfterZero ? (float)Math.Round(Math.Log(ratio) / lgr1, 4) : 0;

            return halftones;
        }
        #endregion

        #region Intervals
        /// <summary> Returns tone index of the given interval. </summary>
        /// <param name="baseNumber">Number of tone symbols.</param>
        /// <param name="formalLength">Formal system length.</param>
        /// <returns> Returns value. </returns>
        public short GuessToneIndex(byte baseNumber, byte formalLength) {
            const float afterZero = 0.0001f;
            Contract.Requires(baseNumber > 0);
            var ratio = this.RatioForInterval(formalLength);
            var discreteRatio = (float)Math.Pow(2.0, DefaultValue.HalfUnit / baseNumber);
            var limit = discreteRatio + afterZero;
            for (byte i = 0; i < baseNumber; i++) {
                discreteRatio = (float)Math.Pow(2.0, ((float)i) / baseNumber);
                if (MathSupport.EqualNumbersRational(ratio, discreteRatio, limit)) {
                    return i;
                }
            }

            return -1;
        }

        /// <summary> Returns name for the given interval. </summary>
        /// <param name="formalLength">Formal system length.</param>
        /// <returns> Returns value. </returns>
        public string GuessNameForInterval(byte formalLength) {
            string[] name24 = {
                "unison",
                "dim.second", "min.second", "mid.second", "maj.second", "aug.second",
                "min.third", "mid.third", "maj.third",
                "dim.fourth", "fourth", "aug.fourth",
                "triton",
                "dim.fifth", "fifth", "aug.fifth",
                "min.sixth", "mid.sixth", "maj.sixth",
                "dim.seventh", "min.seventh", "mid.seventh", "maj.seventh", "aug.seventh" };
            var toneIndex = this.GuessToneIndex(NamingBase24, formalLength);
            var value = toneIndex >= 0 && toneIndex < name24.Length ? name24[toneIndex] : "?" + formalLength.ToString(CultureInfo.CurrentCulture.NumberFormat);

            return value;
        }
        #endregion

        #region Substructures
        /// <summary>
        /// Modality Classes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicModality> ModalityClasses(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewHarmonicModalityVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        genQualifier,
                                        limit);
            return new Collection<HarmonicModality>(hv.StructList);
        }

        /// <summary>
        /// Modality Classes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicModality> ModalityClasses() {
            var hv = StructuralVarietyFactory.NewHarmonicModalityVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        null,
                                        10000);
            return new Collection<HarmonicModality>(hv.StructList);
        }

        /// <summary>
        /// Modality Instances.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicModality> ModalityInstances(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewHarmonicModalityVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Modality Instances.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicModality> ModalityInstances() {
            var hv = StructuralVarietyFactory.NewHarmonicModalityVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Classes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> StructClasses(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewHarmonicStructuralVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Classes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> StructClasses() {
            var hv = StructuralVarietyFactory.NewHarmonicStructuralVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Instances.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> StructInstances(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewHarmonicStructuralVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Instances.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> StructInstances() {
            var hv = StructuralVarietyFactory.NewHarmonicStructuralVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }
        #endregion

        #region String representation

        /// <summary>
        /// Returns symbols for given element.
        /// </summary>
        /// <param name="element">Xml Element.</param>
        /// <param name="sharp">If set to <c>true</c> [sharp].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string Symbol(short element, bool sharp) {
            var symbols = sharp ? this.sharpSymbols : this.flatSymbols;
            if (symbols != null && element >= 0 && element < symbols.Length) {
                return symbols[element];
            }

            var formalElement = element % this.Order;
            if (formalElement < 0) {
                formalElement += this.Order;
            }

            var s = string.Empty;
            if (symbols != null && (formalElement >= 0 && formalElement < symbols.Length)) {
                return symbols[formalElement];
            }

            s = element < 0 ? s.ToUpper(CultureInfo.CurrentCulture) : s + "'";
            return s;
        }

        /// <summary>
        /// Determines whether the specified element is enharmonic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>True</c> if the specified element is enharmonic; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnharmonic(short element) {
            var s = this.sharpSymbols[element];

            return s?.Trim().Length > 1;
        }

        /// <summary>
        /// Returns tone symbol for the given element.
        /// </summary>
        /// <param name="element">Requested element.</param>
        /// <param name="sharp">If set to <c>true</c> [sharp].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string GuessSymbolForElement(byte element, bool sharp) {
            var value = "?";
            const char b = 'b'; //// (char)0x0185;
            if (this.Order <= NamingBase24) {
                string[] sym24Sharp = { "c", "c+", "c#", "cx", "d", "d+", "d#", "dx", "e", "e+", "f", "f+",
                                     "f#", "fx", "g", "g+", "g#", "gx", "a", "a+", "a#", "ax", "h", "h+" };
                string[] sym24Flat = { "c", "c+", "d" + b, "d-", "d", "d+", "e" + b, "e-", "e", "e+", "f", "f+",
                                     "g" + b, "g-", "g", "g+", "a" + b, "a-", "a", "a+", "b", "h-", "h", "h+" };
                var toneIndex = this.GuessToneIndex(NamingBase24, element);
                if (toneIndex >= 0 && toneIndex < NamingBase24) {
                    value = sharp ? sym24Sharp[toneIndex] : sym24Flat[toneIndex];
                }
            }
            else {
                string[] sym48Sharp = { "c", "c'", "c+", "c+'", "c#", "c#'", "cx", "cx'", "d", "d'", "d+", "d+'", 
                                     "d#", "d#'", "dx", "dx'", "e", "e'", "e+", "e+'", "f", "f'", "f+", "f+'", 
                                     "f#", "f#'", "fx", "fx'", "g", "g'", "g+", "g+'", "g#", "g#'", "gx", "gx'", 
                                     "a", "a'", "a+", "a+'", "a#", "a#'", "ax", "ax'", "h", "h'", "h+", "h+'" };
                var toneIndex = this.GuessToneIndex(NamingBase48, element);
                if (toneIndex >= 0 && toneIndex < NamingBase48) {
                    value = sym48Sharp[toneIndex];
                }
            }

            return value;
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append("Harmonic system\r\n");
            s.Append(base.ToString() + Environment.NewLine);
            foreach (var hi in this.Intervals.Where(hi => hi != null)) {
                s.Append(hi + Environment.NewLine);
            }

            return s.ToString();
        }

        #endregion

        #region Private static methods
        /// <summary>
        /// Symbols the array to string.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns> Returns value. </returns>
        private static string SymbolsToString(string[] symbols) {
            var str = new StringBuilder();
            Array.ForEach(
                symbols,
                s => {
                    str.Append(s);
                    str.Append(",");
                });

            return str.ToString();
        }
        #endregion

        #region Private methods
        /// <summary> Makes array of pitch objects. </summary>
        private void MakePitches() {
            this.pitches = new Collection<MusicalPitch>();
            for (byte i = 0; i < 128; i++) {
                var mp = new MusicalPitch(this, i);
                this.Pitches.Add(mp);
            }
        }

        /// <summary> Makes array of interval objects. </summary>
        private void MakeIntervals() {
            this.intervals = new Collection<HarmonicInterval>();
            for (byte i = 0; i < this.Order; i++) {
                var hI = new HarmonicInterval(this, i, 1.0f);
                this.Intervals.Add(hI);
            }
        }

        /// <summary>
        /// Makes array of symbols used in this GSystem.
        /// </summary>
        /// <param name="sharp">If set to <c>true</c> [sharp].</param>
        /// <returns> Returns value. </returns>
        private string[] MakeSymbolArray(bool sharp) {
            var str = new StringBuilder();
            var symbols = new string[this.Order];
            for (byte i = 0; i < this.Order; i++) {
                var s = this.GuessSymbolForElement(i, sharp);
                str.Append(s);
                if (i < this.Order - 1) {
                    str.Append(",");
                }

                if (i < symbols.Length) {
                    symbols[i] = s;
                }
            }

            return symbols;
        }
        #endregion
    }
}