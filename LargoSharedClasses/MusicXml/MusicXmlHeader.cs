// <copyright file="MusicXmlHeader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MusicXml
{
    /// <summary>
    /// MusicXml Header.
    /// </summary>
    public sealed class MusicXmlHeader
    {
        #region Fields
        /// <summary>
        /// Musical File.
        /// </summary>
        private MusicalBlock musicalBlock;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicXmlHeader class.
        /// </summary>
        /// <param name="givenScorePartwise">Score Part wise.</param>
        /// <param name="givenMusicalBlock">Musical block.</param>
        public MusicXmlHeader(XElement givenScorePartwise, MusicalBlock givenMusicalBlock) {
            this.ScorePartwise = givenScorePartwise;
            this.MusicalBlock = givenMusicalBlock;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicXmlHeader"/> class.
        /// </summary>
        [UsedImplicitly]
        public MusicXmlHeader() {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets Score Part wise.
        /// </summary>
        /// <value> Property description. </value>
        public XElement ScorePartwise { get; }

        /// <summary>
        /// Gets Score Parts.
        /// </summary>
        /// <value> Property description. </value>
        public Dictionary<string, ScorePartObject> ScoreParts { get; private set; }
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets Musical File.
        /// </summary>
        /// <value> Property description. </value>
        private MusicalBlock MusicalBlock {
            get {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.musicalBlock == null) {
                    throw new InvalidOperationException("Musical block is null.");
                }

                return this.musicalBlock;
            }

            set => this.musicalBlock = value;
        }
        #endregion

        #region Operations
        /// <summary>
        /// Read Xml Header.
        /// </summary>
        public void ReadXmlHeader() {
            Contract.Requires(this.ScorePartwise != null);
            var work = this.ScorePartwise.Element("work");
            this.ReadWorkElement(work);

            //// string movementNumber = (string)this.ScorePartwise.Element("movement-number");
            //// string movementTitle = (string)this.ScorePartwise.Element("movement-title");

            var identification = this.ScorePartwise.Element("identification");
            this.ReadIdentificationElement(identification);

            var partList = this.ScorePartwise.Element("part-list");
            if (partList != null) {
                this.ReadScorePartListElement(partList);
            }
        }

        /// <summary>
        /// Write Xml Header.
        /// </summary>
        public void WriteXmlHeader() {
            var element = this.WorkElement();
            this.ScorePartwise.Add(element);

            var movementNumber = new XElement("movement-number", string.Empty);
            this.ScorePartwise.Add(movementNumber);

            var movementTitle = new XElement("movement-title", "movement");
            this.ScorePartwise.Add(movementTitle);

            element = this.IdentificationElement();
            this.ScorePartwise.Add(element);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Read ScorePartList Element.
        /// </summary>
        /// <param name="partList">Score PartList.</param>
        private void ReadScorePartListElement(XContainer partList) {
            Contract.Requires(partList != null);
            this.ScoreParts = new Dictionary<string, ScorePartObject>();
            foreach (var scorePartObject in
                partList.Elements("score-part").Select(ScorePartObject.ReadScorePart)) {
                this.ScoreParts[scorePartObject.Id] = scorePartObject;
            }
        }

        /// <summary>
        /// Work element.
        /// </summary>
        /// <param name="work">Musical work.</param>
        private void ReadWorkElement(XContainer work) {
            if (work == null) {
                return;
            }

            var h = this.MusicalBlock.FileHeading;
            h.WorkNumber = (string)work.Element("work-number");
            h.WorkTitle = (string)work.Element("work-title");
            //// this.MusicalBlock.Name = this.MusicalBlock.FileHeading.WorkTitle;
        }

        /// <summary>
        /// Work element.
        /// </summary>
        /// <returns> Returns value. </returns>
        private XElement WorkElement() {
            var h = this.MusicalBlock.FileHeading;
            var header = this.musicalBlock?.Header;
            var work = new XElement(
                                    "work",
                                    new XElement("work-number", h?.WorkNumber ?? string.Empty),
                                    new XElement("work-title", h?.WorkTitle ?? header?.Specification));
            return work;
        }

        /// <summary>
        /// Identification element.
        /// </summary>
        /// <param name="encoding">Musical encoding.</param>
        private void ReadEncodingElement(XContainer encoding) {
            if (encoding == null) {
                return;
            }

            //// this.MusicalBlock.Name = (string)work.Attribute("work-number");
            var h = this.MusicalBlock.FileHeading;
            h.EncodingDate = (string)encoding.Element("encoding-date");
            h.Encoder = (string)encoding.Element("encoder");
            h.Software = (string)encoding.Element("software");
            h.EncodingDescription = (string)encoding.Element("encoding-description");
        }

        /// <summary>
        /// Identification element.
        /// </summary>
        /// <returns> Returns value. </returns>
        private XElement EncodingElement() {
            var h = this.MusicalBlock.FileHeading;
            //// "<encoding-date>2011-01-01</encoding-date>";
            var encoding = new XElement(
                            "encoding",
                            new XElement("encoding-date", h.EncodingDate),
                            new XElement("encoder", h.Encoder),
                            new XElement("software", h.Software),
                            new XElement("encoding-description", h.EncodingDescription));

            return encoding;
        }

        /// <summary>
        /// Identification element.
        /// </summary>
        /// <param name="identification">Musical identification.</param>
        private void ReadIdentificationElement(XContainer identification) {
            if (identification == null) {
                return;
            }

            var encoding = identification.Element("encoding");
            this.ReadEncodingElement(encoding);

            var h = this.MusicalBlock.FileHeading;
            h.Creator = (string)encoding.Element("creator");
            h.Composer = (string)encoding.Element("composer");
            h.Rights = (string)encoding.Element("rights");
            h.Source = (string)encoding.Element("source");
            //// this.MusicalBlock.Name
        }

        /// <summary>
        /// Identification element.
        /// </summary>
        /// <returns> Returns value. </returns>
        private XElement IdentificationElement() {
            var h = this.MusicalBlock.FileHeading;
            //// "<creator type=\"composer\">Largo-muse</creator>";
            //// "<creator type=\"poet\">Largo-muse</creator>";
            var encoding = this.EncodingElement();
            var identification = new XElement("identification");
            if (!string.IsNullOrEmpty(h.Creator)) {
                identification.Add(new XElement("creator", new XAttribute("type", "poet"), h.Creator));
            }

            if (!string.IsNullOrEmpty(h.Composer)) {
                identification.Add(new XElement("composer", new XAttribute("type", "poet"), h.Composer));
            }

            if (!string.IsNullOrEmpty(h.Rights)) {
                identification.Add(new XElement("rights", h.Rights));
            }

            identification.Add(encoding);
            if (!string.IsNullOrEmpty(h.Source)) {
                identification.Add(new XElement("source", h.Source));
            }

            return identification;
        }
        #endregion
    }
}
