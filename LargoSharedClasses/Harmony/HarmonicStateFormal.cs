// <copyright file="HarmonicStateFormal.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace LargoSharedClasses.Harmony
{
    /// <summary>  Formal harmonic state. </summary>
    /// <remarks> Harmonic state represents relations in one formal harmonic structure
    /// and enable to Compute its characteristics (continuity, impulse,..). </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicStateFormal : HarmonicTransfer {
        #region Fields
        /// <summary> Binary schema. </summary>
        [NonSerialized]
        private readonly BinarySchema binSchema;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicStateFormal class.  Serializable. </summary>
        public HarmonicStateFormal() {
        }

        /// <summary> Initializes a new instance of the HarmonicStateFormal class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="binarySchema">Harmonic structure.</param>
        public HarmonicStateFormal(HarmonicSystem harmonicSystem, BinarySchema binarySchema)
            : base(harmonicSystem) {
            Contract.Requires(harmonicSystem != null);
            Contract.Requires(binarySchema != null);
            this.binSchema = binarySchema;
            this.AddAllIntervals();
            this.SetFormalProperties();
        }

        /// <summary> Initializes a new instance of the HarmonicStateFormal class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="binarySchema">Binary schema.</param>
        /// <param name="element">Element of system.</param>
        public HarmonicStateFormal(
            HarmonicSystem harmonicSystem, BinarySchema binarySchema, byte element)
            : base(harmonicSystem) {
            Contract.Requires(harmonicSystem != null);
            Contract.Requires(binarySchema != null);
            this.binSchema = binarySchema;
            this.AddIntervalsLeadingToElement(element);
            this.SetFormalProperties();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStateFormal class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public HarmonicStateFormal(HarmonicSystem givenSystem)
            : base(givenSystem) {
            Contract.Requires(givenSystem != null);
        }
        #endregion

        #region Public Properties
        /// <summary> Gets binary schema. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public BinarySchema BinarySchema {
            get {
                Contract.Ensures(Contract.Result<BinarySchema>() != null);
                if (this.binSchema == null) {
                    throw new InvalidOperationException("Schema is null.");
                }

                return this.binSchema;
            }
        }
        #endregion

        #region Public static methods
        /// <summary> Returns root values of elements in the structure. </summary>
        /// <param name="harmonicStructure">Harmonic structure.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static Collection<float> RootValues(HarmonicStructure harmonicStructure) {
            Contract.Requires(harmonicStructure != null);
            //// if (harmonicStructure == null) { return null; }

            var hS = harmonicStructure.HarmonicSystem;
            var order = hS.Order;
            var values = new Collection<float>();
            for (byte e = 0; e < order; e++) {
                var state = new HarmonicStateFormal(hS, harmonicStructure, e);
                var formalContinuity = state.MeanValueOfProperty(GenProperty.InnerContinuity, false, false);
                values.Add(formalContinuity);
            }

            return values;
        }

        /// <summary> Returns principal values of elements in the structure. </summary>
        /// <param name="harmonicStructure">Harmonic structure.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static Collection<float> PrincipalValues(HarmonicStructure harmonicStructure) {
            Contract.Requires(harmonicStructure != null);
            //// if (harmonicStructure == null) { return null; }

            var hS = harmonicStructure.HarmonicSystem;
            var order = hS.Order;
            var values = new Collection<float>();
            for (byte e = 0; e < order; e++) {
                var state = new HarmonicStateFormal(hS, harmonicStructure, e);
                var formalContinuity = state.MeanValueOfProperty(GenProperty.InnerContinuity, true, false);
                values.Add(formalContinuity);
            }

            return values;
        }
        #endregion

        #region Public methods
        /// <summary> Fills the given array with formal intervals to given element. </summary>
        /// <param name="elementTo">Element of system.</param>
        public void AddIntervalsLeadingToElement(byte elementTo) {
            var places = this.BinarySchema.Places;
            //// MusicalInterval interval = new MusicalInterval(this.HarmonicSystem, elementFrom, elementTo);
            //// Math.Abs?!?  (otherwise 2 times more intervals), GetFormalInterval(formalLength))
            //// Do not convert to linq!!!
            foreach (byte elementFrom in places) {
                var systemLength = elementTo - elementFrom;
                if (systemLength > 0) { //// Math.Abs?!?  (otherwise 2 times more intervals)
                    var formalLength = MusicalProperties.FormalLength(this.HarmonicSystem.Order, systemLength);
                    var interval = this.HarmonicSystem.Intervals[formalLength];
                    this.Intervals.Add(interval);
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary> Makes array of intervals between tones of the cluster. </summary>
        private void AddAllIntervals() {
            var places = this.BinarySchema.Places;
            foreach (var elem in places) {
                this.AddIntervalsLeadingToElement(elem);
            }
        }
        #endregion
    }
}