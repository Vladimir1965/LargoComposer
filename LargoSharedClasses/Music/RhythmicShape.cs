// <copyright file="RhythmicShape.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Rhythm;
using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Rhythmical shape. </summary>
    /// <remarks> Rhythmical shape represents simplified rhythm of one bar. It is always 
    /// assigned to certain rhythmical modality (and to rhythmical GSystem).
    /// It has also inner characteristics (mobility, tension, ..). </remarks>
    [Serializable]
    [XmlRoot]
    public class RhythmicShape : BinarySchema, IRhythmic, IModalStruct
    {
        //// Properties:mean, mobility, tension, jazz,entropy
        #region Constructors
        /// <summary> Initializes a new instance of the RhythmicShape class.  Serializable. </summary>
        public RhythmicShape() {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="bitArray">Bit array.</param>
        public RhythmicShape(GeneralSystem givenSystem, BitArray bitArray)
            : base(givenSystem, bitArray) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public RhythmicShape(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class.
        /// </summary>
        /// <param name="shape">Rhythmic shape.</param>
        public RhythmicShape(BinaryStructure shape)
            : base(shape) {
            Contract.Requires(shape != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class. 
        /// </summary>
        /// <param name="sysOrder">System order.</param>
        /// <param name="structure">Rhythmical structure.</param>
        public RhythmicShape(byte sysOrder, IGeneralStruct structure)
            : base(RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, sysOrder), (string)null) {
            Contract.Requires(structure != null);

            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (structure.IsOn(e)) {
                    this.On(e);
                }
            }

            this.DetermineLevel();
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public RhythmicShape(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public RhythmicShape(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, (long)number) {
        }
        #endregion

        #region Interface
        /// <summary> Gets rhythmical system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicSystem RhythmicSystem => (RhythmicSystem)this.GSystem;

        /// <summary> Gets or sets rhythmical modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicModality RhythmicModality { get; set; }

        /// <summary> Gets or sets previous rhythmical shape. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public RhythmicShape PreviousShape { get; set; }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "Shape",
                                new XAttribute("Code", this.GetStructuralCode),
                                new XAttribute("Schema", this.ElementSchema));
                return xe;
            }
        }
        #endregion

        #region Static factory methods
        /// <summary>
        /// Get NewRhythmicShape.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicShape GetNewRhythmicShape(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var rs = new RhythmicShape(givenSystem, structuralCode);
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Get NewRhythmicShape.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="bitArray">Bit array.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicShape GetNewRhythmicShape(GeneralSystem givenSystem, BitArray bitArray) {
            Contract.Requires(givenSystem != null);
            var rs = new RhythmicShape(givenSystem, bitArray);
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Get New RhythmicShape.
        /// </summary>
        /// <param name="shape">Rhythmic shape.</param>
        /// <returns> Returns value. </returns>
        public static RhythmicShape GetNewRhythmicShape(BinaryStructure shape) {
            Contract.Requires(shape != null);
            var rs = new RhythmicShape(shape);
            rs.DetermineBehavior();
            return rs;
        }

        /// <summary>
        /// Get NewRhythmicShape.
        /// </summary>
        /// <param name="structure">Rhythmical Structure.</param>
        /// <returns> Returns value. </returns>
        public static RhythmicShape GetNewRhythmicShape(FiguralStructure structure) {
            Contract.Requires(structure != null);

            var rs = new RhythmicShape(structure.GSystem.Order, structure);
            rs.DetermineBehavior();
            return rs;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the RhythmicShape object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return GetNewRhythmicShape(this.GSystem, this.GetStructuralCode);
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsEmptyStruct() {
            return base.IsEmptyStruct() || this.IsOff(0);
        }

        /// <summary> Evaluate properties of the structure. </summary>
        public override void DetermineBehavior() {
            this.ComputeRhythmicProperties();
        }
        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object obj) {
            return obj is RhythmicShape rs ? string.Compare(this.ElementSchema, rs.ElementSchema, StringComparison.Ordinal) : 0;
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a RhythmicShape");
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
            return this.ElementSchema != null ? this.ElementSchema.GetHashCode() : 0;
        }

        #endregion

        #region String representation
        /// <returns> Returns value. </returns>
        /// <summary> List of figure elements. </summary>
        public override string ElementString() {
            var s = new StringBuilder();
            for (byte e = 0; e < this.GSystem.Order; e++) {
                s.Append(this.IsOn(e) ? "V" : "-");
            }

            return s.ToString();
        }

        /// <summary> Short string representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public string ToShortString() {
            var s = new StringBuilder();
            s.Append(" " + this.ElementString());
            s.Append(" " + this.DistanceSchema);
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
    }
}
