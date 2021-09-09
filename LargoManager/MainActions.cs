// <copyright file="MainActions.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using LargoSharedClasses.Support;
using LargoSharedControls;
using System.IO;
using System.Windows;

namespace LargoManager
{
    /// <summary>
    /// Main Actions.
    /// </summary>
    public class MainActions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainActions"/> class.
        /// </summary>
        /// <param name="givenWindow">The given window.</param>
        public MainActions(MainWindow givenWindow) {
            this.Main = givenWindow;
        }

        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public MainWindow Main { get; set; }

        #region Private methods

        /// <summary>
        /// Renames the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Rename(object sender, RoutedEventArgs e) {
            var document = PortDocuments.Singleton.SelectedDocument;
            var filePath = document.FilePath;
            if (!File.Exists(filePath)) {
                return;
            }

            var dir = Path.GetDirectoryName(filePath);
            var name = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);

            var inputDialog = new DialogBoxChangeString("Rename:", name);
            if (!(inputDialog.ShowDialog() ?? false)) {
                return;
            }

            var newName = inputDialog.Answer;
            if (!string.IsNullOrWhiteSpace(newName)) {
                var newFilePath = Path.Combine(dir ?? throw new InvalidOperationException(), newName + '.' + ext);
                File.Move(filePath, newFilePath);
                document.FilePath = newFilePath;

                var s = newName.Split('#');
                if (s.Length > 0) {
                    document.Header.FileName = s[0];
                    if (s.Length > 1) {
                        document.Header.Number = int.Parse(s[1]);
                        if (s.Length > 2) {
                            document.Header.Specification = s[2];
                        }
                    }
                }

                var blocks = this.Main.GridBlocks.ItemsSource;
                this.Main.GridBlocks.ItemsSource = null;
                this.Main.GridBlocks.ItemsSource = blocks;

                //// document.Name = newName;
                //// document.Number = XmlSupport.ReadIntegerAttribute(xheader.Attribute("Number"));
            }
        }

        #endregion
    }
}
