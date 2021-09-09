// <copyright file="SideInstrumentWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EditorPanels;

namespace EditorWindows
{
    /// <summary>
    /// Interaction logic for Tools Window.
    /// </summary>
    public partial class SideInstrumentWindow
    {
        #region Fields
        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion

        /// <summary> Initializes a new instance of the <see cref="SideInstrumentWindow" /> class. </summary>
        public SideInstrumentWindow() {
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            EditorSettings.Singleton.SidePanels.PanelOpen("SideInstrument");
            //// this.Localize();

            this.RefreshGridOctave();
            this.RefreshGridLoudness();
            this.ComboType_SelectionChanged(null, null);
        }

        /// <summary> Refresh grid octave. </summary>
        private void RefreshGridOctave() {
            if (this.DataGridOctave == null) {
                return;
            }

            this.DataGridOctave.ItemsSource = DataEnums.ListMusicalOctave;

            if (this.DataGridOctave.Items.Count > 0) {
                this.DataGridOctave.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Refreshes the grid loudness.
        /// </summary>
        private void RefreshGridLoudness() {
            if (this.DataGridLoudness == null) {
                return;
            }

            this.DataGridLoudness.ItemsSource = DataEnums.ListMusicalLoudness;

            if (this.DataGridLoudness.Items.Count > 0) {
                this.DataGridLoudness.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Refreshes the grid instruments.
        /// </summary>
        private void RefreshGridInstruments() {
            if (this.DataGridInstruments == null) {
                return;
            }

            var typeItem = (ComboBoxItem)this.ComboType.SelectedItem;
            var typeTag = (typeItem?.Tag ?? "1").ToString();
            if (typeTag == "1") {
                this.DataGridInstruments.ItemsSource = PortInstruments.MelodicInstruments;
            }
            else {
                this.DataGridInstruments.ItemsSource = PortInstruments.RhythmicInstruments;
            }

            if (this.DataGridInstruments.Items.Count > 0) {
                this.DataGridInstruments.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.RefreshGridInstruments();
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
            if (contextType == typeof(KeyValuePair) && image.Tag.ToString() == "Octave" && this.DataGridOctave.SelectedItem is KeyValuePair octave) {
                DataObject data = new DataObject("MusicalOctave", (MusicalOctave)octave.NumericKey);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }

            if (contextType == typeof(KeyValuePair) && image.Tag.ToString() == "Loudness" && this.DataGridLoudness.SelectedItem is KeyValuePair loudness) {
                DataObject data = new DataObject("MusicalLoudness", (MusicalLoudness)loudness.NumericKey);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }

            if (contextType == typeof(MelodicInstrument) && this.DataGridInstruments.SelectedItem is MelodicInstrument mi) {
                DataObject data = new DataObject("MelodicInstrument", mi);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }

            if (contextType == typeof(RhythmicInstrument) && this.DataGridInstruments.SelectedItem is RhythmicInstrument ri) {
                DataObject data = new DataObject("RhythmicInstrument", ri);
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
            EditorSettings.Singleton.SidePanels.PanelClose("SideInstrument");
        }

        #endregion
    }
}
