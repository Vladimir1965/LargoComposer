// <copyright file="HarmonicEnergyBar.cs" company="Traced-Ideas, Czech republic">
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
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Energy Bar.
    /// </summary>
    public class HarmonicEnergyBar {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicEnergyBar" /> class.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        public HarmonicEnergyBar(int givenBarNumber) {
            this.BarNumber = givenBarNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicEnergyBar"/> class.
        /// </summary>
        /// <param name="givenHarmonicBar">The given harmonic bar.</param>
        public HarmonicEnergyBar(HarmonicBar givenHarmonicBar) {
            this.BarNumber = givenHarmonicBar.BarNumber;
            this.HarmonicConsonance = (decimal)givenHarmonicBar.MeanConsonance; //// Consonance;
            this.HarmonicPotential = (decimal)givenHarmonicBar.MeanPotential;  //// Potential;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicEnergyBar"/> class.
        /// </summary>
        /// <param name="markElement">The mark element.</param>
        public HarmonicEnergyBar(XElement markElement) {
            Contract.Requires(markElement != null);
            if (markElement == null) {
                return;
            }

            this.BarNumber = XmlSupport.ReadIntegerAttribute(markElement.Attribute("BarNumber"));
            this.HarmonicConsonance = (decimal)XmlSupport.ReadDoubleAttribute(markElement.Attribute("HarmonicConsonance"));
            this.HarmonicPotential = (decimal)XmlSupport.ReadDoubleAttribute(markElement.Attribute("HarmonicPotential"));
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xmbar = new XElement("HarmonicEnergyBar");
                xmbar.Add(new XAttribute("BarNumber", this.BarNumber));
                xmbar.Add(new XAttribute("HarmonicConsonance", Math.Round(this.HarmonicConsonance, 2)));
                xmbar.Add(new XAttribute("HarmonicPotential", Math.Round(this.HarmonicPotential, 2)));
                return xmbar;
            }
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

        #region Decimal properties
        /// <summary>
        /// Gets or sets the mobility.
        /// </summary>
        /// <value>
        /// The mobility.
        /// </value>
        public decimal HarmonicConsonance { get; set; }

        /// <summary>
        /// Gets or sets the filling.
        /// </summary>
        /// <value>
        /// The filling.
        /// </value>
        public decimal HarmonicPotential { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                        "Bar={0,6} Consonance={1,6} Potential={2,6} ",
                        this.BarNumber,
                        this.HarmonicConsonance,
                        this.HarmonicPotential);
            return s.ToString();
        }
        #endregion
    }
}