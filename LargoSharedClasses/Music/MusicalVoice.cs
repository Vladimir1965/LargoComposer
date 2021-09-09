// <copyright file="MusicalVoice.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music {
    using Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    /// <summary> A kit voice. </summary>
    public class MusicalVoice : IAbstractVoice
    {
        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="MusicalVoice" /> class. </summary>
        public MusicalVoice() {
            this.Loudness = MusicalLoudness.MeanLoudness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalVoice"/> class.
        /// </summary>
        /// <param name="givenStatus">The given status.</param>
        public MusicalVoice(LineStatus givenStatus) {
            this.Octave = givenStatus.Octave;
            this.Loudness = givenStatus.Loudness;
            this.Instrument = givenStatus.Instrument;
        }
        
        /// <summary> Initializes a new instance of the <see cref="MusicalVoice" /> class. </summary>
        /// <exception cref="ContractException"> Thrown when a method Contract has been broken. </exception>
        /// <param name="markVoice"> The mark line. </param>
        public MusicalVoice(XElement markVoice) : this() {
            Contract.Requires(markVoice != null);
            if (markVoice == null) {
                return;
            }

            var attrib = markVoice.Attribute("Octave");
            if (attrib != null) {
                this.Octave = DataEnums.ReadAttributeMusicalOctave(attrib);
            }

            attrib = markVoice.Attribute("Loudness");
            if (attrib != null) {
                this.Loudness = DataEnums.ReadAttributeMusicalLoudness(attrib);
            }

            attrib = markVoice.Attribute("Channel");
            if (attrib != null) {
                this.Channel = (MidiChannel)XmlSupport.ReadByteAttribute(attrib);
            }

            var xinstrument = markVoice.Element("Instrument");
            if (xinstrument != null) {
                this.Instrument = new MusicalInstrument(xinstrument); //// : null;
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xvoice = new XElement("Voice", null);
                xvoice.Add(new XAttribute("Octave", this.Octave));
                xvoice.Add(new XAttribute("Loudness", this.Loudness));
                xvoice.Add(new XAttribute("Channel", (int)this.Channel));

                var instr = this.Instrument ?? new MusicalInstrument(MidiMelodicInstrument.None);
                xvoice.Add(instr.GetXElement);

                return xvoice;
            }
        }
        #endregion

        #region Properties

        /// <summary>   Gets or sets the zero-based index of the line. </summary>
        /// <value> The line index. </value>
        public IAbstractLine Line { get; set; }

        /// <summary>   Gets or sets the octave. </summary>
        /// <value> The octave. </value>
        public MusicalOctave Octave { get; set; }

        /// <summary>   Gets or sets the instrument. </summary>
        /// <value> The instrument. </value>
        public MusicalInstrument Instrument { get; set; }

        /// <summary> Gets or sets the loudness. </summary>
        /// <value> The loudness. </value>
        public MusicalLoudness Loudness { get; set; }

        /// <summary> Gets or sets the channel. </summary>
        /// <value> The channel. </value>
        public MidiChannel Channel { get; set; }

        #endregion
    }
}
