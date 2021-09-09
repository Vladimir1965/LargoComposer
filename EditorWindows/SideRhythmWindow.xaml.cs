// <copyright file="SideRhythmWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels;

namespace EditorWindows
{
    using LargoSharedClasses.Rhythm;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Rhythm Window.
    /// </summary>
    public partial class SideRhythmWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideRhythmWindow singleton;

        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideRhythmWindow"/> class.
        /// </summary>
        public SideRhythmWindow() {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            EditorSettings.Singleton.SidePanels.PanelOpen("SideRhythm");

            this.RefreshGridRhythmicFaces();
            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideRhythmWindow Singleton {
            get {
                Contract.Ensures(Contract.Result<SideRhythmWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        /// <summary> Refresh grid rhythmic faces. </summary>
        private void RefreshGridRhythmicFaces() {
            if (this.DataGridRhythmicFaces == null) {
                return;
            }

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            this.DataGridRhythmicFaces.ItemsSource = PortCatalogs.RhythmicFaces(path);
            if (this.DataGridRhythmicFaces.Items.Count > 0) {
                this.DataGridRhythmicFaces.SelectedIndex = 0;
            }
        }

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            var contextType = image.DataContext.GetType(); //// .ToString() LargoSharedClasses.Music.RhythmicFace
            if (contextType == typeof(RhythmicFace) && this.DataGridRhythmicFaces.SelectedItem is RhythmicFace rhythmicFace) {
                DataObject data = new DataObject("RhythmicFace", rhythmicFace);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void List_MouseMove(object sender, MouseEventArgs e) {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = this.startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
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
            WindowManager.Singleton.SavePosition(this);
            EditorSettings.Singleton.SidePanels.PanelClose("SideRhythm");
        }

        #endregion
    }
}
