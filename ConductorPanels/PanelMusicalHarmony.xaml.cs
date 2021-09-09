// <copyright file="PanelMusicalHarmony.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Composer;
using LargoSharedClasses.Music;
using System;
using System.Windows;

namespace ConductorPanels
{
    /// <summary>
    /// Panel Musical Zone
    /// </summary>
    public sealed partial class PanelMusicalHarmony
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelMusicalHarmony"/> class.
        /// </summary>
        public PanelMusicalHarmony() {
            this.InitializeComponent();
            this.AllowDrop = true;
            this.Drop += this.DropImage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic modality.
        /// </summary>
        /// <value>
        /// The rhythmic modality.
        /// </value>
        public RhythmicModality RhythmicModality { get; set; }
        #endregion

        #region Load data
        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
        }
        #endregion

        /// <summary>
        /// Gens the next harmonic structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        public HarmonicStructure GenNextHarmonicStructure() {
            int? consonance = 100 - (int)this.Dissonance.Value;
            int? potential = null;
            int? continuity = 50;
            int? impulse = null;
            var newStruct = HarmonyBoard.Singleton.GetNextHarmonicStructure(consonance, potential, continuity, impulse);
            return newStruct;
        }

        #region Private methods
        #endregion

        #region Drag-Drop
        /// <summary>
        /// Drops the image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void DropImage(object sender, DragEventArgs e) {
            //// e.Data.GetDataPresent("RhythmicStructure") ||
            bool handled = false;
            if (e.Data.GetData("HarmonicModality") is HarmonicModality harmonicModality) {
                this.txtHarmonicModality.Text = harmonicModality.ToneSchema + " " + harmonicModality.DistanceSchema;
                this.txtHarmonicModality.ToolTip = harmonicModality.Name;
                this.HarmonicModality = harmonicModality;
                Console.Beep(990, 180);
                handled = true;
            }

            if (e.Data.GetData("RhythmicModality") is RhythmicModality rhythmicModality) {
                this.txtRhythmicModality.Text = rhythmicModality.ElementString(false) + " " + rhythmicModality.DistanceSchema;
                this.txtRhythmicModality.ToolTip = string.Empty;
                this.RhythmicModality = rhythmicModality;
                Console.Beep(990, 180);
                handled = true;
            }

            e.Handled = handled;
        }
        #endregion
    }
}