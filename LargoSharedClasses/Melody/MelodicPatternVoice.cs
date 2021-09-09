// <copyright file="MelodicPatternVoice.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Melody
{
    /// <summary>
    /// Rhythmic Pattern Voice.
    /// </summary>
    public class MelodicPatternVoice
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicPatternVoice"/> class.
        /// </summary>
        public MelodicPatternVoice() {
            this.Name = string.Empty;
            this.Loudness = MusicalLoudness.MeanLoudness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicPatternVoice" /> class.
        /// </summary>
        /// <param name="givenPattern">The given pattern.</param>
        /// <param name="givenElement">The given element.</param>
        public MelodicPatternVoice(MelodicPattern givenPattern, MusicalElement givenElement)
            : this() {
            this.Pattern = givenPattern;
            this.Number = givenElement.MusicalLine.LineNumber;
            this.Element = givenElement;
            this.Name = givenElement.Status.Instrument.ToString();
            this.Tones = this.Element.Tones.Clone(true);
            this.Level = (byte)this.Tones.Count;

            this.Instrument = givenElement.Status.Instrument;
            var rstruct = givenElement.Status.RhythmicStructure;
            //// This is an attempt to simplify structures written to xml files...
            var rd = givenPattern.RhythmicDivisor;
            if (rd > 1) {
                var rorder = (byte)(rstruct.Order / rd);
                var gs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rorder);
                ////var rs = rstruct.ConvertToSystem(gs);

                var rs = new RhythmicStructure { GSystem = gs };
                Collection<short> ec = new Collection<short>();
                for (byte i = 0; i < gs.Order; i++) {
                    var ev = rstruct.ElementList[i * rd];
                    ec.Add(ev);
                }

                rs.SetElementList(ec);

                this.RhythmicStructure = rs;
            }
            else {
                this.RhythmicStructure = rstruct;
            }

            var mstruct = givenElement.Status.MelodicStructure;
            this.MelodicStructure = mstruct;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicPatternVoice" /> class.
        /// </summary>
        /// <param name="givenTickOrder">The given tick order.</param>
        /// <param name="givenRhythmicOrder">The given rhythmic order.</param>
        /// <param name="xvoice">The voice element.</param>
        /// <exception cref="System.InvalidOperationException">Invalid pattern length.</exception>
        public MelodicPatternVoice(byte givenTickOrder, byte givenRhythmicOrder, XElement xvoice) {
            this.Number = XmlSupport.ReadByteAttribute(xvoice.Attribute("Number"));
            this.Name = XmlSupport.ReadStringAttribute(xvoice.Attribute("Name"));
            byte number = XmlSupport.ReadByteAttribute(xvoice.Attribute("Instrument"));
            this.Instrument = new MusicalInstrument((MidiMelodicInstrument)number);
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

            var tickSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, givenTickOrder);
            this.RhythmicShape = new RhythmicShape(tickSystem, bitArray);
            this.RhythmicStructure = new RhythmicStructure(tickSystem.Order, this.RhythmicShape);
            this.Loudness = MusicalLoudness.MeanLoudness;
            var xtones = xvoice.Element("Tones");
            if (xtones != null) {
                this.Tones = new ToneCollection(xtones, givenRhythmicOrder);
                var mtones = new MusicalToneCollection(this.Tones, null, true);
                this.Level = (byte)mtones.Count;
                this.MelodicStructure = mtones.DetermineMelodicStructure(null, null); //// bar.Status.HarmonicBar
                this.Octave = mtones.MeanOctave;
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
        public MelodicPattern Pattern { get; set; }

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
        /// Gets or sets the octave.
        /// </summary>
        /// <value>
        /// The octave.
        /// </value>
        public MusicalOctave Octave { get; set; }

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
        /// Gets or sets the melodic structure.
        /// </summary>
        /// <value>
        /// The melodic structure.
        /// </value>
        public MelodicStructure MelodicStructure { get; set; }

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

                return this.Tones.MelodicPatternIdentifier;
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
                    "MelodicPatternVoice {0} Level {1} {2} {3} {4} {5}", this.Number, this.Level, this.Instrument, this.Loudness, this.Octave, this.Name);

            return s.ToString();
        }
        #endregion
    }
}
