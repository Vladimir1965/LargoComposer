// <copyright file="TonalityChange.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Tonality Change.
    /// </summary>
    public sealed class TonalityChange : AbstractChange {
        #region Fields

        /// <summary>
        /// Harmonic Modality.
        /// </summary>
        private HarmonicModality harmonicModality;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TonalityChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public TonalityChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TonalityChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public TonalityChange(XElement xchange)
            : base(xchange) {
                Contract.Requires(xchange != null);
           //// if (xchange == null) {  return;  }

           this.HarmonicModalityCode = XmlSupport.ReadStringAttribute(xchange.Attribute("HarmonicModalityCode"));
           this.ChangeType = MusicalChangeType.Tonality;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TonalityChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public TonalityChange(int givenBar)
            : base(givenBar, 0, MusicalChangeType.Tonality) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TonalityChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenHarmonicModalityCode">The given harmonic modality code.</param>
        public TonalityChange(int givenBar, string givenHarmonicModalityCode)
            : base(givenBar, 0, MusicalChangeType.Tonality) {
            this.HarmonicModalityCode = givenHarmonicModalityCode;
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
                change.Add(new XAttribute("HarmonicModalityCode", this.HarmonicModalityCode ?? string.Empty));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the harmonic modality code.
        /// </summary>
        /// <value>
        /// The harmonic modality code.
        /// </value>
        [UsedImplicitly]
        public string HarmonicModalityCode { get; set; }

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        [UsedImplicitly]
        public HarmonicModality HarmonicModality
        {
            get
            {
                if (this.harmonicModality != null)
                {
                    return this.harmonicModality;
                }

                return this.harmonicModality;
            }

            set => this.harmonicModality = value;
        }

        /// <summary>
        /// Gets the modality outline.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string ModalityOutline
        {
            get
            {
                var hm = this.HarmonicModality;
                if (hm != null)
                {
                    return hm.ToneSchema;
                }

                return string.Empty;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new TonalityChange(this.BarNumber) {
                HarmonicModalityCode = this.HarmonicModalityCode,
                HarmonicModality = this.HarmonicModality
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
            s.Append("," + this.HarmonicModalityCode);
            return s.ToString();
        }
        #endregion
    }
}
