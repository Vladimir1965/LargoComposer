// <copyright file="MaterialHarmony.xaml.cs" company="Traced-Ideas, Czech republic">
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
    public sealed partial class MaterialHarmony
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialHarmony"/> class.
        /// </summary>
        public MaterialHarmony() {
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
            this.DefaultList = PortCatalogs.DefaultHarmonicStructures(2, 6);
            this.LoadList();
        }

        /// <summary>
        /// Filters the by modality.
        /// </summary>
        /// <param name="givenModality">The given modality.</param>
        public void FilterByModality(HarmonicModality givenModality) {
            this.HarmonicModality = givenModality;
            this.LoadList();
        }

        /// <summary>
        /// Filters the by level.
        /// </summary>
        /// <param name="givenLevelFrom">The given level from.</param>
        /// <param name="givenLevelTo">The given level to.</param>
        public void FilterByLevel(byte givenLevelFrom, byte givenLevelTo) {
            this.LevelFrom = givenLevelFrom;
            this.LevelTo = givenLevelTo;
            this.LoadList();
        }

        /// <summary>
        /// Filter by modality.
        /// </summary>
        /// <returns> Returns value. </returns>
        private List<HarmonicStructure> DefaultModalityList() {
            if (this.HarmonicModality == null) {
                return this.DefaultList;
            }

            var resultList = new List<HarmonicStructure>();
            foreach (var hs in this.DefaultList) {
                bool covered = true;
                for (byte j = 0; j < this.HarmonicModality.GSystem.Order; j++) {
                    if (hs.IsOn(j) && this.HarmonicModality.IsOff(j)) {
                        covered = false;
                        break;
                    }
                }

                if (covered) {
                    resultList.Add(hs);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Loads the list.
        /// </summary>
        private void LoadList() {
            var list = this.DefaultModalityList();
            if (list == null) {
                return;
            }

            var sortedList = (from r in list
                              where r.Level >= this.LevelFrom && r.Level <= this.LevelTo
                              orderby r.Level, r.Shortcut //// x.ClassCode + x.Shortcut
                              select r).ToList();
            this.ResultList = sortedList;

            this.DataGridHarBars.ItemsSource = null;
            this.DataGridHarBars.ItemsSource = this.ResultList;
            this.DataGridHarBars.Items.Refresh();
        }

        /*
        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(HarmonicModel givenModel) {
            base.LoadData();
            var harmonicMaterial = givenModel.ExtractHarmonicMaterial();

            var structures = from s in harmonicMaterial.Structures
                             orderby s.Occurrence descending, s.GetStructuralCode.Length descending
                             select s;
            this.DataGridHarBars.ItemsSource = null;
            this.DataGridHarBars.ItemsSource = structures; ////.ToList();
            this.DataGridHarBars.Items.Refresh();
        }*/

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
            var harmonicStructure = this.DataGridHarBars.SelectedItem as HarmonicStructure;
            if (harmonicStructure == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("HarmonicStructure", harmonicStructure);
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