// <copyright file="MusicXmlMeasure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MusicXml
{
    /// <summary>
    /// MusicXml Measure.
    /// </summary>
    public sealed class MusicXmlMeasure {
        #region Fields
        /// <summary>
        /// Music Xml Header.
        /// </summary>
        [UsedImplicitly]
        private MusicXmlHeader header;

        /// <summary>
        /// Music Xml Reader.
        /// </summary>
        private MusicXmlReader reader;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicXmlMeasure class.
        /// </summary>
        /// <param name="givenHeader">Musical header.</param>
        public MusicXmlMeasure(MusicXmlHeader givenHeader) {
            this.Header = givenHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicXmlMeasure"/> class.
        /// </summary>
        [UsedImplicitly]
        public MusicXmlMeasure() {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Music Xml Header is null.</exception>
        /// <exception cref="System.ArgumentException">Music Xml Header cannot be empty.;value</exception>
        public MusicXmlHeader Header {
            get {
                Contract.Ensures(Contract.Result<MusicXmlHeader>() != null);
                if (this.header == null) {
                    throw new InvalidOperationException("Music Xml Header is null.");
                }

                return this.header;
            }

            set => this.header = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets or sets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        private MusicXmlReader Reader {
            get {
                Contract.Ensures(Contract.Result<MusicXmlReader>() != null);
                if (this.reader == null) {
                    throw new InvalidOperationException("Music Xml Reader is null.");
                }

                return this.reader;
            }

            set => this.reader = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Measure element.
        /// </summary>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="track">Musical track.</param>
        /// <param name="header">The header.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static XElement MeasureElement(int barNumber, MusicalLine track, MusicalHeader header) {
            Contract.Requires(track != null);
            var measure = new XElement(
                                    "measure",
                                    new XAttribute("number", barNumber.ToString(CultureInfo.InvariantCulture)));

            float quotient = 1.0f * header.System.RhythmicOrder / header.Division;
            int beatDivision = header.Division / header.Metric.MetricBeat;
            var attributes = new XElement(
                                    "attributes",
                                    new XElement("divisions", beatDivision.ToString(CultureInfo.InvariantCulture)));

            if (barNumber == 1) {
                var element = KeyElement(); //// track
                attributes.Add(element);

                element = TimeElement(header);
                attributes.Add(element);

                element = ClefElement(track);
                if (!element.IsEmpty) {
                    attributes.Add(element);
                }

                //// element = this.TempoElement();
                //// attributes.Add(element);
            }

            var tones = track.MusicalTonesInBar(barNumber);

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            var firstTone = tones.FirstOrDefault();
            if (firstTone != null && firstTone.BitFrom > 0) {
                    var pause = new MusicalPause(header.System.RhythmicOrder, firstTone.BitFrom, barNumber);
                    var pauseElement = RestElement(pause, beatDivision, quotient);
                    measure.Add(pauseElement);
            }

            IMusicalTone lastTone = null;
            foreach (var tone in tones) {
                int shift = 0;
                if (lastTone != null) {
                    if (tone.BitFrom == lastTone.BitFrom) {
                        shift = -tone.Duration;
                    }
                    else {
                        shift = tone.BitFrom - (lastTone.BitTo + 1);
                    }
                }

                var duration = Math.Abs(shift) / quotient;
                if (shift < 0) {
                    var backwardElement = ShiftElement("backward", shift, quotient);
                    measure.Add(backwardElement);
                }
                else
                if (shift > 0) {
                    var forwardElement = ShiftElement("forward", shift, quotient);
                    measure.Add(forwardElement);
                }

                var mtone = tone as MusicalTone;
                var noteElement = mtone?.Pitch != null ? NoteElement(mtone, beatDivision, quotient)
                                    : RestElement(tone, beatDivision, quotient);

                measure.Add(noteElement);
                lastTone = tone;
            }

            measure.Add(attributes);
            return measure;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Read Musical Bar.
        /// </summary>
        /// <param name="scorePartObject">The score part object.</param>
        /// <param name="musicalPart">Musical Part.</param>
        /// <param name="measure">Musical measure.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="givenReader">The given reader.</param>
        public void ReadMusicalBar(ScorePartObject scorePartObject, MusicalPart musicalPart, XContainer measure, int barNumber, MusicXmlReader givenReader) {
            Contract.Requires(measure != null);
            Contract.Requires(musicalPart != null);
            this.Reader = givenReader;
            var attributes = measure.Element("attributes");
            //// XElement key = attributes.Element("key");
            //// XElement clef = attributes.Element("clef");
            var transpose = attributes?.Element("transpose");
            if (transpose != null) {
                this.Reader.LocalPitchShift = 0;
                //// XElement diatonic = transpose.Element("diatonic");
                //// XElement octaveDouble = transpose.Element("double");
                var octaveChange = transpose.Element("octave-change");
                if (octaveChange != null) {
                    this.Reader.LocalPitchShift += (int)octaveChange * DefaultValue.HarmonicOrder;
                }

                var chromatic = transpose.Element("chromatic");
                if (chromatic != null) {
                    this.Reader.LocalPitchShift += (int)chromatic;
                }
            }

            var actions = measure.Nodes(); //// Elements("note");
            foreach (var action in
                actions.Where(node => node.NodeType == XmlNodeType.Element).OfType<XElement>()) {
                this.AppendMusicalTones(scorePartObject, musicalPart, barNumber, action);
            }
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Shifts the element.
        /// </summary>
        /// <param name="shiftDirection">The shift direction.</param>
        /// <param name="shift">The shift.</param>
        /// <param name="quotient">The quotient.</param>
        /// <returns> Returns value. </returns>
        private static XElement ShiftElement(string shiftDirection, int shift, float quotient) {
            var duration = Math.Abs(shift) / quotient;
            var element = new XElement(
                                shiftDirection,
                                new XElement("duration", duration.ToString(CultureInfo.InvariantCulture)));
            return element;
        }

        /// <summary>
        /// Note element.
        /// </summary>
        /// <param name="mtone">Melodic tone.</param>
        /// <param name="beatDivision">Beat division.</param>
        /// <param name="quotient">The quotient.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static XElement NoteElement(MusicalTone mtone, float beatDivision, float quotient) {
            Contract.Requires(mtone != null);
            Contract.Requires(mtone.Pitch != null);
            var noteType = MusicalProperties.GetNoteType(beatDivision);
            var duration = mtone.Duration / quotient;
            var pitch = new XElement(
                                    "pitch",
                                new XElement("step", MusicalProperties.GetSingleNoteName(mtone.Pitch.Element)),
                                new XElement("alter", MusicalProperties.GetAlterSign(mtone.Pitch.Element).ToString(CultureInfo.InvariantCulture)),
                                new XElement("octave", mtone.Pitch.StandardOctave.ToString(CultureInfo.InvariantCulture)));
            var note = new XElement(
                                "note",
                                pitch,
                                new XElement("duration", duration.ToString(CultureInfo.InvariantCulture)),
                                new XElement("type", noteType));
            return note;
        }

        /// <summary>
        /// Note element.
        /// </summary>
        /// <param name="mtone">Musical Tone.</param>
        /// <param name="beatDivision">Beat division.</param>
        /// <param name="quotient">The quotient.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static XElement RestElement(IMusicalTone mtone, float beatDivision, float quotient) {
            Contract.Requires(mtone != null);
            var noteType = MusicalProperties.GetNoteType(beatDivision);
            var duration = mtone.Duration / quotient;
            var note = new XElement(
                                "note",
                                new XElement("rest", null),
                                new XElement("duration", duration.ToString(CultureInfo.InvariantCulture)),
                                new XElement("type", noteType));
            return note;
        }

        /// <summary>
        /// Read Musical shift.
        /// </summary>
        /// <param name="shift">Musical shift.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="signum">Sign of the shift.</param>
        /// <param name="quotient">Time quotient.</param>
        /// <returns> Returns value. </returns> 
        private static MusicalShift ReadMusicalShift(XContainer shift, int barNumber, short signum, double quotient) {
            Contract.Requires(shift != null);
            var durationElement = shift.Element("duration");
            if (durationElement == null) {
                return null;
            }

            var musicalShift = new MusicalShift { BarNumber = barNumber };

            var duration = (int)((int)durationElement * quotient);
            musicalShift.Value = (short)(signum * duration);

            var staff = shift.Element("staff");
            if (staff != null) {
                musicalShift.Staff = (byte)(int)staff;
            }

            var voice = shift.Element("voice");
            if (voice != null) {
                musicalShift.Voice = (byte)(int)voice;
            }

            return musicalShift;
        }

        /// <summary>
        /// Key element.
        /// </summary>
        /// <returns> Returns value. </returns>
        private static XElement KeyElement() { //// MusicalLine track
            const int fifthNumber = 0;
            var key = new XElement(
                            "key",
                            new XElement("fifths", fifthNumber.ToString(CultureInfo.InvariantCulture)),
                            new XElement("mode", "major"));
            return key;
        }

        /// <summary>
        /// Time element.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static XElement TimeElement(MusicalHeader header) {
            var key = new XElement(
                            "time",
                            new XElement("beats", header.Metric.MetricBeat.ToString(CultureInfo.InvariantCulture)),
                            new XElement("beat-type", header.Metric.MetricGround.ToString(CultureInfo.InvariantCulture)));
            return key;
        }

        /// <summary>
        /// Clef element.
        /// </summary>
        /// <param name="track">Musical track.</param>
        /// <returns> Returns value. </returns>
        private static XElement ClefElement(MusicalLine track) {
            Contract.Requires(track != null);
            var clef = new XElement("clef");
            var bandType = MusicalProperties.BandTypeFromOctave(track.FirstStatus.Octave); //// (MusicalBand)track.BandType
            switch (bandType) {
                case MusicalBand.HighBeat: {
                        clef.Add(new XElement("sign", "G"));
                        clef.Add(new XElement("line", "2"));
                    }

                    break;
                case MusicalBand.MiddleBeat: {
                        clef.Add(new XElement("sign", "G"));
                        clef.Add(new XElement("line", "2"));
                    }

                    break;
                case MusicalBand.BassTones: {
                        clef.Add(new XElement("sign", "F"));
                        clef.Add(new XElement("line", "4"));
                    }

                    break;
                case MusicalBand.Any:
                    break;
                case MusicalBand.MiddleTones:
                    break;
                case MusicalBand.HighTones:
                    break;
                case MusicalBand.BassBeat:
                    break;
                //// resharper default: break;
            }

            return clef;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Appends the musical tones.
        /// </summary>
        /// <param name="scorePartObject">The score part object.</param>
        /// <param name="musicalPart">The musical part.</param>
        /// <param name="barNumber">The bar number.</param>
        /// <param name="action">The action.</param>
        private void AppendMusicalTones(ScorePartObject scorePartObject, MusicalPart musicalPart, int barNumber, XElement action) {
            Contract.Requires(musicalPart != null);
            Contract.Requires(action != null);
            double quotient = 1.0f * this.Reader.CommonDivision / this.Reader.LocalDivision; //// this.Reader.MusicalBlock.RhythmicOrder; //// ;
            quotient = quotient * this.Reader.Block.Header.System.RhythmicOrder / this.Reader.CommonDivision;
            if (action.Name == "backward") {
                var shift = ReadMusicalShift(action, barNumber, -1, quotient);
                if (shift != null) {
                    musicalPart.AddMusicalObject(shift);
                }
            }

            if (action.Name == "forward") {
                var shift = ReadMusicalShift(action, barNumber, +1, quotient);
                if (shift != null) {
                    musicalPart.AddMusicalObject(shift);
                }
            }

            // ReSharper disable once InvertIf
            if (action.Name == "note") {
                var tone = this.ReadMusicalTone(action, barNumber, quotient);
                var chord = action.Element("chord");
                if (chord != null) {
                    if (tone != null) {
                        var shift = new MusicalShift {
                            BarNumber = barNumber,
                            Value = (short)-tone.Duration,
                            Voice = tone.Voice,
                            Staff = tone.Staff
                        };
                        musicalPart.AddMusicalObject(shift);
                    }
                }

                if (tone == null) {
                    return;
                }

                tone.InstrumentNumber = scorePartObject.MidiProgram;
                //// tone.Channel = (MidiChannel)scorePartObject.MidiChannel;
                musicalPart.AddMusicalObject(tone);
            }
        }

        /// <summary>
        /// Read Musical Tone.
        /// </summary>
        /// <param name="note">Musical note.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="quotient">Time quotient.</param>
        /// <returns> Returns value. </returns> 
        private IMusicalTone ReadMusicalTone(XContainer note, int barNumber, double quotient) {
            Contract.Requires(note != null);
            IMusicalTone tone;
            var pitch = note.Element("pitch");
            //// XElement rest = note.Element("rest");
            var durationElement = note.Element("duration");
            if (durationElement == null) {
                return null;
            }

            var duration = (int)((int)durationElement * quotient); //// in ticks
            //// BitRange range = new BitRange(this.Reader.MusicalBlock.RhythmicOrder, bitFrom, duration);
            if (pitch != null) {
                var step = (string)pitch.Element("step");
                var alterString = (string)pitch.Element("alter");
                var alter = (short)(alterString != null ? short.Parse(alterString, CultureInfo.InvariantCulture) : 0);

                var octave = (string)pitch.Element("octave");
                //// byte.Parse(step)
                var noteNumber = MusicalProperties.GetNoteNumber(step, alter);
                var element = noteNumber >= 0 ? (byte)(noteNumber % this.Reader.Block.Header.System.HarmonicOrder)
                    : (byte)(noteNumber + this.Reader.Block.Header.System.HarmonicOrder);

                if (element >= DefaultValue.HarmonicOrder) {
                    throw new DataException("Invalid Musical Element");
                }

                var hs = HarmonicSystem.GetHarmonicSystem(this.Reader.Block.Header.System.HarmonicOrder);
                var musicalPitch = new MusicalPitch(hs, (short)(short.Parse(octave, CultureInfo.InvariantCulture) + 1), element);
                if (this.Reader.LocalPitchShift != 0) {
                    musicalPitch.SetAltitude(musicalPitch.SystemAltitude + this.Reader.LocalPitchShift);
                }

                var mtone = new MusicalTone(musicalPitch, this.Reader.Block.Header.System.RhythmicOrder, (byte)duration, MusicalLoudness.MeanLoudness, barNumber);
                tone = mtone;
                
                var staff = note.Element("staff");
                if (staff != null) {
                    mtone.Staff = (byte)(int)staff;
                }

                var voice = note.Element("voice");
                if (voice != null) {
                    mtone.Voice = (byte)(int)voice;
                }
            }
            else {
                tone = new MusicalPause(this.Reader.Block.Header.System.RhythmicOrder, (byte)duration, barNumber);
                //// tone = new MusicalStrike(MusicalToneType.Empty, this.Reader.Block.Header.System.RhythmicOrder, (byte)duration, MusicalLoudness.None, barNumber);
            }

            return tone;
        }
        #endregion
    }
}
