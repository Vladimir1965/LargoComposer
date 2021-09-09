// <copyright file="PortAbstract.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Localization;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;

namespace LargoSharedClasses.Port
{
    /// <summary>
    /// Port Abstract.
    /// </summary>
    public class PortAbstract
    {
        /// <summary>
        /// Gets or sets the settings import.
        /// </summary>
        /// <value>
        /// The settings import.
        /// </value>
        public static SettingsImport SettingsImport { get; set; }

        /// <summary>
        /// Gets or sets the destination file path.
        /// </summary>
        /// <value>
        /// The destination file path.
        /// </value>
        public string DestinationFilePath { get; set; }

        /// <summary>
        /// Sources the type of extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalSourceType SourceTypeOfExtension(string extension) {
            Contract.Requires(extension != null);

            var mst = MusicalSourceType.None;
            switch (extension.ToUpperInvariant()) {
                case ".MID":
                    mst = MusicalSourceType.MIDI;
                    break;
                case ".MIDI":
                    mst = MusicalSourceType.MIDI;
                    break;
                case ".KAR":
                    mst = MusicalSourceType.MIDI;
                    break;
                case ".MXL":
                    mst = MusicalSourceType.MusicMXL;
                    break;
                case ".XML":
                    mst = MusicalSourceType.MusicXML;
                    break;
                case ".MIF": //// music interchange file,  ".LARGO":
                    mst = MusicalSourceType.MIFI;
                    break;
            }

            return mst;
        }

        /// <summary>
        /// Determines whether [is musical file] [the specified extension].
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        ///   <c>true</c> if [is musical file] [the specified extension]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMusicalFile(string extension) {
            MusicalSourceType sourceType = SourceTypeOfExtension(extension);
            return (sourceType == MusicalSourceType.MIDI) || (sourceType == MusicalSourceType.MusicXML)
            || (sourceType == MusicalSourceType.MusicMXL) || (sourceType == MusicalSourceType.MIFI);
        }

        /// <summary>
        /// Creates the port.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns> Returns value. </returns>
        public static PortAbstract CreatePort(MusicalSourceType sourceType) {
            PortAbstract port;
            switch (sourceType) {
                case MusicalSourceType.MIDI: {
                        port = new PortMidi();
                    }

                    break;
                case MusicalSourceType.MusicXML: {
                        port = new PortMusicXml();
                    }

                    break;
                case MusicalSourceType.MusicMXL: {
                        port = new PortMusicMxl();
                    }

                    break;
                case MusicalSourceType.MIFI: {
                        port = new PortMifi();
                    }

                    break;
                default:
                    return null;
            }

            return port;
        }

        #region Public static methods - Abstract loader
        /// <summary>
        /// Loads from source file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="quietly">if set to <c>true</c> [quietly].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalBundle LoadFromSourceFile(string filePath, MusicalSourceType sourceType, bool quietly) {
            Contract.Requires(!string.IsNullOrEmpty(filePath));
            //// MessageBox.Show("Path:" + filePath, "Loading", MessageBoxButton.OK, MessageBoxImage.Information);

            var ext = Path.GetExtension(filePath);
            var mst = PortAbstract.SourceTypeOfExtension(ext);
            if (mst != sourceType) {
                MessageBox.Show(LocalizedControls.String("Invalid musical file type."), SettingsApplication.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            MusicalBundle musicalBundle;
            //// if (sourceType == MusicalSourceType.Largo) {
            //// 2017/10 musicalBundle = MifiFilePorter.LoadMifiFile(filePath);
            //// }  else { 
            var internalName = Path.GetFileNameWithoutExtension(filePath);
            //// internalName = internalName.ClearSpecialChars();

            try {
                var port = PortAbstract.CreatePort(sourceType);
                musicalBundle = port.ReadMusicFile(filePath, internalName);
                if (musicalBundle == null) {
                    MessageBox.Show(LocalizedControls.String("Import of musical file failed"), SettingsApplication.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            catch (MidiParserException ex) {
                MessageBox.Show(ex.Message, SettingsApplication.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            //// FileModel fileModel = MusicalSettings.Singleton.AnalyzeAfterLoad ? MusicAnalyzer.AnalyzeFile(musicalBundle) : null;
            //// if (fileModel == null) {  return null; } 
            //// LastlyLoadedMusicalBundle = musicalBundle;
            musicalBundle.OriginalPath = filePath;

            if (!quietly) {
                CommandEventSender.Singleton.SendBundleEvent(musicalBundle, ObjectOperation.ObjectLoaded);
            }

            return musicalBundle;
        }

        /// <summary>
        /// Loads from source file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="quietly">if set to <c>true</c> [quietly].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalBundle LoadFromSourceFile(string filePath, bool quietly) {
            var extension = Path.GetExtension(filePath);
            if (extension != null) {
                var ext = extension.ToUpper();
                MusicalSourceType sourceType = PortAbstract.SourceTypeOfExtension(ext);
                return LoadFromSourceFile(filePath, sourceType, quietly);
            }

            return null;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads from files.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public virtual void LoadFromFiles(string givenPath) {
            return;
        }

        /// <summary>
        /// Reads the music file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns> Returns value. </returns>
        public virtual MusicalBundle ReadMusicFile(string filePath, string internalName) {
            return null;
        }

        /// <summary>
        /// Writes the music file.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <param name="path">The path.</param>
        /// <returns> Returns value. </returns>
        public virtual bool WriteMusicFile(MusicalBundle musicalBundle, string path) {
            return false;
        }

        /// <summary>
        /// Saves the bundle.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <returns> Returns value. </returns>
        public virtual bool SaveBundle(MusicalBundle musicalBundle) {
            return false;
        }
        #endregion
    }
}
