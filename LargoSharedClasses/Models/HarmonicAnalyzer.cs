// <copyright file="HarmonicAnalyzer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
namespace LargoSharedClasses.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;

    /// <summary>
    /// Harmonic Analyzer.
    /// </summary>
    public sealed class HarmonicAnalyzer
    {
        #region Fields
        /// <summary>
        /// Musical Block.
        /// </summary>
        private readonly MusicalBody musicalBody;

        /// <summary>
        /// Harmonic motive number.
        /// </summary>
        private int harMotiveNumber;

        /// <summary>
        /// Covered bars by harmony.
        /// </summary>
        private BitArray coveredBars;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicAnalyzer" /> class.
        /// </summary>
        /// <param name="givenMusicalBody">The given musical body.</param>
        public HarmonicAnalyzer(MusicalBody givenMusicalBody)
        {
            this.musicalBody = givenMusicalBody;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicAnalyzer"/> class.
        /// </summary>
        [UsedImplicitly]
        public HarmonicAnalyzer()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Musical Form.
        /// </summary>
        private HarmonicModel HarmonicModel { get; set; }

        #endregion

        #region Public methods - Harmonic Analyze
        /// <summary>
        /// Extract Harmonical Motive.
        /// </summary>
        /// <param name="harmonicModel">The harmonic model.</param>
        /// <param name="analyzeType">Harmonic Analyze Type.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IEnumerable<AbstractChange> AnalyzeHarmony(HarmonicModel harmonicModel, HarmonicAnalysisType analyzeType)
        {
            this.HarmonicModel = harmonicModel;
            var harmonicStream = new HarmonicStream(harmonicModel.Header);  //// "Derived from core"
            //// this.HarmonicStreamAnalyzer.ExtractHarmonicStreamByTicks();
            var changes = new List<AbstractChange>();
            var lastModalityCode = string.Empty;
            switch (analyzeType) {
                case HarmonicAnalysisType.DivisionByTicks: {
                        foreach (var bar in this.musicalBody.Bars) {
                            var harmonicBar = bar.HarmonicBar;
                            if (harmonicBar == null) {
                                continue;
                            }

                            harmonicStream.HarmonicBars.Add(harmonicBar);
                            var code = harmonicBar.GetHarmonicModalityCode;
                            if (code == lastModalityCode) {
                                continue;
                            }

                            var tc = new TonalityChange(bar.BarNumber) { HarmonicModalityCode = code };
                            changes.Add(tc);
                            lastModalityCode = code;
                        }

                        var harmonicChanges = this.ExtractHarmonicalMotive(harmonicStream);
                        changes.AddRange(harmonicChanges);
                    }

                    break;
            }

            return changes;
        }

        /// <summary>
        /// Finds the repeated harmonic motive.
        /// </summary>
        /// <param name="harmonicStream">The harmonic stream.</param>
        /// <returns> Returns value. </returns>
        private HarmonicChange FindRepeatedHarmonicMotive(HarmonicStream harmonicStream)
        {
            Contract.Requires(harmonicStream != null);

            const int minMotiveLength = 3;
            var numberOfBars = harmonicStream.HarmonicBars.Count;
            for (var barIdx = 0; barIdx < numberOfBars; barIdx++) {
                if (barIdx >= this.coveredBars.Length) {
                    continue;
                }

                if (this.coveredBars[barIdx]) {
                    continue;
                }

                var harmonicBar = harmonicStream.HarmonicBars.ElementAt(barIdx);
                if (harmonicBar == null) {
                    continue;
                }

                for (var nextIdx = barIdx + 1; nextIdx < numberOfBars; nextIdx++) {
                    if (nextIdx >= this.coveredBars.Count) { //// 2017/02 error
                        break;
                    }

                    if (this.coveredBars[nextIdx]) {
                        continue;
                    }

                    var nextBar = harmonicStream.HarmonicBars.ElementAt(nextIdx);
                    if (nextBar == null) {
                        continue;
                    }

                    //// Test if next repetition exists
                    var existsRepeatedMotive = string.CompareOrdinal(harmonicBar.UniqueIdentifier, nextBar.UniqueIdentifier) == 0
                                               && harmonicStream.EqualSegments(barIdx + 1, nextIdx + 1, minMotiveLength - 1);
                    if (!existsRepeatedMotive) {
                        continue;
                    }

                    //// Determine Length of the repeated motive
                    var lengthOfMotive = harmonicStream.LengthOfMotive(barIdx, nextIdx);
                    lengthOfMotive = Math.Min(lengthOfMotive, nextIdx - barIdx);
                    this.harMotiveNumber++;
                    var harMotive = HarmonicMotive.GetNewHarmonicMotive(harmonicStream.Header, this.harMotiveNumber);
                    harmonicStream.WriteToMotive(harMotive, barIdx, lengthOfMotive); //// , 1
                    var change = new HarmonicChange(barIdx + 1) { HarmonicMotive = harMotive };
                    //// change.BlockModel = this.MusicalBlock.SourceMusicalBlockModel;  
                    for (var b = barIdx; b < barIdx + lengthOfMotive; b++) {
                        if (b < this.coveredBars.Length) {
                            this.coveredBars[b] = true;
                        }
                    }

                    //// break;
                    return change;
                }
            }

            return null;
        }

        /// <summary>
        /// Occupies the stream by harmonic motive.
        /// </summary>
        /// <param name="harmonicStream">The harmonic stream.</param>
        /// <param name="givenChange">The given change.</param>
        /// <returns> Returns value. </returns>
        private IEnumerable<HarmonicChange> OccupyStreamByHarmonicMotive(HarmonicStream harmonicStream, HarmonicChange givenChange)
        {
            Contract.Requires(harmonicStream != null);
            Contract.Requires(givenChange != null);

            var harmonicChanges = new Collection<HarmonicChange> { givenChange };
            var numberOfBars = harmonicStream.HarmonicBars.Count;
            var barIdx = givenChange.BarNumber - 1;
            var harmonicBar = harmonicStream.HarmonicBars.ElementAt(barIdx);
            if (harmonicBar == null) {
                return harmonicChanges;
            }

            var nextBars = Math.Min(numberOfBars, this.coveredBars.Count);
            for (var nextIdx = barIdx + 1; nextIdx < nextBars; nextIdx++) {
                if (this.coveredBars[nextIdx]) {
                    continue;
                }

                var nextBar = harmonicStream.HarmonicBars.ElementAt(nextIdx);
                if (nextBar == null) {
                    continue;
                }

                //// Test if next repetition exists
                var existsRepeatedMotive = string.CompareOrdinal(harmonicBar.UniqueIdentifier, nextBar.UniqueIdentifier) == 0
                                           && harmonicStream.EqualSegments(barIdx + 1, nextIdx + 1, givenChange.HarmonicMotive.Length - 1);
                if (!existsRepeatedMotive) {
                    continue;
                }

                var change = new HarmonicChange(nextIdx + 1) { HarmonicMotive = givenChange.HarmonicMotive };
                //// change.BlockModel = this.MusicalBlock.SourceMusicalBlockModel;  
                harmonicChanges.Add(change);
                for (var b = nextIdx; b < nextIdx + givenChange.HarmonicMotive.Length; b++) {
                    if (b < this.coveredBars.Length) {
                        this.coveredBars[b] = true;
                    }
                }
            }

            return harmonicChanges;
        }

        /// <summary>
        /// Extract Harmonical Motive.
        /// </summary>
        /// <param name="harmonicStream">The harmonic stream.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private IEnumerable<HarmonicChange> ExtractHarmonicalMotive(HarmonicStream harmonicStream)
        { //// cyclomatic complexity 10:15
            var numberOfBars = this.musicalBody.Context.Header.NumberOfBars;
            this.harMotiveNumber = 0;
            this.coveredBars = new BitArray(numberOfBars);
            var harmonicChanges = new List<HarmonicChange>();
            var finished = false;
            HarmonicChange change;
            while (!finished) {
                change = this.FindRepeatedHarmonicMotive(harmonicStream);
                if (change != null) {
                    this.HarmonicModel.AddMotive(change.HarmonicMotive);
                    var changes = this.OccupyStreamByHarmonicMotive(harmonicStream, change);
                    harmonicChanges.AddRange(changes);
                }
                else {
                    finished = true;
                }
            }

            for (var barIdx = 0; barIdx < numberOfBars; barIdx++) {
                if (this.coveredBars[barIdx]) {
                    continue;
                }

                var nextIdx = barIdx;
                while (nextIdx < numberOfBars && !this.coveredBars[nextIdx]) {
                    this.coveredBars[nextIdx] = true;
                    nextIdx++;
                }

                var lengthOfMotive = nextIdx - barIdx;
                this.harMotiveNumber++;
                var harMotive = HarmonicMotive.GetNewHarmonicMotive(harmonicStream.Header, this.harMotiveNumber);
                harmonicStream.WriteToMotive(harMotive, barIdx, lengthOfMotive); //// , 1
                this.HarmonicModel.AddMotive(harMotive);
                change = new HarmonicChange(barIdx + 1) { HarmonicMotive = harMotive };
                //// change.BlockModel = this.MusicalBlock.SourceMusicalBlockModel;  
                harmonicChanges.Add(change);
            }

            return new Collection<HarmonicChange>(harmonicChanges);
        }
        #endregion
    }
}
