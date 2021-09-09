// <copyright file="MelodicItem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Text;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Melodic Item.
    /// </summary>
    public sealed class MelodicItem {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicItem"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLineIndex">The given line.</param>
        /// <param name="givenRhythmicStructure">The given rhythmic structure.</param>
        /// <param name="givenMelodicStructure">The given melodic structure.</param>
        public MelodicItem(MusicalBar givenBar, int givenLineIndex, RhythmicStructure givenRhythmicStructure, MelodicStructure givenMelodicStructure) {
            this.MusicalBar = givenBar;
            this.LineIndex = givenLineIndex;
            this.RhythmicStructure = givenRhythmicStructure;
            this.MelodicStructure = givenMelodicStructure;
            this.IsCovered = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is covered.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is covered; otherwise, <c>false</c>.
        /// </value>
        public bool IsCovered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is like motive start.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is like motive start; otherwise, <c>false</c>.
        /// </value>
        public bool IsLikeMotiveStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is like motive end.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is like motive end; otherwise, <c>false</c>.
        /// </value>
        public bool IsLikeMotiveEnd { get; set; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value> Property description. </value>
        public int LineIndex { get; }

        /// <summary>
        /// Gets or sets the musical tones.
        /// </summary>
        /// <value>
        /// The musical tones.
        /// </value>
        public ToneCollection MusicalTones { get; set; }

        /// <summary>
        /// Gets or sets the melodic tones.
        /// </summary>
        /// <value>
        /// The melodic tones.
        /// </value>
        public MusicalToneCollection MelodicTones { get; set; }

        /// <summary>
        /// Gets the musical bar.
        /// </summary>
        /// <value>
        /// The musical bar.
        /// </value>
        public MusicalBar MusicalBar { get; }

        /// <summary>
        /// Gets the rhythmic structure.
        /// </summary>
        /// <value>
        /// The rhythmic structure.
        /// </value>
        public RhythmicStructure RhythmicStructure { get; }

        /// <summary>
        /// Gets the melodic structure.
        /// </summary>
        /// <value>
        /// The melodic structure.
        /// </value>
        public MelodicStructure MelodicStructure { get; }
        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Bar {0}, ", this.MusicalBar.BarNumber);
            s.AppendFormat("Line {0}, ", this.LineIndex);
            s.Append(this.RhythmicStructure == null ? string.Empty : this.RhythmicStructure.ElementSchema + ", ");
            s.Append(this.MelodicStructure == null ? string.Empty : this.MelodicStructure.ElementSchema);
            s.Append(this.IsLikeMotiveStart ? " START" : string.Empty);
            s.Append(this.IsLikeMotiveEnd ? " END" : string.Empty);
            return s.ToString();
        }
        #endregion
    }
}
