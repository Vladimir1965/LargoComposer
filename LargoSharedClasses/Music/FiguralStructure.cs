// <copyright file="FiguralStructure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using LargoSharedClasses.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Figural Structure.
    /// </summary>
    public class FiguralStructure : GeneralOwner, IComparable, IGeneralStruct
    {
        #region Fields
        /// <summary>
        /// Structural Code.
        /// </summary>
        private string structuralCode;

        /// <summary>
        /// General system.
        /// </summary>
        private GeneralSystem gsystem; //// readonly

        /// <summary> Number of nonzero bits, number of rotations. </summary>
        private byte level;

        /// <summary> List of elements. </summary>
        private Collection<short> elemList;

        /// <summary> List of differences. </summary>
        private Collection<short> diffList;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the FiguralStructure class.  Serializable. </summary>
        public FiguralStructure() {
            this.elemList = new Collection<short>();
        }

        /// <summary>
        /// Initializes a new instance of the FiguralStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public FiguralStructure(GeneralSystem givenSystem, decimal number)
            : this() {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.DecimalNumber = number;
            this.SetElements(); // 20081224
            //// this.CheckInstance();
        }

        /// <summary>
        /// Initializes a new instance of the FiguralStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenStructuralCode">Structural code.</param>
        public FiguralStructure(GeneralSystem givenSystem, string givenStructuralCode)
            : this() {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            for (byte e = 0; e < this.gsystem.Order; e++) {
                this.ElementList.Add(0);
            }

            this.SetStructuralCode(givenStructuralCode);
            //// this.SetElements(); // 20081224
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the FiguralStructure class. </summary>
        /// <param name="structure">Figural structure.</param>
        public FiguralStructure(FiguralStructure structure)
            : this() {
            Contract.Requires(structure != null);

            this.gsystem = structure.GSystem;
            this.level = structure.Level;
            this.GLevel = structure.GLevel;
            this.SetElements();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }
        #endregion

        #region Properties

        /// <summary> Gets or sets DecimalNumber. </summary>
        /// <value> Property description. </value>
        public decimal DecimalNumber { get; set; }

        /// <summary> Gets or sets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public GeneralSystem GSystem {
            get {
                Contract.Ensures(Contract.Result<GeneralSystem>() != null);
                Contract.Ensures(Contract.Result<GeneralSystem>().Order > 0);
                if (this.gsystem == null) {
                    throw new InvalidOperationException("G-system is null.");
                }

                return this.gsystem;
            }

            set => this.gsystem = value;
        }

        /// <summary> Gets or sets modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public IGeneralStruct Modality { get; set; }

        /// <summary> Gets or sets level of the structure. Sum of digits. </summary>
        /// <value> Property description. </value>
        public int GLevel { get; set; }

        /// <summary> Gets or sets level of the structure. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public byte Level {
            get => this.level;

            set {
                this.level = value;
                this.SetProperty(GenProperty.Level, this.level);
            }
        }

        /// <summary>
        /// Gets or sets the occurrence - for statistical reasons (material,...). May be moved to properties...
        /// </summary>
        /// <value>
        /// The occurrence.
        /// </value>
        public int Occurrence { get; set; }

        /// <summary> Gets variability. </summary>
        /// <value> Property description. </value>
        public float Variability { get; private set; }

        /// <summary> Gets elements of the figure. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<short> ElementList {
            get {
                Contract.Ensures(Contract.Result<Collection<short>>() != null);
                if (this.elemList == null) {
                    throw new InvalidOperationException("List of elements is null.");
                }

                return this.elemList;
            }
        }

        /// <summary> Gets list of differences. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<short> DiffList {
            get {
                Contract.Ensures(Contract.Result<Collection<short>>() != null);
                if (this.diffList == null) {
                    this.MakeDifferences();
                }

                if (this.diffList == null) {
                    throw new InvalidOperationException("List of differences is null.");
                }

                return this.diffList;
            }
        }

        /// <summary> Gets differences. </summary>
        /// <value> Property description. </value>
        public string DiffSchema => this.DifferenceString();

        /// <summary> Gets differences. </summary>
        /// <value> Property description. </value>
        public string PositiveElementSchema => this.PositiveElementString();

        #endregion

        /// <summary>
        /// Gets the Structural Code.
        /// </summary>
        /// <returns> Returns value. </returns>
        /// <value> Property description. </value>
        public string GetStructuralCode {
            get {
                //// 2014/12 Time optimization - was prepared here (why not used?)
                if (string.IsNullOrEmpty(this.structuralCode)) {
                    this.structuralCode = this.DetermineStructuralCode();
                }

                return this.structuralCode;
            }
        }

        #region Static Functions
        /// <summary> Rotate the number, moving first bit to the end. </summary>
        /// <param name="givenDegree">Degree of system.</param>
        /// <param name="order">Order of system.</param>
        /// <param name="number">Number of instance.</param>
        /// <returns>Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public static decimal TransposeLeft(byte givenDegree, byte order, decimal number) {
            var size = (decimal)Math.Pow(givenDegree, order);
            return (number * givenDegree) % size;
        }

        /// <summary> Returns empty number with only bit at position 'i' set. </summary>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public static long BitAt(byte element) {
            return (long)1 << element;
        }

        /// <summary> Returns if selected bit is ON. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public static bool ElementIsOn(decimal number, byte element) {
            return ((long)number & BitAt(element)) != 0;
        }

        /// <summary>
        /// Determine Level.
        /// </summary>
        /// <param name="sysOrder">System order.</param>
        /// <param name="number">Structural Number.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static byte DetermineLevel(byte sysOrder, decimal number) {
            byte level = 0;
            for (byte e = 0; e < sysOrder; e++) {
                if (ElementIsOn(number, e)) {
                    level++;
                }
            }

            return level;
        }
        #endregion

        #region Static operators

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(FiguralStructure structure1, FiguralStructure structure2) { return object.Equals(structure1, structure2); }
        //// public static bool operator !=(FiguralStructure structure1, FiguralStructure structure2) { return !object.Equals(structure1, structure2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(FiguralStructure object1, FiguralStructure object2) {
            if (object1 != null && object2 != null && (object1.DecimalNumber > 0 || object2.DecimalNumber > 0)) {
                return object1.DecimalNumber < object2.DecimalNumber;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >=(FiguralStructure object1, FiguralStructure object2) {
            if (object1 != null && object2 != null && (object1.DecimalNumber > 0 || object2.DecimalNumber > 0)) {
                return object1.DecimalNumber >= object2.DecimalNumber;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <=(FiguralStructure object1, FiguralStructure object2) {
            if (object1 != null && object2 != null && (object1.DecimalNumber > 0 || object2.DecimalNumber > 0)) {
                return object1.DecimalNumber <= object2.DecimalNumber;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >(FiguralStructure object1, FiguralStructure object2) {
            if (object1 != null && object2 != null && (object1.DecimalNumber > 0 || object2.DecimalNumber > 0)) {
                return object1.DecimalNumber > object2.DecimalNumber;
            }

            return false;
        }
        #endregion

        /// <summary> Makes a deep copy of the FiguralStructure object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return new FiguralStructure(this.GSystem, this.GetStructuralCode);
        }

        #region Comparison
        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="value">Object to be compared.</param>
        /// <returns> Returns value.</returns>
        public override int CompareTo(object value) {
            if (!(value is FiguralStructure structure)) {
                return 0;
            }

            if (this.Level < structure.Level) {
                return -1;
            }

            return this.Level > structure.Level ?
                       1 :
                       string.CompareOrdinal(this.ElementString(), structure.ElementString());
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a FiguralStructure");
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
            return this.ElementString().GetHashCode();
        }
        #endregion

        /// <summary> Test of emptiness. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public virtual bool IsEmptyStruct() {
            return this.level == 0;
        }

        /// <summary> Validity test. </summary>
        /// <returns>Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public virtual bool IsValidStruct() {
            return true;
        }

        #region Element gettings
        /// <summary> Returns if selected bit is ON. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsOn(byte element) {
            return element < this.ElementList.Count && this.ElementList[element] != 0;
        }

        /// <summary>
        /// Determines whether the specified element is one.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is one; otherwise, <c>false</c>.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsOne(byte element) {
            return element < this.ElementList.Count && this.ElementList[element] == 1;
        }

        /// <summary> Returns if selected bit is OFF. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns>Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsOff(byte element) {
            if (element < this.ElementList.Count) {
                return this.ElementList[element] == 0;
            }

            return false;
        }

        /// <summary> Returns if selected bit is a start of tone. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsToneStart(byte element) {
            if (element < this.ElementList.Count) {
                return this.ElementList[element] == 1;
            }

            return false;
        }

        /// <summary> Returns if selected bit is a start of pause. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsPauseStart(byte element) {
            if (element < this.ElementList.Count) {
                return this.ElementList[element] == 2;
            }

            return false;
        }

        /// <summary>
        /// Returns corresponding binary structure.
        /// </summary>
        /// <param name="breakForRests">If set to <c>true</c> [break for rests].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public BinaryStructure BinaryStructure(bool breakForRests) {
            var sys = new GeneralSystem(2, this.GSystem.Order);
            var binStr = new BinaryStructure(sys, (string)null);
            for (byte e = 0; e < this.GSystem.Order; e++) {
                //// 2015/01
                if (breakForRests) {
                    if (this.IsOn(e)) {
                        binStr.On(e);
                    }
                }
                else {
                    if (this.IsOne(e)) {
                        binStr.On(e);
                    }
                }
            }

            binStr.DetermineLevel();
            return binStr;
        }

        /// <summary>
        /// Bits the array.
        /// </summary>
        /// <param name="breakForRests">if set to <c>true</c> [break for rests].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public BitArray BitArray(bool breakForRests) {
            var bitArray = new BitArray(this.GSystem.Order);
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (breakForRests) {
                    if (this.IsOn(e)) {
                        bitArray[e] = true;
                    }
                }
                else {
                    if (this.IsOne(e)) {
                        bitArray[e] = true;
                    }
                }
            }

            return bitArray;
        }

        /// <summary> Returns number of bits in selected Range, that are ON. </summary>
        /// <param name="elementFrom">First element.</param>
        /// <param name="elementTo">Last element.</param>
        /// <returns>Number of bits.</returns>
        [JetBrains.Annotations.PureAttribute]
        public byte IsOnInRange(byte elementFrom, byte elementTo) {
            byte s = 0;
            for (var e = elementFrom; e <= elementTo; e++) {
                if (this.IsOn(e)) {
                    s++;
                }
            }

            return s;
        }
        #endregion

        #region Element settings
        /// <summary>
        ///  Sets selected bit to value.
        /// </summary>
        /// <param name="element">Number of element.</param>
        /// <param name="value">Value of element.</param>
        public void SetElement(byte element, short value) {
            if (element < this.ElementList.Count) {
                this.ElementList[element] = value;
            }
        }

        /// <summary>
        /// Set Element List.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        public void SetElementList(Collection<short> givenList) {
            this.elemList = givenList;
            this.CompleteFromElements();
            this.DetermineBehavior();
        }
        #endregion

        /// <summary> Determine and sets the level property. </summary>
        public void DetermineLevel() {
            this.Level = 0;
            this.GLevel = 0;
            var order = this.GSystem.Order;
            for (byte e = 0; e < order; e++) {
                if (e >= this.ElementList.Count || this.ElementList[e] <= 0) {
                    continue;
                }

                this.GLevel += this.ElementList[e];
                this.Level++;
            }

            this.Properties[GenProperty.Level] = this.Level; // used with Qualifiers
        }

        /// <summary>
        /// Determine and sets the level property.
        /// </summary>
        /// <param name="givenBit">Given bit of the structure.</param>
        /// <returns> Returns value. </returns>
        public byte LevelOfBit(byte givenBit) {
            if (givenBit == 0) {
                return 0;
            }

            var bitLevel = -1;
            //// byte order = this.GSystem.Order;
            for (byte e = 0; e <= givenBit; e++) {
                if (e < this.ElementList.Count && this.ElementList[e] > 0) {
                    bitLevel++;
                }
            }

            var lev = (byte)((bitLevel >= 0) ? bitLevel : 0);
            return lev;
        }

        /// <summary>
        /// Determine and sets the level property.
        /// </summary>
        public void DetermineINumber() {
            decimal num = 0;
            if (this.ElementList.Count > 0) {
                var order = this.GSystem.Order;
                var degree = this.GSystem.Degree;
                for (var e = (short)(order - 1); e >= 0; e--) {
                    if (e < this.ElementList.Count && num < decimal.MaxValue / degree) { //// Uff 
                        num = (num * this.GSystem.Degree) + (byte)this.ElementList[e];
                    }
                }
            }

            this.DecimalNumber = num;
        }

        /// <summary> Evaluate properties of the structure. Used in descendant objects. 
        /// Must be virtual, because of call from StructuralVariety. </summary>
        public virtual void DetermineBehavior() {
        }

        /// <summary> Make list of differences. </summary>
        public void MakeDifferences() {
            this.diffList = new Collection<short>();
            if (this.ElementList.Count == 0) {
                return;
            }

            byte p = 0, r = 0;
            var order = this.GSystem.Order;
            for (byte e = 0; e < order - 1; e++) {
                if (e + 1 >= this.ElementList.Count) {
                    continue;
                }

                p = (byte)this.ElementList[e];
                r = (byte)this.ElementList[(byte)(e + 1)];
                this.diffList.Add((short)(r - p));
            }

            if (order - 1 < this.ElementList.Count) {
                p = (byte)this.ElementList[(byte)(order - 1)];
            }

            if (this.ElementList.Count > 0) {
                r = (byte)this.ElementList[0];
            }

            this.diffList.Add((short)(r - p));
        }

        /// <summary>
        /// Complete From Elements.
        /// </summary>
        public virtual void CompleteFromElements() {
            this.DetermineLevel();
            ////this.DetermineINumber();
            this.ComputeVariability();
            this.MakeDifferences();
        }

        #region String representation
        /// <summary> List of figure elements. </summary>
        /// <returns>Returns value.</returns>
        public virtual string ElementString() {
            var s = new StringBuilder();
            var cnt = this.ElementList.Count;
            var order = this.GSystem.Order;
            for (byte e = 0; e < cnt; e++) {
                s.Append(this.ElementList[e]);
                if (e < order - 1) {
                    s.Append(",");
                }
            }

            return s.ToString();
        }

        /// <summary> List of figure elements. </summary>
        /// <returns>Returns value.</returns>
        public string PositiveElementString() {
            const byte shift = 50;
            var s = new StringBuilder();
            var cnt = this.ElementList.Count;
            for (byte e = 0; e < cnt; e++) {
                s.AppendFormat(CultureInfo.CurrentCulture, "{0,2}", (this.ElementList[e] + shift).ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            }

            return s.ToString();
        }

        /// <summary> List of figure elements. </summary>
        /// <returns>Returns value.</returns>
        public string InverseElementString() {
            var s = new StringBuilder();
            for (var e = (short)(this.GSystem.Order - 1); e >= 0; e--) {
                if (e >= this.ElementList.Count) {
                    continue;
                }

                s.Append(this.ElementList[e]);
                if (e < this.GSystem.Order - 1) {
                    s.Append(",");
                }
            }

            return s.ToString();
        }

        /// <summary> List of figure Differences. </summary>
        /// <returns> Returns value. </returns>
        public string DifferenceString() {
            if (this.DiffList == null || this.DiffList.Count == 0) {
                return string.Empty;
            }

            var s = new StringBuilder();
            s.Append("(");
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (e >= this.diffList.Count) {
                    continue;
                }

                s.Append(this.diffList[e]);
                if (e < this.GSystem.Order - 1) {
                    s.Append(",");
                }
            }

            s.Append(")");
            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns>Returns value.</returns>
        public override string ToString() {
            var s = new StringBuilder();
            //// s.Append("<" + this.Data.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat) + ">\t"); //// "D", System.Globalization.CultureInfo.CurrentCulture.NumberFormat
            s.Append("L" + this.Level.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            s.Append(" ");
            s.Append(this.ElementString());
            s.Append(" ");
            return s.ToString();
        }
        #endregion

        #region Structural code

        /// <summary> Determine and sets the level property. </summary>
        /// <param name="givenStructuralCode">Structural code.</param>
        public void SetStructuralCode(string givenStructuralCode) {
            this.structuralCode = givenStructuralCode;
            if (string.IsNullOrWhiteSpace(this.structuralCode)) {
                return;
            }

            ////  Temporary if (this.structuralCode.Length == 1) { return; } 
            var structCode = this.structuralCode.Contains('*') ? UnpackCode(this.structuralCode) : this.structuralCode;

            this.Level = 0;
            this.GLevel = 0;
            this.elemList = new Collection<short>();
            //// byte order = this.GSystem.Order;  //// givenStructuralCode.Count;
            var codes = structCode.Split(',');

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var code in codes) {
                if (string.IsNullOrEmpty(code)) {
                    continue;
                }

                var element = short.Parse(code, CultureInfo.CurrentCulture);
                if (element < 0) {
                    continue;
                }

                if (this.ElementList.Count >= this.GSystem.Order) {
                    continue; //// 2013/01
                }

                this.ElementList.Add(element);
                if (element <= 0) {
                    continue;
                }

                this.Level += 1;
                this.GLevel += element;
            }
            //// Used by Qualifiers
            this.Properties[GenProperty.Level] = this.Level;
        }

        /// <summary> Determine and sets the level property. </summary>
        /// <returns>Returns value.</returns>
        public string DetermineStructuralCode() {
            var order = this.GSystem.Order;
            if (order <= 1) {
                return null;
            }

            //// 2014/12 Time optimization
            var sb = new StringBuilder();
            foreach (var c in this.ElementList) {
                sb.Append(c.ToString(CultureInfo.CurrentCulture));
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            var structCode = sb.ToString();

            //// bool packedCodes = true;
            return structCode.Length > 0 ? PackCode(structCode) : structCode;
        }
        #endregion

        #region Computation of properties
        /// <summary> Determine and sets the variability property. </summary>
        public void ComputeVariability() {
            var me = this.MeanElement();
            var value = (float)0.0;
            var order = this.GSystem.Order;
            for (byte e = 0; e < order; e++) {
                if (e < this.ElementList.Count) {
                    value += (float)Math.Pow(Math.Abs(this.ElementList[e] - me), 2.0);
                }
            }

            this.Variability = 100 * (float)Math.Sqrt(value) / order; // *100/(double)order;
        }
        #endregion

        /// <summary> Determine and sets the elements and level property. </summary>
        public void SetElements() { //// protected
            var num = this.DecimalNumber;
            this.Level = 0;
            this.GLevel = 0;
            this.elemList = new Collection<short>();
            var order = this.GSystem.Order;
            var degree = this.GSystem.Degree;
            for (byte e = 0; e < order; e++) {
                var rest = num % degree;
                if (rest >= 0) {
                    this.ElementList.Add((byte)rest);
                    if (e < this.ElementList.Count && this.ElementList[e] > 0) {
                        this.GLevel += this.ElementList[e];
                        this.Level += 1;
                    }
                }

                num = (num - rest) / this.GSystem.Degree;
            }
            //// Used by Qualifiers
            this.Properties[GenProperty.Level] = this.Level;
        }

        /// <summary>
        /// Similarities the value.
        /// </summary>
        /// <param name="givenStructure">The given structure.</param>
        /// <returns> Returns value. </returns>
        public int SimilarityValue(FiguralStructure givenStructure) {
            var order = Math.Min(this.GSystem.Order, givenStructure.GSystem.Order);
            var value = 0;
            for (byte e = 0; e < order; e++) {
                if (e >= this.ElementList.Count || this.ElementList[e] <= 0) {
                    continue;
                }

                if (e >= givenStructure.ElementList.Count || givenStructure.ElementList[e] <= 0) {
                    continue;
                }

                if (this.ElementList[e] == givenStructure.ElementList[e]) {
                    value++;
                }
            }

            return (100 * value) / order;
        }

        /// <summary> Mean distance of nonzero bits. </summary>
        /// <returns>Returns value.</returns>
        protected float MeanElement() {
            var order = this.GSystem.Order;
            var value = (float)this.GLevel / order;

            return value < 1 ? 1 : value;
        }

        /// <summary> Determine and sets the elements and level property. </summary>
        protected void SetElementsFromDifferences() {
            decimal num = 0; //// this.Data;
            this.Level = 0;
            this.GLevel = 0;
            this.elemList = new Collection<short>();
            this.diffList = new Collection<short>();
            short sum = 0;
            var order = this.GSystem.Order;
            var degree = this.GSystem.Degree;
            var halfDegree = (short)(degree / 2);
            for (byte e = 0; e < order; e++) {
                var eval = (byte)(num % degree);
                var diff = (short)(eval - halfDegree);
                this.diffList.Add(diff);
                sum += diff;
                this.ElementList.Add(sum);
                if (e >= this.ElementList.Count) {
                    continue;
                }

                if (this.ElementList[e] != 0) {
                    this.GLevel += this.ElementList[e];
                    this.Level += 1;
                }

                num /= degree;
            }

            this.Properties[GenProperty.Level] = this.Level; // used with Qualifiers
        }

        #region Code packing
        /// <summary>
        /// Unpacks the code.
        /// </summary>
        /// <param name="packedCode">The packed code.</param>
        /// <returns> Returns value. </returns>
        private static string UnpackCode(string packedCode) {
            Contract.Requires(packedCode != null);
            //// 2014/12 Time optimization
            var sb = new StringBuilder();
            //// string structCode = string.Empty;
            var packedCodes = packedCode.Split(',');
            Array.ForEach(
                packedCodes,
                pc => {
                    if (pc.Contains('*')) {
                        var position = pc.IndexOf('*');
                        var cnt = int.Parse(pc.Substring(0, position), CultureInfo.CurrentCulture);
                        var code = pc.Substring(position + 1);
                        for (var i = 1; i <= cnt; i++) {
                            sb.Append(code);
                            sb.Append(",");
                        }
                    }
                    else {
                        sb.Append(pc);
                        sb.Append(",");
                    }
                });

            return sb.ToString();
        }

        /// <summary>
        /// Packs the code.
        /// </summary>
        /// <param name="structCode">The code.</param>
        /// <returns> Returns value. </returns>
        private static string PackCode(string structCode) {
            Contract.Requires(structCode != null);
            //// 2014/12 Time optimization
            var sb = new StringBuilder();
            var codes = structCode.Split(',');
            var lastCode = codes[0];
            var cnt = 0;
            Array.ForEach(
                codes,
                code => {
                    if (string.CompareOrdinal(code, lastCode) != 0) {
                        if (cnt > 1) {
                            sb.Append(cnt.ToString(CultureInfo.CurrentCulture));
                            sb.Append("*");
                        }

                        sb.Append(lastCode);
                        sb.Append(",");
                        lastCode = code;
                        cnt = 0;
                    }

                    cnt++;
                });

            if (cnt > 1) {
                sb.Append(cnt.ToString(CultureInfo.CurrentCulture));
                sb.Append("*");
            }

            sb.Append(lastCode);
            return sb.ToString();
        }
        #endregion
    }
}
