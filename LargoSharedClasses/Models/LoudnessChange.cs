// <copyright file="LoudnessChange.cs" company="Traced-Ideas, Czech republic">
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
    /// Loudness Change.
    /// </summary>
    public sealed class LoudnessChange : AbstractChange
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LoudnessChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public LoudnessChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoudnessChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public LoudnessChange(XElement xchange)
            : base(xchange) {
            Contract.Requires(xchange != null);
            //// if (xchange == null) { return; }

            this.LoudnessBase = DataEnums.ReadAttributeMusicalLoudness(xchange.Attribute("MusicalLoudness"));
            ////201509!!!!! this.LoudnessBase = (MusicalLoudness)LibSupport.ReadByteAttribute(xchange.Attribute("LoudnessBase"));
            //// this.LoudnessBase = MusicalLoudness.MeanLoudness; 
            this.ChangeType = MusicalChangeType.Loudness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoudnessChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenLoudness">The given loudness.</param>
        public LoudnessChange(int givenBar, int givenLine, MusicalLoudness givenLoudness)
            : base(givenBar, givenLine, MusicalChangeType.Loudness) {
            this.LoudnessBase = givenLoudness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoudnessChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        public LoudnessChange(int givenBar, int givenLine)
            : base(givenBar, givenLine, MusicalChangeType.Loudness) {
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
                change.Add(new XAttribute("LoudnessBase", this.LoudnessBase));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness LoudnessBase { get; set; }

        /// <summary>
        /// Gets LoudnessBaseString.
        /// </summary>
        /// <value> Property description. </value>
        //// Do not make private!!! - It is used by DetailMusicalChanges.xaml.
        [UsedImplicitly]
        public string LoudnessBaseString => LocalizedMusic.String("Loud" + ((byte)this.LoudnessBase).ToString(CultureInfo.CurrentCulture));

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new LoudnessChange(this.BarNumber, this.LineIndex) { LoudnessBase = this.LoudnessBase };
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
            s.Append(", " + this.LoudnessBaseString);
            return s.ToString();
        }
        #endregion
    }
}
