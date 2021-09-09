// <copyright file="LineStatus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Models;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Support;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Line Status.
    /// </summary>
    public sealed class LineStatus : ICloneable
    {
        #region Fields
        /// <summary>
        /// The rhythmic structure
        /// </summary>
        private RhythmicStructure rhythmicStructure;

        /// <summary>
        /// The melodic function
        /// </summary>
        private MelodicFunction melodicFunction;

        /// <summary>
        /// The octave
        /// </summary>
        private MusicalOctave octave;

        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the LineStatus class. </summary>
        public LineStatus() {
            this.Instrument = new MusicalInstrument(MidiMelodicInstrument.None);
            this.Priority = (byte)ProgressPriority.P6;
            this.Octave = MusicalOctave.OneLine;
            this.Loudness = MusicalLoudness.MeanLoudness;
            this.MelodicShape = MelodicShape.None;
            this.LineType = MusicalLineType.None;
            this.MelodicFunction = MelodicFunction.None;
            this.LocalPurpose = LinePurpose.None;
            this.MelodicPlan = new MelodicPlan();
            //// 2016/10 this.PlannedTones = new MusicalStrikeCollection();
            this.RhythmicFace = new RhythmicFace() { Name = "Empty" };
            this.MelodicFace = new MelodicFace() { Name = "Empty" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStatus" /> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public LineStatus(int givenBar) { //// int givenLine
            this.BarNumber = givenBar;
            //// this.LineIndex = givenLine;
            this.Instrument = new MusicalInstrument(MidiMelodicInstrument.None);
            this.MelodicPlan = new MelodicPlan();
            //// 2016/10 this.PlannedTones = new MusicalStrikeCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStatus"/> class.
        /// </summary>
        /// <param name="xelement">The element.</param>
        /// <param name="givenHeader">The given header.</param>
        public LineStatus(XElement xelement, MusicalHeader givenHeader) {
            this.MelodicPlan = new MelodicPlan();
            this.SetXElement(xelement, givenHeader);
            this.BarNumber = XmlSupport.ReadIntegerAttribute(xelement.Attribute("Bar"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStatus"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLineType">Type of the given line.</param>
        /// <param name="givenInstrument">The given instrument.</param>
        /// <param name="givenPurpose">The given purpose.</param>
        /// <param name="givenChannel">The given channel.</param>
        public LineStatus(
            int givenBar,
            MusicalLineType givenLineType,
            MusicalInstrument givenInstrument,
            LinePurpose givenPurpose,
            MidiChannel givenChannel) {
            this.BarNumber = givenBar;
            this.LineType = givenLineType;
            this.Instrument = givenInstrument;
            this.LocalPurpose = givenPurpose;
        }

        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the X element.
        /// </summary>
        /// <returns> Returns value. </returns>
        public XElement GetXElement {
            get {
                var xstatus = new XElement("Status", null);
                xstatus.Add(new XAttribute("Bar", this.BarNumber));

                xstatus.Add(new XAttribute("Octave", this.Octave));
                xstatus.Add(new XAttribute("BandType", this.BandType));
                xstatus.Add(new XAttribute("LineType", this.LineType));
                xstatus.Add(new XAttribute("MelodicFunction", this.MelodicFunction));
                xstatus.Add(new XAttribute("MelodicShape", this.MelodicShape));
                xstatus.Add(new XAttribute("Loudness", this.Loudness));
                xstatus.Add(new XAttribute("LocalPurpose", this.LocalPurpose));

                var instr = this.Instrument ?? new MusicalInstrument(MidiMelodicInstrument.None);
                xstatus.Add(instr.GetXElement);

                var unit = this.OrchestraUnit;
                xstatus.Add(unit != null
                    ? new XAttribute("OrchestraUnit", unit.Name ?? string.Empty)
                    : new XAttribute("OrchestraUnit", string.Empty));

                xstatus.Add(this.RhythmicStructure != null
                    ? new XAttribute("Rhythm", this.RhythmicStructure.GetStructuralCode)
                    : new XAttribute("Rhythm", string.Empty));

                if (this.MelodicStructure != null) {
                    var sc = this.MelodicStructure?.GetStructuralCode; //// 2019/02
                    if (sc != null && this.MelodicStructure?.GSystem != null) {
                        xstatus.Add(new XAttribute("Melody", sc));
                        xstatus.Add(new XAttribute("MelodicDegree", this.MelodicStructure.GSystem.Degree));
                        xstatus.Add(new XAttribute("MelodicOrder", this.MelodicStructure.GSystem.Order));
                    }
                }
                else {
                    xstatus.Add(new XAttribute("Melody", string.Empty));
                }

                xstatus.Add(new XAttribute("Staff", this.Staff));
                xstatus.Add(new XAttribute("Voice", this.Voice));

                if (this.RhythmicFace != null) {
                    xstatus.Add(this.RhythmicFace.GetXElement);
                }

                if (this.MelodicFace != null) {
                    xstatus.Add(this.MelodicFace.GetXElement);
                }

                return xstatus;
            }
        }
        
        /// <summary>
        /// Gets the get tooltip.
        /// </summary>
        /// <value>
        /// The get tooltip.
        /// </value>
        public string GetTooltip {
            get {
                var instr = this.Instrument ?? new MusicalInstrument(MidiMelodicInstrument.None);

                var s = new StringBuilder();
                ////  "Bar", this.BarNumber 
                //// LocalizedMusic.String(this.Purpose.ToString()), 
                s.AppendFormat(
                            "{0} {1} \n",
                            LocalizedMusic.String(this.LineType.ToString()),
                            LocalizedMusic.String("Loud" + ((int)this.Loudness).ToString()));
                s.AppendFormat("{0}\n", LocalizedMusic.String(instr.ToString()));

                //// LocalizedMusic.String("Band" + ((int)this.BandType).ToString()), 
                s.AppendFormat(
                            "{0} \n",
                            LocalizedMusic.String("Octave" + ((int)this.Octave).ToString()));

                if (this.RhythmicStructure != null) {
                    s.AppendFormat("{0}\n", this.RhythmicStructure.ElementSchema);
                }

                s.AppendFormat(
                            "{0} {1} ",
                            LocalizedMusic.String("MelodicFunction" + ((int)this.MelodicFunction).ToString()),
                            LocalizedMusic.String("MelodicShape" + ((int)this.MelodicShape).ToString()));
                if (this.MelodicStructure?.GSystem != null) {
                    var sc = this.MelodicStructure.ElementSchema;
                    if (sc != null) {
                        s.AppendFormat("({0}/{1}) ", this.MelodicStructure.GSystem.Degree, this.MelodicStructure.GSystem.Order);
                        s.AppendFormat("{0} ", sc);
                    }
                }

                return s.ToString();
            }
        }

        #endregion

        #region Properties - Plan (Energy status)

        /// <summary> Gets or sets the rhythmic face. </summary>
        /// <value> The rhythmic face. </value>
        public RhythmicFace RhythmicFace { get; set; }

        /// <summary> Gets or sets the melodic face. </summary>
        /// <value> The melodic face. </value>
        public MelodicFace MelodicFace { get; set; }

        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalSystem System { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        //// [UsedImplicitly]
        public int BarNumber { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the original element - for the dependent elements.
        /// For now elements containing the same tones.
        /// </summary>
        /// <value>
        /// The original element.
        /// </value>
        public MusicalPoint OriginalMelodicPoint { get; set; }

        /// <summary>
        /// Gets or sets the original rhythmic element.
        /// </summary>
        /// <value>
        /// The original rhythmic element.
        /// </value>
        public MusicalPoint OriginalRhythmicPoint { get; set; }
        #endregion

        #region Line Properties

        /// <summary>
        /// Gets or sets current type of line.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalLineType LineType { get; set; }

        /// <summary>
        /// Gets or sets the melodic function.
        /// </summary>
        /// <value>
        /// The melodic function.
        /// </value>
        public MelodicFunction MelodicFunction {
            get => this.melodicFunction;

            set {
                this.melodicFunction = value;
                switch (value) {
                    case MelodicFunction.MelodicMotion:
                        this.Priority = (byte)ProgressPriority.P1;
                        this.MelodicGenus = MelodicGenus.Melodic;
                        break;
                    case MelodicFunction.HarmonicMotion:
                        this.Priority = (byte)ProgressPriority.P2;
                        this.MelodicGenus = MelodicGenus.Harmonic;
                        break;
                    case MelodicFunction.MelodicBass:
                        this.Priority = (byte)ProgressPriority.P3;
                        this.MelodicGenus = MelodicGenus.Melodic;
                        break;
                    case MelodicFunction.HarmonicBass:
                        this.Priority = (byte)ProgressPriority.P4;
                        this.MelodicGenus = MelodicGenus.Harmonic;
                        break;
                    case MelodicFunction.MelodicFilling:
                        this.Priority = (byte)ProgressPriority.P5;
                        this.MelodicGenus = MelodicGenus.Melodic;
                        break;
                    case MelodicFunction.HarmonicFilling:
                        this.Priority = (byte)ProgressPriority.P6;
                        this.MelodicGenus = MelodicGenus.Harmonic;
                        break;
                    default:
                        this.Priority = (byte)ProgressPriority.P6;
                        break;
                }
            }
        }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MelodicGenus MelodicGenus { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is melodic.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is melodic; otherwise, <c>false</c>.
        /// </value>
        public bool IsMelodic => this.LineType == MusicalLineType.Melodic;

        /// <summary>
        /// Gets a value indicating whether this instance is rhythmic.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is rhythmic; otherwise, <c>false</c>.
        /// </value>
        public bool IsRhythmic => this.LineType == MusicalLineType.Rhythmic;

        /// <summary> Gets a value indicating whether harmonic tones. </summary>
        /// <value> Harmonic tones. </value>
        public bool IsHarmonic => this.MelodicGenus == MelodicGenus.Harmonic;

        /// <summary> Gets a value indicating whether harmonic bass. </summary>
        /// <value> Roots and harmonic tones. </value>
        public bool IsHarmonicBass => this.MelodicFunction == MelodicFunction.HarmonicBass;

        /// <summary>
        /// Gets a value indicating whether this instance is filling.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public bool IsFilling => (this.MelodicFunction == MelodicFunction.HarmonicFilling)
                                 || (this.MelodicFunction == MelodicFunction.MelodicFilling);

        #endregion

        #region Staff Status

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public byte Staff { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public byte Voice { get; set; }

        #endregion

        #region Other properties
        /// <summary> Gets or sets purpose of the line. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public LinePurpose LocalPurpose { get; set; }

        /// <summary> Gets or sets priority of generation. </summary>
        /// <value> Property description. </value>
        public byte? Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is melodic original.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is melodic original; otherwise, <c>false</c>.
        /// </value>
        public bool IsMelodicOriginal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rhythmic original.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rhythmic original; otherwise, <c>false</c>.
        /// </value>
        public bool IsRhythmicOriginal { get; set; }
        #endregion

        #region Rhythmic Status
        /// <summary>
        /// Gets or sets the rhythmic motive number.
        /// </summary>
        /// <value>
        /// The rhythmic motive number.
        /// </value>
        public int? RhythmicMotiveNumber { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic motive start bar.
        /// </summary>
        /// <value>
        /// The rhythmic motive start bar.
        /// </value>
        public int RhythmicMotiveStartBar { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic motive.
        /// </summary>
        /// <value>
        /// The rhythmic motive.
        /// </value>
        public RhythmicMotive RhythmicMotive { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic structure.
        /// </summary>
        /// <value>
        /// The rhythmic structure.
        /// </value>
        public RhythmicStructure RhythmicStructure {
            get => this.rhythmicStructure;

            set {
                this.rhythmicStructure = value;
                this.SetRhythmicFaceFromStructure(this.rhythmicStructure);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        public bool HasContent => this.LocalPurpose != LinePurpose.None && this.LocalPurpose != LinePurpose.Mute;

        #endregion

        #region Melodic Status
        /// <summary>
        /// Gets or sets the melodic motive number.
        /// </summary>
        /// <value>
        /// The melodic motive number.
        /// </value>
        public int? MelodicMotiveNumber { get; set; }

        /// <summary>
        /// Gets or sets the melodic motive start bar.
        /// </summary>
        /// <value>
        /// The melodic motive start bar.
        /// </value>
        public int MelodicMotiveStartBar { get; set; }

        /// <summary>
        /// Gets or sets the melodic structure.
        /// </summary>
        /// <value>
        /// The melodic structure.
        /// </value>
        public MelodicStructure MelodicStructure { get; set; }

        /// <summary>
        /// Gets or sets the melodic shape.
        /// </summary>
        /// <value>
        /// The melodic shape.
        /// </value>
        public MelodicShape MelodicShape { get; set; }

        /// <summary>
        /// Gets or sets the melodic motive.
        /// </summary>
        /// <value>
        /// The melodic motive.
        /// </value>
        public MelodicMotive MelodicMotive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has melodic motive.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has melodic motive; otherwise, <c>false</c>.
        /// </value>
        public bool HasMelodicMotive { get; set; }

        #endregion

        #region Instrument Status

        /// <summary>
        /// Gets or sets class of melodic part.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalInstrument Instrument { get; set; }

        /// <summary>
        /// Gets or sets the instrument boost.
        /// </summary>
        /// <value>
        /// The instrument boost.
        /// </value>
        public OrchestraUnit OrchestraUnit { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has instrument.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has instrument; otherwise, <c>false</c>.
        /// </value>
        public bool HasInstrument => this.Instrument != null && this.Instrument.Number != (byte)MidiMelodicInstrument.None;

        #endregion

        #region Octave Status
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalOctave Octave {
            get => this.octave;

            set {
                this.octave = value;
                this.BandType = MusicalProperties.BandTypeFromOctave(this.Octave);
            }
        }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalBand BandType { get; set; }
        #endregion

        #region Loudness Status
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness Loudness { get; set; }
        #endregion

        #region Tonality Status
        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }
        #endregion

        #region Properties for composition

        /// <summary> Gets or sets current melodic interval. </summary>
        /// <value> Property description. </value>
        /// [XmlIgnore]
        public MelodicInterval CurrentMelInterval { get; set; }

        /// <summary>
        /// Gets or sets the melodic variety.
        /// </summary>
        /// <value>
        /// The melodic variety.
        /// </value>
        public IMusicalVariety MelodicVariety { get; set; }
        #endregion

        #region Other Properties
        ///// Old orchestration by sections   public OrchestraVoice CurrentOrchestraVoice { get; set; }

        /// <summary> Gets or sets the melodic plan. </summary>
        /// <value> The melodic plan. </value>
        public MelodicPlan MelodicPlan { get; set; }
        #endregion

        #region Flags
        /// <summary> Gets a value indicating whether Part classification according to current status. </summary>
        /// <value> General musical property.</value>
        public bool IsMelodicalNature => this.LineType == MusicalLineType.Melodic;

        #endregion

        #region Public methods - Set Xml

        /// <summary>
        /// Sets the X element.
        /// </summary>
        /// <param name="xelement">The element.</param>
        /// <param name="givenHeader">The given header.</param>
        public void SetXElement(XElement xelement, MusicalHeader givenHeader) {
            Contract.Requires(xelement != null);

            //// var barNumber = LibSupport.ReadIntegerAttribute(xelement.Attribute("Bar"));

            //// Not all attributes are contained in xelement, only the differences!
            var attribute = xelement.Attribute("Octave");
            if (attribute != null) {
                this.Octave = DataEnums.ReadAttributeMusicalOctave(attribute);
            }

            attribute = xelement.Attribute("LineType");
            if (attribute != null) {
                this.LineType = DataEnums.ReadAttributeMusicalLineType(attribute);
            }

            attribute = xelement.Attribute("MelodicFunction");
            if (attribute != null) {
                this.MelodicFunction = DataEnums.ReadAttributeMelodicFunction(attribute);
            }

            attribute = xelement.Attribute("MelodicShape");
            if (attribute != null) {
                this.MelodicShape = DataEnums.ReadAttributeMelodicShape(attribute);
            }

            attribute = xelement.Attribute("Loudness");
            if (attribute != null) {
                this.Loudness = DataEnums.ReadAttributeMusicalLoudness(attribute);
            }

            attribute = xelement.Attribute("LocalPurpose");
            if (attribute != null) {
                this.LocalPurpose = DataEnums.ReadAttributeLinePurpose(attribute);
            }

            var xinstrument = xelement.Element("Instrument");
            if (xinstrument != null) {
                this.Instrument = new MusicalInstrument(xinstrument); //// : null;
                //// 2016 DataEnums.ReadAttributeMidiMelodicInstrument(xelement.Attribute("MelodicInstrument"));
            }

            var xunit = xelement.Attribute("OrchestraUnit");
            if (xunit != null) {
                this.OrchestraUnit = PortCatalogs.Singleton.GetOrchestraUnit((string)xunit);
                //// new OrchestraUnit(xunit); //// : null;
            }
            
            attribute = xelement.Attribute("Rhythm");
            if (attribute != null && givenHeader != null) {
                var structuralCode = XmlSupport.ReadStringAttribute(attribute);
                if (!string.IsNullOrEmpty(structuralCode)) {
                    var rs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, givenHeader.System.RhythmicOrder);
                    this.RhythmicStructure = new RhythmicStructure(rs, structuralCode);
                }
                else {
                    this.RhythmicStructure = null;
                }
            }
            else {
                this.RhythmicStructure = null;
            }

            attribute = xelement.Attribute("Melody");
            if (attribute != null) {
                var structuralCode = XmlSupport.ReadStringAttribute(attribute);
                if (!string.IsNullOrEmpty(structuralCode)) {
                    var melodicDegree = XmlSupport.ReadByteAttribute(xelement.Attribute("MelodicDegree"));
                    var melodicOrder = XmlSupport.ReadByteAttribute(xelement.Attribute("MelodicOrder"));
                    var ms = new MelodicSystem(melodicDegree, melodicOrder);
                    this.MelodicStructure = new MelodicStructure(ms, structuralCode);
                }
                else {
                    this.MelodicStructure = null;
                }
            }
            else {
                this.MelodicStructure = null;
            }

            this.Staff = XmlSupport.ReadByteAttribute(xelement.Attribute("Staff"));
            this.Voice = XmlSupport.ReadByteAttribute(xelement.Attribute("Voice"));

            var xrhyface = xelement.Element("RhythmicFace");
            if (xrhyface != null) {
                this.RhythmicFace = new RhythmicFace(xrhyface);
            }

            var xmelface = xelement.Element("MelodicFace");
            if (xmelface != null) {
                this.MelodicFace = new MelodicFace(xmelface);
            }
        }

        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the LineStatus object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var tmc = new LineStatus(this.BarNumber) { //// , this.LineIndex
                Instrument = this.Instrument,
                OrchestraUnit = this.OrchestraUnit,
                Octave = this.Octave,
                Loudness = this.Loudness,
                //// RhythmicTension = this.RhythmicTension,
                RhythmicMotive = this.RhythmicMotive,
                RhythmicMotiveNumber = this.RhythmicMotiveNumber,
                RhythmicMotiveStartBar = this.RhythmicMotiveStartBar,
                RhythmicStructure = this.RhythmicStructure,
                MelodicShape = this.MelodicShape,
                MelodicMotive = this.MelodicMotive,
                MelodicMotiveNumber = this.MelodicMotiveNumber,
                MelodicMotiveStartBar = this.MelodicMotiveStartBar,
                MelodicStructure = this.MelodicStructure,
                MelodicVariety = this.MelodicVariety,
                LineType = this.LineType,
                MelodicFunction = this.MelodicFunction,
                MelodicGenus = this.MelodicGenus,
                Staff = this.Staff,
                Voice = this.Voice,
                Priority = this.Priority,
                LocalPurpose = this.LocalPurpose,
                HasMelodicMotive = this.HasMelodicMotive,
                IsMelodicOriginal = this.IsMelodicOriginal,
                IsRhythmicOriginal = this.IsRhythmicOriginal,
                OriginalMelodicPoint = this.OriginalMelodicPoint,
                OriginalRhythmicPoint = this.OriginalRhythmicPoint
            };

            if (this.MelodicFace != null) {
                tmc.MelodicFace = (MelodicFace)this.MelodicFace.Clone();
            }

            if (this.RhythmicFace != null) {
                tmc.RhythmicFace = (RhythmicFace)this.RhythmicFace.Clone();
            }

            if (tmc.Instrument == null) {
                tmc.Instrument = new MusicalInstrument(MidiMelodicInstrument.None);
            }

            tmc.MelodicPlan = this.MelodicPlan; //// 2019/11

            return tmc;
        }

        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="content">The content.</param>
        /// <returns> Returns value. </returns>
        public bool SetStatus(LineStatus status, EditorContent content) {
            switch (content) {
                case EditorContent.InstrumentAndLoudness:
                    this.Instrument = status.Instrument;
                    this.Loudness = status.Loudness;
                    return true;

                case EditorContent.RhythmicStructure:
                    this.RhythmicStructure = status.RhythmicStructure;
                    return true;
            }

            if (this.LineType == MusicalLineType.Rhythmic) {
                return false;
            }

            switch (content) {
                case EditorContent.MelodicStructure:
                    this.MelodicStructure = status.MelodicStructure;
                    break;

                case EditorContent.OctaveAndBand:
                    this.Octave = status.Octave;
                    //// this.Band = status.Band;
                    break;
                case EditorContent.MelodicFunctionAndShape:
                    this.MelodicFunction = status.MelodicFunction;
                    this.MelodicShape = status.MelodicShape;
                    break;
                default:
                    this.Instrument = status.Instrument;
                    this.OrchestraUnit = status.OrchestraUnit;
                    this.Loudness = status.Loudness;
                    this.Octave = status.Octave;
                    this.MelodicFunction = status.MelodicFunction;
                    this.MelodicShape = status.MelodicShape;
                    this.RhythmicStructure = status.RhythmicStructure;
                    this.MelodicStructure = status.MelodicStructure;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Sets the melodic pattern.
        /// </summary>
        /// <param name="givenVoice">The given voice.</param>
        /// <param name="keepOriginalArrange">if set to <c>true</c> [keep original arrange].</param>
        public void SetMelodicPatternVoice(MelodicPatternVoice givenVoice, bool keepOriginalArrange) {
            var rs1 = givenVoice.RhythmicStructure;
            //// Re-computation from patternVoice.Order to Block.Header.System.RhythmicOrder !!!
            var rs2 = rs1.ConvertToSystem(this.System.RhythmicSystem);
            this.RhythmicStructure = rs2;
            this.MelodicStructure = givenVoice.MelodicStructure;
            this.HasMelodicMotive = true;

            if (keepOriginalArrange) {
                this.Loudness = givenVoice.Loudness;
                this.Instrument = givenVoice.Instrument;
                this.Octave = givenVoice.Octave;
            }

            this.MelodicShape = MelodicShape.Scales;
            this.MelodicGenus = MelodicGenus.Harmonic;
            this.MelodicFunction = MelodicFunction.HarmonicFilling;
        }

        /// <summary>
        /// Sets the melodic pattern.
        /// </summary>
        /// <param name="givenVoice">The given voice.</param>
        /// <param name="keepOriginalArrange">if set to <c>true</c> [keep original arrange].</param>
        public void SetRhythmicPatternVoice(RhythmicPatternVoice givenVoice, bool keepOriginalArrange) {
            var rs1 = givenVoice.RhythmicStructure;
            //// Re-computation from patternVoice.Order to Block.Header.System.RhythmicOrder !!!
            var rs2 = rs1.ConvertToSystem(this.System.RhythmicSystem);
            this.RhythmicStructure = rs2;

            if (keepOriginalArrange) {
                this.Loudness = givenVoice.Loudness;
                this.Instrument = givenVoice.Instrument;
            }
        }

        /// <summary>
        /// Sets the rhythmic face.
        /// </summary>
        /// <param name="rstruct">The rhythmic structure.</param>
        public void SetRhythmicFaceFromStructure(RhythmicStructure rstruct) {
            if (rstruct == null) {
                this.RhythmicFace = new RhythmicFace();
                return;
            }

            rstruct.DetermineBehavior(); //// 2020/10

            var toneLevel = rstruct.ToneLevel; //// (byte)melodicTonesInBar.Count;
            var beatLevel = rstruct.Level; //// (byte)barAllTones.Count;
            var rhythmicTension = (byte)rstruct.FormalBehavior.Variance;

            this.RhythmicFace = new RhythmicFace(rstruct.GetStructuralCode, beatLevel, toneLevel, rhythmicTension);
        }

        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(string.Format(CultureInfo.CurrentCulture, "Bar {0}\t", this.BarNumber));
            //// s.Append(string.Format(CultureInfo.CurrentCulture, "Line {0}\t", this.LineIndex.ToString(CultureInfo.CurrentCulture.NumberFormat)));
            //// s.Append("Type " + this.LineTypeString);
            return s.ToString();
        }
        #endregion

        #region Status changes

        /// <summary>
        /// Gets the change x element.
        /// </summary>
        /// <param name="previousStatus">The previous status.</param>
        /// <returns>Returns value.</returns>
        public XElement GetChangeXElement(LineStatus previousStatus) {
            var xstatus = new XElement("Status", null);
            xstatus.Add(new XAttribute("Bar", this.BarNumber));

            if (!this.Instrument.IsEmpty) {
                if (previousStatus.Instrument == null || previousStatus.Instrument.IsEmpty
                                || this.Instrument.Number != previousStatus.Instrument.Number) {
                    xstatus.Add(this.Instrument.GetXElement);
                }
            }
            else {
                if (this.Instrument.Number != previousStatus.Instrument.Number) {
                    var instr = this.Instrument ?? new MusicalInstrument(MidiMelodicInstrument.None);
                    xstatus.Add(instr.GetXElement);
                }
            }

            if (this.OrchestraUnit != null && this.OrchestraUnit != previousStatus.OrchestraUnit) {
                xstatus.Add(new XAttribute("OrchestraUnit", this.OrchestraUnit.Name));
            }

            if (this.Octave != previousStatus.Octave) {
                xstatus.Add(new XAttribute("Octave", this.Octave));
            }

            if (this.BandType != previousStatus.BandType) {
                xstatus.Add(new XAttribute("BandType", this.BandType));
            }

            if (this.LineType != previousStatus.LineType) {
                xstatus.Add(new XAttribute("LineType", this.LineType));
            }

            if (this.MelodicFunction != previousStatus.MelodicFunction) {
                xstatus.Add(new XAttribute("MelodicFunction", this.MelodicFunction));
            }

            if (this.MelodicShape != previousStatus.MelodicShape) {
                xstatus.Add(new XAttribute("MelodicShape", this.MelodicShape));
            }

            if (this.Loudness != previousStatus.Loudness) {
                xstatus.Add(new XAttribute("Loudness", this.Loudness));
            }

            if (this.LocalPurpose != previousStatus.LocalPurpose) {
                xstatus.Add(new XAttribute("LocalPurpose", this.LocalPurpose));
            }

            if (this.RhythmicFace != null) {
                var rfx = this.RhythmicFace.GetXElement;
                if (rfx != previousStatus.RhythmicFace.GetXElement) {
                    xstatus.Add(rfx);
                }
            }

            if (this.MelodicFace != null) {
                var mfx = this.MelodicFace.GetXElement;
                if (mfx != previousStatus.MelodicFace.GetXElement) {
                    xstatus.Add(mfx);
                }
            }

            xstatus.Add(this.RhythmicStructure?.GetStructuralCode != null
                ? new XAttribute("Rhythm", this.RhythmicStructure.GetStructuralCode)
                : new XAttribute("Rhythm", string.Empty));

            if (this.MelodicStructure?.GetStructuralCode != null) {
                xstatus.Add(new XAttribute("Melody", this.MelodicStructure.GetStructuralCode));
                xstatus.Add(new XAttribute("MelodicDegree", this.MelodicStructure.GSystem.Degree));
                xstatus.Add(new XAttribute("MelodicOrder", this.MelodicStructure.GSystem.Order));
            }
            else {
                xstatus.Add(new XAttribute("Melody", string.Empty));
            }

            return xstatus;
        }
        #endregion
    }
}
