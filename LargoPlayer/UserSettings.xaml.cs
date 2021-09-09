// <copyright file="UserSettings.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoPlayer
{
    using LargoSharedClasses.Settings;
    using LargoSharedControls.Abstract;

    /// <summary>
    /// Interaction logic for SettingsWindow.
    /// </summary>
    public partial class UserSettings
    {
        /// <summary>
        /// Initializes a new instance of the UserSettings class.
        /// </summary>
        public UserSettings()
        {
            this.InitializeComponent();

            MusicalSettings.Singleton.Load();
            //// this.LoadWindowManager();
            this.PanelSettingsMain1?.LoadData();

            this.Localize();   
        }

        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }
    }
}
