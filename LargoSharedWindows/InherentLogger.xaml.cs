// <copyright file="InherentLogger.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows {
    using LargoSharedClasses.Localization;

    /// <summary>
    /// Interaction logic for Window Process Logger.
    /// </summary>
    public sealed partial class InherentLogger
    {
        /// <summary>
        /// Initializes a new instance of the InherentLogger class.
        /// </summary>
        public InherentLogger()
        {
            this.InitializeComponent();
            this.Title = LocalizedControls.String("Please wait a moment...");
            //// this.RegisterPanels();
        }
    }
}
