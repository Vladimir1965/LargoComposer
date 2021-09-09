// <copyright file="HarmonicSpace.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace LargoSharedClasses.Harmony
{
    /// <summary>
    /// Harmonic space to help analyze of harmony.
    /// </summary>
    public sealed class HarmonicSpace {
        #region Fields
        /// <summary>
        /// Near-zero value.
        /// </summary>
        private const float NearZero = 0.01f;

        /// <summary>
        /// Harmonic System.
        /// </summary>
        private readonly HarmonicSystem harmonicSystem; 
        
        /// <summary>
        /// Musical bands.
        /// </summary>
        private List<HarmonicBand> musicalBands;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HarmonicSpace class.
        /// </summary>
        /// <param name="givenHarmonicSystem">The given harmonic system.</param>
        public HarmonicSpace(HarmonicSystem givenHarmonicSystem) {
            Contract.Requires(givenHarmonicSystem != null);
            this.harmonicSystem = givenHarmonicSystem;
            this.MusicalBands = new List<HarmonicBand>();
            for (var i = 0; i < this.harmonicSystem.Order; i++) {
                var b = new HarmonicBand(i, 0.0f);
                this.MusicalBands.Add(b);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicSpace"/> class.
        /// </summary>
        [UsedImplicitly]
        public HarmonicSpace() {       
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether [consider energy decrease in time].
        /// </summary>
        /// <value>
        /// <c>True</c> if [consider energy decrease in time]; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool ConsiderEnergyDecreaseInTime { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets a value indicating whether [consider harmonic bindings].
        /// </summary>
        /// <value>
        /// <c>True</c> if [consider harmonic bindings]; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool ConsiderImpulseBindings { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets a value indicating whether [strong impulse bindings].
        /// </summary>
        /// <value>
        /// <c>True</c> if [strong impulse bindings]; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool StrongImpulseBindings { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets a value indicating whether [consider continuity bindings].
        /// </summary>
        /// <value>
        /// <c>True</c> if [consider continuity bindings]; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool ConsiderContinuityBindings { get; set; }   //// CA1044 (FxCop)
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets the musical bands.
        /// </summary>
        /// <value>
        /// The musical bands.
        /// </value>
        private IList<HarmonicBand> MusicalBands {
            get {
                Contract.Ensures(Contract.Result<IList<MusicalBand>>() != null);
                return this.musicalBands ?? (this.musicalBands = new List<HarmonicBand>());
            }

            set => this.musicalBands = (List<HarmonicBand>)value;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Reset analyzer.
        /// </summary>
        /// <param name="harmonicModality">Harmonic modality.</param>
        public void Reset(BinaryStructure harmonicModality) {  //// HarmonicModality
            Contract.Requires(harmonicModality != null);

            for (byte i = 0; i < this.harmonicSystem.Order; i++) {
                if (i >= this.MusicalBands.Count) {
                    continue;
                }

                if (this.MusicalBands[i] == null) {
                    continue;
                }

                this.MusicalBands[i].Value = 0.0f;
                this.MusicalBands[i].Modal = harmonicModality.IsOn(i);
                this.MusicalBands[i].SonanceBonus = 0.0f;
            }
        }

        /// <summary>
        /// Forgets this instance.
        /// </summary>
        public void Forget() {
            const int forgetFactor = 2;

            for (byte i = 0; i < this.harmonicSystem.Order; i++) {
                if (i >= this.MusicalBands.Count) {
                    continue;
                }

                if (this.MusicalBands[i] == null) {
                    continue;
                }

                this.MusicalBands[i].Value = this.MusicalBands[i].Value / forgetFactor;
            }
        }

        /// <summary>
        /// Melodic tone collection.
        /// </summary>
        /// <param name="tones">Melodic tones.</param>
        public void AcceptTonesOfTheTick(IEnumerable<MusicalTone> tones) {
            const float withdraw = 0.5f; //// 0.1f seems to be not enough
            Contract.Requires(tones != null);
            //// if (tones == null) {  return;  }

            if (this.ConsiderEnergyDecreaseInTime) {
                //// Stepwise decrease of band activity 
                foreach (var mb in this.MusicalBands) {
                    if (mb.Value > 0) {
                        mb.Value = Math.Max(mb.Value - withdraw, 0.0f);
                    }

                    if (mb.Value < 0) {
                        mb.Value = Math.Min(mb.Value + withdraw, 0.0f);
                    }
                }
            }

            var elements = (from MusicalTone tone in tones
                            where tone != null && tone.IsTrueTone
                            select tone.Pitch.SystemAltitude % this.harmonicSystem.Order).ToList(); ////.Distinct();
            elements.ForEach(band => this.ChangeToneValue(band, 1.0f));
            //// soundingTones.I 
        }

        /// <summary>
        /// Determine harmonic structure.
        /// </summary>
        /// <param name="givenMaxTonesInChord">The given max tones in chord.</param>
        /// <param name="fullHarmonization">If set to <c>true</c> [full harmonization].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicStructure DetermineHarmonicStructure(byte givenMaxTonesInChord, bool fullHarmonization) {
            var hstruct = new HarmonicStructure(this.harmonicSystem, (string)null);
            int limit = givenMaxTonesInChord; //// max number of tones in the structure
            if (fullHarmonization) {
                const float minimumValue = 10.0f; //// 0.1
                const float bandLowerLimit = -1000.0f;
                this.ResetSonanceBonus();
                //// Sorts toneValues by weight 
                var positiveBands = (from band in this.MusicalBands
                                     where band.Modal && band.Value > minimumValue
                                     orderby band.Value
                                descending
                                     select (byte)band.Number).ToList();

                if (positiveBands.Count < limit) {
                    limit = positiveBands.Count;
                }

                for (var number = 0; number < limit; number++) {
                    //// Sorts toneValues by weight 
                    var bands = (from band in this.MusicalBands
                                         where band.Modal && band.Value + band.SonanceBonus > minimumValue
                                         orderby band.Value + band.SonanceBonus
                                    descending
                                         select (byte)band.Number).ToList();

                    byte elem;
                    if (bands.Count > 0) {
                        elem = bands.FirstOrDefault(); ////0.01
                    }
                    else {
                        var topBands = (from band in this.MusicalBands
                                        orderby band.Value + band.SonanceBonus
                                        descending
                                        select band).ToList();
                        //// For four-part harmonization are full chords needed, so no bandLowerLimit can be used;
                        elem = (from band in topBands
                                where band.Modal && band.Value + band.SonanceBonus > bandLowerLimit
                                select (byte)band.Number).FirstOrDefault(); ////0.01
                    }

                    hstruct.On(elem);
                    this.IncreaseSonanceBonus(elem, 1.0f);
                }
            }
            else {
                const float bandLowerLimit = 0.1f;

                //// Sorts toneValues by weight 
                var topBands = (from mb in this.MusicalBands orderby mb.Value descending select mb).ToList();
                foreach (var elem in from band in topBands 
                                      where band.Modal && band.Value > bandLowerLimit
                                      select (byte)band.Number) { ////0.01
                    hstruct.On(elem);
                    limit--;

                    if (limit == 0) {
                        break;
                    }
                }
            }

            hstruct.DetermineLevel();
            hstruct.DetermineBehavior();
            //// if (hstruct.FormalImpulse > 24) { return lastHarmonicStructure; } 

            hstruct.DetermineStructuralCode();
            hstruct.DetermineShortcut();
            //// string s = hstruct.ToneSchema;
            return hstruct;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Resets the consonance bonus.
        /// </summary>
        private void ResetSonanceBonus() {
           for (byte i = 0; i < this.harmonicSystem.Order; i++) {
                if (i >= this.MusicalBands.Count) {
                    continue;
                }

                if (this.MusicalBands[i] == null) {
                    continue;
                }

                this.MusicalBands[i].SonanceBonus = 0.0f;
           }
        }

        /// <summary>
        /// Increases the consonance bonus.
        /// </summary>
        /// <param name="bandNumber">The band number.</param>
        /// <param name="value">The value.</param>
        private void IncreaseSonanceBonus(int bandNumber, float value) {
            Contract.Requires(this.harmonicSystem != null);
            int order = this.harmonicSystem.Order;

            //// Exclude used band and its halftone surrounding (2015/1)
            if (this.MusicalBands[bandNumber] != null) {
                var bimpulse1d = (bandNumber + order - 1) % order;
                var bimpulse1U = (bandNumber + 1) % order;
                this.MusicalBands[bandNumber].SonanceBonus -= 1000;
                this.MusicalBands[bimpulse1d].SonanceBonus -= 100;
                this.MusicalBands[bimpulse1U].SonanceBonus -= 100;
            }

            //// float halfValue = value/2;
            var bcontinuity7U = (bandNumber + order + 7) % order;
            var bcontinuity4U = (bandNumber + order + 4) % order;
            var bcontinuity5U = (bandNumber + order - 7) % order;
            var bcontinuity8U = (bandNumber + order - 4) % order;
            //// int bcontinuity3u = (bandNumber + order + 3) % order;
            //// int bcontinuity9u = (bandNumber + order - 3) % order;

            if (this.MusicalBands[bcontinuity7U] != null) {
                this.MusicalBands[bcontinuity7U].SonanceBonus += value;
            }

            if (this.MusicalBands[bcontinuity4U] != null) {
                this.MusicalBands[bcontinuity4U].SonanceBonus += value;
            }

            if (this.MusicalBands[bcontinuity5U] != null) {
                this.MusicalBands[bcontinuity5U].SonanceBonus += value;
            }
            
            if (this.MusicalBands[bcontinuity8U] != null) {
                this.MusicalBands[bcontinuity8U].SonanceBonus += value;
            }
        }

        /// <summary>
        /// For 12-tone system only.
        /// </summary>
        /// <param name="bandNumber">The band number.</param>
        /// <param name="value">Energy value.</param>
        private void ChangeToneValue(int bandNumber, float value) {
            Contract.Requires(bandNumber >= 0);

            byte primaryInfluenceFactor = 3; //// 2
            byte secondaryInfluenceFactor = 6; //// 4
            if (this.StrongImpulseBindings)
            {
                primaryInfluenceFactor = 1; //// 2
                secondaryInfluenceFactor = 2; //// 4                
            }

            var quotientV1 = value / primaryInfluenceFactor;
            var quotientV2 = value / secondaryInfluenceFactor;
            var harSystem = this.harmonicSystem;
            if (harSystem.Order == 0) {
                return;
            }

            if (bandNumber >= 0 && bandNumber < this.MusicalBands.Count && this.MusicalBands[bandNumber] != null) {
                this.MusicalBands[bandNumber].Value += value;
                //// if (impulse1) { this.MusicalBands[bandNumber].Value -= quotientV1; }
            }

            if (this.ConsiderImpulseBindings) {
                //// Main influence
                this.MainInfluence(bandNumber, value, quotientV1);
            }

            if (this.ConsiderContinuityBindings) {
                //// Continuity to central tone
                this.ContinuityToCentralTone(bandNumber, quotientV1, quotientV2);
            }

            //// Influence due to sound color
            this.InfluenceDueToSoundColor(bandNumber, value);
        }

        /// <summary>
        /// Mains the influence.
        /// </summary>
        /// <param name="bandNumber">The band number.</param>
        /// <param name="value">The value.</param>
        /// <param name="quotientV1">The quotient v1.</param>
        private void MainInfluence(int bandNumber, float value, float quotientV1) {
            Contract.Requires(this.harmonicSystem != null);
            int order = this.harmonicSystem.Order;
            var halfValue = value / 2;

            var bimpulse2d = (bandNumber + order - 2) % order;
            var bimpulse1d = (bandNumber + order - 1) % order;
            var bimpulse1U = (bandNumber + 1) % order;
            var bimpulse2U = (bandNumber + 2) % order;
            //// bool impulse1 = false;  bool impulse2 = false;

            if (this.MusicalBands[bimpulse2d] != null) {
                //// if (this.MusicalBands[bimpulse2d].Value > 0) {
                this.MusicalBands[bimpulse2d].Value = Math.Max(this.MusicalBands[bimpulse2d].Value - quotientV1, NearZero);
                //// }
                ////impulse2 = true;
            }

            if (this.MusicalBands[bimpulse2U] != null) {
                //// 2013/08 this.MusicalBands[bimpulse2u].Value = -quotientV1;
                //// if (this.MusicalBands[bimpulse2u].Value > 0) {
                this.MusicalBands[bimpulse2U].Value = Math.Max(this.MusicalBands[bimpulse2U].Value - quotientV1, NearZero);
                //// }
                ////impulse2 = true;
            }

            if (this.MusicalBands[bimpulse1d] != null) {
                this.MusicalBands[bimpulse1d].Value -= halfValue; //// -= quotientV1;
                ////impulse1 = true;
            }

            if (this.MusicalBands[bimpulse1U] != null) {
                this.MusicalBands[bimpulse1U].Value -= halfValue; ////-= quotientV1;
                ////impulse1 = true;
            }
        }

        /// <summary>
        /// Influences the color of the due to sound.
        /// </summary>
        /// <param name="bandNumber">The band number.</param>
        /// <param name="value">The value.</param>
        private void InfluenceDueToSoundColor(int bandNumber, float value) {
            Contract.Requires(this.harmonicSystem != null);
            int order = this.harmonicSystem.Order;
            
            var colorValue = value / 10;
            var bcontinuity7U = (bandNumber + order + 7) % order;
            var bcontinuity4U = (bandNumber + order + 4) % order;

            if (this.MusicalBands[bcontinuity7U] != null) {
                this.MusicalBands[bcontinuity7U].Value += colorValue;
            }

            if (this.MusicalBands[bcontinuity4U] != null) {
                this.MusicalBands[bcontinuity4U].Value += colorValue;
            }
            //// toneValue[bcontinuity4u] -= v2;
            //// toneValue[bcontinuity7u] -= v4;
        }

        /// <summary>
        /// Continuities to central tone.
        /// </summary>
        /// <param name="bandNumber">The band number.</param>
        /// <param name="quotientV1">The quotient v1.</param>
        /// <param name="quotientV2">The quotient v2.</param>
        private void ContinuityToCentralTone(int bandNumber, float quotientV1, float quotientV2) {
            Contract.Requires(this.harmonicSystem != null);
            int order = this.harmonicSystem.Order;

            var bcontinuity7d = (bandNumber + order - 7) % order;
            var bcontinuity4d = (bandNumber + order - 4) % order;
            //// int bcontinuity4u = (band + 4) % harSystem.Order;
            //// int bcontinuity7u = (band + 7) % harSystem.Order;
            if (this.MusicalBands[bcontinuity7d] != null) {
                if (this.MusicalBands[bcontinuity7d].Value < 0) {
                    this.MusicalBands[bcontinuity7d].Value = Math.Min(this.MusicalBands[bcontinuity7d].Value + quotientV1, -NearZero);
                }
            }

            if (this.MusicalBands[bcontinuity4d] == null) {
                return;
            }

            if (this.MusicalBands[bcontinuity4d].Value < 0) {
                this.MusicalBands[bcontinuity4d].Value = Math.Min(this.MusicalBands[bcontinuity4d].Value + quotientV2, -NearZero);
            }
        }
        #endregion
    }
}
