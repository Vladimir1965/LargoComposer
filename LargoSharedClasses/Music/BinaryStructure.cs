// <copyright file="BinaryStructure.cs" company="Traced-Ideas, Czech republic">
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
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Binary Structure.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class BinaryStructure : GeneralOwner, IComparable, IGeneralStruct {
        #region Fields
        /// <summary>
        /// Bit Array.
        /// </summary>
        private BitArray bitArray;

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
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the BinaryStructure class.  Serializable. </summary>
        public BinaryStructure() {
        }

        /// <summary> Initializes a new instance of the BinaryStructure class. </summary>
        /// <param name="givenSystem">Abstract system.</param>
        /// <param name="givenStructuralCode">Structural code.</param>
        public BinaryStructure(GeneralSystem givenSystem, string givenStructuralCode) {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.SetStructuralCode(givenStructuralCode);
            this.DetermineLevel();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the BinaryStructure class. </summary>
        /// <param name="givenSystem">Abstract system.</param>
        /// <param name="givenBitArray">Bit array.</param>
        public BinaryStructure(GeneralSystem givenSystem, BitArray givenBitArray) {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.bitArray = givenBitArray;
            this.DetermineLevel();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the BinaryStructure class. </summary>
        /// <param name="givenSystem">Abstract system.</param>
        /// <param name="number">Number of structure.</param>
        public BinaryStructure(GeneralSystem givenSystem, long number) {
            this.gsystem = givenSystem ?? throw new InvalidOperationException("G-system is null.");
            this.DecimalNumber = number;
            this.SetNumber(number);

            this.DetermineLevel();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the BinaryStructure class. </summary>
        /// <param name="structure">Binary structure.</param>
        public BinaryStructure(BinaryStructure structure) {
            Contract.Requires(structure != null);

            this.gsystem = structure.GSystem;
            this.bitArray = (BitArray)structure.BitArray.Clone();
            this.level = structure.Level;
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }
        #endregion

        #region Properties
        /// <summary> Gets BitArray. </summary>
        /// <value> Property description. </value>
        public BitArray BitArray {
            get {
                Contract.Ensures(Contract.Result<BitArray>() != null);
                if (this.bitArray == null) {
                    throw new InvalidOperationException("Bit array is null.");
                }

                return this.bitArray;
            }
        }

        /// <summary> Gets or sets Number. </summary>
        /// <value> Property description. </value>
        public long Number { get; set; }

        /// <summary> Gets or sets DecimalNumber. </summary>
        /// <value> Property description. </value>
        public decimal DecimalNumber { get; set; }

        /// <summary> Gets or sets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        //// [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures")]
        public GeneralSystem GSystem {
            get {
                Contract.Ensures(Contract.Result<GeneralSystem>() != null);
                Contract.Ensures(Contract.Result<GeneralSystem>().Order > 0);
                if (this.gsystem == null) {
                    throw new InvalidOperationException("G-system is null.");
                }

                return this.gsystem;
            }

            set => this.gsystem = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary> Gets or sets modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public IGeneralStruct Modality { get; set; }

        /// <summary> Gets or sets level, i.e. number of ones in the structure. </summary>
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

        /// <summary>
        /// Gets or sets the class code.
        /// </summary>
        /// <value>
        /// The class code.
        /// </value>
        public string ClassCode { get; set; }
        #endregion

        #region Schemas
        /// <summary> Gets positions of nonzero bits. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<byte> BitPlaces {
            get {
                Contract.Ensures(Contract.Result<Collection<byte>>() != null);
                var places = new Collection<byte>();
                var order = this.GSystem.Order;
                for (byte e = 0; e < order; e++) {
                    if (this.IsOn(e)) {
                        places.Add(e);
                    }
                }

                return places;
            }
        }

        /// <summary> Gets distances of nonzero bits. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<byte> BitDistances {
            get {
                var places = this.BitPlaces;
                var distances = new Collection<byte>();
                var order = this.GSystem.Order;
                if (this.Level > 1) {
                    byte p = 0, r = 0;
                    var lastL = (byte)(this.Level - 1);
                    for (byte lev = 0; lev < lastL; lev++) {
                        if (lev + 1 < places.Count) {
                            p = places[lev];     ////  p = this.PlaceAtLevel(lev);
                            r = places[lev + 1]; //// r = this.PlaceAtLevel((byte)(lev + 1));
                        }

                        distances.Add(this.GSystem.FormalLength(r - p)); //// + order
                    }

                    if (lastL < places.Count) {
                        p = places[lastL];     //// p = this.PlaceAtLevel(lastL);
                    }

                    if (places.Count > 0) {
                        r = places[0];         //// r = this.PlaceAtLevel(0);
                    }

                    distances.Add(this.GSystem.FormalLength(r - p)); //// + order
                }
                else {
                    distances.Add(order);
                }

                return distances;
            }
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <returns> Returns value. </returns>
        public long GetNumber {
            get {
                //// Contract.Requires(this.BitArray != null);
                if (this.bitArray == null) {
                    return 0;
                }

                var r = this.bitArray.Count;
                long n = 0;
                for (byte bit = 0; bit < r; bit++) {
                    if (this.bitArray[bit]) {
                        n |= BinaryNumber.BitAt(bit);
                    }
                }

                return n;
            }
        }

        /// <summary>
        /// Gets the class structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        public BinaryStructure GetClassStructure {
            get {
                //// Contract.Requires(this.BitArray != null);
                if (this.bitArray == null) {
                    return null;
                }

                var number = this.GetNumber;
                var classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, number);
                var chs = new BinaryStructure(this.GSystem, classNumber);
                return chs;
            }
        }

        /// <summary>
        /// Gets StructuralCode.
        /// </summary>
        /// <returns> Returns value. </returns>
        /// <value> Property description. </value>
        public string GetStructuralCode => this.structuralCode ?? (this.structuralCode = this.DetermineStructuralCode());

        #endregion

        #region Static functions

        #endregion

        #region Static operators

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(BinaryStructure structure1, BinaryStructure structure2) { return object.Equals(structure1, structure2);  }
        //// public static bool operator !=(BinaryStructure structure1, BinaryStructure structure2) { return !object.Equals(structure1, structure2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(BinaryStructure object1, BinaryStructure object2) {
            if (object1 != null && object2 != null && (object1.Number > 0 || object2.Number > 0)) {
                return object1.Number < object2.Number;
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
        public static bool operator >(BinaryStructure object1, BinaryStructure object2) {
            if (object1 != null && object2 != null && (object1.Number > 0 || object2.Number > 0)) {
                return object1.Number > object2.Number;
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
        public static bool operator <=(BinaryStructure object1, BinaryStructure object2) {
            if (object1 != null && object2 != null && (object1.Number > 0 || object2.Number > 0)) {
                return object1.Number <= object2.Number;
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
        public static bool operator >=(BinaryStructure object1, BinaryStructure object2) {
            if (object1 != null && object2 != null && (object1.Number > 0 || object2.Number > 0)) {
                return object1.Number >= object2.Number;
            }

            return false;
        }

        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="value">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object value) {
            if (!(value is BinaryStructure bs)) {
                return 0;
            }

            if (this.Level < bs.Level) {
                return -1;
            }

            return this.Level > bs.Level ? 1 : string.Compare(this.BitArray.ToString(), bs.BitArray.ToString(), StringComparison.Ordinal);
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a BinaryStructure");
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
            return this.BitArray.GetHashCode();
        }
        #endregion

        #region Public virtual methods
        /// <summary> Test of emptiness. </summary>
        /// <returns> Returns value. </returns>
        public virtual bool IsEmptyStruct() {
            return this.level == 0;
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public virtual bool IsValidStruct() {
            return true;
        }

        /// <summary> Evaluate properties of the structure. Used in descendant objects. 
        /// Must be virtual, because of call from StructuralVariety. </summary>
        public virtual void DetermineBehavior() {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Resets the structure.
        /// </summary>
        public void ResetStructure() {
            this.structuralCode = null;
        }

        /// <summary> Makes a deep copy of the BinaryStructure object. </summary>
        /// <returns> Returns object. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override object Clone() {
            return new BinaryStructure(this.GSystem, this.GetStructuralCode);
        }

        /// <summary>
        /// Determine and sets the level property.
        /// </summary>
        /// <param name="givenBit">Given bit of the structure.</param>
        /// <returns> Returns level containing the given bit. </returns>
        public byte LevelOfBit(byte givenBit) {
            if (givenBit == 0) {
                return 0;
            }

            var bitLevel = -1;
            //// byte order = this.GSystem.Order;
            for (byte e = 0; e <= givenBit; e++) {
                if (this.IsOn(e)) {
                    bitLevel++;
                }
            }

            var lev = (byte)((bitLevel >= 0) ? bitLevel : 0);
            return lev;
        }

        /// <summary> Determine and sets the level property. </summary>
        public void DetermineLevel() {
            this.level = this.IsOnInRange(0, (byte)(this.GSystem.Order - 1));
            this.Properties[GenProperty.Level] = this.level; // used with Qualifiers
        }
        #endregion

        #region Bit gettings
        /// <summary> Gets a value indicating whether is the bit ON. </summary>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value. </returns>
        public bool IsOn(byte element)
        {
            return element < this.BitArray.Count && this.BitArray[element];
        }

        /// <summary> Gets a value indicating whether is the bit OFF. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        public bool IsOff(byte element) {
            if (element >= this.BitArray.Count) {
                return true;
            }

            return !this.BitArray[element];
        }

        /// <summary> Returns number of bits in selected Range, that are ON. </summary>
        /// <param name="elementFrom">First element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        /// <returns> Returns value.</returns>
        public byte IsOnInRange(byte elementFrom, byte elementTo) {
            byte s = 0;
            for (var e = elementFrom; e <= elementTo; e++) {
                if (this.IsOn(e)) {
                    s++;
                }
            }

            return s;
        }

        /// <summary> Returns number of bits in selected Range, that are OFF. </summary>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        /// <returns>Returns value.</returns>
        public byte IsOffInRange(byte elementFrom, byte elementTo) {
            byte s = 0;
            for (var e = elementFrom; e <= elementTo; e++) {
                if (this.IsOff(e)) {
                    s++;
                }
            }

            return s;
        }
        #endregion

        #region Bit setting
        /// <summary> Sets all bits ON. </summary>
        public void OnAll() {
            this.BitArray.SetAll(true);
        }

        /// <summary> Sets selected bit ON. </summary>
        /// <param name="element">Requested element.</param>
        public void On(byte element) {
            if (element >= this.BitArray.Length) {
                return;
            }

            this.BitArray[element] = true;
        }

        /// <summary>
        /// Sets bits in selected Range ON. </summary>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        public void OnRange(byte elementFrom, byte elementTo) {
            for (var e = elementFrom; e <= elementTo; e++) {
                this.On(e);
            }
        }

        /// <summary> Sets all bits OFF. </summary>
        public void OffAll() {
            this.BitArray.SetAll(false);
        }

        /// <summary> Sets selected bit OFF. </summary>
        /// <param name="element">Requested element.</param>
        public void Off(byte element) {
            if (element >= this.BitArray.Length) {
                return;
            }

            this.BitArray[element] = false;
        }

        /// <summary> Sets bits in selected Range OFF. </summary>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        public void OffRange(byte elementFrom, byte elementTo) {
            for (var e = elementFrom; e <= elementTo; e++) {
                this.Off(e);
            }
        }
        #endregion

        #region String representation
        /// <summary> Binary schema of the structure. </summary>
        /// <returns> Returns value. </returns>
        public virtual string ElementString() {
            var s = new StringBuilder();
            //// string test =  this.BitArray.ToString();
            for (byte e = 0; e < this.GSystem.Order; e++) {
                s.Append(this.IsOn(e) ? '1' : '0');
            }

            return s.ToString();
        }

        /// <summary> Inverse binary schema of the structure. </summary>
        /// <returns> Returns value. </returns>
        public string InverseElementString() {
            var s = new StringBuilder();
            for (int e = (byte)(this.GSystem.Order - 1); e >= 0; e--) {
                s.Append(this.IsOn((byte)e) ? '1' : '0');
            }

            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            if (this.bitArray == null) {
                return string.Empty;
            }

            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "<{0}", this.BitArray);
            s.AppendFormat(CultureInfo.CurrentCulture, "L{0,2}", this.level.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            s.Append(" ");
            s.Append(this.ElementString());
            //// s.Append(givenClassNumber); s.Append:this.tran; s.Append:this.rota; 
            return s.ToString();
        }
        #endregion

        #region Structural code

        /// <summary> Determine and sets the level property. </summary>
        /// <param name="givenStructuralCode">Structural code.</param>
        public void SetStructuralCode(string givenStructuralCode) {
            this.structuralCode = givenStructuralCode;
            this.bitArray = new BitArray(this.GSystem.Order);

            if (string.IsNullOrWhiteSpace(this.structuralCode)) {
                return;
            }
            
            var codes = this.structuralCode.Split(',');
            Array.ForEach(
                codes, 
                code => {
                int element = byte.Parse(code, CultureInfo.CurrentCulture);
                if (element < this.BitArray.Length) {
                    this.BitArray[element] = true;
                }
                });
        }

        /// <summary> Determine and sets the level property. </summary>
        /// <returns>Returns value.</returns>
        public string DetermineStructuralCode() {
            var code = string.Empty;
            if (this.level == 0) {
                return code;
            }

            var places = this.BitPlaces;
            var first = true;
            foreach (var place in places) {
                if (first) {
                    first = false;
                } 
                else {
                    code += ",";
                }

                code += place.ToString(CultureInfo.CurrentCulture);
            }

            return code;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Sets the number.
        /// </summary>
        /// <param name="number">The number.</param>
        private void SetNumber(long number) {
            this.Number = number;
            this.bitArray = new BitArray(this.gsystem.Order);
            var bn = new BinaryNumber(this.gsystem, number);
            for (byte i = 0; i < this.gsystem.Order && i < this.BitArray.Count; i++) {
                this.BitArray[i] = bn.IsOn(i);
            }
        }
        #endregion
    }
}
