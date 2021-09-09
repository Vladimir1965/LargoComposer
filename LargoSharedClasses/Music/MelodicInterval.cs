// <copyright file="MelodicInterval.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Music
{
    /// <summary> Melodic interval. </summary>
    /// <remarks> MelodicInterval represents one interval, i.e. relation of two pitches.
    /// Includes characteristics like minimal line Value or easy sing Value used for 
    /// optimization of melodies.  </remarks>
    /// [Serializable]
    [XmlRoot]
    public sealed class MelodicInterval : MusicalInterval
    {
        #region Fields
        /// <summary>
        /// Musical pitch1.
        /// </summary>
        private readonly MusicalPitch pitch1;

        /// <summary>
        /// Musical pitch2.
        /// </summary>
        private readonly MusicalPitch pitch2;

        /// <summary> Interval musical properties. </summary>
        private float? minTrack, easySing;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the MelodicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="givenPitch1">First musical pitch.</param>
        /// <param name="givenPitch2">Second musical pitch.</param>
        public MelodicInterval(HarmonicSystem harmonicSystem, MusicalPitch givenPitch1, MusicalPitch givenPitch2)
            : base(harmonicSystem, givenPitch1, givenPitch2) {
            Contract.Requires(harmonicSystem != null);
            this.pitch1 = givenPitch1;
            this.pitch2 = givenPitch2;
            this.minTrack = null;
            this.easySing = null;
        }

        /// <summary> Initializes a new instance of the MelodicInterval class.  Serializable. </summary>
        public MelodicInterval() {
        }

        /// <summary> Initializes a new instance of the MelodicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        public MelodicInterval(HarmonicSystem harmonicSystem, byte elementFrom, byte elementTo)
            : base(harmonicSystem, elementFrom, elementTo) {
            Contract.Requires(harmonicSystem != null);
        }

        /// <summary> Initializes a new instance of the MelodicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="tone1">First melodic tone.</param>
        /// <param name="tone2">Second melodic tone.</param>
        public MelodicInterval(HarmonicSystem harmonicSystem, MusicalTone tone1, MusicalTone tone2)
            : base(harmonicSystem, tone1, tone2) {
            Contract.Requires(harmonicSystem != null);
            Contract.Requires(tone1 != null);
            Contract.Requires(tone2 != null);
        }
        #endregion

        #region Properties
        /// <summary> Gets the first pitch of the interval. </summary>
        /// <value> Property description. </value>
        public MusicalPitch Pitch1 {
            get {
                Contract.Ensures(Contract.Result<MusicalPitch>() != null);
                if (this.pitch1 == null) {
                    throw new InvalidOperationException("Pitch is null.");
                }

                return this.pitch1;
            }
        }

        /// <summary> Gets the second pitch of the interval. </summary>
        /// <value> Property description. </value>
        public MusicalPitch Pitch2 {
            get {
                Contract.Ensures(Contract.Result<MusicalPitch>() != null);
                if (this.pitch2 == null) {
                    throw new InvalidOperationException("Pitch is null.");
                }

                return this.pitch2;
            }
        }

        /// <summary> Gets the easy-sing value.  </summary>
        /// <value> Evaluates how easy it is to sing given interval. </value>
        public float EasySing {
            get {
                if (this.easySing != null) {
                    return (float)this.easySing;
                }

                var harmonicSystem = this.Pitch2.HarmonicSystem;
                //// int sysLength = this.Pitch1.DistanceFrom(this.Pitch2);
                var frmLength = this.Pitch1.FormalDistanceFrom(this.Pitch2);
                this.easySing = frmLength < harmonicSystem.Median ? 1.0f : -1.0f;
                ////  frmLength == 0 || (sysLength < harmonicSystem.Order && Math.Abs(FormalContinuity) > DefaultValue.Fifty) 
                ////    || (sysLength < harmonicSystem.Order / 2 && FormalImpulse > DefaultValue.Fifty) ? 1.0f : -1.0f;
                return (float)this.easySing;
            }
        }

        /// <summary> Gets the minimum-line value.  </summary>
        /// <value> Is higher for smaller changes in the musical pitch.. </value>
        public float MinimumMotion {
            ////  float impulseValue = (fDist == 0) ? -1 : r;
            get {
                if (this.minTrack != null) {
                    return (float)this.minTrack;
                }

                var harmonicSystem = this.Pitch2.HarmonicSystem;
                float distance = this.Pitch2.DistanceFrom(this.Pitch1);
                var r = distance / harmonicSystem.Order; ////  harmonicSystem.Order != 0 ? distance / harmonicSystem.Order : 0;

                this.minTrack = r;

                return (float)this.minTrack;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.Pitch1);
            s.Append(this.Pitch2);
            return s.ToString();
        }
        #endregion
    }
}
