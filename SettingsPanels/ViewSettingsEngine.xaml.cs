// <copyright file="ViewSettingsEngine.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

////[assembly: CLSCompliant(true)]

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedControls.Abstract;

namespace SettingsPanels
{
    /// <summary>
    /// Panel Composition.
    /// </summary>
    public partial class ViewSettingsEngine
    {
        /* /// <summary>
        //// Running Initialization.
        //// </summary>
        //// private readonly bool runningInit; */

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSettingsEngine"/> class.
        /// </summary>
        public ViewSettingsEngine() {
            //// this.runningInit = true;
            this.InitializeComponent();
            //// this.runningInit = false;

            this.UcLowNotes.LoadData(1);
            this.UcHighNotes.LoadData(2);
        }

        #endregion

        /// <summary>
        /// Load Data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            this.ComboBoxMusicalRules.SelectedIndex = 0;
            MusicalSettings.Singleton.Load();
            this.SetValues();
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Save Changes.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void SaveChanges(object sender, RoutedEventArgs e) {
            var settings = MusicalSettings.Singleton;
            var musicalRules = settings.SettingsComposition.Rules;
            musicalRules.IndividualizeMelodicVoices = this.CheckBoxIndividualizeMelodicVoices.IsChecked ?? false;
            settings.SettingsComposition.TypeOfRules = (MusicalRulesType)this.ComboBoxMusicalRules.SelectedIndex; //// + 1
            settings.SettingsComposition.IndividualizeMelodicVoices = this.CheckBoxIndividualizeMelodicVoices.IsChecked ?? false;
            settings.SettingsComposition.HighlightMelodicVoices = this.CheckBoxHighlightMelodicVoices.IsChecked ?? false;

            settings.SettingsComposition.CorrectResultPitch = this.CheckCorrectResultPitch.IsChecked ?? false;
            settings.SettingsComposition.CorrectOctaves = this.CheckBoxCorrectOctaves.IsChecked ?? false;

            //// Note limits
            if (this.UcLowNotes.Combo.SelectedItem is KeyValuePair vtlow) {
                settings.SettingsComposition.NoteLowest = (byte)vtlow.NumericKey;
            }

            if (this.UcHighNotes.Combo.SelectedItem is KeyValuePair vthigh) {
                settings.SettingsComposition.NoteHighest = (byte)vthigh.NumericKey;
            }

            MusicalSettings.Singleton.Save();  
        }

        #region Private interface
        /// <summary>
        /// Loads the settings values.
        /// </summary>
        private void SetValues() {
            var settings = MusicalSettings.Singleton;
            this.CheckBoxHighlightMelodicVoices.IsChecked = settings.SettingsComposition.HighlightMelodicVoices;
            this.CheckBoxIndividualizeMelodicVoices.IsChecked = settings.SettingsComposition.IndividualizeMelodicVoices;
            this.ComboBoxMusicalRules.SelectedIndex = (int)settings.SettingsComposition.TypeOfRules;
            this.UcLowNotes.SelectItemNumericKey(settings.SettingsComposition.NoteLowest, false);
            this.UcHighNotes.SelectItemNumericKey(settings.SettingsComposition.NoteHighest, false);
        }

        #endregion
    }
}