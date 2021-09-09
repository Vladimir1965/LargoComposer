// <copyright file="LineSpaceCells.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Cells;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EditorPanels
{
    /// <summary> A base chart master. </summary>
    public partial class LineSpace : FrameworkElement
    {
        #region Public properties - Cells

        /// <summary> Gets or sets the track cells. </summary>
        /// <value> The track cells. </value>
        public List<VoiceCell> VoiceCells { get; set; }

        #endregion

        #region Public methods - Find cell

        /// <summary> Gets voice cell. </summary>
        /// <param name="point"> The point. </param>
        /// <returns> The voice cell. </returns>
        public VoiceCell GetVoiceCell(Point point) {
            var cell = (from c in this.VoiceCells
                        where c.ContainsPoint(point)
                        select c).FirstOrDefault();
            return cell;
        }

        /// <summary> Gets the cell. </summary>
        /// <param name="point"> The point. </param>
        /// <returns> Returns value. </returns>
        public BaseCell GetCellAtMousePoint(Point point) {
            var cell = (from c in this.VoiceCells
                        where c.ContainsPoint(point)
                        select c).FirstOrDefault();
            return cell;
        }

        #endregion

        #region Private methods - Make Cells

        /// <summary>
        /// Resets the cells.
        /// </summary>
        private void ResetCells() {
            this.VoiceCells = new List<VoiceCell>();
        }

        #endregion
    }
}
