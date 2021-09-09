// <copyright file="MelodicSystem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Melodic system. </summary>
    /// <remarks>
    /// Melodic system is subclass of GSystem.
    /// It is defined by its degree and order and used for figuration. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class MelodicSystem : GeneralSystem {
        #region Fields
        /// <summary>
        /// Maximum Degree.
        /// </summary>
        public const int MaximumDegree = 24;

        /// <summary> Musical symbols. </summary>
        private string[] musSymbols;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MelodicSystem class.  Serializable. </summary>
        public MelodicSystem() {
        }

        /// <summary> Initializes a new instance of the MelodicSystem class. </summary>
        /// <param name="degree">Degree of system.</param>/// 
        /// <param name="order">Order of system.</param>
        public MelodicSystem(byte degree, byte order)
            : base(degree, order) {
                if (degree <= 0 || degree > MaximumDegree) {
                    throw new ArgumentException("Incorrect degree of system.");
                }

                if (order == 0) {
                    throw new ArgumentException("Order of system must not be 0.");
                }

                this.MakeSymbolArray();
        }
        #endregion

        #region Properties
        /// <summary> Gets Musical symbols. </summary>
        /// <value> Property description. </value>
        public Collection<string> MusicalSymbols {
            get {
                Contract.Ensures(Contract.Result<Collection<string>>() != null);
                if (this.musSymbols == null) {
                    throw new InvalidOperationException("String of symbols is null.");
                }

                return new Collection<string>(this.musSymbols);
            }
        }

        /// <summary> Gets or sets string of musical symbols. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public string StringOfSymbols { get; set; }
        #endregion

        #region Public static methods
        /// <summary> Returns tone symbol for the given element. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value. </returns>
        public static string GuessSymbolForElement(byte element) {
            Contract.Requires(element <= MaximumDegree);
            string[] sym = { "c", "d", "e", "f", "g", "a", "h", "i", "j", "k", "l", "m", 
                               "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var value = sym[element];
            return value;
        }
        #endregion

        #region Substructures
        /// <summary> List of melodical modalities in the given system. </summary>
        /// length Order of system
        /// anAmbit Degree of system   
        /// <returns> Returns value. </returns>
        public Collection<MelodicStructure> Instances() {
            var variety = StructuralVarietyFactory.NewMelStructuralVariety(
                         StructuralVarietyType.Instances, this, null, 10000);
            return variety.StructList;
        }
        #endregion

        #region String representation
        /// <summary> Returns symbols for given element. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value.</returns>
        public string Symbol(short element) {
            Contract.Requires(element < this.MusicalSymbols.Count);
            if (element >= 0 && element < this.Degree) { ////this.Order //// this.musSymbols != null && 
                return this.musSymbols[element];
            }

            return string.Empty;
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendLine("Melodic system");
            s.AppendLine(base.ToString());
            return s.ToString();
        }
        #endregion

        #region Private static methods
        /// <summary> Makes array of symbols used in this GSystem. </summary>
        private void MakeSymbolArray() {
            Contract.Requires(this.Degree <= MaximumDegree);
            var str = new StringBuilder();
            if (this.Degree <= MaximumDegree) {
                var s = string.Empty;
                this.musSymbols = new string[this.Degree];  ////this.Order
                for (byte i = 0; i < this.Degree; i++) { ////this.Order
                    if (i <= MaximumDegree) {
                        s = GuessSymbolForElement(i);
                    }

                    str.Append(s);
                    if (i < this.Degree - 1) { ////this.Order
                        str.Append(",");
                    }

                    if (i <= this.musSymbols.GetUpperBound(0)) {
                        this.musSymbols.SetValue(s, i);
                    }
                }
            }

            this.StringOfSymbols = str.ToString();
        }
        #endregion
    }
}
