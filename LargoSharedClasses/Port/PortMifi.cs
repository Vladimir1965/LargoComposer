// <copyright file="PortMifi.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System;
using System.IO;
using System.Xml.Linq;

namespace LargoSharedClasses.Port
{
     /// <summary>
    /// Port Music file interchange.
    /// </summary>
    /// <seealso cref="LargoSharedClasses.Port.PortAbstract" />
    public class PortMifi : PortAbstract
    {
        #region Public methods
        /// <summary>
        /// Loads from files.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public override void LoadFromFiles(string givenPath) {
            var files = FileDialogs.OpenSelectedMifiFiles(MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport));
            if (files == null) {
                return;
            }

            var logger = WindowManager.OpenWindow("LargoSharedWindows", "InherentLogger", null);
            Array.ForEach(files, filepath => PortAbstract.LoadFromSourceFile(filepath, MusicalSourceType.MIFI, false));
            logger?.Close();

            FileInfo fi = new FileInfo(files[0]);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, fi.DirectoryName);
        }

        /// <summary>
        /// Reads the Musical interface file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public override MusicalBundle ReadMusicFile(string filePath, string internalName) {
            if (!File.Exists(filePath)) {
                return null;
            }

            var xdoc = XDocument.Load(filePath);
            var root = xdoc.Root;
            if (root == null || root.Name != "MIF") {
                return null;
            }

            MusicalBundle musicalBundle = new MusicalBundle(root) {
                FileName = internalName.ClearSpecialChars() //// 2020/10 ujednoceno (nepoužívalo se internalName)
            };

            return musicalBundle;
        }

        /// <summary>
        /// Saves the Musical interface file.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <param name="path">The path.</param>
        /// <returns> Returns value. </returns>
        public override bool WriteMusicFile(MusicalBundle musicalBundle, string path) {
            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder)) {
                return false;
            }

            XElement xbundle = musicalBundle.GetXElement;
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xbundle);
            xdoc.Save(path);
            return true;
        }

        /// <summary>
        /// Saves a bundle.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <returns> Returns value. </returns>
        public override bool SaveBundle(MusicalBundle musicalBundle) {
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var defaultFilePath = Path.Combine(path, musicalBundle.FileName + ".mif");
            this.DestinationFilePath = FileDialogs.SelectMifiFileToSave(defaultFilePath);

            //// 2018/09, 2018/10
            if (string.IsNullOrEmpty(this.DestinationFilePath)) {
                return false;
            }

            return this.WriteMusicFile(musicalBundle, this.DestinationFilePath);
        }

        #endregion
    }
}
