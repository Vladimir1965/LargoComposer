// <copyright file="SettingsFolders.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using Abstract;

    /// <summary>
    /// Musical Folders.
    /// </summary>
    public class SettingsFolders {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SettingsFolders internalSingleton;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFolders" /> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="programName">Name of the program.</param>
        public SettingsFolders(string providerName, string programName) {
            Singleton = this;
            this.Data = new Dictionary<MusicalFolder, string>();
            this.Provider = providerName;
            this.Program = programName;

            this.ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            //// this.CommonData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //// this.UserData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.UserDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //// Binaries
            this.SetFolder(MusicalFolder.BinaryFolder, @"#ProgramFiles/#Provider/#Program/Binary");
            this.SetFolder(MusicalFolder.Licenses, @"#ProgramFiles/#Provider/#Program/Licenses");

            //// Factory folders
            this.SetFolder(MusicalFolder.FactoryData, @"#ProgramFiles/#Provider/#Program/InternalData");
            this.SetFolder(MusicalFolder.FactorySettings, @"#ProgramFiles/#Provider/#Program/InternalSettings");
            this.SetFolder(MusicalFolder.FactoryMusic, @"#ProgramFiles/#Provider/#Program/InternalMusic");
            this.SetFolder(MusicalFolder.FactoryTemplates, @"#ProgramFiles/#Provider/#Program/InternalTemplates");

            //// Data folders
            this.SetFolder(MusicalFolder.InternalData, @"#UserDocs/#Provider/#Program/InternalData");
            this.SetFolder(MusicalFolder.InternalTemplates, @"#UserDocs/#Provider/#Program/InternalTemplates");
            this.SetFolder(MusicalFolder.InternalMusic, @"#UserDocs/#Provider/#Program/InternalMusic");
            this.SetFolder(MusicalFolder.InternalSettings, @"#UserDocs/#Provider/#Program/InternalSettings");

            this.SetFolder(MusicalFolder.Errors, @"#UserDocs/#Provider/#Program/InternalData/Errors");
            this.SetFolder(MusicalFolder.Temporary, @"#UserDocs/#Provider/#Program/InternalMusic/Temp");
            this.SetFolder(MusicalFolder.UserMusic, @"#UserDocs/#Provider/#Program/UserMusic");

            this.CheckFolders();
        }

        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFolders" /> class.
        /// </summary>
        /// <param name="xfolders">The element of folders.</param>
        /// <param name="defaultFolders">The default folders.</param>
        public SettingsFolders(XElement xfolders, SettingsFolders defaultFolders) { //// XElement
            Contract.Requires(xfolders != null);
            if (xfolders == null) {
                return;
            }

            Singleton = this;
            this.Data = new Dictionary<MusicalFolder, string>();
            this.Provider = defaultFolders.Provider;
            this.Program = defaultFolders.Program;
            var xlist = xfolders.Element("List");

            for (int i = 1; i < (int)MusicalFolder.EndOfFolders; i++) {
                var mf = (MusicalFolder)i;
                if (!this.AddXFolder(mf, xlist)) {
                    var path = defaultFolders.GetFolder(mf);
                    this.SetFolder(mf, path);
                }
            }

            this.CheckFolders();
        }
        */

        #endregion

        #region Static properties
        /// <summary>
        /// Gets or sets the ProcessLogger Singleton.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Singleton MusicalSettings is null.</exception>
        public static SettingsFolders Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<SettingsFolders>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton SettingsFolders is null.");
                }

                return internalSingleton;
            }

            set => internalSingleton = value;
        }

        #endregion

        #region Public Properties (Folders)
        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the program.
        /// </summary>
        /// <value>
        /// The program.
        /// </value>
        public string Program { get; set; }

        /// <summary>
        /// Gets or sets MusicDirectory.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public string ProgramFiles { get; set; }

        //// public string CommonData { get; set; }
        //// public string UserData { get; set; }

        /// <summary>
        /// Gets or sets the user documents.
        /// </summary>
        /// <value>
        /// The user documents.
        /// </value>
        public string UserDocuments { get; set; }

        /// <summary>
        /// Gets the get XML element.
        /// </summary>
        /// <value>
        /// The get XML element.
        /// </value>
        public XElement GetXmlElement {
            get {
                XElement xfolders = new XElement("Folders", null);
                XElement xlist = new XElement("List", null);
                for (int i = 1; i < (int)MusicalFolder.EndOfFolders; i++) {
                    var musicFolder = (MusicalFolder)i;
                    var folder = this.GetFolder(musicFolder);
                    if (folder != null) {
                        xlist.Add(new XAttribute(musicFolder.ToString(), folder));
                    }
                }

                xfolders.Add(xlist);

                return xfolders;
            }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public List<KeyValuePair> List {
            get {
                var data = this.Data;
                var list = new List<KeyValuePair>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var record in data) {
                    var item = new KeyValuePair(record.Key.ToString(), record.Value);
                    list.Add(item);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the get temporary directory.
        /// </summary>
        /// <value>
        /// The get temporary directory.
        /// </value>
        public string GetTemporaryFolder {
            get {
                var path = Path.Combine(this.GetFolder(MusicalFolder.Temporary), Path.GetRandomFileName());
                if (string.IsNullOrEmpty(path)) {
                    return null;
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        private Dictionary<MusicalFolder, string> Data { get; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "SettingsFolders";
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Sets the folder.
        /// </summary>
        /// <param name="givenFolder">The given folder.</param>
        /// <param name="givenRelativePath">The given relative path.</param>
        public void SetFolder(MusicalFolder givenFolder, string givenRelativePath) {
            var path = this.DecodePath(givenRelativePath);

            if (this.Data.ContainsKey(givenFolder)) {
                this.Data[givenFolder] = path;
            }
            else {
                this.Data.Add(givenFolder, path);
            }
        }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <param name="givenFolder">The given folder.</param>
        /// <returns> Returns value. </returns>
        public string GetFolder(MusicalFolder givenFolder) {
            string path = null;
            if (this.Data.ContainsKey(givenFolder)) {
                path = this.Data[givenFolder];
            }

            return path;
        }

        /// <summary>
        /// Checks the folders.
        /// </summary>
        /// <returns> Returns value. </returns>
        public bool CheckFolders() {
            bool result = true;
            foreach (var key in this.Data.Keys) {
                var path = this.Data[key];
                if (!string.IsNullOrWhiteSpace(path) && !Directory.Exists(path)) {                    
                        try { 
                            Directory.CreateDirectory(path);
                        }
                        catch (UnauthorizedAccessException) {
                            MessageBox.Show(string.Format("Expected directory does not exist.\n\n {0}", path), SettingsApplication.ApplicationName);
                            result = false;
                        }
                }
            }

            return result;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Adds the x folder.
        /// </summary>
        /// <param name="givenFolder">The given folder.</param>
        /// <param name="xfolders">The folders.</param>
        /// <returns> Returns value. </returns>
        private bool AddXFolder(MusicalFolder givenFolder, XElement xfolders) {
            var path = XmlSupport.ReadStringAttribute(xfolders.Attribute(givenFolder.ToString()));

            this.SetFolder(givenFolder, path);
            return true;
        }

        /// <summary>
        /// Objects the invariant.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(this.Program != null);
        }
        #endregion

        /// <summary>
        /// Decodes the path.
        /// </summary>
        /// <param name="givenRelativePath">The given relative path.</param>
        /// <returns> Returns value. </returns>
        private string DecodePath(string givenRelativePath) {
            //// var myMusic = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var path = givenRelativePath.Replace("#UserDocs", this.UserDocuments);
            //// path = givenRelativePath.Replace("#UserData", this.UserData);
            path = path.Replace("#ProgramFiles", this.ProgramFiles); 
            path = path.Replace("#Provider", this.Provider);
            path = path.Replace("#Program", this.Program);
            path = path.Replace("/", "\\");
            return path;
        }
    }
}
