// <copyright file="InherentException.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows
{
    using LargoSharedClasses.Settings;
    using System;

    /// <summary>
    /// Interaction logic for WindowExceptionCard.
    /// </summary>
    public sealed partial class InherentException
    {
        /// <summary>
        /// Initializes a new instance of the InherentException class.
        /// </summary>
        public InherentException()
        {
            this.InitializeComponent();
            //// this.RegisterPanels();
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <param name="exception">Given exception.</param>
        public void LoadData(Exception exception) {
            if (this.PanelDisplayException == null) {
                return;
            }

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            this.PanelDisplayException.LoadData(exception, path);  
        }
    }
}
