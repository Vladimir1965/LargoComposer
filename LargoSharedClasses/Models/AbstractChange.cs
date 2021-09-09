// <copyright file="AbstractChange.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Models
{
    using Abstract;
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;
    using Localization;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Abstract Change.
    /// </summary>
    public class AbstractChange
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public AbstractChange(XElement xchange) {
            Contract.Requires(xchange != null);
            if (xchange == null) {
                return;
            }

            this.BarNumber = XmlSupport.ReadIntegerAttribute(xchange.Attribute("Bar"));
            this.LineIndex = XmlSupport.ReadByteAttribute(xchange.Attribute("Line"));

            var changeTypeStr = XmlSupport.ReadStringAttribute(xchange.Attribute("Type"));
            this.ChangeType = string.IsNullOrEmpty(changeTypeStr) ? MusicalChangeType.None : (MusicalChangeType)Enum.Parse(typeof(MusicalChangeType), changeTypeStr);

            //// this.ChangeType = (MusicalChangeType)LibSupport.ReadIntegerAttribute(xchange.Attribute("ChangeType"));
            this.IsStop = XmlSupport.ReadBooleanAttribute(xchange.Attribute("IsStop"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLineIndex">The given line.</param>
        /// <param name="givenType">Type of the given.</param>
        protected AbstractChange(int givenBar, int givenLineIndex, MusicalChangeType givenType) {
            this.BarNumber = givenBar;
            this.LineIndex = givenLineIndex;
            this.ChangeType = givenType;
            this.IsStop = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChange"/> class.
        /// </summary>
        protected AbstractChange() {
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                //// Contract.Requires(this.BitRange != null);
                var xmlBitRange = new XElement(
                           "Change",
                           new XAttribute("Type", this.ChangeType),
                           new XAttribute("Bar", this.BarNumber),
                           new XAttribute("Line", this.LineIndex));
                return xmlBitRange;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        /// <value>
        /// The type of the change.
        /// </value>
        public MusicalChangeType ChangeType { get; protected set; }

        /// <summary> Gets class of melodic part. </summary>
        /// <value> Property description. </value>
        public int BarNumber { get; }

        /// <summary> Gets line index i.e. mark of the line in the musical model. </summary>
        /// <value> Property description. </value>
        public int LineIndex { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   Is <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        [System.Diagnostics.Contracts.Pure, UsedImplicitly]
        public bool IsMotivic =>
            this.LineType == MusicalLineType.Melodic || this.LineType == MusicalLineType.Rhythmic
                                 || this.LineType == MusicalLineType.Harmonic;

        /// <summary>
        /// Gets or sets the type of the musical line.
        /// </summary>
        /// <value>
        /// The type of the musical line.
        /// </value>
        public MusicalLineType LineType { get; set; }

        /// <summary> Gets a value indicating whether Part classification according to current status. </summary>
        /// <value> General musical property.</value>
        [System.Diagnostics.Contracts.Pure]
        public bool IsMelodicalNature => this.LineType == MusicalLineType.Melodic;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is stop.
        /// </summary>
        /// <value>
        ///   Is <c>true</c> if this instance is stop; otherwise, <c>false</c>.
        /// </value>
        public bool IsStop { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets LineTypeString.
        /// </summary>
        /// <value> Property description. </value>
        [System.Diagnostics.Contracts.Pure]
        [UsedImplicitly]
        public string LineTypeString => LocalizedMusic.String("LineType" + ((byte)this.LineType).ToString(CultureInfo.CurrentCulture));

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        [System.Diagnostics.Contracts.Pure]
        public virtual object Clone() {
            var tmc = new AbstractChange(this.BarNumber, this.LineIndex, this.ChangeType);
            //// tmc.BlockModel = this.BlockModel;

            return tmc;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(string.Format(CultureInfo.CurrentCulture, "Bar number {0},", this.BarNumber));
            s.Append(string.Format(CultureInfo.CurrentCulture, "Line index {0},", this.LineIndex));
            s.Append("Type " + this.ChangeType);
            return s.ToString();
        }
        #endregion
    }
}
