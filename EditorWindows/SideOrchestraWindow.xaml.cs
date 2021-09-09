// <copyright file="SideOrchestraWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorWindows
{
    using LargoSharedClasses.Orchestra;
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Saved Harmony Window.
    /// </summary>
    public partial class SideOrchestraWindow
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
        /// Initializes a new instance of the <see cref="SideOrchestraWindow"/> class.
        /// </summary>
        public SideOrchestraWindow() {
            this.InitializeComponent();

            this.LoadComboboxes();
            this.RefreshGridOrchestra();

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the combo boxes.
        /// </summary>
        private void LoadComboboxes() {
            this.loading = true;

            var essence = PortCatalogs.Singleton.OrchestraEssence;
            if (essence == null) {
                return;
            }

            var lengths = new List<int> { 0 };
            var items = (from s in essence orderby s?.Count select s?.Count ?? 0).Distinct();
            lengths.AddRange(items);
            this.ComboLength.ItemsSource = lengths;

            var titles = (from b in essence orderby b?.Title select b?.Title).Distinct();
            var item = new ComboBoxItem { Content = string.Empty, Tag = 0, IsSelected = true };
            this.ComboSources.Items.Add(item);

            foreach (var title in titles) {
                item = new ComboBoxItem { Content = title };
                this.ComboSources.Items.Add(item);
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
        private void GridOrchestra_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridOrchestra_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var orchestraUnit = this.GridOrchestra.SelectedItem as OrchestraUnit;
            if (orchestraUnit?.ListVoices != null) {
                this.DataContext = orchestraUnit;
                var os = from s in orchestraUnit.ListVoices orderby s.Octave descending select s;
                this.GridVoices.ItemsSource = os;
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
            if (!(this.GridOrchestra.SelectedItem is OrchestraUnit orchestraUnit)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            //// DataObject data = new DataObject(typeof(ImageSource), image.Source);
            DataObject data = new DataObject("OrchestraUnit", orchestraUnit);
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
        private void RefreshGridOrchestra() {
            var length = (int)(this.ComboLength.SelectedItem ?? 0);

            var sourceItem = (ComboBoxItem)this.ComboSources.SelectedItem;
            var sourceName = sourceItem?.Content.ToString() ?? string.Empty;

            var essence = PortCatalogs.Singleton.OrchestraEssence;
            if (essence == null) {
                return;
            }

            var list = (from b in essence
                        where (sourceName.Length == 0 || b.Title == sourceName) 
                           && (length == 0 || b.Count == length)
                        orderby b.Count, b.Title
                        select b).ToList();

            this.GridOrchestra.ItemsSource = list;
            if (list.Count > 0) {
                this.GridOrchestra.SelectedIndex = 0;
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

            this.RefreshGridOrchestra();
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

            this.RefreshGridOrchestra();
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

            this.RefreshGridOrchestra();
        }
    }
}
