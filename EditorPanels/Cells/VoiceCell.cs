// <copyright file="VoiceCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Cells
{
    using EditorPanels;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Music;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    /// <summary>
    /// Interact logic for LineCell.
    /// </summary>
    public class VoiceCell : BaseCell
    {
        #region Constructors

        /// <summary>
        /// <summary> Initializes a new instance of the VoiceCell class. </summary>
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        /// <param name="givenVoice">The given voice.</param>
        public VoiceCell(LineSpace givenMaster, IAbstractVoice givenVoice) : base(givenMaster) {
            if (givenVoice == null) {
                var instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1);
                var octave = MusicalOctave.OneLine;
                var loudness = MusicalLoudness.MeanLoudness;
                this.Voice = new MusicalVoice {
                    Instrument = instrument,
                    Octave = octave,
                    Loudness = loudness
                };
                return;
            }

            this.Voice = givenVoice;
            this.LineIndex = givenVoice.Line.LineIndex;
        }

        #endregion

        #region Main Properties
        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        /// <value>
        /// The voice.
        /// </value>
        public IAbstractVoice Voice { get; set; }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the X element.
        /// </summary>
        /// <returns> Returns value. </returns>
        public XElement GetXElement {
            get {
                var xstatus = new XElement("Status", null);
                xstatus.Add(new XAttribute("Octave", this.Voice.Octave));
                xstatus.Add(new XAttribute("Loudness", this.Voice.Loudness));
                var instr = this.Voice.Instrument ?? new MusicalInstrument(MidiMelodicInstrument.None);
                xstatus.Add(instr.GetXElement);
                return xstatus;
            }
        }
        #endregion

        #region Public methods - Set Xml

        /// <summary>
        /// Sets the X element.
        /// </summary>
        /// <param name="xelement">The element.</param>
        public void SetXElement(XElement xelement) { //// MusicalHeader givenHeader
            Contract.Requires(xelement != null);

            var attribute = xelement.Attribute("Octave");
            if (attribute != null) {
                this.Voice.Octave = DataEnums.ReadAttributeMusicalOctave(attribute);
            }

            attribute = xelement.Attribute("Loudness");
            if (attribute != null) {
                this.Voice.Loudness = DataEnums.ReadAttributeMusicalLoudness(attribute);
            }

            var xinstrument = xelement.Element("Instrument");
            if (xinstrument != null) {
                this.Voice.Instrument = new MusicalInstrument(xinstrument); //// : null;
                //// 2016 DataEnums.ReadAttributeMidiMelodicInstrument(xelement.Attribute("MelodicInstrument"));
            }
        }

        #endregion

        #region Public methods

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}/ {1} {2}\n", this.LineIndex + 1, this.Voice.Octave, this.Voice.Loudness);
            sb.Append(this.Voice.Instrument);
            var ft = Abstract.AbstractText.Singleton.FormatText(sb.ToString(), (int)this.Width - SeedSize.BasicMargin);
            return ft;
        }
        #endregion

        #region Copy-Paste
        /// <summary>
        /// Copies this instance.
        /// </summary>
        public override void Copy() {
            StringBuilder sb = new StringBuilder();
            var xstatus = this.Voice.GetXElement;
            var item = xstatus.ToString();
            sb.AppendFormat("{0};", item);
            Clipboard.SetText(sb.ToString());
            //// Clipboard.SetDataObject(xstatus); 

            Console.Beep(880, 180);
        }

        /// <summary>
        /// Pastes this instance.
        /// </summary>
        public override void Paste() {
            var s = Clipboard.GetText();
            if (string.IsNullOrEmpty(s)) {
                return;
            }

            var splitArray = s.Split(';');
            if (!splitArray.Any()) {
                return;
            }

            var item = splitArray.First();
            //// var xstatus = XElement.Parse(item);
            //// this.voice
            Console.Beep(990, 180);
        }
        #endregion
    }
}