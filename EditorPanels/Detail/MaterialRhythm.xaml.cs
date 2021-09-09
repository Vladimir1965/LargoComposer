// <copyright file="MaterialRhythm.xaml.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;

namespace EditorPanels.Detail
{
    /// <summary>
    /// PanelRhythmic Motive.
    /// </summary>
    public sealed partial class MaterialRhythm {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRhythm"/> class.
        /// </summary>
        public MaterialRhythm() {
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
        public List<RhythmicStructure> List { get; set; }

        /// <summary>
        /// Gets or sets the filtered list.
        /// </summary>
        /// <value>
        /// The filtered list.
        /// </value>
        public List<RhythmicStructure> ResultList { get; set; }

        #endregion

        /// <summary> Loads the data. </summary>
        public override void LoadData() {
            base.LoadData();
        }

        /// <summary>
        /// Loads the modal structures.
        /// </summary>
        /// <param name="givenModality">The given modality.</param>
        public void LoadModalStructures(RhythmicModality givenModality) {
            //// base.LoadData();
            /*
            var system = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, DefaultValue.RhythmicOrder);
            var q = new GeneralQualifier();
            var variety = StructuralVarietyFactory.NewRhythmicStructuralVariety(StructuralVarietyType.Instances, RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, system.Order), q, 20000);
            if (variety.StructList == null) {
                return;
            }
            */

            var rv = StructuralVarietyFactory.NewRhythmicStructModalVariety(
                StructuralVarietyType.FiguralSubstructuresOfModality,
                givenModality,
                null,
                10000);
            var list = rv.StructList.ToList<RhythmicStructure>();
            this.LoadList(list);
        }

        /// <summary>
        /// Loads the list.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        public void LoadList(List<RhythmicStructure> givenList) {
            //// this.List = givenList.OrderBy(x => x.Level).ThenBy(x => x.ElementSchema).ToList();
            this.List = givenList.OrderBy(x => x.Level).ThenByDescending(x => x.ToneLevel).ThenBy(x => x.RhythmicBehavior.Complexity).ToList();
            this.ResultList = this.List;

            //// var ordered = list.OrderBy(x => x.ClassCode + x.Shortcut);
            this.DataGridRhyBars.ItemsSource = null;
            this.DataGridRhyBars.ItemsSource = this.ResultList;
            this.DataGridRhyBars.Items.Refresh();
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(RhythmicModality givenModality) {
            if (this.List == null) {
                return;
            }

            var resultList = new List<RhythmicStructure>();
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
            this.DataGridRhyBars.ItemsSource = null;
            this.DataGridRhyBars.ItemsSource = resultList;
            this.DataGridRhyBars.Items.Refresh();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(RhythmicModel givenModel)
        {
            base.LoadData();
            var list = givenModel.GetGroupedRhythmicStructures();
            var ordered = list.OrderByDescending(x => x.Occurrence); //// x.CodeMark
            this.DataGridRhyBars.ItemsSource = null;
            this.DataGridRhyBars.ItemsSource = ordered;
            this.DataGridRhyBars.Items.Refresh();
        }

        #region RhyBars

        /// <summary>
        /// Handles the SelectionChanged event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridRhyBars_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            e.Handled = true;
        }
        #endregion

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //// this.DataGridRhyBars.Focus(); //// To select row before dragging ?!?
            var rhythmicStructure = this.DataGridRhyBars.SelectedItem as RhythmicStructure;
            if (rhythmicStructure == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("RhythmicStructure", rhythmicStructure);
            DragDrop.DoDragDrop(image, data, DragDropEffects.All);
        }

        /// <summary>
        /// Handles the MouseMove event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void List_MouseMove(object sender, MouseEventArgs e)
        {
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