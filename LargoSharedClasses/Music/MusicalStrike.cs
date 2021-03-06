// <copyright file="MusicalStrike.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Notation;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary>  Musical tone. </summary>
    /// <remarks> Tone is defined in given bit Range with a given Loudness. </remarks>
    [Serializable]
    [XmlRoot]
    public class MusicalStrike : GeneralOwner, IMusicalTone
    {
        #region Fields and variables
        /// <summary> Mark for musical pause. </summary>
        public const string CPause = " ";

        /// <summary> Mark for rhythmical bang. </summary>
        public const string CBeat = "!";

        /// <summary> Mark for duration. </summary>
        public const string CRepeat = "-";

        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the MusicalStrike class.  Serializable. </summary>
        public MusicalStrike() {
            this.OrdinalIndex = -1;
        }

        /// <summary> Initializes a new instance of the MusicalStrike class. </summary>
        /// <param name="toneType">Type of tone.</param>
        /// <param name="givenBitRange">Rhythmical range of bits.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalStrike(
                        MusicalToneType toneType,
                        BitRange givenBitRange,
                        MusicalLoudness loudness,
                        int barNumber) {
            this.ToneType = toneType;
            this.BitRange = givenBitRange;
            this.Loudness = loudness;
            this.BarNumber = barNumber;
        }

        /// <summary> Initializes a new instance of the MusicalStrike class. </summary>
        /// <param name="toneType">Type of tone.</param>
        /// <param name="givenRhythmicOrder">Rhythmical order.</param>        
        /// <param name="givenDuration">Musical duration.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalStrike(
                        MusicalToneType toneType,
                        byte givenRhythmicOrder,
                        byte givenDuration,
                        MusicalLoudness loudness,
                        int barNumber) {
            this.ToneType = toneType;
            this.RhythmicOrder = givenRhythmicOrder;
            this.BitFrom = 0;
            this.Duration = givenDuration;
            this.Loudness = loudness;
            this.BarNumber = barNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStrike"/> class.
        /// </summary>
        /// <param name="xelement">The xml element.</param>
        /// <param name="rorder">The rhythmic order.</param>
        public MusicalStrike(XElement xelement, byte rorder) {
            Contract.Requires(xelement != null);
            //// if (xelement == null) { return; }

            //// this.bitRange = new BitRange(); 
            //// this.bitRange.SetXElement(element);
            this.BarNumber = XmlSupport.ReadIntegerAttribute(xelement.Attribute("Bar"));
            this.BitFrom = XmlSupport.ReadByteAttribute(xelement.Attribute("Start"));
            this.Duration = XmlSupport.ReadByteAttribute(xelement.Attribute("Length"));
            this.BitRange = new BitRange(rorder, this.BitFrom, (byte)this.Duration);
            //// string s = LibSupport.ReadStringAttribute(xelement.Attribute("ToneType"));
            //// this.ToneType = string.IsNullOrEmpty(s) ? MusicalToneType.Empty : (MusicalToneType)Enum.Parse(typeof(MusicalToneType), s);
            this.ToneType = MusicalToneType.Rhythmic;
            this.Loudness = DataEnums.ReadAttributeMusicalLoudness(xelement.Attribute("Loudness"));
            var attrInstr = xelement.Attribute("Instrument");
            if (attrInstr != null) {
                this.InstrumentNumber = XmlSupport.ReadByteAttribute(attrInstr);
            }
            else {
                this.InstrumentNumber = (byte)MidiMelodicInstrument.None;
            }
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                var xe = new XElement(
                                "Tick",
                                new XAttribute("Bar", this.BarNumber),
                                new XAttribute("Start", this.BitFrom),
                                new XAttribute("Length", this.Duration));
                if (this.Loudness != MusicalLoudness.MeanLoudness) {
                    xe.Add(new XAttribute("Loudness", this.Loudness.ToString()));
                }

                return xe;
            }
        }
        #endregion

        #region Tone Properties
        /// <summary> Gets or sets type of musical tone. </summary>
        /// <value> Property description. </value>
        public MusicalToneType ToneType { get; set; }

        /// <summary> Gets or sets ordinal index of tone in musical track. </summary>
        /// <value> Property description. </value>
        public int OrdinalIndex { get; set; }

        /// <summary> Gets or sets loudness. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness Loudness { get; set; }

        /// <summary> Gets or sets a value indicating whether property of musical tone. </summary>
        /// <value> Property description. </value>
        public bool IsReady { get; set; }

        /// <summary> Gets or sets bar number. </summary>
        /// <value> Property description. </value>
        public int BarNumber { get; set; }

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte Staff { get; set; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte Voice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from previous bar.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is from previous bar; otherwise, <c>false</c>.
        /// </value>
        public bool IsFromPreviousBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [continue to next bar].
        /// </summary>
        /// <value>
        /// <c>True</c> if [continue to next bar]; otherwise, <c>false</c>.
        /// </value>
        public bool IsGoingToNextBar { get; set; }

        /// <summary> Gets property of musical tone. </summary>
        /// <value> Property description. </value>
        public int BitPosition => ((this.BarNumber - 1) * this.RhythmicOrder) + this.BitFrom;

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte RhythmicOrder { get; set; }

        /// <summary> Gets or sets BitFrom. </summary>
        /// <value> Property description. </value>
        public byte BitFrom { get; set; }

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the bit range.
        /// </summary>
        /// <value>
        /// The bit range.
        /// </value>
        public BitRange BitRange {
            get {
                Contract.Ensures(Contract.Result<BitRange>() != null);
                var bitRange = new BitRange(this.RhythmicOrder, this.BitFrom, (byte)this.Duration);
                return bitRange;
            }

            set {
                var bitRange = value;
                if (bitRange == null) {
                    return;
                }

                this.RhythmicOrder = bitRange.Order;
                this.BitFrom = bitRange.BitFrom;
                this.Duration = bitRange.Length;
            }
        }

        /// <summary> Gets BitTo. </summary>
        /// <value> Property description. </value>
        public byte BitTo => this.BitRange.BitTo;

        /// <summary> Gets a value indicating whether IsPause. </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public bool IsPause => (this.ToneType == MusicalToneType.Empty) || (this.Loudness == 0);

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEmpty => this.IsPause;

        /// <summary> Gets or sets index to harmonic structure.
        /// Musical instrument.
        /// </summary>
        /// <value> Property description. </value>
        public byte InstrumentNumber { get; set; }

        #endregion

        #region Note Properties
        /// <summary>
        /// Gets or sets the length of the note.
        /// </summary>
        /// <value>
        /// The length of the note.
        /// </value>
        public NoteLength NoteLength { get; set; }

        /// <summary> Gets The MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public virtual byte NoteNumber => 0;

        /// <summary> Gets the MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public virtual string Note => string.Empty;

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets the melodic identifier.
        /// </summary>
        /// <value>
        /// The melodic identifier.
        /// </value>
        public virtual string MelodicIdentifier => $"Tick#{this.BitFrom}/{this.Duration}";

        /// <summary>
        /// Gets the rhythmic identifier.
        /// </summary>
        /// <value>
        /// The rhythmic identifier.
        /// </value>
        public virtual string RhythmicIdentifier => $"{this.BitFrom}/{this.Duration}";

        #endregion

        #region Public static methods

        /// <summary>
        /// Gets the new musical tone.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="quotient">The quotient.</param>
        /// <param name="midiTone">The midi tone.</param>
        /// <param name="barNumber">The bar number.</param>
        /// <param name="realBarNumber">The real bar number.</param>
        /// <param name="melodicTrack">if set to <c>true</c> [melodic track].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IMusicalTone GetNewMusicalTone(
                    MusicalHeader givenHeader,
                    int quotient,
                    IMidiTone midiTone,
                    int barNumber,
                    int realBarNumber,
                    bool melodicTrack) {
            //// this.Strip.Context.Header //// this.Status.IsMelodic
            IMusicalTone musTone;
            var bitFrom = (int)Math.Round((double)midiTone.StartTime / quotient, 0);
            var bitTo = (int)Math.Round((double)(midiTone.StartTime + midiTone.Duration) / quotient, 0) - 1;
            var rhythmicOrder = givenHeader.System.RhythmicOrder;
            var bitLimit = rhythmicOrder * realBarNumber; //// Zero bit of next bar
            var bitZero = bitLimit - rhythmicOrder;
            if (bitTo < bitZero) {
                return null;
            }

            var isFromPreviousBar = barNumber > midiTone.BarNumberFrom;
            var continueToNextBar = barNumber < midiTone.BarNumberTo && bitTo >= bitLimit;

            var barBitFrom = !isFromPreviousBar ? bitFrom % rhythmicOrder : 0;
            var barBitTo = !continueToNextBar ? bitTo % rhythmicOrder : rhythmicOrder - 1;
            var duration = barBitTo - barBitFrom + 1;

            if (duration <= 0) {
                return null;
            }

            var bitRange = new BitRange(rhythmicOrder, (byte)barBitFrom, (byte)duration);

            if (midiTone.Loudness == MusicalLoudness.None) {
                return null;
                //// 2019/02 Reduction of redundant pauses, here was == MusicalLoudness.None ==>
                //// mtone = MusicalPause.CreatePause(rhythmicOrder, (byte)barBitFrom, (byte)duration, barNumber); return mtone; 
            }

            if (melodicTrack) {
                var hs = givenHeader.System.HarmonicSystem;
                musTone = MusicalTone.CreateMelodicTone(hs, bitRange, midiTone.NoteNumber, barNumber, midiTone.Loudness);
            }
            else {
                musTone = new MusicalStrike(MusicalToneType.Rhythmic, bitRange, midiTone.Loudness, barNumber);
            }

            musTone.ToneType = melodicTrack ? MusicalToneType.Melodic : MusicalToneType.Rhythmic;

            //// 2019/02 if (!(mtone is MusicalStrike musTone)) {  return null;  } 

            ////if (midiTone.Loudness != 0) {
            musTone.InstrumentNumber = melodicTrack ? midiTone.InstrumentNumber : midiTone.NoteNumber;
            //// musTone.Channel = this.Status.IsMelodic ? midiTone.Channel : MidiChannel.DrumChannel;
            ////}
            ////  else { musTone.IsFromPreviousBar = false;   musTone.IsGoingToNextBar = false; } 
            musTone.IsFromPreviousBar = isFromPreviousBar;
            musTone.IsGoingToNextBar = continueToNextBar;

            //// musTone.BarNumberTo = midiTone.BarNumberTo;
            return musTone;
        }

        /// <summary>
        /// Corrects the incorrect binding.
        /// </summary>
        /// <param name="firstTone">The first tone.</param>
        /// <param name="nextTone">The next tone.</param>
        public static void CorrectBadBinding(MusicalStrike firstTone, MusicalStrike nextTone) {
            if (firstTone == null || nextTone == null) {
                return;
            }

            if (firstTone.IsGoingToNextBar && !nextTone.IsFromPreviousBar) {
                firstTone.IsGoingToNextBar = false;
            }

            if (nextTone is MusicalTone nextMelTone && firstTone is MusicalTone firstMelTone && firstTone.IsGoingToNextBar
                            && firstMelTone.Pitch.SystemAltitude != nextMelTone.Pitch.SystemAltitude) {
                firstTone.IsGoingToNextBar = false;
                nextTone.IsFromPreviousBar = false;
            }

            if (!firstTone.IsGoingToNextBar || firstTone.BarNumber == nextTone.BarNumber - 1) {
                return;
            }

            firstTone.IsGoingToNextBar = false;
            nextTone.IsFromPreviousBar = false;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Accept content of given tone.
        /// </summary>
        /// <param name="tone">Musical Strike.</param>
        public void SetMusicalTone(MusicalStrike tone) {
            if (tone == null) {
                return;
            }

            //// this.SetBitRange(tone.BitRange());
            this.BitRange = (BitRange)tone.BitRange.Clone();
            this.Loudness = tone.Loudness;
            this.ToneType = tone.ToneType;
            this.BarNumber = tone.BarNumber;
            if (tone.Properties != null) {
                this.CopyProperties(tone.Properties);
            }
        }

        /// <summary>
        /// Makes a deep copy of the GeneralOwner object.
        /// </summary>
        /// <returns>
        /// Returns object.
        /// </returns>
        public override object Clone() {
            return this.CloneTone();
        }

        /// <summary> Makes a deep copy of the MusicalStrike object. </summary>
        /// <returns> Returns object. </returns>
        public virtual object CloneTone() {
            var mt = new MusicalStrike(this.ToneType, this.BitRange, this.Loudness, this.BarNumber)  //// (this.BarNumber)
            { OrdinalIndex = this.OrdinalIndex };
            return mt;
        }
        #endregion

        #region Public Midi Support
        /// <summary>
        /// Write real tone to midi collection.
        /// </summary>
        /// <param name="midiEvents">Midi event collection.</param>
        /// <param name="barDivision">Bar division.</param>
        public void WriteTo(MidiEventCollection midiEvents, int barDivision) {
            if (midiEvents == null) {
                return;
            }

            //// BitRange br = this.BitRange(this.BarNumber);
            if (this.RhythmicOrder <= 0) {
                return;
            }

            const int startTime = 0;
            var duration = MusicalProperties.MidiDuration(this.RhythmicOrder, this.Duration, barDivision);
            var stopTime = startTime + duration;
            byte note = 0;
            //// byte channel = (byte)this.Channel;
            var loudness = (byte)MusicalLoudness.None;
            switch (this.ToneType) {
                case MusicalToneType.Rhythmic: {
                        //// channel = (byte)MidiChannel.DrumChannel;
                        if (this.InstrumentNumber == 0) {
                            this.InstrumentNumber = (byte)MidiRhythmicInstrument.HighBongo;
                        }

                        loudness = (byte)this.Loudness;  //// (byte)MusicalLoudness.MeanLoudness;
                        note = this.InstrumentNumber;
                        break;
                    }

                case MusicalToneType.Melodic: {
                        if (!this.IsPause && (this.Loudness > 0)) {  //// melTone != null && (melTone.Pitch != null) &&
                            if (this is MusicalTone melTone && melTone.IsTrueTone) {
                                note = melTone.Pitch.MidiKeyNumber; //// (byte)melTone.Pitch.MidiPitchBend(),
                                loudness = (byte)melTone.Loudness;
                                //// channel = (byte)this.Channel;
                            }
                        }

                        break;
                    }
            }

            midiEvents.PutNote(startTime, note, stopTime, loudness, this.IsFromPreviousBar, this.IsGoingToNextBar);
        }

        /// <summary>
        /// Write Tone Events.
        /// </summary>
        /// <param name="midiEvents">Midi event collection.</param>
        /// <param name="barDivision">Bar division.</param>
        /// <param name="bitDuration">Delta Time of bit.</param>
        /// <param name="barDuration">Delta Time of bar.</param>
        /// <param name="deltaTimeShift">Delta Time Shift.</param>
        /// <returns> Returns value. </returns>
        public bool WriteTo(MidiEventCollection midiEvents, int barDivision, int bitDuration, int barDuration, int deltaTimeShift) {  //// cyclomatic complexity 10:13
            if (midiEvents == null) {
                return false;
            }

            if (this.RhythmicOrder <= 0) {
                return false;
            }

            var mtone = this;
            var bitRangeTo = mtone.BitRange; //// (mtone.BarNumberTo)
            if (mtone.RhythmicOrder <= 0 || bitRangeTo == null) {
                return false;
            }

            var barStartTime = 1 + (barDuration * (mtone.BarNumber - 1)) - deltaTimeShift;
            var startTime = barStartTime + (bitDuration * mtone.BitFrom); //// lastTotalTime = 0
            var stopTime = barStartTime + (bitDuration * (mtone.BitTo + 1)) - 1;
            if (stopTime <= startTime) {
                return false;
            }

            //// midiEvent = this.GetNoteOff(deltaTime2 - deltaTimeShift - 1); //// Temporary - decreased by 1, see comment on SortByStartTime
            switch (this.ToneType) {
                case MusicalToneType.Rhythmic: {
                        this.WriteRhythmicToneTo(midiEvents, startTime, stopTime);
                        break;
                    }

                case MusicalToneType.Melodic: {
                        this.WriteMelodicToneTo(midiEvents, startTime, stopTime);
                        break;
                    }
            }

            return true;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public virtual string ToShortString() {
            var s = new StringBuilder();
            if (this.Loudness == 0) {
                s.Append(MusicalStrike.CPause);
            }

            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder(string.Empty);
            //// if (this is MusicalTone) { return s.ToString(); } 
            //// s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", this.Loudness > 0 ? MusicalStrike.CBeat : MusicalStrike.CPause);
            //// for (byte lev = 2; lev <= this.Duration; lev++) {
            ////   s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", MusicalStrike.CRepeat); } 

            s.AppendFormat(" <{0,3}>", this.Duration);
            //// s.Append(",lev" + Loudness.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat) + ")");
            //// s.Append(base.ToString());
            return s.ToString();
        }

        /// <summary>
        /// Rhythmic to string.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string RhythmicToString() {
            var s = new StringBuilder(string.Empty);
            //// if (this is MusicalTone) {  return s.ToString(); }
            s.AppendFormat(" <{0,3}>", this.Duration);
            return s.ToString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Writes the melodic tone to.
        /// </summary>
        /// <param name="midiEvents">The midi events.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="stopTime">The stop time.</param>
        private void WriteMelodicToneTo(MidiEventCollection midiEvents, int startTime, int stopTime) {
            byte note = 0, loudness = (byte)MusicalLoudness.None;
            if (!this.IsPause && (this.Loudness > 0)) {  //// melTone != null && (melTone.Pitch != null) &&
                if (this is MusicalTone melTone && melTone.IsTrueTone) {
                    note = melTone.Pitch.MidiKeyNumber; //// (byte)melTone.Pitch.MidiPitchBend(),
                    loudness = (byte)melTone.Loudness;
                }
            }

            if (note > 0) {
                midiEvents.PutNote(startTime, note, stopTime, loudness, this.IsFromPreviousBar, this.IsGoingToNextBar);
            }
        }

        /// <summary>
        /// Writes the rhythmic tone to.
        /// </summary>
        /// <param name="midiEvents">The midi events.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="stopTime">The stop time.</param>
        private void WriteRhythmicToneTo(MidiEventCollection midiEvents, int startTime, int stopTime) {
            Contract.Requires(midiEvents != null);

            if (this.InstrumentNumber == 0) {
                this.InstrumentNumber = (byte)MidiRhythmicInstrument.HighBongo;
            }

            var loudness = (byte)this.Loudness;
            var note = this.InstrumentNumber;
            //// if (note > 0) {
            midiEvents.PutNote(startTime, note, stopTime, loudness, this.IsFromPreviousBar, this.IsGoingToNextBar);
            //// }
        }
        #endregion
    }
}
