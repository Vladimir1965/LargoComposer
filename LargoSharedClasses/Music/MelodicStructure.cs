// <copyright file="MelodicStructure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;

namespace LargoSharedClasses.Music
{
    /// <summary> Melodic fragment. </summary>
    /// <remarks> Melodic fragment is a subclass of figural structure. Two types of figures are distinguished:
    /// modal (tones step within current harmonic modality) and harmonic (tones in actual harmonic structure).
    /// Melodic fragment is defined by its pattern. In addition to that some characteristics 
    /// (like variability, stability or homogeneity...) are planned to be computed. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class MelodicStructure : FiguralSchema, IModalStruct
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MelodicStructure class.  Serializable. </summary>
        public MelodicStructure() {
            this.ToneSchema = this.SymbolStringOfMode(0);
        }

        /// <summary>
        /// Initializes a new instance of the MelodicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public MelodicStructure(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
            //// if (givenSystem == null) { return; }
            //// if (givenSystem.Order == 0) { throw new ArgumentException("Order of MelodicStructure not defined."); }
            //// HarmonicSystem hs = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            //// this.HarmonicModality = HarmonicModality.GetNewHarmonicModality(hs, 2741);
        }

        /// <summary>
        /// Initializes a new instance of the MelodicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public MelodicStructure(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, number) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary> Initializes a new instance of the MelodicStructure class. </summary>
        /// <param name="structure">Figural structure.</param>
        public MelodicStructure(FiguralStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
        }
        #endregion

        #region Interface - simple properties
        /// <summary> Gets or sets the starting drift of the structure. </summary>
        /// <value> Property description. </value>
        public int Drift { get; set; }

        /// <summary> Gets or sets the starting octave of the structure. </summary>
        /// <value> Property description. </value>
        public MusicalOctave Octave { get; set; }

        /// <summary> Gets or sets the starting band type of the structure. </summary>
        /// <value> Property description. </value>
        public MusicalBand BandType { get; set; }

        /// <summary> Gets or sets the starting guessed part type of the structure. </summary>
        /// <value> Property description. </value>
        public MelodicFunction MelodicFunction { get; set; }

        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        public int BarNumber { get; set; }
        #endregion

        #region Interface - object properties
        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public MelodicSystem MelodicSystem => (MelodicSystem)this.GSystem;

        /// <summary> Gets or sets harmonic modality. </summary>
        /// <value> Property description. </value>
        public HarmonicModality HarmonicModality {
            get => (HarmonicModality)this.Modality;
            set => this.Modality = value;
        }

        /// <summary> Gets schema of tones. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema { get; private set; }

        /// <summary> Gets direction. </summary>
        /// <value> Property description. </value>
        public float Direction { get; private set; }

        #endregion

        #region Properties
        /// <summary>
        /// Gets MelodicTypeString.
        /// </summary>
        /// <value> Property description. </value>
        public string MelodicTypeString => LocalizedMusic.String("MelodicFunction" + ((byte)this.MelodicFunction).ToString(CultureInfo.CurrentCulture));

        /// <summary>
        /// Gets BandTypeString.
        /// </summary>
        /// <value> Property description. </value>
        public string OctaveString => LocalizedMusic.String("Octave" + ((byte)this.Octave).ToString(CultureInfo.CurrentCulture));

        #endregion

        #region Static factory methods
        /// <summary>
        /// Get New Melodic Struct.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MelodicStructure GetNewMelStruct(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var ms = new MelodicStructure(givenSystem, structuralCode);
            ms.DetermineBehavior();
            return ms;
        }
        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object obj) {
            return obj is MelodicStructure ms ? string.Compare(this.PositiveElementSchema, ms.PositiveElementSchema, StringComparison.Ordinal) : 0;
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a MelodicStructure");
        }

        /// <summary> Test of equality. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override bool Equals(object obj) {
            //// check null (this pointer is never null in C# methods)
            if (object.ReferenceEquals(obj, null)) {
                return false;
            }

            if (object.ReferenceEquals(this, obj)) {
                return true;
            }

            if (this.GetType() != obj.GetType()) {
                return false;
            }

            return this.CompareTo(obj) == 0;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Get Hash Code.
        /// </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode() {
            const byte hashBase = 17;
            const byte hashQuotient = 23;
            unchecked {
                int result = hashBase;
                result = (result * hashQuotient) + (this.ToneSchema != null ? this.ToneSchema.GetHashCode() : 0);
                result = (result * hashQuotient) + this.Drift.GetHashCode();
                result = (result * hashQuotient) + this.Octave.GetHashCode();
                result = (result * hashQuotient) + this.BandType.GetHashCode();
                result = (result * hashQuotient) + this.MelodicFunction.GetHashCode();
                result = (result * hashQuotient) + this.Direction.GetHashCode();
                return result;
            }
        }

        /// <summary> Evaluate properties of the structure. Used in descendant objects. 
        /// Must be virtual, because of call from StructuralVariety. </summary>
        public override void DetermineBehavior() {
            this.SetElementsFromDifferences();
        }

        /// <summary> Test of emptiness. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsEmptyStruct() {
            return false;
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsValidStruct() {
            if (this.ElementList.Count <= 0) {
                return true;
            }

            int sum = this.ElementList[this.ElementList.Count - 1];
            Contract.Assume(sum > short.MinValue);
            //// Only fragments not exceeding 10 modality steps (c. 1-2 octaves)
            var ok = Math.Abs(sum) <= 10;

            return ok;
        }

        /// <summary> Determine and sets the direction property. </summary>
        public void ComputeDirection() {
            Contract.Requires(this.ElementList.Count > 0);
            short firstElem = (byte)this.ElementList[0];
            short lastElem = (byte)this.ElementList[this.ElementList.Count - 1];
            this.Direction = lastElem - firstElem;
        }

        /// <summary>
        /// Complete FromElements.
        /// </summary>
        public override void CompleteFromElements() {
            //// Contract.Requires(0 < this.ElementList.Count);
            if (this.ElementList.Count > 0) {
                base.CompleteFromElements();
            }

            if (this.ElementList.Count > 0) {
                this.ComputeDirection();
            }
        }

        /// <summary>
        /// Planned altitude.
        /// </summary>
        /// <param name="harmonicSystem">The harmonic system.</param>
        /// <param name="harmonicModality">The harmonic modality.</param>
        /// <param name="modalityIndex">Index of the modality.</param>
        /// <returns> Returns value. </returns>
        public int PlannedAltitude(GeneralSystem harmonicSystem, BinarySchema harmonicModality, int modalityIndex) {
            var elements = this.ElementList;
            if (harmonicSystem == null) {
                return 0;
            }

            if (harmonicModality == null) {
                return 0;
            }

            if (elements.Count <= 0) {
                return 0;
            }

            while (modalityIndex < 0) {
                modalityIndex += elements.Count;
            }

            int index = elements[modalityIndex % elements.Count];
            int octaveShift = modalityIndex / harmonicModality.Level;
            var altitude = (byte)(this.Octave + octaveShift) * harmonicSystem.Order;
            if (harmonicModality.Level <= 0) {
                return altitude;
            }

            index = index % harmonicModality.Level;
            if (index >= 0) {
                altitude += harmonicModality.Places[index];
            }

            return altitude;
        }
        #endregion

        #region String representation
        /// <summary>
        /// Make string of musical symbols.
        /// </summary>
        /// <param name="givenShift">Given Shift.</param>
        /// <returns> Returns value. </returns>
        public string SymbolStringOfMode(short givenShift) {
            var str = new StringBuilder();
            var lastE = (byte)(this.GSystem.Order - 1);
            var hm = (HarmonicModality)this.Modality;
            if (hm == null) {
                return string.Empty;
            }

            string s; //// = hm.Symbol(givenShift);
            //// str.Append(s.PadRight(2));
            short level;
            for (byte e = 0; e < lastE; e++) {
                if (e >= this.ElementList.Count) {
                    continue;
                }

                level = (short)(this.ElementList[e] + givenShift);
                s = hm.SymbolAtLevel(level);
                if (s != null) {
                    str.Append(s.PadRight(2));
                }
            }

            if (lastE >= this.ElementList.Count) {
                return str.ToString();
            }

            level = (short)(this.ElementList[lastE] + givenShift);
            s = hm.SymbolAtLevel(level);
            if (s != null) {
                str.Append(s.PadRight(2));
            }

            return str.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value.</returns>
        public override string ToString() {
            // s.Append(this.ElementString());
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", base.ToString(), this.ToneSchema);
        }
        #endregion

        #region Transformations
        /// <summary>
        /// Makes the descending.
        /// </summary>
        public void MakeDescending() {
            var cnt = this.ElementList.Count;
            for (byte e = 0; e < cnt; e++) {
                this.ElementList[e] = (short)(cnt - e);
            }
        }

        /// <summary>
        /// Makes the ascending.
        /// </summary>
        public void MakeAscending() {
            var cnt = this.ElementList.Count;
            for (byte e = 0; e < cnt; e++) {
                this.ElementList[e] = e;
            }
        }

        /// <summary>
        /// Inverts this instance.
        /// </summary>
        public void Invert() {
            var cnt = this.ElementList.Count;
            for (byte e = 0; e < cnt; e++) {
                this.ElementList[e] = (short)-this.ElementList[e];
            }
        }

        /// <summary>
        /// Magnifies this instance.
        /// </summary>
        public void Magnify() {
            var cnt = this.ElementList.Count;
            for (byte e = 0; e < cnt; e++) {
                this.ElementList[e] = (short)(2 * this.ElementList[e]);
            }
        }
        #endregion
    }
}