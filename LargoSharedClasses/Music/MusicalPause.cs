// <copyright file="MusicalPause.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Notation;

namespace LargoSharedClasses.Music
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;
    using Midi;

    /// <summary>  Musical tone. </summary>
    /// <remarks> Tone is defined in given bit Range with a given Loudness. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class MusicalPause : GeneralOwner, IMusicalTone
    {
        #region Fields and variables

        /// <summary>
        /// Bar Number To.
        /// </summary>
        private int barNumberTo;

        //// <summary>
        //// Bit Range.
        //// </summary>
        //// private BitRange bitRange;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the MusicalPause class.  Serializable. </summary>
        public MusicalPause() {
        }

        /// <summary> Initializes a new instance of the MusicalPause class. </summary>
        /// <param name="givenBitRange">Rhythmical range of bits.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalPause(
                        BitRange givenBitRange,
                        int barNumber) {
            this.BitRange = givenBitRange;
            this.BarNumber = barNumber;
            this.ToneType = MusicalToneType.Empty;
            this.Loudness = MusicalLoudness.None;
            this.InstrumentNumber = (byte)MidiMelodicInstrument.None;
        }

        /// <summary> Initializes a new instance of the MusicalPause class. </summary>
        /// <param name="givenRhythmicOrder">Rhythmical order.</param>        
        /// <param name="givenDuration">Musical duration.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalPause(
                        byte givenRhythmicOrder,
                        byte givenDuration,
                        int barNumber) {
            this.RhythmicOrder = givenRhythmicOrder;
            this.Duration = givenDuration;
            this.BarNumber = barNumber;
            this.ToneType = MusicalToneType.Empty;
            this.Loudness = MusicalLoudness.None;
            this.InstrumentNumber = (byte)MidiMelodicInstrument.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalPause"/> class.
        /// </summary>
        /// <param name="xelement">The xml element.</param>
        /// <param name="rorder">The rhythmic order.</param>
        public MusicalPause(XElement xelement, byte rorder) {
            Contract.Requires(xelement != null);

            this.BarNumber = XmlSupport.ReadIntegerAttribute(xelement.Attribute("Bar"));
            this.BitFrom = XmlSupport.ReadByteAttribute(xelement.Attribute("Start")); ////BitFrom
            this.Duration = XmlSupport.ReadByteAttribute(xelement.Attribute("Length"));
            var br = new BitRange(rorder, this.BitFrom, (byte)this.Duration);
            this.BitRange = br;
            this.ToneType = MusicalToneType.Empty;
            this.Loudness = MusicalLoudness.None;

            //// 2019/11
            //// this.InstrumentNumber = (byte)MidiMelodicInstrument.None;
            var attrInstr = xelement.Attribute("Instrument");
            if (attrInstr != null) {
                this.InstrumentNumber = XmlSupport.ReadByteAttribute(attrInstr);
                //// see lastInstrument ...
            }
            else {
                this.InstrumentNumber = (int)MidiMelodicInstrument.None;
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get X element.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "Pause",
                                new XAttribute("Bar", this.BarNumber),
                                new XAttribute("Start", this.BitFrom),
                                new XAttribute("Length", this.Duration));
                return xe;
            }
        }
        #endregion

        #region Tone Properties
        /// <summary> Gets or sets type of musical tone. </summary>
        /// <value> Property description. </value>
        public MusicalToneType ToneType { get; set; }

        /// <summary> Gets or sets ordinal index of tone in musical line. </summary>
        /// <value> Property description. </value>
        public int OrdinalIndex { get; set; }

        /// <summary> Gets or sets loudness. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness Loudness { get; set; }

        /// <summary> Gets or sets a value indicating whether property of musical tone. </summary>
        /// <value> Property description. </value>
        public bool IsReady { get; set; }

        /// <summary> Gets a value indicating whether IsPause. </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public bool IsPause => true;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => true;

        /// <summary> Gets or sets bar number. </summary>
        /// <value> Property description. </value>
        public int BarNumber { get; set; }

        /// <summary> Gets or sets bar number. </summary>
        /// <value> Property description. </value>
        public int BarNumberTo {
            get => this.barNumberTo != 0 ? this.barNumberTo : this.BarNumber;

            set => this.barNumberTo = value;
        }

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte RhythmicOrder { get; set; }

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public int Duration { get; set; }

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

        /// <summary> Gets or sets BitFrom. </summary>
        /// <value> Property description. </value>
        public byte BitFrom { get; set; }

        /// <summary> Gets BitTo. </summary>
        /// <value> Property description. </value>
        public byte BitTo {
            get {
                var br = this.BitRange;
                return br?.BitTo ?? 0;
            }
        }

        /// <summary> Gets or sets index to harmonic structure.
        /// Musical instrument.
        /// </summary>
        /// <value> Property description. </value>
        public byte InstrumentNumber { get; set; }

        /// <summary> Gets or sets index to harmonic structure. </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; set; }

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

        #endregion

        #region Note Properties
        /// <summary>
        /// Gets or sets the length of the note.
        /// </summary>
        /// <value>
        /// The length of the note.
        /// </value>
        public NoteLength NoteLength { get; set; }

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets the melodic identifier.
        /// </summary>
        /// <value>
        /// The melodic identifier.
        /// </value>
        public string MelodicIdentifier => $"Pause#{this.BitFrom}/{this.Duration}";

        /// <summary>
        /// Gets the rhythmic identifier.
        /// </summary>
        /// <value>
        /// The rhythmic identifier.
        /// </value>
        public string RhythmicIdentifier => $"{this.BitFrom}/{this.Duration}";

        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Creates MusicalPause.
        /// </summary>
        /// <param name="rhythmicOrder">Rhythmical order.</param>
        /// <param name="bitFrom">Number of bit from.</param>
        /// <param name="duration">Midi Duration.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <returns> Returns value. </returns>
        public static MusicalPause CreatePause(byte rhythmicOrder, byte bitFrom, byte duration, int barNumber) {
            if (duration <= 0) {
                return null;
            }

            var bitRange = new BitRange(rhythmicOrder, bitFrom, duration);
            var pause = new MusicalPause(bitRange, barNumber);
            return pause;
        }
        #endregion 

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalStrike object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var mt = new MusicalPause(this.BitRange, this.BarNumber);
            return mt;
        }

        /// <summary> Makes a deep copy of the MusicalStrike object. </summary>
        /// <returns> Returns object. </returns>
        public object CloneTone() {
            var mt = new MusicalPause(this.BitRange, this.BarNumber);
            return mt;
        }
        #endregion

        #region Midi Support
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

            var duration = MusicalProperties.MidiDuration(this.RhythmicOrder, this.Duration, barDivision);
            midiEvents.PutNote(0, 0, duration, (byte)MusicalLoudness.None, false, false); //// (byte)0,
        }

        /// <summary>
        /// Write Tone Events.
        /// </summary>
        /// <param name="midiEvents">Midi events.</param>
        /// <param name="barDivision">Bar division.</param>
        /// <param name="bitDuration">Delta Time of bit.</param>
        /// <param name="barDuration">Delta Time of bar.</param>
        /// <param name="deltaTimeShift">Delta Time Shift.</param>
        /// <returns> Returns value. </returns>
        public bool WriteTo(MidiEventCollection midiEvents, int barDivision, int bitDuration, int barDuration, int deltaTimeShift) {
            if (midiEvents == null) {
                return false;
            }

            //// BitRange br = this.BitRange(this.BarNumber);
            if (this.RhythmicOrder <= 0) {
                return false;
            }

            var pause = this;
            //// MusicalTone melTone = mtone as MusicalTone;
            var bitRangeTo = pause.BitRange;
            if (pause.RhythmicOrder <= 0 || bitRangeTo == null) {
                return false;
            }

            var startTime = 1 + (barDuration * (pause.BarNumber - 1)) + (bitDuration * pause.BitFrom) - deltaTimeShift; //// lastTotalTime = 0
            var barNumber2 = pause.BarNumberTo != 0 ? pause.BarNumberTo : pause.BarNumber;
            if (barNumber2 <= 0) {  //// 2016/10
                return false;
            }

            var startTime2 = 1 + (barDuration * (barNumber2 - 1)) + (bitDuration * (bitRangeTo.BitTo + 1)) - deltaTimeShift;
            if (startTime2 <= startTime) {
                return false;
            }

            //// midiEvent = this.GetNoteOff(deltaTime2 - deltaTimeShift - 1); //// Temporary - decreased by 1, see comment on SortByStartTime
            var duration = startTime2 - 1;
            midiEvents.PutNote(0, 0, duration, (byte)MusicalLoudness.None, false, false); //// (byte)0,

            return true;
        }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public string ToShortString() { //// virtual
            var s = new StringBuilder();
            s.Append(MusicalStrike.CPause);
            s.Append(this.Duration);
            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder(string.Empty);
            s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", MusicalStrike.CPause);
            //// for (byte lev = 2; lev <= this.Duration; lev++) {
            //// s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", MusicalStrike.CRepeat); }
            s.AppendFormat(" <{0,3}>", this.Duration);
            //// s.Append(",lev" + Loudness.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat) + ")");

            //// s.Append(base.ToString());
            return s.ToString();
        }
        #endregion
    }
}
