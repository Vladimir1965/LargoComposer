// <copyright file="SideHarmonicStructuresWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedPanels;

namespace LargoSharedWindows
{
    using LargoSharedClasses.Abstract;
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
    public partial class SideHarmonicStructuresWindow 
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SideHarmonicStructuresWindow singleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SideHarmonicStructuresWindow"/> class.
        /// </summary>
        public SideHarmonicStructuresWindow()
        {
            Singleton = this;
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            SharedWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            SidePanels.Singleton.PanelOpen("SideHarmonicStructures");
            //// this.Localize();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SideHarmonicStructuresWindow Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<SideHarmonicStructuresWindow>() != null);
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
        public List<HarmonicStructure> ResultList => this.PanelDetailMaterial.ResultList;

        #region Public methods

        /// <summary> Loads the data. </summary>
        public override void LoadData()
        {
            this.PanelDetailMaterial.LoadData();

            this.ModalityLevel.SelectedIndex = 4; //// Heptatonics
            this.ModalityLevel_SelectionChanged(null, null);

            this.ChordLevel.SelectedIndex = 2; //// Triads
            this.ChordLevel_SelectionChanged(null, null);
        }

        /// <summary> Filter by modality. </summary>
        /// <param name="givenModality"> The given modality. </param>
        public void FilterByModality(HarmonicModality givenModality) {
            this.HarmonicModality = givenModality;
            if (this.ModalityLevel?.Items != null) {
                foreach (var item in this.ModalityLevel.Items) {
                    if (byte.Parse((string)((ComboBoxItem)item).Tag) == givenModality.Level) {
                        this.ModalityLevel.SelectedItem = item;
                        break;
                    }
                }
            }

            this.ControlHarmonicModality.SelectModality(givenModality);
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
        /// Handles the SelectionChanged event of the ListType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ListType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.ListType.SelectedItem as ComboBoxItem;
            if (item != null) {
                var tag = byte.Parse((string)item.Tag);
                switch (tag) {
                    case 1: {
                        break;
                    }

                    case 2: {
                        break;
                    }

                    case 3: {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ModalityLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ModalityLevel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.ModalityLevel.SelectedItem as ComboBoxItem;
            if (item == null) {
                return;
            }

            var level = byte.Parse((string)item.Tag);
            this.ControlHarmonicModality.LoadData(DefaultValue.HarmonicOrder, level, level);
            if (level == 7) {
                this.ControlHarmonicModality.SelectItem(2741); //// 1451, 2773, 1387
            }

            this.ControlHarmonicModality_SelectionChanged(null, null);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ChordLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ChordLevel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.ChordLevel.SelectedItem as ComboBoxItem;
            if (item == null) {
                return;
            }

            var level = byte.Parse((string)item.Tag);
            if (level == 0) {
                this.PanelDetailMaterial.FilterByLevel(2, 6);
            } 
            else {
                this.PanelDetailMaterial.FilterByLevel(level, level);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the dissonance Level control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SonanceLevel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }
    }
}
