// <copyright file="SupportFiles.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Support Files.
    /// </summary>
    public static class SupportFiles {
        #region Public static methods - Filesystem
        /// <summary>
        /// Gets the local directory.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static string GetTemporaryDirectory {
            get {
                var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (string.IsNullOrEmpty(path)) {
                    return null;
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// Gets the today files.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public static List<string> GetTodayFiles(string folderPath) {
            List<string> todayFiles = new List<string>();
            foreach (var file in Directory.GetFiles(folderPath)) {
                DirectoryInfo di = new DirectoryInfo(file);
                if (di.CreationTime.ToShortDateString().Equals(DateTime.Now.ToShortDateString())) {
                    todayFiles.Add(file);
                }
            }

            return todayFiles;
        }

        /// <summary> Latest file. </summary>
        /// <param name="path">File path.</param>
        /// <param name="mask">File mask.</param>
        /// <returns> Returns value. </returns>
        public static FileInfo LatestFile(string path, string mask) { //// to find Xml file for import
            Contract.Requires(path != null);
            var folder = new DirectoryInfo(path);
            var files = folder.GetFiles(mask, SearchOption.TopDirectoryOnly);
            //// Resharper:﻿Code is heuristically unreachable  (vs. CodeContracts  if (files == null) return null )

            var datum = DateTime.Now;
            FileInfo fileInfo = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var fi in files) {
                if (fi == null) {
                    continue;
                }

                if (datum.Ticks < fi.LastWriteTime.Ticks) {
                    continue; // is there older
                }

                datum = fi.LastWriteTime;
                fileInfo = new FileInfo(fi.FullName.Trim());
            }

            return fileInfo;
        }
        #endregion

        #region APublic static methods - abstract dialogs
        /// <summary>
        /// Open selected file.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="extension">File extension.</param>
        /// <param name="filter">Filter of files.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string OpenSelectedFile(string initialFolder, string fileName, string extension, string filter) {
            string filepath;
            OpenFileDialog dlg = null;
            try {
                dlg = new OpenFileDialog {
                    Title = Localization.LocalizedMusic.String("File selection"),
                    InitialDirectory = initialFolder,
                    FileName = fileName,
                    DefaultExt = extension,
                    Filter = filter,
                    Multiselect = false,
                    RestoreDirectory = false
                };
                var result = dlg.ShowDialog();
                filepath = result == DialogResult.OK ? string.IsNullOrEmpty(dlg.FileName) ? string.Empty : dlg.FileName : string.Empty;
            }
            catch (Exception) {
                filepath = string.Empty;
            }
            finally
            {
                dlg?.Dispose();
            }

            return filepath;
        }

        /// <summary>
        /// Open selected file.
        /// </summary>
        /// <param name="initialFolder">The initial folder.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="extension">File extension.</param>
        /// <param name="filter">Filter of files.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string[] OpenSelectedFiles(string initialFolder, string fileName, string extension, string filter) {
            string[] files;
            using (var dlg = new OpenFileDialog {
                Title = Localization.LocalizedMusic.String("Selection of files"),
                InitialDirectory = initialFolder,
                FileName = fileName,
                DefaultExt = extension,
                Filter = filter,
                Multiselect = true,
                RestoreDirectory = false
            }) {
                var result = dlg.ShowDialog();
                files = result == DialogResult.OK ? dlg.FileNames : null;
            }

            return files;
        }

        /// <summary>
        /// Select File To Save.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="extension">File extension.</param>
        /// <param name="filter">Filter of files.</param>
        /// <returns>Returns value.</returns>
        public static string SelectFileToSave(string fileName, string extension, string filter) {
            string filename;
            using (var dlg = new SaveFileDialog {
                Title = Localization.LocalizedMusic.String("File to save"),
                FileName = fileName,
                DefaultExt = extension,
                Filter = filter
            }) {
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                dlg.FilterIndex = 1;
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Directory != null)
                {
                    dlg.InitialDirectory = fileInfo.Directory.FullName;
                }

                //// In Windows Vista/Seven the behavior is always as FileDialog.RestoreDirectory = true 
                //// (yes, even if you set it to false...).
                dlg.RestoreDirectory = true;
                var result = dlg.ShowDialog();
                if (result != DialogResult.OK) {
                    return null;
                }

                filename = dlg.FileName;
            }

            return filename;
        }

        #endregion

        #region Public static methods - FileToString
        /// <summary> Read File. </summary>
        /// <returns> Returns value. </returns>
        /// <param name="filePath">File path.</param>
        /// <returns> Returns value. </returns>
        public static string FileToString(string filePath) {
            if (!File.Exists(filePath)) {
                return string.Empty;
            }

            using (var sr = new StreamReader(filePath)) {
                var s = sr.ReadToEnd();
                //// sr.Close();
                return s;
            }
        }

        /// <summary> Write string to file. </summary>
        /// <param name="content">Content of file.</param>
        /// <param name="filePath">File path.</param>
        public static void StringToFile(string content, string filePath) {
            using (var sw = new StreamWriter(filePath)) {
                sw.Write(content);
                //// sw.Close();
            }
        }

        #endregion

        #region Public static methods - compression
        /// <summary>
        /// Decompress g-zipped file.
        /// readStream is the stream you need to read
        /// writeStream is the stream you want to write to
        /// string filePath = Path.GetTempFileName() + ".text";
        /// Decompress(fileInfo, filePath);
        /// </summary>
        /// <param name="fileInfo">File info.</param>
        /// <param name="filePath">File path.</param>
        [UsedImplicitly]
        public static void Decompress(FileInfo fileInfo, string filePath) {
            Contract.Requires(fileInfo != null);
            Contract.Requires(filePath != null);

            if (fileInfo == null) {
                return;
            }

            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            //// Get the stream of the source file.
            using (var fileStream = fileInfo.OpenRead()) {
                //// Get original file extension, for example
                //// "doc" from report.doc.gz.
                var curFile = fileInfo.FullName;
                var origName = curFile.Remove(curFile.Length - fileInfo.Extension.Length);
                if (string.IsNullOrEmpty(origName)) {
                    return;
                }

                ///// Create the decompressed file.
                using (File.Create(origName)) {
                    var gzs = new GZipStream(fileStream, CompressionMode.Decompress);
                    ////  Copy the decompression stream 
                    ////  into the output file.
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                        CopyStream(gzs, fs);
                    }
                }
            }
        }
        #endregion

        #region Streams
        /// <summary>
        /// Copy Stream.
        /// </summary>
        /// <param name="readStream">Read Stream.</param>
        /// <param name="writeStream">Write Stream.</param>
        private static void CopyStream(Stream readStream, Stream writeStream) {
            Contract.Requires(readStream != null);
            Contract.Requires(writeStream != null);
            const int length = 256;
            var buffer = new byte[length];
            var bytesRead = readStream.Read(buffer, 0, length);
            //// write the required bytes
            while (bytesRead > 0) {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }

            readStream.Close();
            writeStream.Close();
        }
        #endregion
}
}
