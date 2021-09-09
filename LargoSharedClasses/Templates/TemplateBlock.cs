// <copyright file="TemplateBlock.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Templates
{
    using LargoSharedClasses.Music;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Musical Header.
    /// </summary>
    public class TemplateBlock
    {
        /// <summary>
        /// The lines
        /// </summary>
        private List<TemplateLine> lines; //// 2021/08 (virtual method call in constructor)

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBlock"/> class.
        /// </summary>
        public TemplateBlock() {
            this.Header = MusicalHeader.GetDefaultMusicalHeader;
            this.lines = new List<TemplateLine>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBlock"/> class.
        /// </summary>
        /// <param name="markBlock">The mark block.</param>
        public TemplateBlock(XElement markBlock) {
            Contract.Requires(markBlock != null);
            if (markBlock == null) {
                return;
            }

            var xheader = markBlock.Element("Header");
            this.Header = new MusicalHeader(xheader, true);

            XElement xstrip = markBlock.Element("Strip");
            if (xstrip != null) {
                var xlines = xstrip.Elements("Line");
                this.Lines = new List<TemplateLine>();
                foreach (var xline in xlines) {
                    var line = new TemplateLine(xline, this.Header); 
                    this.Lines.Add(line);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary> Gets or sets the header. </summary>
        /// <value> The header. </value>
        public MusicalHeader Header { get; set; }

        /// <summary> Gets or sets the lines. </summary>
        /// <value> The lines. </value>
        public virtual List<TemplateLine> Lines {
            get { return this.lines; }
            set { this.lines = value; }
        }

        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get x element.
        /// </summary>
        /// <value>
        /// The get x element.
        /// </value>
        public XElement GetXElement {
            get {
                XElement xblock = new XElement("Tectonic");
                XElement xheader = this.Header.GetXElement;
                xblock.Add(xheader);

                XElement xlines = new XElement("Strip");
                foreach (var line in this.Lines) {
                    XElement xline = line.GetXElement;
                    xlines.Add(xline);
                }

                xblock.Add(xlines);
                return xblock;
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
        public static List<TemplateBlock> ReadTemplates(string givenPath, string xmlFileName) {
            var list = new List<TemplateBlock>();
            var filepath = Path.Combine(givenPath, xmlFileName); 
            if (!File.Exists(filepath)) {
                return null;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "TectonicTemplates") {
                return null;
            }

            var xlist = root;
            foreach (var xblock in xlist.Elements()) {
                TemplateBlock block = new TemplateBlock(xblock);
                list.Add(block);
            }

            return list; 
        }

        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        /// <param name="givenPath">The given path.</param>
        /// <param name="xmlFileName">Filename of the XML file.</param>
        public static void WriteTemplates(List<TemplateBlock> givenList, string givenPath, string xmlFileName) {
            if (givenList == null) {
                return;
            }

            XElement xlist = new XElement("TectonicTemplates");
            foreach (var block in givenList) {
                var xblock = block.GetXElement;
                xlist.Add(xblock);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var filepath = Path.Combine(givenPath, xmlFileName); 
            xdoc.Save(filepath); 
        }
        
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.Header.ToString());

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalHeader object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var block = new TemplateBlock();
            var header = (MusicalHeader)this.Header.Clone();
            block.Header = header;

            return block;
        }
        #endregion
    }
}
