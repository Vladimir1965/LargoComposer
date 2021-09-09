// <copyright file="HarmonicRelation.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Harmony
{
    /// <summary>  Harmonic relation. </summary>
    /// <remarks> Harmonic relation represents one harmonic connection and keeps 
    /// its characteristics (continuity, impulse,..). It is used for optimization 
    /// of harmonic stream in given time point. </remarks>
    [Serializable] 
    [XmlRoot]
    public sealed class HarmonicRelation : HarmonicTransfer {
        #region Fields
        /// <summary>
        /// Structure A.
        /// </summary>
        [NonSerialized]
        private BinarySchema structA;
        
        /// <summary>
        /// Structure B.
        /// </summary>
        [NonSerialized]
        private BinarySchema structB;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicRelation class.  Serializable. </summary>
        public HarmonicRelation() { 
        }

        /// <summary> Initializes a new instance of the HarmonicRelation class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="structureA">First harmonic structure.</param>
        /// <param name="structureB">Second harmonic structure.</param>
        public HarmonicRelation(
                        HarmonicSystem harmonicSystem,
                        BinarySchema structureA,
                        BinarySchema structureB)
            : base(harmonicSystem) {
                Contract.Requires(harmonicSystem != null);
            Contract.Requires(structureA != null);
            Contract.Requires(structureB != null);
            this.StructA = structureA;
            this.StructB = structureB;
            this.AddAllIntervals();
            this.SetFormalProperties();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicRelation class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public HarmonicRelation(HarmonicSystem givenSystem)
            : base(givenSystem) {
            Contract.Requires(givenSystem != null);
        }
         
        #endregion

        #region Properties
        /// <summary>Gets or sets the first harmonic structure. </summary>
        /// <value> Property description. </value>
        public BinarySchema StructA {
            get {
                Contract.Ensures(Contract.Result<BinarySchema>() != null);
                if (this.structA == null) {
                    throw new InvalidOperationException("Structure is null.");
                }

                return this.structA;
            }

            set => this.structA = value;
        }

        /// <summary> Gets or sets the second harmonic structure. </summary>
        /// <value> Property description. </value>
        public BinarySchema StructB {
            get {
                Contract.Ensures(Contract.Result<BinarySchema>() != null);
                if (this.structB == null) {
                    throw new InvalidOperationException("Structure is null.");
                }

                return this.structB;
            }

            set => this.structB = value;
        }
        #endregion

        #region Static methods
        /// <summary> Compute total value of given characteristic. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="structureA">First harmonic structure.</param>
        /// <param name="structureB">Second harmonic structure.</param>
        /// <param name="positive">Positive values.</param>
        /// <returns> Returns value. </returns>
        public static float RelationalCharacteristic(
                        GenProperty property,
                        BinarySchema structureA,
                        BinarySchema structureB,
                        bool positive) {
            Contract.Requires(structureA != null);
            Contract.Requires(structureB != null);

            var harmonicSystem = (HarmonicSystem)structureA.GSystem;
            var harRelation = new HarmonicRelation(harmonicSystem, structureA, structureB);
            var value = harRelation.MeanValueOfProperty(property, positive, false);
            return value;
        }

        #endregion 

        #region Public methods
        /// <summary> Fills the given array with formal intervals to given element. </summary>
        /// <param name="element">Element of system.</param>
        public void AddIntervalsLeadingToElement(byte element)
        {
            var places = this.StructA.Places;
            foreach (var interval in
                places.Select(e => new MusicalInterval(this.HarmonicSystem, e, element)))
            {
                this.Intervals.Add(interval);
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0}->{1};", this.StructA, this.StructB);
            //// s.Append(this.continuity); s.Append(this.impulse);
            return s.ToString();
        }
        #endregion

        #region Private methods
        /// <summary> Makes array of intervals between tones of the cluster. </summary>
        private void AddAllIntervals() {
            var places = this.StructB.Places;
            foreach (var elem in places) {
                this.AddIntervalsLeadingToElement(elem);
            }
        }
        #endregion
    }
}