// <copyright file="RhythmicStructure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary> Rhythmical structure. </summary>
    /// <remarks> Rhythmical structure represents rhythm of one bar. It is always assigned to certain
    /// rhythmical modality (and to rhythmical system). It has some inner characteristics 
    /// (mobility, tension, entropy,..). </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class RhythmicStructure : FiguralSchema, IRhythmic, IModalStruct
    {
        #region Constructors
        /// <summary> Initializes a new instance of the RhythmicStructure class.  Serializable. </summary>
        public RhythmicStructure() {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public RhythmicStructure(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
            this.DetermineToneLevel();
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public RhythmicStructure(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, number) {
            Contract.Requires(givenSystem != null);
            this.DetermineToneLevel();
        }

        /// <summary> Initializes a new instance of the RhythmicStructure class. </summary>
        /// <param name="structure">Rhythmical structure.</param>
        public RhythmicStructure(FiguralStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
            this.DetermineToneLevel();
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicStructure class.
        /// </summary>
        /// <param name="sysOrder">System order.</param>
        /// <param name="shape">Binary Shape.</param>
        public RhythmicStructure(byte sysOrder, RhythmicShape shape)  ////
            : base(RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, sysOrder), null) {
            Contract.Requires(shape != null);
            if (shape == null || this.GSystem.Order == 0) {
                return;
            }

            this.SetElement(0, 2);  //// start of pause needed!          
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (shape.IsOn(e)) {
                    this.SetElement(e, (byte)RhythmicElement.StartTone);
                }
            }

            //// 2016/05
            this.CompleteFromElements();
            this.DetermineBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicStructure class.  Serializable.
        /// </summary>
        /// <param name="rhythmicOrder">Rhythmical order.</param>
        /// <param name="musicalTones">Musical tones.</param>
        public RhythmicStructure(byte rhythmicOrder, IList<IMusicalTone> musicalTones)
            : base(RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder), null) {
            Contract.Requires(musicalTones != null);

            //// if (musicalTones == null) { return; }
            var cnt = musicalTones.Count;
            this.SetElement(0, (byte)RhythmicElement.StartRest); //// Potential pause  start
            for (var i = 0; i < cnt; i++) {
                var mt = musicalTones.ElementAt(i);
                if (mt == null) {
                    continue;
                }

                //// Tone start
                this.SetElement(mt.BitFrom, mt.IsPause ? (byte)RhythmicElement.StartRest : (byte)RhythmicElement.StartTone);
                //// Potential pause start
                var limitTo = rhythmicOrder;
                if (i + 1 < cnt) {
                    var nextTone = musicalTones.ElementAt(i + 1);
                    if (nextTone != null) {
                        limitTo = nextTone.BitFrom;
                    }
                }

                if (mt.BitRange.BitTo + 1 < limitTo) { //// (mt.BarNumberFrom)
                    this.SetElement((byte)(mt.BitRange.BitTo + 1), (byte)RhythmicElement.StartRest); //// (mt.BarNumberFrom)
                }
            }

            //// 2015/01 Added condition && !firstTone.IsPause
            if (musicalTones.FirstOrDefault() is MusicalStrike firstTone && firstTone.BitFrom == 0
                   && !firstTone.IsPause && firstTone.IsFromPreviousBar) {
                this.SetElement(0, (byte)RhythmicElement.ContinuePrevious);
            }

            //// this.DetermineINumber();
            this.DetermineLevel();
            this.DetermineToneLevel();
            //// this.ResetSchema();
            //// this.ComputeRhythmicProperties();
        }

        /// <summary> Initializes a new instance of the RhythmicStructure class. </summary>
        /// <param name="givenSystem"> The given system. </param>
        /// <param name="markRhythm">  The mark rhythm. </param>
        public RhythmicStructure(GeneralSystem givenSystem, XElement markRhythm)
                                    : base(givenSystem, string.Empty) {
            string code = XmlSupport.ReadStringAttribute(markRhythm.Attribute("Code"));
            this.SetStructuralCode(code);
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "Structure",
                                new XAttribute("Code", this.GetStructuralCode),
                                new XAttribute("Schema", this.ElementSchema));
                return xe;
            }
        }
        #endregion

        #region Interface - simple properties
        /// <summary> Gets or sets Tone Level. </summary>
        /// <value> Property description. </value>
        public byte ToneLevel { get; set; }

        /// <summary>
        /// Gets a value indicating whether is from previous bar.
        /// </summary>
        /// <value>The is from previous bar.</value>
        public bool IsFromPreviousBar => this.ElementList.ElementAt(0) == (byte)RhythmicElement.ContinuePrevious;

        /// <summary>
        /// Gets a value indicating whether this instance has properties.
        /// </summary>
        /// <value>
        /// Is <c>true</c> if this instance has properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasProperties => this.Properties.ContainsKey(GenProperty.FormalMobility);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
        /// </value>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Gets the element schema and occurrence.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string ElementSchemaAndOccurrence => string.Format(
            CultureInfo.CurrentCulture,
            "({0}/{1}) {2} ({3})",
            this.ToneLevel,
            this.Level,
            this.ElementSchema,
            this.Occurrence);

        /// <summary>
        /// Gets the level outline.
        /// </summary>
        /// <value>
        /// The level outline.
        /// </value>
        public string LevelOutline => string.Format(
                    CultureInfo.CurrentCulture, 
                    "{0} / {1} / {2}",
                    this.ToneLevel,
                    this.Level,
                    this.GSystem.Order);

        /// <summary>
        /// Gets a value indicating whether [start with formal rest].
        /// </summary>
        /// <value>
        /// <c>True</c> if [start with formal rest]; otherwise, <c>false</c>.
        /// </value>
        public bool StartsWithFormalRest {
            get {
                if (this.ElementList.Count == 0) {
                    return true;
                }

                var first = (byte)this.ElementList[0];
                if (first != 2) {
                    return false;
                }

                byte length = 1;
                for (byte i = 1; i < this.ElementList.Count; i++) {
                    var value = (byte)this.ElementList[i];
                    length++;
                    if (value != (byte)RhythmicElement.ContinuePrevious) {
                        break;
                    }
                }

                return length >= this.RhythmicSystem.Order / 3;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [end with formal rest].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [end with formal rest]; otherwise, <c>false</c>.
        /// </value>
        public bool EndsWithFormalRest {
            get {
                byte length = 0;
                for (var i = this.ElementList.Count - 1; i >= 0; i--) {
                    var value = (byte)this.ElementList[i];
                    if (value == (byte)RhythmicElement.StartTone) {
                        return false;
                    }

                    length++;
                    if (value == (byte)RhythmicElement.StartRest) {
                        break;
                    }
                }

                return length >= this.RhythmicSystem.Order / 3;
            }
        }

        #endregion

        #region Interface - object properties
        /// <summary> Gets rhythmical system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicSystem RhythmicSystem => (RhythmicSystem)this.GSystem;

        /// <summary> Gets or sets rhythmical modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicModality RhythmicModality {
            get => (RhythmicModality)this.Modality;

            set => this.Modality = value;
        }

        /// <summary>
        /// Gets the get rhythmic shape.
        /// </summary>
        /// <value>
        /// The get rhythmic shape.
        /// </value>
        public RhythmicShape GetRhythmicShape {
            get {
                var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, this.Order);
                var bitArray = new BitArray(this.Order);
                for (byte i = 0; i < this.Order; i++) {
                    bitArray[i] = this.IsOn(i);
                }

                var shape = new RhythmicShape(rsystem, bitArray);
                return shape;
            }
        }

        /// <summary> Gets or sets previous rhythmical structure. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicStructure PreviousStruct { get; set; }

        /// <summary>
        /// Gets the code mark.
        /// </summary>
        /// <value>
        /// The code mark.
        /// </value>
        public string CodeMark {
            get {
                int beat = (int)this.RhythmicBehavior.Beat / 4;  //// ?!?!? bounds of Variance???  RhythmicBehavior.Tension
                var beatCode = MusicalProperties.GetLetter(beat, true);
                int tension = (int)this.FormalBehavior.Variance / 10;  //// ?!?!? bounds of Variance???  RhythmicBehavior.Tension
                var tensionCode = MusicalProperties.GetLetter(tension, true);
                var codeMark = string.Format(
                                    "{0,2}/{1,2}-{2}.{3}",
                                    this.Level,
                                    this.Level - this.ToneLevel,
                                    tensionCode,
                                    beatCode);
                return codeMark;
            }
        }
        #endregion

        #region Static factory methods
        /// <summary>
        /// Get new harmonic structure.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Structural number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicStructure GetNewRhythmicStructure(GeneralSystem givenSystem, long number) {
            Contract.Requires(givenSystem != null);
            Contract.Ensures(Contract.Result<RhythmicStructure>() != null);
            var rs = new RhythmicStructure(givenSystem, number);
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Gets Rhythmic Structure.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicStructure GetNewRhythmicStructure(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            Contract.Ensures(Contract.Result<RhythmicStructure>() != null);
            var rs = new RhythmicStructure(givenSystem, structuralCode);
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Gets the full structure.
        /// </summary>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns> Returns value. </returns>
        public static RhythmicStructure GetFullStructure(byte rhythmicOrder) {
            var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder);
            var code = $"1,{rhythmicOrder - 1}*0";
            var rstruct = GetNewRhythmicStructure(rsystem, code);
            return rstruct;
        }

        /// <summary>
        /// Gets the rest structure.
        /// </summary>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns> Returns value. </returns>
        public static RhythmicStructure GetRestStructure(byte rhythmicOrder) {
            var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder);
            var code = $"2,{rhythmicOrder - 1}*0";
            var rstruct = GetNewRhythmicStructure(rsystem, code);
            return rstruct;
        }

        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the RhythmicStructure object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return GetNewRhythmicStructure(this.GSystem, this.GetStructuralCode);
        }

        /// <summary>
        /// Range of bit pair on given Level.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public BitRange OverrunRange() {
            if (this.IsOn(0)) { //// this.Level == 0 || 
                return null;
            }

            var order = this.GSystem.Order;
            byte place = 0; //// this.PlaceAtLevel(0);
            for (byte tick = 0; tick < order; tick++) {
                if (tick > this.ElementList.Count - 1) {  //// 2014/01
                    break;
                }

                if (this.ElementList[tick] <= 0) {
                    continue;
                }

                place = tick;
                break;
            }

            if (place == 0) {
                place = order;
            }

            var range = new BitRange(order, 0, place);
            return range;
        }

        /// <summary>
        /// Half-divided structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicStructure HalfEnrichedStructure() {
            var rs = new RhythmicStructure { GSystem = this.GSystem };
            short lastToneElement = -1;
            for (byte i = 0; i < this.ElementList.Count; i++) {
                var value = (byte)this.ElementList[i];
                rs.ElementList.Add(value);
                if (value > 0) {
                    if (lastToneElement >= 0) {
                        var half = (byte)((lastToneElement + i) / 2);
                        rs.ElementList[half] = 1;
                    }

                    lastToneElement = i;
                }
            }

            if (lastToneElement >= 0) {
                var half = (byte)((lastToneElement + this.ElementList.Count - 1) / 2);
                rs.ElementList[half] = 1;
            }

            rs.CompleteFromElements();
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Half-reduced structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        public RhythmicStructure HalfReducedStructure() {
            var rs = new RhythmicStructure { GSystem = this.GSystem };
            bool spaceSwitch = false;
            for (byte i = 0; i < this.ElementList.Count; i++) {
                var value = (byte)this.ElementList[i];
                if (value > 0) {
                    if (spaceSwitch) {
                        value = 0;
                    }

                    spaceSwitch = !spaceSwitch;
                }

                rs.ElementList.Add(value);
            }

            rs.CompleteFromElements();
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Inverted structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        public new RhythmicStructure InvertedStructure() {
            var rs = new RhythmicStructure { GSystem = this.GSystem };
            byte cnt = 0;
            for (byte i = 0; i < this.ElementList.Count; i++) {
                var value = (byte)this.ElementList[this.ElementList.Count - i - 1];
                if (value > 0) {
                    rs.ElementList.Add(value);
                    for (byte n = 0; n < cnt; n++) {
                        rs.ElementList.Add(0);
                    }

                    cnt = 0;
                }
                else {
                    cnt++;
                }
            }

            rs.CompleteFromElements();
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsEmptyStruct() {
            return this.IsOff(0) || base.IsEmptyStruct();
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsValidStruct() {
            var ok = true;
            byte lastNum = 0;
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (e >= this.ElementList.Count) {
                    continue;
                }

                var num = (byte)this.ElementList[e];
                if (num == (byte)RhythmicElement.StartRest && lastNum == (byte)RhythmicElement.StartRest) { // two pauses in sequence
                    ok = false;
                    break;
                }

                if (num != 0) {
                    lastNum = num;
                }
            }

            return ok;
        }

        /// <summary> Evaluate properties of the structure. </summary>
        public override void DetermineBehavior() {
            //// this.SetElements();
            this.DetermineToneLevel();
            this.ComputeRhythmicProperties();
            this.ComputeMobility();
        }

        /// <summary>
        /// Converts to system.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicStructure ConvertToSystem(GeneralSystem givenSystem) { //// RhythmicSystem
            Contract.Requires(givenSystem != null);

            if (this.Order == givenSystem.Order) {
                return this;
            }

            var realRhythmicOrder = givenSystem.Order;
            var d = (float)realRhythmicOrder / this.Order;
            var realStruct = new RhythmicStructure(givenSystem, string.Empty);
            float index = 0;
            for (byte bit = 0; index < realRhythmicOrder; index += d, bit++) {
                var element = (byte)Math.Round(index);
                if (bit < this.ElementList.Count && element < realStruct.ElementList.Count) {
                    realStruct.ElementList[element] = this.ElementList[bit];
                }
            }

            //// 2016/05
            realStruct.CompleteFromElements();
            realStruct.DetermineBehavior();
            return realStruct;
        }

        /// <summary>
        /// Determine ToneLevel.
        /// </summary>
        public void DetermineToneLevel() {
            byte s = 0;
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (e < this.ElementList.Count && this.ElementList[e] == 1) {
                    s++;
                }
            }

            this.ToneLevel = s;
            if (!this.Properties.ContainsKey(GenProperty.ToneLevel)) {
                this.Properties[GenProperty.ToneLevel] = this.ToneLevel;
            }
        }
        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and ElementSchema. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object obj) {
            if (!(obj is RhythmicStructure rs)) {
                return 0;
            }

            if (this.Level < rs.Level) {
                return -1;
            }

            return this.Level > rs.Level ? 1 : string.Compare(this.ElementSchema, rs.ElementSchema, StringComparison.Ordinal);

            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a RhythmicStructure");
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

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode() {
            return this.ElementSchema.GetHashCode();
        }

        #endregion

        #region String representation
        /// <summary> List of figure elements. </summary>
        /// <returns> Returns value. </returns>
        public override string ElementString() {
            var s = new StringBuilder();
            var limit = this.GSystem.Order;
            for (byte e = 0; e < limit; e++) { //// 2016/04 was e += 2
                if (e  >= this.ElementList.Count) {
                    continue;
                }

                var elem1 = (byte)this.ElementList[e];
                switch (elem1) {
                    case 0:
                        s.Append("-");
                        break;
                    case 1:
                        s.Append("T");
                        break;
                    case 2:
                        s.Append("P");
                        break;
                        //// resharper default: break;
                }
            }

            return s.ToString();
        }

        /// <summary>
        /// Compacts the element string.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string CompactElementString() {
            var s = new StringBuilder();
            var median = this.GSystem.Order / 2;
            for (byte e = 0; e < median; e += 1) { //// 2016/04 was e += 2
                if ((e * 2) + 1 >= this.ElementList.Count) {
                    continue;
                }

                var elem1 = (byte)this.ElementList[e * 2];
                var elem2 = (byte)this.ElementList[(e * 2) + 1];
                if (elem2 == 0) {
                    switch (elem1) {
                        case 0:
                            s.Append("-");
                            break;
                        case 1:
                            s.Append("T");
                            break;
                        case 2:
                            s.Append("P");
                            break;
                            //// resharper default: break;
                    }
                }
                else {
                    s.Append("X");
                }
            }

            return s.ToString();
        }

        /// <summary> Short string representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public string ToShortString() {
            var s = new StringBuilder();
            s.Append(" " + this.ElementString());
            //// s.Append(" " + this.DistanceString());
            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(string.Format(CultureInfo.InvariantCulture, "{0},{1}", base.ToString(), this.ElementString()));
            s.Append(",");
            s.AppendLine(this.StringOfProperties());
            return s.ToString();
        }
        #endregion

        #region Relation to previous struct
        /// <summary> Sets previous harmonic structure and Compute corresponding properties. </summary>
        /// <param name="structure">Harmonic structure.</param>
        public void SetPreviousStruct(RhythmicStructure structure) {
            Contract.Requires(structure != null);
            this.PreviousStruct = structure;
            this.DetermineBehaviorFromPreviousStruct();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Determine and sets the mobility property.
        /// </summary>
        /// <returns>Returns value.</returns>
        private float ComputeToneMobility() {
            //// float mobility = this.GSystem.Order > 0 ? (this.Level / (float)this.GSystem.Order) * 100f : 0;
            //// float mobility = this.GSystem.Order > 0 ? (this.ToneLevel / (float)this.GSystem.Order) * 100f : 0;
            var toneMobility = (this.ToneLevel / (float)this.Level) * 100f;
            return toneMobility;
        }

        /// <summary> Sets properties of the structure with regard to previous structure. </summary>
        private void DetermineBehaviorFromPreviousStruct() {
            Contract.Requires(this.PreviousStruct != null);
            //// var rhythmicSystem = (RhythmicSystem)this.GSystem;
            //// var harRelation = new HarmonicRelation(harmonicSystem, this.PreviousStruct, this);
            //// var continuity = harRelation.MeanValueOfProperty(GenProperty.FormalContinuity, false, true);
            //// var impulse = harRelation.MeanValueOfProperty(GenProperty.FormalImpulse, false, true);
            //// this.Properties[GenProperty.RelatedContinuity] = continuity; // add with repetition
            //// this.Properties[GenProperty.RelatedImpulse] = impulse; // add with repetition
        }
        #endregion
    }
}
