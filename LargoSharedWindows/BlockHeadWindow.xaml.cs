// <copyright file="BlockHeadWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows
{
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Support;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;

    /// <summary>
    /// Harmony Window.
    /// </summary>
    public partial class BlockHeadWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static BlockHeadWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockHeadWindow"/> class.
        /// </summary>
        public BlockHeadWindow()
        {
            Singleton = this;
            this.InitializeComponent();
            this.AllowDrop = true;
            this.Drop += this.DropImage;

            WindowManager.Singleton.LoadPosition(this);
            //// 2021/08 UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();

            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static BlockHeadWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<BlockHeadWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        #region Public methods

        /// <summary> Loads the data. </summary>
        public override void LoadData()
        {
        }

        /// <summary>
        /// Loads the block.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="givenFilePath">The given file path.</param>
        public void LoadBlock(MusicalBlock givenBlock, string givenFilePath) {
            this.Title = givenBlock.Header.FullName;
            this.EditorHeadPanel.LoadBlock(givenBlock, givenFilePath);
            this.BlockProperties.LoadBlock(givenBlock);
            this.GridTracks.ItemsSource = givenBlock.Strip.Lines;

            // this.TextBlockFile.Text = document.Header.FileName;
            // this.TextBlockFile.ToolTip = document.Header.FilePath;
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
        }

        #endregion

        /// <summary> Event handler. Called by GridTracks for selection changed events. </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e">      Selection changed event information. </param>
        private void GridTracks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
        }

        #region Drag-Drop
        /// <summary>
        /// Drops the image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropImage(object sender, DragEventArgs e) {
            bool exists = e.Data.GetDataPresent("MusicDocument");

            if (exists) {
                if (e.Data.GetData("MusicDocument") is MusicDocument musicDocument) {
                    var header = musicDocument.Header;
                    PortDocuments.Singleton.LoadBundle(header.FullName, false);
                    Console.Beep(990, 180);
                    e.Handled = true;
                }
            }
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
    }
}
