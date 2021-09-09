// <copyright file="SettingsImport.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings Import.
    /// </summary>
    public class SettingsImport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsImport"/> class.
        /// </summary>
        public SettingsImport()
        {
            this.LastUsedFormat = MusicalSourceType.MIDI;
            this.SplitMultiTracks = FileSplit.None;
            this.SkipNegligibleTones = true;
        }

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement markSettings = new XElement("Import");
                markSettings.Add(new XAttribute("LastUsedFormat", this.LastUsedFormat));
                markSettings.Add(new XAttribute("SplitMultiTracks", this.SplitMultiTracks));
                markSettings.Add(new XAttribute("SkipNegligibleTones", this.SkipNegligibleTones));

                return markSettings;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the last used source.
        /// </summary>
        /// <value>
        /// The last used source.
        /// </value>
        public MusicalSourceType LastUsedFormat { get; set; }

        /// <summary>
        /// Gets or sets the split multi tracks.
        /// </summary>
        /// <value>
        /// The split multi tracks.
        /// </value>
        public FileSplit SplitMultiTracks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip negligible tones].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip negligible tones]; otherwise, <c>false</c>.
        /// </value>
        public bool SkipNegligibleTones { get; set; }
        #endregion

        /// <summary>
        /// Sets the x element.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void SetXElement(XElement markSettings)
        {
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            this.LastUsedFormat = DataEnums.ReadAttributeSourceType(markSettings.Attribute("LastUsedFormat"));
            this.SplitMultiTracks = DataEnums.ReadAttributeFileSplit(markSettings.Attribute("SplitMultiTracks"));
            this.SkipNegligibleTones = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("SkipNegligibleTones"));
        }
    }
}
