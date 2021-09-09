// <copyright file="MainWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoManager
{
    using LargoSharedClasses.Localization;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Port;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using LargoSharedClasses.Templates;
    using LargoSharedControls.Abstract;
    using LargoSharedWindows;
    using ManagerPanels;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.Linq;

    /// <summary>
    /// Master Window.
    /// </summary>
    public partial class MainWindow
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() {
            this.InitializeComponent();
            this.Actions = new MainActions(this);
            this.Menus = new MainMenus(this, this.Actions);

            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"));
                return;
            }

            DataInstaller.InstallMissingFiles();
            UserFileLoader.Singleton.LoadWindowManager("LargoManager", "MainWindow", typeof(MainWindow));
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            PortCatalogs.Singleton.ReadXmlFiles(path);
            
            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Localize();

            path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
            if (!PortDocuments.Singleton.LoadDocuments(path)) {
                MessageBox.Show(string.Format("Internal file not found! {0}\n\n", path), SettingsApplication.ApplicationName);
                return;
            }

            //// path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
            PortAnalysis.Singleton.InternalPath = ManagerSettings.Singleton.PathToInternalTemplates;
            PortCatalogs.Singleton.ReadXmlFiles(ManagerSettings.Singleton.PathToInternalTemplates);

            this.MainImage.ContextMenu = CommonMenus.Singleton.ContextMenuInImage;
            //// this.ButtonSave.ContextMenu = UserFileLoader.Singleton.ContextMenuOfSave;

            CommandEventSender.Singleton.BundleLoaded += this.BundleLoaded;
            this.GridBlocks.ContextMenu = this.Menus.ContextMenuInGrid;
            this.AllowDrop = true;
            this.Drop += this.Window_Drop;

            this.LoadBlocks();
            //// settings.Save();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the editor menus.
        /// </summary>
        /// <value>
        /// The editor menus.
        /// </value>
        public MainMenus Menus { get; set; }

        /// <summary>
        /// Gets or sets the editor actions.
        /// </summary>
        /// <value>
        /// The editor actions.
        /// </value>
        public MainActions Actions { get; set; }
        #endregion

        #region Public methods

        /// <summary> Loads the files. </summary>
        public void LoadFile() {
            string filePath = MainApplication.Parameter0;   //// App. //// MessageBox.Show(App.Parameter0);
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            if (!PortDocuments.Singleton.LoadBundle(filePath, false)) {
                return;
            }
        }

        #endregion

        #region Closing
        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            var msg = LocalizedControls.String(@"Do you want to close the program?");
            var result = MessageBox.Show(msg, LocalizedControls.String("Largo"), MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) {
                e.Cancel = true;
                return;
            }

            this.Save(null, null);
            WindowManager.Singleton.SavePosition(this);
            WindowManager.SaveWindowManager(WindowManager.Singleton.Status, WindowManager.Singleton.ManagerPath);
            MusicalPlayer.Singleton.StopPlaying();
            TimedPlayer.Singleton.StopPlaying();
            ManagerSettings.Singleton.Save();
            MusicalSettings.Singleton.Save();
            Application.Current.Shutdown();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Handles the MouseDown event of the LargoImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void LargoImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            //// this.OpenAbout(null, null);
        }

        #endregion

        #region Private menu events

        /// <summary> Open Blocks. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        private void BlockInspector(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoSharedWindows", "BlockHeadWindow", null);
            if (BlockHeadWindow.Singleton != null) {
                if (!(this.GridBlocks.SelectedItem is MusicDocument document) || !PortDocuments.Singleton.LoadDocument(document, true)) {
                    return;
                }

                var block = PortDocuments.Singleton.MusicalBlock;
                BlockHeadWindow.Singleton.LoadBlock(block, document.FilePath);
            }
        }
        #endregion

        #region Load data 
        /// <summary>
        /// Loads the blocks.
        /// </summary>
        private void LoadBlocks() {
            this.GridBlocks.ItemsSource = null;
            var list = PortDocuments.Singleton.BlockDocumentMaster.DocumentList;

            this.GridBlocks.ItemsSource = list;
            if (list?.Count > 0) {
                this.GridBlocks.SelectedIndex = 0;
            }
        }
        #endregion

        #region Save data 

        /// <summary>
        /// Saves the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Save(object sender, RoutedEventArgs e) {
            PortDocuments.Singleton.SaveDocuments();
            MusicalSettings.SaveMusicalFolders(MusicalSettings.Singleton.Folders);
        }

        /// <summary>
        /// Handles the UnloadingRow event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridRowEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_UnloadingRow(object sender, DataGridRowEventArgs e) {
            if (((DataGrid)sender).SelectedItem != null || ((DataGrid)sender).CurrentItem == null) {
                return;
            }

            //// Row was deleted ....
            PortDocuments.Singleton.SaveDocuments();
            MusicalSettings.SaveMusicalFolders(MusicalSettings.Singleton.Folders);
        }

        #endregion

        #region External call from Windows (Drag-Drop) 
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

        #region Internal Callback methods
        /// <summary>
        /// Midis the file loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="BundleEventArgs"/> instance containing the event data.</param>
        private void BundleLoaded(object sender, BundleEventArgs args) {
            if (!PortDocuments.Singleton.BundleLoaded(args.MusicalBundle)) {
                return;
            }

            PortAnalysis.Singleton.ExtractModels(args.MusicalBundle, ManagerSettings.Singleton.PathToInternalTemplates);
            PortDocuments.Singleton.BlockDocumentMaster.AddBlocksOfBundle(args.MusicalBundle, true);
            PortDocuments.Singleton.SaveDocuments();

            var cnt = PortDocuments.Singleton.BlockDocumentMaster.DocumentList.Count;
            if (cnt > 0) {
                this.GridBlocks.ItemsSource = null;
                this.GridBlocks.ItemsSource = PortDocuments.Singleton.BlockDocumentMaster.DocumentList;
                this.GridBlocks.SelectedIndex = cnt - 1;
            }
        }

    #endregion

        #region GridBlocks events
        /// <summary>
        /// Handles the SelectionChanged event of the gridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
                MusicDocument document = this.GridBlocks.SelectedItem as MusicDocument;
                PortDocuments.Singleton.SelectedDocument = document;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the gridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            CommonActions.Singleton.Edit(null, null);
        }
        #endregion

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenWindow("LargoManager", "MusicFileLoader", null);
        }

        /// <summary>
        /// Creates new file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NewFile(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenWindow("LargoManager", "NewFileMaker", null);
        }
    }
}
