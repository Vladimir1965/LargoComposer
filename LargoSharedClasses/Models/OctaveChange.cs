// <copyright file="OctaveChange.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Octave Change.
    /// </summary>
    public sealed class OctaveChange : AbstractChange
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public OctaveChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public OctaveChange(XElement xchange)
            : base(xchange) {
            Contract.Requires(xchange != null);
            //// if (xchange == null) {  return;  }

            this.MusicalOctave = DataEnums.ReadAttributeMusicalOctave(xchange.Attribute("MusicalOctave"));
            this.MusicalBand = DataEnums.ReadAttributeMusicalBandType(xchange.Attribute("MusicalBand"));
            ////201509!!!!! this.MusicalOctave = (MusicalOctave)LibSupport.ReadByteAttribute(xchange.Attribute("MusicalOctave"));
            //// this.MusicalOctave = LargoBase.Enums.MusicalOctave.OneLine;
            ////201509!!!!! this.MusicalBand = (MusicalBand)LibSupport.ReadByteAttribute(xchange.Attribute("MusicalBand"));
            //// this.MusicalBand = LargoBase.Enums.MusicalBand.MiddleTones;
            this.ChangeType = MusicalChangeType.Octave;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        public OctaveChange(int givenBar, int givenLine)
            : base(givenBar, givenLine, MusicalChangeType.Octave) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenOctave">The given octave.</param>
        public OctaveChange(int givenBar, int givenLine, MusicalOctave givenOctave)
            : base(givenBar, givenLine, MusicalChangeType.Octave) {
            this.MusicalOctave = givenOctave;
            this.MusicalBand = MusicalProperties.BandTypeFromOctave(givenOctave);
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
                change.Add(new XAttribute("MusicalOctave", this.MusicalOctave));
                change.Add(new XAttribute("MusicalBand", this.MusicalBand));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalOctave MusicalOctave { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalBand MusicalBand { get; set; }

        /// <summary>
        /// Gets BandTypeString.
        /// </summary>
        /// <value> Property description. </value>
        //// Do not make private!!! - It is used by DetailMusicalChanges.xaml.
        [UsedImplicitly]
        public string OctaveString => LocalizedMusic.String("Octave" + ((int)this.MusicalOctave).ToString(CultureInfo.CurrentCulture));

        /// <summary>
        /// Gets BandTypeString.
        /// </summary>
        /// <value> Property description. </value>
        //// Do not make private!!! - It is used by DetailMusicalChanges.xaml.
        [UsedImplicitly]
        public string BandTypeString => LocalizedMusic.String("Band" + ((int)this.MusicalBand).ToString(CultureInfo.CurrentCulture));

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new OctaveChange(this.BarNumber, this.LineIndex) {
                MusicalOctave = this.MusicalOctave,
                MusicalBand = this.MusicalBand
            };
            //// tmc.BlockModel = this.BlockModel;

            return tmc;
        }
        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, base.ToString());
            s.Append("," + this.OctaveString);
            s.Append("," + this.BandTypeString);
            return s.ToString();
        }
        #endregion
    }
}
