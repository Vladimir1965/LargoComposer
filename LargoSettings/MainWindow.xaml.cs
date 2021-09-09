// <copyright file="MainWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;
using System.Windows;

namespace LargoSettings
{
    /// <summary>
    /// Interaction logic for Application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Open settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MainSettings(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoSharedWindows", "ConfigMain", null);
        }

        /// <summary>
        /// Imports the settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportSettings(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoManager", "ConfigImport", null);
        }

        /// <summary>
        /// Opens Composition Settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CompositionSettings(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoSharedWindows", "ConfigComposition", null);
        }
    }
}
