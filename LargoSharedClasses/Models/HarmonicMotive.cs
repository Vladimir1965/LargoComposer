// <copyright file="HarmonicMotive.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Harmony;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Harmonic Motive.
    /// </summary>
    [Serializable]
    public sealed class HarmonicMotive
    {
        #region Fields
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicMotive" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="harmonicMotiveNumber">The harmonic motive number.</param>
        public HarmonicMotive(MusicalHeader givenHeader, int harmonicMotiveNumber) : this() {
            this.HarmonicStream = new HarmonicStream(givenHeader);
            this.Shortcut = harmonicMotiveNumber.ToString();
            this.Name = string.Format(CultureInfo.InvariantCulture, "H{0} from {1}", harmonicMotiveNumber, givenHeader.FileName);
            this.Number = harmonicMotiveNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicMotive"/> class.
        /// </summary>
        public HarmonicMotive() {
            this.RhythmicBehavior = new RhythmicBehavior();
            this.HarmonicBehavior = new HarmonicBehavior();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [UsedImplicitly]
        public string Name { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value> Property description. </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets Harmonic Motive Bars.
        /// </summary>
        public HarmonicStream HarmonicStream { get; set; }

        /// <summary>
        /// Gets the outline of bars.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string OutlineOfBars {
            get {
                if (this.Length == 0) {
                    return string.Empty;
                }

                var outline = new StringBuilder();
                foreach (var s in this.HarmonicStream.HarmonicBars) {
                    outline.Append(s.SimpleStructuralOutline);
                    outline.Append(" | ");
                }

                return outline.ToString();
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <returns> Property description. </returns>
        public int Length {
            get {
                if (this.HarmonicStream?.HarmonicBars == null) {
                    return 0;
                }

                return this.HarmonicStream.HarmonicBars.Count;
            }
        }

        /// <summary>
        /// Gets or sets the harmonic behavior.
        /// </summary>
        /// <value>
        /// The harmonic behavior.
        /// </value>
        public HarmonicBehavior HarmonicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic behavior.
        /// </summary>
        /// <value>
        /// The rhythmic behavior.
        /// </value>
        public RhythmicBehavior RhythmicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public string Shortcut { get; set; }
        #endregion

        #region Private Properties
        /// <summary> Gets inner continuity. </summary>
        /// <value> Property description. </value>
        private float MeanContinuity {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicBar s in this.HarmonicStream.HarmonicBars select s.HarmonicBehavior.Continuity).Sum();
                var f = total / this.Length;
                if (f != null) {
                    var meanContinuity = (float)f;
                    return meanContinuity;
                }

                return 0;
            }
        }

        /// <summary> Gets inner impulse. </summary>
        /// <value> Property description. </value>
        private float MeanImpulse {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicBar s in this.HarmonicStream.HarmonicBars select s.HarmonicBehavior.Impulse).Sum();
                var f = total / this.Length;
                if (f != null) {
                    var meanImpulse = (float)f;
                    return meanImpulse;
                }

                return 0;
            }
        }

        /// <summary> Gets inner measure of dissonance. </summary>
        /// <value> Property description. </value>
        private float MeanConsonance {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicBar s in this.HarmonicStream.HarmonicBars select s.HarmonicBehavior.Consonance).Sum();
                var meanConsonance = total / this.Length;
                return meanConsonance;
            }
        }

        /// <summary>
        /// Gets the mean rhythmic mobility.
        /// </summary>
        private float MeanRhythmicMobility {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicBar s in this.HarmonicStream.HarmonicBars select s.RhythmicBehavior.Mobility).Sum();
                var meanValue = total / this.Length;
                return meanValue;
            }
        }

        /// <summary>
        /// Gets the mean rhythmic tension.
        /// </summary>
        private float MeanRhythmicTension {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicBar s in this.HarmonicStream.HarmonicBars select s.RhythmicBehavior.Tension).Sum();
                var meanValue = total / this.Length;
                return meanValue;
            }
        }

        #endregion

        #region Public static methods
        /// <summary>
        /// Gets the new harmonic motive.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="harmonicMotiveNumber">The harmonic motive number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicMotive GetNewHarmonicMotive(MusicalHeader givenHeader, int harmonicMotiveNumber) {
            //// var shortcut = ("0000" + harmonicMotiveNumber.ToString(CultureInfo.CurrentCulture)).Right(4);
            var harMotive = new HarmonicMotive(givenHeader, harmonicMotiveNumber);

            return harMotive;
        }

        /// <summary>
        /// Returns value of characteristic planned for given musical bar.
        /// </summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <param name="system">The system.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <exception cref="ArgumentException">No harmonic motive bar!</exception>
        public HarmonicBar HarmonicBarWithNumber(int barNumber, MusicalSystem system) {
            var listBars = this.HarmonicStream.HarmonicBars; //// (List<HarmonicBar>)this.HarmonicBars;
            var barCount = listBars.Count;
            switch (barCount) {
                case 1:
                    return listBars.First();
                case 0:
                    throw new ArgumentException("No harmonic motive bar!");
            }

            int barnum; //// int lastBarNum = (from tmb in listBars select tmb.BarNumber).Max();
            checked {
                barnum = ((barNumber - 1) % barCount) + 1;  ////  % lastBarNum
            }

            var selectedBar = from trb in listBars where (trb.BarNumber == barnum) select trb;
            //// var system = this.Core.Header.System;
            var harmonicBar = selectedBar.FirstOrDefault() ??
                                     HarmonicBar.EmptyBar(system.HarmonicOrder, system.RhythmicOrder);

            return harmonicBar;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the bar.
        /// </summary>
        /// <param name="bar">The harmonic motive bar.</param>
        public void AddBar(HarmonicBar bar) {
            Contract.Requires(bar != null);

            bar.Recompute();
            ((List<HarmonicBar>)this.HarmonicStream.HarmonicBars).Add(bar);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [UsedImplicitly]
        public override string ToString() {
            return this.Name;
        }

        /// <summary>
        /// Re-computes this instance.
        /// </summary>
        public void Recompute() {
            this.HarmonicBehavior.Continuity = this.MeanContinuity;
            this.HarmonicBehavior.Impulse = this.MeanImpulse;
            this.HarmonicBehavior.Consonance = this.MeanConsonance;
            this.RhythmicBehavior.Mobility = this.MeanRhythmicMobility;
            this.RhythmicBehavior.Tension = this.MeanRhythmicTension;
        }
        #endregion
    }
}
