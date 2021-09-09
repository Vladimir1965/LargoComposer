// <copyright file="TemplatesUserHarmonic.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System.IO;
using System.Windows;

namespace LargoManager
{
    /// <summary>
    /// Templates Window.
    /// </summary>
    public partial class TemplatesUserHarmonic : WinAbstract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesUserHarmonic" /> class.
        /// </summary>
        public TemplatesUserHarmonic() {
            this.InitializeComponent();
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
            var blockTemps = HarmonicStream.ReadStreams(path, "HarmonicTemplates.xml"); //// UserFileLoader.Singleton.LoadBlockTemplates();
            if (blockTemps == null) {
                return;
            }

            this.GridTemplates.ItemsSource = blockTemps;
            if (blockTemps.Count > 0) {
                this.GridTemplates.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridTemplates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (!(this.GridTemplates.SelectedItem is HarmonicStream stream)) {
                return;
            }

            HarmonicStream stream1 = stream;
            this.DataContext = stream1;
            this.GridLines.ItemsSource = stream.HarmonicBars;
        }

        /// <summary>
        /// Loads from harmony.
        /// </summary>
        private void LoadFromHarmony() {
            var importPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport);
            var filePath = FileDialogs.OpenSelectedHarmonicFile(importPath);
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            //// FileInfo fi = new FileInfo(filePath); fi.DirectoryName
            var folder = Path.GetDirectoryName(filePath);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, folder);

            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "Harmony") {
                var xharmony = root;
                var harmonicStream = new HarmonicStream(xharmony, false);
                /* not finished
                foreach (var hbar in harmonicStream.HarmonicBars) {
                    foreach (var hstruct in hbar.HarmonicStructures) {
                        this.Material.Add(hstruct);
                        this.Stream.Add(hstruct);
                    }
                }

                this.GridMaterial.ItemsSource = null;
                this.GridMaterial.ItemsSource = this.Material;
                this.GridStream.ItemsSource = null;
                this.GridStream.ItemsSource = this.Stream;
                */
            }
        }

        /// <summary>
        /// Loads from harmony text.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        private void LoadFromHarmonyText(MusicalHeader givenHeader) {
            var importPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport);
            var filePath = FileDialogs.OpenSelectedTextFile(importPath);
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            //// FileInfo fi = new FileInfo(filePath); fi.DirectoryName
            var folder = Path.GetDirectoryName(filePath);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, folder);
            var content = SupportFiles.FileToString(filePath);
            var harmonicStream = new HarmonicStream(givenHeader, content);
            if (harmonicStream == null) {
                return;
            }

            /* not finished
          foreach (var hbar in harmonicStream.HarmonicBars) {
                foreach (var hstruct in hbar.HarmonicStructures) {
                    this.Material.Add(hstruct);
                    this.Stream.Add(hstruct);
                }
            }

            this.GridMaterial.ItemsSource = null;
            this.GridMaterial.ItemsSource = this.Material;
            this.GridStream.ItemsSource = null;
            this.GridStream.ItemsSource = this.Stream;
            */
        }

        /// <summary>
        /// Handles the Checked event of the ImportedStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportHarmonicFile(object sender, RoutedEventArgs e) {
            this.LoadFromHarmony();
        }

        /// <summary>
        /// Handles the Checked event of the ImportedTextStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportTextFile(object sender, RoutedEventArgs e) {
            var header = MusicalHeader.GetDefaultMusicalHeader;
            this.LoadFromHarmonyText(header);
        }

        /// <summary>
        /// Smarts the generator.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SmartGenerator(object sender, RoutedEventArgs e)
        {
            var win = (SmartHarmonyWindow)WindowManager.OpenWindow("LargoManager", "SmartHarmonyWindow", null);
        }

        /*
        /// <summary>
        /// Automatics the harmony.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SmartHarmony(object sender, RoutedEventArgs e) {
            if (SideHarmonicStructuresWindow.Singleton == null) {
                return;
            }

            /*
            if (!(this.EditorSpace.MusicalContent is MusicalBlock block)) {
                return;
            }

            this.Block = block;
            *
            var win = (SmartHarmonyWindow)WindowManager.OpenWindow("EditorWindows", "SmartHarmonyWindow", null);
            //// win.Block = this.Block;

            //// this.EditorSpace.ResetBarCellsText();
            //// this.ReloadCanvas();
        } */
    }
}
