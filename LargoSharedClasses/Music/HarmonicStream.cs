// <copyright file="HarmonicStream.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Models;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Harmonic Stream.
    /// </summary>
    [UsedImplicitly]
    public class HarmonicStream {
        #region Fields
        /// <summary>
        /// The harmonic bars
        /// </summary>
        private List<HarmonicBar> harmonicBars;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStream"/> class.
        /// </summary>
        public HarmonicStream() {
            this.HarmonicBars = new List<HarmonicBar>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStream" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        public HarmonicStream(MusicalHeader givenHeader)
            : this() {
            this.Header = givenHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStream" /> class.
        /// </summary>
        /// <param name="xstream">The Xml stream.</param>
        /// <param name="full">if set to <c>true</c> [full].</param>
        public HarmonicStream(XElement xstream, bool full) //// MusicalHeader header
            : this() {
            if (xstream == null) {
                return;
            }

            XElement xheader = xstream.Element("Header");
            this.Header = new MusicalHeader(xheader, full);

            var xbars = xstream.Elements("Bar");
            foreach (var xb in xbars) {
                HarmonicBar harmonicBar = new HarmonicBar(this.Header, xb);
                this.HarmonicBars.Add(harmonicBar);
            }

            this.DetermineModalities();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStream" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="givenImportedContent">Content of the given imported.</param>
        public HarmonicStream(MusicalHeader givenHeader, string givenImportedContent) {
            this.HarmonicBars = new List<HarmonicBar>();
            var barRows = givenImportedContent.Split('\n');
            int barNumber = 1;
            foreach (var barRow in barRows) {
                HarmonicBar harmonicBar = new HarmonicBar(givenHeader, (HarmonicStructure)null) {
                    BarNumber = barNumber++
                };
                this.HarmonicBars.Add(harmonicBar);
                if (string.IsNullOrEmpty(barRow)) {
                    continue;
                }

                //// barRow: a#f# (0,0.18), C#mi (0.18,0.92)
                var items = barRow.Split(',');
                foreach (var item in items) {
                    //// item: a#f# (0,0.18)
                    if (string.IsNullOrEmpty(item)) {
                        continue;
                    }

                    HarmonicStructure hstruct = new HarmonicStructure(givenHeader.System.HarmonicSystem, item, true);
                    harmonicBar.AddStructure(hstruct);
                }
            }
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement
        {
            get
            {
                XElement xstream = new XElement("Harmony");
                xstream.Add(this.Header.GetXElement);

                //// var xheader = this.Header.GetXElement;
                //// xstream.Add(xheader);

                //// Structures
                foreach (HarmonicBar harBar in this.HarmonicBars) {
                    //// XElement xstructs = new XElement("Structures");
                    XElement xbar = new XElement(
                        "Bar",
                        new XAttribute("Number", harBar.BarNumber), //// BarNumberInMotive
                        new XAttribute("OriginalNumber", harBar.OriginalBarNumber),
                        new XAttribute("Schema", harBar.RhythmicStructure != null ? harBar.RhythmicStructure.ElementSchema : string.Empty));

                    foreach (HarmonicStructure hstruct in harBar.HarmonicStructures) {
                        var xstruct = hstruct.GetXElement;
                        xbar.Add(xstruct); //// xstructs
                    }

                    //// xbar.Add(xstructs);
                    xstream.Add(xbar);
                }

                return xstream;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the harmonic motive bars.
        /// </summary>
        /// <value>
        /// The harmonic motive bars.
        /// </value>
        public IList<HarmonicBar> HarmonicBars
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<HarmonicBar>>() != null);
                if (this.harmonicBars == null) {
                    throw new InvalidOperationException("Harmonic bars are null.");
                }

                return this.harmonicBars;
            }

            set => this.harmonicBars = (List<HarmonicBar>)value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName => this.Header.FileName;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => this.HarmonicBars.Count;

        /// <summary>
        /// Gets the outline.
        /// </summary>
        /// <value>
        /// The outline.
        /// </value>
        public string Outline {
            get {
                var s = this.ChordsToString();
                var outline = s.Length > 50 ? s.Left(50) + " ..." : s;
                return outline;
            }
        }
        #endregion

        #region Lists

        /// <summary>
        /// Loads the documents.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static List<HarmonicStream> ReadStreams(string givenPath, string xmlFileName) {
            var list = new List<HarmonicStream>();
            var filepath = Path.Combine(givenPath, xmlFileName); 
            if (!File.Exists(filepath)) {
                return null;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "HarmonicTemplates") {
                return null;
            }

            var xlist = root;
            foreach (var xstream in xlist.Elements()) {
                HarmonicStream stream = new HarmonicStream(xstream, true);
                list.Add(stream);
            }

            return list;
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        /// <param name="givenPath">The given path.</param>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        public static void WriteStreams(List<HarmonicStream> givenList, string givenPath, string xmlFileName) {
            if (givenList == null) {
                return;
            }

            XElement xlist = new XElement("HarmonicTemplates");
            foreach (var stream in givenList) {
                var xstream = stream.GetXElement;
                xlist.Add(xstream);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var filepath = Path.Combine(givenPath, xmlFileName); 
            xdoc.Save(filepath);
        }
        
        #endregion

        #region Public static factory methods
        /// <summary>
        /// Gets the musical tectonic.
        /// </summary>
        /// <param name="givenFilePath">The given file path.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public static HarmonicStream GetHarmonicStream(string givenFilePath) {
            var fileName = Path.GetFileNameWithoutExtension(givenFilePath);
            if (fileName == null) {
                return null;
            }

            //// var settings = MusicalSettings.Singleton;
            //// string tectonicFolder = settings.Folders.GetFolder(MusicalFolder.Tectonics);
            var root = XmlSupport.GetXDocRoot(givenFilePath);
            if (root != null && root.Name == "Harmony") {
                var xharmonicStream = root;
                var t = new HarmonicStream(xharmonicStream, true); ////  Name = fileName.Trim()
                return t;
            }

            return null;
        }

        /// <summary>
        /// Extends the harmonic stream.
        /// </summary>
        /// <param name="givenStream">The given stream.</param>
        /// <param name="givenBarCount">The given bar count.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public static HarmonicStream ExtendHarmonicStream(HarmonicStream givenStream, int givenBarCount) {
            if (givenStream == null || givenStream.HarmonicBars.Count == givenBarCount) {
                return givenStream;
            }

            var harStream = new HarmonicStream(givenStream.Header);
            for (int i = 0; i < givenBarCount; i++) {
                var cnt = givenStream.HarmonicBars.Count;
                var idx = i % cnt;
                var harBar = givenStream.HarmonicBars[idx];
                harStream.HarmonicBars.Add(harBar);
            }

            return harStream;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("HarmonicStream (Length {0})", this.HarmonicBars.Count);

            return s.ToString();
        }

        /// <summary>
        /// Chords to string.
        /// </summary>
        /// <returns>Returns value.</returns>
        public string ChordsToString() {
            var s = new StringBuilder();
            foreach (var harBar in this.HarmonicBars) {
                s.Append(harBar);
                s.AppendLine(" # ");
            }

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Renumbers the bars.
        /// </summary>
        [UsedImplicitly]
        public void RenumberBars() {
            int barNumber = 1;
            foreach (var harBar in this.HarmonicBars) {
                if (harBar != null)
                {
                    harBar.BarNumber = barNumber++;
                }
            }
        }

        /// <summary>
        /// Length Of Motive.
        /// </summary>
        /// <param name="firstIndex">First Index.</param>
        /// <param name="secondIndex">Second Index.</param>
        /// <returns> Returns value. </returns>
        public int LengthOfMotive(int firstIndex, int secondIndex) {
            var shift = 0;
            while (secondIndex + shift < this.HarmonicBars.Count) {
                var harBar = firstIndex + shift >= 0 ? this.HarmonicBars.ElementAt(firstIndex + shift) : null;
                var nextBar = secondIndex + shift >= 0 ? this.HarmonicBars.ElementAt(secondIndex + shift) : null;
                if (harBar != null && nextBar != null && string.CompareOrdinal(harBar.UniqueIdentifier, nextBar.UniqueIdentifier) == 0) {
                    shift++;
                }
                else {
                    break;
                }
            }

            return shift;
        }

        /// <summary>
        /// Equal Segments.
        /// </summary>
        /// <param name="firstIndex">First Index.</param>
        /// <param name="secondIndex">Second Index.</param>
        /// <param name="minLength">Minimal Length.</param>
        /// <returns> Returns value. </returns>
        public bool EqualSegments(int firstIndex, int secondIndex, int minLength) {
            if (firstIndex + minLength > this.HarmonicBars.Count || secondIndex + minLength > this.HarmonicBars.Count) {
                return false;
            }

            var isEqual = true;
            for (var shift = 0; shift < minLength; shift++) {
                var harBar = firstIndex + shift > 0 ? this.HarmonicBars.ElementAt(firstIndex + shift) : null;
                var nextBar = secondIndex + shift > 0 ? this.HarmonicBars.ElementAt(secondIndex + shift) : null;
                if (harBar != null && nextBar != null && string.CompareOrdinal(harBar.UniqueIdentifier, nextBar.UniqueIdentifier) == 0) {
                    continue;
                }

                isEqual = false;
                break;
            }

            return isEqual;
        }

        /// <summary>
        /// Write To Motive.
        /// </summary>
        /// <param name="harmonicMotive">Harmonic Motive.</param>
        /// <param name="firstIndex">First Index.</param>
        /// <param name="length">Length to write.</param>
        public void WriteToMotive(HarmonicMotive harmonicMotive, int firstIndex, int length) { //// int firstBarNumber
            var newBarNumber = 1; //// 2016/08 firstBarNumber;
            for (var idx = firstIndex; idx < firstIndex + length; idx++) {
                if (idx < 0 || idx >= this.HarmonicBars.Count) {
                    continue;
                }

                if (harmonicMotive?.HarmonicStream.HarmonicBars == null) {
                    continue;
                }

                var harBar = this.HarmonicBars.ElementAt(idx);
                if (harBar == null) {
                    continue;
                }

                var harmonicBarInMotive = (HarmonicBar)harBar.Clone(); //// 2016/08
                harmonicBarInMotive.BarNumber = newBarNumber++;     //// 201508 
                harmonicMotive.AddBar(harmonicBarInMotive);

                ////core harmonicMotive.Length = harmonicMotive.HarmonicBars.Count();
            }
        }

        #endregion

        /// <summary>
        /// Determines the modalities.
        /// Determine modality of every bar from its harmonic structures.
        /// </summary>
        private void DetermineModalities() {
            foreach (var harmonicBar in this.HarmonicBars) {
                var bitArray = new BitArray(this.Header.System.HarmonicOrder);
                foreach (var harStruct in harmonicBar.HarmonicStructures) {
                    if (harStruct == null) {
                        continue;
                    }

                    bitArray = bitArray.Or(harStruct.BitArray);
                }

                var binStruct = new BinaryStructure(this.Header.System.HarmonicSystem, bitArray);
                var harModality = new HarmonicModality(binStruct);
                harmonicBar.HarmonicModality = harModality;
            }
        }
    }
}
