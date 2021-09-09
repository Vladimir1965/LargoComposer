// <copyright file="ViewSettingsFolders.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

////[assembly: CLSCompliant(true)]

using System.IO;
using System.Windows;
using JetBrains.Annotations;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedControls.Abstract;

namespace SettingsPanels
{
    /// <summary>
    /// Panel Composition.
    /// </summary>
    public partial class ViewSettingsFolders
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSettingsFolders" /> class.
        /// </summary>
        public ViewSettingsFolders() {
            this.InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Load Data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            MusicalSettings.Singleton.Load();
            this.SetValues();
            CultureMaster.Localize(this);
            //// var setup = MusicalSettings.Singleton;
        }

        /// <summary>
        /// Save Changes.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        [UsedImplicitly]
        private void SaveChanges(object sender, RoutedEventArgs e) {
            var settings = MusicalSettings.Singleton;
            MusicalSettings.SaveMusicalFolders(settings.Folders);
        }

        #region Private interface

        /// <summary>
        /// Loads the setup values.
        /// </summary>
        private void SetValues() {
            var settings = MusicalSettings.Singleton;
            if (settings.Folders != null) {
                this.DataGrid.ItemsSource = settings.Folders.List;
            }
        }
        #endregion
    }
}