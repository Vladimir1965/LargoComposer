// <copyright file="MusicalStyle.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Harmonic Streams Port.
    /// </summary>
    public class MusicalStyle
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStyle" /> class.
        /// </summary>
        public MusicalStyle()
        {
            this.RhythmicPatterns = new List<RhythmicPattern>();
            this.MelodicPatterns = new List<MelodicPattern>();
            this.RhythmicStructures = new List<RhythmicStructure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStyle"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public MusicalStyle(MusicalBlock givenBlock)
        {
            this.ObjectName = givenBlock.Header.FullName;
            this.Header = givenBlock.Header;
            this.LoadMelodicPatternsOfBlock(givenBlock);
            this.LoadRhythmicPatternsOfBlock(givenBlock);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>
        /// The name of the set.
        /// </value>
        public string ObjectName { get; set; }

        public IList<MelodicPattern> MelodicPatterns { get; set; }

        public IList<RhythmicPattern> RhythmicPatterns { get; set; }

        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool IsValid => this.MelodicPatterns.Any();

        /// <summary>
        /// Gets or sets the rhythmic structures.
        /// </summary>
        /// <value>
        /// The rhythmic structures.
        /// </value>
        public IList<RhythmicStructure> RhythmicStructures { get; set; }

        #region Public static factory methods

        #endregion

        /// <summary>
        /// Gets the get rhythmic structures.
        /// </summary>
        /// <param name="justSelected">if set to <c>true</c> [just selected].</param>
        /// <returns> Returns value. </returns>
        /// <value>
        /// The get rhythmic structures.
        /// </value>
        [UsedImplicitly]
        public IList<RhythmicStructure> GetRhythmicStructures(bool justSelected)
        {
            var rhyStructures = new List<RhythmicStructure>(); //// style.RhythmicStructures;

            foreach (var p in this.RhythmicPatterns) {
                if (justSelected && !p.Selected) {
                    continue;
                }

                foreach (var v in p.Voices) {
                    if (v.RhythmicStructure != null) {
                        rhyStructures.Add(v.RhythmicStructure);
                    }
                }
            }

            return rhyStructures;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("Style {0}", this.ObjectName);

            return s.ToString();
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Adds the content of the style.
        /// </summary>
        /// <param name="style">The style.</param>
        [UsedImplicitly]
        public void AddObjectContent(MusicalStyle style)
        {
            //// this = style.HarmonicOrder;  this.RhythmicOrder = style.RhythmicOrder; //// ??????? 
            var mp = (List<MelodicPattern>)this.MelodicPatterns;
            mp.AddRange(style.MelodicPatterns);

            var rp = (List<RhythmicPattern>)this.RhythmicPatterns;
            rp.AddRange(style.RhythmicPatterns);

            var rs = (List<RhythmicStructure>)this.RhythmicStructures;
            rs.AddRange(style.RhythmicStructures);
        }

        /// <summary>
        /// Saves the style.
        /// </summary>
        /// <param name="givenFolder">The given folder.</param>
        [UsedImplicitly]
        public void SaveToFolder(string givenFolder)
        {
            var tmpFolderPath = SupportFiles.GetTemporaryDirectory;
            Directory.CreateDirectory(tmpFolderPath);
            XElement xcomponent = new XElement(
                "Component",
                new XAttribute("Description", string.Empty),
                new XAttribute("Name", this.ObjectName),
                this.Header.GetXElement);

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xcomponent);
            xdoc.Save(Path.Combine(tmpFolderPath, @"Component.xml"));

            //// Melodic Patterns
            var xpatterns = this.WriteMelodicPatterns();
            xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xpatterns);
            //// var filepath = Path.Combine(givenPath, "MelodicPatterns.xml");
            xdoc.Save(Path.Combine(tmpFolderPath, @"MelodicPatterns.xml"));

            //// Rhythmic Patterns
            xpatterns = this.WriteRhythmicPatterns();
            xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xpatterns);
            //// var filepath = Path.Combine(givenPath, "RhythmicPatterns.xml");
            xdoc.Save(Path.Combine(tmpFolderPath, @"RhythmicPatterns.xml"));

            //// Rhythmic Structures
            var xstructures = this.WriteRhythmicStructures();
            xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xstructures);
            //// var filepath = Path.Combine(givenPath, model.FullName + ".xml");
            xdoc.Save(Path.Combine(tmpFolderPath, @"RhythmicStructures.xml"));

            var finalPath = Path.Combine(givenFolder, this.ObjectName.ClearSpecialChars() + ".style");
            ZipFile.CreateFromDirectory(tmpFolderPath, finalPath);
            //// ZipFileCover.CreateZipFileRecursive(tmpFolderPath, finalPath, false, true);
            Directory.Delete(tmpFolderPath, true);
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStyle" /> class.
        /// </summary>
        /// <param name="givenFilePath">The given file path.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        private static MusicalStyle GetStyle(string givenFilePath)
        {
            var style = new MusicalStyle();
            var fileName = Path.GetFileNameWithoutExtension(givenFilePath);
            if (fileName == null) {
                return null;
            }

            style.ObjectName = fileName.Trim();

            var tmpFolderPath = MusicalSettings.Singleton.Folders.GetTemporaryFolder; //// SupportFiles.GetTemporaryDirectory;
            if (tmpFolderPath == null) {
                return null;
            }

            ZipFile.ExtractToDirectory(givenFilePath, tmpFolderPath);
            //// ZipFileCover.UnzipFile(givenFilePath, tmpFolderPath);
            var fi = SupportFiles.LatestFile(tmpFolderPath, "*.xml");
            if (fi == null) {
                if (Directory.Exists(tmpFolderPath)) {
                    Directory.Delete(tmpFolderPath, true);
                }

                return null;
            }

            var root = XmlSupport.GetXDocRoot(Path.Combine(tmpFolderPath, "Component.xml"));
            if (root != null && root.Name == "Component") {
                var xstyle = root;
                //// var description = XmlSupport.ReadStringAttribute(xstyle.Attribute("Description"));
                //// unused var name = XmlSupport.ReadStringAttribute(xstyle.Attribute("Name"));
                style.Header = new MusicalHeader(xstyle.Element("Header"), true);
                //// this.HarmonicOrder = XmlSupport.ReadByteAttribute(xstyle.Attribute("HarmonicOrder"));
                //// this.RhythmicOrder = XmlSupport.ReadByteAttribute(xstyle.Attribute("RhythmicOrder"));
            }

            style.LoadMelodicPatterns(Path.Combine(tmpFolderPath, "MelodicPatterns.xml"));
            style.LoadRhythmicPatterns(Path.Combine(tmpFolderPath, "RhythmicPatterns.xml"));
            style.LoadRhythmicStructures(Path.Combine(tmpFolderPath, "RhythmicStructures.xml"));
            //// MusicalStyle.internalSingleton = this;

            if (Directory.Exists(tmpFolderPath)) {
                Directory.Delete(tmpFolderPath, true);
            }

            return style;
        }
        #endregion

        #region Private methods - Load finals

        /// <summary>
        /// Loads the melodic patterns.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void LoadMelodicPatterns(string filePath)
        {
            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "MelodicPatterns") {
                var xbundle = root;
                var xpatterns = xbundle.Elements("MelodicPattern");

                var list = new List<MelodicPattern>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var xpattern in xpatterns) {
                    var stream = new MelodicPattern(xpattern);
                    list.Add(stream);
                }

                this.MelodicPatterns = list;
            }
        }

        /// <summary>
        /// Loads the rhythmic patterns.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void LoadRhythmicPatterns(string filePath)
        {
            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "RhythmicPatterns") {
                var xbundle = root;
                var xpatterns = xbundle.Elements("RhythmicPattern");

                var list = new List<RhythmicPattern>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var xpattern in xpatterns) {
                    var stream = new RhythmicPattern(xpattern);
                    list.Add(stream);
                }

                this.RhythmicPatterns = list;
            }
        }

        /// <summary>
        /// Loads the rhythmic structures.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void LoadRhythmicStructures(string filePath)
        {
            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "RhythmicStructures") {
                var xbundle = root;
                var xstructs = xbundle.Elements("RhythmicStructure");

                var list = new List<RhythmicStructure>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var xstruct in xstructs) {
                    var rs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, this.Header.System.RhythmicOrder);
                    var code = XmlSupport.ReadStringAttribute(xstruct.Attribute("Code"));
                    var rstruct = new RhythmicStructure(rs, code);
                    list.Add(rstruct);
                }

                this.RhythmicStructures = list;
            }
        }

        #endregion

        #region Pattern - Load from block
        /// <summary>
        /// Gets the melodic patterns.
        /// </summary>
        /// <value>
        /// The melodic patterns.
        /// </value>
        private void LoadMelodicPatternsOfBlock(MusicalBlock givenBlock)
        {
            var body = givenBlock.Body;
            var list = new List<MelodicPattern>();

            foreach (var bar in body.Bars) {
                var pattern = new MelodicPattern(givenBlock.Header, bar) { SetName = givenBlock.Header.FullName };
                if (!pattern.IsEmpty && !pattern.ExistsInPatterns(list)) {
                    list.Add(pattern);
                }
            }

            this.MelodicPatterns = list;
        }

        /// <summary>
        /// Gets the rhythmic patterns.
        /// </summary>
        /// <value>
        /// The rhythmic patterns.
        /// </value>
        public void LoadRhythmicPatternsOfBlock(MusicalBlock givenBlock)
        {
            var body = givenBlock.Body;
            var list = new List<RhythmicPattern>();

            foreach (var bar in body.Bars) {
                var pattern = new RhythmicPattern(givenBlock.Header, bar) { SetName = givenBlock.Header.FullName };
                if (!pattern.IsEmpty && !pattern.ExistsInPatterns(list)) {
                    list.Add(pattern);
                }
            }

            this.RhythmicPatterns = list;
        }
        #endregion

        #region Private methods - Save final

        /// <summary>
        /// Writes the melodic patterns.
        /// </summary>
        /// <returns>Returns value.</returns>
        private XElement WriteMelodicPatterns()
        {
            XElement xpatterns = new XElement(
                "MelodicPatterns",
                new XAttribute("Description", string.Empty));

            foreach (var pattern in this.MelodicPatterns) {
                XElement xpattern = pattern.GetXElement;
                xpatterns.Add(xpattern);
            }

            return xpatterns;
        }

        /// <summary>
        /// Writes the rhythmic patterns.
        /// </summary>
        /// <returns>Returns value.</returns>
        private XElement WriteRhythmicPatterns()
        {
            XElement xpatterns = new XElement(
                "RhythmicPatterns",
                new XAttribute("Description", string.Empty));

            foreach (var pattern in this.RhythmicPatterns) {
                XElement xpattern = pattern.GetXElement;
                xpatterns.Add(xpattern);
            }

            return xpatterns;
        }

        /// <summary>
        /// Writes the rhythmic structures.
        /// </summary>
        /// <returns>Returns value.</returns>
        private XElement WriteRhythmicStructures()
        {
            XElement xstructures = new XElement(
                "RhythmicStructures",
                new XAttribute("Description", string.Empty));

            foreach (var rstruct in this.RhythmicStructures) {
                XElement xstructure = rstruct.GetXElement;
                xstructures.Add(xstructure);
            }

            return xstructures;
        }
        #endregion
    }
}