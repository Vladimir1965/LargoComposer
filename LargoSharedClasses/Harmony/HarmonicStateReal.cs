// <copyright file="HarmonicStateReal.cs" company="Traced-Ideas, Czech republic">
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
using System.Xml.Serialization;

namespace LargoSharedClasses.Harmony
{
    /// <summary> Real harmonic state. </summary>
    /// <remarks> Harmonic state represents relations in one real harmonic cluster
    /// and enable to Compute its characteristics (continuity, impulse,..). </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicStateReal : HarmonicTransfer {
        #region Fields
        /// <summary> List  of tones. </summary>
        private readonly MusicalToneCollection toneList;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicStateReal class.  Serializable. </summary>
        public HarmonicStateReal() {
        }

        /// <summary> Initializes a new instance of the HarmonicStateReal class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="toneArray">List of melodic tones.</param>
        public HarmonicStateReal(HarmonicSystem harmonicSystem, MusicalToneCollection toneArray)
            : base(harmonicSystem) {
                Contract.Requires(harmonicSystem != null);
                Contract.Requires(toneArray != null);
            //// if (toneArray == null) { return; } 
            this.toneList = toneArray;
            if (this.toneList.Count > 1) {
                byte idx = 0;
                //// string tmpStr = string.Empty;
                foreach (var mt in this.ToneList) {
                    mt.ToneIndex = idx++;
                    //// tmpStr += MusicalTone.GetNoteName(mt.Pitch.Element) + mt.Pitch.Octave;
                }

                //// HarmonicStateReal.LocalTemporaryProtocol += tmpStr + "\r\n";
                this.AddAllIntervals();
            }

            this.SetRealProperties();
        }

        /// <summary> Initializes a new instance of the HarmonicStateReal class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="toneArray">List of melodic tones.</param>
        /// <param name="givenTone">Melodic tone.</param>
        public HarmonicStateReal(HarmonicSystem harmonicSystem, MusicalToneCollection toneArray, MusicalTone givenTone)
            : base(harmonicSystem) {
                Contract.Requires(harmonicSystem != null);
                Contract.Requires(givenTone != null);
                Contract.Requires(toneArray != null);

            //// if (toneArray == null) {  return;  }
            this.toneList = toneArray;
            if (this.toneList.Count > 1) {
                byte idx = 0;
                foreach (var mt in this.ToneList) {
                    mt.ToneIndex = idx++;
                }
            }

            this.AddIntervalsLeadingToTone(givenTone);
            this.SetRealProperties();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStateReal class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public HarmonicStateReal(HarmonicSystem givenSystem)
            : base(givenSystem) {
                Contract.Requires(givenSystem != null); 
        }
        #endregion

        #region Properties
        //// Leads to out of memory exception
        //// public static Dictionary<string, HarmonicStateReal> UsedStates = new Dictionary<string,HarmonicStateReal>();

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float RealContinuity { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float RealImpulse { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float RealPotential { get; private set; }

        /// <summary> Gets the property. </summary>        
        /// <value> Property description. </value>
        public float RealConsonance { get; private set; }

        /// <summary> Gets list of tones. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public MusicalToneCollection ToneList {
            get {
                Contract.Ensures(Contract.Result<MusicalToneCollection>() != null);
                if (this.toneList == null) {
                    throw new InvalidOperationException("List of tones is null.");
                }

                return this.toneList;
            }
        }

        #endregion

        #region Public methods
        /// <summary> Adds to the given array real intervals to given tone. </summary>
        /// <param name="toTone">Melodic tone.</param>
        public void AddIntervalsLeadingToTone(MusicalTone toTone) {
            Contract.Requires(toTone != null);
            //// if (toTone == null) { return; }
            //// const bool ignoreLongIntervals = true;
            //// this.HarmonicSystem.GetRealInterval(fromTone.Pitch.SystemAltitude, toTone.Pitch.IntervalFrom(fromTone.Pitch));
            //// if (toTone.Pitch == null)  { return; }

            var harmonicSystem = this.HarmonicSystem;
            int toneIndex = toTone.ToneIndex;
            //// 2011/11 - interval with two equal tones ignored
            var toneAltitude = toTone.Pitch.SystemAltitude; 
            foreach (var interval in from fromTone in this.ToneList
                                                 where fromTone != null && fromTone.ToneIndex != toneIndex
                                                        && fromTone.Pitch != null && fromTone.Pitch.SystemAltitude != toneAltitude
                                     select new MusicalInterval(harmonicSystem, fromTone, toTone))
            {
                this.Intervals.Add(interval);
            }
        }

        /// <summary> Sets harmonic properties of the cluster. </summary>
        public void SetRealProperties() {
            switch (this.Intervals.Count)
            {
                case 0:
                    return;
                case 1:
                    this.SetRealPropertiesOfSingleTone();
                    return;
            }

            // float Level = (float)(Math.Sqrt(1+8*intervals.Count)+1)/2;
            var rcontinuity = this.MeanValueOfProperty(GenProperty.RealContinuity, true, true);
            var rimpulse = this.MeanValueOfProperty(GenProperty.RealImpulse, true, true);
            var rsonance = HarmonicSystem.Consonance(rcontinuity, rimpulse);

            this.RealContinuity = rcontinuity;
            this.RealImpulse = rimpulse;
            this.RealPotential = 0f;
            this.RealConsonance = rsonance;
        }

        /// <summary> Sets harmonic properties of the cluster. </summary>
        public void SetRealPropertiesOfSingleTone() {
            this.RealContinuity = 0f;
            this.RealImpulse = 0f;
            this.RealPotential = 0f;
            this.RealConsonance = 0f;
        }
        #endregion

        #region Private methods
        /// <summary> Makes array of intervals between tones of the cluster. </summary>
        private void AddAllIntervals()
        {
            foreach (var mt in this.ToneList.Where(mt => mt?.Pitch != null))
            {
                this.AddIntervalsLeadingToTone(mt);
            }
        }
        #endregion
    }
}
