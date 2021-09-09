// <copyright file="SideRhythmicModalityWindow.xaml.cs" company="Traced-Ideas, Czech republic">
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
    public partial class SideRhythmicModalityWindow 
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideRhythmicModalityWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideRhythmicModalityWindow"/> class.
        /// </summary>
        public SideRhythmicModalityWindow()
        {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            //// ConductorSettings.Singleton.SettingsEditor.PanelOpen("SideRhythmicModality");

            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideRhythmicModalityWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<SideRhythmicModalityWindow>() != null);
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
        public List<RhythmicModality> ResultList => this.PanelDetailMaterial.ResultList;

        #region Public methods

        /// <summary> Loads the data. </summary>
        public override void LoadData()
        {
            this.PanelDetailMaterial.LoadData();
            //// this.panelDetailMotives.LoadData(givenModel);
            this.ControlRhythmicModality.LoadData(DefaultValue.RhythmicOrder, 0, 12);
            //// this.ControlRhythmicModality.SelectItem(1); 
            this.ControlRhythmicModality_SelectionChanged(null, null);
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(RhythmicModality givenModality) {
            this.RhythmicModality = givenModality;
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
            //// EditorSettings.Singleton.SettingsEditor.PanelClose("SideRhythmicModality");
        }

        #endregion

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

            this.FilterByModality(mod);
        }

        /*
        /// <summary>
        /// Handles the MouseDoubleClick event of the PanelDetailMaterial control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PanelDetailMaterial_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var mod = this.ControlRhythmicModality.SelectedModality();
            if (mod == null || Singleton == null || SideRhythmicStructuresWindow.Singleton == null) {
                return;
            }

            SideRhythmicStructuresWindow.Singleton.FilterByModality(mod);
        }*/
    }
}
