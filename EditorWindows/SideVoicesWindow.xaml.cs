// <copyright file="SideVoicesWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels;
using EditorPanels.Abstract;
using EditorPanels.Cells;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedControls.Abstract;
using LargoSharedWindows;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EditorWindows
{
    /// <summary>
    /// Interaction logic for Voices Window.
    /// </summary>
    public partial class SideVoicesWindow 
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideVoicesWindow singleton;

        /// <summary> The chart master. </summary>
        private LineSpace lineSpace;

        //// <summary> The dragged cell. </summary>
        //// private BaseCell draggedCell;

        /// <summary> The current cell. </summary>
        private BaseCell currentCell;

        //// <summary> The context menu of the window. </summary>
        //// private ContextMenu contextMenuOfTheWindow;
        #endregion

        #region Context Menus

        /// <summary>
        /// The context menu of line
        /// </summary>
        private ContextMenu contextMenuOfVoice;
        #endregion

        /// <summary> Initializes a new instance of the <see cref="SideVoicesWindow" /> class. </summary>
        public SideVoicesWindow() {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            SharedWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            //// this.Localize();

            this.AllowDrop = true;
            this.Drop += this.DropImage;

            //// this.ContextMenu = this.ContextMenuOfTheWindow;
            //// this.editorSpace = null; ////  EditorWindow.Singleton.EditorSpace;
            this.ContextMenu = this.ContextMenuOfVoice;
            SidePanels.Singleton.PanelOpen("SideVoices");
        }

        #region Static properties
        /// <summary>
        /// Gets the SideVoicesWindow Singleton.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public static SideVoicesWindow Singleton {
            get {
                Contract.Ensures(Contract.Result<SideVoicesWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        #region Menu properties

        /// <summary>
        /// Gets the context menu of single line.
        /// </summary>
        /// <value>
        /// The context menu of single line.
        /// </value>
        public ContextMenu ContextMenuOfVoice {
            get {
                if (this.contextMenuOfVoice != null) {
                    return this.contextMenuOfVoice;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                //// Item Delete
                var item = new MenuItem {
                    Header = "Delete voice",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += this.DeleteLineVoice;
                contextMenu.Items.Add(item);

                this.contextMenuOfVoice = contextMenu;
                return contextMenu;
            }
        }

        #endregion

        #region Public methods 
        /// <summary>
        /// Reloads the given given chart master.
        /// </summary>
        /// <param name="givenEditorLine">The given editor line.</param>
        public void LoadEditorLine(EditorLine givenEditorLine) { //// , MusicalBlock block
            this.ChartCanvas.Children.Clear();
            this.lineSpace = new LineSpace {
                Line = givenEditorLine.Line
            };

            this.lineSpace.LoadVoices(givenEditorLine.Line);
            this.ChartCanvas.Children.Add(this.lineSpace);
            this.lineSpace.InvalidateVisual();
        }

        /// <summary>
        /// Reloads the muster grid.
        /// </summary>
        public void ReloadCanvas() {
            this.lineSpace?.InvalidateVisual();
            //// this.draggedCell = this.EditorSpace.AllCells.FirstOrDefault();
        }

        #endregion

        #region Drag-Drop
        /// <summary>
        /// Drops the image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropImage(object sender, DragEventArgs e) {
            bool exists = e.Data.GetDataPresent("MelodicInstrument") || e.Data.GetDataPresent("RhythmicInstrument")
                    || e.Data.GetDataPresent("MusicalOctave") || e.Data.GetDataPresent("MusicalLoudness");
            if (!exists) {
                return;
            }

            var point = e.GetPosition(this.ChartCanvas);
            this.MouseEnterCell(point);

            if (this.currentCell == null) {
                return;
            }

            if (this.currentCell.GetType() == typeof(VoiceCell)) {
                var cell = this.currentCell as VoiceCell;
                if (cell == null) {
                    return;
                }

                if (e.Data.GetData("MelodicInstrument") is MelodicInstrument melodicInstrument) {
                    var instrument = new MusicalInstrument((MidiMelodicInstrument)melodicInstrument.Id);
                    cell.Voice.Instrument = instrument;
                    this.lineSpace.InvalidateVisual();
                    System.Console.Beep(990, 180);
                }

                if (e.Data.GetData("RhythmicInstrument") is RhythmicInstrument rhythmicInstrument) {
                    var instrument = new MusicalInstrument((MidiRhythmicInstrument)rhythmicInstrument.Id);
                    cell.Voice.Instrument = instrument;
                    this.lineSpace.InvalidateVisual();
                    System.Console.Beep(990, 180);
                }

                if (e.Data.GetData("MusicalOctave") is MusicalOctave musicalOctave) {
                    cell.Voice.Octave = musicalOctave;
                    this.lineSpace.InvalidateVisual();
                    System.Console.Beep(990, 180);
                }

                if (e.Data.GetData("MusicalLoudness") is MusicalLoudness musicalLoudness) {
                    cell.Voice.Loudness = musicalLoudness;
                    this.lineSpace.InvalidateVisual();
                    System.Console.Beep(990, 180);
                }
            }

            e.Handled = true;
        }

        #endregion

        #region Closing
        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
            SidePanels.Singleton.PanelClose("SideVoices");
            SideVoicesWindow.Singleton = null;
        }

        #endregion

        #region Private methods 

        /// <summary> Plan mouse enter cell. </summary>
        /// <param name="point"> The point. </param>
        private void MouseEnterCell(Point point) {
            var cell = this.lineSpace.GetVoiceCell(point);
            if (cell == null) {
                return;
            }

            //// 2020/01 this.currentCell?.RedrawCell(false);
            this.currentCell = cell;
            //// 2020/01 cell.RedrawCell(true);
        }

        /// <summary>
        /// Add line voice.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddLineVoice(object sender, RoutedEventArgs e) {
            var line = this.lineSpace.Line;
            var voice = new MusicalVoice {
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1),
                Octave = MusicalOctave.OneLine,
                Loudness = MusicalLoudness.MeanLoudness,
                Line = line
            };

            line.Voices.Add(voice);
            this.lineSpace.LoadVoices(line);

            //// this.lineSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
            //// this.ReloadScrollBars();
            e.Handled = true;
        }

        /// <summary>
        /// Delete line voice.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DeleteLineVoice(object sender, RoutedEventArgs e) {
            var line = this.lineSpace.Line;
            //// var x = this.currentCell.LineIndex;
            if (line.Voices.Count > 0) {
                line.Voices.RemoveAt(0);
                this.lineSpace.LoadVoices(line);
            }

            //// this.lineSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
            //// this.ReloadScrollBars();
            e.Handled = true;
            e.Handled = true;
        }

        #endregion
    }
}
