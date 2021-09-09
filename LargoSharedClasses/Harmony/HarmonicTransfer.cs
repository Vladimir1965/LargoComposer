// <copyright file="HarmonicTransfer.cs" company="Traced-Ideas, Czech republic">
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
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Harmony
{
    /// <summary>  Harmonic transfer. </summary>
    /// <remarks>
    /// Is prototype for computation of characteristics needed
    /// by harmonic state and harmonic relation classes. </remarks>
    [Serializable] 
    [XmlRoot]
    public class HarmonicTransfer { 
        #region Fields
        /// <summary>
        /// Harmonic system.
        /// </summary>
        private readonly HarmonicSystem harSystem;

        /// <summary> List of musical intervals. </summary>
        private readonly Collection<MusicalInterval> intervals;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicTransfer class.  Serializable. </summary>
        public HarmonicTransfer() { 
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicTransfer class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public HarmonicTransfer(HarmonicSystem givenSystem) {
            Contract.Requires(givenSystem != null);
            this.harSystem = givenSystem;
            this.intervals = new Collection<MusicalInterval>();
        }
        #endregion

        #region Properties
        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float FormalContinuity { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float FormalImpulse { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float FormalPotential { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float FormalConsonance { get; private set; }

        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public HarmonicSystem HarmonicSystem {
            get {
                Contract.Ensures(Contract.Result<HarmonicSystem>() != null);
                if (this.harSystem == null) {
                    throw new InvalidOperationException("Harmonic system is null.");
                }

                return this.harSystem;
            }
        }

        /// <summary> Gets list of musical intervals. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<MusicalInterval> Intervals {
            get {
                Contract.Ensures(Contract.Result<Collection<MusicalInterval>>() != null);
                if (this.intervals == null) {
                    throw new InvalidOperationException("List of intervals is null.");
                }

                return this.intervals; 
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Harmonic transfer (order={0})", this.HarmonicSystem.Order);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Computes mean value of given property in bindings.
        /// </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="positive">Positive values.</param>
        /// <param name="eliminateZeros">If set to <c>true</c> [eliminate zeroes].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public float MeanValueOfProperty(GenProperty property, bool positive, bool eliminateZeros) {
            var v = 0f;
            if (this.Intervals.Count == 0) {
                return v;
            }

            var cnt = 0;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var value in
                from interval in this.intervals 
                where interval != null 
                select interval.ValueOfProperty(property))
            {
                if (eliminateZeros && (int)value == 0) {
                    continue;
                }
                
                v = positive && value < 0 ? v - value : v + value;
                cnt++;
            }

            return cnt > 0 ? v / cnt : 0f;
        }

        /// <summary> Sets harmonic properties of the cluster. </summary>
        public void SetFormalProperties() {
            // float Level = (float)(Math.Sqrt(1+8*this.intervals.Count)+1)/2;
            if (this.Intervals.Count == 0) {
                return;
            }

            this.FormalContinuity = this.MeanValueOfProperty(GenProperty.InnerContinuity, false, true); //// 15.7.2013, was true
            //// formalContinuity = formalContinuity*this.intervals.Count/Level;
            this.FormalImpulse = this.MeanValueOfProperty(GenProperty.InnerImpulse, true, true);
            //// formalImpulse = formalImpulse*this.intervals.Count/Level;
            this.FormalPotential = this.MeanValueOfProperty(GenProperty.FormalPotentialInfluence, true, true);
            this.FormalConsonance = HarmonicSystem.Consonance(this.FormalContinuity, this.FormalImpulse);
            //// const float formalGenus = 0;
        }
        #endregion
    }
}
