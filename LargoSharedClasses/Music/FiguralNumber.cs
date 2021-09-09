// <copyright file="FiguralNumber.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Figural Number.
    /// </summary>
    public sealed class FiguralNumber
    {
        /// <summary>
        /// General system.
        /// </summary>
        private readonly GeneralSystem gsystem; //// readonly

        /// <summary> List of elements. </summary>
        private Collection<short> elemList;

        //// private Collection<short> diffList;

        #region Constructors
        /// <summary> Initializes a new instance of the FiguralNumber class.  Serializable. </summary>
        public FiguralNumber() {
        }

        /// <summary>
        /// Initializes a new instance of the FiguralNumber class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public FiguralNumber(GeneralSystem givenSystem, decimal number) {
            Contract.Requires(givenSystem != null);
            this.gsystem = givenSystem;
            this.DecimalNumber = number;
            this.SetElements(); // 20081224
            //// this.CheckInstance();
        }

        /// <summary> Initializes a new instance of the FiguralNumber class. </summary>
        /// <param name="structure">Figural structure.</param>
        public FiguralNumber(FiguralNumber structure) {
            Contract.Requires(structure != null);

            this.gsystem = structure.GSystem;
            this.DecimalNumber = structure.DecimalNumber;
            this.Level = structure.Level;
            this.GLevel = structure.GLevel;
            this.SetElements();
            //// this.CheckInstance();
        }
        #endregion

        #region Properties

        /// <summary> Gets or sets instance number of the structure. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public decimal DecimalNumber { get; set; }

        /// <summary> Gets or sets level of the structure. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public byte Level { get; set; }

        /// <summary> Gets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private GeneralSystem GSystem {
            get {
                Contract.Ensures(Contract.Result<GeneralSystem>() != null);
                Contract.Ensures(Contract.Result<GeneralSystem>().Order > 0);
                if (this.gsystem == null) {
                    throw new InvalidOperationException("G-system is null.");
                }

                return this.gsystem;
            }
        }

        /// <summary> Gets or sets level of the structure. Sum of digits. </summary>
        /// <value> Property description. </value>
        private int GLevel { get; set; }

        /// <summary> Gets elements of the figure. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private Collection<short> ElementList {
            get {
                Contract.Ensures(Contract.Result<Collection<short>>() != null);
                if (this.elemList == null) {
                    throw new InvalidOperationException("List of elements is null.");
                }

                return this.elemList;
            }
        }
        #endregion

        #region Static Functions
        /// <summary> Returns if selected bit is ON. </summary>
        /// <param name="number">Number of instance.</param>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        public static bool ElementIsOn(decimal number, byte element) {
            return ((long)number & BitAt(element)) != 0;
        }
        #endregion

        #region Element gettings
        /// <summary> Returns if selected bit is ON. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        public bool IsOn(byte element) {
            return element < this.ElementList.Count && this.ElementList[element] != 0;
        }

        #endregion

        #region Element settings
        /// <summary>
        ///  Sets selected bit to value.
        /// </summary>
        /// <param name="element">Number of element.</param>
        /// <param name="value">Value of element.</param>
        public void SetElement(byte element, byte value) {
            if (element < this.ElementList.Count) {
                this.ElementList[element] = value;
            }
        }
        #endregion

        /// <summary> Determine and sets the level property. </summary>
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

        #region Private static
        /// <summary> Returns empty number with only bit at position 'i' set. </summary>
        /// <param name="element">Element of system.</param>
        /// <returns> Returns value.</returns>
        private static long BitAt(byte element) {
            return (long)1 << element;
        }
        #endregion

        /// <summary> Determine and sets the elements and level property. </summary>
        private void SetElements() {
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
            //// this.Properties[GenProperty.Level] = (float)this.Level;
        }
    }
}
