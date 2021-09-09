// <copyright file="MusicalInterval.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Harmony;
using LargoSharedClasses.Interfaces;

namespace LargoSharedClasses.Music
{
    /// <summary> Harmonic interval. </summary>
    /// <remarks> Musical Interval represents one interval i.e. acoustic relation of two tones. 
    /// Some characteristics (continuity, impulse, potential influence, Similarity,..) 
    /// are assigned to interval. </remarks>
    [Serializable]
    [XmlRoot]
    public class MusicalInterval : IComparable, IHarmonic
    {
        #region Constants
        /// <summary> Natural logarithm of 2. </summary>
        protected const float Log2 = 0.693147181f;

        /// <summary> Inverted Logarithm(2). </summary>
        protected const float InvLog2 = 1.442695041f;

        /// <summary> Tolerance of continuity. </summary>
        protected const float ContinuityTolerance = 1.016F;
        #endregion

        #region Fields
        /// <summary>
        /// The formal behavior
        /// </summary>
        private readonly BindingBehavior formalBehavior;

        /// <summary>
        /// The real behavior
        /// </summary>
        private readonly BindingBehavior realBehavior;

        /// <summary> Formal properties. </summary>
        private float? frmPotentialInfluence, frmSimilarity;

        /// <summary> Interval ratio. </summary>
        private float? ratio;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalInterval class.  Serializable.
        /// </summary>
        public MusicalInterval() {
            this.formalBehavior = new BindingBehavior {
                Continuity = null,
                Impulse = null
            };
            this.frmPotentialInfluence = null;
            this.frmSimilarity = null;
            this.realBehavior = new BindingBehavior {
                Continuity = null,
                Impulse = null
            };
        }

        /// <summary> Initializes a new instance of the MusicalInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>        
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        public MusicalInterval(HarmonicSystem harmonicSystem, byte elementFrom, byte elementTo)
            : this() {
            Contract.Requires(harmonicSystem != null);
            //// if (harmonicSystem == null) {  return;  }

            this.HarmonicSystem = harmonicSystem;
            this.HarmonicOrder = harmonicSystem.Order;
            this.SystemLength = elementTo - elementFrom;
            this.ratio = null;
            this.Weight = 1.0f; // SetProperties();
            this.FormalLength = MusicalProperties.FormalLength(this.HarmonicOrder, this.SystemLength);
        }

        /// <summary> Initializes a new instance of the MusicalInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="givenPitch1">First musical pitch.</param>
        /// <param name="givenPitch2">Second musical pitch.</param>
        public MusicalInterval(HarmonicSystem harmonicSystem, MusicalPitch givenPitch1, MusicalPitch givenPitch2)
            : this() {
            Contract.Requires(harmonicSystem != null);
            //// if (harmonicSystem == null) {  return;  }

            this.HarmonicSystem = harmonicSystem;
            this.HarmonicOrder = harmonicSystem.Order;
            this.ratio = null;
            this.SystemLength = givenPitch2?.IntervalFrom(givenPitch1) ?? 0;

            this.Weight = 1.0f;
            if (givenPitch1 != null) {
                this.SystemAltitude = givenPitch1.SystemAltitude;
            }

            this.FormalLength = MusicalProperties.FormalLength(this.HarmonicOrder, this.SystemLength);
            //// SetProperties();
        }

        /// <summary> Initializes a new instance of the MusicalInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="tone1">First melodic tone.</param>
        /// <param name="tone2">Second melodic tone.</param>
        public MusicalInterval(HarmonicSystem harmonicSystem, MusicalTone tone1, MusicalTone tone2)
            : this() {
            Contract.Requires(harmonicSystem != null);
            Contract.Requires(tone1 != null);
            Contract.Requires(tone2 != null);

            //// if (harmonicSystem == null) {  return;  }

            //// if (tone1 == null || tone2 == null) {
            //// throw new ArgumentException("Tone of interval must not be null."); }

            this.SystemLength = 0;
            this.HarmonicSystem = harmonicSystem;
            this.HarmonicOrder = harmonicSystem.Order;
            this.ratio = null;
            this.Weight = tone1.Weight + tone2.Weight; //// 0.5f *
            //// this.Weight = 1.0f; (when lower time of composing needed)
            //// Time optimized
            //// MusicalPitch pitch1 = tone1.Pitch;
            //// MusicalPitch pitch2 = tone2.Pitch;

            if (tone1.Pitch != null) {
                //// this.SystemAltitude = tone1.Pitch.SystemAltitude;

                if (tone2.Pitch != null) {
                    //// Time optimized (== pitch2.IntervalFrom(pitch1);)
                    this.SystemLength = tone2.Pitch.SystemAltitude - tone1.Pitch.SystemAltitude;
                    //// this.ratio = this.HarmonicSystem.RatioForInterval(this.SysLength);
                }
            }

            this.FormalLength = MusicalProperties.FormalLength(this.HarmonicOrder, this.SystemLength);

            //// Time optimization
            var formalInterval = this.HarmonicSystem.Intervals[this.FormalLength];  ////GetFormalInterval(this.FormalLength);
            if (formalInterval == null) {
                return;
            }

            this.formalBehavior.Continuity = formalInterval.FormalContinuity;
            this.formalBehavior.Impulse = formalInterval.FormalImpulse;
            this.frmPotentialInfluence = formalInterval.FormalPotentialInfluence;
            this.frmSimilarity = formalInterval.FormalSimilarity;
            //// SetProperties();
        }
        #endregion

