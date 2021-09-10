// <copyright file="SmartHarmonyWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Composer;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LargoSharedWindows
{
    /// <summary>
    /// Interact logic pro SmartHarmonyWindow.
    /// </summary>
    public partial class SmartHarmonyWindow
    {
        #region Fields
        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartHarmonyWindow"/> class.
        /// </summary>
        public SmartHarmonyWindow() {
            InitializeComponent();
            this.SliderContinuity.Value = 100;
            this.Material = new List<HarmonicStructure>();
            this.Stream = new List<HarmonicStructure>();

            SharedWindows.Singleton.SideHarmonicModality(null, null);
        }
        #endregion 

        #region Properties

        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        public MusicalBlock Block { get; set; }

        /// <summary>
        /// Gets or sets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public List<HarmonicStructure> Material { get; set; }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        public List<HarmonicStructure> Stream { get; set; }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        /// <summary>
        /// Oks the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Rebuild(object sender, System.Windows.RoutedEventArgs e) {
            var board = HarmonyBoard.Singleton;
            //// SideHarmonicStructuresWindow 
            board.HarmonicModality = SideHarmonicModalityWindow.Singleton.HarmonicModality;
            board.HarmonicStructures = this.Material;
            board.SelectedStructures = this.Material;

            this.Stream = new List<HarmonicStructure>();
            for (int i = 0; i < 8; i++) {
                //// int consonance = 100; //// (int)this.SliderConsonance.Value, 
                //// int potential = 100; //// (int)this.SliderPotential.Value;
                //// int continuity = 100; //// (int)this.SliderContinuity.Value, impulse = (int)this.SliderImpulse.Value;
                //// var newStruct = board.GetNextHarmonicStructure(consonance, potential, continuity, impulse);
                //// this.Stream.Add(newStruct);
            }

            this.GridStream.ItemsSource = this.Stream;
        }

        /*
        /// <summary>
        /// Builds the material.
        /// </summary>
        private void BuildMaterial() {
            this.GridMaterial.ItemsSource = null;
            this.Material = new List<HarmonicStructure>();
            if (this.ModalityStructures.IsChecked ?? false) {
                //// var harmonicModality = SideHarmonicStructuresWindow.Singleton.HarmonicModality;
                var harmonicStructures = SideHarmonicStructuresWindow.Singleton.ResultList;
                this.Material.AddRange(harmonicStructures);
            }

            this.GridMaterial.ItemsSource = this.Material;
        }

        /// <summary>
        /// Handles the Unchecked event of the ModalityStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ModalityStructures_Unchecked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }

        /// <summary>
        /// Handles the Checked event of the ModalityStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ModalityStructures_Checked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }
        */

        /// <summary>
        /// Adds to stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddToStream(object sender, RoutedEventArgs e) {
            /*
            if (this.GridMaterial.SelectedItem is HarmonicStructure hstruct) {
                //// if (rstruct.Level > 0 || i > 10) {  break;  }
                this.Stream.Add(hstruct);
                this.GridStream.ItemsSource = null;
                this.GridStream.ItemsSource = this.Stream;
            } */
        }

        /// <summary>
        /// Handles the Unchecked event of the ImportedStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportedStructures_Unchecked(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Handles the Unchecked event of the ImportedTextStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportedTextStructures_Unchecked(object sender, RoutedEventArgs e) {
        }
        #endregion

        #region Drag-drop
        /*
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(this.GridMaterial.SelectedItem is HarmonicStructure harmonicStructure)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
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
            Vector diff = this.startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
            }
        }*/

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the Stream control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Stream_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (this.Stream == null || this.Stream.Count == 0) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            //// DataObject data = new DataObject(typeof(ImageSource), image.Source);
            var harmonicStream = new HarmonicStream(this.Block.Header);
            foreach (var harmonicStructure in this.Stream) {
                var harmonicBar = new HarmonicBar(this.Block.Header, harmonicStructure);
                harmonicStream.HarmonicBars.Add(harmonicBar);
            }

            DataObject data = new DataObject("HarmonicStream", harmonicStream);
            DragDrop.DoDragDrop(image, data, DragDropEffects.All);
        }

        /// <summary>
        /// Handles the MouseMove event of the Stream control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Stream_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
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
        /// Handles the SelectionChanged event of the StreamType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void StreamType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}