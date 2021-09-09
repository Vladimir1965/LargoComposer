// <copyright file="TempoChange.cs" company="Traced-Ideas, Czech republic">
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
    /// Instrument Change.
    /// </summary>
    public sealed class TempoChange : AbstractChange {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TempoChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public TempoChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempoChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public TempoChange(XElement xchange)
            : base(xchange) {
                Contract.Requires(xchange != null);
           //// if (xchange == null) { return; }

           this.TempoNumber = XmlSupport.ReadByteAttribute(xchange.Attribute("TempoNumber"));
           this.ChangeType = MusicalChangeType.Tempo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempoChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public TempoChange(int givenBar)
            : base(givenBar, 0, MusicalChangeType.Tempo) {
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
                change.Add(new XAttribute("TempoNumber", this.TempoNumber));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public int TempoNumber { get; set; }

        /// <summary>
        /// Gets InstrumentString.
        /// </summary>
        /// <value> General musical property.</value>
        //// Do not make private!!! - It is used by DetailMusicalChanges.xaml.
        [UsedImplicitly]
        public string TempoString => string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.TempoNumber.ToString(CultureInfo.CurrentCulture), MusicalProperties.GetTempoValue(this.TempoNumber));

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new TempoChange(this.BarNumber) { TempoNumber = this.TempoNumber };
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
            s.Append(", " + this.TempoString);
            return s.ToString();
        }
        #endregion
    }
}
