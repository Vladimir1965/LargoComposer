// <copyright file="PortMusicXml.cs" company="Traced-Ideas, Czech republic">
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
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Port
{
     /// <summary>
    /// Port MusicXml.
    /// </summary>
    /// <seealso cref="LargoSharedClasses.Port.PortAbstract" />
    public class PortMusicXml : PortAbstract
    {
        #region Public methods
        /// <summary>
        /// Loads from files.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public override void LoadFromFiles(string givenPath) {
            var files = FileDialogs.OpenSelectedMusicXmlFiles(MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport));
            if (files == null) {
                return;
            }

            var logger = WindowManager.OpenWindow("LargoSharedWindows", "InherentLogger", null);
            Array.ForEach(files, filepath => PortAbstract.LoadFromSourceFile(filepath, MusicalSourceType.MusicXML, false));

            logger?.Close();

            FileInfo fi = new FileInfo(files[0]);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, fi.DirectoryName);
        }

        /// <summary>
        /// Read MusicXml file..
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public override MusicalBundle ReadMusicFile(string filePath, string internalName) {
            Contract.Requires(filePath != null);
            if (string.IsNullOrEmpty(filePath)) {
                return null;
            }

            //// string fileName = Path.GetFileNameWithoutExtension(path);
            //// WinAbstract logger = this.Window.EditorWindow.OpenWindow("InherentLogger", null);

            var musicXmlReader = new MusicXmlReader();
            var musicXmlDocument = XDocument.Load(filePath);
            ProcessLogger.Singleton.SendMessageEvent(Path.GetFileName(filePath), LocalizedMusic.String("Reading MusicXml file ... "), 0);
            var musicalBundle = musicXmlReader.ExtractMusicalFile(musicXmlDocument, internalName, PortAbstract.SettingsImport);
            //// musicalBundle.MidFileId = midiFile.Id;

            //// string name = Path.GetFileNameWithoutExtension(path);
            //// musicalBundle.FileName = internalName.ClearSpecialChars(); //// name ?? "Unknown";
            return musicalBundle;
        }

        /// <summary>
        /// Write To MusicXml File.
        /// </summary>
        /// <param name="musicalBundle">Musical file.</param>
        /// <param name="path">File path.</param>
        /// <returns> Returns value. </returns>
        public override bool WriteMusicFile(MusicalBundle musicalBundle, string path) {
            Contract.Requires(musicalBundle != null);
            Contract.Requires(!string.IsNullOrEmpty(path));

            var encoding = Encoding.GetEncoding("UTF-8");

            using (var file = new FileStream(path, FileMode.Create)) {
                using (var sw = new StreamWriter(file, encoding)) {
                    var mxml = new MusicXmlWriter(musicalBundle);
                    mxml.WriteTo(sw);
                    sw.Flush();
                }

                //// file.Flush(); //// cannot access the closed file
                //// file.Close(); //// Error while disposing!?!?
            }

            return true;
        }

        /// <summary>
        /// Saves a bundle.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <returns> Returns value. </returns>
        public override bool SaveBundle(MusicalBundle musicalBundle) {
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var defaultFilePath = Path.Combine(path, musicalBundle.FileName + ".xml");
            this.DestinationFilePath = FileDialogs.SelectMusicXmlFileToSave(defaultFilePath);
            //// var xmlFileName = Path.Combine(MusicalSettings.Singleton.Folders.MusicXml, this.resultName + ".xml");

            //// 2018/09, 2018/10
            if (string.IsNullOrEmpty(this.DestinationFilePath)) {
                return false;
            }

            return this.WriteMusicFile(musicalBundle, this.DestinationFilePath);
        }
        #endregion
    }
}
