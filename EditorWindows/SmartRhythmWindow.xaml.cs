// <copyright file="SmartRhythmWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EditorWindows
{
    /// <summary>
    /// Interact logic pro SmartRhythmWindow
    /// </summary>
    public partial class SmartRhythmWindow
    {
        #region Fields
        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion 

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartRhythmWindow"/> class.
        /// </summary>
        public SmartRhythmWindow() {
            InitializeComponent();
            //// this.Test = 5;
            //// this.SliderContinuity.Value = 100;
            //// this.SliderImpulse.Value = 0;
            this.Structures = new List<RhythmicStructure>();
            this.Stream = new List<RhythmicStructure>();
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
        /// Gets or sets the rhythmic model.
        /// </summary>
        /// <value>
        /// The rhythmic model.
        /// </value>
        public RhythmicModel RhythmicModel { get; set; }
        
        /// <summary>
        /// Gets or sets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public List<RhythmicStructure> Structures { get; set; }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        public List<RhythmicStructure> Stream { get; set; }
        #endregion

        #region Public methods

        /// <summary>
        /// Picks the random rhythm.
        /// </summary>
        public void PickRandomRhythm() {
            var cnt = this.Structures.Count;
            for (int i = 0; i < 8; i++) {
                var idx = MathSupport.RandomNatural(cnt);
                var rstruct = this.Structures[idx];
                //// if (rstruct.Level > 0 || i > 10) {  break;  }
                this.Stream.Add(rstruct);
            }
        }

        /// <summary>
        /// Gets the rhythmic structures.
        /// </summary>
        /// <param name="numberOfTopRhythmicStructuresToSelect">The number of top rhythmic structures to select.</param>
        /// <param name="regularity">The regularity.</param>
        /// <param name="fromLevel">From level.</param>
        /// <param name="toLevel">To level.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public IList<RhythmicStructure> GetRhythmicStructures(int numberOfTopRhythmicStructuresToSelect, bool regularity, byte fromLevel, byte toLevel) {
            List<RhythmicStructure> list;
            if (regularity) {
                list = null; ////  this.RegularRhythmicStructures(2).ToList();
            }
            else {
                list = (from rs in this.Structures
                        where rs.FormalBehavior.Variance > 1 && rs.FormalBehavior.Variance < 99 && (rs.ZeroStart || rs.ToneLevel == 0)
                        select rs).ToList();
            }

            var resultList = (from rs in list
                              where rs.ToneLevel >= fromLevel && rs.ToneLevel <= toLevel
                              orderby rs.ToneLevel descending, rs.RhythmicBehavior.Filling descending, rs.FormalBehavior.Variance descending
                              select rs).Take(numberOfTopRhythmicStructuresToSelect).ToList();

            var distinctList = (list ?? throw new InvalidOperationException())
                                  .GroupBy(s => s.ElementSchema)
                                  .Select(g => g.First())
                                  .ToList();

            var sortedList = (from s in distinctList
                              orderby s.Level, s.ToneLevel
                              select s).ToList();

            var sortedStructs = (from str in list
                                 orderby str.Level ascending, str.ElementSchema descending //// .Mobility
                                 where str != null
                                 select str).Distinct().ToList();

            return null;
        }

        /// <summary>
        /// Oks the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Rebuild(object sender, System.Windows.RoutedEventArgs e) {
            if (this.RandomSelection.IsChecked ?? false) {
                this.PickRandomRhythm();
            }

            this.GridStream.ItemsSource = null;
            this.GridStream.ItemsSource = this.Stream;
        }

        /// <summary>
        /// Adds to stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddToStream(object sender, System.Windows.RoutedEventArgs e) {
            if (this.GridMaterial.SelectedItem is RhythmicStructure rstruct) {
                //// if (rstruct.Level > 0 || i > 10) {  break;  }
                this.Stream.Add(rstruct);
                this.GridStream.ItemsSource = null;
                this.GridStream.ItemsSource = this.Stream;
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
            if (!(this.GridMaterial.SelectedItem is RhythmicStructure rhythmicStructure)) {
                return;
            }

            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            DataObject data = new DataObject("RhythmicStructure", rhythmicStructure);
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
            var rhythmicStream = new RhythmicStream();
            foreach (var rhythmicStructure in this.Stream) {
                rhythmicStream.Structures.Add(rhythmicStructure);
            }

            DataObject data = new DataObject("RhythmicStream", rhythmicStream);
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

        #region Private methods

        /// <summary>
        /// Builds the material.
        /// </summary>
        private void BuildMaterial() {
            this.GridMaterial.ItemsSource = null;
            this.Structures = new List<RhythmicStructure>();
            if (this.RegularStructures.IsChecked ?? false) {
                var list = RhythmicStructureFactory.RegularStructures(this.Block.Header.System.RhythmicOrder);
                this.Structures.AddRange(list);
            }

            if (this.BlockStructures.IsChecked ?? false) {
                var list = this.RhythmicModel.RhythmicStructuresOfMotives;
                this.Structures.AddRange(list);
            }

            this.GridMaterial.ItemsSource = this.Structures;
        }

        /// <summary>
        /// Handles the Checked event of the RegularStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RegularStructures_Checked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }

        /// <summary>
        /// Handles the Unchecked event of the RegularStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RegularStructures_Unchecked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }

        /// <summary>
        /// Handles the Unchecked event of the BlockStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BlockStructures_Unchecked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }

        /// <summary>
        /// Handles the Checked event of the BlockStructures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BlockStructures_Checked(object sender, System.Windows.RoutedEventArgs e) {
            this.BuildMaterial();
        }
        #endregion
    }
}
