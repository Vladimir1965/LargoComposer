// <copyright file="FiguralSchema.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Figural Schema.
    /// </summary>
    public class FiguralSchema : FiguralStructure
    {
        #region Constructors
        /// <summary> Initializes a new instance of the FiguralSchema class. </summary>
        public FiguralSchema() {
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the FiguralSchema class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public FiguralSchema(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the FiguralSchema class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of instance.</param>
        public FiguralSchema(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, number) {
            Contract.Requires(givenSystem != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary> Initializes a new instance of the FiguralSchema class. </summary>
        /// <param name="structure">Figural structure.</param>
        public FiguralSchema(FiguralStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }
        #endregion

        #region Properties - Behavior
        /// <summary>
        /// Gets or sets the harmonic behavior.
        /// </summary>
        /// <value>
        /// The harmonic behavior.
        /// </value>
        public FormalBehavior FormalBehavior { get; set; }

        /// <summary>
        /// Gets or sets the harmonic behavior.
        /// </summary>
        /// <value>
        /// The harmonic behavior.
        /// </value>
        public RhythmicBehavior RhythmicBehavior { get; set; }
        #endregion

        #region Properties
        /// <summary> Gets order of system. </summary>
        /// <value> Property description. </value>
        public byte Order => this.GSystem.Order;

        /// <summary> Gets schema of elements. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ElementSchema {
            get {
                var s = this.ElementString();
                return s?.Trim() ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [zero start].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [zero start]; otherwise, <c>false</c>.
        /// </value>
        public bool ZeroStart {
            get {
                var s = this.ElementSchema;
                return s.StartsWith("T", StringComparison.CurrentCulture) || s.StartsWith("X", StringComparison.CurrentCulture);
            }
        }

        #endregion

        #region Compute properties

        /// <summary> Evaluate and set Rhythmic properties . </summary>
        public void ComputeRhythmicProperties() {
            this.FormalBehavior.Variance = this.ComputeVariance();
            this.FormalBehavior.Balance = this.ComputeBalance();
            this.RhythmicBehavior.Filling = this.ComputeFilling();
            this.RhythmicBehavior.Beat = this.ComputeBeat();
            this.RhythmicBehavior.Mobility = this.ComputeMobility();
            this.RhythmicBehavior.Complexity = this.ComputeComplexity(); //// 2020/11
            //// this.ComputeEntropy();
        }

        /// <summary>
        /// Writes the behavior to properties.
        /// </summary>
        public override void WriteBehaviorToProperties() {
            if (this.FormalBehavior != null) {
                this.Properties[GenProperty.FormalBalance] = this.FormalBehavior.Balance;
                this.Properties[GenProperty.FormalVariance] = this.FormalBehavior.Variance;
                this.Properties[GenProperty.FormalEntropy] = this.FormalBehavior.Entropy;
            }

            if (this.RhythmicBehavior != null) {
                this.Properties[GenProperty.FormalFilling] = this.RhythmicBehavior.Filling;
                this.Properties[GenProperty.FormalBeat] = this.RhythmicBehavior.Beat;
                this.Properties[GenProperty.FormalMobility] = this.RhythmicBehavior.Mobility;
                this.Properties[GenProperty.FormalComplexity] = this.RhythmicBehavior.Complexity;
            }
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the FiguralSchema object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return new FiguralStructure(this.GSystem, this.GetStructuralCode);
        }

        /// <summary>
        /// Returns inverted structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        public FiguralSchema InvertedStructure() {
            var rs = new FiguralSchema { GSystem = this.GSystem };
            for (byte i = 0; i < this.ElementList.Count; i++) {
                var value = (byte)this.ElementList[this.ElementList.Count - i - 1];
                rs.ElementList.Add(value);
            }

            rs.CompleteFromElements();
            rs.DetermineBehavior();
            return rs;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder(base.ToString());
            s.Append(" ");
            //// s.Append(this.DistanceString());
            return s.ToString();
        }
        #endregion

        #region Evaluation of properties
        /// <summary> Mean distance of nonzero bits. </summary>
        /// <returns> Returns value.</returns>
        protected float MeanDistance() {
            if (this.Level < 1) {
                return 1.0F;
            } // (float)Math.Pow(value, 1.0/Level);
            var value = this.GSystem.Order / (float)this.Level;
            return value;
        }

        /// <summary>
        /// Determine and sets the mobility property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeMobility() {
            this.DetermineLevel(); 
            float mobility = this.Level;
            //// float mobility = this.GSystem.Order > 0 ? (this.Level / (float)this.GSystem.Order) * 100f : 0;
            //// float mobility = this.GSystem.Order > 0 ? (this.ToneLevel / (float)this.GSystem.Order) * 100f : 0;
            //// var mobility = (this.ToneLevel / (float)this.Level) * 100f;
            return mobility;
        }

        /// <summary>
        /// Determine and sets the variance property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeVariance() { // variational quotient
            var bst = this.BinaryStructure(false); //// 2015/01
            var bs = new BinarySchema(bst);
            //// bs.ComputeRhythmicProperties();
            var variance = bs.ComputeVariance();  //// bs.Variance;
            return variance;
        }

        /// <summary>
        /// Computes the complexity.
        /// </summary>
        /// <returns> Returns value. </returns>
        protected float ComputeComplexity() { // variational quotient
            var bst = this.BinaryStructure(false); //// 2015/01
            var bs = new BinarySchema(bst);
            //// bs.ComputeRhythmicProperties();
            var complexity = bs.ComputeComplexity();  //// bs.Variance;
            return complexity;
        }

        /// <summary>
        /// Determine and sets the mobility property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeFilling() {
            // float filling = ((float)Level/(float)GSystem.Order)*100f;
            var order = this.GSystem.Order;
            byte sum = 0;
            var hasFilling = false;
            for (byte e = 0; e < order; e++) {
                if (this.IsOn(e)) {
                    if (this.IsPauseStart(e)) {
                        hasFilling = false;
                    }
                    else {
                        hasFilling = true;
                        sum++;
                    }
                }
                else {
                    if (hasFilling) {
                        sum++;
                    }
                }
            }

            var filling = 100.0f * sum / order;
            return filling;
        }

        /// <summary>
        /// Determine and sets the side property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeBalance() {
            var balance = 0f;
            if (this.Level > 0) {
                var median = this.GSystem.Median;
                if (median > 0) {
                    float left = this.IsOnInRange(0, (byte)(median - 1));
                    float right = this.IsOnInRange(median, (byte)(this.GSystem.Order - 1));
                    balance = (DefaultValue.HalfUnit + ((right - left) / this.GSystem.Order)) * 100.0f;
                }
            }

            return balance;
        }

        /// <summary>
        /// Determine and sets the entropy property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeEntropy() {
            var entropy = 0f;
            if (this.Level > 1) {
                const float value = 0.0f;
                //// for (byte lev = 0; lev < Level; lev++) {}

                var logLevel = (float)Math.Log(this.Level);
                if (logLevel >= DefaultValue.AfterZero && logLevel <= DefaultValue.LargeNumber) {
                    entropy = -value / logLevel * 100f;
                }
            }

            return entropy;
        }

        /// <summary>
        /// Determine and sets the beat property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeBeat() {
            var beat = 0F;
            if (this.Level > 0) {
                float v = 0, tv = 0;
                var order = this.GSystem.Order;
                for (byte m = 2; m < order; m++) {
                    if (order % m != 0) {
                        continue;
                    }

                    for (byte e = 0; e < order; e++) {
                        if (e % m != 0) {
                            continue;
                        }

                        float dv = m;
                        if (this.IsOn(e) && !this.IsPauseStart(e)) {
                            v += dv;
                        }

                        tv += dv;
                    }
                }

                if (tv >= DefaultValue.AfterZero && tv <= DefaultValue.LargeNumber) {
                    beat = v / tv * 100.0f;
                }
            }

            return beat;
        }
        #endregion
    }
}
