// <copyright file="MusicalContent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using JetBrains.Annotations;
    using LargoSharedClasses.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary> A musical content. </summary>
    public class MusicalContent : IMusicalContent
    {
        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="MusicalContent" /> class. </summary>
        public MusicalContent() {
            this.Header = MusicalHeader.GetDefaultMusicalHeader;
        }

        #endregion

        #region Properties

        /// <summary> Gets or sets the header. </summary>
        /// <value> The header. </value>
        public MusicalHeader Header { get; set; }

        /// <summary> Gets or sets the lines. </summary>
        /// <value> The lines. </value>
        public virtual List<IAbstractLine> ContentLines { get; set; }

        /// <summary> Gets or sets bars. </summary>
        /// <value> The bars. </value>
        public virtual List<IAbstractBar> ContentBars { get; set; }

        /// <summary>
        /// Gets the content elements.
        /// </summary>
        /// <value>
        /// The content elements.
        /// </value>
        public IList<MusicalElement> ContentElements {
            get {
                var elems = new List<MusicalElement>();
                foreach (var bar in this.ContentBars) {
                    elems.AddRange(bar.Elements);
                }

                return elems;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [contains music].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [contains music]; otherwise, <c>false</c>.
        /// </value>
        public bool ContainsMusic { get; set; }

        /// <summary> Gets total duration of the block. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public long TotalDuration {
            get {
                long num = this.Header.System.RhythmicOrder * this.Header.NumberOfBars;
                return num;
            }
        }

        /// <summary>
        /// Gets or sets the tonality key.
        /// </summary>
        /// <value>
        /// The tonality key.
        /// </value>
        public TonalityKey TonalityKey { get; set; }

        /// <summary>
        /// Gets or sets the tonality genus.
        /// </summary>
        /// <value>
        /// The tonality genus.
        /// </value>
        public TonalityGenus TonalityGenus { get; set; }

        #endregion

        #region Add content

        /// <summary>
        /// Adds the content line.
        /// </summary>
        /// <param name="lineStatus">The line status.</param>
        /// <returns> Returns value. </returns>
        public virtual IAbstractLine AddContentLine(LineStatus lineStatus) {
            return null;
        }

        /// <summary>
        /// Adds the elements for line.
        /// </summary>
        /// <param name="givenLine">The given new line.</param>
        public void AddElementsForLine(IAbstractLine givenLine) {
            foreach (var bar in this.ContentBars) {
                var element = new MusicalElement {
                    Bar = bar,
                    Line = givenLine,
                    IsLive = true,
                    IsComposed = true,
                    Status = (LineStatus)givenLine.FirstStatus.Clone()
                };

                if (element.Status.LineType == MusicalLineType.None || element.Status.LineType == MusicalLineType.Empty) {
                    element.Status.LineType = MusicalLineType.Melodic;
                }

                element.Status.BarNumber = bar.BarNumber; //// 2017/03
                element.Status.LocalPurpose = LinePurpose.Composed;

                //// 2019/01 element.Status.LocalPurpose = givenLine.Purpose; //// 2018/12
                //// var elements = ((List<MusicalElement>)bar.Elements);
                //// if (elements == null) { return; }
                bar.Elements.Add(element);
            }
        }
        #endregion

        /// <summary>
        /// Prepares the block.
        /// </summary>
        /// <param name="givenHarmonicStream">The given harmonic stream.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public bool SetHarmonicStream(HarmonicStream givenHarmonicStream) {
            //// header.NumberOfBars = givenHarmonicStream.HarmonicBars.Count;
            var firstHarBar = givenHarmonicStream?.HarmonicBars.FirstOrDefault();
            if (firstHarBar == null) {
                return false;
            }

            this.Header.System.RhythmicOrder = firstHarBar.RhythmicStructure.Order;
            foreach (var bar in this.ContentBars) {
                if (bar.BarNumber <= 1 || bar.BarNumber >= givenHarmonicStream.HarmonicBars.Count) {
                    continue;
                }

                var harmonicBar = givenHarmonicStream.HarmonicBars[bar.BarNumber - 1];
                var newHarmonicBar = (HarmonicBar)harmonicBar.Clone();
                newHarmonicBar.BarNumber = bar.BarNumber;
                bar.SetHarmonicBar(newHarmonicBar);
                bool respectHarmonicRhythm = false;
                if (respectHarmonicRhythm) {
                    foreach (var elem in bar.Elements) {
                        elem.Status = (LineStatus)elem.MusicalLine.FirstStatus.Clone();
                        elem.Status.RhythmicStructure = harmonicBar.RhythmicStructure;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Lines the index of line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public int LineIndexOfLine(Guid lineIdent) {
            var line = (from t in this.ContentLines where t.LineIdent == lineIdent select t).FirstOrDefault();
            var idx = this.ContentLines.IndexOf(line);
            return idx;
        }
    }
}
