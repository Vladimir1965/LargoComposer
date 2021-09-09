// <copyright file="HarmonicChange.cs" company="Traced-Ideas, Czech republic">
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
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Harmonic Change.
    /// </summary>
    public sealed class HarmonicChange : AbstractChange {
        #region Fields
        /// <summary> Harmonic motive. </summary>
        private HarmonicMotive harmonicMotive;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public HarmonicChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public HarmonicChange(XElement xchange)
            : base(xchange) {
                Contract.Requires(xchange != null);
           if (xchange == null) {
                return;
           }

           this.MotiveNumber = XmlSupport.ReadIntegerAttribute(xchange.Attribute("MotiveNumber"));
           this.LineType = MusicalLineType.Harmonic;
           this.ChangeType = MusicalChangeType.Harmonic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public HarmonicChange(int givenBar)
            : base(givenBar, 0, MusicalChangeType.Harmonic) {
            this.LineType = MusicalLineType.Harmonic;
        }

        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public int? MotiveNumber { get; set; }

        /// <summary>
        /// Gets or sets THarmonicMotive.
        /// </summary>
        /// <value> General musical property.</value>
        public HarmonicMotive HarmonicMotive {
            get {
                var isPrepared = this.harmonicMotive != null; //// && this.tharMotive.Number == this.MotiveNumber && this.tharMotive.CoreId == this.BlockModel.MusicalCore.Id;
                if (isPrepared) {
                    return this.harmonicMotive;
                }

                if (this.MotiveNumber == null) {
                    return null;
                }
                
                //// if (this.BlockModel != null) {
                ////     this.harmonicMotive = this.BlockModel.Core.HarmonicCore.GetHarmonicMotive((int)this.MotiveNumber);
                //// }

                return this.harmonicMotive ?? (this.harmonicMotive = new HarmonicMotive());
            }

            set {
                Contract.Requires(value != null);

                this.harmonicMotive = value;
                this.MotiveNumber = this.harmonicMotive.Number;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new HarmonicChange(this.BarNumber) { MotiveNumber = this.MotiveNumber };
            //// tmc.BlockModel = this.BlockModel;

            return tmc;
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, base.ToString());
            s.Append(", Motive " + this.MotiveNumber);
            return s.ToString();
        }
        #endregion
    }
}
