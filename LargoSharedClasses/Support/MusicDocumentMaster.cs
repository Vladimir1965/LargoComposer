// <copyright file="MusicDocumentMaster.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Support
{
    /// <summary> A music document master. </summary>
    public class MusicDocumentMaster
    {
        /// <summary>
        /// Gets or sets the document path.
        /// </summary>
        /// <value>
        /// The document path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the block list.
        /// </summary>
        /// <value>
        /// The block list.
        /// </value>
        public List<MusicDocument> DocumentList { get; set; }

        #region Public methods Documents

        /// <summary>
        /// Loads the documents.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        /// <param name="rootName">Name of the root.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool LoadDocuments(string givenPath,  string xmlFileName, string rootName) {
            var list = new List<MusicDocument>();
            //// var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.LargoManager);
            //// var fileName = "UserDocuments.xml";
            this.FilePath = Path.Combine(givenPath, xmlFileName);
            if (File.Exists(this.FilePath)) {
                var xdoc = XDocument.Load(this.FilePath);
                var root = xdoc.Root;
                if (root == null || root.Name != rootName) {
                    return false;
                }

                var xlist = root;
                foreach (var xdocument in xlist.Elements()) {
                    MusicDocument document = new MusicDocument(xdocument);
                    if (document.IsValid) {
                        list.Add(document);
                    }
                }

                //// var orderByDescending = list.OrderByDescending(document => document?.Header?.Changed ?? DateTime.Now);
                this.DocumentList = list.Take(MusicalSettings.Singleton.SettingsProgram.NumberOfDocumentsToDisplay).ToList();
            }
            else {
                this.DocumentList = list;
            }

            return true; ////this.DocumentList != null;
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        public void SaveDocuments(string rootName) {
            if (this.DocumentList == null) {
                return;
            }

            XElement xlist = new XElement(rootName);
            foreach (var document in this.DocumentList) {
                if (!document.IsValid) {
                    continue;
                }

                var xdocument = document.GetXElement;
                xlist.Add(xdocument);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            xdoc.Save(this.FilePath);
        }

        /// <summary>
        /// Adds the blocks of bundle.
        /// </summary>
        /// <param name="musicBundle">The music bundle.</param>
        /// <param name="saveBlocks">if set to <c>true</c> [save blocks].</param>
        public void AddBlocksOfBundle(MusicalBundle musicBundle, bool saveBlocks) {
            string folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            foreach (var block in musicBundle.Blocks) {
                var document = new MusicDocument(block.Header);
                if (PortDocuments.Singleton.BlockDocumentMaster.DocumentList != null) { //// !?!?!
                    PortDocuments.Singleton.BlockDocumentMaster.DocumentList.Add(document);
                }

                if (saveBlocks) {
                    var destinationFilePath = Path.Combine(folder, document.Header.FullName + ".mif");
                    //// block.Header.FileName = document.Header.DocumentFileName;
                    var bundle = MusicalBundle.GetEnvelopeOfBlock(block, document.Header.FullName);
                    document.FilePath = destinationFilePath;
                    var port = PortAbstract.CreatePort(MusicalSourceType.MIFI);
                    port.WriteMusicFile(bundle, destinationFilePath);
                }
            }
        }
        #endregion
    }
}
