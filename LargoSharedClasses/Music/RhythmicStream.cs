// <copyright file="RhythmicStream.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Rhythmic Stream.
    /// </summary>
    public class RhythmicStream {
        #region Fields
        /// <summary>
        /// The harmonic bars
        /// </summary>
        private List<RhythmicStructure> structures;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicStream"/> class.
        /// </summary>
        public RhythmicStream() {
            this.structures = new List<RhythmicStructure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicStream" /> class.
        /// </summary>
        /// <param name="xstream">The Xml stream.</param>
        /// <param name="full">if set to <c>true</c> [full].</param>
        public RhythmicStream(XElement xstream, bool full) //// MusicalHeader header
            : this() {
            if (xstream == null) {
                return;
            }

            XElement xheader = xstream.Element("Header");
            this.Header = new MusicalHeader(xheader, full);

            var xstructures = xstream.Elements("Bar");
            foreach (var xb in xstructures) {
                RhythmicStructure rhythmicStructure = null; ////  new RhythmicStructure(this.Header, xb);
                this.Structures.Add(rhythmicStructure);
            }

            ///// this.DetermineModalities();
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
        /// Gets or sets the energy bars.
        /// </summary>
        /// <value>
        /// The energy bars.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Energy bars are null.</exception>
        /// <exception cref="System.ArgumentException">Energy bars cannot be empty.;value</exception>
        public IList<RhythmicStructure> Structures
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<RhythmicStructure>>() != null);
                if (this.structures == null) {
                    throw new InvalidOperationException("Structures are null.");
                }

                return this.structures;
            }

            set => this.structures = (List<RhythmicStructure>)value ?? throw new ArgumentException("Structures cannot be empty.", nameof(value));
        }
        #endregion

        #region Lists
        
        /// <summary>
        /// Loads the documents.
        /// </summary>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static List<RhythmicStream> ReadStreams(string xmlFileName) {
            var list = new List<RhythmicStream>();
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var filepath = Path.Combine(path, xmlFileName);
            if (!File.Exists(filepath)) {
                return null;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "RhythmicStream") {
                return null;
            }

            var xlist = root;
            foreach (var xstream in xlist.Elements()) {
                RhythmicStream stream = new RhythmicStream(xstream, true);
                list.Add(stream);
            }

            return list;
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        public static void WriteStreams(List<HarmonicStream> givenList, string xmlFileName) {
            if (givenList == null) {
                return;
            }

            XElement xlist = new XElement("RhythmicStream");
            foreach (var stream in givenList) {
                var xstream = stream.GetXElement;
                xlist.Add(xstream);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var filepath = Path.Combine(path, xmlFileName);
            xdoc.Save(filepath);
        }
        
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("RhythmicStream (Length {0})", this.Structures.Count);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Structures the in bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <returns>Returns value.</returns>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public RhythmicStructure StructureInBar(int barNumber) {
            if (barNumber < 1 || barNumber > this.Structures.Count) {
                return null;
            }

            return this.Structures[barNumber - 1];
        }
        #endregion
    }
}
