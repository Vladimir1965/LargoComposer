// <copyright file="EditorSettings.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Settings;

namespace EditorPanels
{
    /// <summary>
    /// Musical Settings.
    /// </summary>
    public class EditorSettings
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static EditorSettings internalSingleton = new EditorSettings();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorSettings"/> class.
        /// </summary>
        public EditorSettings() {
            this.PathSettings = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            this.PathToMusicList = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
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
        public static EditorSettings Singleton {
            get {
                Contract.Ensures(Contract.Result<EditorSettings>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton EditorSettings is null.");
                }

                return internalSingleton;
            }

            set => internalSingleton = value;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the settings import.
        /// </summary>
        /// <value>
        /// The settings import.
        /// </value>
        public SidePanels SidePanels { get; set; }

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

        #endregion

        #region MusicalSettings - Load/Save
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        public void Load() {
            var path = this.PathSettings;
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "MainSettings") {
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
            this.SidePanels.SetXElement(ximport);
        }

        /// <summary>
        /// Writes the musical setup.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public XElement Write() {
            XElement xsetup = new XElement("MainSettings", null);

            //// Program settings, musical limit
            XElement ximport = this.SidePanels.GetXElement;
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
