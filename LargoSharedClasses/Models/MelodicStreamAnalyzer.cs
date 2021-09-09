// <copyright file="MelodicStreamAnalyzer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MelodicStreamAnalyzer {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicStreamAnalyzer" /> class.
        /// </summary>
        /// <param name="givenMelodicItems">The given melodic items.</param>
        public MelodicStreamAnalyzer(IList<MelodicItem> givenMelodicItems) {
            Contract.Requires(givenMelodicItems != null);

            this.MelodicItems = givenMelodicItems;
            this.EstimateMotiveMarkers();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the melodic items.
        /// </summary>
        /// <value>
        /// The melodic items.
        /// </value>
        private IList<MelodicItem> MelodicItems { get; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return $"MelodicStreamAnalyzer (Number of items {this.MelodicItems.Count})";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the next motive items.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        /// <returns> Returns value. </returns>
        public IList<MelodicItem> GetNextMotiveItems(int lineIndex) {
            if (this.MelodicItems == null) {
                return null;
            }

            var motiveItems = new List<MelodicItem>();
            var motiveStarted = false;
            var lineItems = this.MelodicItems.Where(it => it.LineIndex == lineIndex && !it.IsCovered).ToList();
            foreach (var item in lineItems) {
                if (item.RhythmicStructure == null || item.RhythmicStructure.ToneLevel == 0) {  //// 2018/12 above: || item.IsCovered
                    continue;
                }

                if (item.IsLikeMotiveStart) {
                    motiveStarted = true;
                }

                if (motiveStarted) {
                    item.IsCovered = true;
                    motiveItems.Add(item);

                    if (item.IsLikeMotiveEnd) {
                        break;
                    }
                }
            }

            return motiveItems;
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Prepares the line breaks.
        /// </summary>
        private void EstimateMotiveMarkers() { //// Start and ends
            MelodicItem previousItem = null;
            if (this.MelodicItems == null) {
                return;
            }

            foreach (var melodicItem in this.MelodicItems) {
                if (previousItem != null && melodicItem.LineIndex != previousItem.LineIndex) {
                    previousItem = null; //// Next Line ...
                }

                if (melodicItem.RhythmicStructure == null) {
                    continue;
                }

                if (previousItem != null && melodicItem.MusicalBar.BarNumber - previousItem.MusicalBar.BarNumber > 1)
                {
                    previousItem.IsLikeMotiveEnd = true;
                    melodicItem.IsLikeMotiveStart = true;
                    previousItem = melodicItem;
                    continue;
                }

                //// var nextBar = melodicItem.MusicalBar.NextBar; var motiveEnd = musicalBar.IsLastInHarmonicMotive;
                //// var motiveStart = melodicItem.MusicalBar.IsFirstInHarmonicMotive && melodicItem.MusicalTones.HasAnySoundingTone;
                //// if (!motiveStart) {
                if (melodicItem.MelodicTones == null || !melodicItem.MelodicTones.Any()) { //// 201509
                    //// 2016/04 Rhythmic lïnes!? 
                    melodicItem.IsLikeMotiveStart = melodicItem.MusicalTones != null && melodicItem.MusicalTones.Any();
                    previousItem = melodicItem;
                    continue;
                }

                if (previousItem != null) {
                    if (previousItem.MelodicTones == null || !previousItem.MelodicTones.Any()) { //// 201509
                        previousItem = melodicItem;
                        continue;
                    }

                    if (!previousItem.IsLikeMotiveEnd) {
                        previousItem.IsLikeMotiveEnd = previousItem.MelodicTones.Any() && !melodicItem.MelodicTones.Any();
                    }

                    if (!melodicItem.IsLikeMotiveEnd && melodicItem.MelodicTones.Any()) {
                        melodicItem.IsLikeMotiveEnd = ((melodicItem.RhythmicStructure.ToneLevel < previousItem.RhythmicStructure.ToneLevel)
                                                            && !melodicItem.RhythmicStructure.StartsWithFormalRest)
                            || melodicItem.RhythmicStructure.EndsWithFormalRest;
                    }

                    melodicItem.IsLikeMotiveStart = (previousItem.IsLikeMotiveEnd || !previousItem.MelodicTones.Any())
                                                    && melodicItem.MelodicTones.Any();

                    if (!melodicItem.IsLikeMotiveStart && melodicItem.RhythmicStructure != null) {
                        melodicItem.IsLikeMotiveStart = melodicItem.RhythmicStructure.StartsWithFormalRest;
                    }
                }
                else {
                    melodicItem.IsLikeMotiveStart = true;
                }

                //// The assumed motive continues.
                if (previousItem != null && previousItem.IsLikeMotiveEnd && melodicItem.IsLikeMotiveStart && melodicItem.IsLikeMotiveEnd) {
                    previousItem.IsLikeMotiveEnd = false;
                    melodicItem.IsLikeMotiveStart = false;
                }

                previousItem = melodicItem;
            }
        }

        #endregion
    }
}
