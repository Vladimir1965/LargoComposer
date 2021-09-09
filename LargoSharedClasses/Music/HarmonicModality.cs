// <copyright file="HarmonicModality.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Abstract;
    using LargoSharedClasses.Harmony;
    using LargoSharedClasses.Interfaces;

    /// <summary> Harmonic modality. </summary>
    /// <remarks>
    /// Harmonic modality is defined by its number and appropriateness
    /// to harmonic GSystem. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicModality : BinarySchema, IHarmonic, IModalStruct
    {
        #region Fields
        /// <summary> String of musical symbols. </summary>
        private string toneSchema;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicModality class.  Serializable. </summary>
        public HarmonicModality() {
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public HarmonicModality(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenBitArray">Bit array.</param>
        public HarmonicModality(GeneralSystem givenSystem, BitArray givenBitArray)
            : base(givenSystem, givenBitArray) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of modality.</param>
        /// <param name="transposition">Transposition shift.</param>
        public HarmonicModality(GeneralSystem givenSystem, long number, byte transposition)
            : base(givenSystem, BinaryNumber.Transposition(givenSystem, number, transposition)) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicModality class.  Serializable. 
        /// </summary>
        /// <param name="harmonicOrder">Harmonic order.</param>
        /// <param name="melodicTones">Melodic tones.</param>
        /// <param name="minAltitude">Min Altitude.</param>
        /// <param name="propertiesNeeded">Properties Needed.</param>
        public HarmonicModality(byte harmonicOrder, IEnumerable<MusicalStrike> melodicTones, int minAltitude, bool propertiesNeeded)
            : base(HarmonicSystem.GetHarmonicSystem(harmonicOrder), (string)null) {
            Contract.Requires(harmonicOrder != 0);
            Contract.Requires(melodicTones != null);

            //// if (melodicTones == null) { return; }
            //// mt.Pitch.Element
            foreach (var mt in melodicTones) {
                if (mt is MusicalTone tone && tone.IsTrueTone) {
                    byte element = (byte)((tone.Pitch.SystemAltitude - minAltitude) % harmonicOrder);
                    this.On(element);
                }
            }

            this.DetermineLevel();
            if (propertiesNeeded) {
                this.ComputeVariance();
                this.ComputeBalance();
            }
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public HarmonicModality(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
        }

        /// <summary> Initializes a new instance of the HarmonicModality class. </summary>
        /// <param name="structure">Binary structure.</param>
        public HarmonicModality(BinaryStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
        }
        #endregion

        #region Interface - simple properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary> Gets or sets tone representation. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema {
            get => this.toneSchema ?? (this.toneSchema = this.SchemaOfTones());

            set => this.toneSchema = value;
        }

        /// <summary>
        /// Gets the name and tones.
        /// </summary>
        /// <value>
        /// The name and tones.
        /// </value>
        public string NameAndTones => this.Name + this.ToneSchema;

        /// <summary>
        /// Gets or sets the formal energy.
        /// </summary>
        /// <value>
        /// The formal energy.
        /// </value>
        public HarmonicBehavior FormalEnergy { get; set; }

        /// <summary> Gets inner continuity. </summary>
        /// <value> Property description. </value>
        public float FormalContinuity => this.Properties.ContainsKey(GenProperty.InnerContinuity) ? this.Properties[GenProperty.InnerContinuity] : 0f;

        /// <summary> Gets inner impulse. </summary>
        /// <value> Property description. </value>
        public float FormalImpulse => this.Properties.ContainsKey(GenProperty.InnerImpulse) ? this.Properties[GenProperty.InnerImpulse] : 0f;

        /// <summary> Gets inner heterogeneity. </summary>
        /// <value> Property description. </value>
        public float Heterogeneity => this.Properties.ContainsKey(GenProperty.FormalVariance) ? this.Properties[GenProperty.FormalVariance] : 0f;

        /// <summary> Gets inner balance. </summary>
        /// <value> Property description. </value>
        public float FormalBalance => this.Properties.ContainsKey(GenProperty.FormalBalance) ? this.Properties[GenProperty.FormalBalance] : 0f;

        /// <summary> Gets inner balance. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public float HarmonicModulationPower { get; private set; }

        #endregion

        #region Interface - object properties
        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public HarmonicSystem HarmonicSystem {
            get {
                Contract.Ensures(Contract.Result<HarmonicSystem>() != null);
                return (HarmonicSystem)this.GSystem;
            }
        }

        #endregion

        #region Static factory methods
        /// <summary>
        /// Get New Harmonic Modality.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicModality GetNewHarmonicModality(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var hm = new HarmonicModality(givenSystem, structuralCode);
            hm.DetermineBehavior();
            return hm;
        }

        /// <summary>
        /// Get New Harmonic Modality.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="bitArray">Bit array.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicModality GetNewHarmonicModality(GeneralSystem givenSystem, BitArray bitArray) {
            Contract.Requires(givenSystem != null);
            var hm = new HarmonicModality(givenSystem, bitArray);
            hm.DetermineBehavior();
            return hm;
        }

        /// <summary>
        /// Get New Harmonic Modality.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Structural Number.</param>
        /// <param name="givenTransposition">Given Transposition.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicModality GetNewHarmonicModality(GeneralSystem givenSystem, long number, byte givenTransposition) {
            Contract.Requires(givenSystem != null);
            var hm = new HarmonicModality(givenSystem, number, givenTransposition);
            hm.DetermineBehavior();
            return hm;
        }

        /// <summary>
        /// Loads the harmonic modality.
        /// </summary>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <param name="modNumber">The mod number.</param>
        /// <param name="modName">Name of the mod.</param>
        /// <returns> Returns value. </returns>
        public static HarmonicModality LoadHarmonicModality(byte harmonicOrder, long modNumber, string modName) {
            var hs = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
            var harModality = new HarmonicModality(hs, modNumber) { Name = modName };
            return harModality;
        }
        #endregion

        #region Public methods
        /// <summary> Clone object. </summary>
        /// <returns> Returns value. </returns>
        public override object Clone() {
            var hm = GetNewHarmonicModality(this.GSystem, this.GetStructuralCode);
            hm.CopyProperties(this.Properties);
            return hm;
        }
        #endregion

        #region Properties
        /// <summary> Evaluate properties of the structure. </summary>
        public override void DetermineBehavior() {
            this.ComputeVariance();
            this.ComputeBalance();
            var fs = new HarmonicStateFormal((HarmonicSystem)this.GSystem, this);
            this.Properties[GenProperty.InnerContinuity] = fs.FormalContinuity;
            this.Properties[GenProperty.InnerImpulse] = fs.FormalImpulse;
            this.Properties[GenProperty.Consonance] = fs.FormalConsonance;
        }

        /// <summary> Sets properties of the modality with regard to other modality. </summary>
        /// <param name="modality">Abstract modality.</param>
        public void DetermineBehaviorAfterModality(BinarySchema modality) {
            if (modality == null) {
                return;
            }

            var harmonicSystem = (HarmonicSystem)this.GSystem;
            var harRelation = new HarmonicRelation(harmonicSystem, modality, this);
            var continuity = harRelation.MeanValueOfProperty(GenProperty.InnerContinuity, true, true);
            var impulse = harRelation.MeanValueOfProperty(GenProperty.InnerImpulse, false, true);
            var power = (100.0f + impulse - continuity) / 2;
            this.HarmonicModulationPower = power;
        }
        #endregion

        #region Potential
        /// <summary> Compute formal potential of given element. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value. </returns>
        public float PotentialOfElement(byte element) {
            var hs = new HarmonicStateFormal((HarmonicSystem)this.GSystem, this);
            hs.AddIntervalsLeadingToElement(element);
            var p = hs.MeanValueOfProperty(GenProperty.FormalPotentialInfluence, false, true);
            return p;
        }
        #endregion

        #region Substructures
        /// <summary>
        /// Harmonic substructures.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> Substructures(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewHarmonicStructModalVariety(
                                        StructuralVarietyType.BinarySubstructuresOfModality,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Harmonical Substructures.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<HarmonicStructure> Substructures() {
            var hv = StructuralVarietyFactory.NewHarmonicStructModalVariety(
                                        StructuralVarietyType.BinarySubstructuresOfModality,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }
        #endregion

        #region String representation
        /// <summary>
        /// Symbols at place.
        /// </summary>
        /// <param name="givenPlace">The given place.</param>
        /// <returns> Returns value. </returns>
        public string SymbolAtPlace(short givenPlace) {
            var p = givenPlace;
            var c = this.HarmonicSystem.Symbol(p, true);
            if (!this.HarmonicSystem.IsEnharmonic(p)) {
                return c;
            }

            var below = (byte)((p + this.Order - 1) % this.Order);
            var above = (byte)((p + this.Order + 1) % this.Order);
            var halftoneBelow = this.BitArray[below];
            var halftoneAbove = this.BitArray[above];
            if (halftoneAbove) {
                return c;
            }

            if (halftoneBelow) {
                c = this.HarmonicSystem.Symbol(p, false);
            }
            else {
                var down2 = (byte)((p + this.Order - 2) % this.Order);
                var down3 = (byte)((p + this.Order - 3) % this.Order);
                var toneDown2 = this.BitArray[down2];
                var toneDown3 = this.BitArray[down3];
                if (toneDown2 && toneDown3) {
                    c = this.HarmonicSystem.Symbol(p, false);
                }
            }

            return c;
        }

        /// <summary>
        /// Returns symbols for given level.
        /// </summary>
        /// <param name="givenLevel">Given level.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string SymbolAtLevel(short givenLevel) {
            var p = this.RealPlaceAtLevel(givenLevel);
            var c = this.SymbolAtPlace(p);
            return c;
        }

        /// <summary>
        /// Makes tone representation of the structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string SchemaOfTones() {
            if (this.Level <= 0) {
                return string.Empty;
            }

            var str = new StringBuilder();
            var lastL = (byte)(this.Level - 1);
            string s;
            for (byte level = 0; level < lastL; level++) {
                s = this.SymbolAtLevel(level);  //// this.PlaceAtLevel
                str.Append(s);
                str.Append("-");
            }

            s = this.SymbolAtLevel(lastL); //// this.PlaceAtLevel
            str.Append(s);

            return str.ToString().Trim();
        }

        /// <summary> Writes particular formal potentials to string. </summary>
        /// <returns> Returns value. </returns>
        public string PotentialString() {
            var s = new StringBuilder();
            foreach (var e in this.Places) {
                var p = this.PotentialOfElement(e);
                s.AppendFormat(CultureInfo.CurrentCulture, "{0,4}:  ", this.SymbolAtPlace(e));
                s.AppendFormat(CultureInfo.CurrentCulture, "{0,4:F1}", p);
                s.Append(Environment.NewLine);
            }

            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            //// s.AppendLine("Harmonic modality");
            //// s.AppendLine(base.ToString());
            s.Append(this.ToneSchema);
            //// [s appendFormat:@"%@\n",[[self potentialValues] description]];
            //// [s appendFormat:@"%@\n",[[self harFunctions] description]];
            return s.ToString();
        }
        #endregion

        //// Potentials of given harmonic structure

        /// <summary> Compute formal potentials of given harmonic structure. </summary>
        /// <param name="structure">Harmonic structure.</param>
        /// <returns> Returns value. </returns>
        private Collection<float> PotentialsForHarmonicStructure(BinarySchema structure) {
            Contract.Requires(structure != null);

            var places = structure.Places;
            var arr = new Collection<float>(); ////struct.Level

            float potentialSum = 0;
            foreach (var p in places.Select(this.PotentialOfElement)) {
                arr.Add(p);
                potentialSum = potentialSum + p;
            }

#pragma warning disable 168
            var i = 0;
            //// ReSharper disable once UnusedVariable
            foreach (var e in places.Where(e => potentialSum >= DefaultValue.AfterZero && potentialSum <= DefaultValue.LargeNumber)) {
                if (i < arr.Count) {
                    arr[i] = arr[i] / potentialSum * 100.0f;
                }

                i++;
            }

            return arr;
        }
    }
}