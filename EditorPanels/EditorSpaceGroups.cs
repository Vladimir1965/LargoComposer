// <copyright file="EditorSpaceGroups.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Cells;
using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Linq;

namespace EditorPanels
{
    /// <summary>
    /// Editor Space.
    /// </summary>
    /// <seealso cref="System.Windows.FrameworkElement" />
    /// <seealso cref="LargoSharedClasses.Interfaces.IEditorSpace" />
    public partial class EditorSpace
    {
        #region Public properties - Group Cells

        /// <summary>
        /// Gets the group cells.
        /// </summary>
        /// <value>
        /// The group cells.
        /// </value>
        public List<GroupCell> GroupCells {
            get {
                var cells = new List<GroupCell>();
                foreach (var editorLine in this.EditorLines) {
                    cells.AddRange(editorLine.GroupCells);
                }

                return cells;
            }
        }

        /// <summary>
        /// Gets the selected group cells.
        /// </summary>
        /// <value>
        /// The selected group cells.
        /// </value>
        public List<GroupCell> SelectedGroupCells {
            get {
                var cells = new List<GroupCell>();
                foreach (var editorLine in this.EditorLines) {
                    var gcs = from gc in editorLine.GroupCells where gc.IsSelected select gc;
                    cells.AddRange(gcs);
                }

                return cells;
            }
        }
        #endregion

        #region Public methods - Group Cells

        /// <summary>
        /// Prepares the cells.
        /// </summary>
        /// <param name="givenContentType">Type of the given content.</param>
        /// <param name="givenIsMusic">if set to <c>true</c> [given is music].</param>
        public void PrepareGroupCells(EditorContent givenContentType, bool givenIsMusic) {
            this.ContentType = givenContentType;
            foreach (var editorLine in this.EditorLines) {
                if (editorLine.Line.Purpose == LinePurpose.Mute && !this.ShowMutedLines) {
                    continue;
                }

                GroupCell lastGroupCell = null;
                editorLine.GroupCells = new List<GroupCell>();
                foreach (var cell in editorLine.ContentCells) {
                    cell.Element.PrepareContent(this.ContentType);

                    if (lastGroupCell?.FirstElement == null) {
                        lastGroupCell = new GroupCell(this, cell);
                        continue;
                    }

                    if (givenIsMusic && cell.Element != null && lastGroupCell.FirstElement.IsCompatibleWith(cell.Element)) {
                        lastGroupCell.AddInnerCell(cell);
                    }
                    else {
                        editorLine.GroupCells.Add(lastGroupCell);
                        lastGroupCell = new GroupCell(this, cell);
                    }
                }

                if (lastGroupCell != null) {
                    editorLine.GroupCells.Add(lastGroupCell);
                }
            }

            this.RebuildAllCells();
        }

        /// <summary>
        /// Selects the next.
        /// </summary>
        /// <param name="up">Motion up.</param>
        /// <param name="right">Motion right.</param>
        public void SelectNext(int up, int right) { //// bool select
            var c = this.SelectedCell;
            if (c == null) {
                return;
            }

            var p = c.Point;
            //// if (p.BarNumber == 0) { return; }

            var nextLineIndex = p.LineIndex - up;
            if (!(c is GroupCell gc)) {
                return;
            }

            var editorLine = this.EditorLines[p.LineIndex];
            var cellIndex = editorLine.GroupCells.IndexOf(gc);   //// c.CellIndex + right;
            var nextCellIndex = cellIndex + right;
            BaseCell cell = null;
            if (nextLineIndex >= 0 && nextLineIndex < this.LineCells.Count) {
                if (nextCellIndex >= 0) {
                    cell = this.GetGroupCell(nextLineIndex, nextCellIndex);
                    //// cell = this.GetContentCell(nextLineIndex, nextCellIndex);
                }
                else {
                    if (nextLineIndex >= 0 && nextLineIndex < this.LineCells.Count) {
                        cell = this.LineCells[nextLineIndex];
                    }
                }
            }
            else {
                if (nextCellIndex >= 0 && nextCellIndex < this.BarCells.Count) {
                    cell = this.BarCells[nextCellIndex];
                }
            }

            if (cell != null) {
                c.IsHighlighted = false;
                cell.IsHighlighted = true;
                this.SelectedCellChanged(cell);
                //// this.SetHighlightedCell(cell);
                this.InvalidateVisual();
            }
        }

        #endregion
    }
}
