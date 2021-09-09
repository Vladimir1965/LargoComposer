// <copyright file="OrchestraVoice.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;

namespace LargoSharedClasses.Orchestra
{
    /// <summary>  Musical material. </summary>
    /// <remarks> Musical class. </remarks>
    ////    [Serializable]
    [ContractVerification(false)]
    public sealed class OrchestraVoice
    { //// partial
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the OrchestraVoice class.
        /// </summary>
        /// <param name="givenOctave">The given octave.</param>
        /// <param name="givenInstrument">The given instrument.</param>
        public OrchestraVoice(MusicalOctave givenOctave, MusicalInstrument givenInstrument) //// int scoreLineId
            : this() {
            this.Octave = givenOctave;
            this.BandType = MusicalProperties.BandTypeFromOctave(givenOctave);
            this.Instrument = givenInstrument;
        }

        /// <summary> Initializes a new instance of the <see cref="OrchestraVoice"/> class. </summary>
        /// <param name="givenInstrument"> The given instrument. </param>
        public OrchestraVoice(MusicalInstrument givenInstrument) //// int scoreLineId
            : this() {
            this.Octave = MusicalOctave.None;
            this.BandType = MusicalBand.MiddleBeat;
            this.Instrument = givenInstrument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestraVoice"/> class.
        /// </summary>
        public OrchestraVoice() {
            this.Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestraVoice"/> class.
        /// </summary>
        /// <param name="xtrack">The given line.</param>
        public OrchestraVoice(XElement xtrack) {
            Contract.Requires(xtrack != null);
            if (xtrack == null) {
                return;
            }

            this.Octave = DataEnums.ReadAttributeMusicalOctave(xtrack.Attribute("Octave"));
            this.BandType = DataEnums.ReadAttributeMusicalBandType(xtrack.Attribute("Band"));
            var xInstrument = xtrack.Element("Instrument");
            this.Instrument = new MusicalInstrument(xInstrument);

            if (this.Instrument.Genus == InstrumentGenus.Melodical) {
                var melodicGroup = PortInstruments.GetGroupOfMelodicInstrument(this.Instrument.Number);
                this.InstrumentGroupString = melodicGroup.ToString();
            }
            else {
                var rhythmicGroup = PortInstruments.GetGroupOfRhythmicInstrument(this.Instrument.Number);
                this.InstrumentGroupString = rhythmicGroup.ToString();
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
                XElement xtrack = new XElement(
                            "Line",
                            new XAttribute("Band", this.BandType),
                            new XAttribute("Octave", this.Octave));
                xtrack.Add(this.Instrument.GetXElement);

                return xtrack;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        [UsedImplicitly]
        public MusicalInstrument Instrument { get; set; }

        /// <summary> Gets or sets the instrument group string. </summary>
        /// <value> The instrument group string. </value>
        public string InstrumentGroupString { get; set; }

        /// <summary>
        /// Gets the instrument number.
        /// </summary>
        /// <value>
        /// The instrument number.
        /// </value>
        public byte InstrumentNumber => this.Instrument?.Number ?? (byte)MidiMelodicInstrument.AcousticGrandPiano;

        /// <summary>
        /// Gets or sets the type of the band.
        /// </summary>
        /// <value>
        /// The type of the band.
        /// </value>
        public MusicalBand BandType { get; set; }

        /// <summary>
        /// Gets or sets the octave.
        /// </summary>
        /// <value>
        /// The octave.
        /// </value>
        public MusicalOctave Octave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
        /// </value>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Gets Unique Identifier.
        /// </summary>
        /// <value> Property description. </value> 
        public string UniqueIdentifier => $"{this.BandType}{this.Instrument}";

        #endregion

        #region Export

        /// <summary>
        /// Gets OctaveString.
        /// </summary>
        /// <value> General musical property.</value> 
        [UsedImplicitly]
        public string OctaveString => LocalizedMusic.String("Octave" + ((int)this.Octave).ToString(CultureInfo.CurrentCulture));

        /// <summary>
        /// Gets BandTypeString.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string BandTypeString => LocalizedMusic.String("Band" + ((int)this.BandType).ToString(CultureInfo.CurrentCulture));

        /// <summary>
        /// Gets the instrument string.
        /// </summary>
        /// <value>
        /// The instrument string.
        /// </value>
        public string InstrumentString => this.Instrument.ToString();

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.OctaveString + "-");
            s.Append(this.BandTypeString + ": ");
            s.AppendLine(this.Instrument.ToString());
            //// s.Append("\t" + this.RhythmicOrder.ToString(CultureInfo.CurrentCulture));
            return s.ToString();
        }
        #endregion
    }
}