        #region Basic properties

        /// <summary> Gets inner balance. </summary>
        /// <value> Property description. </value>
        public static float FormalBalance => 1f;

        /// <summary> Gets or sets ratio of frequencies. </summary>
        /// <value> Property description. </value>
        public HarmonicSystem HarmonicSystem { get; set; }

        /// <summary> Gets or sets the string of musical symbols. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema { get; set; }

        /// <summary> Gets or sets ratio of frequencies. </summary>
        /// <value> Property description. </value>
        public byte HarmonicOrder { get; set; }

        /// <summary> Gets or sets altitude from zero level. </summary>
        /// <value> Property description. </value>
        public int SystemAltitude { get; set; }

        /// <summary> Gets or sets ratio of frequencies. </summary>
        /// <value> Property description. </value>
        public int SystemLength { get; set; }

        /// <summary> Gets or sets ratio of frequencies. </summary>
        /// <value> Property description. </value>
        public byte FormalLength { get; set; }

        /// <summary> Gets or sets ratio of frequencies. </summary>
        /// <value> Property description. </value>
        public float Ratio {
            get {
                if (this.ratio == null) {
                    this.ratio = this.HarmonicSystem.RatioForInterval(this.SystemLength);
                }

                return (float)this.ratio;
            }

            set => this.ratio = value;
        }

        /// <summary> Gets or sets tone weight (lower tones are heavier). </summary>
        /// <value> Property description. </value>
        public float Weight { get; set; }

        /// <summary> Gets tone weight (lower tones are heavier). </summary>
        /// <value> Property description. </value>
        public float MeanWeight => this.Weight / 2;

        #endregion

        #region Formal properties

        /// <summary>
        /// Gets or sets the formal energy.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicBehavior FormalEnergy { get; set; }

        /// <summary> Gets value of formal impulse. </summary>
        /// <value> Property description. </value>
        public float FormalImpulse {
            get {
                if (this.formalBehavior.Impulse == null) {
                    this.formalBehavior.Impulse = this.FImpulse;
                }

                return (float)this.formalBehavior.Impulse;
            }
        }

        /// <summary>  Gets value of formal continuity. </summary>
        /// <value> Property description. </value>
        public float FormalContinuity {
            get {
                if (this.formalBehavior.Continuity == null) {
                    this.formalBehavior.Continuity = this.FContinuity;
                }

                return (float)this.formalBehavior.Continuity;
            }
        }

        /// <summary> Gets potential influence of the given interval. </summary>
        /// <value> Property description. </value>
        public float FormalPotentialInfluence {
            get {
                if (this.frmPotentialInfluence == null) {
                    this.frmPotentialInfluence = this.FPotentialInfluence;
                }

                return (float)this.frmPotentialInfluence;
            }
        }

        /// <summary> Gets similarity of the given interval. </summary>
        /// <value> Property description. </value>
        public float FormalSimilarity {
            get {
                if (this.frmSimilarity == null) {
                    this.frmSimilarity = FSimilarity(this.Ratio);
                }

                return (float)this.frmSimilarity;
            }
        }

        /// <summary> Gets Real impulse. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public float RealImpulse {
            get {
                if (this.realBehavior.Impulse == null) {
                    //// 2014/1 was this.FormalImpulse * this.Weight
                    this.realBehavior.Impulse = this.FormalImpulse * (1 + (this.Weight / 5));
                }

                return (float)this.realBehavior.Impulse;
            }
        }

