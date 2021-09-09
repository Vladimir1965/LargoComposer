// <copyright file="MaterialRhythmicModality.xaml.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Support;

namespace ConductorPanels
{
    /// <summary>
    /// PanelRhythmic Motive.
    /// </summary>
    public sealed partial class MaterialRhythmicModality
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRhythmicModality" /> class.
        /// </summary>
        public MaterialRhythmicModality() {
            this.InitializeComponent();
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
        public List<RhythmicModality> List { get; set; }

        /// <summary>
        /// Gets or sets the filtered list.
        /// </summary>
        /// <value>
        /// The filtered list.
        /// </value>
        public List<RhythmicModality> ResultList { get; set; }

        #endregion

        /// <summary> Loads the data. </summary>
        public override void LoadData() {
            base.LoadData();
            var system = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, DefaultValue.RhythmicOrder);
            var structures = PortCatalogs.Singleton.RhythmicEssence;
            if (structures == null) {
                return;
            }

            var modalities = (from ms in structures where ms.Level >= 0 && ms.Level <= 12 select ms).ToList();
            var list = new List<RhythmicModality>();
            foreach (var modality in modalities) {
                list.Add(modality);
            }

            this.List = list.OrderBy(x => x.Level).ThenBy(x => x.ElementSchema).ToList();
            this.ResultList = this.List;

            //// var ordered = list.OrderBy(x => x.ClassCode + x.Shortcut);
            this.DataGridMaterial.ItemsSource = null;
            this.DataGridMaterial.ItemsSource = this.ResultList;
            this.DataGridMaterial.Items.Refresh();
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(RhythmicModality givenModality) {
            if (this.List == null) {
                return;
            }

            var resultList = new List<RhythmicModality>();
            foreach (var hs in this.List) {
                bool covered = true;
                for (byte j = 0; j < givenModality.GSystem.Order; j++) {
                    if (hs.IsOn(j) && givenModality.IsOff(j)) {
                        covered = false;
                        break;
                    }
                }

                if (covered) {
                    resultList.Add(hs);
                }
            }

            this.ResultList = resultList;
            this.DataGridMaterial.ItemsSource = null;
            this.DataGridMaterial.ItemsSource = resultList;
            this.DataGridMaterial.Items.Refresh();
        }

        #region Rhythmic Modalities

        /// <summary>
        /// Handles the Selection Changed event of the Data Grid Rhythmic Bars control.
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
            var rhythmicModality = this.DataGridMaterial.SelectedItem as RhythmicModality;
            if (rhythmicModality == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("RhythmicModality", rhythmicModality);
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