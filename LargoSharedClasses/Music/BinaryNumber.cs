// <copyright file="BinaryNumber.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music {
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Xml.Serialization;
    using JetBrains.Annotations;
    using LargoSharedClasses.Interfaces;

    /// <summary>
    /// Binary Number.
    /// </summary>
    public sealed class BinaryNumber : GeneralOwner, IGeneralStruct {
        /// <summary>
        /// All Bites On.
        /// </summary>
        public const long AllBitesOn = 0xFFFFFFFF;

        /// <summary>
        /// General system.
        /// </summary>
        private GeneralSystem gsystem; //// readonly

        #region Constructors
        /// <summary> Initializes a new instance of the BinaryNumber class.  Serializable. </summary>
        public BinaryNumber()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BinaryNumber class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public BinaryNumber(GeneralSystem givenSystem, long number)
        {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.Number = number;
            this.DetermineLevel();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary>
        /// Initializes a new instance of the BinaryNumber class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public BinaryNumber(GeneralSystem givenSystem, decimal number)
        {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.Number = (long)number;
            this.DetermineLevel();
            //// 2014/12 Time optimization
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the BinaryNumber class. </summary>
        /// <param name="structure">Binary structure.</param>
        public BinaryNumber(BinaryNumber structure)
        {
            Contract.Requires(structure != null);

            this.gsystem = structure.GSystem;
            this.Number = structure.Number;
            this.Level = structure.Level;
            //// this.CheckInstance();
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public BinaryNumber Modality { get; set; }

        /// <summary> Gets or sets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        //// [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures")]
        [System.Diagnostics.Contracts.Pure]
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

        /// <summary> Gets or sets binary structure written as decimal number.  </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public long Number { get; set; }

        /// <summary> Gets or sets binary structure written as decimal number.  </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        [System.Diagnostics.Contracts.Pure]
        public decimal DecimalNumber {
            get => this.Number;

            set => this.Number = (long)value;
        }

        /// <summary> Gets binary structure written as decimal number.  </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        [System.Diagnostics.Contracts.Pure]
        public long ClassNumber => DetermineClassNumber(this.GSystem.Order, this.Number);

        /// <summary> Gets or sets level, i.e. number of ones in the structure. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public byte Level { get; set; }
        #endregion

        #region Static functions
        /// <summary> Returns empty number with only bit at position 'i' set. </summary>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        public static long BitAt(byte element) {
            return (long)1 << element;
        }

        /// <summary> Returns full number with only zero bit at position 'i'. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        public static long NotBitAt(byte element) {
            return AllBitesOn - BitAt(element);
        }

        /// <summary> Returns if selected bit is ON. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        public static bool ElementIsOn(long number, byte element) {
            return (number & BitAt(element)) != 0;
        }

        /// <summary> Schema of distances. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="number">Number of instance.</param>
        /// <returns> Returns list. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static Collection<byte> DistSchema(byte order, long number) {
            var lb = new Collection<byte>();
            short lastPosition = -1;
            int di, sumDistance = 0;
            for (byte j = 0; j < order; j++) {
                if (!ElementIsOn(number, j))
                {
                    continue;
                }

                if (lastPosition != -1) {
                    di = j - lastPosition;
                    sumDistance += di;
                    if (di >= byte.MinValue && di <= byte.MaxValue) {
                        lb.Add((byte)di);
                    }
                }

                lastPosition = j;
            }

            di = order - sumDistance;
            if (di >= byte.MinValue && di <= byte.MaxValue) {
                lb.Add((byte)di);
            }

            return lb;
        }

        /// <summary> Returns if selected bit is OFF. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        public static bool ElementIsOff(long number, byte element) {
            return (number & BitAt(element)) == 0;
        }

        /// <summary> Returns number of bits in selected Range, that are ON. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        /// <returns> Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        public static byte ElementIsOnInRange(long number, byte elementFrom, short elementTo) {
            byte s = 0;
            for (var e = elementFrom; e <= elementTo; e++) {
                if (ElementIsOn(number, e)) {
                    s++;
                }
            }

            return s;
        }

        /// <summary> Returns number of bits in selected Range, that are OFF. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static byte ElementIsOffInRange(long number, byte elementFrom, short elementTo) {
            byte s = 0;
            for (var e = elementFrom; e <= elementTo; e++) {
                if (ElementIsOff(number, e)) {
                    s++;
                }
            }

            return s;
        }

        /// <summary> Determine and sets the level property. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="number">Number of instance.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static byte DetermineLevel(byte order, long number) {
            byte level = 0;
            for (byte e = 0; e < order; e++) {
                if (ElementIsOn(number, e)) {
                    level++;
                }
            }

            return level;
        }

        /// <summary> Rotate the number, moving first bit to the end. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="number">Number of instance.</param>
        /// <returns>Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        public static long TransposeLeft(byte order, long number) {
            if (order == 0) {
                return number;
            }

            int integerOrder = order;
            var shift = (byte)(integerOrder - 1);
            return (number & 1) << shift | number >> 1;
        }

        /// <summary> Rotate the number to given Transposition. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="givenClassNumber">Number of modality.</param>
        /// <param name="transpositionNumber">Transposition number.</param>
        /// <returns> Returns value.</returns>
        public static long Transposition(byte order, long givenClassNumber, byte transpositionNumber) {
            var number = givenClassNumber;
            if (transpositionNumber > 0) {
                number *= (long)Math.Pow(2, order - transpositionNumber);
            }

            var limit = (long)(Math.Pow(2, order) - 1);
            number = (number == limit || limit == 0) ? number : number % limit;
            return number;
        }

        /// <summary>
        /// Rotate the number to given Transposition.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenClassNumber">Number of modality.</param>
        /// <param name="transpositionNumber">Transposition number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static long Transposition(GeneralSystem givenSystem, long givenClassNumber, byte transpositionNumber) {
            Contract.Requires(givenSystem != null);
            return givenSystem == null ? givenClassNumber : Transposition(givenSystem.Order, givenClassNumber, transpositionNumber);
        }

        /// <summary> Returns the transposition with the lowest number. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="number">Number of instance.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static long DetermineClassNumber(byte order, long number) {
            var minNum = number;
            for (byte n = 1; n < order; n++) {
                number = TransposeLeft(order, number);
                if (number < minNum) {
                    minNum = number;
                }
            }

            return minNum;
        }
        #endregion

        /// <summary> Test of emptiness. </summary>
        /// <returns> Returns value. </returns>
        public bool IsEmptyStruct() { //// virtual
            return this.Level == 0;
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public bool IsValidStruct() { //// virtual
            return true;
        }

        #region Bit gettings
        /// <summary> Gets a value indicating whether is the bit ON. </summary>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value. </returns>
        public bool IsOn(byte element) {
            return (this.Number & BitAt(element)) != 0;
        }

        /// <summary> Gets a value indicating whether is the bit OFF. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        public bool IsOff(byte element) {
            return (this.Number & BitAt(element)) == 0;
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
            this.Number = AllBitesOn;
        }

        /// <summary> Sets selected bit ON. </summary>
        /// <param name="element">Requested element.</param>
        public void On(byte element) {
            this.Number = this.Number | BitAt(element);
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
            this.Number = 0;
        }

        /// <summary> Sets selected bit OFF. </summary>
        /// <param name="element">Requested element.</param>
        public void Off(byte element) {
            this.Number = this.Number & ~BitAt(element);
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

        /// <summary> Determine and sets properties. </summary>
        public void DetermineBehavior() {
        }

        /// <summary> Determine and sets the level property. </summary>
        public void DetermineLevel() {
            this.Level = this.IsOnInRange(0, (byte)(this.GSystem.Order - 1));
            //// this.Properties[GenProperty.Level] = (float)this.level; // used with Qualifiers
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return this.Number.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary> Determine if it is a substructure of given mask. </summary>
        /// <param name="givenMask">Masking number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private bool CoverIn(long givenMask) {
            return this.Number == (givenMask & this.Number);
        }
    }
}
