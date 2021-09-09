// <copyright file="FileDialogs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Support Files.
    /// </summary>
    public static class FileDialogs {
        #region Largo Standard single files

        /// <summary>
        /// Opens the selected largo file.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedMifiFile(string initialFolder)
        {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.largo", ".largo", "Largo files (*.largo)|*.largo|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedMidiFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.mid", ".mid", "Midi files (*.mid)|*.mid|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedMusicXmlFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.xml", ".xml", "MusicXml files (*.xml)|*.xml|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedMusicMxlFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.mxl", ".mxl", "MusicMxl files (*.mxl)|*.mxl|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected harmonic File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedHarmonicFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.har", ".har", "Harmony files (*.har)|*.har|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected text File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedTextFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.txt", ".txt", "Text files (*.txt)|*.txt|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Opens the selected notator.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedNotator(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.exe", ".exe", "Notator (*.exe)|*.exe|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Opens the selected board file.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public static string OpenSelectedBoardFile(string initialFolder) {
            return SupportFiles.OpenSelectedFile(initialFolder, "*.board", ".board", "Board files (*.board)|*.board|All Files (*.*)|*.*");
        }
        #endregion

        #region Largo Standard multi files

        /// <summary>
        /// Open selected music interchange File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string[] OpenSelectedMifiFiles(string initialFolder)
        {
            return SupportFiles.OpenSelectedFiles(initialFolder, "*.mif", ".mif", "Mifi files (*.mif)|*.mif|Mifi files (*.mifi)|*.mifi|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string[] OpenSelectedMidiFiles(string initialFolder) {
            return SupportFiles.OpenSelectedFiles(initialFolder, "*.mid", ".mid", "Midi files (*.mid)|*.mid|Midi files (*.midi)|*.midi|Karaoke files (*.kar)|*.kar|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string[] OpenSelectedMusicXmlFiles(string initialFolder) {
            return SupportFiles.OpenSelectedFiles(initialFolder, "*.xml", ".xml", "MusicXml files (*.xml)|*.xml|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string[] OpenSelectedMusicMxlFiles(string initialFolder) {
            return SupportFiles.OpenSelectedFiles(initialFolder, "*.mxl", ".mxl", "MusicMxl files (*.mxl)|*.mxl|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected music plan File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string[] OpenSelectedMipiFiles(string initialFolder) {
            return SupportFiles.OpenSelectedFiles(initialFolder, "*.mip", ".mip", "Mipi files (*.mip)|*.mip|Mipi files (*.mipi)|*.mipi|All Files (*.*)|*.*");
        }

        #endregion

        #region Largo Standard files to save
        /// <summary>
        /// Open selected largo File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string SelectMifiFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.mif";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".mif", "Mifi files(*.mif) | *.mif | Mifi files(*.mifi) | *.mifi | All Files(*.*) | *.* ");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        [System.Diagnostics.Contracts.Pure]
        public static string SelectMidiFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.mid";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".mid", "Midi files (*.mid)|*.mid|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static string SelectMusicXmlFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.xml";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".xml", "MusicXml files (*.xml)|*.xml|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected midi File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static string SelectMusicMxlFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.mxl";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".mxl", "MusicMxl files (*.mxl)|*.mxl|All Files (*.*)|*.*");
        }

        /// <summary> Select plan file to save. </summary>
        /// <param name="initialFolder"> The initial folder. </param>
        /// <returns> A string. </returns>
        public static string SelectMipiFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.mip";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".mip", "Plan files (*.mip)|*.mip|Plan files (*.mipi)|*.mipi|All Files (*.*)|*.*");
        }

        /// <summary>
        /// Open selected harmonic File.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static string SelectHarmonicFileToSave(string initialFolder) {
            if (string.IsNullOrEmpty(initialFolder)) {
                initialFolder = "*.mih";
            }

            return SupportFiles.SelectFileToSave(initialFolder, ".mih", "Harmony files (*.mih)|*.mih|All Files (*.*)|*.*");
        }

        #endregion
    }
}
