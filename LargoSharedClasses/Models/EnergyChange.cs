// <copyright file="EnergyChange.cs" company="Traced-Ideas, Czech republic">
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
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Energy Change.
    /// </summary>
    public sealed class EnergyChange : AbstractChange {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public EnergyChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public EnergyChange(XElement xchange)
            : base(xchange) {
            Contract.Requires(xchange != null);
            if (xchange == null) {
                return;
            }

            this.BeatLevel = XmlSupport.ReadByteAttribute(xchange.Attribute("BeatLevel"));
            this.ToneLevel = XmlSupport.ReadByteAttribute(xchange.Attribute("ToneLevel"));
            this.RhythmicTension = XmlSupport.ReadByteAttribute(xchange.Attribute("RhythmicTension"));
            this.MelodicDirection = XmlSupport.ReadByteAttribute(xchange.Attribute("MelodicDirection"));
            this.HarmonicPotential = XmlSupport.ReadByteAttribute(xchange.Attribute("HarmonicPotential"));
            this.ChangeType = MusicalChangeType.Energy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        public EnergyChange(int givenBar, int givenLine)
            : base(givenBar, givenLine, MusicalChangeType.Energy) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenBeatLevel">The given beat level.</param>
        /// <param name="givenToneLevel">The given tone level.</param>
        /// <param name="givenRhythmicTension">The given rhythmic tension.</param>
        public EnergyChange(int givenBar, int givenLine, byte givenBeatLevel, byte givenToneLevel, byte givenRhythmicTension)
            : base(givenBar, givenLine, MusicalChangeType.Energy) {
            this.BeatLevel = givenBeatLevel;
            this.ToneLevel = givenToneLevel;
            this.RhythmicTension = givenRhythmicTension;
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
                change.Add(new XAttribute("BeatLevel", this.BeatLevel));
                change.Add(new XAttribute("ToneLevel", this.ToneLevel));
                change.Add(new XAttribute("RhythmicTension", this.RhythmicTension));
                change.Add(new XAttribute("MelodicDirection", this.MelodicDirection));
                change.Add(new XAttribute("HarmonicPotential", this.HarmonicPotential));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the beat level.
        /// </summary>
        /// <value>
        /// The beat level.
        /// </value>
        [UsedImplicitly]
        public byte BeatLevel { get; set; }

        /// <summary>
        /// Gets or sets the tone level.
        /// </summary>
        /// <value>
        /// The tone level.
        /// </value>
        [UsedImplicitly]
        public byte ToneLevel { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic tension.
        /// </summary>
        /// <value>
        /// The rhythmic tension.
        /// </value>
        [UsedImplicitly]
        public byte RhythmicTension { get; set; }

        /// <summary>
        /// Gets or sets the melodic direction.
        /// </summary>
        /// <value>
        /// The melodic direction.
        /// </value>
        [UsedImplicitly]
        public byte MelodicDirection { get; set; }

        /// <summary>
        /// Gets or sets the harmonic potential.
        /// </summary>
        /// <value>
        /// The harmonic potential.
        /// </value>
        [UsedImplicitly]
        public byte HarmonicPotential { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new EnergyChange(this.BarNumber, this.LineIndex) {
                BeatLevel = this.BeatLevel,
                ToneLevel = this.ToneLevel,
                RhythmicTension = this.RhythmicTension,
                MelodicDirection = this.MelodicDirection,
                HarmonicPotential = this.HarmonicPotential
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
            s.Append("," + this.BeatLevel);
            s.Append("," + this.ToneLevel);
            s.Append("," + this.RhythmicTension);
            s.Append("," + this.MelodicDirection);
            s.Append("," + this.HarmonicPotential);
            return s.ToString();
        }
        #endregion
    }
}
