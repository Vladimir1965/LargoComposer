// <copyright file="BinarySchema.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Xml.Serialization;
    using Abstract;
    using LargoSharedClasses.Rhythm;

    /// <summary>
    /// Binary Schema.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class BinarySchema : BinaryStructure
    {
        #region Fields
        /// <summary> List of places. </summary>
        private Collection<byte> places;

        /// <summary> List of distances. </summary>
        private Collection<byte> distances;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the BinarySchema class. </summary>
        public BinarySchema() //// resharper - redundant call : base() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the BinarySchema class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public BinarySchema(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the BinarySchema class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenBitArray">Bit array.</param>
        public BinarySchema(GeneralSystem givenSystem, BitArray givenBitArray)
            : base(givenSystem, givenBitArray) {
            Contract.Requires(givenSystem != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the BinarySchema class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public BinarySchema(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary> Initializes a new instance of the BinarySchema class. </summary>
        /// <param name="structure">Binary structure.</param>
        public BinarySchema(BinaryStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }
        #endregion

        #region Properties
        /// <summary> Gets order of system. </summary>
        /// <value> Property description. </value>
        public byte Order => this.GSystem.Order;

        /// <summary> Gets schema of positions. </summary>
        /// <value> Property description. </value>
        public string PosSchema => this.PlaceString();

        /// <summary> Gets binary schema of elements. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public virtual string ElementSchema {
            get {
                var s = new StringBuilder();
                for (byte e = 0; e < this.GSystem.Order; e++) {
                    s.Append(this.IsOn(e) ? '1' : '0');
                }

                //// string se =  (from e in Enumerable.Range(0, this.GSystem.Order).Cast<Byte>()
                //// select this.IsOn(e) ? "1":"0")  .Aggregate((current, next) => current + ", " + next); */
                return s.ToString().Trim();
            }
        }
        #endregion

        #region Places and distances
        /// <summary> Gets positions of nonzero bits. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<byte> Places {
            get {
                Contract.Ensures(Contract.Result<Collection<byte>>() != null);
                if (this.places == null) {
                    this.places = this.BitPlaces;
                }

                //// if (this.places == null) {
                //// throw new InvalidOperationException("List of places is null."); }

                return new Collection<byte>(this.places);
            }
        }

        /// <summary> Gets distances of nonzero bits. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<byte> Distances {
            get {
                Contract.Ensures(Contract.Result<Collection<byte>>() != null);
                if (this.distances == null) {
                    this.distances = this.BitDistances;
                }

                if (this.distances == null) {
                    throw new InvalidOperationException("List of distances is null.");
                }

                return new Collection<byte>(this.distances);
            }
        }

        /// <summary> Gets or sets Tone Level. </summary>
        /// <value> Property description. </value>
        public byte ToneLevel { get; set; }

        /// <summary> Gets measure of rhythmical motion. </summary>
        /// <value> Property description. </value>
        public float Mobility => this.Properties.ContainsKey(GenProperty.FormalMobility) ? this.Properties[GenProperty.FormalMobility] : 0f;

        /// <summary> Gets measure of rhythmical density. </summary>
        /// <value> Property description. </value>
        public float Filling => this.Properties.ContainsKey(GenProperty.FormalFilling) ? this.Properties[GenProperty.FormalFilling] : 0f;

        /// <summary> Gets measure of rhythmical regularity. </summary>
        /// <value> Property description. </value>
        public float Variance => this.Properties.ContainsKey(GenProperty.FormalVariance) ? this.Properties[GenProperty.FormalVariance] : 0f;

        /// <summary> Gets measure of rhythmical beat. </summary>
        /// <value> Property description. </value>
        public float Beat => this.Properties.ContainsKey(GenProperty.FormalBeat) ? this.Properties[GenProperty.FormalBeat] : 0f;

        /// <summary> Gets measure of rhythmical balance. </summary>
        /// <value> Property description. </value>
        public float Balance => this.Properties.ContainsKey(GenProperty.FormalBalance) ? this.Properties[GenProperty.FormalBalance] : 0f;

        /// <summary> Gets measure of rhythmical complexity. </summary>
        /// <value> Property description. </value>
        public float Complexity => this.Properties.ContainsKey(GenProperty.FormalComplexity) ? this.Properties[GenProperty.FormalComplexity] : 0f;

        /// <summary> Gets list of nonzero bits distances. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value.</returns>
        public string DistanceSchema {
            get {
                var s = new StringBuilder();
                s.Append("(");
                for (byte lev = 0; lev < this.Level; lev++) {
                    var p = (lev == this.Level - 1) ? string.Empty : ",";
                    if (lev > 0 && lev < this.Distances.Count) {
                        s.Append(this.Distances[lev] + p);
                    }
                }

                s.Append(")");
                return s.ToString();
            }
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

        #region Public methods
        /// <summary>
        /// Resets the schema.
        /// </summary>
        public void ResetSchema() {
            this.places = null;
            this.distances = null;
        }

        /// <summary> Makes a deep copy of the BinarySchema object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return new BinarySchema(this.GSystem, this.GetStructuralCode);
        }

        /// <summary> Formal position of the given nonzero bit. </summary>
        /// <param name="level">Requested formal level.</param>
        /// <returns>Returns value.</returns>
        public byte PlaceAtLevel(byte level) {
            if (level >= this.Places.Count) {
                throw new ArgumentException("Incorrect level");
            }

            return this.Places[level];
        }

        /// <summary> Real position of the given nonzero bit. </summary>
        /// <param name="level">Requested real level.</param>
        /// <returns>Returns value.</returns>
        public short RealPlaceAtLevel(short level) {
            short p = 0;
            if (this.Level == 0) {
                return p;
            }

            while (level < 0) {
                level += this.Level;
                p -= this.GSystem.Order;
            }

            while (level >= this.Level) {
                level -= this.Level;
                p += this.GSystem.Order;
            }

            var idx = level % this.Level;
            if (idx < this.Places.Count) {
                p += this.Places[idx];
            }

            return p;
        }

        /// <summary> Distance of bit pair on given Level. </summary>
        /// <param name="level">Requested level.</param>
        /// <returns>Returns value.</returns>
        public byte DistanceAtLevel(byte level) {
            if (level >= this.Distances.Count) {
                throw new ArgumentException("Incorrect level");
            }

            return (byte)(level < this.Level ? this.Distances[level] : 0);
        }

        /// <summary>
        /// Range of bit pair on given Level.
        /// </summary>
        /// <param name="givenLevel">The given level.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public BitRange RangeAtLevel(byte givenLevel) {
            if (givenLevel >= this.Places.Count) {
                throw new ArgumentException("Incorrect level");
            }

            if (givenLevel >= this.Level) {
                return null;
            }

            var order = this.GSystem.Order;
            var place = this.PlaceAtLevel(givenLevel);
            var length = this.DistanceAtLevel(givenLevel);
            if (givenLevel == (byte)(this.Level - 1)) {
                var diff = (short)(place + length - order);
                if (diff > 0 && diff < length) {
                    length = (byte)(length - diff);
                }
            }

            var range = new BitRange(order, place, length);
            return range;
        }

        /// <summary> Range of bit pair on given Level - simplified algorithm. </summary>
        /// <param name="level">Requested level.</param>
        /// <returns>Returns value.</returns>
        public BitRange RangeForLevel(byte level) {
            if (level >= this.Places.Count) {
                throw new ArgumentException("Incorrect level");
            }

            var range = new BitRange(this.GSystem.Order, this.PlaceAtLevel(level), this.DistanceAtLevel(level));
            return range;
        }

        /// <summary> Returns frequency ratio of given level. </summary>
        /// <param name="level">Requested level.</param>
        /// <value> Given level can exceed modality level. </value>
        /// <returns> Returns value. </returns>
        public float RatioForLevel(int level) {
            Contract.Requires(this.Level != 0);
            if (level % this.Level >= this.Places.Count) {
                throw new ArgumentException("Incorrect level");
            }

            var lev = (byte)(level % this.Level);
            //// if (this.Level <= 0) { return r; }

            var n = level / this.Level;
            //// if (this.Order != 0) { //// was r (nonsense)
            var r = (float)Math.Pow(2, n + ((float)this.PlaceAtLevel(lev) / this.Order));

            return r;
        }

        /// <summary> Returns index of level containing given bit. </summary>
        /// <param name="givenBit">Given element/bit.</param>
        /// <returns> Returns value. </returns>
        public byte LevelContainingBit(byte givenBit) {
            for (byte lev = 0; lev < this.Level; lev++) {
                if (this.RangeForLevel(lev).ContainsBit(givenBit)) {
                    return lev;
                }
            }

            return 0;
        }
        #endregion

        #region String representation

        /// <summary> List of nonzero bits places. </summary>
        /// <returns> Returns value.</returns>
        public string PlaceString() {
            var s = new StringBuilder();
            for (byte lev = 0; lev < this.Level; lev++) {
                if (lev >= this.Places.Count) {
                    continue;
                }

                s.Append(this.Places[lev]);
                if (lev < this.Level - 1) {
                    s.Append(",");
                }
            }

            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns object. </returns>
        public override string ToString() {
            var s = new StringBuilder(base.ToString());
            s.Append(" ");
            s.Append(this.DistanceSchema);
            return s.ToString();
        }
        #endregion

        #region Compute properties
        /// <summary> Evaluate and set Rhythmic properties. 
        /// </summary>
        public void ComputeRhythmicProperties() {
            this.FormalBehavior.Variance = this.ComputeVariance();
            this.FormalBehavior.Balance = this.ComputeBalance();
            this.RhythmicBehavior.Filling = this.ComputeFilling();
            this.RhythmicBehavior.Beat = this.ComputeBeat();
            this.RhythmicBehavior.Mobility = this.ComputeMobility();
            this.RhythmicBehavior.Complexity = this.ComputeComplexity();
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

        #region Evaluation of properties

        /// <summary>
        /// Determine and sets the variance property.
        /// </summary>
        /// <returns>Returns value.</returns>
        public float ComputeVariance() { // variational quotient
            float variance = 0;
            if (this.Level > 1) {
                var md = this.MeanDistance();
                var value = 0.0F;
                for (byte e = 0; e < this.Level; e++) {
                    value = value + (float)Math.Pow(
                            Math.Abs(this.DistanceAtLevel(e) - md), 2.0);
                }

                if (this.Level > 0 && md >= DefaultValue.AfterZero && md <= DefaultValue.LargeNumber) { //// Math.Abs(md) > 0.00001
                    variance = (float)Math.Sqrt(value / this.Level) / md * 100f; // Level-1
                }
            }

            return variance;
        }

        /// <summary>
        /// Determine and sets the complexity.
        /// </summary>
        /// <returns>Returns value.</returns>
        public float ComputeComplexity() {
            var complexity = 0f;
            if (this.Level > 1) {
                int value = 0, tv = 0;
                for (byte lev = 0; lev < this.Level; lev++) {
                    var p = this.PlaceAtLevel(lev);
                    if (p <= 0) {
                        continue;
                    }

                    var d = (byte)MathSupport.GreatestCommonDivisor(this.GSystem.Order, p);
                    tv++;
                    if (d < p) {
                        value++;
                    }

                    tv++;
                    if (d == 1) {
                        value++;
                    }
                }

                if (tv != 0) {
                    complexity = (float)value / tv * 100.0f;
                }
            }

            return complexity;
        }

        /// <summary>
        /// Determine and sets the complexity.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeComplexity2() {
            //// the algorithm is not very good ?!
            var complexity = 0f;
            if (this.Level > 1) {
                long value = this.GSystem.Order; // 1L;
                for (byte lev = 0; lev < this.Level; lev++) {
                    long p = this.DistanceAtLevel(lev);
                    if (p > 0) {
                        value = (long)MathSupport.LeastCommonMultiple(value, p);
                    }
                }

                var denominator = 2.0f * this.GSystem.Order * this.GSystem.Order;
                complexity = value / denominator * 100.0f;
                if (complexity > 100) {
                    complexity = 100.0f;
                }
            }

            return complexity;
        }

        /// <summary>
        /// Mean distance of nonzero bits.
        /// </summary>
        /// <returns>
        /// Returns object.
        /// </returns>
        protected float MeanDistance() {
            ////  Contract.Requires(this.GSystem != null);
            if (this.Level < 1) {
                return 1.0F;
            }

            var value = this.GSystem.Order / (float)this.Level;
            return value;
        }

        /// <summary>
        /// Determine and sets the mobility property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeMobility() {
            //// float mobility = this.GSystem.Order != 0 ? (Level / (float)this.GSystem.Order) * 100f : 0;
            var mobility = (this.ToneLevel / (float)this.Level) * 100f; //// this.GSystem.Order > 0 ? (this.ToneLevel / (float)this.Level) * 100f : 0;

            return mobility;
        }

        /// <summary>
        /// Determine and sets the filling property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeFilling() {
            byte sum = 0;
            var order = this.GSystem.Order;
            var isPause = this.IsOn(0);
            for (byte e = 0; e < order; e++) {
                if (this.IsOn(e)) {
                    isPause = false;
                }

                if (!isPause) {
                    sum++;
                }
            }

            var filling = order != 0 ? 100.0f * sum / order : 0;

            return filling;
        }

        /// <summary>
        /// Determine and sets the side property.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        protected float ComputeBalance() {
            var median = this.GSystem.Median;
            float left = this.IsOnInRange(0, (byte)(median - 1));
            ////  Contract.Assume(this.GSystem != null);
            float right = this.IsOnInRange(median, (byte)(this.GSystem.Order - 1));
            var balance = this.Level > 0 ? (DefaultValue.HalfUnit + ((right - left) / this.GSystem.Order)) * 100.0f : 0f;

            return balance;
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
                        if (e == 1 || e % m != 0) {
                            continue;
                        }

                        float dv = m;
                        if (e < this.BitArray.Count && this.BitArray[e]) { //// = this.IsOn(e) (time optimization)
                            v += dv;
                        }

                        tv += dv;
                    }
                }

                if (tv >= DefaultValue.AfterZero && tv <= DefaultValue.LargeNumber) { //// Math.Abs(tv) > 0.0001
                    beat = v / tv * 100.0f;
                }
            }

            return beat;
        }

        /// <summary>
        /// Determine and sets the entropy property.
        /// </summary>
        /// <returns>Returns value.</returns>
        protected float ComputeEntropy() {
            ////  Contract.Assume(this.GSystem != null);
            var entropy = 0f;
            if (this.Level > 1) {
                var value = 0.0f;
                for (byte lev = 0; lev < this.Level; lev++) {
                    var p = (float)this.DistanceAtLevel(lev) / this.GSystem.Order;

                    if (p > 0) {
                        value = (float)(value + (p * Math.Log(p)));
                    }
                }

                var logLevel = (float)Math.Log(this.Level);
                if (logLevel >= DefaultValue.AfterZero && logLevel <= DefaultValue.LargeNumber) {
                    entropy = -value / logLevel * 100f;
                }
            }

            return entropy;
        }

        #endregion
    }
}
