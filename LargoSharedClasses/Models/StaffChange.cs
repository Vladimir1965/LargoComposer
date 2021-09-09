// <copyright file="StaffChange.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Staff Change.
    /// </summary>
    public sealed class StaffChange : AbstractChange {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StaffChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public StaffChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public StaffChange(XElement xchange)
            : base(xchange) {
                Contract.Requires(xchange != null);
           //// if (xchange == null) { return; }

           this.Staff = XmlSupport.ReadByteAttribute(xchange.Attribute("Staff"));
           this.Voice = XmlSupport.ReadByteAttribute(xchange.Attribute("Voice"));
           this.ChangeType = MusicalChangeType.Staff;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        public StaffChange(int givenBar, int givenLine)
            : base(givenBar, givenLine, MusicalChangeType.Staff) {
        }

        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets Xml representation.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public override XElement GetXElement {
            get {
                var change = base.GetXElement;
                change.Add(new XAttribute("Staff", this.Staff));
                change.Add(new XAttribute("Voice", this.Voice));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public byte Staff { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public byte Voice { get; set; }

        /// <summary>
        /// Gets StaffVoiceString.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string StaffVoiceString => string.Format(CultureInfo.CurrentCulture, "{0}/{1}", this.Staff, this.Voice);

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new StaffChange(this.BarNumber, this.LineIndex) {
                Staff = this.Staff, Voice = this.Voice
            };
            //// tmc.BlockModel = this.BlockModel;

            return tmc;
        }
        #endregion
    }
}
