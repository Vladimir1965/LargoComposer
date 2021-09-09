// <copyright file="TemplatesSavedHarmonic.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;

namespace EditorWindows
{
    using LargoSharedClasses.Music;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Saved Harmony Window.
    /// </summary>
    public partial class TemplatesSavedHarmonic
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
        /// Initializes a new instance of the <see cref="TemplatesSavedHarmonic"/> class.
        /// </summary>
        public TemplatesSavedHarmonic() {
            this.InitializeComponent();

            this.LoadComboboxes();
            this.RefreshGridStreams();

            if (this.ComboSources.Items.Count > 0) {
                this.ComboSources.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads the combo boxes.
        /// </summary>
        private void LoadComboboxes() {
            this.loading = true;

            var streams = PortAnalysis.Singleton.HarmonicStreamList;
            if (streams == null) {
                return;
            }

            var lengths = new List<int> { 0 };
            var items = (from s in streams orderby s?.Length select s?.Length ?? 0).Distinct();
            lengths.AddRange(items);
            this.ComboLength.ItemsSource = lengths;

            var names = (from b in streams orderby b?.FileName select b?.FileName).Distinct();
            var item = new ComboBoxItem { Content = string.Empty, Tag = 0, IsSelected = true };
            this.ComboSources.Items.Add(item);
            foreach (var name in names) {
                item = new ComboBoxItem { Content = name };
                this.ComboSources.Items.Add(item);
            }

            var dates = (from b in streams orderby b?.Header?.Origin.ToString() select b?.Header?.Origin.ToString()).Distinct();
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
        private void GridStreams_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridBlocks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridStreams_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var harmonicStream = this.GridStreams.SelectedItem as HarmonicStream;
            if (harmonicStream?.HarmonicBars != null) {
                this.DataContext = harmonicStream;
                this.GridBars.ItemsSource = harmonicStream.HarmonicBars;
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
            if (!(this.GridStreams.SelectedItem is HarmonicStream harmonicStream)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            //// DataObject data = new DataObject(typeof(ImageSource), image.Source);
            DataObject data = new DataObject("HarmonicStream", harmonicStream);
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
        private void RefreshGridStreams() {
            var length = (int)(this.ComboLength.SelectedItem ?? 0);
            
            var sourceItem = (ComboBoxItem)this.ComboSources.SelectedItem;
            var sourceName = sourceItem?.Content.ToString() ?? string.Empty;

            var dateItem = (ComboBoxItem)this.ComboDates.SelectedItem;
            var dateString = dateItem?.Content.ToString() ?? string.Empty;

            var streams = PortAnalysis.Singleton.HarmonicStreamList;
            if (streams == null) {
                return;
            }

            var list = (from b in streams
                        where (sourceName.Length == 0 || b.FileName == sourceName) 
                                 && (length == 0 || b.Length == length)
                                 && (dateString == string.Empty || b.Header.Origin.ToString() == dateString)
                        orderby b.Length, b.FileName
                        select b).ToList();

            this.GridStreams.ItemsSource = list;
            if (list.Count > 0) {
                this.GridStreams.SelectedIndex = 0;
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

            this.RefreshGridStreams();
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

            this.RefreshGridStreams();
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

            this.RefreshGridStreams();
        }
    }
}
