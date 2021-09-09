// <copyright file="ManagerSettings.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Settings;

namespace ManagerPanels
{
    /// <summary>
    /// Musical Settings.
    /// </summary>
    public class ManagerSettings
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static ManagerSettings internalSingleton = new ManagerSettings();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerSettings"/> class.
        /// </summary>
        public ManagerSettings() {
            this.SettingsImport = new SettingsImport();
            this.SkipNegligibleLines = true;
            this.SaveInternalTemplates = true;
            this.PathToInternalTemplates = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
            var folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            var path = Path.Combine(folder, "ManagerSettings.xml");
            this.PathSettings = path;
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
        public static ManagerSettings Singleton {
            get {
                Contract.Ensures(Contract.Result<ManagerSettings>() != null);
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
        /// Gets or sets CultureInfo.
        /// </summary>
        /// <value> General musical property.</value>
        public CultureInfo CultureInfo { get; set; }
        #endregion

        #region Public properties - settings sessions

        /// <summary>
        /// Gets or sets the settings import.
        /// </summary>
        /// <value>
        /// The settings import.
        /// </value>
        public SettingsImport SettingsImport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip negligible lines].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip negligible lines]; otherwise, <c>false</c>.
        /// </value>
        public bool SkipNegligibleLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save internal templates].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save internal templates]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveInternalTemplates { get; set; }

        /// <summary>
        /// Gets or sets the path to internal templates.
        /// </summary>
        /// <value>
        /// The path to internal templates.
        /// </value>
        public string PathToInternalTemplates { get; set; }

        /// <summary>
        /// Gets or sets the path to music files.
        /// </summary>
        /// <value>
        /// The path to music files.
        /// </value>
        public string PathToMusicFiles { get; set; }
        #endregion

        #region MusicalSettings - Load/Save
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        public void Load() {
            var path = this.PathSettings;
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "ManagerSettings") {
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
            XElement xsetup = new XElement("ManagerSettings", null);

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
