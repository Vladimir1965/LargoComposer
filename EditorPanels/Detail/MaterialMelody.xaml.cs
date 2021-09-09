// <copyright file="MaterialMelody.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;

namespace EditorPanels.Detail {
    /// <summary>
    /// PanelMelodic Motive.
    /// </summary>
    public sealed partial class MaterialMelody {
        #region Fields
        /// <summary>
        /// Musical Block Model.
        /// </summary>
        private MelodicModel model;

        /// <summary>
        /// Melodic Material.
        /// </summary>
        private MelodicMaterial melodicMaterial;
       #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialMelody"/> class.
        /// </summary>
        public MaterialMelody() {
            //// PanelManager.DetailMelodicMotives = this;
            this.InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Sets the musical block model.
        /// </summary>
        /// <value>
        /// The musical block model.
        /// </value>
        public MelodicModel MelodicModel
        {
            set {
                this.model = value;

                if (this.model == null) {
                    return;
                }

                this.MelodicMaterial = this.model.ExtractMelodicMaterial();
            }
        }

        /// <summary>
        /// Gets or sets the drag start point.
        /// </summary>
        /// <value>
        /// The drag start point.
        /// </value>
        public Point DragStartPoint { get; set; }

        /// <summary>
        /// Gets or sets TMelodic Motive.
        /// </summary>
        /// <value> Property description. </value>
        private MelodicMaterial MelodicMaterial {
            get => this.melodicMaterial;

            set {
                this.melodicMaterial = value;
                if (this.melodicMaterial == null)
                {
                    return;
                }

                this.DataContext = this.melodicMaterial;
                this.RebindMelodicStructures();
            }
        }

        #endregion

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(MelodicModel givenModel)
        {
            base.LoadData();
            this.MelodicModel = givenModel; //// MenuEventSender.Singleton.LastSelectedMusicalBlockModel;
            //// this.PanelGroup.EventSender.TemplateChanged += this.MusicalTemplateChanged;
        }

        /// <summary>
        /// Rebind Melodic Motive Structures.
        /// </summary>
        private void RebindMelodicStructures() {
            // s.GetStructuralCode.Length
            var structures = from s in this.MelodicMaterial.Structures
                             orderby s.Occurrence descending, s.GSystem.Degree ascending, s.GSystem.Order ascending
                             select s;
            this.DataGridMelStructures.ItemsSource = null;
            this.DataGridMelStructures.ItemsSource = structures; ////.ToList();
            this.DataGridMelStructures.Items.Refresh();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the DataGridMelStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridMelStructures_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            e.Handled = true;
        }

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var melodicStructure = this.DataGridMelStructures.SelectedItem as MelodicStructure;
            if (melodicStructure == null) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.DragStartPoint = e.GetPosition(null);
            DataObject data = new DataObject("MelodicStructure", melodicStructure);
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