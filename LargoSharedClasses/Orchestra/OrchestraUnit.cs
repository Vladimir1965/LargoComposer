// <copyright file="OrchestraUnit.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace LargoSharedClasses.Orchestra
{
    /// <summary>
    /// Orchestra Unit.
    /// </summary>
    public class OrchestraUnit
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestraUnit"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="title">The title.</param>
        /// <param name="author">The author.</param>
        /// <param name="voice1">The voice1.</param>
        /// <param name="voice2">The voice2.</param>
        /// <param name="voice3">The voice3.</param>
        /// <param name="voice4">The voice4.</param>
        public OrchestraUnit(string name, string title, string author, MusicalVoice voice1, MusicalVoice voice2, MusicalVoice voice3, MusicalVoice voice4) {
            this.ListVoices = new List<MusicalVoice>();
            this.Name = name;
            this.Title = title;
            this.Author = author;
            this.AddVoices(voice1, voice2, voice3, voice4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestraUnit"/> class.
        /// </summary>
        /// <param name="markUnit">The given line.</param>
        public OrchestraUnit(XElement markUnit) {
            Contract.Requires(markUnit != null);
            if (markUnit == null) {
                return;
            }

            this.ListVoices = new List<MusicalVoice>();
            this.Name = XmlSupport.ReadStringAttribute(markUnit.Attribute("Name"));
            this.Title = XmlSupport.ReadStringAttribute(markUnit.Attribute("Title"));
            this.Author = XmlSupport.ReadStringAttribute(markUnit.Attribute("Author"));

            var xElement = markUnit.Element("Voices");
            if (xElement != null) {
                var xvoices = xElement.Elements("Voice");
                foreach (var xvoice in xvoices) {
                    MusicalVoice voice = new MusicalVoice(xvoice);
                    this.ListVoices.Add(voice);
                }
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xmunit = new XElement(
                        "Orchestra",
                        new XAttribute("Name", this.Name),
                        new XAttribute("Title", this.Title),
                        new XAttribute("Author", this.Author));

                //// Lines
                XElement xvoices = new XElement("Voices");
                foreach (MusicalVoice voice in this.ListVoices.Where(voice => voice != null)) {
                    var xvoice = voice.GetXElement;
                    xvoices.Add(xvoice);
                }

                xmunit.Add(xvoices);
                return xmunit;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.ListVoices.Count();

        /// <summary>
        /// Gets or sets the list voices.
        /// </summary>
        /// <value>
        /// The list voices.
        /// </value>
        public List<MusicalVoice> ListVoices { get; set; }
        #endregion

        /// <summary>
        /// Adds the voices.
        /// </summary>
        /// <param name="voice1">The voice1.</param>
        /// <param name="voice2">The voice2.</param>
        /// <param name="voice3">The voice3.</param>
        /// <param name="voice4">The voice4.</param>
        public void AddVoices(MusicalVoice voice1, MusicalVoice voice2, MusicalVoice voice3, MusicalVoice voice4) {
            if (voice1 != null) {
                this.ListVoices.Add(voice1);
            }

            if (voice2 != null) {
                this.ListVoices.Add(voice2);
            }

            if (voice3 != null) {
                this.ListVoices.Add(voice3);
            }

            if (voice4 != null) {
                this.ListVoices.Add(voice4);
            }
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion
    }
}
