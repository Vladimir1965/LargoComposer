// <copyright file="PortDocuments.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.IO;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Data Interface.
    /// </summary>
    public class PortDocuments
    {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly PortDocuments InternalSingleton = new PortDocuments();

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the PortDocuments class from being created.
        /// </summary>
        private PortDocuments()
        {
            this.BlockDocumentMaster = new MusicDocumentMaster();
            this.ResultDocumentMaster = new MusicDocumentMaster();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static PortDocuments Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<PortDocuments>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton DataInterface is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Properties

        /// <summary> Gets or sets the block document master. </summary>
        /// <value> The block document master. </value>
        public MusicDocumentMaster BlockDocumentMaster { get; set; }

        /// <summary> Gets or sets the result document master. </summary>
        /// <value> The result document master. </value>
        public MusicDocumentMaster ResultDocumentMaster { get; set; }

        /// <summary> Gets the name of the selected music interchange file. </summary>
        /// <value> The name of the selected music interchange file. </value>
        public string SelectedMifPath
        {
            get
            {
                var document = this.SelectedDocument;
                var filePath = document.FilePath;
                //// var folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
                //// var fileName = document.Header.DocumentFileName ?? "No name";
                //// var filePath = Path.Combine(folder, fileName.Trim() + ".mif");
                return filePath;
            }
        }

        /// <summary>
        /// Gets or sets the selected block.
        /// </summary>
        /// <value>
        /// The selected block.
        /// </value>
        public MusicDocument SelectedDocument { get; set; }
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets the musical file.
        /// </summary>
        /// <value>
        /// The musical file.
        /// </value>
        public MusicalBundle MusicalBundle { get; set; }

        /// <summary>
        /// Gets or sets the musical block.
        /// </summary>
        /// <value>
        /// The musical block.
        /// </value>
        public MusicalBlock MusicalBlock { get; set; }

        /// <summary>
        /// Gets or sets the name of the music interchange file.
        /// </summary>
        /// <value>
        /// The name of the editor file music interchange file.
        /// </value>
        public string FilePath { get; set; }
        #endregion

        #region Public methods Documents
        /// <summary>
        /// Loads the documents.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool LoadDocuments(string givenPath) {
            return this.BlockDocumentMaster.LoadDocuments(givenPath, "UserListDocuments.xml", "Documents");
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        public void SaveDocuments() {
            this.BlockDocumentMaster.SaveDocuments("Documents");
        }

        #endregion

        #region Public methods Results
        /// <summary>
        /// Loads the documents.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool LoadResults(string givenPath)
        {
            return this.ResultDocumentMaster.LoadDocuments(givenPath, "UserListResults.xml", "Documents");
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        public void SaveResults()
        {
            this.ResultDocumentMaster.SaveDocuments("Documents");
        }

        #endregion

        #region Public methods - Blocks
        /// <summary>
        /// Loads a document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="quietly">if set to <c>true</c> [quietly].</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool LoadDocument(MusicDocument document, bool quietly) {
            PortDocuments.Singleton.SelectedDocument = document;
            if (document == null) {
                return false;
            }

            //// string folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            //// var filePath = Path.Combine(folder, document.Header.DocumentFileName + ".mif");
            if (!this.LoadBundle(document.FilePath, quietly)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="quietly">if set to <c>true</c> [quietly].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool LoadBundle(string filePath, bool quietly) {
            var extension = Path.GetExtension(filePath);
            if (extension == null) {
                return false;
            }

            if (PortAbstract.IsMusicalFile(extension)) {
                this.FilePath = filePath;
                var musicBundle = PortAbstract.LoadFromSourceFile(filePath, quietly);
                if (musicBundle == null) {
                    return false;
                }

                this.MusicalBundle = musicBundle;
                this.MusicalBlock = this.MusicalBundle.Blocks[0];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Bundles the loaded.
        /// </summary>
        /// <param name="musicBundle">The music bundle.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool BundleLoaded(MusicalBundle musicBundle) {
            if (musicBundle == null) {
                return false;
            }

            if (musicBundle.Blocks.Count == 0) {
                return false;
            }

            this.MusicalBundle = musicBundle;
            this.MusicalBlock = this.MusicalBundle.Blocks[0];
            //// this.MusicalBlock.Header.FilePath = this.FilePath;
            return true;
        }

        #endregion
    }
}
