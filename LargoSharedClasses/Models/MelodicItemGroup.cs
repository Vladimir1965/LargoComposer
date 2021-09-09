// <copyright file="MelodicItemGroup.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Melodic Item Group.
    /// </summary>
    public class MelodicItemGroup {
        #region Fields
        /// <summary>
        /// Melodic Items.
        /// </summary>
        private List<MelodicItem> items;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicItemGroup"/> class.
        /// </summary>
        /// <param name="givenItems">The given items.</param>
        /// <param name="givenLine">The given line.</param>
        public MelodicItemGroup(IList<MelodicItem> givenItems, MusicalLine givenLine) {
            Contract.Requires(givenItems != null);

            this.Items = givenItems as List<MelodicItem>;
            this.Length = givenItems.Count;

            var rs = new StringBuilder();
            var ms = new StringBuilder();
            foreach (var item in givenItems) {
                if (item.RhythmicStructure != null) {
                    rs.Append(item.RhythmicStructure.GetStructuralCode);
                }

                rs.Append(";");

                if (item.MelodicStructure != null) {
                    ms.Append(item.MelodicStructure.GetStructuralCode);
                }

                ms.Append(";");
            }

            this.RhythmicIdentifier = rs.ToString(); //// Identifier of the rhythmic motive
            this.MelodicIdentifier = ms.ToString();  //// Identifier of the melodic motive

            this.MusicalLine = givenLine;

            if (this.Items == null) {
                return;
            }

            var firstItem = this.Items.First();
            this.FirstBarNumber = firstItem.MusicalBar.BarNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets the rhythmic identifier.
        /// </summary>
        /// <value>
        /// The rhythmic identifier.
        /// </value>
        public string RhythmicIdentifier { get; }

        /// <summary>
        /// Gets the melodic identifier.
        /// </summary>
        /// <value>
        /// The melodic identifier.
        /// </value>
        public string MelodicIdentifier { get; }

        /// <summary>
        /// Gets or sets the musical line.
        /// </summary>
        /// <value>
        /// The musical line.
        /// </value>
        public MusicalLine MusicalLine { get; set; }

        /// <summary>
        /// Gets the first bar number.
        /// </summary>
        /// <value>
        /// The first bar number.
        /// </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private int FirstBarNumber { get; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        private List<MelodicItem> Items {
            get {
                Contract.Ensures(Contract.Result<List<MelodicItem>>() != null);
                if (this.items == null) {
                    throw new InvalidOperationException("No melodic items.");
                }

                return this.items;
            }

            set => this.items = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }
        #endregion

        #region Public static methods
        #endregion

        #region Methods
        /// <summary>
        /// Rhythmic change.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalArea GetArea() {
            var firstItem = this.Items.First();
            var length = this.Length;
            var p0 = MusicalPoint.GetPoint(this.MusicalLine.LineIndex, firstItem.MusicalBar.BarNumber);
            var p1 = MusicalPoint.GetPoint(this.MusicalLine.LineIndex, firstItem.MusicalBar.BarNumber + length);
            var area = new MusicalArea(p0, p1);

            return area;
        }

        /// <summary>
        /// Rhythmic motive.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <param name="givenName">Name of the given.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicMotive RhythmicMotive(int givenNumber, string givenName) {
            var firstItem = this.Items.First();
            var rhythmicStructures = new RhythmicStructureCollection();
            foreach (var item in this.Items) {
                if (item.RhythmicStructure == null) {
                    continue;
                }

                rhythmicStructures.Add(item.RhythmicStructure);
            }

            var rhythmicMotive = new RhythmicMotive(givenName, rhythmicStructures) {
                FirstBarNumber = firstItem.MusicalBar.BarNumber,
                Number = givenNumber
            };

            return rhythmicMotive;
        }

        /// <summary>
        /// Melodic motive.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <param name="givenName">Name of the given.</param>
        /// <returns> Returns value. </returns>
        public MelodicMotive MelodicMotive(int givenNumber, string givenName) {
            var firstItem = this.Items.First();
            var melodicStructures = new MelodicStructureCollection();
            foreach (var item in this.Items) {
                if (item.MelodicStructure == null) {
                    continue;
                }

                melodicStructures.Add(item.MelodicStructure);
            }

            var melodicMotive = new MelodicMotive(givenName, melodicStructures) {
                FirstBarNumber = firstItem.MusicalBar.BarNumber,
                Number = givenNumber,
                MelodicFunction = MusicalToneCollection.GuessMelodicType(this.MusicalLine.FirstStatus.BandType, true),
                Octave = this.MusicalLine.FirstStatus.Octave
            };

            return melodicMotive;
        }
        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var item in this.Items) {
                sb.Append(item);
                sb.Append("| ");
            }

            return sb.ToString();
        }
        #endregion
    }
}
