// <copyright file="ConfigMain.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows
{
    using JetBrains.Annotations;
    using LargoSharedClasses.Localization;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using LargoSharedControls.Abstract;
    using System.Windows;

    /// <summary>
    /// Interaction logic for SettingsWindow.
    /// </summary>
    public partial class ConfigMain
    {
        /// <summary>
        /// Initializes a new instance of the ConfigMain class.
        /// </summary>
        public ConfigMain()
        {
            this.InitializeComponent();

            MusicalSettings.Singleton.Load();
            this.PanelSettingsMain1?.LoadData();
            this.PanelSettings2?.LoadData();

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
