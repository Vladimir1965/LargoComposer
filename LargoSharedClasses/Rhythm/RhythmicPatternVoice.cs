// <copyright file="RhythmicPatternVoice.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections;
using System.Text;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Rhythm
{
    /// <summary>
    /// Rhythmic Pattern Voice.
    /// </summary>
    public class RhythmicPatternVoice
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPatternVoice"/> class.
        /// </summary>
        public RhythmicPatternVoice() {
            this.Name = string.Empty;
            this.Loudness = MusicalLoudness.MeanLoudness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPatternVoice" /> class.
        /// </summary>
        /// <param name="givenPattern">The given pattern.</param>
        /// <param name="givenElement">The given element.</param>
        public RhythmicPatternVoice(RhythmicPattern givenPattern, MusicalElement givenElement)
            : this() {
            this.Pattern = givenPattern;
            this.Number = givenElement.MusicalLine.LineNumber;
            this.Element = givenElement;
            this.Name = givenElement.Status.Instrument.ToString();
            this.Tones = this.Element.Tones.Clone(true);
            this.Level = (byte)this.Tones.Count;
            this.Instrument = givenElement.Status.Instrument;
            if (this.Instrument.IsEmpty) {
                this.Instrument = new MusicalInstrument(MidiRhythmicInstrument.LowMidTom);
            }

            this.RhythmicStructure = givenElement.Status.RhythmicStructure;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicPatternVoice" /> class.
        /// </summary>
        /// <param name="givenTickOrder">The given tick order.</param>
        /// <param name="givenRhythmicOrder">The given rhythmic order.</param>
        /// <param name="xvoice">The voice element.</param>
        /// <exception cref="System.InvalidOperationException">Invalid pattern length.</exception>
        public RhythmicPatternVoice(byte givenTickOrder, byte givenRhythmicOrder, XElement xvoice) {
            this.Number = XmlSupport.ReadByteAttribute(xvoice.Attribute("Number"));
            this.Name = XmlSupport.ReadStringAttribute(xvoice.Attribute("Name"));
            byte instrumentNumber = XmlSupport.ReadByteAttribute(xvoice.Attribute("Instrument"));
            this.Instrument = new MusicalInstrument((MidiRhythmicInstrument)instrumentNumber);
            var bitString = (string)xvoice.Element("Ticks");
            var ok = bitString.Length >= givenTickOrder;
            if (!ok) {
                givenTickOrder = (byte)bitString.Length;
                //// throw new InvalidOperationException("Invalid pattern length.");
            }

            var bitArray = new BitArray(givenTickOrder);
            for (int i = 0; i < givenTickOrder; i++) {
                bitArray[i] = bitString[i] == '1';
            }

            var system = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, givenTickOrder);
            this.RhythmicShape = new RhythmicShape(system, bitArray);
            this.RhythmicStructure = new RhythmicStructure(system.Order, this.RhythmicShape);
            this.Loudness = MusicalLoudness.MeanLoudness;
            var xtones = xvoice.Element("Tones");
            if (xtones != null) {
                this.Tones = new ToneCollection(xvoice.Element("Tones"), givenRhythmicOrder);
                this.Level = (byte)this.Tones.Count;
            }
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
                var topElement = new XElement("Voice");
                topElement.Add(new XAttribute("Number", this.Number));
                topElement.Add(new XAttribute("Name", this.Name));
                topElement.Add(new XAttribute("Instrument", this.Instrument.Number));

                if (this.RhythmicStructure == null) {
                    return topElement;
                }

                var sticks = this.RhythmicStructure.BinaryStructure(false).ElementString();
                XElement xticks = new XElement("Ticks", sticks);
                topElement.Add(xticks);

                if (this.Tones != null) {
                    XElement xtones = this.Tones.GetXElement;
                    topElement.Add(xtones);
                }

                return topElement;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public RhythmicPattern Pattern { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        public MusicalInstrument Instrument { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic shape.
        /// </summary>
        /// <value>
        /// The rhythmic shape.
        /// </value>
        public RhythmicShape RhythmicShape { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic structure.
        /// </summary>
        /// <value>
        /// The rhythmic structure.
        /// </value>
        public RhythmicStructure RhythmicStructure { get; set; }

        /// <summary>
        /// Gets or sets the loudness.
        /// </summary>
        /// <value>
        /// The loudness.
        /// </value>
        public MusicalLoudness Loudness { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Identifier {
            get {
                if (this.Tones == null) {
                    return string.Empty;
                }

                return this.Tones.RhythmicPatternIdentifier;
            }
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public MusicalElement Element { get; set; }

        /// <summary>
        /// Gets or sets the tones.
        /// </summary>
        /// <value>
        /// The tones.
        /// </value>
        public ToneCollection Tones { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => !this.Tones.HasAnySoundingTone;

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                    "RhythmicPatternVoice {0} Level {1} {2} {3} {4}", this.Number, this.Level, this.Instrument, this.Loudness, this.Name);

            return s.ToString();
        }
        #endregion
    }
}
