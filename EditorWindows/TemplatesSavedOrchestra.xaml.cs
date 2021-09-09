// <copyright file="TemplatesSavedOrchestra.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;

namespace EditorWindows
{
    using LargoSharedClasses.Orchestra;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Orchestration Window.
    /// </summary>
    public partial class TemplatesSavedOrchestra
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
        /// Initializes a new instance of the <see cref="TemplatesSavedOrchestra"/> class.
        /// </summary>
        public TemplatesSavedOrchestra() {
            this.InitializeComponent();

            this.LoadComboboxes();
            this.RefreshGridBlocks();

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the combo boxes.
        /// </summary>
        private void LoadComboboxes() {
            this.loading = true;
            var item = new ComboBoxItem { Content = string.Empty, Tag = 0 };
            this.ComboSections.Items.Add(item);

            for (int s = 1; s <= 6; s++) {
                item = new ComboBoxItem { Content = s.ToString(), Tag = s, IsSelected = s == 2 };
                this.ComboSections.Items.Add(item);
            }

            item = new ComboBoxItem { Content = string.Empty, Tag = 0 };
            this.ComboInstruments.Items.Add(item);
            for (int i = 1; i <= 20; i++) {
                item = new ComboBoxItem { Content = i.ToString(), Tag = i, IsSelected = i == 5 };
                this.ComboInstruments.Items.Add(item);
            }

            var blocks = PortAnalysis.Singleton.OrchestraBlockList;
            if (blocks == null) {
                return;
            }

            var names = (from b in blocks orderby b?.FileName select b?.FileName).Distinct();
            item = new ComboBoxItem { Content = string.Empty, Tag = 0, IsSelected = true };
            this.ComboSources.Items.Add(item);
            foreach (var name in names) {
                item = new ComboBoxItem { Content = name };
                this.ComboSources.Items.Add(item);
            }

            var dates = (from b in blocks orderby b?.Header?.Origin.ToString() select b?.Header?.Origin.ToString()).Distinct();
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
        private void GridBlocks_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridBlocks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var orchestraBlock = this.GridBlocks.SelectedItem as OrchestraBlock;
            if (orchestraBlock?.Strip?.OrchestraVoices != null) {
                this.DataGridTracks.ItemsSource = orchestraBlock.Strip.OrchestraVoices;
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
            if (!(this.GridBlocks.SelectedItem is OrchestraBlock orchestraBlock)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            //// DataObject data = new DataObject(typeof(ImageSource), image.Source);
            DataObject data = new DataObject("OrchestraBlock", orchestraBlock);
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
        private void RefreshGridBlocks() {
            var sectionItem = (ComboBoxItem)this.ComboSections.SelectedItem;
            var sectionCount = (int)(sectionItem?.Tag ?? 1);

            var instrumentItem = (ComboBoxItem)this.ComboInstruments.SelectedItem;
            var instrumentCount = (int)(instrumentItem?.Tag ?? 1);

            var sourceItem = (ComboBoxItem)this.ComboSources.SelectedItem;
            var sourceName = sourceItem?.Content.ToString() ?? string.Empty;

            var dateItem = (ComboBoxItem)this.ComboDates.SelectedItem;
            var dateString = dateItem?.Content.ToString() ?? string.Empty;

            var blocks = PortAnalysis.Singleton.OrchestraBlockList;
            if (blocks == null) {
                return;
            }

            var list = (from b in blocks
                        where
                            (sourceName.Length == 0 || b.FileName == sourceName)
                            && (sectionCount == 0 || b.SectionCount == sectionCount)
                            && (instrumentCount == 0 || b.InstrumentCount == instrumentCount)
                            && (dateString == string.Empty || b.Header.Origin.ToString() == dateString)
                        orderby b.SectionCount, b.InstrumentCount, b.TrackCount, b.FileName
                        select b).ToList();

            this.GridBlocks.ItemsSource = list;
            if (list.Count > 0) {
                this.GridBlocks.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboSections control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboSections_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.loading) {
                return;
            }

            this.RefreshGridBlocks();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboInstruments control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboInstruments_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.loading) {
                return;
            }

            this.RefreshGridBlocks();
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

            if (this.ComboSections.Items.Count > 0) {
                this.ComboSections.SelectedIndex = 0;
            }

            if (this.ComboInstruments.Items.Count > 0) {
                this.ComboInstruments.SelectedIndex = 0;
            }

            if (this.ComboDates.Items.Count > 0) {
                this.ComboDates.SelectedIndex = 0;
            }

            this.RefreshGridBlocks();
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

            if (this.ComboSections.Items.Count > 0) {
                this.ComboSections.SelectedIndex = 0;
            }

            if (this.ComboInstruments.Items.Count > 0) {
                this.ComboInstruments.SelectedIndex = 0;
            }

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }

            this.RefreshGridBlocks();
        }
    }
}
