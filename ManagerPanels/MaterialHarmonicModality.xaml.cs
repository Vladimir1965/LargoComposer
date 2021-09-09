// <copyright file="MaterialHarmonicModality.xaml.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Support;

namespace ManagerPanels
{
    /// <summary>
    /// PanelHarmonic Motive.
    /// </summary>
    public sealed partial class MaterialHarmonicModality
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialHarmonicModality"/> class.
        /// </summary>
        public MaterialHarmonicModality() {
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
        public List<HarmonicModality> List { get; set; }

        /// <summary>
        /// Gets or sets the filtered list.
        /// </summary>
        /// <value>
        /// The filtered list.
        /// </value>
        public List<HarmonicModality> ResultList { get; set; }

        #endregion

        /// <summary> Loads the data. </summary>
        public override void LoadData() {
            base.LoadData();
            var system = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            var structures = PortCatalogs.Singleton.HarmonicEssence;
            if (structures == null) {
                return;
            }

            var modalities = (from ms in structures where ms.ModalityName.Length > 0 && ms.Level >= 5 && ms.Level <= 7 select ms).ToList();
            var list = new List<HarmonicModality>();
            foreach (var modality in modalities) {
                var hs = new HarmonicModality(system, modality.StructuralCode) {
                    Name = modality.ModalityName,
                    //// Shortcut = !string.IsNullOrWhiteSpace(chord.Shortcut) ? chord.Shortcut : "X",                    
                    ClassCode = modality.ClassNumber.ToString(),
                    Number = modality.Number,
                    DecimalNumber = modality.Number
                };
                list.Add(hs);
            }

            this.List = list.OrderBy(x => x.Level).ThenBy(x => x.ToneSchema).ToList();
            this.ResultList = this.List;

            //// var ordered = list.OrderBy(x => x.ClassCode + x.Shortcut);
            this.DataGridMaterial.ItemsSource = null;
            this.DataGridMaterial.ItemsSource = this.ResultList;
            this.DataGridMaterial.Items.Refresh();
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(HarmonicModality givenModality) {
            if (this.List == null) {
                return;
            }

            var resultList = new List<HarmonicModality>();
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

        #region Harmonic Modalities

        /// <summary>
        /// Selected modality.
        /// </summary>
        /// <returns> Returns value. </returns>
        public HarmonicModality SelectedModality() {
            var item = this.DataGridMaterial.SelectedItem as HarmonicModality;
            if (item == null) {
                return null;
            }

            return item;
        }

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
            var harmonicModality = this.DataGridMaterial.SelectedItem as HarmonicModality;
            if (harmonicModality == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("HarmonicModality", harmonicModality);
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