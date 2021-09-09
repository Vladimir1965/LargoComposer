// <copyright file="RhythmicModality.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Rhythmical modality.  </summary>
    /// <remarks> Rhythmical modality is defined by its number and appropriateness 
    /// to rhythmical GSystem. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class RhythmicModality : BinarySchema, IRhythmic, IModalStruct
    {
        #region Constructors
        /// <summary> Initializes a new instance of the RhythmicModality class.  Serializable. </summary>
        public RhythmicModality() {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public RhythmicModality(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Modality number.</param>
        /// <param name="givenBitShift">Modus given as bit shift.</param>
        public RhythmicModality(GeneralSystem givenSystem, long number, byte givenBitShift)
            : base(givenSystem, BinaryNumber.Transposition(givenSystem, number, givenBitShift)) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenBitArray">Bit array.</param>
        public RhythmicModality(GeneralSystem givenSystem, BitArray givenBitArray)
            : base(givenSystem, givenBitArray) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicModality class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public RhythmicModality(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
        }

        /// <summary> Initializes a new instance of the RhythmicModality class. </summary>
        /// <param name="structure">Binary structure.</param>
        public RhythmicModality(BinaryStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
        }
        #endregion

        #region Interface - simple properties

        /// <summary> Gets schema of elements. </summary>
        /// <value> Property description. </value>
        public override string ElementSchema => this.ElementString(false);

        /// <summary>
        /// Gets the combo outline.
        /// </summary>
        /// <value>
        /// The combo outline.
        /// </value>
        public string ComboOutline {
            get {
                var s = this.ElementString(true);
                return string.Format("{0}/ {1}", this.GSystem.Order, s);
            }
        }

        #endregion

        #region Interface - object properties
        /// <summary> Gets rhythmical system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicSystem RhythmicSystem => (RhythmicSystem)this.GSystem;

        #endregion

        #region Static factory methods
        /// <summary>
        /// Get NewRhythmicModality.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicModality GetNewRhythmicModality(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var hm = new RhythmicModality(givenSystem, structuralCode);
            hm.DetermineBehavior();
            return hm;
        }

        /// <summary>
        /// Get NewRhythmicModality.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Structural Number.</param>
        /// <param name="givenTransposition">Given Transposition.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicModality GetNewRhythmicModality(GeneralSystem givenSystem, long number, byte givenTransposition) {
            Contract.Requires(givenSystem != null);
            var hm = new RhythmicModality(givenSystem, number, givenTransposition);
            hm.DetermineBehavior();
            return hm;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the RhythmicModality object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return GetNewRhythmicModality(this.GSystem, this.GetStructuralCode);
        }

        /// <summary> Evaluate properties of the structure. </summary>
        public override void DetermineBehavior() {
            this.ComputeRhythmicProperties();
        }
        #endregion

        #region Substructures
        /// <summary>
        /// Rhythmical Subshapes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> Subshapes(GeneralQualifier genQualifier, int limit) {
            var rv = StructuralVarietyFactory.NewRhythmicShapeModalVariety(
                                        StructuralVarietyType.BinarySubstructuresOfModality,
                                        this,
                                        genQualifier,
                                        limit);
            return rv.StructList;
        }

        /// <summary>
        /// Rhythmical Subshapes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> Subshapes() {
            var rv = StructuralVarietyFactory.NewRhythmicShapeModalVariety(
                                        StructuralVarietyType.BinarySubstructuresOfModality,
                                        this,
                                        null,
                                        10000);
            return rv.StructList;
        }

        /// <summary>
        /// Rhythmical Substructures.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> Substructures(GeneralQualifier genQualifier, int limit) {
            var rv = StructuralVarietyFactory.NewRhythmicStructModalVariety(
                                        StructuralVarietyType.FiguralSubstructuresOfModality,
                                        this,
                                        genQualifier,
                                        limit);
            return rv.StructList;
        }

        /// <summary>
        /// Rhythmic Substructures.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> Substructures() {
            var rv = StructuralVarietyFactory.NewRhythmicStructModalVariety(
                                        StructuralVarietyType.FiguralSubstructuresOfModality,
                                        this,
                                        null,
                                        10000);
            return rv.StructList;
        }
        #endregion

        #region String representation

        /// <summary>
        /// Binary schema of the structure.
        /// </summary>
        /// <param name="extended">if set to <c>true</c> [extended].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string ElementString(bool extended) {
            var s = new StringBuilder();
            for (byte e = 0; e < this.GSystem.Order; e++) {
                s.Append(this.IsOn(e) ? 'V' : '-');
                if (extended) {
                    s.Append(' ');
                }
            }

            return s.ToString().Trim();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append("Rhythmical modality\r\n");
            s.AppendLine(base.ToString());
            return s.ToString();
        }

        #endregion
    }
}