        /// <summary> Gets Real continuity. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public float RealContinuity {
            get {
                if (this.realBehavior.Continuity == null) {
                    //// 2014/1 was this.FormalImpulse * this.Weight
                    this.realBehavior.Continuity = this.FormalContinuity * (1 + (this.Weight / 5));
                }

                return (float)this.realBehavior.Continuity;
            }
        }
        #endregion

        #region Formal properties

        /// <summary> Gets value of formal continuity. </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public float FContinuity {
            get {
                var value = ComputeFContinuity(this.Ratio);
                return value;
            }
        }

        /// <summary> Gets formal impulse. </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public float FImpulse {
            get {
                var value = ComputeFImpulse(this.Ratio);
                return value;
            }
        }

        /// <summary> Gets potential influence of the given interval. </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public float FPotentialInfluence {
            get {
                var vi = this.FImpulse;
                var vc = this.FContinuity;
                var value = Math.Abs(vc) - (DefaultValue.HalfUnit * vi);
                return value;
            }
        }

        #endregion

        #region Static operators
        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(MusicalInterval interval1, MusicalInterval interval2) { return object.Equals(interval1, interval2);  }
        //// public static bool operator !=(MusicalInterval interval1, MusicalInterval interval2) { return !object.Equals(interval1, interval2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="interval1">The interval1.</param>
        /// <param name="interval2">The interval2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(MusicalInterval interval1, MusicalInterval interval2) {
            if (interval1 != null && interval2 != null) {
                return interval1.SystemAltitude < interval2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="interval1">The interval1.</param>
        /// <param name="interval2">The interval2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >(MusicalInterval interval1, MusicalInterval interval2) {
            if (interval1 != null && interval2 != null) {
                return interval1.SystemAltitude > interval2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="interval1">The interval1.</param>
        /// <param name="interval2">The interval2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <=(MusicalInterval interval1, MusicalInterval interval2) {
            if (interval1 != null && interval2 != null) {
                return interval1.SystemAltitude <= interval2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="interval1">The interval1.</param>
        /// <param name="interval2">The interval2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >=(MusicalInterval interval1, MusicalInterval interval2) {
            if (interval1 != null && interval2 != null) {
                return interval1.SystemAltitude >= interval2.SystemAltitude;
            }

            return false;
        }
        #endregion

        #region Static methods - abstract
        /// <summary> Logarithmic value of the formal interval ratio. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float FormalLog(float ratio) {
            var slog = (float)(Math.Log(ratio) * InvLog2);
            var flog = slog - (float)Math.Floor(slog);
            return flog;
        }

        /// <summary> Value of the formal interval ratio, e.g. 1.5 for the quint. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value.</returns>
        public static float FormalRatio(float ratio) {
            var flog = FormalLog(ratio);
            var fr = (float)Math.Pow(2, flog);
            return fr;
        }

        /// <summary> Logarithmic value of the smaller formal interval, e.g. quart not quint. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float SharpLog(float ratio) {
            var flog = FormalLog(ratio);
            var sharpLog = Math.Min(flog, 1 - flog);
            return sharpLog;
        }

        /// <summary> Value of the smaller formal interval, e.g. quart not quint. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float SharpRatio(float ratio) {
            var sharpLog = SharpLog(ratio);
            var sharpRatio = (float)Math.Pow(2, sharpLog);
            return sharpRatio;
        }

        /// <summary> Returns Similarity of the given interval. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float FSimilarity(float ratio) {
            const float tolerance = 0.05f;
            var fr = FormalRatio(ratio);
            float value = Math.Abs(fr - 1) < tolerance ? +1 : 0;

            return value;
        }

        /// <summary> Returns Similarity of the given interval. </summary>
        /// <param name="formalDistance">Formal distance.</param>
        /// <returns> Returns value. </returns>
        public static float GuessSonanceValue(int formalDistance) {
            const float influenceOfFifths = 0.1f;
            const float influenceOfThirds = 0.1f;
            const float influenceOfSeconds = -0.3f;
            const float influenceOfHalftones = -0.5f;
            //// The tones consonant with other tones have higher values
            float value = 0;
            formalDistance = formalDistance > short.MinValue ? Math.Abs(formalDistance) : short.MinValue;

            if (formalDistance == (byte)IntervalType.Fourth || formalDistance == (byte)IntervalType.Fifth) {
                value += influenceOfFifths;
            }

            if (formalDistance == (byte)IntervalType.MinorThird || formalDistance == (byte)IntervalType.MajorThird) {
                value += influenceOfThirds;
            }

            if (formalDistance == (byte)IntervalType.Second) {
                value += influenceOfSeconds;
            }

            if (formalDistance == (byte)IntervalType.Halftone) {
                value += influenceOfHalftones;
            }

            return value;
        }
        #endregion

        #region Static methods - formal properties
        /// <summary> Formal impulse. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float ComputeFImpulse(float ratio) {
            const float tolerance = 0.01f;
            const float fi1 = 12.0f; //// (float)(22.17f / Math.Exp(24 * SharpLog((float)(1.05946))));
            const float definedBase = 22.17f;
            const float definedMultiple = 24.0f;
            //// 2006/06 - simplified, float mRatio = medianRatio(ratio);
            var sharpLog = SharpLog(ratio);
            //// float r = (float)(24.0*Math.Log(mRatio)/this.Log2);
            var r = definedMultiple * sharpLog;
            var expr = Math.Exp(r);
            var result = !MathSupport.EqualNumbers((float)expr, 0.00f, tolerance) ? (float)(definedBase * r * r / expr) : 0f;

            var formalValue = (float)Math.Round((result / fi1) * 100.0f, 3);
            return formalValue < 100f ? formalValue : 100f;
        }

        /// <summary> Value of formal continuity. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float ComputeFContinuity(float ratio) {
            const float ratioC1 = 1.500000F;
            const float inverseC1 = 1.333333F;
            const float ratioC2 = 1.250000F;
            const float inverseC2 = 1.600000F;
            const float ratioC3 = 1.750000F;
            const float inverseC3 = 1.142857F;
            var fr = FormalRatio(ratio);
            var formalValue = MathSupport.EqualNumbersRational(fr, ratioC1, ContinuityTolerance) ? -HarmonicSystem.C1 : 0.0F;

            if (MathSupport.EqualNumbersRational(fr, inverseC1, ContinuityTolerance)) { // 4.0F/3
                formalValue = HarmonicSystem.C1;
            }
            else {
                if (MathSupport.EqualNumbersRational(fr, ratioC2, ContinuityTolerance)) { // 5.0F/4
                    formalValue = -HarmonicSystem.C2;
                }
                else {
                    if (MathSupport.EqualNumbersRational(fr, inverseC2, ContinuityTolerance)) { // 8.0F/5
                        formalValue = HarmonicSystem.C2;
                    }
                    else {
                        if (MathSupport.EqualNumbersRational(fr, ratioC3, ContinuityTolerance)) { // 7.0F/4
                            formalValue = -HarmonicSystem.C3;
                        }
                        else {
                            if (MathSupport.EqualNumbersRational(fr, inverseC3, ContinuityTolerance)) { // 8.0F/7
                                formalValue = HarmonicSystem.C3;
                            }
                        }
                    }
                }
            }

            return formalValue / HarmonicSystem.C1 * 100.0F;
        }

        /// <summary> Returns potential influence of the given interval. </summary>
        /// <param name="ratio">Interval ratio.</param>
        /// <returns> Returns value. </returns>
        public static float ComputeFPotentialInfluence(float ratio) {
            var vi = ComputeFImpulse(ratio);      //// see also FImpulse
            var vc = ComputeFContinuity(ratio);   //// see also FContinuity
            var value = Math.Abs(vc) - (DefaultValue.HalfUnit * vi);
            return value;
        }
        #endregion

        #region Comparison
        /// <summary> Support of sorting according to interval ratio. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public int CompareTo(object obj) {
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a MusicalInterval");
            return obj is MusicalInterval mi ? this.Ratio.CompareTo(mi.Ratio) : 0;
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
            return this.Ratio.GetHashCode();
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Value Of Property.
        /// </summary>
        /// <param name="property">Musical property.</param>
        /// <returns> Returns value. </returns>
        public float ValueOfProperty(GenProperty property) {
            float value;
            switch (property) {
                case GenProperty.InnerContinuity:
                    value = this.FormalContinuity;
                    break;
                case GenProperty.InnerImpulse:
                    value = this.FormalImpulse;
                    break;
                case GenProperty.FormalPotentialInfluence:
                    value = this.FormalPotentialInfluence;
                    break;
                case GenProperty.RealContinuity:
                    value = this.RealContinuity;
                    break;
                case GenProperty.RealImpulse:
                    value = this.RealImpulse;
                    break;
                default:
                    value = 0;
                    break;
            }

            return value;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(
                $"{string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6}", this.Ratio)} ({string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6}", this.Weight)})\t");
            //// s.Append(this.StringOfProperties());
            return s.ToString();
        }
        #endregion
    }
}
