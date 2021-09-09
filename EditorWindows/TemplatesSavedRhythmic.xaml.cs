// <copyright file="TemplatesSavedRhythmic.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;

namespace EditorWindows
{
    using LargoSharedClasses.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Saved Harmony Window.
    /// </summary>
    public partial class TemplatesSavedRhythmic
    {
        #region Fields
        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// The loading
        /// </summary>
        private bool loading;
        #endregion 

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesSavedRhythmic"/> class.
        /// </summary>
        public TemplatesSavedRhythmic() {
            this.InitializeComponent();

            this.LoadComboboxes();
            this.RefreshGridMaterials();

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the combo boxes.
        /// </summary>
        private void LoadComboboxes() {
            this.loading = true;

            var materials = PortAnalysis.Singleton.RhythmicMaterialList;
            if (materials == null) {
                return;
            }

            var lengths = new List<int> { 0 };
            var items = (from s in materials orderby s?.Count select s?.Count ?? 0).Distinct();
            lengths.AddRange(items);
            this.ComboLength.ItemsSource = lengths; 

            var names = (from b in materials orderby b?.FileName select b?.FileName).Distinct();
            var item = new ComboBoxItem { Content = string.Empty, Tag = 0, IsSelected = true };
            this.ComboSources.Items.Add(item);

            foreach (var name in names) {
                item = new ComboBoxItem { Content = name };
                this.ComboSources.Items.Add(item);
            }

            var dates = (from b in materials orderby b?.Header?.Origin.ToString() select b?.Header?.Origin.ToString()).Distinct();
            item = new ComboBoxItem { Content = string.Empty, Tag = 0, IsSelected = true };
            this.ComboDates.Items.Add(item);
            foreach (var date in dates) {
                item = new ComboBoxItem { Content = date };
                this.ComboDates.Items.Add(item);
            }

            this.loading = false;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Handles the MouseDoubleClick event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void GridMaterials_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridMaterials_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var rhythmicMaterial = this.GridMaterials.SelectedItem as RhythmicMaterial;
            if (rhythmicMaterial?.Structures != null) {
                this.DataContext = rhythmicMaterial;
                var os = from s in rhythmicMaterial.Structures orderby s.ToneLevel, s.Level select s;
                this.GridStructures.ItemsSource = os;
            }
        }

        #endregion

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(this.GridMaterials.SelectedItem is RhythmicMaterial rhythmicMaterial)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            //// DataObject data = new DataObject(typeof(ImageSource), image.Source);
            DataObject data = new DataObject("RhythmicMaterial", rhythmicMaterial);
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
            Vector diff = this.startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
            }
        }
        #endregion

        /// <summary>
        /// Refreshes the grid blocks.
        /// </summary>
        private void RefreshGridMaterials() {
            var length = (int)(this.ComboLength.SelectedItem ?? 0);

            var sourceItem = (ComboBoxItem)this.ComboSources.SelectedItem;
            var sourceName = sourceItem?.Content.ToString() ?? string.Empty;

            var dateItem = (ComboBoxItem)this.ComboDates.SelectedItem;
            var dateString = dateItem?.Content.ToString() ?? string.Empty;

            var materials = PortAnalysis.Singleton.RhythmicMaterialList;
            if (materials == null) {
                return;
            }

            var list = (from b in materials
                        where (sourceName.Length == 0 || b.FileName == sourceName) 
                           && (length == 0 || b.Count == length)
                           && (dateString == string.Empty || b.Header.Origin.ToString() == dateString)
                        orderby b.Count, b.FileName
                        select b).ToList();

            this.GridMaterials.ItemsSource = list;
            if (list.Count > 0) {
                this.GridMaterials.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboSections control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboLength_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.loading) {
                return;
            }

            this.RefreshGridMaterials();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboSources control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboSources_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.loading) {
                return;
            }

            if (this.ComboLength.Items.Count > 0) {
                this.ComboLength.SelectedIndex = 0;
            }

            if (this.ComboDates.Items.Count > 0) {
                this.ComboDates.SelectedIndex = 0;
            }

            this.RefreshGridMaterials();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboDates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboDates_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.loading) {
                return;
            }

            if (this.ComboLength.Items.Count > 0) {
                this.ComboLength.SelectedIndex = 0;
            }

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }

            this.RefreshGridMaterials();
        }
    }
}
