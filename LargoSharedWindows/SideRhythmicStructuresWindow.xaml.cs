// <copyright file="SideRhythmicStructuresWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedPanels;

namespace LargoSharedWindows
{
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Controls;

    /// <summary>
    /// Harmony Window.
    /// </summary>
    public partial class SideRhythmicStructuresWindow
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideRhythmicStructuresWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideRhythmicStructuresWindow"/> class.
        /// </summary>
        public SideRhythmicStructuresWindow() {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            SharedWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            SidePanels.Singleton.PanelOpen("SideRhythmicStructures");
            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideRhythmicStructuresWindow Singleton {
            get {
                Contract.Ensures(Contract.Result<SideRhythmicStructuresWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        /// <summary>
        /// Gets or sets the Rhythmic modality.
        /// </summary>
        /// <value>
        /// The Rhythmic modality.
        /// </value>
        public RhythmicModality RhythmicModality { get; set; }

        /// <summary>
        /// Gets the ordered list.
        /// </summary>
        /// <value>
        /// The ordered list.
        /// </value>
        public List<RhythmicStructure> ResultList => this.PanelDetailMaterial.ResultList;

        #region Public methods

        /// <summary> Loads the data. </summary>
        public override void LoadData() {
            this.PanelDetailMaterial.LoadData();
            //// this.panelDetailMotives.LoadData(givenModel);
            this.ModalityOrder.SelectedIndex = 2;
            this.ModalityOrder_SelectionChanged(null, null);
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(RhythmicModality givenModality) {
            this.RhythmicModality = givenModality;
            this.ControlRhythmicModality.SelectModality(givenModality);
            this.PanelDetailMaterial.FilterByModality(givenModality);
        }

        #endregion

        #region Closing
        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
            SidePanels.Singleton.PanelClose("SideHarmony");
        }

        #endregion

        /// <summary> Open Window - Rhythm. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        private void RhythmicPlan(object sender, System.Windows.RoutedEventArgs e) {
            /*
            //// if (this.sideRhythmWindow != null && this.sideRhythmWindow.IsVisible) { return; }
            //// this.sideRhythmWindow = 
            WindowManager.OpenWindow("EditorWindows", "SideRhythmWindow", null);

            if (SharedWindows.SideRhythmWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            } */
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ModalityOrder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ModalityOrder_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.ModalityOrder.SelectedItem as ComboBoxItem;
            if (item == null) {
                return;
            }

            var order = byte.Parse((string)item.Tag);
            this.ControlRhythmicModality.LoadData(order, 0, 6);  //// Temporary limited to max 6 beats
            if (this.ControlRhythmicModality.SortedList == null) {
                return;
            }

            if (order == 4 && this.ControlRhythmicModality.SortedList.Count > 5) {
                this.ControlRhythmicModality.SelectItem(5);
            }
            else {
                if (this.ControlRhythmicModality.SortedList.Count > 0) {
                    this.ControlRhythmicModality.SelectItem(0);
                }
            }

            this.ControlRhythmicModality_SelectionChanged(null, null);
        }

        /// <summary>
        /// Event handler. Called by ControlRhythmicModality for selection changed events.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e">      Event information. </param>
        private void ControlRhythmicModality_SelectionChanged(object sender, EventArgs e) {
            var mod = this.ControlRhythmicModality.SelectedModality();
            if (mod == null || Singleton == null) {
                return;
            }

            this.RhythmicModality = mod;
            this.PanelDetailMaterial.LoadModalStructures(mod);
            //// this.FilterByModality(mod);
        }
    }
}
