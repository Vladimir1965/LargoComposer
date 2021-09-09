// <copyright file="RhythmicPattern.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Rhythm
{
    /// <summary>
    /// Rhythmic Pattern.
    /// </summary>
    public class RhythmicPattern {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPattern"/> class.
        /// </summary>
        public RhythmicPattern() {
            this.Voices = new List<RhythmicPatternVoice>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPattern" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="givenBar">The given bar.</param>
        public RhythmicPattern(MusicalHeader givenHeader, MusicalBar givenBar)
            : this() {
            this.Number = givenBar.BarNumber;
            this.Header = givenHeader;
            this.RhythmicOrder = givenHeader.System.RhythmicOrder;
            foreach (var element in givenBar.Elements) {
                if (element.Status.LineType != MusicalLineType.Rhythmic && element.Status.LineType != MusicalLineType.Melodic) {
                    continue;
                }

                if (element.Status.RhythmicStructure == null || element.Status.RhythmicStructure.Level == 0) {
                    continue;
                }
                                
                //// if (element.Status.Instrument.RhythmicInstrument == MidiRhythmicInstrument.None) { continue; }
                var voice = new RhythmicPatternVoice(this, element);
                if (!voice.IsEmpty && !this.ExistsPatternVoice(voice)) {
                    voice.Pattern = this;
                    this.Voices.Add(voice);
                }
            }

            this.InitializeVoiceProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPattern"/> class.
        /// </summary>
        /// <param name="xpattern">The pattern element.</param>
        public RhythmicPattern(XElement xpattern) {
            XElement xheader = xpattern.Element("Header");
            this.Header = new MusicalHeader(xheader, true);
            this.SetName = (string)xpattern.Attribute("SetName");

            var number = (int)xpattern.Attribute("Number");
            this.Number = (byte)number;

            var tickOrder = (int)xpattern.Attribute("TickOrder");
            this.TickOrder = (byte)tickOrder;

            var rhythmicOrder = (int)xpattern.Attribute("RhythmicOrder");
            this.RhythmicOrder = (byte)rhythmicOrder;

            this.Header = new MusicalHeader(xpattern.Element("Header"), true);

            var xvoicesTag = xpattern.Elements("Voices");
            var xvoices = xvoicesTag.Elements("Voice");

            this.Voices = new List<RhythmicPatternVoice>();
            foreach (var xvoice in xvoices) {
                var voice = new RhythmicPatternVoice(this.RhythmicOrder, this.RhythmicOrder, xvoice) { Pattern = this };
                this.Voices.Add(voice);
            }

            this.InitializeVoiceProperties();
        }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get x element.
        /// </summary>
        /// <value>
        /// The get x element.
        /// </value>
        public XElement GetXElement {
            get {
                if (this.RhythmicDivisor == 0) {
                    this.RhythmicDivisor = 1;
                }

                XElement topElement = new XElement("RhythmicPattern");

                var xheader = this.Header.GetXElement;
                topElement.Add(xheader);

                topElement.Add(new XAttribute("SetName", this.SetName));
                topElement.Add(new XAttribute("Number", this.Number));
                topElement.Add(new XAttribute("TickOrder", this.RhythmicOrder / this.RhythmicDivisor));
                topElement.Add(new XAttribute("RhythmicOrder", this.RhythmicOrder));
                topElement.Add(this.Header.GetXElement);

                XElement xvoices = new XElement("Voices");
                foreach (var v in this.Voices) {
                    xvoices.Add(v.GetXElement);
                }

                topElement.Add(xvoices);
                return topElement;
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
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int VoiceCount { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int VoiceLevel { get; set; }

        /// <summary>
        /// Gets or sets the voices levels.
        /// </summary>
        /// <value>
        /// The voices levels.
        /// </value>
        public string VoiceLevels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RhythmicPattern"/> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the name of the set.
        /// </summary>
        /// <value>
        /// The name of the set.
        /// </value>
        public string SetName { get; set; }

        /// <summary>
        /// Gets or sets The rhythmic order
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets The rhythmic order
        /// </summary>
        public byte TickOrder { get; set; }

        /// <summary>
        /// Gets or sets The rhythmic order
        /// </summary>
        public byte RhythmicOrder { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic divisor.
        /// </summary>
        /// <value>
        /// The rhythmic divisor.
        /// </value>
        public byte RhythmicDivisor { get; set; }

        /// <summary>
        /// Gets or sets The voices
        /// </summary>
        public List<RhythmicPatternVoice> Voices { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Identifier {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.RhythmicOrder);
                foreach (var v in this.Voices) {
                    sb.Append(v.Identifier);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => this.Voices == null || this.Voices.Count == 0;

        #endregion

        #region Public static

        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0,6} Set={1,20} Cnt={2,4}", this.Number, this.SetName, this.VoiceCount);
            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Exists the pattern.
        /// </summary>
        /// <param name="givenPatterns">The given patterns.</param>
        /// <returns>Returns value.</returns>
        public bool ExistsInPatterns(IList<RhythmicPattern> givenPatterns) {
            var ident = this.Identifier;
            return givenPatterns.Any(pattern => pattern.Identifier == ident);
        }

        /// <summary>
        /// Exists the pattern.
        /// </summary>
        /// <param name="givenVoice">The given voice.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool ExistsPatternVoice(RhythmicPatternVoice givenVoice) {
            var ident = givenVoice.Identifier;
            return this.Voices.Any(voice => voice.Identifier == ident);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Initializes the voice properties.
        /// </summary>
        private void InitializeVoiceProperties() {
            if (!this.Voices.Any()) {
                return;
            }

            this.VoiceCount = this.Voices.Count;
            this.VoiceLevel = (from v in this.Voices select v.Level).Sum(level => level);
            StringBuilder sb = new StringBuilder();
            foreach (var v in this.Voices) {
                sb.AppendFormat("{0},", v.Level);
            }

            var s = sb.ToString();
            if (s.Length > 0) {
                s = s.Substring(0, s.Length - 1);
            }

            this.VoiceLevels = s;
        }
        #endregion
    }
}
