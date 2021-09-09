// <copyright file="OrchestraBlock.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;

namespace LargoSharedClasses.Orchestra
{
    /// <summary>  Musical material. </summary>
    /// <remarks> Musical class. </remarks>
    ////    [Serializable]
    [ContractVerification(false)]
    public sealed class OrchestraBlock {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the OrchestraBlock class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="givenBarNumber">The given bar number.</param>
        /// <param name="givenStrip">The given strip.</param>
        public OrchestraBlock(MusicalHeader givenHeader, int givenBarNumber, OrchestraStrip givenStrip)
            : this() {
            this.Header = givenHeader;
            this.BarNumberFrom = givenBarNumber;
            this.Strip = givenStrip;
            this.DetermineProperties();
            //// this.DetermineOrchestraGroup();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestraBlock" /> class.
        /// </summary>
        /// <param name="markOrchestraBlock">The mark orchestra block.</param>
        public OrchestraBlock(XElement markOrchestraBlock) : this() {
            Contract.Requires(markOrchestraBlock != null);
            if (markOrchestraBlock == null) {
                return;
            }

            XElement xheader = markOrchestraBlock.Element("Header");
            this.Header = new MusicalHeader(xheader, true);

            this.BarNumberFrom = XmlSupport.ReadIntegerAttribute(markOrchestraBlock.Attribute("BarNumberFrom"));
            this.BarNumberTo = XmlSupport.ReadIntegerAttribute(markOrchestraBlock.Attribute("BarNumberTo"));
            this.NumberOfVocals = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfVocals"));
            this.NumberOfStrings = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfStrings"));
            this.NumberOfWoodwinds = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfWoodwinds"));
            this.NumberOfKeyboards = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfKeyboards"));
            this.NumberOfBrass = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfBrass"));
            this.NumberOfGuitars = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfGuitars"));
            this.NumberOfSynthetic = XmlSupport.ReadByteAttribute(markOrchestraBlock.Attribute("NumberOfSynthetic"));

            var xElement = markOrchestraBlock.Element("Lines");
            if (xElement != null) {
                var xtracks = xElement.Elements("Line");
                foreach (var xtrack in xtracks) {
                    OrchestraVoice line = new OrchestraVoice(xtrack);
                    this.Strip.OrchestraVoices.Add(line);
                }

                this.Strip.RecomputeProperties();
            }
        }

        /// <summary>
        /// Initializes a new instance of the OrchestraBlock class.
        /// </summary>
        public OrchestraBlock() {
            this.Strip = new OrchestraStrip();
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xmblock = new XElement(
                        "Block",
                        new XAttribute("BarNumberFrom", this.BarNumberFrom),
                        new XAttribute("BarNumberTo", this.BarNumberTo),
                        new XAttribute("TrackCount", this.TrackCount),
                        new XAttribute("NumberOfVocals", this.NumberOfVocals),
                        new XAttribute("NumberOfStrings", this.NumberOfStrings),
                        new XAttribute("NumberOfWoodwinds", this.NumberOfWoodwinds),
                        new XAttribute("NumberOfKeyboards", this.NumberOfKeyboards),
                        new XAttribute("NumberOfBrass", this.NumberOfBrass),
                        new XAttribute("NumberOfGuitars", this.NumberOfGuitars),
                        new XAttribute("NumberOfSynthetic", this.NumberOfSynthetic));

                var xheader = this.Header.GetXElement;
                xmblock.Add(xheader);

                //// Lines
                XElement xtracks = new XElement("Lines");
                foreach (OrchestraVoice track in this.Strip.OrchestraVoices.Where(track => track != null)) {
                    //// mtrack.MusicalBlock = givenMusicalBlock;
                    var xtrack = track.GetXElement;
                    xtracks.Add(xtrack);
                }

                xmblock.Add(xtracks);
                return xmblock;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName => this.Header.FileName;

        /// <summary>
        /// Gets or sets the bar number from.
        /// </summary>
        /// <value>
        /// The bar number from.
        /// </value>
        public int BarNumberFrom { get; set; }

        /// <summary>
        /// Gets or sets the bar number to.
        /// </summary>
        /// <value>
        /// The bar number to.
        /// </value>
        public int BarNumberTo { get; set; }

        /// <summary>
        /// Gets or sets the orchestra strip.
        /// </summary>
        /// <value>
        /// The orchestra strip.
        /// </value>
        public OrchestraStrip Strip { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        [UsedImplicitly]
        public string Description {
            get {
                var description = string.Format(CultureInfo.InvariantCulture, "Bar {0,4}-{1,4}", this.BarNumberFrom, this.BarNumberTo);
                return description;
            }
        }

        /// <summary>
        /// Gets the section outline.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string OrchestraOutline => this.OrchestraGroup.ToString();

        /// <summary>
        /// Gets the section outline.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string SectionOutline {
            get {
                var s = new StringBuilder();

                if (this.NumberOfStrings > 0) {
                    s.Append("Strings,");
                }

                if (this.NumberOfWoodwinds > 0) {
                    s.Append("Woodwinds,");
                }

                if (this.NumberOfBrass > 0) {
                    s.Append("Brass,");
                }

                if (this.NumberOfGuitars > 0) {
                    s.Append("Guitars,");
                }

                if (this.NumberOfKeyboards > 0) {
                    s.Append("Keyboards,");
                }

                if (this.NumberOfSynthetic > 0) {
                    s.Append("Synthetic,");
                }

                var outline = s.ToString();
                if (outline.Length > 1) {
                    outline = outline.Substring(0, outline.Length - 1);
                }

                return outline;
            }
        }

        /// <summary>
        /// Gets the voice count.
        /// </summary>
        /// <value>
        /// The voice count.
        /// </value>
        public int TrackCount {
            get 
            {
                if (this.Strip == null) {
                    return 0;
                }

                return this.Strip.OrchestraVoices.Count;
            }
        }

        /// <summary>
        /// Gets the melodic track count.
        /// </summary>
        /// <value>
        /// The melodic track count.
        /// </value>
        public int MelodicLineCount {
            get {
                if (this.Strip == null) {
                    return 0;
                }

                return this.Strip.MelodicOrchestraVoices.Count;
            }
        }

        /// <summary>
        /// Gets the rhythmic track count.
        /// </summary>
        /// <value>
        /// The rhythmic track count.
        /// </value>
        public int RhythmicLineCount {
            get {
                if (this.Strip == null) {
                    return 0;
                }

                return this.Strip.RhythmicOrchestraVoices.Count;
            }
        }

        /// <summary>
        /// Gets the section count.
        /// </summary>
        /// <value>
        /// The section count.
        /// </value>
        public int SectionCount
        {
            get
            {
                if (this.Strip == null) {
                    return 0;
                }

                return this.Strip.SectionCount;
            }
        }

        /// <summary>
        /// Gets the instrument count.
        /// </summary>
        /// <value>
        /// The instrument count.
        /// </value>
        public int InstrumentCount
        {
            get
            {
                if (this.Strip == null) {
                    return 0;
                }

                return this.Strip.InstrumentCount;
            }
        }

        /// <summary>
        /// Gets or sets the instrument group.
        /// </summary>
        /// <value> Property description. </value>
        public OrchestraGroup OrchestraGroup { get; set; }

        #endregion

        #region Private properties

        /// <summary>
        /// Gets or sets a value indicating whether [number of strings].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of strings]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfStrings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [number of woodwinds].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of woodwinds]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfWoodwinds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [number of keyboards].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of keyboards]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfKeyboards { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [number of brass].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of brass]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfBrass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [number of guitars].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of guitars]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfGuitars { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [number of synthetic].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [number of synthetic]; otherwise, <c>false</c>.
        /// </value>
        public byte NumberOfSynthetic { get; set; }

        /// <summary>
        /// Gets or sets the number of vocals.
        /// </summary>
        /// <value> Property description. </value>
        public byte NumberOfVocals { get; set; }
        #endregion 

        #region Public methods
        /// <summary>
        /// Determine Instrumentation Class.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private OrchestraGroup DetermineOrchestraGroup() {
            var ic = OrchestraGroup.Uniform;
            if (this.TrackCount == 1) {
                ic = OrchestraGroup.Solo;
            }

            if (this.NumberOfVocals > 1) {
                ic = OrchestraGroup.Choral;
                return ic;
            }

            if (this.NumberOfVocals == 1) {
                ic = OrchestraGroup.Vocal;
                return ic;
            }

            //// if (this.HasSynthetic) { ic = OrchestraGroup.Synthetic; } 

            var n = 0;
            if (this.NumberOfStrings > 0) {
                n++;
            }

            if (this.NumberOfWoodwinds > 0) {
                n++;
            }

            if (this.NumberOfBrass > 0) {
                n++;
            }

            if (this.NumberOfGuitars > 0) {
                n++;
            }

            if (this.NumberOfKeyboards > 0) {
                n++;
            }

            if (n > 1) {
                ic = this.NumberOfSynthetic > 0 ? OrchestraGroup.ModernGroup : OrchestraGroup.ChamberMusic;
            }

            if (this.NumberOfStrings > 0 && this.NumberOfWoodwinds > 0 && this.NumberOfBrass > 0) {
                ic = this.NumberOfSynthetic > 0 ? OrchestraGroup.ModernOrchestra : OrchestraGroup.ClassicOrchestra;
            }

            //// ic = InstrumentGroup.Others;
            return ic;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Determines the properties.
        /// </summary>
        private void DetermineProperties() {
            this.NumberOfStrings = 0;
            this.NumberOfWoodwinds = 0;
            this.NumberOfKeyboards = 0;
            this.NumberOfGuitars = 0;
            this.NumberOfBrass = 0;
            this.NumberOfSynthetic = 0;
            this.NumberOfVocals = 0;

            if (PortInstruments.MelodicInstruments == null) {
                return;
            }

            var strings = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Strings);
            var woodwinds = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Woodwind);
            var keyboards = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Keyboards);
            var guitars = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Guitars);
            var brass = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Brass);
            var synthetics = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Synthetic);
            var vocals = PortInstruments.MelodicInstrumentsOfGroup(InstrumentGroupMelodic.Vocal);

            foreach (var track in this.Strip.OrchestraVoices) {
                var instrument = track.InstrumentNumber;

                if (strings.Contains(instrument)) {
                    this.NumberOfStrings++;
                }
                
                if (woodwinds.Contains(instrument)) {
                    this.NumberOfWoodwinds++;
                }

                if (keyboards.Contains(instrument)) {
                    this.NumberOfKeyboards++;
                }

                if (guitars.Contains(instrument)) {
                    this.NumberOfGuitars++;
                }

                if (brass.Contains(instrument)) {
                    this.NumberOfBrass++;
                }

                if (synthetics.Contains(instrument)) {
                    this.NumberOfSynthetic++;
                }

                if (vocals.Contains(instrument)) {
                    this.NumberOfVocals++;
                }
            }
        }
        #endregion
    }
}