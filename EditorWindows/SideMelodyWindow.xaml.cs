// <copyright file="SideMelodyWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels;

namespace EditorWindows
{
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Harmony Window.
    /// </summary>
    public partial class SideMelodyWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideMelodyWindow singleton;

        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideMelodyWindow"/> class.
        /// </summary>
        public SideMelodyWindow()
        {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            EditorSettings.Singleton.SidePanels.PanelOpen("SideMelody");

            //// this.Localize();
            this.RefreshGridMelodicType();
            this.RefreshGridMelodicShape();
            this.RefreshGridMelodicFaces();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideMelodyWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<SideMelodyWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        /*
        /// <summary>
        /// Sets a value indicating whether this instance is music editor.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is music editor; otherwise, <c>false</c>.
        /// </value>
        public bool IsMusicEditor {
            set => this.TabControl.SelectedIndex = value ? 1 : 0;
        }*/

        /// <summary> Refresh grid melodic type. </summary>
        private void RefreshGridMelodicType() {
            if (this.DataGridMelodicType == null) {
                return;
            }

            this.DataGridMelodicType.ItemsSource = DataEnums.ListMelodicFunction;

            if (this.DataGridMelodicType.Items.Count > 0) {
                this.DataGridMelodicType.SelectedIndex = 0;
            }
        }

        /// <summary> Refresh grid melodic shape. </summary>
        private void RefreshGridMelodicShape() {
            if (this.DataGridMelodicShape == null) {
                return;
            }

            this.DataGridMelodicShape.ItemsSource = DataEnums.ListMelodicShape;

            if (this.DataGridMelodicShape.Items.Count > 0) {
                this.DataGridMelodicShape.SelectedIndex = 0;
            }
        }

        /// <summary> Refresh grid melodic faces. </summary>
        private void RefreshGridMelodicFaces() {
            if (this.DataGridMelodicFaces == null) {
                return;
            }

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            this.DataGridMelodicFaces.ItemsSource = PortCatalogs.MelodicFaces(path);
            if (this.DataGridMelodicFaces.Items.Count > 0) {
                this.DataGridMelodicFaces.SelectedIndex = 0;
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
            if (contextType == typeof(KeyValuePair) && image.Tag.ToString() == "MelodicFunction" && this.DataGridMelodicType.SelectedItem is KeyValuePair function) {
                DataObject data = new DataObject("MelodicFunction", (MelodicFunction)function.NumericKey);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }

            if (contextType == typeof(KeyValuePair) && image.Tag.ToString() == "MelodicShape" && this.DataGridMelodicShape.SelectedItem is KeyValuePair shape) {
                DataObject data = new DataObject("MelodicShape", (MelodicShape)shape.NumericKey);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }

            if (contextType == typeof(MelodicFace) && this.DataGridMelodicFaces.SelectedItem is MelodicFace mface) {
                DataObject data = new DataObject("MelodicFace", mface);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                //// return;
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
            EditorSettings.Singleton.SidePanels.PanelClose("SideMelody");
        }

        #endregion
    }
}
