// <copyright file="MusicDocument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Music Document.
    /// </summary>
    public class MusicDocument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicDocument"/> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        public MusicDocument(MusicalHeader givenHeader)
        {
            this.Header = givenHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicDocument" /> class.
        /// </summary>
        /// <param name="markDocument">The mark document.</param>
        public MusicDocument(XElement markDocument)
        {
            Contract.Requires(markDocument != null);
            if (markDocument == null) {
                return;
            }

            var xheader = markDocument.Element("Header");
            this.FilePath = XmlSupport.ReadStringAttribute(markDocument.Attribute("FilePath"));
            this.Header = new MusicalHeader(xheader, true);
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement
        {
            get
            {
                XElement markDocument = new XElement("Document");
                markDocument.Add(new XAttribute("FilePath", this.FilePath ?? string.Empty));
                XElement xheader = this.Header.GetXElement;
                markDocument.Add(xheader);
                return markDocument;
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => this.Header != null;
        #endregion
    }
}
