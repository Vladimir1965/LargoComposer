// <copyright file="HarmonicStreamAnalyzer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using LargoSharedClasses.Harmony;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Harmonic Stream Analyzer.
    /// </summary>
    public class HarmonicStreamAnalyzer {
        #region Fields
        /// <summary>
        /// Maximum number of Tones In Chord.
        /// </summary>
        private readonly byte maxTonesInChord;

        /// <summary>
        /// Full Harmonization.
        /// </summary>
        private readonly bool fullHarmonization;

        /// <summary>
        /// The header
        /// </summary>
        private readonly MusicalHeader header;

        /// <summary>
        /// Last harmonic structure.
        /// </summary>
        private HarmonicStructure lastHarmonicStructure;

        /// <summary>
        /// The first tick.
        /// </summary>
        private byte firstTick;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStreamAnalyzer" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="givenMaxTonesInChord">The given max tones in chord.</param>
        /// <param name="givenFullHarmonization">If set to <c>true</c> [given full harmonization].</param>
        public HarmonicStreamAnalyzer(MusicalHeader givenHeader, byte givenMaxTonesInChord, bool givenFullHarmonization) {
            this.header = givenHeader;
            this.maxTonesInChord = givenMaxTonesInChord;
            this.fullHarmonization = givenFullHarmonization;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Harmonic Space.
        /// </summary>
        public HarmonicSpace HarmonicSpace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Sharp Chord Edges.
        /// </summary>
        public bool SharpChordEdges { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return $"HarmonicStreamAnalyzer (SharpChordEdges {this.SharpChordEdges})";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determine Harmony In Bar.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicBar DetermineHarmonyInBar(MusicalBar givenBar) {
            var sourceTones = givenBar.MelodicTonesAround(6, 6);
            var harmonicModality = new HarmonicModality(this.header.System.HarmonicOrder, sourceTones, 0, false);
            var barTones = givenBar.MelodicTones;
            if (barTones == null) {
                return null;
            }

            HarmonicBar harmonicBar;

            var musicalTones = barTones;
            if (!musicalTones.Any()) {
                harmonicBar = HarmonicBar.EmptyBar(this.header.System.HarmonicOrder, this.header.System.RhythmicOrder);
                harmonicBar.BarNumber = 1; //// 2016/08 givenBar.BarNumber; //// 2016/08 ?!
                return harmonicBar;
            }

            var rsystem = this.header.System.RhythmicSystem;
            var rorder = this.header.System.RhythmicOrder;
            var barMetric = new BinaryStructure(rsystem, 0);
            //// barMetric.On(0);
            harmonicBar = new HarmonicBar(0, givenBar.BarNumber) { Header = givenBar.Body.Context.Header };

            this.lastHarmonicStructure = null;
            //// int length;
            this.firstTick = 0;
            this.HarmonicSpace.Reset(harmonicModality);
            for (byte tick = 0; tick < rorder; tick++) {
                var br = new BitRange(rorder, tick, 1);
                var tonesAtTick = new MusicalToneCollection(musicalTones, br, false);
                this.AnalyzeHarmonyAtTick(harmonicBar, tick, tonesAtTick, barMetric);
            }

            if (this.firstTick < this.header.System.RhythmicOrder) {
                var harmonicStructure = this.HarmonicSpace.DetermineHarmonicStructure(this.maxTonesInChord, this.fullHarmonization);
                if (harmonicStructure != null) {
                    this.ResolveStructure(harmonicBar, barMetric, harmonicStructure, this.header.System.RhythmicOrder);
                }
            }

            barMetric.DetermineLevel();
            var barMetricCode = barMetric.GetStructuralCode;
            harmonicBar.SetBarMetricCode(barMetricCode);
            harmonicBar.SetHarmonicModalityCode(harmonicModality.GetStructuralCode);
            return harmonicBar;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Analyzes the harmony at tick.
        /// </summary>
        /// <param name="harmonicBar">The harmonic motive bar.</param>
        /// <param name="givenTick">The tick.</param>
        /// <param name="givenTones">The melodic tones.</param>
        /// <param name="barMetric">The bar metric.</param>
        private void AnalyzeHarmonyAtTick(HarmonicBar harmonicBar, byte givenTick, ICollection<MusicalTone> givenTones, BinaryStructure barMetric) {
            Contract.Requires(givenTones != null);

            var tick = givenTick;

            var anyStartAtThisTick = givenTones.Count > 0 && (from dt in givenTones
                                                          where dt.BitFrom == tick
                                                          select 1).Any();

            if (anyStartAtThisTick && tick > 0) {
                var harmonicStructure = this.HarmonicSpace.DetermineHarmonicStructure(this.maxTonesInChord, this.fullHarmonization);
                if (harmonicStructure != null) {
                    //// var harSystem = harmonicStructure.HarmonicSystem; //// HarmonicSystem.GetHarmonicSystem(this.SysOrder);
                    //// var modality = new HarmonicModality(harSystem, 1387);
                    ////201508 harmonicStructure = DataLink.BridgeHarmony.CompleteHarmonicStructure(harSystem, modality, harmonicStructure.GetStructuralCode());
                    //// harmonicStructure.DetermineBehavior();

                    this.ResolveStructure(harmonicBar, barMetric, harmonicStructure, tick);
                    this.firstTick = tick;

                    if (this.SharpChordEdges) {
                        this.HarmonicSpace.Forget();
                        //// this.HarmonicSpace.Reset(harmonicModality);
                    }
                }
            }
            else {
                barMetric.Off(tick);
            }

            if (givenTones.Count > 0) {
                this.HarmonicSpace.AcceptTonesOfTheTick(givenTones);
            }
        }

        /// <summary>
        /// Resolves the structure.
        /// </summary>
        /// <param name="harmonicBar">The harmonic motive bar.</param>
        /// <param name="barMetric">The bar metric.</param>
        /// <param name="harmonicStructure">The harmonic structure.</param>
        /// <param name="tick">The given tick.</param>
        private void ResolveStructure(HarmonicBar harmonicBar, BinaryStructure barMetric, HarmonicStructure harmonicStructure, byte tick) {
            Contract.Requires(barMetric != null);
            Contract.Requires(harmonicStructure != null);

            var extendPrevious = (this.lastHarmonicStructure != null) && (string.CompareOrdinal(harmonicStructure.ElementSchema, this.lastHarmonicStructure.ElementSchema) == 0);
            if (extendPrevious) {
                barMetric.Off(this.firstTick);
                this.lastHarmonicStructure.Length = (byte)(this.lastHarmonicStructure.Length + tick - this.firstTick);
            }
            else {
                barMetric.On(this.firstTick);
                harmonicStructure.BitFrom = this.firstTick;
                var length = tick - this.firstTick;
                harmonicStructure.Length = (byte)length;
                harmonicBar.AddStructure(harmonicStructure);
                this.lastHarmonicStructure = harmonicStructure;
            }
        }
        #endregion
    }
}
