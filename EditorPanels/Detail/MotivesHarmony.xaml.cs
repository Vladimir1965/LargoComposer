// <copyright file="MotivesHarmony.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using LargoSharedClasses.Models;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace EditorPanels.Detail
{
    /// <summary>
    /// PanelHarmonic Motive.
    /// </summary>
    public sealed partial class MotivesHarmony {
        #region Fields
        /// <summary>
        /// Musical Block Model.
        /// </summary>
        private HarmonicModel model;

        /// <summary>
        /// THarmonic Motive.
        /// </summary>
        private HarmonicMotive harMotive;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MotivesHarmony"/> class.
        /// </summary>
        public MotivesHarmony() {
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
        public HarmonicModel HarmonicModel
        {
            set {
                this.model = value;

                if (this.model == null) {
                    return;
                }

                this.DataGridMotives.ItemsSource = this.model.HarmonicMotives;
                if (this.DataGridMotives.Items.Count > 0) {
                    this.DataGridMotives.SelectedIndex = 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenModel">The given block model.</param>
        public void LoadData(HarmonicModel givenModel)
        {
            base.LoadData();
            this.HarmonicModel = givenModel; //// MenuEventSender.Singleton.LastSelectedMusicalBlockModel;
            //// this.PanelGroup.EventSender.TemplateChanged += this.MusicalTemplateChanged;
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
        }

        #region HarBars
        /// <summary>
        /// Rebind HarmonicBars.
        /// </summary>
        private void RebindHarmonicBars() {
            var harmonicMotive = this.harMotive;
            if (harmonicMotive == null) {
                this.DataGridHarBars.ItemsSource = null;
                this.DataGridHarmonicChanges.ItemsSource = null;
                return;
            }

            this.DataGridHarBars.ItemsSource = harmonicMotive.HarmonicStream.HarmonicBars;
            this.DataGridHarBars.Items.Refresh();

            var blockChanges = this.model.BlockChanges;
            if (blockChanges == null) {
                throw new InvalidDataException("blockChanges");
            }

            var harmonicChanges = blockChanges.HarmonicChanges;
            if (harmonicChanges == null) {
                throw new InvalidDataException("harmonicChanges");
            }

            var motiveChanges = from rc in harmonicChanges where rc.MotiveNumber == harmonicMotive.Number select rc;
            this.DataGridHarmonicChanges.ItemsSource = motiveChanges;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Data Grid Harmonic Bars control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridHarBars_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //// var selectedHarmonicBar = this.DataGridHarBars.SelectedValue as HarmonicBar;
            //// var panel = PanelHarmonicBar.Singleton;
            //// if (panel != null) { panel.HarmonicBar = selectedHarmonicBar; } 
            e.Handled = true;
        }

        //// private void SelectHarBar(int musicalBar) { this.DataGridHarBars.SelectedIndex = musicalBar; }

        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the DataGridMotives control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridMotives_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.harMotive = this.DataGridMotives.SelectedItem as HarmonicMotive;
            this.RebindHarmonicBars();
        }

        /// <summary>
        /// Editors the element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args) {
            if (args.Bar.HarmonicBar?.HarmonicMotive == null) {
                return;
            }

            var motive = args.Bar.HarmonicBar.HarmonicMotive;
            var motives = this.DataGridMotives.Items;
            var idx = motives.IndexOf(motive);
            if (idx >= 0) {
                this.DataGridMotives.SelectedIndex = idx;
                this.DataGridMotives.ScrollIntoView(motive);
            }
        }
    }
}