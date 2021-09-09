// <copyright file="PanelMusicalZone.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Support;
using System;
using System.Linq;
using System.Windows;

namespace ConductorPanels
{
    /// <summary>
    /// Panel Musical Zone
    /// </summary>
    public sealed partial class PanelMusicalZone
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelMusicalZone" /> class.
        /// </summary>
        public PanelMusicalZone() {
            this.InitializeComponent();
            //// this.zone = new MusicalZone(24, 1, 1, 1, 1, string.Empty, null);
            this.AllowDrop = true;
            this.Drop += this.DropImage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Intervals.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalZone Zone { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads the panel.
        /// </summary>
        /// <param name="givenZone">The given zone.</param>
        public void LoadPanel(MusicalZone givenZone) {
            this.Zone = givenZone;
            this.ComboOrchestra.ItemsSource = PortCatalogs.Singleton.OrchestraEssence;
            this.sliderVolume.Value = 100.0f * (int)this.Zone.Loudness / (int)MusicalLoudness.MaxLoudness;
            this.controlMobility.PlanFunction = this.Zone.MobilityPlanFunction;
            this.ComboOrchestra.SelectedItem = this.Zone.Orchestra;

            var harmonicModality = this.Zone.HarmonicModality;
            if (harmonicModality != null) {
                this.txtHarmonicModality.Text = harmonicModality.ToneSchema + " " + harmonicModality.DistanceSchema;
                this.txtHarmonicModality.ToolTip = harmonicModality.Name;
            }

            var rhythmicModality = this.Zone.RhythmicModality;
            if (rhythmicModality != null) {
                this.txtRhythmicModality.Text = rhythmicModality.ElementString(false) + " " + rhythmicModality.DistanceSchema;
                this.txtRhythmicModality.ToolTip = string.Empty;
            }
        }

        /// <summary>
        /// Updates the zone from panel.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        public void UpdateZoneFromPanel(int givenBarNumber) {
            if (this.Zone == null) {
                return;
            }

            //// var regularity = (int)Math.Round(this.Regularity.Value / 100 * 24, 0);
            var volume = (int)Math.Round(this.sliderVolume.Value / 100.0f * (int)MusicalLoudness.MaxLoudness, 0);
            this.Zone.Loudness = (MusicalLoudness)volume;
            this.Zone.Orchestra = this.ComboOrchestra.SelectedItem as OrchestraUnit;
            if (this.Zone.Orchestra != null) {
                this.Zone.Name = this.Zone.Orchestra.ToString();
            }

            this.Zone.MobilityPlanFunction = this.controlMobility.PlanFunction;
            this.Zone.DetermineMobilityForBar(givenBarNumber);
        }

        /// <summary>
        /// Selects the orchestra item.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SelectOrchestraItem(string name) {
            var m = (from item in this.ComboOrchestra.Items
                        .Cast<OrchestraUnit>()
                     where item.Name == name
                     select item).FirstOrDefault();
            if (m != null) {
                this.ComboOrchestra.SelectedItem = m;
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh() {
            if (this.Zone.Orchestra != null) {
                this.SelectOrchestraItem(this.Zone.Orchestra.Name);
            }
        }
        #endregion

        #region Load data
        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
        }
        #endregion

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
                this.Zone.HarmonicModality = harmonicModality;
                Console.Beep(990, 180);
                handled = true;
            }

            if (e.Data.GetData("RhythmicModality") is RhythmicModality rhythmicModality) {
                this.txtRhythmicModality.Text = rhythmicModality.ElementString(false) + " " + rhythmicModality.DistanceSchema;
                this.txtRhythmicModality.ToolTip = string.Empty;
                this.Zone.RhythmicModality = rhythmicModality;
                Console.Beep(990, 180);
                handled = true;
            }

            e.Handled = handled;
        }
        #endregion
    }
}