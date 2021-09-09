// <copyright file="MusicFileLoader.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
namespace LargoManager
{
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Port;
    using LargoSharedControls.Abstract;
    using ManagerPanels;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SettingsWindow.
    /// </summary>
    public partial class MusicFileLoader
    {
        /// <summary>
        /// Initializes a new instance of the MusicFileLoader class.
        /// </summary>
        public MusicFileLoader()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Load Data.
        /// </summary>
        public override void LoadData()
        {
            base.LoadData();
            this.LoadSettings();
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Loads the music file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> Instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">Invalid tag.</exception>
        private void LoadMusicFile(object sender, RoutedEventArgs e)
        {
            var intTag = int.Parse(this.ButtonLoad.Tag as string ?? throw new InvalidOperationException());
            MusicalSourceType sourceType = (MusicalSourceType)intTag;
            var port = PortAbstract.CreatePort(sourceType);
            PortAbstract.SettingsImport = ManagerSettings.Singleton.SettingsImport;
            string path = ManagerSettings.Singleton.PathToMusicFiles;
            port.LoadFromFiles(path);
            this.Close();
        }

        /// <summary>
        /// Switch of the format.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SwitchFormat(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            this.ButtonLoad.Tag = menuItem.Tag;
            this.ButtonLoad.Content = menuItem.Header; //// LocalizedString ...

            MusicalSourceType sourceType = (MusicalSourceType)int.Parse(menuItem.Tag.ToString());
            var settings = ManagerSettings.Singleton;
            settings.SettingsImport.LastUsedFormat = sourceType;
            settings.Save();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the uCSplitFile1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UCSplitFile1_SelectionChanged(object sender, EventArgs e)
        {
            var settings = ManagerSettings.Singleton;
            settings.SettingsImport.SplitMultiTracks = (FileSplit)(byte)this.UcSplitFile1.Combo.SelectedIndex;
            settings.Save();
        }

        /// <summary>
        /// Handles the Click event of the CheckBoxSkipNegligibleLines control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxSkipNegligibleLines_Click(object sender, RoutedEventArgs e)
        {
            var settings = ManagerSettings.Singleton;
            settings.SettingsImport.SkipNegligibleTones = this.CheckBoxSkipNegligibleLines.IsChecked ?? false;
            settings.Save();
        }

        #region Private interface
        /// <summary>
        /// Loads the settings values.
        /// </summary>
        private void LoadSettings()
        {
            var settings = ManagerSettings.Singleton;
            settings.Load();
            //// this.DataContext = settings;
            MusicalSourceType source = ManagerSettings.Singleton.SettingsImport.LastUsedFormat;
            this.ButtonLoad.Tag = ((int)source).ToString();
            this.ButtonLoad.Content = source.ToString();  //// LocalizableString
            this.UcSplitFile1.Combo.SelectedIndex = (int)settings.SettingsImport.SplitMultiTracks;
            this.CheckBoxSkipNegligibleLines.IsChecked = settings.SettingsImport.SkipNegligibleTones;
        }
        #endregion
    }
}
