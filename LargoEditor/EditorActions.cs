// <copyright file="EditorActions.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels;
using EditorPanels.Cells;
using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LargoEditor
{
    /// <summary>
    /// Editor Actions.
    /// </summary>
    public class EditorActions
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorActions"/> class.
        /// </summary>
        /// <param name="givenEditorWindow">The given editor window.</param>
        public EditorActions(EditorWindow givenEditorWindow) {
            this.Editor = givenEditorWindow;
            this.EditorSpace = this.Editor.EditorSpace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public EditorWindow Editor { get; set; }

        /// <summary>
        /// Gets the chart master.
        /// </summary>
        /// <value>
        /// The chart master.
        /// </value>
        private EditorSpace EditorSpace { get; }
        #endregion

        #region Export
        /// <summary>
        /// Exports file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">Tag exception.</exception>
        public void ExportFile(object sender, RoutedEventArgs e) {
            var intTag = int.Parse((sender as MenuItem)?.Tag as string ?? throw new InvalidOperationException());
            MusicalSourceType sourceType = (MusicalSourceType)intTag;

            MusicalBlock block = this.Editor.Block;
            if (block == null) {
                return;
            }

            var bundle = MusicalBundle.GetEnvelopeOfBlock(block, block.Header.FullName ?? "Unnamed");
            var port = PortAbstract.CreatePort(sourceType);
            port.SaveBundle(bundle);
            MessageBox.Show(string.Format("File saved!\n\n{0}", port.DestinationFilePath), SettingsApplication.ApplicationName);
        }
        #endregion

        #region Private methods - Shift selection

        /// <summary>
        /// Shifts content right.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ShiftContentRight(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.ShiftContentRight();
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Shifts content left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ShiftContentLeft(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.ShiftContentLeft();
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Moves octave above.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void ShiftOctaveAbove(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.ShiftOctaves(1);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Moves octave below.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void ShiftOctaveBelow(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.ShiftOctaves(-1);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        #endregion

        #region Public - Rhythmic changes
        /// <summary>
        /// Sets simple rhythm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void SetSimpleRhythm(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.SetSimpleRhythm(this.Editor.Block.Header.System.RhythmicSystem);
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Enriches the rhythm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void EnrichRhythm(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.EnrichRhythm();
            //// this.EditorSpace.ResetGroupCellsText();
            //// this.EditorSpace.SelectedCellChanged(this.currentCell);
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Reduces the rhythm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ReduceRhythm(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            WorkSpace.Singleton.ReduceRhythm();
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Sets harmonic rhythm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void SetHarmonicRhythm(object sender, RoutedEventArgs e) {
            WorkSpace.Singleton.SetCells(this.EditorSpace.SelectedGroupCells);
            byte variant = byte.Parse(((MenuItem)sender).Tag as string ?? throw new InvalidOperationException());
            WorkSpace.Singleton.SetHarmonicRhythm(this.Editor.Block.Header.System.RhythmicSystem, variant == 0);
            this.EditorSpace.ResetBarCellsText();
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        #endregion

        #region Public - Delete line
        /// <summary>
        /// Deletes the editor line.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void DeleteEditorLine(object sender, RoutedEventArgs e) {
            var line = EditorInspector.Singleton.InspectElement.Line;
            if (line == null) {
                return;
            }

            var block = this.Editor.Block;
            block.DeleteLine(line.LineIdent);

            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            //// this.FinishChanges();

            e.Handled = true;
        }
        #endregion

        #region Private methods - Reduce selection
        /// <summary>
        /// Eliminates the rhythmic dependencies.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [UsedImplicitly]
        public void EliminateRhythmicDependencies(object sender, RoutedEventArgs e) {
            var list = this.Editor.Block.Body.AllElements;
            var master = new ElementMaster(list);
            master.EliminateRhythmicDependencies();
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Eliminates the melodic dependencies.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        [UsedImplicitly]
        public void EliminateMelodicDependencies(object sender, RoutedEventArgs e) {
            var list = this.Editor.Block.Body.AllElements;
            var master = new ElementMaster(list);
            master.EliminateMelodicDependencies();
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }

        /// <summary>
        /// Deletes the unused lines.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void DeleteUnusedLines(object sender, RoutedEventArgs e) {
            var block = this.Editor.Block;
            var listToDelete = new List<Guid>();
            foreach (var track in block.Strip.Lines) {
                var list = block.Body.ElementsOfLine(track.LineIdent);
                var master = new ElementMaster(list);
                var havePurpose = master.HaveAnyPurpose();
                if (!havePurpose) {
                    listToDelete.Add(track.LineIdent);
                }
            }

            if (listToDelete.Count == 0) {
                return;
            }

            for (int i = listToDelete.Count - 1; i >= 0; i--) {
                block.DeleteLine(listToDelete[i]);
            }

            //// this.EditorGridMaster.NumberOfLines = block.Header.NumberOfLines;
            this.Editor.ReloadCanvas();
            e.Handled = true;
        }
        #endregion

        #region Public methods - Mark tracks
        /// <summary>
        /// Marks the editor purpose.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void MarkEditorPurpose(object sender, RoutedEventArgs e) {
            var menuItem = (MenuItem)sender;
            var intTag = (int)menuItem.Tag;
            var purpose = (intTag == 2) ? LinePurpose.Composed : (intTag == 1) ? LinePurpose.Fixed : LinePurpose.Mute;
            foreach (var line in this.EditorSpace.MusicalContent.ContentLines) {
                line.Purpose = purpose;
            }

            this.Editor.Block.Body.SetPurposeToAllElements(purpose);
            //// this.EditorSpace.SetPurposeToEditorCells(purpose);
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
        }

        /// <summary>
        /// Marks the tracks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void MarkEditorLinePurpose(object sender, RoutedEventArgs e) {
            var line = this.CurrentLine();
            if (line == null) {
                return;
            }

            //// var editorLine = this.EditorSpace.GetEditorLine(line.LineIdent);
            var menuItem = (MenuItem)sender;
            var intTag = (int)menuItem.Tag;
            var purpose = (intTag == 2) ? LinePurpose.Composed : (intTag == 1) ? LinePurpose.Fixed : LinePurpose.Mute;
            line.Purpose = purpose;
            var elements = this.Editor.Block.Body.ElementsOfLine(line.LineIdent);
            MusicalBody.SetPurposeToElements(purpose, elements);
            //// this.EditorSpace.SetPurposeToEditorLineCells(editorLine, purpose);
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
        }

        /// <summary>
        /// Marks the line cells.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <param name="givenModule">The given module.</param>
        /// <param name="givenRest">The given rest.</param>
        /// <param name="flagSelected">if set to <c>true</c> [flag selected].</param>
        public void MarkLineCells(Guid lineIdent, byte givenModule, byte givenRest, bool flagSelected) {
            var editorLine = this.EditorSpace.GetEditorLine(lineIdent);
            foreach (var cell in editorLine.GroupCells) {
                if (givenModule == 1 || cell.BarIndex % givenModule == givenRest) {
                    cell.IsSelected = flagSelected;
                }
            }

            this.Editor.ReloadCanvas();
        }

        /// <summary>
        /// Selects the line cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SelectLineCells(object sender, RoutedEventArgs e) {
            var line = this.CurrentLine();
            if (line == null) {
                return;
            }

            this.MarkLineCells(line.LineIdent, 1, 0, true);
        }

        /// <summary>
        /// Selects the odd line cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SelectOddLineCells(object sender, RoutedEventArgs e) {
            var line = this.CurrentLine();
            if (line == null) {
                return;
            }

            this.MarkLineCells(line.LineIdent, 2, 0, true);
        }

        /// <summary>
        /// Selects the even line cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SelectEvenLineCells(object sender, RoutedEventArgs e) {
            var line = this.CurrentLine();
            if (line == null) {
                return;
            }

            this.MarkLineCells(line.LineIdent, 2, 1, true);
        }

        /// <summary>
        /// Unselects the line cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void UnselectLineCells(object sender, RoutedEventArgs e) {
            var line = this.CurrentLine();
            if (line == null) {
                return;
            }

            this.MarkLineCells(line.LineIdent, 1, 0, true);
        }

        /// <summary>
        /// Selects all cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SelectAllCells(object sender, RoutedEventArgs e) {
            foreach (var cell in this.EditorSpace.GroupCells) {
                cell.IsSelected = true;
            }

            this.Editor.ReloadCanvas();
        }

        /// <summary>
        /// Unselects all cells.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void UnselectAllCells(object sender, RoutedEventArgs e) {
            foreach (var cell in this.EditorSpace.AllCells) { //// 2020/11
                cell.IsSelected = false;
            }

            this.Editor.ReloadCanvas();
        }

        /// <summary>
        /// Shows the muted lines.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ShowMutedLines(object sender, RoutedEventArgs e) {
            this.EditorSpace.ShowMutedLines = true;
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            //// this.EditorSpace.LoadContent(this.EditorSpace.MusicalContent, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            this.Editor.ReloadScrollBars();
        }

        /// <summary>
        /// Hides the muted lines.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void HideMutedLines(object sender, RoutedEventArgs e) {
            this.EditorSpace.ShowMutedLines = false;
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.Editor.ToggleContentBase.IsChecked ?? false);
            //// this.EditorSpace.LoadContent(this.EditorSpace.MusicalContent, this.Editor.ToggleContentBase.IsChecked ?? false);
            this.Editor.ReloadCanvas();
            this.Editor.ReloadScrollBars();
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Currents the line.
        /// </summary>
        /// <returns> Returns value. </returns>
        private IAbstractLine CurrentLine() {
            IAbstractLine line = null;
            if (this.Editor.CurrentCell is GroupCell cell) {
                line = cell.FirstCell.Line;
            }
            else {
                if (this.Editor.CurrentCell is LineCell linecell) {
                    line = linecell.Line;
                }
            }

            return line;
        }
        #endregion
    }
}
