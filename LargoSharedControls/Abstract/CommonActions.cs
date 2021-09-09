// <copyright file="CommonActions.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Composer;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LargoSharedControls.Abstract
{
    /// <summary>Common Actions </summary>
    public class CommonActions
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        public static readonly CommonActions Singleton = new CommonActions();

        #endregion

        #region Properties
        /// <summary> Gets or sets the selected block. </summary>
        /// <value> The selected block. </value>
        public MusicalBlock SelectedBlock { get; set; }
        #endregion
        
        /// <summary>
        /// Plays the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Play(object sender, RoutedEventArgs e) {
            var filePath = PortDocuments.Singleton.SelectedMifPath;
            this.PerformByPlayer(filePath);
        }

        /// <summary>
        /// Edits the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Edit(object sender, RoutedEventArgs e) {
            var filePath = PortDocuments.Singleton.SelectedMifPath;
            this.EditFile(filePath);
        }

        /// <summary>
        /// Edits the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="tectonicTemplatePath">The tectonic template path.</param>
        /// <param name="harmonicTemplatePath">The harmonic template path.</param>
        public void EditFile(string filePath, string tectonicTemplatePath, string harmonicTemplatePath) {
            var args = '"' + filePath + '"' + ' ' + '"' + tectonicTemplatePath + '"' + ' ' + '"' + harmonicTemplatePath + '"';
            var programPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.BinaryFolder);
            var exeFile = Path.Combine(programPath, @"LargoEditor.exe");
            if (!File.Exists(exeFile)) {
                MessageBox.Show("LargoEditor not installed", SettingsApplication.ApplicationName);
                return;
            }

            Process.Start(exeFile, args);
        }

        /// <summary>
        /// Plays the specified sender.
        /// </summary>
        /// <param name="givenFilePath">The given file path.</param>
        public void PerformByPlayer(string givenFilePath) {
            SystemProcesses.KillApplication("LargoPlayer");

            if (string.IsNullOrWhiteSpace(givenFilePath)) {
                return;
            }

            var args = '"' + givenFilePath + '"';
            var programPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.BinaryFolder);
            var exeFile = Path.Combine(programPath, @"LargoPlayer.exe");
            if (!File.Exists(exeFile)) {
                MessageBox.Show("LargoPlayer not installed", SettingsApplication.ApplicationName);
                return;
            }

            Process.Start(exeFile, args);
        }

        /// <summary>
        /// Composes the specified block to compose.
        /// </summary>
        /// <param name="blockToCompose">The block to compose.</param>
        /// <returns> Returns value. </returns>
        public MusicalBundle Compose(MusicalBlock blockToCompose) {
            //// Console.WriteLine(bundleToCompose.ActualName + " ==> " + destinationFilePath);
            //// Console.ReadLine();
            if (blockToCompose == null) {
                return null;
            }

            BlockComposer blockComposer = new BlockComposer();
            MusicalBundle result = blockComposer.ComposeMusic(blockToCompose);
            return result;
        }

        /// <summary>Composes the specified sender.</summary>
        /// <param name="givenFileToCompose">The given file to compose.</param>
        /// <param name="givenFileResult">The given file result.</param>
        public void Compose(string givenFileToCompose, string givenFileResult) {
            var args = '"' + givenFileToCompose + '"';
            args += " " + '"' + givenFileResult + '"';
            var programPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.BinaryFolder);
            var command = Path.Combine(programPath, "XLargoCompose.exe");

            //// Start the process.
            Process p = Process.Start(command, args);

            //// Wait for the window to finish loading.
            if (p != null) {
                p.WaitForInputIdle();

                //// Wait for the process to end.
                p.WaitForExit();
            }

            //// Process.Start(command, args);
        }

        /// <summary>
        /// Edits the specified file.
        /// </summary>
        /// <param name="givenFilePath">The given file path.</param>
        public void EditFile(string givenFilePath) {
            bool isMusic = true;
            var args = '"' + givenFilePath + '"' + ' ' + '"' + (isMusic ? '1' : '0') + '"';
            var programPath = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.BinaryFolder);
            var exeFile = Path.Combine(programPath, @"LargoEditor.exe");
            if (!File.Exists(exeFile)) {
                MessageBox.Show("LargoEditor not installed", SettingsApplication.ApplicationName);
                return;
            }

            Process.Start(exeFile, args);
        }

        #region Private actions, functions

        /// <summary>
        /// Exports file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">Tag exception.</exception>
        public void ExportFile(object sender, RoutedEventArgs e) {
            var intTag = int.Parse((sender as MenuItem)?.Tag as string ?? throw new InvalidOperationException());
            MusicalSourceType sourceType = (MusicalSourceType)intTag;
            MusicDocument document = PortDocuments.Singleton.SelectedDocument;
            if (document == null) {
                return;        
            }

            //// CommonActions.Singleton.SelectedBlock = resultBlock;
            PortDocuments.Singleton.LoadDocument(document, true);
            MusicalBlock block = PortDocuments.Singleton.MusicalBlock; //// this.SelectedBlock;
            if (block == null) {
                return;
            }

            var bundle = MusicalBundle.GetEnvelopeOfBlock(block, document.Header.FullName ?? "Unnamed");
            var port = PortAbstract.CreatePort(sourceType);
            if (port.SaveBundle(bundle)) {
                MessageBox.Show(
                    string.Format("File saved!\n\n{0}", port.DestinationFilePath),
                    SettingsApplication.ApplicationName);
            }
            else {
                MessageBox.Show(
                    string.Format("File save filed!\n\n{0}", port.DestinationFilePath),
                    SettingsApplication.ApplicationName);
            }
        }
        #endregion

        #region Private external windows
        /// <summary>Opens the about.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void OpenAbout(object sender, RoutedEventArgs e) {
            WindowManager.OpenWindow("LargoSharedWindows", "InherentAbout", null);
        }
        #endregion
    }
}
