// <copyright file="SettingsProgram.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings Program.
    /// </summary>
    public class SettingsProgram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProgram"/> class.
        /// </summary>
        public SettingsProgram() {
            this.DefaultCulture = 0; //// English
            this.ParallelMode = false;
            this.HasTraceValues = false;
            this.MaxNumberOfBars = 1000;
            this.NumberOfDocumentsToDisplay = 100;
            this.NumberOfResultsToDisplay = 100;

            this.Notator = "Notation Player";
            //// this.Soundfont = "SGM";
        }

        #region Properties - Program main settings
        /// <summary>
        /// Gets or sets The default culture
        /// </summary>
        public byte DefaultCulture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [parallel mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [parallel mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ParallelMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [instrument in voices].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [instrument in voices]; otherwise, <c>false</c>.
        /// </value>
        public bool InstrumentInVoices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check respect pauses].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [check respect pauses]; otherwise, <c>false</c>.
        /// </value>
        public bool RespectPauses { get; set; }

        #endregion

        #region Properties - Internal properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance has trace values.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has trace values; otherwise, <c>false</c>.
        /// </value>
        public bool HasTraceValues { get; set; }

        /// <summary>
        /// Gets or sets the number of documents to display.
        /// </summary>
        /// <value>
        /// The number of documents to display.
        /// </value>
        public int NumberOfDocumentsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets the number of results to display.
        /// </summary>
        /// <value>
        /// The number of results to display.
        /// </value>
        public int NumberOfResultsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of bars.
        /// </summary>
        /// <value>
        /// The maximum number of bars.
        /// </value>
        public int MaxNumberOfBars { get; set; }

        /// <summary>
        /// Gets or sets the note editor.
        /// </summary>
        /// <value>
        /// The note editor.
        /// </value>
        public string Notator { get; set; }

        /// <summary>
        /// Gets or sets the current folder.
        /// </summary>
        /// <value>
        /// The current folder identifier.
        /// </value>
        public string CurrentFolder { get; set; }
        #endregion

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement markSettings = new XElement("Program");
                markSettings.Add(new XAttribute("DefaultCulture", this.DefaultCulture));
                markSettings.Add(new XAttribute("ParallelMode", this.ParallelMode));
                markSettings.Add(new XAttribute("InstrumentInVoices", this.InstrumentInVoices));
                markSettings.Add(new XAttribute("RespectPauses", this.RespectPauses));

                //// markSettings.Add(new XAttribute("CultureInfo", this.CultureInfo));
                markSettings.Add(new XAttribute("CurrentFolder", this.CurrentFolder ?? string.Empty));
                markSettings.Add(new XAttribute("Notator", this.Notator));
                markSettings.Add(new XAttribute("MaxNumberOfBars", this.MaxNumberOfBars));
                return markSettings;
            }
        }

        #region Properties - Xml
        /// <summary>
        /// Sets the x element.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void SetXElement(XElement markSettings) {
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            this.DefaultCulture = XmlSupport.ReadByteAttribute(markSettings.Attribute("DefaultCulture"));
            this.ParallelMode = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("ParallelMode"));
            this.InstrumentInVoices = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("InstrumentInVoices"));
            this.RespectPauses = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("RespectPauses"));

            this.Notator = XmlSupport.ReadStringAttribute(markSettings.Attribute("Notator"));
            this.CurrentFolder = XmlSupport.ReadStringAttribute(markSettings.Attribute("CurrentFolder"));
            this.MaxNumberOfBars = XmlSupport.ReadIntegerAttribute(markSettings.Attribute("MaxNumberOfBars"));
            if (this.MaxNumberOfBars == 0) {
                this.MaxNumberOfBars = 1000; //// Temporary, while settings is not well made 
            }
        }

        #endregion
    }
}
