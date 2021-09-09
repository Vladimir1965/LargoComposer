// <copyright file="HarmonicCluster.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Harmony;
using LargoSharedClasses.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Harmonic cluster. </summary>
    /// <remarks>
    /// Harmonic cluster is a set of tones representing a cross-section of voices in given time point.
    /// It is designed for check of tone doubling and for computation of actual Ambit, density,
    /// measure of dissonance, potential,...  </remarks>
    [Serializable]
    //// [XmlInclude(typeof(MusicalTone))]
    [XmlRoot]
    public sealed class HarmonicCluster : IHarmonic {
        #region Fields
        /// <summary> List of tones. </summary>
        private readonly MusicalToneCollection toneList;

        /// <summary>
        /// Harmonic system.
        /// </summary>
        private HarmonicSystem harSystem;

        /// <summary>
        /// Harmonic structure.
        /// </summary>
        private HarmonicStructure harStruct;

        /// <summary> List of tones. </summary>
        private MusicalToneCollection validToneList; //// MusicalToneCollection

        /// <summary> String of musical symbols. </summary>
        private string toneSchema;

        /// <summary>
        /// Real harmonic state of the cluster.
        /// </summary>
        private HarmonicStateReal realHarmonicState;

        /// <summary>
        /// Formal harmonic state of the cluster.
        /// </summary>
        private HarmonicStateFormal formalHarmonicState;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicCluster class.  Serializable. </summary>
        public HarmonicCluster()
        {
            this.FormalEnergy = new HarmonicBehavior();
            this.RealEnergy = new HarmonicBehavior();
        }

        /// <summary> Initializes a new instance of the HarmonicCluster class. </summary>
        /// <param name="harmonicStructure">Harmonic structure.</param>
        /// <param name="tick">Rhythmical tick.</param>        
        /// <param name="duration">Rhythmical duration.</param>
        public HarmonicCluster(HarmonicStructure harmonicStructure, byte tick, byte duration) : this()
        {
            this.HarmonicStructure = harmonicStructure;
            this.Tick = tick;
            this.Duration = duration;
            this.toneList = new MusicalToneCollection();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>The bar number.</value>
        public int BarNumber { get; set; }

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

        /// <summary> Gets or sets harmonics structure corresponding to the cluster. </summary>
        /// <value> Property description. </value>
        public HarmonicStructure HarmonicStructure {
            get => this.harStruct;

            set {
                this.harStruct = value;
                if (value != null) {
                    this.harSystem = value.HarmonicSystem;
                }
            }
        }

        /// <summary> Gets or sets first tick of the cluster. </summary>        
        /// <value> Property description. </value>
        public byte Tick { get; set; }

        /// <summary> Gets or sets duration of the cluster. </summary>        
        /// <value> Property description. </value>
        public byte Duration { get; set; }

        /// <summary> Gets or sets current effective length of the cluster. </summary>        
        /// <value> Property description. </value>
        public byte CurrentEffectiveLength { get; set; }

        /// <summary>
        /// Gets the formal energy.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicBehavior FormalEnergy { get; }

        /// <summary>
        /// Gets the real energy.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicBehavior RealEnergy { get; }

        /// <summary> Gets or sets the property. </summary>        
        /// <value> Property description. </value>
        public float IndexOfCentre { get; set; }

        /// <summary> Gets or sets the property. </summary>        
        /// <value> Property description. </value>
        public float Weight { get; set; }

        /// <summary> Gets or sets the property. </summary>        
        /// <value> Property description. </value>
        public int Ambit { get; set; }

        /// <summary> Gets or sets the property. </summary>        
        /// <value> Property description. </value>
        public int FormalAmbit { get; set; }

        /// <summary> Gets or sets the property. </summary>        
        /// <value> Property description. </value>
        public float Density { get; set; }

        /// <summary> Gets list of all tones. </summary>
        /// <value> Property description. </value>
        public MusicalToneCollection ToneList {
            get {
                Contract.Ensures(Contract.Result<MusicalToneCollection>() != null);
                if (this.toneList == null) {
                    throw new InvalidOperationException("List of tones is null.");
                }

                return this.toneList;
            }
        }

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public MusicalToneCollection ValidToneList {
            get {
                Contract.Ensures(Contract.Result<MusicalToneCollection>() != null);
                if (this.validToneList == null) {
                    throw new InvalidOperationException("Valid tone list must not be null.");
                }

                return this.validToneList;
            }
        }

        /// <summary>
        /// Gets real harmonic state of the cluster.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicStateReal RealHarmonicState {
            get
            {
                Contract.Ensures(Contract.Result<HarmonicStateReal>() != null);
                return this.realHarmonicState ??
                       (this.realHarmonicState = new HarmonicStateReal(this.HarmonicSystem, this.ValidToneList));
            }
        }

        /// <summary>
        /// Gets formal harmonic state of the cluster.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicStateFormal FormalHarmonicState {
            get {
                Contract.Ensures(Contract.Result<HarmonicStateFormal>() != null);
                if (this.formalHarmonicState != null) {
                    return this.formalHarmonicState;
                }

                if (this.HarmonicStructure == null) {
                    throw new InvalidOperationException("Harmonic structure is null.");
                }

                this.formalHarmonicState = new HarmonicStateFormal(this.HarmonicSystem, this.HarmonicStructure);

                return this.formalHarmonicState;
            }
        }

        /// <summary> Gets cluster duration. </summary>
        /// <value> Property description. </value>
        public HarmonicClusterExtent Extent { get; private set; }

        /// <summary> Gets or sets string of musical tone symbols.</summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema {
            get => this.toneSchema ?? (this.toneSchema = this.ToneList.ToneSchema);

            set => this.toneSchema = value;
        }

        /// <summary>
        /// Gets a value indicating whether Is chord.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsChord => this.IsValid && this.ValidToneList.Count > 1 && this.harSystem != null;

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => this.validToneList != null;

        #endregion

        #region Cluster tones
        /// <summary> Adds tone to the cluster. </summary>
        /// <param name="givenTone">Melodic tone.</param>
        public void AddMelodicTone(MusicalTone givenTone) {
            this.ToneList.Add(givenTone);
        }

        /// <summary> Returns all the harmonic tones. </summary>
        /// <returns> Returns value. </returns>
        public MusicalToneCollection HarmonicTones() {
           if (this.HarmonicStructure == null) {
                return null;
           }

           var hs = this.HarmonicStructure;
           var tones = from mt in this.ValidToneList 
               where hs.IsOn(mt.Pitch.Element) 
               select mt;
           var collection = new MusicalToneCollection(tones, false);
           return collection; //// new MusicalToneCollection(tones.ToList());
        }

        /// <summary> Returns all the non-harmonic tones. </summary>
        /// <returns> Returns value.</returns>
        public MusicalToneCollection MelodicTones() {
            if (this.HarmonicStructure == null) {
                return null;
            }

            var hs = this.HarmonicStructure;
            var tones = from mt in this.ValidToneList 
                        where hs.IsOff(mt.Pitch.Element) 
                        select mt;
            var collection = new MusicalToneCollection(tones, false);
            return collection; //// new MusicalToneCollection(list.ToList());        
        }

        /// <summary> Returns number of tones corresponding to given formal element. </summary>
        /// <param name="givenPitchElement">Pitch element.</param>
        /// <returns> Returns value. </returns>
        public int NumberOfEqualTones(byte givenPitchElement) {
            var number = (from melTone in this.ToneList
                          where melTone.IsTrueTone && (melTone.Pitch.Element == givenPitchElement)
                          select 1)
                          .Count(); 

            return number;
        }

        /// <summary>
        /// Numbers the of mel tones.
        /// </summary>
        /// <returns> Returns value. </returns>
        public int NumberOfTrueTones() {
            var number = (from melTone in this.ToneList
                          where melTone.IsTrueTone 
                          select 1)
                          .Count();

            return number;
        }
        #endregion

        #region Public methods
        /// <summary> Completes initialization (obsolete). </summary>
        public void Recompute() {
            //// Properties.Clear();
            if ((this.ToneList == null) || (this.ToneList.Count <= 0))
            {
                return;
            }

            ////SortToneArray();
            this.validToneList = this.ToneList.ValidToneList;
            this.realHarmonicState = null;
            //// ?!? this.formalHarmonicState = null;
            if (this.validToneList.Count > 0) {
                //// // // list.Sort();
                this.ComputeAmbit(); //// this.validToneList
                this.ComputeIndexOfCentre(); //// this.validToneList
                this.ComputeState(); //// this.validToneList
                this.ComputeWeight(); //// this.validToneList
            }

            this.toneSchema = null;
        }

        /// <summary> Returns formal representation of the cluster. </summary>
        /// <returns> Returns value.</returns>
        public HarmonicStructure FormalStruct() {
            this.ToneList.Where(mt => mt != null && mt.IsTrueTone).Aggregate(0L, (current, mt) => current | BinaryNumber.BitAt(mt.Pitch.Element));

            var hS = HarmonicStructure.GetNewHarmonicStructure(this.HarmonicSystem, null); //// number
            return hS;
        }
        #endregion

        #region Public methods - Roots
        /// <summary>
        /// Returns root values of elements in the structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public Collection<float> RootValues() {
            var hS = this.HarmonicSystem;
            var values = new Collection<float>();
            foreach (var rcontinuity in from mt in this.ToneList
                                        where mt != null
                                        let mts = this.ToneList
                                        select new HarmonicStateReal(hS, mts, mt)
                                          into state
                                        select state.MeanValueOfProperty(GenProperty.RealContinuity, false, true)) {
                values.Add(rcontinuity);
            }

            return values;
        }

        /// <summary>
        /// Returns principal values of elements in the structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public Collection<float> PrincipalValues() {
            var hS = this.HarmonicSystem;
            var values = new Collection<float>();
            foreach (var rcontinuity in from mt in this.ToneList
                                        where mt != null
                                        let mts = this.ToneList
                                        select new HarmonicStateReal(hS, mts, mt)
                                          into state
                                        select state.MeanValueOfProperty(GenProperty.RealContinuity, true, true)) {
                values.Add(rcontinuity);
            }

            return values;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value.</returns>
        public override string ToString() {
            var s = new StringBuilder();
            //// s.AppendFormat("Harmonic Cluster");
            //// s.Append(this.StringOfProperties()+"\n");
            //// s.Append(harStruct.ToString(CultureInfo.CurrentCulture));
            foreach (var mt in this.ToneList.Where(mt => mt != null))
            {
                s.Append(mt.ToShortString() + ",");
            }
            //// s.Append(StringOfProperties());
            return s.ToString();
        }
        #endregion

        #region Computation of properties
        /// <summary> Sets properties derived from Ambit. </summary>
        public void ComputeState() { ///// MusicalToneCollection melodicTones
            //// Contract.Requires(melodicTones != null);
            //// if (melodicTones == null) { return;  }

            if (this.HarmonicStructure == null) {
                return;
            }

            //// HarmonicSystem hS = (HarmonicSystem)this.HarmonicStructure.HarmonicSystem;
            if (this.ValidToneList.Count != 0) {
                ////  HarmonicStateReal state = new HarmonicStateReal(hS, melodicTones);
                //// string key = this.ValidToneList.ToneKey;
                var state = this.RealHarmonicState;
                //// if (HarmonicStateReal.UsedStates.ContainsKey(key)) { state = HarmonicStateReal.UsedStates[key];
                //// }  else {  state = this.RealHarmonicState;  HarmonicStateReal.UsedStates[key] = state; } 
                this.RealEnergy.Continuity = state.RealContinuity;     //// GetProperty(GenProperty.RealContinuity);
                this.RealEnergy.Impulse = state.RealImpulse;           //// GetProperty(GenProperty.RealImpulse);
                this.RealEnergy.Potential = state.RealPotential;       //// GetProperty(GenProperty.RealPotential);
                this.RealEnergy.Consonance = state.RealConsonance;     //// GetProperty(GenProperty.RealConsonance);
            }
            else {
                this.RealEnergy.Consonance = 100f;
            }

            //// HarmonicStateFormal formalState = new HarmonicStateFormal(hS, HarmonicStructure);
            //// 2019/05
            if (this.HarmonicStructure.Level > 0) {
                var formalState = this.FormalHarmonicState;
                this.FormalEnergy.Continuity =
                    formalState.FormalContinuity; //// GetProperty(GenProperty.FormalContinuity);
                this.FormalEnergy.Impulse = formalState.FormalImpulse; //// GetProperty(GenProperty.FormalImpulse);
                this.FormalEnergy.Potential =
                    formalState.FormalPotential; //// GetProperty(GenProperty.FormalPotential);
                this.FormalEnergy.Consonance =
                    formalState.FormalConsonance; //// GetProperty(GenProperty.FormalConsonance);
                this.FormalEnergy.Balance = 0;
            }
        }
        #endregion

        #region Private methods
        /// <summary> Determines estimated cluster extent. </summary>
        /// <param name="givenDensity">Density of cluster.</param>
        private void DetermineExtent(float givenDensity) {
            const float wideToMiddleBreak = 6.0f;
            const float middleToTightBreak = 3.0f;
            this.Extent = givenDensity > middleToTightBreak ? givenDensity > wideToMiddleBreak ? HarmonicClusterExtent.WideExtent : HarmonicClusterExtent.MiddleExtent : HarmonicClusterExtent.TightExtent;
        }

        /// <summary> Compute pitch center of the cluster. </summary>
        private void ComputeIndexOfCentre() {
            //// Contract.Requires(melodicTones != null);
            var melodicTones = this.validToneList;
            if (melodicTones == null || melodicTones.Count == 0) {
                return;
            }

            float centralIndex;
            if (melodicTones.Count > 1) {
                float total = (from mt in melodicTones select mt.Pitch.SystemAltitude).Sum();
                //// if (melodicTones.Count != 0) {
                centralIndex = total / melodicTones.Count;
                //// }
            }
            else {
                centralIndex = melodicTones.First().Pitch.SystemAltitude; 
            }

            this.IndexOfCentre = centralIndex;
        }

        /// <summary> Sets properties derived from Ambit. </summary>
        private void ComputeWeight() {
            //// Contract.Requires(melodicTones != null);
            var melodicTones = this.validToneList;
            if (melodicTones == null || melodicTones.Count == 0) {
                return;
            }

            float weight = 0;
            if (melodicTones.Count > 1) {
                var total = (from mt in melodicTones select mt.Weight).Sum();
                if (this.ToneList.Count > 0) {
                    weight = total / this.ToneList.Count;
                }
            }
            else {
                weight = 1.0f;
            }

            this.Weight = weight;
        }

        /// <summary> Sets properties derived from Ambit. </summary>
        private void ComputeAmbit() {
            //// Contract.Requires(melodicTones != null);
            var melodicTones = this.validToneList;
            if (melodicTones == null || melodicTones.Count == 0) {
                return;
            }

            var sysLength = 0;
            if (melodicTones.Count > 1) { //// melodicTones.Count != 0
                var t0 = melodicTones.ElementAt(0);
                var t1 = melodicTones.ElementAt(melodicTones.Count - 1);
                sysLength = t0 != null && t1 != null && t0.IsTrueTone && t1.IsTrueTone ? 
                                t1.Pitch.SystemAltitude - t0.Pitch.SystemAltitude : 
                                0;
            }

            float realAmbit = sysLength;
            var formalAmbit = this.HarmonicStructure?.HarmonicSystem.FormalLength(sysLength) ?? 0;

            var density = realAmbit / melodicTones.Count;  //// melodicTones.Count > 1 ? realAmbit / melodicTones.Count : 0;

            this.Ambit = (int)realAmbit;
            this.FormalAmbit = formalAmbit;
            this.Density = density;
            this.DetermineExtent(density);
        }
        #endregion
    }
}