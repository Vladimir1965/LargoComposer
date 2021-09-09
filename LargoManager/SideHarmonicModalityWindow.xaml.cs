// <copyright file="SideHarmonicModalityWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoManager
{
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Harmony Window.
    /// </summary>
    public partial class SideHarmonicModalityWindow 
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideHarmonicModalityWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideHarmonicModalityWindow"/> class.
        /// </summary>
        public SideHarmonicModalityWindow()
        {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            //// EditorSettings.Singleton.SettingsEditor.PanelOpen("SideHarmonicModality");

            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideHarmonicModalityWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<SideHarmonicModalityWindow>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        #endregion

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }

        /// <summary>
        /// Gets the ordered list.
        /// </summary>
        /// <value>
        /// The ordered list.
        /// </value>
        public List<HarmonicModality> ResultList => this.PanelDetailMaterial.ResultList;

        #region Public methods

        /// <summary> Loads the data. </summary>
        public override void LoadData()
        {
            this.PanelDetailMaterial.LoadData();
            //// this.panelDetailMotives.LoadData(givenModel);
            this.ControlHarmonicModality.LoadData(DefaultValue.HarmonicOrder, 7, 12);
            this.ControlHarmonicModality.SelectItem(4095); //// 2741
            this.ControlHarmonicModality_SelectionChanged(null, null);
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(HarmonicModality givenModality) {
            this.HarmonicModality = givenModality;
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
            //// EditorSettings.Singleton.SettingsEditor.PanelClose("SideHarmonicModality");
        }

        #endregion

        /// <summary>
        /// Event handler. Called by ControlHarmonicModality for selection changed events.
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e">      Event information. </param>
        private void ControlHarmonicModality_SelectionChanged(object sender, EventArgs e) {
            var mod = this.ControlHarmonicModality.SelectedModality();
            if (mod == null || Singleton == null) {
                return;
            }

            this.FilterByModality(mod);
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the PanelDetailMaterial control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PanelDetailMaterial_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var mod = this.PanelDetailMaterial.SelectedModality();
            if (mod == null || Singleton == null) {
                return;
            }

            //// SideHarmonicStructuresWindow.Singleton.FilterByModality(mod);
        }
    }
}
