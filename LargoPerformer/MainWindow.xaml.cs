// <copyright file="MainWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace LargoPerformer
{
    /// <summary>
    /// Interaction logic for the application.
    /// </summary>
    public partial class MainWindow : Window {
        #region Static Fields
        /// <summary>
        /// The void handler
        /// </summary>
        private static readonly VoidHandler Handler = () => { }; //// 2016/08

        #endregion

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

            WindowManager.Singleton.LoadPosition(this);
            //// UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            ////  this.Show();

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            PortCatalogs.Singleton.ReadXmlFiles(path);

            this.MusicPort = new PortMifi();
            this.Running = false;
            MidiFilePlayer.PrepareMidi();
            MidiFilePlayer.OpenMidi();
            this.BarNumber = 0;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Void Handler.
        /// </summary>
        private delegate void VoidHandler();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MainWindow"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        private bool Running { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MainWindow"/> is playing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if playing; otherwise, <c>false</c>.
        /// </value>
        private bool Playing { get; set; }

        /// <summary>
        /// Gets the music port.
        /// </summary>
        /// <value>
        /// The music port.
        /// </value>
        private PortAbstract MusicPort { get; }

        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        private int BarNumber { get; set; }
        #endregion

        #region Public Support 
        /// <summary>
        /// Lasts the file.
        /// </summary>
        /// <returns> Returns value. </returns>
        public FileInfo LastFile() { // najdu si XML soubor k importu
            var path = ConductorSettings.Singleton.PathToInternalStream;
            DirectoryInfo adresar = new DirectoryInfo(path);
            FileInfo[] files = adresar.GetFiles("*.mif", SearchOption.TopDirectoryOnly);
            DateTime datum = DateTime.Now;
            var f = from file in files orderby file.LastWriteTime.Ticks select file;
            FileInfo fileInfo = f.LastOrDefault();

            foreach (FileInfo fi in files) {
                if (fi.Name == fileInfo.Name) { 
                    continue; 
                }

                var filepath = fi.FullName;
                var ext = fi.Extension;
                if (ext != ".mif") {
                    continue;
                }

                File.Delete(filepath);
            }

            return fileInfo;
        }

        /// <summary>
        /// Next file.
        /// </summary>
        /// <returns> Returns value. </returns>
        public FileInfo NextFile() { // najdu si XML soubor k importu
            var path = ConductorSettings.Singleton.PathToInternalStream;
            DirectoryInfo adresar = new DirectoryInfo(path);
            FileInfo[] files = adresar.GetFiles("*.mif", SearchOption.TopDirectoryOnly);
            DateTime datum = DateTime.Now;
            FileInfo fileInfo = null;
            foreach (FileInfo fi in files) {
                if (datum.Ticks < fi.LastWriteTime.Ticks) {
                    continue; // je tam starsi
                }

                datum = fi.LastWriteTime;
                fileInfo = new FileInfo(fi.FullName.Trim());
            }

            return fileInfo;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
            WindowManager.SaveWindowManager(WindowManager.Singleton.Status, WindowManager.Singleton.ManagerPath);
            MusicalPlayer.Singleton.StopPlaying();
            TimedPlayer.Singleton.StopPlaying();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Mains the loop.
        /// </summary>
        private void MainLoop() {
            while (this.Running) {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, Handler);
                this.Playing = TimedPlayer.Singleton.IsPlaying;
                if (this.Playing) {
                    continue;
                }

                var fileInfo = this.LastFile(); //// NextFile();
                if (fileInfo == null) {
                    continue;
                }

                var path = fileInfo.FullName;
                var ext = fileInfo.Extension;
                if (ext != ".mif") {
                    continue;
                }

                this.BarNumber++;
                this.TextBlock.Text = string.Format("Bar {0} ({1})", this.BarNumber, fileInfo.Name);
                var statusPath = ConductorSettings.Singleton.PathToStatusFile;
                SupportFiles.StringToFile(this.BarNumber.ToString(), statusPath);
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, Handler);

                this.ProcessFile(path);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Stops the continue.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void StopContinue(object sender, RoutedEventArgs e) {
            this.Running = !this.Running;
            if (this.Running) {
                this.MainLoop();
            }
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        private void ProcessFile(string path) {
            TimedPlayer.Singleton.StopPlaying();
            //// var settingsImport = new SettingsImport();
            var bundle = this.MusicPort.ReadMusicFile(path, "test"); //// settingsImport
            if (bundle == null) {
                return;
            }

            CompactMidiBlock midiBlock = new CompactMidiBlock(bundle.Blocks[0]);
            midiBlock.Sequence("Test");
            midiBlock.EnqueueMidiEvents();
            //// midiBlock.CollectMidiEvents();
            TimedPlayer.Singleton.PlayBlock(midiBlock);
        }

        #endregion
    }
}
