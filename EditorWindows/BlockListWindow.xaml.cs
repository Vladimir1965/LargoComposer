// <copyright file="BlockListWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorWindows
{
    using LargoSharedClasses.Support;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Detail Window.
    /// </summary>
    public partial class BlockListWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static BlockListWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockListWindow"/> class.
        /// </summary>
        public BlockListWindow()
        {
            Singleton = this;
            this.InitializeComponent();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static BlockListWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<BlockListWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the drag start point.
        /// </summary>
        /// <value>
        /// The drag start point.
        /// </value>
        public Point DragStartPoint { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            //// this.LoadBlocks();
        }

        /// <summary>
        /// Loads the blocks.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        private void LoadBlocks(string givenPath) {
            this.GridBlocks.ItemsSource = null;

            if (PortDocuments.Singleton.BlockDocumentMaster.DocumentList == null) {
                PortDocuments.Singleton.BlockDocumentMaster.LoadDocuments(givenPath, "UserListDocuments.xml", "Documents");
            }

            var list = PortDocuments.Singleton.BlockDocumentMaster.DocumentList;

            this.GridBlocks.ItemsSource = list;
            if (list?.Count > 0) {
                this.GridBlocks.SelectedIndex = 0;
            }
        }
        #endregion

        /// <summary>
        /// Handles the MouseDoubleClick event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }

        /// <summary>
        /// Handles the UnloadingRow event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.DataGridRowEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_UnloadingRow(object sender, DataGridRowEventArgs e) {
        }

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //// this.DataGridRhyBars.Focus(); //// To select row before dragging ?!?
            var musicDocument = this.GridBlocks.SelectedItem as MusicDocument;
            if (musicDocument == null) {
                return;
            }

            if (!PortDocuments.Singleton.LoadDocument(musicDocument, true)) { 
                return;
            }

            if (PortDocuments.Singleton.MusicalBlock == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("MusicalBlock", PortDocuments.Singleton.MusicalBlock);
            DragDrop.DoDragDrop(image, data, DragDropEffects.All);
        }

        /// <summary>
        /// Handles the MouseMove event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void List_MouseMove(object sender, MouseEventArgs e) {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = this.DragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
            }
        }
        #endregion
    }
}
