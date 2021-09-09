// <copyright file="MusicalTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;

namespace LargoSharedClasses.Music
{
    /// <summary> Melodic tone. </summary>
    /// <remarks> Tone is defined as pitch placed in given bit Range with a given loudness.
    /// (it would possibly  incorporate melodic ornaments?).
    /// It is planned to keep some properties of harmonic clusters
    /// (e.g. harmonic cover, potential or doubling). </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class MusicalTone : MusicalStrike, IComparable
    {
        #region Fields
        /// <summary>
        /// Musical pitch.
        /// </summary>
        private MusicalPitch pitch;

        /// <summary>
        /// Sound weight.
        /// </summary>
        private float weight;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the MusicalTone class.  Serializable. </summary>
        public MusicalTone() {
            this.ToneType = MusicalToneType.Melodic;
            this.weight = -1;
        }

        /// <summary> Initializes a new instance of the MusicalTone class. </summary>
        /// <param name="givenBitRange">Rhythmical range of bits.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalTone(BitRange givenBitRange, int barNumber)
            : base(MusicalToneType.Melodic, givenBitRange, 0, barNumber) {
            this.weight = -1;
        }

        /// <summary> Initializes a new instance of the MusicalTone class. </summary>
        /// <param name="givenPitch">Musical pitch.</param>
        /// <param name="givenBitRange">Rhythmical range of bits.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalTone(MusicalPitch givenPitch, BitRange givenBitRange, MusicalLoudness loudness, int barNumber)
            : base(MusicalToneType.Melodic, givenBitRange, loudness, barNumber) {
            this.weight = -1;
            this.Pitch = givenPitch;
            //// if (this.IsEmpty) { return; }
        }

        /// <summary>
        /// Initializes a new instance of the MusicalTone class.
        /// </summary>
        /// <param name="givenPitch">Musical pitch.</param>
        /// <param name="givenRhythmicOrder">Rhythmic Order.</param>
        /// <param name="givenDuration">Musical duration.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalTone(MusicalPitch givenPitch, byte givenRhythmicOrder, byte givenDuration, MusicalLoudness loudness, int barNumber)
            : base(MusicalToneType.Melodic, givenRhythmicOrder, givenDuration, loudness, barNumber) {
            this.weight = -1;
            this.Pitch = givenPitch;
            //// if (this.IsEmpty) { return; }
        }

        /// <summary> Initializes a new instance of the MusicalTone class. </summary>
        /// <param name="toneType">Type of tone.</param>
        /// <param name="givenBitRange">Rhythmical range of bits.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalTone(MusicalToneType toneType, BitRange givenBitRange, MusicalLoudness loudness, int barNumber)
            : base(toneType, givenBitRange, loudness, barNumber) {
            this.weight = -1;
        }

        /// <summary> Initializes a new instance of the MusicalTone class. </summary>
        /// <param name="toneType">Type of tone.</param>
        /// <param name="givenRhythmicOrder">Rhythmical order.</param>
        /// <param name="givenDuration">Musical duration.</param>
        /// <param name="loudness">Musical loudness.</param>
        /// <param name="barNumber">Number of musical bar.</param>
        public MusicalTone(MusicalToneType toneType, byte givenRhythmicOrder, byte givenDuration, MusicalLoudness loudness, int barNumber)
            : base(toneType, givenRhythmicOrder, givenDuration, loudness, barNumber) {
            this.weight = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalTone"/> class.
        /// </summary>
        /// <param name="xelement">The xml element.</param>
        /// <param name="rorder">The rhythmic order.</param>
        public MusicalTone(XElement xelement, byte rorder) {
            //// if (xelement == null) { return; }

            this.BarNumber = XmlSupport.ReadIntegerAttribute(xelement.Attribute("Bar"));
            //// this.bitRange = new BitRange();
            //// this.bitRange.SetXElement(element);
            this.BitFrom = XmlSupport.ReadByteAttribute(xelement.Attribute("Start"));
            this.Duration = XmlSupport.ReadByteAttribute(xelement.Attribute("Length"));
            this.BitRange = new BitRange(rorder, this.BitFrom, (byte)this.Duration);

            //// string s = LibSupport.ReadStringAttribute(xelement.Attribute("ToneType"));
            //// this.ToneType = string.IsNullOrEmpty(s) ? MusicalToneType.Empty : (MusicalToneType)Enum.Parse(typeof(MusicalToneType), s);
            this.ToneType = MusicalToneType.Melodic;
            this.Loudness = DataEnums.ReadAttributeMusicalLoudness(xelement.Attribute("Loudness"));
            //// 2018/10 !?!?!? commented - instrument directed by status
            //// 2018/12 back to tone instruments 

            var attrInstr = xelement.Attribute("Instrument");
            if (attrInstr != null) {
                this.InstrumentNumber = XmlSupport.ReadByteAttribute(attrInstr);
                //// see lastInstrument ...
            }
            else {
                this.InstrumentNumber = (int)MidiMelodicInstrument.None;
            }

            var harmonicSystem = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            //// if (harmonicSystem == null) {  return;  }

            var xpitch = xelement.Attribute("Pitch");
            if (xpitch == null) {
                return;
            }

            this.pitch = new MusicalPitch(harmonicSystem);
            this.pitch.SetXAttribute(xpitch);
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public override XElement GetXElement {
            get {
                XElement xe;
                if (this.pitch != null) {
                    xe = new XElement(
                                "Tone", //// xmlMelTone.Add(new XAttribute("ToneType", this.ToneType));
                                new XAttribute("Bar", this.BarNumber),
                                new XAttribute("Start", this.BitFrom), //// this.BitRange.GetXElement
                                new XAttribute("Length", this.Duration),
                                new XAttribute("Note", this.Pitch.ToString()),  //// this.Pitch.GetXElement
                                this.pitch.GetXAttribute);
                    //// 2019/01 if (this.Loudness != MusicalLoudness.MeanLoudness) {
                    xe.Add(new XAttribute("Loudness", this.Loudness.ToString()));
                    //// }
                }
                else {
                    xe = new XElement("Pause", new XAttribute("Length", this.Duration));
                }

                return xe;
            }
        }
        #endregion

        #region Tone Properties
        /// <summary>
        /// Gets or sets the pause - pause following the note (e.g. because of note shortening)
        /// Duration of the pause is calculated separately.
        /// </summary>
        /// <value>
        /// The pause.
        /// </value>
        public MusicalPause Pause { get; set; }

        /// <summary> Gets or sets index to harmonic modality. </summary>
        /// <value> Property description. </value>
        public short ModalityIndex { get; set; }

        /// <summary> Gets or sets index to harmonic structure. </summary>
        /// <value> Property description. </value>
        public byte HarmonicIndex { get; set; }

        /// <summary> Gets or sets index for better identification of intervals (see HarmonicStateReal). </summary>
        /// <value> Property description. </value>
        public byte ToneIndex { get; set; }

        /// <summary> Gets musical pitch.</summary>
        /// <value> Property description. </value>
        public MusicalPitch Pitch {
            get {
                //// Contract.Requires(this.pitch != null);
                Contract.Ensures(Contract.Result<MusicalPitch>() != null);
                //// Time optimization - critical time of comparison !?
                //// if (this.pitch == null) { throw new InvalidOperationException("Pitch is null.");  } 

                return this.pitch;
            }

            private set => this.pitch = value?.Clone() as MusicalPitch;
        }

        /// <summary> Gets weight of the tone. </summary>
        /// <value> Property description. </value>
        public float Weight {
            get {
                if (this.weight >= 0) {
                    return this.weight;
                }

                var altitude = this.Pitch.OctaveAltitude(0f); // -4.0f
                var w = altitude >= DefaultValue.AfterZero && altitude <= DefaultValue.LargeNumber ? (short)this.Loudness / altitude : 0;

                this.weight = w;
                return this.weight;
            }
        }

        /// <summary> Gets a value indicating whether Is empty tone. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public override bool IsEmpty => this.pitch == null;

        /// <summary> Gets a value indicating whether is melodic tone. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public bool IsTrueTone => (this.ToneType == MusicalToneType.Melodic) && (this.pitch != null) && (this.Loudness > 0);

        /// <summary>
        /// Gets the melodic identifier.
        /// </summary>
        /// <value>
        /// The melodic identifier.
        /// </value>
        public override string MelodicIdentifier => $"{this.Pitch}#{this.BitFrom}/{this.Duration}";

        /// <summary>
        /// Gets the rhythmic identifier.
        /// </summary>
        /// <value>
        /// The rhythmic identifier.
        /// </value>
        public override string RhythmicIdentifier => $"{this.BitFrom}/{this.Duration}";

        #endregion

        #region Note Properties

        /// <summary>Gets or sets The MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public override byte NoteNumber {
            get {
                if (this.Pitch == null) {
                    return 0;
                }

                return this.Pitch.MidiKeyNumber; ////  (this.Pitch != null) ? this.Pitch.MidiKeyNumber : (byte)0;
            }
        }

        /// <summary>Gets or sets The MIDI note to modify (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public override string Note => MusicalProperties.GetNoteNameAndOctave(this.NoteNumber, DefaultValue.HarmonicOrder);

        /// <summary>
        /// Gets the note letter.
        /// </summary>
        /// <value> Property description. </value>
        public string NoteLetter => MusicalProperties.GetSingleNoteName(this.NoteNumber);

        /// <summary>
        /// Gets the note alter.
        /// </summary>
        /// <value> Property description. </value>
        public short NoteAlter => MusicalProperties.GetAlterSign(this.NoteNumber);

        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Creates MusicalTone.
        /// </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="bitRange">The bit range.</param>
        /// <param name="midiNote">Midi note.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="toneLoudness">Tone loudness.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IMusicalTone CreateMelodicTone(
                                            HarmonicSystem harmonicSystem,
                                            BitRange bitRange,
                                            byte midiNote,
                                            int barNumber,
                                            MusicalLoudness toneLoudness) {
            Contract.Requires(harmonicSystem != null);
            //// if (harmonicSystem == null) {  return;  }

            IMusicalTone musTone;
            if (midiNote > 0 && toneLoudness != MusicalLoudness.None) {
                //// musPitch = new MusicalPitch(harmonicSystem, midiNote);
                var musPitch = harmonicSystem.GetPitch(midiNote);
                var loudness = toneLoudness;
                //// loudness = 5;
                musTone = new MusicalTone(musPitch, bitRange, loudness, barNumber);
            }
            else {
                musTone = new MusicalPause(bitRange.Order, bitRange.Length, barNumber);
            }

            return musTone;
        }

        #endregion

        #region Static operators

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(MusicalTone tone1, MusicalTone tone2) {  return object.Equals(tone1, tone2);  }
        //// public static bool operator !=(MusicalTone tone1, MusicalTone tone2) {  return !object.Equals(tone1, tone2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(MusicalTone object1, MusicalTone object2) {
            if (object1 != null && object2 != null) {
                return object1.Pitch < object2.Pitch;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >(MusicalTone object1, MusicalTone object2) {
            if (object1 != null && object2 != null) {
                return object1.Pitch > object2.Pitch;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <=(MusicalTone object1, MusicalTone object2) {
            if (object1 != null && object2 != null) {
                return object1.Pitch <= object2.Pitch;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >=(MusicalTone object1, MusicalTone object2) {
            if (object1 != null && object2 != null) {
                return object1.Pitch >= object2.Pitch;
            }

            return false;
        }
        #endregion

        #region Comparison
        /// <summary> Compare tones. </summary>
        /// <param name="value">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object value) {
            var mt = value as MusicalTone;

            if (mt?.pitch == null || this.pitch == null) {
                return 0;
            }

            return this.pitch.CompareTo(mt.Pitch);
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a MusicalTone");
        }

        /// <summary> Test of equality. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override bool Equals(object obj) {
            //// check null (this pointer is never null in C# methods)
            if (object.ReferenceEquals(obj, null)) {
                return false;
            }

            if (object.ReferenceEquals(this, obj)) {
                return true;
            }

            if (this.GetType() != obj.GetType()) {
                return false;
            }

            return this.CompareTo(obj) == 0;
        }

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode() {
            return this.Pitch == null ? 0 : this.Pitch.GetHashCode();
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Makes a deep copy of the MusicalStrike object.
        /// </summary>
        /// <returns>
        /// Returns object.
        /// </returns>
        public override object Clone() {
            return this.CloneTone();
        }

        /// <summary> Makes a deep copy of the MusicalTone object. </summary>
        /// <returns> Returns object. </returns>
        public override object CloneTone() {
            var mt = new MusicalTone(this.Pitch, this.BitRange, this.Loudness, this.BarNumber) { //// (this.BarNumber)
                ModalityIndex = this.ModalityIndex,
                HarmonicIndex = this.HarmonicIndex,
                OrdinalIndex = this.OrdinalIndex,

                IsFromPreviousBar = this.IsFromPreviousBar, //// 2013/03
                IsGoingToNextBar = this.IsGoingToNextBar,
                InstrumentNumber = this.InstrumentNumber,
                //// Channel = this.Channel,
                Staff = this.Staff,
                Voice = this.Voice
            };
            return mt;
        }

        /// <summary> Accepts properties of the given tone. </summary>
        /// <param name="givenTone">Melodic tone.</param>
        public void SetMelTone(MusicalTone givenTone) {
            Contract.Requires(givenTone != null);
            //// if (givenTone == null) { return false;  }

            this.SetMusicalTone(givenTone);
            this.Pitch = givenTone.Pitch;
            this.ModalityIndex = givenTone.ModalityIndex;
            this.HarmonicIndex = givenTone.HarmonicIndex;
            this.OrdinalIndex = givenTone.OrdinalIndex;

            this.IsFromPreviousBar = givenTone.IsFromPreviousBar; //// 2013/03
            this.IsGoingToNextBar = givenTone.IsGoingToNextBar;
            this.InstrumentNumber = givenTone.InstrumentNumber;
            //// this.Channel = givenTone.Channel;
            this.Staff = givenTone.Staff;
            this.Voice = givenTone.Voice;
        }

        /// <summary>
        /// Sets the pitch.
        /// </summary>
        /// <param name="givenPitch">The given pitch.</param>
        public void SetPitch(MusicalPitch givenPitch) {
            this.Pitch = givenPitch;
        }
        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToShortString() {
            var s = new StringBuilder();
            if (!this.IsEmpty && this.Loudness > 0) {
                s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", this.Pitch);
            }
            else {
                s.Append(MusicalStrike.CPause);
            }

            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            switch (this.ToneType) {
                case MusicalToneType.Rhythmic:
                    s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", this.Loudness > 0 ? MusicalStrike.CBeat : MusicalStrike.CPause);
                    break;
                case MusicalToneType.Melodic:
                    s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", !this.IsEmpty && this.Loudness > 0 ? this.Pitch.ToString() : MusicalStrike.CPause);
                    break;
                case MusicalToneType.Empty:
                    break;
            }

            //// for (byte level = 2; level <= this.Duration; level++) { s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}", MusicalStrike.CRepeat); } 
            //// s.Append("(" + Duration.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat));
            //// s.Append("<" + Loudness.ToString(System.Globalization.CultureInfo.CurrentCulture.NumberFormat) + ">");
            s.AppendFormat("<{0}> ", this.Duration);
            //// s.Append(base.ToString()); 
            return s.ToString();
        }
        #endregion
    }
}
