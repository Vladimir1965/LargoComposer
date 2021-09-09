// <copyright file="ViewSettingsMain.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Settings;
using LargoSharedControls.Abstract;
using System;
using System.Windows;

namespace SettingsPanels
{
    //// using LargoRepository.Filesystem;

    /// <summary>
    /// PanelMain Settings.
    /// </summary>
    public partial class ViewSettingsMain
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSettingsMain"/> class.
        /// </summary>
        public ViewSettingsMain() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            var settings = MusicalSettings.Singleton;
            this.DataContext = settings;
            MusicalSettings.Singleton.Load();
            this.SetValues();

            CultureMaster.Localize(this);
        }
        #endregion

        /// <summary>
        /// Save Changes.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void SaveChanges(object sender, RoutedEventArgs e) {
            var settings = MusicalSettings.Singleton;
            settings.SettingsProgram.DefaultCulture = (byte)this.UcCulture1.Combo.SelectedIndex;
            settings.SettingsProgram.ParallelMode = this.CheckMultitasking.IsChecked ?? false;
            settings.SettingsAnalysis.LongTones = this.CheckLongTones.IsChecked ?? false;
            settings.SettingsProgram.InstrumentInVoices = this.CheckInstrumentInVoices.IsChecked ?? false;
            settings.SettingsProgram.RespectPauses = this.CheckRespectPauses.IsChecked ?? false;

            MusicalSettings.Singleton.Save();
            //// this.Window.Close();
        }

        /// <summary>
        /// Loads the settings values.
        /// </summary>
        private void SetValues() {
            var settings = MusicalSettings.Singleton;
            this.UcCulture1.Combo.SelectedIndex = settings.SettingsProgram.DefaultCulture;
            this.CheckMultitasking.IsChecked = settings.SettingsProgram.ParallelMode;
            this.CheckLongTones.IsChecked = settings.SettingsAnalysis.LongTones;
            this.CheckInstrumentInVoices.IsChecked = settings.SettingsProgram.InstrumentInVoices;
            this.CheckRespectPauses.IsChecked = settings.SettingsProgram.RespectPauses;
        }

        #region Private interface

        /// <summary>
        /// Handles the Click event of the check Long tones control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckLongTones_Click(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Handles the Click event of the checkAnalyzeAfterLoad control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckMultitasking_Click(object sender, RoutedEventArgs e) {
        }

        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the ControlCulture control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UCCulture1_SelectionChanged(object sender, EventArgs e) {
            var settings = MusicalSettings.Singleton;
            if (this.UcCulture1.Combo.SelectedItem is KeyValuePair item) {
                switch (item.Key) {
                    case "en":
                        settings.SettingsProgram.DefaultCulture = 0;
                        break;
                    case "it":
                        settings.SettingsProgram.DefaultCulture = 1;
                        break;
                    case "cz":
                        settings.SettingsProgram.DefaultCulture = 2;
                        break;
                }
            }

            settings.InitializeCultureInfo();
        }
    }
}