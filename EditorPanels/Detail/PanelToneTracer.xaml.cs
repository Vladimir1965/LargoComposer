// <copyright file="PanelToneTracer.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using System.Linq;
using System.Windows.Controls;

namespace EditorPanels.Detail
{
    /// <summary>
    /// Panel Musical Score.
    /// </summary>
    public sealed partial class PanelToneTracer {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelToneTracer"/> class. 
        /// </summary>
        public PanelToneTracer() {
            this.InitializeComponent();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the editor element.
        /// </summary>
        /// <value>
        /// The editor element.
        /// </value>
        public MusicalElement MusicalElement { get; set; }
        #endregion

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
            //// this.Localize();
        }

        #region Listeners
        /// <summary>
        /// Musicals the element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args) {
            this.MusicalElement = args.Element;
            if (this.MusicalElement?.TonePacket == null) {
                return;
            }

            this.DataGridPackets.ItemsSource = this.MusicalElement.TonePacket.BarTones;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the DataGridPackets control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridPackets_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var mtone = this.DataGridPackets.SelectedItem as IMusicalTone;
            //// if (tonePacket == null || tonePacket.IntendedTones == null) { return; } 
            var intendedTones = from w in this.MusicalElement.TonePacket.IntendedTones
                         where mtone != null && (w.BitFrom == mtone.BitFrom && w.Duration == mtone.Duration)
                         orderby w.TotalValue
                         descending
                         select w;
            this.DataGridWrappers.ItemsSource = intendedTones;
        }
        #endregion
    }
}