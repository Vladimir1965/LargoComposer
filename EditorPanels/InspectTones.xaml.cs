// <copyright file="InspectTones.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using LargoSharedClasses.Music;
using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;

namespace EditorPanels
{
    /// <summary>
    /// PanelMid Files.
    /// </summary>
    public sealed partial class InspectTones
    {
        #region Variables
        /// <summary>
        /// Musical Block.
        /// </summary>
        private MusicalBlock musicalBlock;

        /// <summary>
        /// Musical tones.
        /// </summary>
        private ToneCollection musicalTones;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectTones"/> class.
        /// </summary>
        public InspectTones() {
            this.InitializeComponent();
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
            //// PanelManager.InspectorTones = this;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance has display real tones.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has display real tones; otherwise, <c>false</c>.
        /// </value>
        private bool HasDisplayRealTones { get; set; }

        /// <summary>
        /// Gets or sets the musical tones.
        /// </summary>
        /// <value>
        /// The musical tones.
        /// </value>
        private ToneCollection MusicalTones {
            get => this.musicalTones;

            set {
                this.musicalTones = value;
                this.DataGridTones.ItemsSource = this.musicalTones;
            }
        }

        /// <summary>
        /// Gets or sets the musical block.
        /// </summary>
        /// <value>
        /// The musical block.
        /// </value>
        /// <summary>
        /// Gets or sets musical block.
        /// </summary>
        /// <value> Property description. </value>
        private MusicalBlock MusicalBlock {
            get {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.musicalBlock == null) {
                    throw new InvalidOperationException("Musical block is null.");
                }

                return this.musicalBlock;
            }

            set => this.musicalBlock = value ?? throw new ArgumentException("Argument cannot be null", nameof(value));
        }
        #endregion

        #region Load data
        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="realTones">if set to <c>true</c> [real tones].</param>
        public void LoadData(bool realTones) {
            base.LoadData();
            this.HasDisplayRealTones = realTones;
            //// EditorEventSender.Singleton.EditorChanged += this.MusicalElementChanged;
        }

        #endregion

        /// <summary>
        /// Plays the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Play(object sender, RoutedEventArgs e) {
            if (this.MusicalBlock == null || this.MusicalTones == null) {
                return;
            }

            //// TimedPlayer.PlayTones(this.MusicalTones, this.MusicalBlock);
        }

        /// <summary>
        /// DataGridFiles MouseDoubleClick.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void DataGridTones_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }

        /// <summary>
        /// Stops the playing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void StopPlaying(object sender, RoutedEventArgs e) {
            MusicalPlayer.Singleton.StopPlaying();
        }

        /// <summary>
        /// Editors the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args) { 
            if (args.Element == null) {
                return;
            }

            //// this.MusicalBlock = blockEditor.Block;  //// BlockModel.SourceMusicalBlock
            this.MusicalTones = this.HasDisplayRealTones ? args.Element.Tones : args.Element.Status.MelodicPlan.PlannedTones;
            //// Notation  var viewer = this.panelStave;
            //// viewer.IncipitData.DisplayTones(this.MusicalBlock.Header, this.MusicalTones);
            //// viewer.InvalidateVisual();
        }
    }
}
