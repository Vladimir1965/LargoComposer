// <copyright file="WorkSpace.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Cells;
using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace EditorPanels
{
    /// <summary>
    /// Work Space.
    /// </summary>
    public class WorkSpace {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static WorkSpace singleton = new WorkSpace();

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static WorkSpace Singleton {
            get {
                Contract.Ensures(Contract.Result<WorkSpace>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        #region Private properties
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        private IList<GroupCell> Cells { get; set; }
        #endregion 

        #region Public methods
        /// <summary>
        /// Sets the cells.
        /// </summary>
        /// <param name="givenCells">The given cells.</param>
        public void SetCells(IList<GroupCell> givenCells) {
            this.Cells = givenCells;
        }
        #endregion 

        #region Shift Content of Group Cells
        /// <summary>
        /// Shifts the line content right.
        /// </summary>
        public void ShiftContentRight() {
            /* var lastCell = this.Cells.LastOrDefault();
            if (lastCell == null) {
                return;
            }
            var firstCell = this.Cells.FirstOrDefault();
            */
            GroupCell previousCell = null;
            for (int i = this.Cells.Count - 1; i >= 0; i--) { 
                var cell = this.Cells[i];
                previousCell?.SetInnerCellsStatus(cell.InnerCells);

                previousCell = cell;
            }

            //// firstCell.SetInnerCellsStatus(tmpInner);
            //// firstCell.Point = new MusicalPoint(firstCell.Point.LineIndex, tmpBar);
        }

        /// <summary> Shift content left. </summary>
        public void ShiftContentLeft() {
            /* var lastCell = this.Cells.LastOrDefault();
            if (lastCell == null) {
                return;
            }
            var firstCell = this.Cells.FirstOrDefault();
            */
            GroupCell previousCell = null;
            foreach (var cell in this.Cells) {
                previousCell?.SetInnerCellsStatus(cell.InnerCells);

                previousCell = cell;
            }

            //// firstCell.SetInnerCellsStatus(tmpInner);
            //// firstCell.Point = new MusicalPoint(firstCell.Point.LineIndex, tmpBar);
        }

        /// <summary>
        /// Shifts the line octaves.
        /// </summary>
        /// <param name="numberOfOctaves">The number of octaves.</param>
        public void ShiftOctaves(int numberOfOctaves) { //// int lineIndex,
            foreach (var groupCell in this.Cells) {
                foreach (var cell in groupCell.InnerCells) {
                    cell.Element.ShiftOctave(numberOfOctaves);
                }
            }
        }
        #endregion

        #region Rhythmic changes
        /// <summary>
        /// Sets the simple rhythm.
        /// </summary>
        /// <param name="rhythmicSystem">The rhythmic system.</param>
        public void SetSimpleRhythm(RhythmicSystem rhythmicSystem) {
            const decimal rhythmNumber = 1;
            var rstruct = new RhythmicStructure(rhythmicSystem, rhythmNumber);
            foreach (var groupCell in this.Cells) {
                foreach (var cell in groupCell.InnerCells) {
                    cell.Element.Status.RhythmicStructure = rstruct;
                }
            }
        }

        /// <summary>
        /// Enriches the rhythm.
        /// </summary>
        public void EnrichRhythm() {
            foreach (var groupCell in this.Cells) {
                foreach (var cell in groupCell.InnerCells) {
                    var rstruct = cell.Element.Status.RhythmicStructure;
                    cell.Element.Status.RhythmicStructure = rstruct.HalfEnrichedStructure();
                }
            }
        }

        /// <summary>
        /// Reduces the rhythm.
        /// </summary>
        public void ReduceRhythm() {
            foreach (var groupCell in this.Cells) {
                foreach (var cell in groupCell.InnerCells) {
                    var rstruct = cell.Element.Status.RhythmicStructure;
                    cell.Element.Status.RhythmicStructure = rstruct.HalfReducedStructure();
                }
            }
        }

        /// <summary>
        /// Sets the harmonic rhythm.
        /// </summary>
        /// <param name="rhythmicSystem">The rhythmic system.</param>
        /// <param name="hasToBeStructure">if set to <c>true</c> [has to be structure].</param>
        public void SetHarmonicRhythm(RhythmicSystem rhythmicSystem, bool hasToBeStructure) {
            foreach (var groupCell in this.Cells) {
                foreach (var cell in groupCell.InnerCells) {
                    var bar = cell.Bar as MusicalBar;
                    if (bar != null) {
                        RhythmicStructure rstruct = bar.GetHarmonicRhythm(hasToBeStructure);
                        //// var targetRhythmicStructure = rhyStructure.ConvertToSystem(targetSystem);
                        cell.Element.Status.RhythmicStructure = rstruct;
                    }
                }
            }
        }
        #endregion
    }
}
