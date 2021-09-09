﻿// <copyright file="ViewSettingsImport.xaml.cs" company="Traced-Ideas, Czech republic">
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
    public partial class ViewSettingsImport
    {
        /*// <summary>
        //// Running Initialization.
        //// </summary>
        ////private readonly bool runningInit; */

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSettingsImport"/> class.
        /// </summary>
        public ViewSettingsImport() {
            //// this.runningInit = true;
            this.InitializeComponent();
            //// this.runningInit = false;
        }

        #endregion

        /// <summary>
        /// Load Data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            //// this.textBlockModel.Text = MusicalInformer.Singleton.MusicalBlockModelTitle;
            //// this.uCMusScore.LoadData();
            var settings = MusicalSettings.Singleton;
            this.DataContext = settings;
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
            //// var musicalRules = settings.SettingsComposition.Rules;

            /* Note limits
            if (this.UcLowNotes.Combo.SelectedItem is KeyValuePair vt) {
                settings.SettingsProgram.NoteLowest = (byte)vt.NumericKey;
            }
            if (this.UcHighNotes.Combo.SelectedItem is KeyValuePair vt) {
                settings.SettingsProgram.NoteHighest = (byte)vt.NumericKey;
            }
            */

            //// 2021/08 settings.SettingsImport.SplitMultiTracks = (FileSplit)(byte)this.UcSplitFile1.Combo.SelectedIndex;
            MusicalSettings.Singleton.Save();
        }

        #region Private interface
        /// <summary>
        /// Loads the settings values.
        /// </summary>
        private void SetValues() {
            var settings = MusicalSettings.Singleton;

            //// 2021/08 this.UcSplitFile1.Combo.SelectedIndex = (int)settings.SettingsImport.SplitMultiTracks;
        }

        /// <summary>
        /// Handles the Click event of the checkBoxHighlightMelodicVoices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxHighlightMelodicVoices_Click(object sender, RoutedEventArgs e) {
        }

        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the uCLowNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UCLowNotes_SelectionChanged(object sender, EventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the uCHighNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UCHighNotes_SelectionChanged(object sender, EventArgs e) {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the uCSplitFile1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UCSplitFile1_SelectionChanged(object sender, EventArgs e) {
        }

        /// <summary>
        /// Handles the Click event of the CheckBoxSkipNegligibleLines control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxSkipNegligibleLines_Click(object sender, RoutedEventArgs e) {
        }
    }
}