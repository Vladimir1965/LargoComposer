// <copyright file="MaterialHarmonicBars.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;

namespace EditorPanels.Detail
{
    /// <summary>
    /// PanelHarmonic Motive.
    /// </summary>
    public sealed partial class MaterialHarmonicBars
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialHarmonicBars"/> class.
        /// </summary>
        public MaterialHarmonicBars() {
            this.InitializeComponent();
            this.LevelFrom = 0;
            this.LevelTo = 0;
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

        /// <summary> Gets or sets a list of ordered. </summary>
        /// <value> A list of ordered. </value>
        public List<HarmonicStructure> DefaultList { get; set; }

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }

        /// <summary>
        /// Gets or sets the level from.
        /// </summary>
        /// <value>
        /// The level from.
        /// </value>
        public byte LevelFrom { get; set; }

        /// <summary>
        /// Gets or sets the level to.
        /// </summary>
        /// <value>
        /// The level to.
        /// </value>
        public byte LevelTo { get; set; }

        /// <summary>
        /// Gets or sets the filtered list.
        /// </summary>
        /// <value>
        /// The filtered list.
        /// </value>
        public List<HarmonicStructure> ResultList { get; set; }

        #endregion

        /// <summary> Loads the data. </summary>
        public override void LoadData() {
            base.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(HarmonicModel givenModel) {
            base.LoadData();
            var harmonicBars = givenModel.GetGroupedHarmonicBars(); //// HarmonicBars;

            var structures = from s in harmonicBars
                             orderby s.Occurrence descending
                             select s; 
            this.DataGridHarBars.ItemsSource = null;
            this.DataGridHarBars.ItemsSource = structures; ////.ToList();
            this.DataGridHarBars.Items.Refresh();
        }

        #region Harmonic Bars

        /// <summary>
        /// Handles the Selection Changed event of the Data Grid Harmonic Bars control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridHarBars_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            e.Handled = true;
        }
        #endregion

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var harmonicBar = this.DataGridHarBars.SelectedItem as HarmonicBar;
            if (harmonicBar == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("HarmonicBar", harmonicBar);
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