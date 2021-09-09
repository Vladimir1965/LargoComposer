 // <copyright file="MusicalSettings.cs" company="Traced-Ideas, Czech republic">
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
    public class MusicalSettings
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static MusicalSettings internalSingleton = new MusicalSettings();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalSettings"/> class.
        /// </summary>
        public MusicalSettings() {
            this.SettingsProgram = new SettingsProgram();
            //// this.SettingsImport = new SettingsImport();
            //// this.SidePanels = new SidePanels();
            this.SettingsComposition = new SettingsComposition();
            this.SettingsAnalysis = new SettingsAnalysis();
            this.InitializeCultureInfo();
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
        public static MusicalSettings Singleton {
            get {
                Contract.Ensures(Contract.Result<MusicalSettings>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton MusicalSettings is null.");
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
        /// Gets or sets CultureInfo.
        /// </summary>
        /// <value> General musical property.</value>
        public CultureInfo CultureInfo { get; set; }
        #endregion

        #region Public properties - settings sessions
        /// <summary>
        /// Gets or sets the settings program.
        /// </summary>
        /// <value>
        /// The settings program.
        /// </value>
        public SettingsProgram SettingsProgram { get; set; }

        /// <summary>
        /// Gets or sets the settings composition.
        /// </summary>
        /// <value>
        /// The settings composition.
        /// </value>
        public SettingsComposition SettingsComposition { get; set; }

        /// <summary>
        /// Gets or sets the settings analysis.
        /// </summary>
        /// <value>
        /// The settings analysis.
        /// </value>
        public SettingsAnalysis SettingsAnalysis { get; set; }
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
            var defaultFolders = new SettingsFolders(providerName, programName); 
            var setupFolder = defaultFolders.GetFolder(MusicalFolder.InternalSettings);
            var folders = defaultFolders;

            var path = Path.Combine(setupFolder, "MusicalSettings.xml");
            var settings = new MusicalSettings {
                PathSettings = path
            };
            settings.Load();
            settings.Folders = folders;
            Singleton = settings;

            return true;
        }
        
        #endregion

        #region Folders
        /// <summary>
        /// Saves the musical folders.
        /// </summary>
        /// <param name="folders">The folders.</param>
        public static void SaveMusicalFolders(SettingsFolders folders) {
            var settings = Singleton;
            string pathSettings = settings.Folders.GetFolder(MusicalFolder.InternalSettings);
            var path = Path.Combine(pathSettings, "SettingsFolders.xml");
            XElement xfolders = folders.GetXmlElement;
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xfolders);
            xdoc.Save(path);
        }
        #endregion

        #region MusicalSettings - Load/Save
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        public void Load() {
            var path = this.PathSettings;
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "MusicalSettings") {
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

            XElement xprogram = markSettings.Element("Program");
            XElement xanalysis = markSettings.Element("Analysis");
            XElement xcomposition = markSettings.Element("Composition");

            ////  Program
            this.SettingsProgram.SetXElement(xprogram);

            ////  Analysis
            this.SettingsAnalysis.SetXElement(xanalysis);

            //// Composition
            this.SettingsComposition.SetXElement(xcomposition);

            this.InitializeCultureInfo();
        }

        /// <summary>
        /// Writes the musical setup.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public XElement Write() {
            XElement xsetup = new XElement("MusicalSettings", null);

            //// Program settings, musical limit
            XElement xprogram = this.SettingsProgram.GetXElement;
            xsetup.Add(xprogram);

            ////  Analysis
            XElement xanalysis = this.SettingsAnalysis.GetXElement;
            xsetup.Add(xanalysis);

            //// Composition
            XElement xcomposition = this.SettingsComposition.GetXElement;
            xsetup.Add(xcomposition);

            return xsetup;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "MusicalSettings";
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the culture info.
        /// </summary>
        public void InitializeCultureInfo() {
            var cultureIdent = "en-US";
            switch (this.SettingsProgram.DefaultCulture) {
                case 0: {
                        cultureIdent = "en-US";
                        break;
                    }

                case 1: {
                        cultureIdent = "it-IT";
                        break;
                    }

                case 2: {
                        cultureIdent = "cs-CZ";
                        break;
                    }

                ////  Unused localizations: "de-DE", "fr-FR", "pl-PL", "es-ES"
                //// resharper default: break;
            }

            this.CultureInfo = new CultureInfo(cultureIdent);
        }

        #endregion
    }
}
