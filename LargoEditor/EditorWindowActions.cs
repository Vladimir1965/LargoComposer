// <copyright file="EditorWindowActions.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels;
using EditorPanels.Cells;
using EditorPanels.Detail;
using EditorWindows;
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedControls.Abstract;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LargoEditor
{
    /// <summary>
    /// Editor Window 
    /// </summary>
    /// <seealso cref="LargoSharedClasses.Support.WinAbstract" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public sealed partial class EditorWindow
    {
        #region Public methods - Line type 
        /// <summary>
        /// Converts the editor line to melodic.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ConvertEditorLineToMelodic(object sender, RoutedEventArgs e) {
            var line = EditorInspector.Singleton.InspectElement.Line;
            if (line == null) {
                return;
            }

            var block = this.Block;
            line.LineType = MusicalLineType.Melodic;
            line.MainVoice.Channel = block.Strip.FindFreeChannel(line.LineIndex);
            var list = block.Body.ElementsOfLine(line.LineIdent);
            var master = new ElementMaster(list);
            master.ConvertToMelodic(line.MainVoice.Channel);
            e.Handled = true;
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
        }

        /// <summary>
        /// Converts the editor line to rhythmic.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ConvertEditorLineToRhythmic(object sender, RoutedEventArgs e) {
            var line = EditorInspector.Singleton.InspectElement.Line;
            if (line == null) {
                return;
            }

            var block = this.Block;
            line.LineType = MusicalLineType.Rhythmic;
            line.MainVoice.Channel = MidiChannel.DrumChannel;
            var list = block.Body.ElementsOfLine(line.LineIdent);
            var master = new ElementMaster(list);
            master.ConvertToRhythmic(line.MainVoice.Channel);
            e.Handled = true;
            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
        }
        #endregion

        #region Panels from block menu
        /// <summary>
        /// Blocks the harmonic.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void BlockHarmonic(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("EditorWindows", "BlockHarmonyWindow", null);
            var musicalBlock = this.EditorSpace.MusicalContent as MusicalBlock;
            var harmonicModel = HarmonicModel.GetNewModel("Inner", musicalBlock);
            //// var wrap = new MusicalBlockWrap(block);
            BlockHarmonyWindow.Singleton.LoadData(harmonicModel);
        }

        /// <summary>
        /// Rhythmic of the blocks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void BlockRhythmic(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("EditorWindows", "BlockRhythmWindow", null);
            var musicalBlock = this.EditorSpace.MusicalContent as MusicalBlock;
            var rhythmicModel = RhythmicModel.GetNewModel("Inner", musicalBlock);
            //// var wrap = new MusicalBlockWrap(block);
            BlockRhythmWindow.Singleton.LoadData(rhythmicModel);
        }

        /// <summary>
        /// Melodic of the blocks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void BlockMelodic(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("EditorWindows", "BlockMelodyWindow", null);
            var musicalBlock = this.EditorSpace.MusicalContent as MusicalBlock;
            var rhythmicModel = RhythmicModel.GetNewModel("Inner", musicalBlock);
            var melodicModel = MelodicModel.GetNewModel("Inner", musicalBlock);
            var melodicAnalyzer = new MelodicAnalyzer();
            melodicAnalyzer.AnalyzeMusicalLines(melodicModel, rhythmicModel, musicalBlock);
            //// var wrap = new MusicalBlockWrap(block);
            BlockMelodyWindow.Singleton.LoadData(melodicModel);
        }

        /// <summary>
        /// Opens the saved patterns.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SavedHarmonicTemplates(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.SavedHarmonicTemplates();
        }

        /// <summary>
        /// Opens the saved patterns.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SavedRhythmicTemplates(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.SavedRhythmicTemplates();
        }
        #endregion

        #region Public methods - Tracer
        /// <summary>
        /// Opens the tone tracer.
        /// </summary>
        [UsedImplicitly]
        public void OpenToneTracer() {
            var pm = new PanelToneTracer();
            pm.LoadData();
            var settings = MusicalSettings.Singleton;
            settings.SettingsProgram.HasTraceValues = true;
        }

        #endregion

        #region Drag-Drop
        /// <summary>
        /// Drops the image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropImage(object sender, DragEventArgs e) {
            //// e.Data.GetDataPresent("RhythmicStructure") ||
            bool handled = false;
            var point = e.GetPosition(this.ChartCanvas);
            this.MouseEnterCell(point);
            this.CurrentCell = this.EnteredCell; //// 2020/11

            var exists = !handled && e.Data.GetDataPresent("MusicalBlock");
            if (exists) {
                if (e.Data.GetData("MusicalBlock") is MusicalBlock musicalBlock) {
                    this.LoadBlockToSpace(musicalBlock);
                    Console.Beep(990, 180);
                    handled = true;
                }
            }
                    
            exists = !handled && e.Data.GetDataPresent("RhythmicStream");
            if (exists) {
                if (e.Data.GetData("RhythmicStream") is RhythmicStream rhythmicStream) {
                    var block = this.Block;
                    var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
                    var selectedGroupCells = this.EditorSpace.SelectedGroupCells;
                   
                    if (selectedGroupCells != null && selectedGroupCells.Count > 0) {
                        var idx = 0;
                        foreach (var rhythmicStructure in rhythmicStream.Structures) {
                            idx = (idx + 1) % selectedGroupCells.Count;
                            var groupCell = selectedGroupCells[idx];
                            foreach (var cell in groupCell.InnerCells) {
                                cell.Status.RhythmicStructure = rhythmicStructure;
                            }
                        }
                    }

                    //// this.EditorSpace.MakeCellsFromContent(block, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
                    Console.Beep(990, 180);
                    handled = true;
                }
            }

            exists = !handled && (e.Data.GetDataPresent("HarmonicModality") || e.Data.GetDataPresent("HarmonicStream")
                            || e.Data.GetDataPresent("RhythmicMaterial") || e.Data.GetDataPresent("OrchestraBlock"));
            if (exists) {
                if (e.Data.GetData("HarmonicModality") is HarmonicModality harmonicModality) {
                    var block = this.Block;
                    var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
                    block.Modulate(area, harmonicModality);
                    this.EditorSpace.BarCells = new List<BarCell>();
                    this.EditorSpace.MakeBarCells(block.ContentBars);
                    Console.Beep(990, 180);
                    handled = true;
                }

                if (e.Data.GetData("HarmonicStream") is HarmonicStream harmonicStream) {
                    var block = this.Block;
                    var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
                    block.Harmonize(area, harmonicStream);
                    this.EditorSpace.BarCells = new List<BarCell>();
                    this.EditorSpace.MakeBarCells(block.ContentBars);
                    Console.Beep(990, 180);
                    handled = true;
                }

                if (e.Data.GetData("RhythmicMaterial") is RhythmicMaterial rhythmicMaterial) {
                    var block = this.Block;
                    var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
                    block.ChangeRhythmic(area, rhythmicMaterial);
                    this.EditorSpace.MakeCellsFromContent(block, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
                    Console.Beep(990, 180);
                    handled = true;
                }

                if (e.Data.GetData("OrchestraBlock") is OrchestraBlock orchestraBlock) {
                    var block = this.Block;
                    var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
                    block.Orchestrate(area, orchestraBlock);
                    this.EditorSpace.MakeCellsFromContent(block, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
                    Console.Beep(990, 180);
                    handled = true;
                }

                e.Handled = handled;
                return;
            }

            bool ok = false;
            exists = e.Data.GetDataPresent("HarmonicStructure") || e.Data.GetDataPresent("MusicalTempo");
            if (exists) {
                var selectedBarCells = this.EditorSpace.SelectedBarCells;
                if (selectedBarCells != null && selectedBarCells.Count > 0) {
                    ok = true;
                    foreach (var cell in selectedBarCells) {
                        ok = ok && cell.SetData(e.Data);
                    }
                }
                else {
                    if (this.CurrentCell != null && this.CurrentCell.CellType == CellType.BarCell) {
                        var cell = (BarCell)this.CurrentCell;
                        ok = cell.SetData(e.Data);
                    }
                }

                handled = ok;
            }

            exists = !handled && (e.Data.GetDataPresent("MelodicInstrument") || e.Data.GetDataPresent("RhythmicInstrument")
                    || e.Data.GetDataPresent("MusicalOctave") || e.Data.GetDataPresent("MusicalLoudness"));
            if (exists) {
                var selectedLineCells = this.EditorSpace.SelectedLineCells;
                if (selectedLineCells != null && selectedLineCells.Count > 0) {
                    ok = true;
                    foreach (var cell in selectedLineCells) {
                        ok = ok && cell.SetData(e.Data);
                    }
                }
                else {
                    if (this.CurrentCell != null && this.CurrentCell.CellType == CellType.LineCell) {
                        var cell = (LineCell)this.CurrentCell;
                        ok = cell.SetData(e.Data);
                    }
                }

                handled = true;
            }

            exists = !handled && (e.Data.GetDataPresent("RhythmicFace") || e.Data.GetDataPresent("MelodicFace")
                    || e.Data.GetDataPresent("MelodicFunction") || e.Data.GetDataPresent("MelodicShape")
                    || e.Data.GetDataPresent("RhythmicStructure") || e.Data.GetDataPresent("MelodicStructure")
                    || e.Data.GetDataPresent("OrchestraUnit"));
            if (exists) {
                var selectedGroupCells = this.EditorSpace.SelectedGroupCells;
                if (selectedGroupCells != null && selectedGroupCells.Count > 0) {
                    ok = true;
                    foreach (var cell in selectedGroupCells) {
                        ok = ok && cell.SetData(e.Data);
                    }
                }
                else {
                    if (this.CurrentCell != null && this.CurrentCell.CellType == CellType.GroupCell) {
                        var cell = (GroupCell)this.CurrentCell;
                        ok = cell.SetData(e.Data);
                    }
                }

                handled = ok;
            }

            /* Tempo
            if (e.Data.GetData("MusicalTempo") is KeyValuePair tempo) {
                //// var rs = new BarStatus { TempoNumber = int.Parse(tempo.Key) };
                //// this.Bar.Status.TempoNumber = int.Parse(tempo.Key);
                System.Console.Beep(990, 180);
            } */

            if (handled) {
                //// 2020/01 this.currentCell.RedrawCell(false);
                this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
                Console.Beep(990, 180);
                this.ReloadCanvas();
            }

            e.Handled = handled;
        }

        /// <summary>
        /// Handles the Drop event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (files != null) {
                    PortDocuments.Singleton.LoadBundle(files[0], false);
                }
            }
        }
        #endregion

        #region Private methods - Compose plan/Edit

        /// <summary>
        /// Plays the online.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ComposeAndPlay(object sender, RoutedEventArgs e) {
            MusicalBundle result = CommonActions.Singleton.Compose(this.Block);
            if (result == null) {
                return;
            }

            var resultBlock = result.Blocks[0];
            CommonActions.Singleton.SelectedBlock = resultBlock;

            this.MusicStop(null, null);
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.LeftCtrl)) {
                //// Play outside in windows
                MusicalPlayer.Play(resultBlock, true);  //// UserFileLoader.Singleton.MusicalBlock
                return;
            }
             
            //// Standard online play in editor
            this.LoadBlockToSpace(resultBlock);
            MusicalPlayer.Play(resultBlock, true);  //// UserFileLoader.Singleton.MusicalBlock
        }

        /// <summary>
        /// Plays the in panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void PlayMp3(object sender, RoutedEventArgs e) {
            MusicalBundle result = CommonActions.Singleton.Compose(this.Block);
            if (result == null) {
                return;
            }

            //// if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.LeftCtrl)) {
            //// Play in panel
            this.Save(null, null);
            CommonActions.Singleton.PerformByPlayer(PortDocuments.Singleton.FilePath);
            return;
            //// }

            var resultBlock = result.Blocks[0];
            CommonActions.Singleton.SelectedBlock = resultBlock;
            this.MusicStop(null, null);
            //// UserFileLoader.Singleton.MusicPlayMp3(resultBlock);
        }

        #endregion

        #region Private methods - Panels
        /// <summary>
        /// Views the panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> Instance containing the event data.</param>
        /// <exception cref="System.InvalidOperationException">Invalid Operation Exception</exception>
        private void ViewPanel(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var intTag = int.Parse(menuItem?.Tag as string ?? throw new InvalidOperationException());

            var w = UserWindows.Singleton.GetPanel(intTag);
            if (w != null && w.IsVisible) {
                w.Close();
                //// menuItem.Foreground = Brushes.White;
                return;
            }            

            //// menuItem.Foreground = Brushes.Yellow;
            UserWindows.Singleton.ViewPanel(intTag); //// this.EditorSpace, this.CurrentCell
            if (w != null) {
                //// w.MenuItem = menuItem;
            }
        }

            /*
            /// <summary>
            /// Views all panels.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
            private void ViewAllPanels(object sender, RoutedEventArgs e) {
                UserWindows.Singleton.AllSidePanels();
            }
            */

            /// <summary>
            /// Views the inspector.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
            private void ViewInspector(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.Inspector(null, null);
            this.EditorSpace.SelectedCellChanged(this.CurrentCell);
        }

        /*
        /// <summary>
        /// Smarts the rhythm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SmartRhythm(object sender, RoutedEventArgs e) {
            if (SideHarmonicStructuresWindow.Singleton == null) {
                return;
            }

            if (!(this.EditorSpace.MusicalContent is MusicalBlock block)) {
                return;
            }

            var win = (SmartRhythmWindow)WindowManager.OpenWindow("EditorWindows", "SmartRhythmWindow", null);
            win.Block = this.Block;
            win.RhythmicModel = this.BlockWrap.RhythmicModel;
        }
        */

        /// <summary>
        /// Saved orchestrations.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SavedOrchestrations(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.SavedOrchestraTemplates(null, null);
        }

        /// <summary>
        /// Block list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BlockList(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("EditorWindows", "BlockListWindow", null);
            BlockListWindow.Singleton.LoadData();
        }

        #endregion

        #region Private methods - Music
        /// <summary>
        /// Music stop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MusicStop(object sender, RoutedEventArgs e) {
            TimedPlayer.Singleton.StopPlaying(); //// 2016/07
            //// MusicalPlayer.Singleton.StopPlaying();
            //// SystemProcesses.KillApplication("XLargoPlay");
            //// 2016/12 TimedPlayer.Singleton.Reset();
        }
        #endregion
    }
}