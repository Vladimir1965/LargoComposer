// <copyright file="MelodicAnalyzer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Melody
{
    /// <summary>
    /// Melodic Analyzer.
    /// </summary>
    [UsedImplicitly]
    public sealed class MelodicAnalyzer {
        #region Properties
        /// <summary>
        /// Gets the melodic model.
        /// </summary>
        /// <value>
        /// The melodic model.
        /// </value>
        public MelodicModel MelodicModel { get; private set; }

        /// <summary>
        /// Gets the rhythmic model.
        /// </summary>
        /// <value>
        /// The rhythmic model.
        /// </value>
        public RhythmicModel RhythmicModel { get; private set; }
        #endregion

        #region Public methods - Melodic Analyze
        /// <summary>
        /// Analyze Musical Lines.
        /// </summary>
        /// <param name="melodicModel">The melodic model.</param>
        /// <param name="rhythmicModel">The rhythmic model.</param>
        /// <param name="givenBlock">The musical block.</param>
        public void AnalyzeMusicalLines(MelodicModel melodicModel, RhythmicModel rhythmicModel, MusicalBlock givenBlock) {
            Contract.Requires(givenBlock != null);
            //// if (givenBlock == null) { return; }
            this.MelodicModel = melodicModel;
            this.RhythmicModel = rhythmicModel;

            //// Cannot be run as parallel (because of direct addition of motives to entity framework collections?!?) 
            IList<MelodicItem> melodicItems = this.ExtractMelodicItems(givenBlock);
            var melodicStreamAnalyzer = new MelodicStreamAnalyzer(melodicItems);
            var itemGroups = new List<MelodicItemGroup>();
            
            //// Determine motives as group of items
            foreach (var musicalLine in givenBlock.Strip.Lines) {
                while (true) {
                    var items = melodicStreamAnalyzer.GetNextMotiveItems(musicalLine.LineIndex); //// .ToList();   
                    if (items == null || !items.Any()) {
                        break;
                    }

                    var itemGroup = new MelodicItemGroup(items, musicalLine);
                    itemGroups.Add(itemGroup);
                }
            }

            //// Main algorithm to determine motivic classes and their instances
            this.RhythmicModel.AppendRhythmicMotives(itemGroups);
            this.MelodicModel.AppendMelodicMotives(itemGroups);
        }

        /// <summary>
        /// Extracts the melodic items.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <returns>
        /// Returns object.
        /// </returns>
        public IList<MelodicItem> ExtractMelodicItems(MusicalBlock givenBlock) {
            List<MelodicItem> items = new List<MelodicItem>();
            //// MusicalTone lastMelodicTone = null;
            for (int lineIndex = 0; lineIndex < givenBlock.Header.NumberOfLines; lineIndex++) {
                foreach (var bar in givenBlock.Body.Bars) {
                    var element = bar.Elements[lineIndex];
                    if (element.Line == null || !element.Status.HasContent) {
                        continue;
                    }

                    //// Rhythmical and melodical structure in bar
                    //// Rhythmical structure in bar
                    RhythmicStructure rstruct = element.Status.RhythmicStructure;
                    if (rstruct == null) {
                        continue;
                    }

                    //// Melodic structure in bar
                    MelodicStructure mstruct = null;
                    var isMelodic = element.Status.IsMelodic;
                    if (isMelodic) {
                        mstruct = element.Status.MelodicStructure;
                    }

                    var musicalTonesInBar = element.Tones;
                    MusicalToneCollection melodicTonesInBar = element.SingleMelodicTones();
                    
                    var melodicItem = new MelodicItem(bar, element.Line.LineIndex, rstruct, mstruct) {
                        MusicalTones = musicalTonesInBar,
                        MelodicTones = melodicTonesInBar
                    };

                    items.Add(melodicItem);
                }
            }

            return items;
        }

        #endregion
    }
}
