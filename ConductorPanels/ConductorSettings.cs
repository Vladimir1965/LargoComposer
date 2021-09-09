// <copyright file="ConductorSettings.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Musical Settings.
    /// </summary>
    public class ConductorSettings
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static ConductorSettings internalSingleton = new ConductorSettings();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConductorSettings"/> class.
        /// </summary>
        public ConductorSettings() {
            this.LoadDefaultValues();
        }

        #endregion

        #region Static properties
        /// <summary>
        /// Gets or sets the ProcessLogger Singleton.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Singleton MusicalSettings is null.</exception>
        public static ConductorSettings Singleton {
            get {
                Contract.Ensures(Contract.Result<ConductorSettings>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton ManagerSettings is null.");
                }

                return internalSingleton;
            }

            set => internalSingleton = value;
        }
        #endregion

        #region General Properties
        /// <summary>
        /// Gets or sets the folders.
        /// </summary>
        /// <value>
        /// The folders.
        /// </value>
        public SettingsFolders Folders { get; set; }

        /// <summary>
        /// Gets or sets the path settings.
        /// </summary>
        /// <value>
        /// The path settings.
        /// </value>
        public string PathSettings { get; set; }

        /// <summary>
        /// Gets or sets the path to music list.
        /// </summary>
        /// <value>
        /// The path to music list.
        /// </value>
        public string PathToMusicList { get; set; }

        /// <summary>
        /// Gets or sets CultureInfo.
        /// </summary>
        /// <value> General musical property.</value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the settings import.
        /// </summary>
        /// <value>
        /// The settings import.
        /// </value>
        public SettingsImport SettingsImport { get; set; }

        /// <summary>
        /// Gets or sets the path to sound-fonts.
        /// </summary>
        /// <value>
        /// The path to sound-fonts.
        /// </value>
        public string PathToSoundfonts { get; set; }

        /// <summary>
        /// Gets or sets the path to internal converter.
        /// </summary>
        /// <value>
        /// The path to internal converter.
        /// </value>
        public string PathToInternalConverter { get; set; }

        /// <summary>
        /// Gets or sets the path to internal stream.
        /// </summary>
        /// <value>
        /// The path to internal stream.
        /// </value>
        public string PathToInternalStream { get; set; }

        /// <summary>
        /// Gets or sets the path to status file.
        /// </summary>
        /// <value>
        /// The path to status file.
        /// </value>
        public string PathToStatusFile { get; set; }

        #endregion

        /// <summary>
        /// Loads the default values.
        /// </summary>
        public void LoadDefaultValues()
        {
            var musicFolder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
            this.PathToMusicList = musicFolder;
            this.PathToInternalConverter = Path.Combine(musicFolder, "Converter");
            this.PathToSoundfonts = Path.Combine(this.PathToInternalConverter, "Soundfonts");
            this.PathSettings = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            this.PathToInternalStream = Path.Combine(musicFolder, "Stream");
            this.PathToStatusFile = Path.Combine(this.PathToInternalStream, "Status.txt");
        }

        #region MusicalSettings - Load/Save
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        public void Load() {
            var path = this.PathSettings;
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "ConductorSettings") {
                return;
            }

            var xsetup = root;
            this.Read(xsetup);
        }

        /// <summary>
        /// Saves the musical settings.
        /// </summary>
        public void Save() {
            var xsetup = this.Write();
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xsetup);
            xdoc.Save(this.PathSettings);
        }

        /// <summary>
        /// Reads the musical settings.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void Read(XContainer markSettings) { //// XElement
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            XElement ximport = markSettings.Element("Import");

            ////  Import
            this.SettingsImport.SetXElement(ximport);
        }

        /// <summary>
        /// Writes the musical setup.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public XElement Write() {
            XElement xsetup = new XElement("ConductorSettings", null);

            //// Program settings, musical limit
            XElement ximport = this.SettingsImport.GetXElement;
            xsetup.Add(ximport);

            return xsetup;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "ConductorSettings";
        }
        #endregion
    }
}
