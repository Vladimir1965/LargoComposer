// <copyright file="HarmonicInterval.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Music
{
    /// <summary> Harmonic interval. </summary>
    /// <remarks>
    /// HarmonicInterval represents one formal interval,
    /// i.e. relation of two tones of the formal harmonic GSystem.
    /// Interval has assigned name, (formal) distance and values
    /// of continuity, impulse, potential influence and Similarity. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicInterval : MusicalInterval {
        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicInterval class.  Serializable. </summary>
        public HarmonicInterval() {
        }

        /// <summary> Initializes a new instance of the HarmonicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="length">Interval length.</param>
        /// <param name="weight">Weight of darkness.</param>
        public HarmonicInterval(HarmonicSystem harmonicSystem, byte length, float weight) {
            Contract.Requires(harmonicSystem != null);
            //// if (harmonicSystem == null) {  return;  }

            this.HarmonicSystem = harmonicSystem;
            this.SystemLength = length;
            this.Weight = weight;
            this.FormalLength = harmonicSystem.FormalLength(this.SystemLength);
            this.Name = harmonicSystem.GuessNameForInterval(this.FormalLength);
            this.Ratio = harmonicSystem.RatioForInterval(this.SystemLength);
            this.Halftones = harmonicSystem.HalftonesForInterval(this.FormalLength);
        }

        /// <summary> Initializes a new instance of the HarmonicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="elementFrom">Fist element of system.</param>
        /// <param name="elementTo">Second element of system.</param>
        public HarmonicInterval(HarmonicSystem harmonicSystem, byte elementFrom, byte elementTo)
            : base(harmonicSystem, elementFrom, elementTo) {
                Contract.Requires(harmonicSystem != null);
        }

        /// <summary> Initializes a new instance of the HarmonicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="givenPitch1">First musical pitch.</param>
        /// <param name="givenPitch2">Second musical pitch.</param>
        public HarmonicInterval(HarmonicSystem harmonicSystem, MusicalPitch givenPitch1, MusicalPitch givenPitch2)
            : base(harmonicSystem, givenPitch1, givenPitch2) {
                Contract.Requires(harmonicSystem != null);
        }

        /// <summary> Initializes a new instance of the HarmonicInterval class. </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="tone1">First melodic tone.</param>
        /// <param name="tone2">Second melodic tone.</param>
        public HarmonicInterval(HarmonicSystem harmonicSystem, MusicalTone tone1, MusicalTone tone2)
            : base(harmonicSystem, tone1, tone2) {
                Contract.Requires(harmonicSystem != null);
                Contract.Requires(tone1 != null);
                Contract.Requires(tone2 != null);
        }
         
        #endregion

        #region Properties
        /// <summary> Gets or sets length in halftones. </summary>
        /// <value> Property description. </value>
        public float Halftones { get; set; }

        /// <summary> Gets or sets name. </summary>
        /// <value> Property description. </value>
        public string Name { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(string.Format(CultureInfo.InvariantCulture, "{0}/{1}\t", string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,3}", this.FormalLength), string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,12}", this.Name)));
            //// s.Append(this.StringOfProperties());
            return s.ToString();
        }
        #endregion
    }
}
