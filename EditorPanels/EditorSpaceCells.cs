// <copyright file="EditorSpaceCells.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using EditorPanels.Cells;
using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace EditorPanels
{
    /// <summary> A base chart master. </summary>
    public partial class EditorSpace : FrameworkElement
    {
        /// <summary>
        /// The corner cell
        /// </summary>
        private CornerCell cornerCell;

        #region Public properties - Cells

        /// <summary> Gets or sets the bar cells. </summary>
        /// <value> The bar cells. </value>
        public List<BarCell> BarCells { get; set; }

        /// <summary> Gets or sets the track cells. </summary>
        /// <value> The track cells. </value>
        public List<LineCell> LineCells { get; set; }

        /// <summary> Gets or sets the editor tracks. </summary>
        /// <value> The editor tracks. </value>
        public List<EditorLine> EditorLines { get; set; }

        /// <summary> Gets or sets all cells. </summary>
        /// <value> all cells. </value>
        public List<BaseCell> AllCells { get; set; }

        /// <summary> Gets or sets the voice cells. </summary>
        /// <value> The voice cells. </value>
        public List<VoiceCell> VoiceCells { get; set; }

        /// <summary>
        /// Gets the content cells.
        /// </summary>
        /// <value>
        /// The content cells.
        /// </value>
        public List<ContentCell> ContentCells {
            get {
                var cells = new List<ContentCell>();
                foreach (var editorLine in this.EditorLines) {
                    cells.AddRange(editorLine.ContentCells);
                }

                return cells;
            }
        }

        /// <summary>
        /// Gets the selected content cells bar sorted.
        /// </summary>
        /// <value>
        /// The selected content cells bar sorted.
        /// </value>
        public List<ContentCell> SelectedContentCellsBarSorted {
            get {
                var contentCells = this.ContentCells;
                var cells = (from cell in contentCells where cell.IsSelected orderby cell.BarIndex select cell).ToList();
                return cells;
            }
        }

        /// <summary>
        /// Gets the selected bar cells.
        /// </summary>
        /// <value>
        /// The selected bar cells.
        /// </value>
        public List<BarCell> SelectedBarCells {
            get {
                var cells = (from cell in this.BarCells where cell.IsSelected select cell).ToList();
                return cells;
            }
        }

        /// <summary>
        /// Gets the selected line cells.
        /// </summary>
        /// <value>
        /// The selected line cells.
        /// </value>
        public List<LineCell> SelectedLineCells {
            get {
                var cells = (from cell in this.LineCells where cell.IsSelected select cell).ToList();
                return cells;
            }
        }
        #endregion

        #region Public methods - Make cells

        /// <summary>
        /// Resets the bar cells text.
        /// </summary>
        public void ResetBarCellsText() {
            foreach (var cell in this.BarCells) {
                cell.SetFormattedText(null);
            }
        }        
        
        /// <summary>
        /// Resets the line cells text.
        /// </summary>
        [UsedImplicitly]
        public void ResetLineCellsText() {
            foreach (var cell in this.LineCells) {
                cell.SetFormattedText(null);
            }
        }

        /// <summary>
        /// Resets the group cells text.
        /// </summary>
        [UsedImplicitly]
        public void ResetGroupCellsText() {
            foreach (var cell in this.GroupCells) {
                cell.SetFormattedText(null);
            }
        }

        /// <summary> Loads the bars. </summary>
        /// <param name="musicalBars"> The musical plan. </param>
        public void MakeBarCells(List<IAbstractBar> musicalBars) {
            this.NumberOfBars = musicalBars.Count;
            foreach (var bar in musicalBars) {
                //// var cell = this.BarCells[bar.BarIndex];
                var cell = new BarCell(this, bar) {
                    PenBrush = Brushes.Black,
                    ContentBrush = Brushes.WhiteSmoke,
                    Width = SeedSize.CurrentWidth - SeedSize.BasicMargin,
                    Height = (2 * SeedSize.CurrentHeight) - SeedSize.BasicMargin
                };

                if (bar.HarmonicStructure != null) {
                    cell.HarmonicStructure = bar.HarmonicStructure;
                }

                this.BarCells.Add(cell);
            }
        }

        /// <summary>
        /// Makes the content of the cells from.
        /// </summary>
        /// <param name="givenContent">Content of the given.</param>
        /// <param name="givenContentType">Type of the given content.</param>
        /// <param name="givenIsMusic">if set to <c>true</c> [given is music].</param>
        public void MakeCellsFromContent(IMusicalContent givenContent, EditorContent givenContentType, bool givenIsMusic) {
            this.ResetCells();
            this.cornerCell = new CornerCell(this) {
                PenBrush = Brushes.Black,
                ContentBrush = Brushes.WhiteSmoke,
                LineIndex = -1,
                BarIndex = -1,
            };

            this.MakeBarCells(givenContent.ContentBars);
            this.MakeLineCells(givenContent.ContentLines);
            this.MakeContentCells(givenContent.ContentLines, givenContent.ContentElements);

            //// this.LoadVoices(givenContent);
            this.PrepareGroupCells(givenContentType, givenIsMusic);
            this.RebuildAllCells();
        }

        /// <summary>
        /// Rebuilds all cells.
        /// </summary>
        public void RebuildAllCells() {
            //// Put all cells to one list.
            this.AllCells = new List<BaseCell> {
                this.cornerCell
            };
            this.AllCells.AddRange(this.BarCells);
            this.AllCells.AddRange(this.LineCells);

            foreach (var line in this.EditorLines) {
                this.AllCells.AddRange(line.GroupCells);
            }
        }

        /// <summary>
        /// Reflect the plan.
        /// </summary>
        public void ReflectPlan() {
            foreach (var editorLine in this.EditorLines) {
                foreach (var cell in editorLine.ContentCells) {
                    var status = cell.Element.Status;
                    status.RhythmicStructure = cell.Element.GetFaceRhythmicStructure;
                    //// status.LocalPurpose = LinePurpose.Composed;
                }
            }
        }

        /// <summary>
        /// Reflect the music.
        /// </summary>
        public void ReflectMusic() {
            foreach (var editorLine in this.EditorLines) {
                foreach (var cell in editorLine.ContentCells) {
                    var status = cell.Element.Status;
                    cell.Element.Status.SetRhythmicFaceFromStructure(status.RhythmicStructure);
                }
            }
        }
        #endregion

        #region Public methods - Find cell

        /// <summary>
        /// Gets the group cell.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns> Returns value. </returns>
        public GroupCell GetGroupCell(int lineIndex, int cellIndex) {
            GroupCell cell = null;
            if (lineIndex >= 0 && lineIndex < this.EditorLines.Count) {
                var editorLine = this.EditorLines[lineIndex];
                if (cellIndex >= 0 && cellIndex < editorLine.ContentCells.Count) {
                    cell = editorLine.GroupCells[cellIndex];
                }
            }

            return cell;
        }

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
            var cell = (from c in this.AllCells
                        where c.ContainsPoint(point)
                        select c).FirstOrDefault();
            //// Check: if (cell != null) { cell.ContentBrush = Brushes.Yellow; }

            return cell;
        }

        /// <summary>
        /// Gets the editor track.
        /// </summary>
        /// <param name="lineIdent">The track identifier.</param>
        /// <returns> Returns value. </returns>
        public EditorLine GetEditorLine(Guid lineIdent) {
            var editorLine = (from t in this.EditorLines
                               where t.Line.LineIdent == lineIdent
                               select t).FirstOrDefault();

            return editorLine;
        }

        #endregion

        #region Public methods - Change selection

        /// <summary>
        /// Clears the selection.
        /// </summary>
        public void ClearSelection() {
            this.IsSelectionInProgress = false;
            foreach (var cell in this.ContentCells) {
                if (cell.IsSelected) {
                    cell.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Selects the line.
        /// </summary>
        /// <param name="lineIdent">The track identifier.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        /// <param name="inversion">if set to <c>true</c> [inversion].</param>
        public void SelectCellsOfLine(Guid lineIdent, bool selected, bool inversion) {
            var editorLine = this.GetEditorLine(lineIdent);
            foreach (var cell in editorLine.ContentCells) {
                bool flag;
                if (inversion) {
                    flag = !cell.IsSelected;
                }
                else {
                    flag = selected;
                }

                cell.IsSelected = flag;
            }
        }

        /// <summary>
        /// Selected cell changed.
        /// </summary>
        /// <param name="givenCell">The given cell.</param>
        public void SelectedCellChanged(BaseCell givenCell) {
            if (givenCell == null) {
                return;
            }

            this.SelectedCell = givenCell;

            if (givenCell.GetType() == typeof(BarCell)) {
                EditorInspector.Singleton.EnablePanel(InspectorType.Bar);

                if (givenCell is BarCell barCell) {
                    var mbar = barCell.Bar;
                    EditorEventSender.Singleton.SendEditorChangedEvent(mbar);
                    EditorInspector.Singleton.InspectBar.ResetIdentifiers();
                    EditorInspector.Singleton.InspectBar.AddMainIdentifiers(mbar.Identifiers);
                }
            }

            if (givenCell.GetType() == typeof(LineCell)) {
                EditorInspector.Singleton.EnablePanel(InspectorType.Line);
                LineCell lineCell = givenCell as LineCell;
                if (lineCell?.Line != null) {
                    EditorEventSender.Singleton.SendEditorChangedEvent(lineCell.Line);
                    EditorInspector.Singleton.InspectLine.ResetIdentifiers();
                    EditorInspector.Singleton.InspectLine.AddMainIdentifiers(lineCell.Line.Identifiers);
                }
            }

            /*
            if (givenCell.GetType() == typeof(ContentCell)) {
                EditorInspector.Singleton.EnablePanel(InspectorType.Element);
                ContentCell contentCell = givenCell as ContentCell;
                if (contentCell?.Element != null) {
                    EditorEventSender.Singleton.SendEditorChangedEvent(contentCell.Element);
                    EditorInspector.Singleton.InspectElement.ResetIdentifiers();
                    EditorInspector.Singleton.InspectElement.AddMainIdentifiers(contentCell.Identifiers);
                }
            } */

            if (givenCell.GetType() == typeof(GroupCell)) {
                EditorInspector.Singleton.EnablePanel(InspectorType.Element);
                GroupCell groupCell = givenCell as GroupCell;
                if (groupCell?.FirstElement != null) {
                    EditorEventSender.Singleton.SendEditorChangedEvent(groupCell.FirstElement);
                    EditorInspector.Singleton.InspectElement.ResetIdentifiers();
                    EditorInspector.Singleton.InspectElement.AddMainIdentifiers(groupCell.FirstElement.Identifiers);
                }
            }
        }
        #endregion

        #region Private methods - Make Cells

        /// <summary>
        /// Resets the cells.
        /// </summary>
        private void ResetCells() {
            this.BarCells = new List<BarCell>();
            this.LineCells = new List<LineCell>();
            //// this.EditorCells = new List<ContentCell>();
            this.AllCells = new List<BaseCell>();
            this.VoiceCells = new List<VoiceCell>();
            this.EditorLines = new List<EditorLine>();
        }

        /// <summary>
        /// Loads the lines.
        /// </summary>
        /// <param name="contentLines">The content lines.</param>
        private void MakeLineCells(List<IAbstractLine> contentLines) {
            //// this.Lines = contentLines;

            this.NumberOfLines = contentLines.Count;
            foreach (var line in contentLines) {
                if (line.Purpose == LinePurpose.Mute && !this.ShowMutedLines) {
                    continue;
                }

                //// var cell = this.LineCells[line.LineIndex];
                var cell = new LineCell(this, line) {
                    PenBrush = Brushes.Black,
                    ContentBrush = Brushes.WhiteSmoke,
                    Width = (2 * SeedSize.CurrentWidth) - SeedSize.BasicMargin,
                    Height = SeedSize.CurrentHeight - SeedSize.BasicMargin
                };

                this.LineCells.Add(cell);
            }
        }

        /// <summary>
        /// Loads the elements.
        /// </summary>
        /// <param name="contentLines">The content lines.</param>
        /// <param name="givenElements">The given elements.</param>
        private void MakeContentCells(List<IAbstractLine> contentLines, IList<MusicalElement> givenElements) {
            this.EditorLines = new List<EditorLine>();
            foreach (var line in contentLines) {
                if (line.Purpose == LinePurpose.Mute && !this.ShowMutedLines) {
                    continue;
                }

                var editorLine = new EditorLine(line);
                var lineElements = (from element in givenElements
                                    where element.Line.LineIdent == line.LineIdent
                                    select element).ToList();

                //// int cellIndex = 0;
                foreach (var element in lineElements) {
                    var cell = new ContentCell(this, element);
                    editorLine.ContentCells.Add(cell);
                    //// cellIndex++;
                }

                foreach (var cell in editorLine.ContentCells) {
                    cell.PenBrush = Brushes.DarkGray;
                    cell.ContentBrush = Brushes.GhostWhite;
                    cell.Width = SeedSize.CurrentWidth - SeedSize.BasicMargin;
                    cell.Height = SeedSize.CurrentHeight - SeedSize.BasicMargin;
                }

                this.EditorLines.Add(editorLine);
            }
        }

            #endregion
        }
    }
