// <copyright file="BlockHarmonyWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorWindows
{
    using LargoSharedClasses.Models;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Support;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;

    /// <summary>
    /// Detail Window.
    /// </summary>
    public partial class BlockHarmonyWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static BlockHarmonyWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockHarmonyWindow"/> class.
        /// </summary>
        public BlockHarmonyWindow()
        {
            Singleton = this;
            this.InitializeComponent();
            this.AllowDrop = true;
            this.Drop += this.DropImage;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static BlockHarmonyWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<BlockHarmonyWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(HarmonicModel givenModel)
        {
            this.PanelDetailMaterial.LoadData(givenModel);
            //// this.panelDetailMotives.LoadData(givenModel);
        }
        #endregion

        #region Drag-Drop
        /// <summary>
        /// Drops the image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropImage(object sender, DragEventArgs e) {
            var exists = e.Data.GetDataPresent("MusicalBlock");
            if (exists) {
                if (e.Data.GetData("MusicalBlock") is MusicalBlock musicalBlock) {
                    var harmonicModel = HarmonicModel.GetNewModel("Inner", musicalBlock);
                    this.PanelDetailMaterial.LoadData(harmonicModel);
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
