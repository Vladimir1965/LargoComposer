// <copyright file="ConfigComposition.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows
{
    using LargoSharedClasses.Settings;
    using LargoSharedControls.Abstract;

    /// <summary>
    /// Interaction logic for SettingsWindow.
    /// </summary>
    public partial class ConfigComposition
    {
        /// <summary>
        /// Initializes a new instance of the ConfigComposition class.
        /// </summary>
        public ConfigComposition()
        {
            this.InitializeComponent();

            MusicalSettings.Singleton.Load();
            this.PanelSettings1?.LoadData();

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
