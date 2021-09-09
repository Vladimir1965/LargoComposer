// <copyright file="PortMusicMxl.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.MusicXml;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Port
{
    /// <summary>
    /// Port Music zipped.
    /// </summary>
    /// <seealso cref="LargoSharedClasses.Port.PortAbstract" />
    public class PortMusicMxl : PortAbstract
    {
        #region Public methods
        /// <summary>
        /// Loads from files.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public override void LoadFromFiles(string givenPath) {
            var files = FileDialogs.OpenSelectedMusicMxlFiles(MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport));
            if (files == null) {
                return;
            }

            var logger = WindowManager.OpenWindow("LargoSharedWindows", "InherentLogger", null);
            Array.ForEach(files, filepath => PortAbstract.LoadFromSourceFile(filepath, MusicalSourceType.MusicMXL, false));
            logger?.Close();

            FileInfo fi = new FileInfo(files[0]);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, fi.DirectoryName);
        }

        /// <summary>
        /// Extract Musical File from the Music Xml.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public override MusicalBundle ReadMusicFile(string filePath, string internalName) {
            Contract.Requires(filePath != null);
            Contract.Requires(filePath.Length != 0);
            if (string.IsNullOrEmpty(filePath)) {
                return null;
            }

            ProcessLogger.Singleton.SendMessageEvent(filePath, LocalizedMusic.String("Reading MusicMxl file ... "), 0);
            var tempFolder = Path.GetTempPath();
            var subfolder = Guid.NewGuid().ToString();
            var subfolderPath = Path.Combine(tempFolder, subfolder);
            if (string.IsNullOrEmpty(subfolderPath)) {
                return null;
            }

            Directory.CreateDirectory(subfolderPath);
            ZipFile.ExtractToDirectory(filePath, subfolderPath);
            //// ZipFileCover.UnzipFile(path, subfolderPath);

            var fi = SupportFiles.LatestFile(subfolderPath, "*.xml");
            if (fi == null) {
                return null;
            }

            var musicXmlReader = new MusicXmlReader();
            var musicXmlDocument = XDocument.Load(fi.FullName);
            var musicalBundle = musicXmlReader.ExtractMusicalFile(musicXmlDocument, internalName, PortAbstract.SettingsImport);

            //// string name = Path.GetFileNameWithoutExtension(path);
            //// musicalBundle.FileName = internalName.ClearSpecialChars(); //// name ?? "Unknown";

            return musicalBundle;
        }

        /// <summary>
        /// Write To Musical File.
        /// </summary>
        /// <param name="musicalBundle">Musical file.</param>
        /// <param name="path">File path.</param>
        /// <returns> Returns value. </returns>
        public override bool WriteMusicFile(MusicalBundle musicalBundle, string path) {
            Contract.Requires(musicalBundle != null);
            Contract.Requires(!string.IsNullOrEmpty(path));

            var tempFolder = Path.GetTempPath();
            var subfolder = Guid.NewGuid().ToString();
            var subfolderPath = Path.Combine(tempFolder, subfolder);
            Directory.CreateDirectory(subfolderPath);
            var name = Path.GetFileNameWithoutExtension(path);
            var xmlFileName = Path.Combine(subfolderPath, name + ".xml");

            var port = PortAbstract.CreatePort(MusicalSourceType.MusicXML);
            port.WriteMusicFile(musicalBundle, xmlFileName);

            var metaInfoPath = Path.Combine(subfolderPath, "META-INF");
            Directory.CreateDirectory(metaInfoPath);

            var containerFileName = Path.Combine(metaInfoPath, "container.xml");
            var metaFileString = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><container> <rootfiles> <rootfile full-path=\"{0}.xml\"> </rootfile> </rootfiles> </container> ", name);
            var encoding = Encoding.GetEncoding("UTF-8");

            using (var file = new FileStream(containerFileName, FileMode.Create)) {
                using (var sw = new StreamWriter(file, encoding)) {
                    sw.Write(metaFileString);
                    sw.Flush();
                }

                //// file.Flush(); Cannot access the closed file
                //// file.Close();
            }

            ZipFile.CreateFromDirectory(subfolderPath, path);
            //// ZipFileCover.CreateZipFileRecursive(subfolderPath, path, false, true);
            return true;
        }

        /// <summary>
        /// Saves a bundle.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <returns> Returns value. </returns>
        public override bool SaveBundle(MusicalBundle musicalBundle) {
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var defaultFilePath = Path.Combine(path, musicalBundle.FileName + ".mxl");
            this.DestinationFilePath = FileDialogs.SelectMusicMxlFileToSave(defaultFilePath);
            //// var mxlFileName = Path.Combine(MusicalSettings.Singleton.Folders.MusicMxl, this.resultName + ".mxl");

            //// 2018/09, 2018/10
            if (string.IsNullOrEmpty(this.DestinationFilePath)) {
                return false;
            }

            return this.WriteMusicFile(musicalBundle, this.DestinationFilePath);
        }
        #endregion
    }
}
