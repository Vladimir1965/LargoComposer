// <copyright file="SettingsAnalysis.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Models;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings Analysis.
    /// </summary>
    public class SettingsAnalysis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAnalysis"/> class.
        /// </summary>
        public SettingsAnalysis() {
            //// 
            this.HarmonicAnalysis = HarmonicAnalysisType.DivisionByTicks; //// HalfBarDivision;
            this.MinimalModalityLevel = 7;
            this.FullHarmonization = true; //// no imitations
        }

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement markSettings = new XElement("Analysis");
                markSettings.Add(new XAttribute("LongTones", this.LongTones));
                markSettings.Add(new XAttribute("MinimalModalityLevel", this.MinimalModalityLevel));
                markSettings.Add(new XAttribute("FullHarmonization", this.FullHarmonization));
                markSettings.Add(new XAttribute("HarmonicAnalysis", this.HarmonicAnalysis));
                return markSettings;
            }
        }
        #endregion

        // public bool AnalyzeAfterLoad { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [long tones].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [long tones]; otherwise, <c>false</c>.
        /// </value>
        public bool LongTones { get; set; }

        /// <summary>
        /// Gets or sets the minimal modality level.
        /// </summary>
        /// <value>
        /// The minimal modality level.
        /// </value>
        public byte MinimalModalityLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether full Harmonization. It provides harmony to e.g. one-voice song.
        /// Not-Full Harmonization is aimed for imitations (similarities).
        /// </summary>
        /// <value>
        ///   <c>True</c> if [full harmonization]; otherwise, <c>false</c>.
        /// </value>
        public bool FullHarmonization { get; set; }

        /// <summary>
        /// Gets or sets Harmonic Analysis - applied during analyze of music.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public HarmonicAnalysisType HarmonicAnalysis { get; set; }

        /// <summary>
        /// Sets the x element.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void SetXElement(XElement markSettings) {
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            this.LongTones = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("LongTones"));
            this.MinimalModalityLevel = XmlSupport.ReadByteAttribute(markSettings.Attribute("MinimalModalityLevel"));
            this.FullHarmonization = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("FullHarmonization"));
            //// this.HarmonicAnalysis = DataEnums.ReadAttributeHarmonicAnalysis(markTrack.Attribute("HarmonicAnalysis")),
        }
    }
}

//// public bool AnalyzeAfterLoad { get; set; }
