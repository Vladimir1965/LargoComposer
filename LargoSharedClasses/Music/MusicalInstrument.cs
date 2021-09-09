// <copyright file="MusicalInstrument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Structure to represent one instrument on channel.

namespace LargoSharedClasses.Music
{
    using Abstract;
    using Interfaces;
    using JetBrains.Annotations;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Rhythm;
    using Localization;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// General Instrument.
    /// </summary>
    public sealed class MusicalInstrument
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        /// <param name="givenInstrument">The given instrument.</param>
        public MusicalInstrument(MidiMelodicInstrument givenInstrument) {
            this.Genus = InstrumentGenus.Melodical;
            this.Number = (byte)givenInstrument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        /// <param name="givenInstrument">The given instrument.</param>
        public MusicalInstrument(MidiRhythmicInstrument givenInstrument) {
            this.Genus = InstrumentGenus.Rhythmical;
            this.Number = (byte)givenInstrument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        /// <param name="givenInstrumentNumber">The given instrument number.</param>
        /// <param name="givenLineType">Type of the given line.</param>
        public MusicalInstrument(byte givenInstrumentNumber, MusicalLineType givenLineType) {
            this.Number = givenInstrumentNumber;

            if (givenLineType == MusicalLineType.Melodic) {
                this.Genus = InstrumentGenus.Melodical;
            }

            if (givenLineType == MusicalLineType.Rhythmic) {
                this.Genus = InstrumentGenus.Rhythmical;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        /// <param name="tones">The tones.</param>
        public MusicalInstrument(IEnumerable<IMusicalTone> tones) {
            Contract.Requires(tones != null);

            this.TakeInstrumentFrom(tones);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        /// <param name="tones">The tones.</param>
        public MusicalInstrument(IEnumerable<MusicalTone> tones) {
            Contract.Requires(tones != null);

            this.TakeInstrumentFrom(tones);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument" /> class.
        /// </summary>
        /// <param name="markInstrument">The mark instrument.</param>
        public MusicalInstrument(XElement markInstrument) {
            this.Genus = DataEnums.ReadAttributeInstrumentGenus(markInstrument.Attribute("Genus"));
            this.Number = XmlSupport.ReadByteAttribute(markInstrument.Attribute("Number"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalInstrument"/> class.
        /// </summary>
        public MusicalInstrument() {
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
                XElement mainElement = null;
                if (this.Genus == InstrumentGenus.Melodical) {
                    mainElement = new XElement(
                        "Instrument",
                        new XAttribute("Genus", this.Genus),
                        new XAttribute("Name", this.MelodicInstrument),
                        new XAttribute("Number", (byte)this.MelodicInstrument));
                }
                else
                if (this.Genus == InstrumentGenus.Rhythmical) {
                    mainElement = new XElement(
                        "Instrument",
                        new XAttribute("Genus", this.Genus),
                        new XAttribute("Name", this.RhythmicInstrument),
                        new XAttribute("Number", (byte)this.RhythmicInstrument));
                }

                return mainElement;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the instrument genus.
        /// </summary>
        /// <value>
        /// The genus.
        /// </value>
        public InstrumentGenus Genus { get; set; }

        /// <summary>
        /// Gets the melodic instrument.
        /// </summary>
        /// <value>
        /// The melodic instrument.
        /// </value>
        public MidiMelodicInstrument MelodicInstrument {
            get {
                if (this.Genus == InstrumentGenus.Melodical) {
                    return (MidiMelodicInstrument)this.Number;
                }
                else {
                    return MidiMelodicInstrument.None;
                }
            }
        }

        /// <summary>
        /// Gets the rhythmic instrument.
        /// </summary>
        /// <value>
        /// The rhythmic instrument.
        /// </value>
        public MidiRhythmicInstrument RhythmicInstrument {
            get {
                if (this.Genus == InstrumentGenus.Rhythmical) {
                    return (MidiRhythmicInstrument)this.Number;
                }
                else {
                    return MidiRhythmicInstrument.None;
                }
            }
        }

        /// <summary>
        /// Gets the melodic section.
        /// </summary>
        /// <value>
        /// The melodic section.
        /// </value>
        [UsedImplicitly]
        public MidiMelodicSection MelodicSection {
            get {
                if (this.Genus == InstrumentGenus.Melodical) {
                    return (MidiMelodicSection)this.Section;
                }
                else {
                    return MidiMelodicSection.None;
                }
            }
        }

        /// <summary> Gets or sets the file name. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public byte Number { get; set; }

        /// <summary> Gets or sets the file name. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public byte Section { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => this.Number == 127;

        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <value> General musical property.</value>
        public override string ToString() {
            if (this.Genus == InstrumentGenus.Melodical) {
                return LocalizedMusic.String("MelInstr" + this.Number.ToString(CultureInfo.CurrentCulture));
            }
            else {
                return LocalizedMusic.String("RhyInstr" + this.Number.ToString(CultureInfo.CurrentCulture));
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Takes the instrument and channel from.
        /// </summary>
        /// <param name="tones">The tones.</param>
        private void TakeInstrumentFrom(IEnumerable<IMusicalTone> tones) {
            Contract.Requires(tones != null);
            this.Genus = InstrumentGenus.None;

            var melTone = (from t in tones where !t.IsPause select t).FirstOrDefault();
            if (melTone == null) {
                return;
            }

            this.Genus = InstrumentGenus.Rhythmical;
            this.Number = melTone.InstrumentNumber;
        }

        /// <summary>
        /// Takes the instrument and channel from.
        /// </summary>
        /// <param name="tones">The tones.</param>
        private void TakeInstrumentFrom(IEnumerable<MusicalTone> tones) {
            Contract.Requires(tones != null);
            this.Genus = InstrumentGenus.None;

            var melTone = (from t in tones where !t.IsPause select t).FirstOrDefault();
            if (melTone == null) {
                return;
            }

            this.Genus = InstrumentGenus.Melodical;
            this.Number = melTone.InstrumentNumber;
        }
        #endregion
    }
}
