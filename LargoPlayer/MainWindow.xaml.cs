// <copyright file="MainWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoPlayer
{
    using LargoSharedClasses.Localization;
    using LargoSharedClasses.MidiFile;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using LargoSharedControls.Abstract;
    using LargoSharedPanels;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() {
            this.InitializeComponent();

            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"));
                return;
            }

            PlayerSettings.Singleton.LoadDefaultValues();
            UserFileLoader.Singleton.LoadWindowManager("LargoPlayer", "MainWindow", typeof(MainWindow)); //// 2021/08
            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);

            this.Localize();

            string path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
            if (!PortDocuments.Singleton.LoadResults(path)) {
                MessageBox.Show(string.Format("Internal file not found! {0}\n\n", path), SettingsApplication.ApplicationName);
            }

            this.DataGrid.ContextMenu = CommonMenus.Singleton.ContextMenuInGrid;
            PortCatalogs.Singleton.ReadXmlFiles(PlayerSettings.Singleton.PathToMusicList);
            this.LoadNotators();
            this.MainImage.ContextMenu = CommonMenus.Singleton.ContextMenuInImage;

            this.AllowDrop = true;
            this.Drop += this.Window_Drop;

            this.RefreshGrid();

            CommandEventSender.Singleton.BundleLoaded += this.BundleLoaded;
        }

        #endregion

        #region Public methods

        /// <summary> Loads the files. </summary>
        public void LoadFiles() {
            string filePath = MainApplication.Parameter0;   //// App. //// MessageBox.Show(App.Parameter0);
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            if (!PortDocuments.Singleton.LoadBundle(filePath, false)) {
                return;
            }

            PortDocuments.Singleton.FilePath = filePath;
            this.Play(null, null);
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Transforms the music.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="tag">The tag.</param>
        /// <returns> Returns value. </returns>
        private static MusicalBlock TransformMusic(MusicalBlock givenBlock, int tag) {
            var blockToPlay = givenBlock;
            if (tag > 1) {
                blockToPlay = givenBlock.Clone(true, true);
                blockToPlay.MusicalBundle = givenBlock.MusicalBundle;

                var strip = blockToPlay.Strip;
                switch (tag) {
                    case 2:
                        strip.VerticalInversion();
                        break;
                    case 3:
                        strip.BarInversion();
                        break;
                    case 4:
                        strip.VerticalExtension();
                        break;
                    case 5:
                        strip.VerticalNarrowing();
                        break;
                    case 6:
                        strip.ModularDeformation();
                        break;
                    case 7:
                        strip.HorizontalInversion();
                        break;
                    case 8:
                        strip.OctaveUp();
                        break;
                    case 9:
                        strip.OctaveDown();
                        break;
                }
            }

            blockToPlay.ConvertStripToBody(false);
            return blockToPlay;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Bundles the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="BundleEventArgs"/> instance containing the event data.</param>
        private void BundleLoaded(object sender, BundleEventArgs args) {
            if (!PortDocuments.Singleton.BundleLoaded(args.MusicalBundle)) {
                return;
            }

            var result = new MusicDocument(PortDocuments.Singleton.MusicalBlock.Header);
            if (result == null) {
                return;
            }

            result.FilePath = PortDocuments.Singleton.FilePath;
            PortDocuments.Singleton.ResultDocumentMaster.DocumentList.Insert(0, result);
            PortDocuments.Singleton.SelectedDocument = result;
            PortDocuments.Singleton.SaveResults();
            this.RefreshGrid();
        }

        /// <summary>
        /// Refreshes the grid.
        /// </summary>
        private void RefreshGrid() {
            var cnt = PortDocuments.Singleton.ResultDocumentMaster.DocumentList.Count;
            this.DataGrid.ItemsSource = null;
            this.DataGrid.ItemsSource = PortDocuments.Singleton.ResultDocumentMaster.DocumentList;
            if (cnt > 0) {
                this.DataGrid.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the note editors.
        /// </summary>
        private void LoadNotators() {
            this.ControlNotator.LoadData();
            var settings = MusicalSettings.Singleton;
            if (settings != null) {
                this.ControlNotator.SelectItem(settings.SettingsProgram.Notator);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the gridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(this.DataGrid.SelectedItem is MusicDocument document)) {
                return;
            }

            this.TextBlockFile.Text = document.Header.FileName;
            PortDocuments.Singleton.SelectedDocument = document;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the gridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (this.DataGrid.SelectedItem is MusicDocument) { //// document
                ///// var result = UserFileLoader.Singleton.LoadBundle(document.Header.FilePath);
                //// if (result == null)  {    return;     } 
            }

            this.Play(null, null);
        }

        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Music to note editor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MusicToNotator(object sender, RoutedEventArgs e) {
            var mn = (MusicalNotator)this.ControlNotator.Combo.SelectedItem;
            if (mn == null) {
                return;
            }

            var musicBlock = PortDocuments.Singleton.MusicalBlock;
            var midiBlock = new CompactMidiBlock(musicBlock);
            var sequence = midiBlock.Sequence(musicBlock.Header.FullName);
            //// MusicalPlayer.Singleton.PlayImmediately = false;
            MusicalPlayer.Singleton.TakeSequence(sequence, false);
            MultimediaCommands.Singleton.MusicToNotator(mn.Path);
        }

        /// <summary>
        /// Music play.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Play(object sender, RoutedEventArgs e) {
            const bool playOnline = true; //// 2018/09 !?!?!?!?
            if (TimedPlayer.Singleton.IsPause) {
                TimedPlayer.Singleton.ContinuePlaying(); //// 2016/07
                return;
            }

            PortDocuments.Singleton.LoadDocument(PortDocuments.Singleton.SelectedDocument, true);

            //// var document = DataInterface.Singleton.ResultDocumentMaster.SelectedDocument;
            if (PortDocuments.Singleton.MusicalBlock == null) {
                return;
            }

            //// Transformation (deformation)
            if (!(this.ComboDeformation.SelectedItem is ComboBoxItem item)) {
                return;
            }

            var tag = int.Parse(item.Tag.ToString());
            MusicalBlock blockToPlay = TransformMusic(PortDocuments.Singleton.MusicalBlock, tag);
            MusicalPlayer.Play(blockToPlay, playOnline);
        }

        /// <summary>
        /// Edits the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Edit(object sender, RoutedEventArgs e) {
            if (this.DataGrid.SelectedItem is MusicDocument document) {
                var selectedMifName = document.FilePath;

                var fileName = selectedMifName;
                if (string.IsNullOrWhiteSpace(fileName)) {
                    return;
                }

                var args = '"' + fileName + '"';
                var programPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.BinaryFolder);
                var command = Path.Combine(programPath, "XLargoEdit.exe");
                Process.Start(command, args);
            }
        }

        /// <summary>
        /// Music pause.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MusicPause(object sender, RoutedEventArgs e) {
            TimedPlayer.Singleton.PausePlaying();
            MusicalPlayer.Singleton.StopPlaying();
        }

        /// <summary>
        /// Music stop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MusicStop(object sender, RoutedEventArgs e) {
            TimedPlayer.Singleton.StopPlaying(); //// 2016/07
            MusicalPlayer.Singleton.StopPlaying();
            //// 2016/12 TimedPlayer.Singleton.Reset();
        }

        #endregion

        #region Closing

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            PortDocuments.Singleton.SaveResults();
            WindowManager.Singleton.SavePosition(this);
            WindowManager.SaveWindowManager(WindowManager.Singleton.Status, WindowManager.Singleton.ManagerPath);
            MusicalPlayer.Singleton.StopPlaying();
            TimedPlayer.Singleton.StopPlaying();
            MusicalSettings.SaveMusicalFolders(MusicalSettings.Singleton.Folders);
            Application.Current.Shutdown();
        }

        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the ComboDeformation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboDeformation_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }

        /// <summary>
        /// Music play MP3.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MusicPlayMp3(object sender, RoutedEventArgs e) {
            this.MusicStop(null, null);
            UserFileLoader.Singleton.MusicPlayMp3(PortDocuments.Singleton.MusicalBlock);
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
                //// var result = 
                if (files == null) {
                    return;
                }

                PortDocuments.Singleton.LoadBundle(files[0], false);
                this.Play(null, null);
                //// this.LoadFile(files[0]);
            }
        }

        /// <summary>
        /// Saves the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Save(object sender, RoutedEventArgs e) {
            PortDocuments.Singleton.SaveDocuments();
            MusicalSettings.SaveMusicalFolders(MusicalSettings.Singleton.Folders);
        }
    }
}
