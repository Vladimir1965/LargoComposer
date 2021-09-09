// <copyright file="RhythmicMetric.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Music
{
    /// <summary>  Metric structure. </summary>
    /// <remarks> Represents general metric structure of rhythm in bars. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class RhythmicMetric : FiguralSchema
    {
        #region Constructors
        /// <summary> Initializes a new instance of the RhythmicMetric class.  Serializable. </summary>
        public RhythmicMetric() {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicMetric class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public RhythmicMetric(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicMetric class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public RhythmicMetric(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, number) {
            Contract.Requires(givenSystem != null);
        }

        /// <summary> Initializes a new instance of the RhythmicMetric class. </summary>
        /// <param name="structure">Figural structure.</param>
        public RhythmicMetric(FiguralStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
        }

        #endregion

        #region Static factory methods
        /// <summary>
        /// Get New Rhythmical Metric.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicMetric GetNewRhythmicMetric(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var ms = new RhythmicMetric(givenSystem, structuralCode);
            ms.DetermineBehavior();
            return ms;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the RhythmicMetric object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return new RhythmicMetric(this.GSystem, this.GetStructuralCode);
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsEmptyStruct() {
            if (this.IsOff(0)) {
                return true;
            }

            if (this.Level == 1 && this.IsPauseStart(0)) {
                return true;
            }

            if (this.Level != 0 && this.GSystem.Order % this.Level != 0) {
                return true;
            }

            return base.IsEmptyStruct();
        }

        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsValidStruct() {
            //// return base.IsValidStruct();
            var ok = true;
            byte dist = 0, lastDist = 0;
            for (byte e = 0; e < this.Level; e++) {
                if (e < this.DiffList.Count) {  //// Distances
                    dist = (byte)this.DiffList[e];
                }

                if (lastDist != 0 && dist != lastDist) {
                    ok = false;
                    break;
                }

                lastDist = dist;
            }

            return ok;
        }

        /// <summary> Evaluate properties of the structure. Used in descendant objects. 
        /// Must be virtual, because of call from StructuralVariety. </summary>
        public override void DetermineBehavior() {
            this.SetElements();
        }
        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and elem.schema. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object obj) {
            if (!(obj is RhythmicMetric rs)) {
                return 0;
            }

            if (this.Level < rs.Level) {
                return -1;
            }

            return this.Level > rs.Level ? 1 : string.Compare(this.ElementSchema, rs.ElementSchema, StringComparison.Ordinal);
            //// This kills the DataGrid                 
            //// throw new ArgumentException("Object is not a RhythmicMetric");
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
            for (byte e = 0; e < this.GSystem.Order; e++) {
                if (e >= this.ElementList.Count) {
                    continue;
                }

                switch ((byte)this.ElementList[e]) {
                    case 0:
                        s.Append("-");
                        break;
                    case 1:
                        s.Append("A");
                        break;
                        //// resharper default: break;
                }
            }

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
