// <copyright file="EditorWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoEditor
{
    using EditorPanels;
    using EditorPanels.Cells;
    using EditorWindows;
    using LargoSharedClasses.Localization;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.MidiFile;
    using LargoSharedClasses.Models;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Port;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using LargoSharedClasses.Templates;
    using LargoSharedControls.Abstract;
    using LargoSharedWindows;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Xml.Linq;

    /// <summary>
    /// Interaction logic for Editor Window.
    /// </summary>
    public sealed partial class EditorWindow
    {
        /// <summary>
        /// The void handler
        /// </summary>
        private static readonly VoidHandler Handler = () => { }; //// 2016/08

        #region Fields

        /// <summary>
        /// The dragged cell
        /// </summary>
        private BaseCell draggedCell;

        /// <summary> Last Horizontal Scroll. </summary>
        private double lastHorizontalScrollValue;

        /// <summary> Last Vertical Scroll. </summary>
        private double lastVerticalScrollValue;
        #endregion

        #region Rectangle for dragging 
        /// <summary> True to drag. </summary>
        private bool drag;

        /// <summary> The start point. </summary>
        private Point dragStartPoint;

        /// <summary> The last location. </summary>
        private Point lastLoc;

        /// <summary> Gets the canvas top. </summary>
        /// <value> The canvas top. </value>
        private double dragNewCanvasLeft, dragNewCanvasTop;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorWindow"/> class.
        /// </summary>
        public EditorWindow() {
            this.InitializeComponent();
            this.EditorSpace = new EditorSpace();
            this.ChartCanvas.Children.Add(this.EditorSpace);
            this.Actions = new EditorActions(this);
            this.Menus = new EditorMenus(this, this.Actions);

            //// 2021/08 Taken from MainApplication.OnStartup
            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"));
                return;
            }

            UserFileLoader.Singleton.LoadWindowManager("LargoEditor", "EditorWindow", typeof(EditorWindow)); //// 2021/08
            //// 2021/08 Taken from MainApplication.OnStartup
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            //// EditorSettings.Singleton.PathToMusicList
            PortCatalogs.Singleton.ReadXmlFiles(path);

            WindowManager.Singleton.LoadPosition(this);
            //// Load window coordinates
            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            this.Focusable = true;
            this.ChartCanvas.Focusable = true;
            this.Localize();
            this.AssignEventHandlers();
            UserWindows.Singleton.UserSidePanels();
            this.ButtonSave.ContextMenu = this.Menus.ContextMenuOfSave;
            CommandEventSender.Singleton.BundleLoaded += this.BundleLoaded; //// 2019/02
            this.SizeChanged += this.WindowSizeChanged;
            TimedPlayer.Singleton.SkipToBar += this.EditorSkipToBar;

            PortAnalysis.Singleton.InternalPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
        }

        #endregion

        #region Delegates
        /// <summary>
        /// Void Handler.
        /// </summary>
        private delegate void VoidHandler();
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the editor menus.
        /// </summary>
        /// <value>
        /// The editor menus.
        /// </value>
        public EditorMenus Menus { get; set; }

        /// <summary>
        /// Gets or sets the editor actions.
        /// </summary>
        /// <value>
        /// The editor actions.
        /// </value>
        public EditorActions Actions { get; set; }

        /// <summary>
        /// Gets or sets the chart master.
        /// </summary>
        /// <value>
        /// The chart master.
        /// </value>
        public EditorSpace EditorSpace { get; set; }

        /// <summary>
        /// Gets or sets Musical Block Model.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalBlock Block { get; set; }

        /// <summary>
        /// Gets or sets the block wrap.
        /// </summary>
        /// <value>
        /// The block wrap.
        /// </value>
        public MusicalBlockWrap BlockWrap { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public EditorContent ContentType { get; set; }

        /// <summary>
        /// Gets or sets the entered cell.
        /// </summary>
        /// <value>
        /// The entered cell.
        /// </value>
        public BaseCell EnteredCell { get; set; }

        /// <summary>
        /// Gets or sets the current cell
        /// </summary>
        /// <value>
        /// The current cell.
        /// </value>
        public BaseCell CurrentCell { get; set; }
        #endregion

        #region Public methods - Music file
        /// <summary>
        /// Loads the files.
        /// </summary>
        /// <param name="tectonicTemplatePath">The tectonic template path.</param>
        /// <param name="harmonicTemplatePath">The harmonic template path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool NewMusicFile(string tectonicTemplatePath, string harmonicTemplatePath) {
            var fileName = "New file";
            var tectonicTemplate = new TemplateBlock();
            if (File.Exists(tectonicTemplatePath)) {
                var xdoc = XDocument.Load(tectonicTemplatePath);
                var root = xdoc.Root;
                if (root == null || root.Name != "Tectonic") {
                    return false;
                }

                tectonicTemplate = new TemplateBlock(root);
            }

            var harmonicTemplate = new HarmonicStream();
            if (File.Exists(harmonicTemplatePath)) {
                var xdoc = XDocument.Load(harmonicTemplatePath);
                var root = xdoc.Root;
                if (root == null || root.Name != "Harmony") {
                    return false;
                }

                harmonicTemplate = new HarmonicStream(root, true);
            }

            var block = MusicalBlock.DefaultBlock(tectonicTemplate);
            var area = new MusicalSection(1, block.Header.NumberOfBars, string.Empty);
            block.Harmonize(area, harmonicTemplate);
            //// this.EditorSpace.BarCells = new List<BarCell>();
            //// this.EditorSpace.MakeBarCells(block.ContentBars);
            block.Rhythmize(area, RhythmicStructure.GetNewRhythmicStructure(block.Header.System.RhythmicSystem, 1));

            var bundle = MusicalBundle.GetEnvelopeOfBlock(block, fileName);
            PortDocuments.Singleton.MusicalBlock = block;

            var args = new BundleEventArgs(bundle, ObjectOperation.ObjectLoaded, false);
            this.BundleLoaded(null, args);

            var header = this.EditorSpace.GetMusicalHeader;
            header.FileName = fileName;
            this.Title = string.Format("Music file ({0})", header.FullName);

            this.ToggleContentBase.IsChecked = false;
            this.ToggleMusicToPlan(null, null);
            return true;
        }

        /// <summary> Loads the files. </summary>
        /// <returns> Returns value. </returns>
        public bool LoadMusicFile() {
            string filePath = MainApplication.Parameter0;   //// App. //// MessageBox.Show(App.Parameter0);
            if (string.IsNullOrEmpty(filePath)) {
                return false;
            }

            if (!File.Exists(filePath)) {
                MessageBox.Show(filePath, @"File not found!?");
                return false;
            }

            if (!PortDocuments.Singleton.LoadBundle(filePath, false)) {
                return false;
            }

            this.ToggleContentBase.IsChecked = true;
            this.TogglePlanToMusic(null, null);

            this.Title = string.Format("Music file ({0})", filePath);
            return true;
        }

        #endregion

        #region Public methods - Canvas
        /// <summary>
        /// Reloads the muster grid.
        /// </summary>
        public void ReloadCanvas() {
            if (this.EditorSpace != null) {
                this.EditorSpace.InvalidateVisual();
                this.draggedCell = this.EditorSpace.AllCells.FirstOrDefault();
            }
        }

        #endregion

        #region Private methods - Lines, Bars
        /// <summary>
        /// Clones the line as is.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void CloneLineAsIs(object sender, RoutedEventArgs e) {
            var line = EditorInspector.Singleton.InspectElement.Line;
            this.CloneLine((MusicalLine)line);
        }

        /// <summary>
        /// Charts the add line.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void EditorAddLine(object sender, RoutedEventArgs e) {
            if (this.EditorSpace == null) {
                return;
            }

            int tag = 1;
            var item = sender as MenuItem;
            if (item != null) {
                tag = int.Parse(item.Tag as string ?? throw new InvalidOperationException());
            }

            LineRhythm lineRhythm = LineRhythm.None;
            BarScope barScope = BarScope.AllBars;
            switch (tag) {
                case 1: {
                        lineRhythm = LineRhythm.SimpleOneTone;
                        break;
                    }

                case 2: {
                        lineRhythm = LineRhythm.HarmonicStructure;
                        break;
                    }

                case 3: {
                        lineRhythm = LineRhythm.HarmonicShape;
                        break;
                    }

                case 4: {
                        lineRhythm = LineRhythm.HarmonicStructure;
                        barScope = BarScope.OddBars;
                        break;
                    }

                case 5: {
                        lineRhythm = LineRhythm.HarmonicStructure;
                        barScope = BarScope.EvenBars;
                        break;
                    }

                case 6: {
                        lineRhythm = LineRhythm.HarmonicShape;
                        barScope = BarScope.OddBars;
                        break;
                    }

                case 7: {
                        lineRhythm = LineRhythm.HarmonicShape;
                        barScope = BarScope.EvenBars;
                        break;
                    }
            }

            this.EditorAddLine(lineRhythm, barScope);
            ////  this.EditorSpace.InvalidateVisual();
        }

        /// <summary> Plan add bar. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void EditorAddBar(object sender, RoutedEventArgs e) {
            var block = EditorSpace?.MusicalContent as MusicalBlock;

            var lastBar = block?.Body.Bars.LastOrDefault();
            //// if (lastBar == null) { return; }

            block.AddContentBar((lastBar?.BarNumber ?? 0) + 1, lastBar);

            //// Convert current body back to strip!?! (Status is in elements)
            block.ConvertBodyToStrip(false, false);
            this.EditorSpace.LoadContent(block, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
            this.ReloadScrollBars();
        }

        /// <summary>
        /// Reloads the scroll bars.
        /// </summary>
        public void ReloadScrollBars() {
            this.VerticalScrollBar.Maximum = this.EditorSpace.NumberOfLines;
            this.HorizontalScrollBar.Maximum = this.EditorSpace.NumberOfBars;
            //// this.EditorSpace.MaxLeft = this.EditorSpace.LeftSpace - 5 + (int)14 * SeedSize.CurrentWidth;
            //// this.EditorSpace.MaxTop = this.EditorSpace.TopSpace - 5 + (int)18 * SeedSize.CurrentHeight;

            int availableBars = (int)Math.Floor((this.Width - this.EditorSpace.LeftSpace - this.EditorSpace.LeftMargin - 20) / SeedSize.CurrentWidth);
            int availableLines = (int)Math.Floor((this.Height - this.EditorSpace.TopSpace - this.EditorSpace.TopMargin - 10) / SeedSize.CurrentHeight);
            int displayLines = Math.Min(this.Block.Header.NumberOfLines, availableLines);

            this.EditorSpace.MaxLeft = this.EditorSpace.LeftSpace + this.EditorSpace.LeftMargin + (availableBars * SeedSize.CurrentWidth); //// 1
            this.EditorSpace.MaxTop = this.EditorSpace.TopSpace + this.EditorSpace.TopMargin + (availableLines * SeedSize.CurrentHeight);  //// 5

            this.HorizontalScrollBar.Width = this.EditorSpace.LeftMargin + (availableBars * SeedSize.CurrentWidth);
            this.HorizontalScrollBar.Height = 20;
            this.HorizontalScrollBar.Margin = new Thickness(this.EditorSpace.LeftSpace, this.EditorSpace.TopSpace + this.EditorSpace.TopMargin + (displayLines * SeedSize.CurrentHeight), 0, 0);

            this.VerticalScrollBar.Width = 20;
            this.VerticalScrollBar.Height = this.EditorSpace.TopMargin + (displayLines * SeedSize.CurrentHeight);
            this.VerticalScrollBar.Margin = new Thickness(this.EditorSpace.MaxLeft, this.EditorSpace.TopSpace, 0, 0);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboContentType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        public void ContentTypeChanged(object sender, RoutedEventArgs e) {
            if (sender != null) {
                int tag = int.Parse((sender as MenuItem)?.Tag as string ?? throw new InvalidOperationException());
                this.ContentType = (EditorContent)tag;
            }
            else {
                this.ContentType = EditorContent.Cell;
            }

            this.EditorSpace.PrepareGroupCells(this.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
        }
        #endregion

        #region Private static

        /// <summary>
        /// Does the void events.
        /// </summary>
        private static void DoVoidEvents() {
            //// Stack Overflow Exception
            try {
                //// Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new VoidHandler(() => { }));
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, Handler);
            }
            catch { //// StackOverflowException ex
                //// return;  //// !?!?!
            }
        }

        /// <summary>
        /// Checks the editor window.
        /// </summary>
        private static void CheckEditorWindow() {
            int countEditorWindows = 0;
            for (int intCounter = Application.Current.Windows.Count - 1; intCounter >= 0; intCounter--) {
                if (intCounter >= Application.Current.Windows.Count) {
                    continue;
                }

                var win = Application.Current.Windows[intCounter];
                if (win is EditorWindow) {
                    countEditorWindows++;
                }
            }

            //// Exists another editor window
            if (countEditorWindows > 1) {
                return;
            }
        }
        #endregion

        #region Private methods - Music Editor

        /// <summary>
        /// Midis the file loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="BundleEventArgs"/> instance containing the event data.</param>
        private void BundleLoaded(object sender, BundleEventArgs args) {
            if (!PortDocuments.Singleton.BundleLoaded(args.MusicalBundle)) {
                return;
            }

            this.LoadBlockToSpace(PortDocuments.Singleton.MusicalBlock);
        }

        /// <summary>
        /// Loads the block to space.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        private void LoadBlockToSpace(MusicalBlock givenBlock) {
            this.Block = givenBlock;
            if (this.Block == null) {
                return;
            }

            this.MusicStop(null, null);
            this.BlockWrap = new MusicalBlockWrap(this.Block);

            //// Chart Master (FrameworkElement)
            this.EditorSpace.LoadContent(this.Block, true);
            this.EditorSpace.NumberOfBars = this.Block.Header.NumberOfBars;

            this.ReloadScrollBars();
            this.ContentTypeChanged(null, null);
        }
        #endregion

        #region Private menu methods

        /// <summary>
        /// Saves the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Save(object sender, RoutedEventArgs e) {
            var header = this.EditorSpace.GetMusicalHeader;
            if (string.IsNullOrWhiteSpace(header.FullName)) {
                header.FileName = "New file";
            }

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            if (path == null) {
                return;
            }

            var filePath = Path.Combine(path, header.FullName + ".mif");
            var destinationFilePath = filePath;
            bool existsFile = File.Exists(filePath);
            if (!existsFile) {
                destinationFilePath = FileDialogs.SelectMifiFileToSave(filePath);
                if (string.IsNullOrEmpty(filePath)) {
                    return;
                }
            }

            PortDocuments.Singleton.FilePath = destinationFilePath;
            var bundle = MusicalBundle.GetEnvelopeOfBlock(this.Block, header.FullName);
            var port = PortAbstract.CreatePort(MusicalSourceType.MIFI);
            port.WriteMusicFile(bundle, destinationFilePath);

            MessageBox.Show(string.Format("File saved!\n\n{0}", destinationFilePath), SettingsApplication.ApplicationName);
        }
        #endregion

        #region Private methods - Support for Lines
        /// <summary>
        /// Adds the melodic line.
        /// </summary>
        /// <param name="givenLineRhythm">The given line rhythm.</param>
        /// <param name="barScope">The bar scope.</param>
        private void EditorAddLine(LineRhythm givenLineRhythm, BarScope barScope) {
            var lineStatus = new LineStatus() {
                LineType = MusicalLineType.Melodic,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1),
                Octave = MusicalOctave.TwoLine,
                Loudness = MusicalLoudness.MeanLoudness,
                MelodicFunction = MelodicFunction.HarmonicMotion,
                MelodicShape = MelodicShape.Scales,
                LocalPurpose = LinePurpose.Composed
            };

            var newLine = this.EditorSpace.MusicalContent.AddContentLine(lineStatus);
            newLine.Purpose = LinePurpose.Composed;
            newLine.LineIdent = Guid.NewGuid();

            if (this.EditorSpace.MusicalContent is MusicalBlock block) {
                //// Complete rhythmic of the track
                byte targetRhythmicOrder = block.Header.System.RhythmicOrder;
                
                block.Body.SetRhythmToBars(newLine.LineIdent, givenLineRhythm, barScope, targetRhythmicOrder, true); //// settings!
                block.Body.PrepareRhythmInLine(newLine.LineIdent);

                var elements = block.Body.ElementsOfLine(newLine.LineIdent).ToList();
                MusicalBody.SetPurposeToElements(newLine.Purpose, elements);

                //// Convert current body back to strip!?! (Status is in elements)
                block.ConvertBodyToStrip(false, false);
            }

            var voice = new MusicalVoice {
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1),
                Octave = MusicalOctave.OneLine,
                Loudness = MusicalLoudness.MeanLoudness,
                Line = newLine
            };

            newLine.Voices.Add(voice);
            newLine.MainVoice = newLine.Voices.FirstOrDefault();

            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
            this.ReloadScrollBars();
        }

        /// <summary>
        /// Clones the line.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        private void CloneLine(MusicalLine givenLine) {
            if (givenLine == null) {
                return;
            }

            var newStatus = (LineStatus)givenLine.FirstStatus.Clone();
            var newLine = this.Block.AddContentLine(newStatus);
            if (newLine == null) {
                return;
            }

            var block = this.Block;
            var channel = block.Strip.FindFreeChannel(newLine.LineIndex);
            newLine.MainVoice.Channel = channel;
            newLine.Purpose = LinePurpose.Composed;

            foreach (var bar in block.Body.Bars) {
                var point = MusicalPoint.GetPoint(givenLine.LineIndex, bar.BarNumber);
                var me = block.Body.GetElement(point);
                if (me == null) {
                    continue;
                }

                var newPoint = MusicalPoint.GetPoint(newLine.LineIndex, bar.BarNumber);
                var newElement = block.Body.GetElement(newPoint);
                if (newElement == null) {
                    continue;
                }

                newElement.Status = (LineStatus)me.Status.Clone();
                newElement.Status.LocalPurpose = LinePurpose.Composed;
            }

            //// Convert current body back to strip!?! (Status is in elements)
            block.ConvertBodyToStrip(false, true);

            this.EditorSpace.MakeCellsFromContent(this.EditorSpace.MusicalContent, this.EditorSpace.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
            this.ReloadScrollBars();
        }

        #endregion

        #region Private methods - Others

        /// <summary>
        /// Assigns the event handlers.
        /// </summary>
        private void AssignEventHandlers() {
            this.MouseLeftButtonDown += this.WindowMouseDown;
            this.MouseRightButtonDown += this.WindowMouseDown;
            this.MouseMove += this.WindowMouseMove;
            this.MouseUp += this.WindowMouseUp;
            this.MouseDoubleClick += this.WindowMouseDoubleClick;
            this.MouseWheel += this.WindowMouseWheel;
            this.KeyUp += this.WindowKeyUp;
            this.AllowDrop = true;
            this.Drop += this.DropImage;
        }

        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
            //// CultureMaster.Localize(this.layoutHeader);
        }

        /// <summary>
        /// Handles the StateChanged event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_StateChanged(object sender, EventArgs e) {
            WindowManager.Singleton.ChangeStateTo(this.WindowState);
        }
        #endregion

        #region Private methods and events - Size Changed, Scroll bars
        /// <summary>
        /// Windows the size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e) {
            if (this.ContextMenu != null) {
                return;
            }

            if (this.EditorSpace == null) {
                return;
            }

            this.ReloadScrollBars();
            this.ReloadCanvas();
        }

        /// <summary>
        /// Handles the Scroll event of the VerticalScrollBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollEventArgs"/> instance containing the event data.</param>
        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e) {
            var value = Math.Floor(((ScrollBar)sender).Value) * SeedSize.BasicHeight;
            ///// var value = Math.Round(((ScrollBar)sender).Value, 0) * SeedSize.BasicHeight;
            if (Math.Abs(value - this.lastVerticalScrollValue) < EditorSpace.DeltaVerticalScroll) {
                return;
            }

            this.lastVerticalScrollValue = value;
            this.EditorSpace.TopScroll = value;
            this.ReloadCanvas();
        }

        /// <summary>
        /// Handles the Scroll event of the HorizontalScrollBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollEventArgs"/> instance containing the event data.</param>
        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e) {
            var scrollLength = Math.Floor(((ScrollBar)sender).Value) * SeedSize.BasicWidth;
            ///// var value = Math.Round(((ScrollBar)sender).Value, 0) * SeedSize.BasicWidth;

            if (Math.Abs(scrollLength - this.lastHorizontalScrollValue) < EditorSpace.DeltaHorizontalScroll) {
                return;
            }

            this.lastHorizontalScrollValue = scrollLength;
            this.EditorSpace.LeftScroll = scrollLength;
            this.ReloadCanvas();
        }
        #endregion

        #region Private Events - Keys (copy-paste)
        /// <summary>
        /// Windows the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void WindowKeyUp(object sender, KeyEventArgs e) {
            //// var cell = this.EditorSpace.SelectedCell;
            if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)) {
                switch (e.SystemKey) {
                    case Key.H: //// Harmony
                        UserWindows.Singleton.ViewPanel(1); 
                        break;
                    case Key.R: //// Rhythm
                        UserWindows.Singleton.ViewPanel(2);
                        break;
                    case Key.M: //// Melody
                        UserWindows.Singleton.ViewPanel(3);
                        break;
                    case Key.I: //// Instrument
                        UserWindows.Singleton.ViewPanel(4);
                        break;
                    case Key.T: //// Tempo
                        UserWindows.Singleton.ViewPanel(5);
                        break;
                    case Key.V: //// Voices
                        UserWindows.Singleton.ViewPanel(5);
                        break;
                    case Key.K: //// Keys
                        UserWindows.Singleton.SideHarmonicModality(null, null);
                        UserWindows.Singleton.SideRhythmicModality(null, null);
                        break;
                }
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) {
                switch (e.Key) {
                    case Key.X:
                        this.EditorSpace.CopyPaste('X');
                        break;
                    case Key.C:
                        this.EditorSpace.CopyPaste('C');
                        break;
                    case Key.V:
                        this.EditorSpace.CopyPaste('V');
                        break;
                }
            }
            else {
                switch (e.Key) {
                    case Key.Left:
                        this.EditorSpace.SelectNext(0, -1);
                        break;
                    case Key.Right:
                        this.EditorSpace.SelectNext(0, 1);
                        break;
                    case Key.Up:
                        this.EditorSpace.SelectNext(1, 0);
                        break;
                    case Key.Down:
                        this.EditorSpace.SelectNext(-1, 0);
                        break;
                }
            }

            e.Handled = true;
        }

        #endregion

        #region Mouse Events - Enter/Down/Move/Up
        /// <summary>
        /// Kits the mouse enter cell.
        /// </summary>
        /// <param name="point">The point.</param>
        private void MouseEnterCell(Point point) {
            if (this.ContextMenu != null && this.ContextMenu.IsOpen && this.ContextMenu.IsMouseOver) {
                return;
            }

            //// It runs also when Menu is on
            this.dragStartPoint = point; //// e.GetPosition(this.ChartCanvas);
            var cell = this.EditorSpace.GetCellAtMousePoint(this.dragStartPoint);
            //// if (cell == null) {  return;   }

            if (this.EnteredCell != null && cell != this.EnteredCell) {
                this.EnteredCell.IsHighlighted = false;
            }

            if (cell != null) { //// 2020/10
                this.EnteredCell = cell;
            }

            if (this.EnteredCell == null) {
                return;
            }

            this.EnteredCell.IsHighlighted = true;

            if (this.ContextMenu != null) {
                return;
            }

            this.ReloadCanvas(); //// !!!! 
        }

        /// <summary>
        /// Windows the mouse double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void WindowMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (this.EditorSpace == null) {
                return;
            }

            if (!(this.CurrentCell is LineCell)) {
                return;
            }

            if (SideVoicesWindow.Singleton != null) {
                SideVoicesWindow.Singleton.WindowState = WindowState.Normal;
                return;
            }

            //// this.Voices(null, null);
        }

        /// <summary> Window mouse down. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Mouse button event information. </param>
        private void WindowMouseDown(object sender, MouseButtonEventArgs e) {
            if (this.ContextMenu != null && this.ContextMenu.IsOpen && this.ContextMenu.IsMouseOver) {
                return;
            }

            if (this.EditorSpace == null) {
                return;
            }

            if (this.EnteredCell != null) {
                this.CurrentCell = this.EnteredCell;
            }

            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) { //// Necessary for line determination ...!? 
                this.EditorSpace.SelectedCellChanged(this.CurrentCell);
                
                //// WindowManager.OpenWindow("LargoEditor", "SideVoicesWindow", null);
                if (SideVoicesWindow.Singleton != null) {
                    if (this.CurrentCell.GetType() == typeof(LineCell)) {
                        LineCell lineCell = this.CurrentCell as LineCell;
                        if (lineCell?.Line != null) {
                            //// var block = this.EditorSpace.MusicalContent as MusicalBlock;
                            var editorLine = this.EditorSpace.GetEditorLine(lineCell.Line.LineIdent);
                            SideVoicesWindow.Singleton.LoadEditorLine(editorLine);
                        }
                    }
                }
            }

            if (Keyboard.IsKeyDown(Key.LeftShift)) {
                if (this.CurrentCell == null) {
                    return;
                }

                this.CurrentCell.IsSelected = !this.CurrentCell.IsSelected;
                this.CurrentCell.IsHighlighted = false;
                //// var contentCell = this.currentCell as ContentCell;
            }

            if (e.RightButton == MouseButtonState.Pressed) {
                if (this.ButtonBlock.IsMouseOver) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfBlock;
                    e.Handled = true;
                    return;
                }                

                if (this.ButtonAddLine.IsMouseOver) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfAddLine;
                    e.Handled = true;
                    return;
                }

                if (this.ButtonHarmony.IsMouseOver) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfHarmony;
                    e.Handled = true;
                    return;
                }

                if (this.ButtonRhythm.IsMouseOver) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfRhythm;
                    e.Handled = true;
                    return;
                }
                
                if (this.CurrentCell == null) {
                    return;
                }

                if (this.CurrentCell.GetType() == typeof(CornerCell)) {
                    this.ContextMenu = null;
                    this.ContextMenu = Keyboard.IsKeyDown(Key.LeftCtrl) ? this.Menus.ContextMenuContentType : this.Menus.ContextMenuOfCorner;
                    e.Handled = true;
                    return;
                }

                if (this.CurrentCell.GetType() == typeof(BarCell)) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfBar;
                    e.Handled = true;
                    return;
                }

                if (this.CurrentCell.GetType() == typeof(LineCell)) {
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfLine;
                    e.Handled = true;
                    return;
                }

                if (this.CurrentCell.GetType() == typeof(GroupCell)) {  //// ContentCell
                    this.ContextMenu = null;
                    this.ContextMenu = this.Menus.ContextMenuOfContent;
                    ////  this.ContextMenu = this.MainContextMenu; Print?!?
                    e.Handled = true;
                    return;
                }

                if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                    this.drag = true;
                    this.Cursor = Cursors.Hand;
                    this.draggedCell = this.CurrentCell;
                    //// this.draggedCell.DrawEmptyCell(this.ChartCanvas);
                    //// var draggedCellWidth = (int)this.draggedCell.Width;
                    //// var draggedCellHeight = (int)this.draggedCell.Height;
                    this.lastLoc = new Point(this.draggedCell.Left, this.draggedCell.Top);
                    Mouse.Capture((IInputElement)sender);
                }
            }

            //// Twoo click outside the editor cell make unselect of all cells    
            if (this.EnteredCell == null) { //// 2020/11
                this.Actions.UnselectAllCells(null, null);
            }

            this.EnteredCell = null; //// 2020/11
            this.ContextMenu = null;  //// 2020/11
            //// this.CurrentCell = null;
            this.ReloadCanvas(); //// EditorSpace
        }

        /// <summary> Event handler. Called by Rectangle_MouseMove for 1 events. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Mouse event information. </param>
        private void WindowMouseMove(object sender, MouseEventArgs e) {
            if (this.EditorSpace == null) {
                return;
            }

            try {
                if (this.drag && Keyboard.IsKeyDown(Key.LeftCtrl)) {
                    var newX = this.dragStartPoint.X + (e.GetPosition(this.ChartCanvas).X - this.dragStartPoint.X);
                    var newY = this.dragStartPoint.Y + (e.GetPosition(this.ChartCanvas).Y - this.dragStartPoint.Y);
                    Point offset = new Point(this.dragStartPoint.X - this.lastLoc.X, this.dragStartPoint.Y - this.lastLoc.Y);
                    this.dragNewCanvasTop = newY - offset.Y;
                    this.dragNewCanvasLeft = newX - offset.X;
                    this.draggedCell.Top = this.dragNewCanvasTop;
                    this.draggedCell.Left = this.dragNewCanvasLeft;
                    //// 2020/01 this.draggedCell.DrawDraggingCell(this.ChartCanvas);
                }
                else {
                    var point = e.GetPosition(this.ChartCanvas);
                    this.MouseEnterCell(point);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, SettingsApplication.ApplicationName);
            }
        }

        /// <summary> Window mouse up. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Mouse button event information. </param>
        private void WindowMouseUp(object sender, MouseButtonEventArgs e) {
            if (this.ContextMenu != null && this.ContextMenu.IsOpen) {
                return;
            } 

            if (this.EditorSpace == null) {
                return;
            }

            this.drag = false;
            this.Cursor = Cursors.Arrow;

            if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                Mouse.Capture(null);
                //// 2020/01 this.draggedCell.DrawCell(this.ChartCanvas);
            }
        }

        /// <summary>
        /// Handles the OnMouseWheel event of the EditorMainPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs"/> instance containing the event data.</param>
        private void WindowMouseWheel(object sender, MouseWheelEventArgs e) {
            if (this.ContextMenu != null) {
                return;
            } 

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                this.PercentSizeChanged(e.Delta);
                e.Handled = true;
            }
        }

        #endregion

        #region Private methods - Zooming
        /// <summary>
        /// The size changed as percent.
        /// </summary>
        /// <param name="givenValue">The given value.</param>
        private void PercentSizeChanged(int givenValue) {
            SeedSize.PercentSizeChanged(givenValue);
        }

        #endregion

        #region Private Events - Windows
        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
            CheckEditorWindow();
            WindowManager.SaveWindowManager(WindowManager.Singleton.Status, WindowManager.Singleton.ManagerPath);
            MusicalPlayer.Singleton.StopPlaying();
            TimedPlayer.Singleton.StopPlaying();
            MusicalSettings.SaveMusicalFolders(MusicalSettings.Singleton.Folders);
            MusicalSettings.Singleton.Save();
            this.CloseOtherWindows(); //// Have to be after MusicalSettings.Singleton.Save!
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Closes the other windows.
        /// </summary>
        private void CloseOtherWindows() {
            //// Close all
            for (int intCounter = Application.Current.Windows.Count - 1; intCounter >= 0; intCounter--) {
                if (intCounter >= Application.Current.Windows.Count) {
                    continue;
                }

                var win = Application.Current.Windows[intCounter];
                if (win != null && (win.GetType() != typeof(EditorWindow) && win.Title != this.Title)) {
                    //// Application.Current.Windows[intCounter].Window_Closing(null, null);
                    win.Close();
                }
            }
        }

        #endregion

        #region Private Events - Content combo
        /// <summary>
        /// Toggles the content base check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TogglePlanToMusic(object sender, RoutedEventArgs e) {
            if (this.EditorSpace == null) {
                return;
            }

            this.EditorSpace.IsMusicEditor = true;
            this.EditorSpace.ReflectPlan();

            this.ToggleContentBase.Content = " 0 -- I ";
            this.EditorSpace.PrepareGroupCells(this.ContentType, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
        }

        /// <summary>
        /// Toggles the content base unchecked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleMusicToPlan(object sender, RoutedEventArgs e) {
            if (this.EditorSpace == null) {
                return;
            }

            this.EditorSpace.IsMusicEditor = false;
            this.EditorSpace.ReflectMusic();

            this.ToggleContentBase.Content = " I -- 0 ";
            this.EditorSpace.PrepareGroupCells(EditorContent.Cell, this.ToggleContentBase.IsChecked ?? false);
            this.ReloadCanvas();
        }

        #endregion

        #region Private methods - Others

        /// <summary>
        /// Highlights the bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        private void HighlightBar(int barNumber) {
            //// MessageBox.Show(barNumber.ToString());
            if (barNumber >= 1) {
                if (barNumber >= 2) {
                    var previousCell = this.EditorSpace.BarCells[barNumber - 2];
                    previousCell.IsHighlighted = false;
                }

                var range = this.HorizontalScrollBar.Maximum - this.HorizontalScrollBar.Minimum;
                var value = (range * barNumber / this.EditorSpace.BarCells.Count) - 1;
                var scrollLength = value * SeedSize.BasicWidth;
                this.HorizontalScrollBar.Value = value;
                this.lastHorizontalScrollValue = value;
                this.EditorSpace.LeftScroll = scrollLength;

                if (barNumber - 1 < this.EditorSpace.BarCells.Count) {
                    var cell = this.EditorSpace.BarCells[barNumber - 1];
                    cell.IsHighlighted = true;
                    //// this.EditorSpace.InvalidateVisual();
                }

                this.ReloadCanvas();
            }
        }

        /// <summary> Open Blocks. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        private void BlockInspector(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoSharedWindows", "BlockHeadWindow", null);
            if (BlockHeadWindow.Singleton != null) {
                var block = this.EditorSpace.MusicalContent as MusicalBlock; //// UserFileLoader.Singleton.MusicalBlock;
                BlockHeadWindow.Singleton.LoadBlock(block,  PortDocuments.Singleton.FilePath);
            }
        }

        /// <summary>
        /// Files the save as.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void FileSaveAs(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Harmonics the template.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HarmonicTemplates(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.SavedHarmonicTemplates();
            //// WindowManager.OpenWindow("EditorWindows", "TemplatesUserHarmonic", null);
        }

        /// <summary>
        /// Rhythmical templates.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RhythmicTemplates(object sender, RoutedEventArgs e) {
            UserWindows.Singleton.SavedRhythmicTemplates();
            //// WindowManager.OpenWindow("EditorWindows", "TemplatesUserRhythmic", null);
        }
        #endregion

        #region Listeners
        /// <summary>
        /// Editors the skip to bar.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SkipToBarEventArgs"/> instance containing the event data.</param>
        private void EditorSkipToBar(object sender, SkipToBarEventArgs args) {
            var dispatcher = this.Dispatcher;
            dispatcher?.BeginInvoke(
                DispatcherPriority.Background,
                (Action)(() => this.HighlightBar(args.BarNumber)));

            DoVoidEvents();
            //// this.GetButton(args.BarNumber, 0);
        }
        #endregion
    }
}
