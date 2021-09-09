// <copyright file="FileHeading.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using Abstract;
    using JetBrains.Annotations;
    using Localization;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Musical File Heading
    /// </summary>
    public class FileHeading {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeading"/> class.
        /// </summary>
        /// <param name="xelement">The element.</param>
        public FileHeading(XElement xelement)
        {
            Contract.Requires(xelement != null);
            if (xelement == null) {
                return;
            }

            this.WorkTitle = XmlSupport.ReadStringAttribute(xelement.Attribute("WorkTitle"));
            this.WorkNumber = XmlSupport.ReadStringAttribute(xelement.Attribute("WorkNumber"));
            this.EncodingDate = XmlSupport.ReadStringAttribute(xelement.Attribute("EncodingDate"));
            this.Encoder = XmlSupport.ReadStringAttribute(xelement.Attribute("Encoder"));
            this.Software = XmlSupport.ReadStringAttribute(xelement.Attribute("Software"));
            this.EncodingDescription = XmlSupport.ReadStringAttribute(xelement.Attribute("EncodingDescription"));
            this.Software = XmlSupport.ReadStringAttribute(xelement.Attribute("Software"));
            this.Creator = XmlSupport.ReadStringAttribute(xelement.Attribute("Creator"));
            this.Composer = XmlSupport.ReadStringAttribute(xelement.Attribute("Composer"));
            this.Rights = XmlSupport.ReadStringAttribute(xelement.Attribute("Rights"));
            this.Source = XmlSupport.ReadStringAttribute(xelement.Attribute("Source"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHeading"/> class.
        /// </summary>
        public FileHeading()
        {            
        }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get XML.
        /// </summary>
        /// <value>
        /// The get XML.
        /// </value>
        public XElement GetXElement {
            get {
                XElement xheading = new XElement("Heading");

                if (!string.IsNullOrEmpty(this.WorkTitle)) {
                    xheading.Add(new XAttribute("WorkTitle", this.WorkTitle));
                }

                if (!string.IsNullOrEmpty(this.WorkNumber)) {
                    xheading.Add(new XAttribute("WorkNumber", this.WorkNumber));
                }

                if (!string.IsNullOrEmpty(this.EncodingDate)) {
                    xheading.Add(new XAttribute("EncodingDate", this.EncodingDate));
                }

                if (!string.IsNullOrEmpty(this.Encoder)) {
                    xheading.Add(new XAttribute("Encoder", this.Encoder));
                }

                if (!string.IsNullOrEmpty(this.Software)) {
                    xheading.Add(new XAttribute("Software", this.Software));
                }

                if (!string.IsNullOrEmpty(this.EncodingDescription)) {
                    xheading.Add(new XAttribute("EncodingDescription", this.EncodingDescription));
                }

                if (!string.IsNullOrEmpty(this.Creator)) {
                    xheading.Add(new XAttribute("Creator", this.Creator));
                }

                if (!string.IsNullOrEmpty(this.Composer)) {
                    xheading.Add(new XAttribute("Composer", this.Composer));
                }

                if (!string.IsNullOrEmpty(this.Rights)) {
                    xheading.Add(new XAttribute("Rights", this.Rights));
                }

                if (!string.IsNullOrEmpty(this.Source)) {
                    xheading.Add(new XAttribute("Source", this.Source));
                }

                return xheading;
            }
        }
        #endregion

        #region Heading properties

        /// <summary>
        /// Gets or sets Work title.
        /// </summary>
        /// <value> Property description. </value>
        public string WorkTitle { get; set; }

        /// <summary>
        /// Gets or sets Work number.
        /// </summary>
        /// <value> Property description. </value>
        public string WorkNumber { get; set; }

        /// <summary>
        /// Gets or sets Encoding Date.
        /// </summary>
        /// <value> Property description. </value>
        public string EncodingDate { get; set; }

        /// <summary>
        /// Gets or sets encoder.
        /// </summary>
        /// <value> Property description. </value>
        public string Encoder { get; set; }

        /// <summary>
        /// Gets or sets Software.
        /// </summary>
        /// <value> Property description. </value>
        /// <value> Property description. </value>
        public string Software { get; set; }

        /// <summary>
        /// Gets or sets Encoding Description.
        /// </summary>
        /// <value> Property description. </value>
        public string EncodingDescription { get; set; }

        /// <summary>
        /// Gets or sets Creator.
        /// </summary>
        /// <value> Property description. </value>
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets Composer.
        /// </summary>
        /// <value> Property description. </value>
        public string Composer { get; set; }

        /// <summary>
        /// Gets or sets Rights.
        /// </summary>
        /// <value> Property description. </value>
        public string Rights { get; set; }

        /// <summary>
        /// Gets or sets Source.
        /// </summary>
        /// <value> Property description. </value>
        public string Source { get; set; }

        #endregion

        #region Other public Properties
        /// <summary>
        /// Gets the identification.
        /// </summary>
        /// <value>
        /// The identification.
        /// </value>
        [UsedImplicitly]
        public IList<KeyValuePair> Identification {
            get {
                var items = new Collection<KeyValuePair>();
                KeyValuePair item;

                if (!string.IsNullOrEmpty(this.WorkTitle)) {
                    item = new KeyValuePair(LocalizedMusic.String("Work title"), this.WorkTitle);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.WorkNumber)) {
                    item = new KeyValuePair(LocalizedMusic.String("Work number"), this.WorkNumber);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.EncodingDate)) {
                    item = new KeyValuePair(LocalizedMusic.String("Encoding date"), this.EncodingDate);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.Encoder)) {
                    item = new KeyValuePair(LocalizedMusic.String("Encoder"), this.Encoder);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.Software)) {
                    item = new KeyValuePair(LocalizedMusic.String("Software"), this.Software);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.EncodingDescription)) {
                    item = new KeyValuePair(LocalizedMusic.String("Encoding description"), this.EncodingDescription);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.Creator)) {
                    item = new KeyValuePair(LocalizedMusic.String("Creator"), this.Creator);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.Composer)) {
                    item = new KeyValuePair(LocalizedMusic.String("Composer"), this.Composer);
                    items.Add(item);
                }

                if (!string.IsNullOrEmpty(this.Rights)) {
                    item = new KeyValuePair(LocalizedMusic.String("Rights"), this.Rights);
                    items.Add(item);
                }

                // ReSharper disable once InvertIf
                if (!string.IsNullOrEmpty(this.Source)) {
                    item = new KeyValuePair(LocalizedMusic.String("Source"), this.Source);
                    items.Add(item);
                }

                return items;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("FileHeading {0}", this.WorkTitle);

            return s.ToString();
        }
        #endregion
    }
}
