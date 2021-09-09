// <copyright file="PlayerSettings.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Settings
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Xml.Linq;
    using LargoSharedClasses.Abstract;
    
    /// <summary>
                                          /// Musical Settings.
                                          /// </summary>
    public class PlayerSettings
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static PlayerSettings internalSingleton = new PlayerSettings();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSettings"/> class.
        /// </summary>
        public PlayerSettings() {
            this.SettingsImport = new SettingsImport();

            ////this.PathToInternalTemplates = @"c:\Users\Krakonoš\Documents\Indefinite Software\Largo 2021\LargoPlayer\InternalTemplates";            
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
        public static PlayerSettings Singleton {
            get {
                Contract.Ensures(Contract.Result<PlayerSettings>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton PlayerSettings is null.");
                }

                return internalSingleton;
            }

            set => internalSingleton = value;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path settings.
        /// </summary>
        /// <value>
        /// The path settings.
        /// </value>
        public string PathSettings { get; set; }

        /// <summary>
        /// Gets or sets the settings import.
        /// </summary>
        /// <value>
        /// The settings import.
        /// </value>
        public SettingsImport SettingsImport { get; set; }

        /// <summary>
        /// Gets or sets the path to music list.
        /// </summary>
        /// <value>
        /// The path to music list.
        /// </value>
        public string PathToMusicList { get; set; }

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

        #endregion

        #region Public static factory methods

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="programName">Name of the program.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool LoadSettingsStartup(string providerName, string programName) {
            /*
            var defaultFolders = new SettingsFolders(providerName, programName);
            var setupFolder = defaultFolders.GetFolder(MusicalFolder.LargoSettings);
            var path = Path.Combine(setupFolder, "SettingsFolders.xml");

            var folders = LoadMusicalFolders(path, defaultFolders);
            if (folders == null) {
                return false;
            }

            if (!folders.CheckFolders()) {
                return false;
            }*/

            var defaultFolders = new SettingsFolders(providerName, programName);
            var folder = defaultFolders.GetFolder(MusicalFolder.InternalSettings);
            var path = Path.Combine(folder, "ConductorSettings.xml");
            var settings = new PlayerSettings {
                PathSettings = path
            };
            settings.Load();
            //// settings.Folders = folders;
            Singleton = settings;

            return true;
        }

        #endregion

        /// <summary>
        /// Loads the default values.
        /// </summary>
        public void LoadDefaultValues()
        {
            var musicFolder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
            this.PathToMusicList = musicFolder;
            this.PathToInternalConverter = Path.Combine(musicFolder, "Converter");
            this.PathToSoundfonts = Path.Combine(PlayerSettings.Singleton.PathToInternalConverter, "Soundfonts");
            this.PathSettings = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
        }

        #region MusicalSettings - Load/Save
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        public void Load() {
            var path = this.PathSettings;
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "PlayerSettings") {
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
            XElement xsetup = new XElement("PlayerSettings", null);

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
            return "ManagerSettings";
        }
        #endregion
    }
}
