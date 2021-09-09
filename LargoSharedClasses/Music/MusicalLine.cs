// <copyright file="MusicalLine.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.MidiFile;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Track.
    /// </summary>
    [Serializable]
    public sealed class MusicalLine : IAbstractLine
    {
        #region Fields
        /// <summary> Complete list of all tones in musical part. </summary>
        private ToneCollection tones;

        /// <summary>
        /// Musical Block.
        /// </summary>
        [NonSerialized]
        private MusicalStrip strip;

        /// <summary> Gets or sets corresponding MIDI track. </summary>
        /// <value> Property description. </value>
        private MidiEventCollection midiEventColl;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalLine class.
        /// </summary>
        public MusicalLine() {
            this.FirstStatus = new LineStatus {
                LocalPurpose = LinePurpose.None,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.None) //// FixedInstrument
            };

            this.MakeInnerObjects();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalLine" /> class.
        /// </summary>
        /// <param name="givenStatus">The given status.</param>
        public MusicalLine(LineStatus givenStatus) {
            this.Reset();
            this.FirstStatus = givenStatus ?? new LineStatus();
            this.MakeInnerObjects();
        }

        /// <summary>
        /// Initializes a new instance of the MusicalLine class.
        /// </summary>
        /// <param name="givenMidiTrack">The given midi track.</param>
        /// <param name="givenStrip">The given strip.</param>
        public MusicalLine(IMidiTrack givenMidiTrack, MusicalStrip givenStrip) //// 2019/01 MusicalSection givenArea 
        {
            Contract.Requires(givenMidiTrack != null);

            this.Name = givenMidiTrack.Name;
            this.TrackNumber = (byte)givenMidiTrack.TrackNumber;
            this.LineIdent = Guid.NewGuid();
            this.Strip = givenStrip;
            this.StatusList = new List<LineStatus>();
            this.Purpose = LinePurpose.Fixed;

            MusicalLineType lineType = MusicalLineType.None;
            MusicalInstrument instrument = null;
            if (givenMidiTrack.IsMelodic) {
                lineType = MusicalLineType.Melodic;
                instrument = new MusicalInstrument(givenMidiTrack.FirstMelodicInstrumentInEvents);
            }
            else {
                if (givenMidiTrack.IsRhythmical) {
                    lineType = MusicalLineType.Rhythmic;
                    instrument = new MusicalInstrument(givenMidiTrack.FirstRhythmicInstrumentInEvents);
                }
            }

            this.FirstStatus = new LineStatus {
                LineType = lineType,
                Instrument = instrument,
                Staff = givenMidiTrack.Staff,
                Voice = givenMidiTrack.Voice,
                LocalPurpose = LinePurpose.Fixed //// 2019/01 LinePurpose.Fixed
            };
            this.LineType = lineType;

            this.MainVoice = new MusicalVoice {
                Octave = this.FirstStatus.Octave,
                Instrument = this.FirstStatus.Instrument,
                Loudness = this.FirstStatus.Loudness,
                Line = this,
                Channel = givenMidiTrack.Channel
            };
            this.Voices = new List<IAbstractVoice> { this.MainVoice };

            var midiTones = new MidiTones(givenMidiTrack); //// 2019/01 givenArea 
            if (midiTones.List.Count <= 0) {
                return;
            }

            this.AppendMidiTrackTones(midiTones); //// midiTones
            //// this.Purpose = LinePurpose.Fixed; 2020/10 se above 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalLine" /> class.
        /// </summary>
        /// <param name="markTrack">The mark track.</param>
        /// <param name="givenHeader">The given header.</param>
        public MusicalLine(XElement markTrack, MusicalHeader givenHeader) {
            Contract.Requires(markTrack != null);
            if (markTrack == null) {
                return;
            }

            byte rhythmicOrder = givenHeader.System.RhythmicOrder;
            this.LineType = DataEnums.ReadAttributeMusicalLineType(markTrack.Attribute("LineType"));
            this.Purpose = DataEnums.ReadAttributeLinePurpose(markTrack.Attribute("Purpose"));
   
            this.LineIndex = XmlSupport.ReadByteAttribute(markTrack.Attribute("LineIndex"));

            this.StatusList = new List<LineStatus>();
            this.FirstStatus = new LineStatus {
                LocalPurpose = DataEnums.ReadAttributeLinePurpose(markTrack.Attribute("Purpose"))
            };
            var xstatusList = markTrack.Element("StatusList");
            if (xstatusList != null) {
                var xstatusListElements = xstatusList.Elements();
                var xtrackStatuses = xstatusListElements.ToList();
                var xtrackStatus1 = xtrackStatuses.FirstOrDefault();
                if (xtrackStatus1 != null) {
                    this.FirstStatus.SetXElement(xtrackStatus1, givenHeader);
                }

                this.CurrentStatus = this.FirstStatus.Clone() as LineStatus;
                foreach (var xtrackStatus in xtrackStatuses) {
                    //// var writtenStatus = new TrackStatus(xtrackStatus, givenHeader);
                    if (this.CurrentStatus == null) {
                        continue;
                    }

                    this.CurrentStatus.SetXElement(xtrackStatus, givenHeader);
                    var status = this.CurrentStatus.Clone() as LineStatus;
                    if (status == null) {
                        continue;
                    }

                    status.BarNumber = XmlSupport.ReadIntegerAttribute(xtrackStatus.Attribute("Bar"));
                    this.StatusList.Add(status);
                }

                var purpose = (from ts in this.StatusList where ts.LocalPurpose != LinePurpose.None select ts.LocalPurpose)
                    .FirstOrDefault();
                this.Purpose = purpose;
            }

            this.LineIdent = Guid.NewGuid();

            //// Voices
            this.Voices = new List<IAbstractVoice>();
            var xvoices = markTrack.Element("Voices");
            if (xvoices != null) {
                var xvoicesElements = xvoices.Elements();
                foreach (var xvoice in xvoicesElements) {
                    var voice = new MusicalVoice(xvoice) { Line = this };
                    this.Voices.Add(voice);
                }
            }

            if (this.Voices != null && this.Voices.Count > 0) {
                this.MainVoice = this.Voices.First();
            }
            else {
                this.MainVoice = new MusicalVoice {
                    Octave = this.FirstStatus.Octave,
                    Instrument = this.FirstStatus.Instrument,
                    Loudness = this.FirstStatus.Loudness,
                    Line = this
                };

                this.Voices = new List<IAbstractVoice> { this.MainVoice };
            }

            //// Tones
            var attribute = markTrack.Attribute("Channel");
            if (attribute != null) {
                this.MainVoice.Channel = (MidiChannel)XmlSupport.ReadByteAttribute(attribute);
            }

            var xtones = markTrack.Element("Tones");
            if (xtones != null) {
                var mts = new ToneCollection(xtones, rhythmicOrder);
                this.SetTones(mts);
            }
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xtrack = new XElement("Line", null);
                xtrack.Add(new XAttribute("LineIndex", this.LineIndex));
                xtrack.Add(new XAttribute("Channel", (int)this.MainVoice.Channel));
                xtrack.Add(new XAttribute("Purpose", this.Purpose));

                //// Status
                var statusList = this.StatusList;
                if (statusList != null) {
                    XElement xstatusList = new XElement("StatusList");

                    LineStatus lastStatus = null;
                    foreach (var ts in statusList) {
                        var xstatus = lastStatus == null ? ts.GetXElement : ts.GetChangeXElement(lastStatus);
                        xstatusList.Add(xstatus);
                        lastStatus = ts;
                    }

                    xtrack.Add(new XAttribute("LineType", this.LineType));  //// 2020/10
                    xtrack.Add(xstatusList);
                }

                //// Voices
                var voices = this.Voices;
                if (voices != null) {
                    XElement xvoices = new XElement("Voices");

                    foreach (var tv in voices) {
                        var xvoice = tv.GetXElement;
                        xvoices.Add(xvoice);
                    }

                    xtrack.Add(xvoices);
                }
                
                //// Tones
                XElement xrealTones = new XElement("Tones");
                var mtones = this.Tones;
                if (mtones != null) {
                    byte lastInstrument = (byte)MidiMelodicInstrument.None;
                    foreach (IMusicalTone mtone in mtones.Where(mtone => mtone != null)) {
                        var xelement = mtone.GetXElement;
                        if (mtone.InstrumentNumber != lastInstrument) { //// 2019/02 rt is MusicalStrike mtone &&
                            xelement.Add(new XAttribute("Instrument", mtone.InstrumentNumber));
                            lastInstrument = mtone.InstrumentNumber;
                        }

                        xrealTones.Add(xelement);
                    }
                }

                xtrack.Add(xrealTones);
                return xtrack;
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>
        /// The type of the line.
        /// </value>
        public MusicalLineType LineType {
            get {
                if (this.StatusList == null || this.FirstStatus == null) {
                    return MusicalLineType.None;
                }

                return this.FirstStatus.LineType;
            }

            set {
                if (this.StatusList != null && this.FirstStatus != null) {
                    this.FirstStatus.LineType = value;
                }
            }
        }        

        /// <summary> Gets or sets purpose of the track. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public LinePurpose Purpose { get; set; }
        
        /// <summary>
        /// Gets or sets the line identifier.
        /// </summary>
        /// <value>
        /// The line identifier.
        /// </value>
        public Guid LineIdent { get; set; }

        /// <summary> Gets or sets line index - i.e. an unique mark of the line in the musical model. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public byte LineNumber => (byte)(this.LineIndex + 1);

        /// <summary>
        /// Gets or sets the current instrument.
        /// </summary>
        /// <value>
        /// The current instrument.
        /// </value>
        public byte CurrentInstrument { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool HasContent => this.Purpose != LinePurpose.None && this.Purpose != LinePurpose.Mute;

        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public LineStatus FirstStatus {
            get {
                Contract.Ensures(Contract.Result<LineStatus>() != null);
                var firstStatus = this.StatusList.FirstOrDefault();
                //// 2020/10 if (firstStatus == null) {  throw new InvalidOperationException("First status is null."); }  

                return firstStatus;
            }

            set =>
                this.StatusList = new List<LineStatus>
            {
                value
            };
        }

        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public LineStatus CurrentStatus { get; set; }

        /// <summary>
        /// Gets or sets the status list.
        /// </summary>
        /// <value>
        /// The status list.
        /// </value>
        public List<LineStatus> StatusList { get; set; }

        /// <summary>
        /// Gets or sets line engine MusicalTrackEngine. 
        /// </summary>
        /// <value> Property description. </value>
        public object TrackEngine { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The line name.
        /// </value>
        public string Name { get; set; }

        /// <summary> Gets or sets track number - original midi-track number. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public byte TrackNumber { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets corresponding MIDI track.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MidiEventCollection MidiEventCollection {
            get {
                if (this.midiEventColl == null || this.midiEventColl.Channel != this.MainVoice.Channel) {
                    this.midiEventColl = new MidiEventCollection(this.MainVoice.Channel);
                }

                return this.midiEventColl;
            }
        }

        /// <summary> Gets a value indicating whether of empty musical part. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public bool IsEmpty {
            get {
                if (this.Tones == null || this.Tones.Count == 0) {
                    return true;
                }

                var realTones = from t in this.Tones where t.ToneType != MusicalToneType.Empty && !t.IsEmpty select 1;
                return !realTones.Any();
            }
        }

        /// <summary>
        /// Gets or sets musical block.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalStrip Strip {
            get {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.strip == null) {
                    throw new InvalidOperationException("Musical strip is null.");
                }

                return this.strip;
            }

            set => this.strip = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets complete list of all tones in musical part.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [XmlIgnore]
        public ToneCollection Tones {
            get {
                Contract.Ensures(Contract.Result<ToneCollection>() != null);
                return this.tones ?? (this.tones = new ToneCollection());
            }
        }

        /// <summary>   Gets or sets the system length. </summary>
        /// <value> The length of the system. </value>
        public int SystemLength { get; set; }

        /// <summary> Gets or sets the main voice. </summary>
        /// <value> The main voice. </value>
        public IAbstractVoice MainVoice { get; set; }

        /// <summary> Gets or sets the voices. </summary>
        /// <value> The voices. </value>
        public List<IAbstractVoice> Voices { get; set; }
   
        /// <summary>
        /// Gets a list of identifiers.
        /// </summary>
        /// <value>
        /// A list of identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers {
            get {
                var items = new List<KeyValuePair>();

                var item = new KeyValuePair("Line type", this.LineType.ToString());
                items.Add(item);

                item = new KeyValuePair("Line number", this.LineNumber.ToString());
                items.Add(item);

                item = new KeyValuePair("Purpose", this.Purpose.ToString());
                items.Add(item);

                item = new KeyValuePair("Octave", this.MainVoice.Octave.ToString());
                items.Add(item);

                item = new KeyValuePair("Loudness", this.MainVoice.Loudness.ToString());
                items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Channel"), this.MainVoice.Channel.ToString());
                items.Add(item);

                item = new KeyValuePair("Instrument", this.MainVoice.Instrument.ToString());
                items.Add(item);

                //// 2020/09  item = new KeyValuePair("Rhythmic tension", this.FirstStatus.RhythmicTension.ToString()); items.Add(item);
                return items;
            }
        }
        #endregion

        #region Tones
        /// <summary> Gets total number of not empty tones in this part. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public int NumberOfTones {
            get {
                //// .Cast<MusicalTone>()
                var num = (from tone in this.Tones
                           where tone != null //// && melTone.Pitch != null
                           select 1)
                    .Count();
                return num;
            }
        }

        /// <summary> Gets total number of not empty tones in this part. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public long DurationOfTones {
            get {
                //// .Cast<MusicalTone>()
                long num = (from melTone in this.Tones
                            where melTone != null //// && melTone.Pitch != null 
                            select melTone.Duration)
                    .Sum();
                return num;
            }
        }

        /// <summary>
        /// Gets the number of tones.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int NumberOfAllTones => this.Tones.Count;

        #endregion

        #region Tone history
        /// <summary> Gets or sets current melodic tone. </summary>
        /// <value> Property description. </value>
        /// [XmlIgnore]
        public MusicalTone CurrentTone { get; set; }

        /// <summary> Gets or sets last melodic tone. </summary>
        /// <value> Property description. </value>
        /// [XmlIgnore]
        public MusicalTone LastTone { get; set; }

        /// <summary> Gets or sets last but one melodic tone. </summary>
        /// <value> Property description. </value>
        /// [XmlIgnore]
        public MusicalTone PenultTone { get; set; }
        #endregion

        #region Meta Texts
        /// <summary>
        /// Gets Midi Meta Text.
        /// </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        public string MidiMetaText {
            get {
                var s = "*** TRACK ***" + this.LineIndex.ToString(CultureInfo.CurrentCulture);
                //// if (this.LineIndex == 1) {  ////  if (this.StatusChanged && this.Status != null) {
                ////     s = this.AppendMetaTrackProperties(s);
                //// }
                return s;
            }
        }
        #endregion

        #region Private Properties
        /// <summary> Gets or sets file name. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        private byte Multiplicity { get; set; }

        #endregion

        #region Static factory methods
        /// <summary>
        /// Gets the new musical line.
        /// </summary>
        /// <param name="lineType">Type of the line.</param>
        /// <param name="musicalBlock">The musical block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalLine GetNewMusicalLine(MusicalLineType lineType, MusicalBlock musicalBlock) {
            var firstStatus = new LineStatus() {
                LocalPurpose = LinePurpose.None,
                LineType = lineType //// channel == MidiChannel.DrumChannel ? MusicalLineType.Rhythmic : MusicalLineType.Melodic
            };

            var line = new MusicalLine(firstStatus) {
                Strip = musicalBlock.Strip,
                Purpose = LinePurpose.None
            };

            return line;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Makes the inner objects.
        /// </summary>
        public void MakeInnerObjects() {
            this.tones = new ToneCollection();
            this.LineIdent = Guid.NewGuid();
            this.Purpose = LinePurpose.None;

            if (this.StatusList == null) {
                this.StatusList = new List<LineStatus>();
                this.FirstStatus = new LineStatus {
                    LocalPurpose = LinePurpose.None,
                    Instrument = new MusicalInstrument(MidiMelodicInstrument.None) //// FixedInstrument
                };
            }

            this.MainVoice = new MusicalVoice {
                Octave = this.FirstStatus.Octave,
                Instrument = this.FirstStatus.Instrument,
                Loudness = this.FirstStatus.Loudness,
                Line = this
            };

            this.Voices = new List<IAbstractVoice> { this.MainVoice };
        }

        /// <summary>
        /// Sets the tones.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        public void SetTones(ToneCollection givenTones) {
            this.tones = givenTones;
        }

        /// <summary>
        /// Sets the tones.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        /// <param name="givenBarNumber">The given bar number.</param>
        [UsedImplicitly]
        public void SetTones(ToneCollection givenTones, int givenBarNumber) {
            this.tones = givenTones;
            foreach (var tone in this.tones) {
                tone.BarNumber = givenBarNumber;
            }
        }

        /// <summary>
        /// Sets the midi event collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void SetMidiEventCollection(MidiEventCollection collection) {
            this.midiEventColl = collection;
        }

        /// <summary>
        /// Reset musical line.
        /// </summary>
        /// <param name="musicalStrip">The musical strip.</param>
        /// <param name="clearTones">Clear Tones.</param>
        public void Reset(MusicalStrip musicalStrip, bool clearTones) { //// MusicalRules musicalRules,
            this.Strip = musicalStrip;
            //// this.tones = new MusicalStrikeCollection();
            this.FirstStatus.MelodicPlan = new MelodicPlan();
            if (clearTones) {
                this.Reset();
            }
        }

        /// <summary> Makes a deep copy of the MusicalLine object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            //// this.LineIndex/0
            var status = (LineStatus)this.FirstStatus.Clone();
            var line = new MusicalLine(status) {
                Name = this.Name,
                LineIndex = this.LineIndex,
                TrackNumber = this.TrackNumber,
                Strip = this.Strip,
                Multiplicity = this.Multiplicity,
                TrackEngine = this.TrackEngine,
                Purpose = this.Purpose //// 2020/10
            };

            line.Reset();
            if (this.Voices != null && this.Voices.Count > 0) {
                line.Voices = new List<IAbstractVoice>();
                foreach (var voice in this.Voices) {
                    line.Voices.Add(voice); //// .Clone()!!!
                }
            }

            return line;
        }

        /// <summary>
        /// Clones the specified include tones.
        /// </summary>
        /// <param name="includeTones">If set to <c>true</c> [include tones].</param>
        /// <returns> Returns value. </returns>
        public MusicalLine Clone(bool includeTones) {
            var line = (MusicalLine)this.Clone();
            if (includeTones) {
                line.Tones.AddCollection(this.Tones.Clone(false), false);
            }

            return line;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            this.tones = new ToneCollection();
        }

        #endregion

        #region Public Methods - Characteristics
        /// <summary>
        /// Quotients the of occupation.
        /// </summary>
        /// <returns> Returns value. </returns>
        public float QuotientOfOccupation() {
            var cnt = 0;
            var sum = 0;
            var header = this.Strip.Context.Header;
            for (var barNumber = 1; barNumber <= header.NumberOfBars; barNumber++) {
                var occupation = this.OccupationOfBar(header.System.RhythmicOrder, barNumber);

                var barSum = (from v in occupation where v > 0 select v).Sum();
                var barCount = (from v in occupation where v > 0 select v).Count();
                sum += barSum;
                cnt += barCount;
            }

            var value = ((float)sum) / cnt;
            return value;
        }

        /// <summary> Returns number of tones with given musical pitch. </summary>
        /// <param name="pitch">Musical pitch.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public byte CountOfUsedPitch(MusicalPitch pitch) {
            Contract.Requires(pitch != null);

            var num = (from melTone in this.Tones
                       .Cast<MusicalTone>()
                       where melTone.Pitch != null && melTone.Pitch.IsEqualTo(pitch)
                       select 1)
                       .Count();

            return (byte)num;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "{0,4}, ", this.LineIndex); //\r\n\
            s.Append(this.FirstStatus.LineType);
            //// s.Append(this.OctaveString);
            //// s.Append(this.BandTypeString);
            //// s.Append(this.InstrumentString);
            //// if (this.MusicalTones == null) { return s.ToString(); }

            var firstTone = this.Tones.FirstOrDefault();
            if (firstTone != null) {
                s.Append(string.Format(CultureInfo.InvariantCulture, "#Bar{0}#", firstTone.BarNumber));
            }

            s.AppendLine(string.Empty);
            s.Append(this.Tones.TonesToString());
            s.AppendFormat("Ident={0}", this.LineIdent);

            //// this.TonesToString(s);
            return s.ToString();
        }
        #endregion

        #region Public Methods - Load from Midi Lines
        /// <summary>
        /// Append MidiTrack Tones.
        /// </summary>
        /// <param name="midiTones">The midi tones.</param>
        public void AppendMidiTrackTones(IMidiTones midiTones) {
            if (midiTones == null) {
                return;
            }

            this.FirstStatus.LineType = midiTones.IsRhythmical ? MusicalLineType.Rhythmic : MusicalLineType.Melodic;
            this.Name = midiTones.Name;
            this.FirstStatus.Octave = midiTones.Octave;
            this.FirstStatus.BandType = midiTones.BandType;  //// MusicalPitch.BandTypeFromOctave(musicalLine.MusicalOctave);
            if (this.Strip.Context.Header.Metric.MetricGround > 0) { //// && midiTrack.Sequence != null
                var ts = ToneCollection.GetTones(this.Strip.Context.Header, midiTones, this.FirstStatus.IsMelodic);
                this.SetTones(ts);
            }
        }

        #endregion

        #region Public Methods - Transformation
        /// <summary>
        /// Total Horizontal Inversion.
        /// </summary>
        public void TotalHorizontalInversion() {
            if (this.Tones == null || this.Tones.Count == 0) {
                return;
            }

            var mtc = new ToneCollection();
            var lastMusTone = this.Tones.LastOrDefault();
            if (lastMusTone != null) {
                var extraBarNumber = lastMusTone.BarNumber + 1;
                foreach (var mtone in this.Tones) {
                    if (!(mtone is MusicalStrike tone)) {
                        continue;
                    }

                    tone.BarNumber = extraBarNumber - tone.BarNumber;
                    var p = tone.IsFromPreviousBar;
                    tone.IsFromPreviousBar = tone.IsGoingToNextBar;
                    tone.IsGoingToNextBar = p;
                    mtc.Insert(0, tone);
                }
            }

            this.tones = mtc;
        }

        /// <summary>
        /// Bar Horizontal Inversion.
        /// </summary>
        public void BarHorizontalInversion() {
            if (this.Tones == null || this.Tones.Count == 0) {
                return;
            }

            var mtc = new ToneCollection();
            var lastMusTone = this.Tones.LastOrDefault();
            if (lastMusTone == null) {
                return;
            }

            var lastBarNumber = lastMusTone.BarNumber;
            for (var barNum = 1; barNum <= lastBarNumber; barNum++) {
                var barTones = this.MusicalTonesInBar(barNum);
                switch (barTones.Count) {
                    case 0:
                        continue;
                    case 1:
                        if (barTones[0] is MusicalStrike mtone) {
                            mtone.IsGoingToNextBar = false;
                            mtone.IsFromPreviousBar = false;
                            mtc.Add(mtone);
                        }

                        continue;
                }

                DoInversion(barTones, mtc);
            }

            this.tones = mtc;
        }

        /// <summary>
        /// Divides the tones.
        /// </summary>
        /// <param name="factorNumber">The factor number.</param>
        [UsedImplicitly]
        public void DivideTones(byte factorNumber) {
            var list = this.Tones.ToList();
            //// Introduce foreach action - DefExpress 
            foreach (var tn in list) {
                var tone = tn as MusicalStrike;
                if (tone != null && (tone.IsPause || tone.Duration % factorNumber != 0)) {
                    continue;
                }

                if (tone != null) {
                    var d = (byte)(tone.BitRange.Length / factorNumber);
                    tone.BitRange = new BitRange(tone.BitRange.Order, tone.BitRange.BitFrom, d);
                    tone.Duration = d;
                    MusicalStrike newTone = null;
                    for (var i = 1; i < factorNumber; i++) {
                        newTone = (MusicalStrike)tone.CloneTone();
                        newTone.IsGoingToNextBar = false;
                        newTone.IsFromPreviousBar = false;
                        newTone.BitRange = new BitRange(tone.BitRange.Order, (byte)(tone.BitRange.BitFrom + (i * d)), d);
                        this.Tones.Add(newTone);
                    }

                    if (newTone != null) {
                        newTone.IsGoingToNextBar = tone.IsGoingToNextBar;
                    }
                }

                if (tone != null) {
                    tone.IsGoingToNextBar = false;
                }
            }

            var sortedTones = from mt in this.Tones
                              orderby mt.BitPosition
                              select mt;
            this.tones = new ToneCollection(sortedTones.ToList());
        }
        #endregion

        #region Public Methods - Manipulation with tones
        /// <summary> Gets or sets previous melodic tone. </summary>
        /// <value> Property description. </value>
        /// <param name="givenTone">Melodic tone.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MusicalTone PreviousTone(MusicalStrike givenTone) {
            Contract.Requires(givenTone != null);
            MusicalTone tone = null;
            //// if (givenTone == null) { return false;  }

            var idx = givenTone.OrdinalIndex;   //// this.MusicalTones.IndexOf(givenTone);
            if (idx <= 0) {
                return null;
            }

            for (var i = idx - 1; i >= 0; i--) {
                if (i >= this.Tones.Count) {
                    continue;
                }

                tone = this.Tones[i] as MusicalTone;
                if (tone != null && tone.IsTrueTone) {  ////  Loudness > 0
                    return tone; //// Avoid multiple or conditional return statements.
                }
            }

            return tone;
        }

        /// <summary> List of tones in one bar of musical part. </summary>
        /// <param name="givenArea">Musical area.</param>
        /// <returns> Returns value. </returns>
        public ToneCollection MusicalTonesInArea(MusicalSection givenArea) {
            Contract.Ensures(Contract.Result<ToneCollection>() != null);

            var list = from mT in this.Tones
                       where mT.BarNumber >= givenArea.BarFrom && mT.BarNumber <= givenArea.BarTo
                       select mT;
            var mtc = new ToneCollection(list.ToList());

            //// 2015/01
            var newTone = (from mT in this.Tones
                           where mT.BarNumber == givenArea.BarTo + 1 && mT.IsFromPreviousBar
                           select mT).FirstOrDefault();
            if (newTone != null) {
                mtc.AddTone(newTone, true);
            }

            return mtc;
        }

        /// <summary> List of tones in one bar of musical part. </summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <returns> Returns value. </returns>
        public ToneCollection MusicalTonesInBar(int barNumber) {
            Contract.Ensures(Contract.Result<ToneCollection>() != null);

            var list = from mT in this.Tones
                       where mT.BarNumber == barNumber
                       select mT;
            var mtc = new ToneCollection(list.ToList());
            return mtc;
        }

        /// <summary> List of tones in one bar of musical part. </summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <returns> Returns value. </returns>
        public MusicalToneCollection MelodicTonesInBar(int barNumber) {
            var list = from mT in this.Tones
                       where mT.BarNumber == barNumber && mT.ToneType == MusicalToneType.Melodic
                       select mT;
            var mtc = new MusicalToneCollection();
            foreach (var mtone in list.OfType<MusicalTone>().Where(mtone => !mtone.IsEmpty)) {
                mtc.Add(mtone);
            }

            return mtc;
        }

        /// <summary>
        /// Returns musical tone at given bar and tick in this part.
        /// </summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <param name="tick">Rhythmical tick.</param>
        /// <param name="rhythmicStructure">The rhythmic structure.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public MusicalStrike MusicalToneAt(int barNumber, byte tick, FiguralStructure rhythmicStructure) {  //// RhythmicStructure
            var list = this.MusicalTonesInBar(barNumber);
            if (rhythmicStructure == null) {
                return null;
            }

            var level = rhythmicStructure.LevelOfBit(tick);
            return (level < list.Count ? list[level] : null) as MusicalStrike;
        }

        /// <summary> Add on musical tone to the end of part. </summary>
        /// <param name="givenTone">Musical tone.</param>
        public void AddMusicalTone(IMusicalTone givenTone) {
            Contract.Requires(givenTone != null);
            //// if (givenTone == null) { return false;  }

            givenTone.OrdinalIndex = this.Tones.Count;
            this.Tones.Add(givenTone);
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Does the inversion.
        /// </summary>
        /// <param name="barTones">The bar tones.</param>
        /// <param name="tones">The tones.</param>
        private static void DoInversion(IList<IMusicalTone> barTones, ICollection<IMusicalTone> tones) {
            Contract.Requires(barTones != null);
            Contract.Requires(tones != null);
            byte bit = 0;
            IMusicalTone mtoneA = null, mtoneB = null;
            var tonePosition = barTones.Count;
            while (--tonePosition >= 0) {
                var mtone0 = barTones[tonePosition];
                if (mtone0 == null) {
                    continue;
                }

                if (mtone0.IsPause && tonePosition > 0) {
                    var mtone1 = mtone0;
                    mtone0 = barTones[tonePosition - 1];
                    //// swap tones followed by a short pause
                    if (mtone0 != null) {
                        if (!mtone0.IsPause && mtone1.IsPause && mtone0.Duration > mtone1.Duration) {
                            mtoneA = mtone0;
                            mtoneB = mtone1;
                        }
                        else {
                            mtoneA = mtone1;
                            mtoneB = mtone0;
                        }
                    }

                    if (mtoneA != null) {
                        mtoneA.BitFrom = bit;
                        mtoneA.IsGoingToNextBar = false;
                        mtoneA.IsFromPreviousBar = false;

                        //// mtoneA.BitTo = (byte)(bit + mtoneA.Duration - 1);
                        bit = (byte)(mtoneA.BitTo + 1);
                        tones.Add(mtoneA);

                        mtoneB.BitFrom = bit;
                        mtoneB.IsGoingToNextBar = false;
                        mtoneB.IsFromPreviousBar = false;

                        //// mtoneB.BitTo = (byte)(bit + mtoneB.Duration - 1);
                        bit = (byte)(mtoneB.BitTo + 1);
                        tones.Add(mtoneB);
                    }

                    tonePosition--;
                }
                else {
                    mtone0.BitFrom = bit;
                    mtone0.IsGoingToNextBar = false;
                    mtone0.IsFromPreviousBar = false;

                    //// mtone0.BitTo = (byte)(bit + mtone0.Duration - 1);
                    bit = (byte)(mtone0.BitTo + 1);
                    tones.Add(mtone0);
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Occupations the of bar.
        /// </summary>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <param name="barNumber">The number of bar.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private int[] OccupationOfBar(byte rhythmicOrder, int barNumber) {
            var occupation = new int[rhythmicOrder];
            for (byte tick = 0; tick < rhythmicOrder; tick++) {
                occupation[tick] = 0;
            }

            var tonesInBar = this.MusicalTonesInBar(barNumber);
            foreach (var tone in tonesInBar) {
                for (var tick = tone.BitFrom; tick <= tone.BitTo; tick++) {
                    occupation[tick] += 1;
                }
            }

            return occupation;
        }
        #endregion
    }
}
