// <copyright file="RhythmicEnergyBar.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Templates
{
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Rhythm;
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Rhythmic Energy Bar.
    /// </summary>
    public class RhythmicEnergyBar {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicEnergyBar" /> class.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        public RhythmicEnergyBar(int givenBarNumber) {
            this.BarNumber = givenBarNumber;
            this.FormalBehavior = new FormalBehavior();
            this.RhythmicBehavior = new RhythmicBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicEnergyBar"/> class.
        /// </summary>
        /// <param name="rhythmicStructure">The rhythmic structure.</param>
        public RhythmicEnergyBar(RhythmicStructure rhythmicStructure) {
            Contract.Requires(rhythmicStructure != null);
            if (rhythmicStructure == null) {
                return;
            }

            this.ToneLevel = rhythmicStructure.ToneLevel;
            this.Level = rhythmicStructure.Level;
            this.FormalBehavior = rhythmicStructure.FormalBehavior;
            this.RhythmicBehavior = rhythmicStructure.RhythmicBehavior;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicEnergyBar"/> class.
        /// </summary>
        /// <param name="markElement">The mark element.</param>
        public RhythmicEnergyBar(XElement markElement) {
            Contract.Requires(markElement != null);
            if (markElement == null) {
                return;
            }

            this.BarNumber = XmlSupport.ReadIntegerAttribute(markElement.Attribute("BarNumber"));
            this.ToneLevel = XmlSupport.ReadFloatAttribute(markElement.Attribute("ToneLevel"));
            this.Level = XmlSupport.ReadFloatAttribute(markElement.Attribute("Level"));

            this.FormalBehavior = new FormalBehavior
            {
                Variance = XmlSupport.ReadFloatAttribute(markElement.Attribute("RhythmicVariance")),
                Balance = XmlSupport.ReadFloatAttribute(markElement.Attribute("RhythmicBalance"))
            };

            this.RhythmicBehavior = new RhythmicBehavior
            {
                Mobility = XmlSupport.ReadFloatAttribute(markElement.Attribute("RhythmicMobility")),
                Filling = XmlSupport.ReadFloatAttribute(markElement.Attribute("RhythmicFilling")),
                Beat = XmlSupport.ReadFloatAttribute(markElement.Attribute("RhythmicBeat"))
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Bar Number In Motive.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public int BarNumber { get; set; }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xmbar = new XElement("RhythmicEnergyBar");
                xmbar.Add(new XAttribute("BarNumber", this.BarNumber));
                xmbar.Add(new XAttribute("ToneLevel", Math.Round(this.ToneLevel, 2)));
                xmbar.Add(new XAttribute("Level", Math.Round(this.Level, 2)));
                if (this.FormalBehavior != null) {
                    xmbar.Add(new XAttribute("RhythmicVariance", Math.Round(this.FormalBehavior.Variance, 2)));
                    xmbar.Add(new XAttribute("RhythmicBalance", Math.Round(this.FormalBehavior.Balance, 2)));
                }

                if (this.RhythmicBehavior != null) {
                    xmbar.Add(new XAttribute("RhythmicMobility", Math.Round(this.RhythmicBehavior.Mobility, 2)));
                    xmbar.Add(new XAttribute("RhythmicFilling", Math.Round(this.RhythmicBehavior.Filling, 2)));
                    xmbar.Add(new XAttribute("RhythmicBeat", Math.Round(this.RhythmicBehavior.Beat, 2)));
                }

                return xmbar;
            }
        }
        #endregion

        #region Double properties

        /// <summary>
        /// Gets or sets the tone level.
        /// </summary>
        /// <value>
        /// The tone level.
        /// </value>
        public double ToneLevel { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public double Level { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic behavior.
        /// </summary>
        /// <value>
        /// The rhythmic behavior.
        /// </value>
        public RhythmicBehavior RhythmicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the formal behavior.
        /// </summary>
        /// <value>
        /// The formal behavior.
        /// </value>
        public FormalBehavior FormalBehavior { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                        "Bar={0,6} ToneLevel={1,6} Level={2,6} {3} {4}",
                        this.BarNumber, 
                        this.ToneLevel, 
                        this.Level, 
                        this.RhythmicBehavior, 
                        this.FormalBehavior);
            return s.ToString();
        }
        #endregion
    }
}
