// <copyright file="MusicalElement.cs" company="Traced-Ideas, Czech republic">
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
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Element.
    /// </summary>
    public class MusicalElement : IAbstractElement
    {
        #region Fields

        /// <summary> Complete list of all tones in musical part. </summary>
        private ToneCollection musicalTones;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalElement"/> class. Unused.
        /// </summary>
        public MusicalElement() {
            this.Status = new LineStatus();
            this.IsLive = true;
            this.IsComposed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalElement" /> class.
        /// </summary>
        /// <param name="givenStatus">The given status.</param>
        /// <param name="givenPreviousElement">The given previous element.</param>
        public MusicalElement(LineStatus givenStatus, MusicalElement givenPreviousElement) {
            this.Status = (LineStatus)givenStatus.Clone(); //// !!!!
            this.PreviousElement = givenPreviousElement;
            this.IsLive = true;
            this.IsComposed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalElement"/> class.
        /// </summary>
        /// <param name="xelement">The element.</param>
        /// <param name="header">The header.</param>
        public MusicalElement(XElement xelement, MusicalHeader header) : this() {
            Contract.Requires(xelement != null);
            if (xelement == null) {
                return;
            }

            this.Status = new LineStatus(xelement.Element("Status"), header);
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xelement = new XElement("Element");
                if (this.Bar == null || this.Line == null) {
                    return xelement;
                }

                xelement.Add(new XAttribute("Bar", this.Bar.BarNumber));
                xelement.Add(new XAttribute("LineIndex", this.Line.LineIndex));

                var xstatus = this.Status.GetXElement;
                xelement.Add(xstatus);

                if (this.Tones != null && this.Tones.Count > 0) {
                    var xtones = this.Tones.GetXElement;
                    xelement.Add(xtones);
                }

                return xelement;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
        public IAbstractBar Bar { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public IAbstractLine Line { get; set; }

        /// <summary> Gets the musical line. </summary>
        /// <value> The musical line. </value>
        public MusicalLine MusicalLine => (MusicalLine)this.Line;

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>
        /// The point.
        /// </value>
        public MusicalPoint Point {
            get {
                if (this.Line == null || this.Bar == null) {
                    return new MusicalPoint(0, 0);
                }

                return new MusicalPoint(this.Line.LineIndex, this.Bar.BarNumber);
            }
        }

        /// <summary>
        /// Gets or sets Current Musical Line Status.
        /// </summary>
        public LineStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mute.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this instance is mute; otherwise, <c>false</c>.
        /// </value>
        public bool IsLive { get; set; }  //// private set

        /// <summary>
        /// Gets or sets a value indicating whether this instance is composed.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is composed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComposed { get; set; }  //// private set

        /// <summary>
        /// Gets or sets a value indicating whether this instance is finished = composed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished { get; set; }  //// private set

        /// <summary>
        /// Gets or sets the previous element (bar of line).
        /// </summary>
        /// <value>
        /// The previous element.
        /// </value>
        public MusicalElement PreviousElement { get; set; }

        /// <summary>
        /// Gets or sets the previous element (bar of line).
        /// </summary>
        /// <value>
        /// The previous element (bar of line).
        /// </value>
        public MusicalElement NextElement { get; set; }

        /// <summary>
        /// Gets or sets the musical tones.
        /// </summary>
        /// <value>
        /// The musical tones.
        /// </value>
        [XmlIgnore]
        public ToneCollection Tones {
            get {
                Contract.Ensures(Contract.Result<ToneCollection>() != null);
                return this.musicalTones ?? (this.musicalTones = new ToneCollection());
            }

            set => this.musicalTones = value;
        }

        /// <summary> Gets last but one melodic tone. </summary>
        /// <value> Property description. </value>
        /// [XmlIgnore]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public MusicalTone PreviousBarFirstTone { get; private set; }

        /// <summary>
        /// Gets or sets the tone packets - detail info about all tones in the element - for tracing;
        /// </summary>
        /// <value>
        /// Returns value.
        /// </value>
        public TonePacket TonePacket { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>
        /// The tool tip.
        /// </value>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        public bool HasContent { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public EditorContent ContentType { get; set; }
        #endregion

        #region Plan
        /// <summary> Gets rhythmic structure. </summary>
        /// <returns> The rhythmic structure. </returns>
        public RhythmicStructure GetFaceRhythmicStructure {
            get {
                if (this.Status?.RhythmicFace == null) {
                    return null;
                }

                var structuralCode = this.Status.RhythmicFace.StructuralCode;
                RhythmicSystem rhythmicSystem = this.Bar.Header.System.RhythmicSystem;
                var rs = new RhythmicStructure(rhythmicSystem, structuralCode);
                return rs;
            }
        }
        #endregion

        #region Identifiers
        /// <summary>
        /// Gets the main identifiers.
        /// </summary>
        /// <value>
        /// The main identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers {
            get {
                var items = new List<KeyValuePair>();

                var item = new KeyValuePair("Line type", this.Line.FirstStatus.LineType.ToString());
                items.Add(item);

                item = new KeyValuePair("Line number", this.Line.LineNumber.ToString());
                items.Add(item);

                item = new KeyValuePair("Bar number", this.Bar.BarNumber.ToString());
                items.Add(item);

                var status = this.Status;
                if (status == null) {
                    return items;
                }

                item = new KeyValuePair(
                                    LocalizedMusic.String("LocalPurpose"),
                                    LocalizedMusic.String(status.LocalPurpose.ToString()));
                items.Add(item);

                item = new KeyValuePair("Instrument", status.Instrument.ToString());
                items.Add(item);

                //// 2021/01 - instrument boost 
                item = new KeyValuePair("OrchestraUnit", status.OrchestraUnit?.Name ?? string.Empty);
                items.Add(item);

                if (status.RhythmicStructure != null) {
                    item = new KeyValuePair(
                                    LocalizedMusic.String("Rhythmic structure"), status.RhythmicStructure.ElementString());
                    items.Add(item);

                    if (status.RhythmicStructure.Level == 1 && status.RhythmicStructure.ToneLevel == 0) {
                        return items;
                    }
                }

                item = new KeyValuePair(LocalizedMusic.String("Loudness"), LocalizedMusic.String("Loud" + ((int)status.Loudness).ToString()));
                items.Add(item);

                if (status.IsMelodic) {
                    item = new KeyValuePair(LocalizedMusic.String("Octave"), LocalizedMusic.String("Octave" + ((int)status.Octave).ToString()));
                    items.Add(item);

                    item = new KeyValuePair(LocalizedMusic.String("Melodic shape"), LocalizedMusic.String("MelodicShape" + ((int)status.MelodicShape).ToString()));
                    items.Add(item);

                    item = new KeyValuePair(LocalizedMusic.String("Melodic function"), LocalizedMusic.String("MelodicFunction" + ((int)status.MelodicFunction).ToString()));
                    items.Add(item);

                    if (status.MelodicStructure != null) {
                        item = new KeyValuePair(
                                    LocalizedMusic.String("Melodic structure"), status.MelodicStructure.ElementString());
                        items.Add(item);
                    }

                    item = new KeyValuePair(LocalizedMusic.String("Band"), LocalizedMusic.String("Band" + ((int)status.BandType).ToString()));
                    items.Add(item);

                    item = new KeyValuePair(LocalizedMusic.String("Priority"), status.Priority.ToString());
                    items.Add(item);

                    var tones = this.Tones.ToString();
                    item = new KeyValuePair(LocalizedMusic.String("Tones"), tones);
                    items.Add(item);

                    var rf = this.Status.RhythmicFace;
                    if (rf != null) {
                        item = new KeyValuePair("Rhythmic face", rf.Name);
                        items.Add(item);
                        item = new KeyValuePair("Beat level", rf.BeatLevel.ToString(CultureInfo.InvariantCulture));
                        items.Add(item);
                        item = new KeyValuePair("Tone level", rf.ToneLevel.ToString(CultureInfo.InvariantCulture));
                        items.Add(item);
                        item = new KeyValuePair("Rhythmic tension", rf.RhythmicTension.ToString(CultureInfo.InvariantCulture));
                        items.Add(item);
                        //// item = new KeyValuePair("R-face code", rf.StructuralCode);
                        //// items.Add(item);
                        item = new KeyValuePair("R-face structure", this.GetFaceRhythmicStructure?.ToShortString());
                        items.Add(item);
                    }

                    var mf = this.Status.MelodicFace;
                    if (mf != null) {
                        item = new KeyValuePair("Melodic face", mf.Name);
                        items.Add(item);
                        item = new KeyValuePair("Melodic direction", mf.MelodicDirection.ToString(CultureInfo.InvariantCulture));
                        items.Add(item);
                    }
                }

                return items;
            }
        }

        #endregion

        /// <summary>
        /// Determines whether [is compatible with] [the specified given musical unit].
        /// </summary>
        /// <param name="givenElement">The given element.</param>
        /// <returns>
        ///   <c>true</c> if [is compatible with] [the specified given musical unit]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatibleWith(MusicalElement givenElement) {
            return this.HasContent && givenElement.DisplayText == this.DisplayText;
        }

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalElement object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var element = new MusicalElement {
                Bar = this.Bar,
                Line = this.Line,
                Status = (LineStatus)this.Status.Clone(),
                Tones = this.Tones.Clone(true)
            };

            return element;
        }

        /// <summary>
        /// Sets the tones.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        public void SetTones(ToneCollection givenTones) {
            this.Tones = givenTones;
        }

        /// <summary>
        /// Prepares the tone packets.
        /// </summary>
        public void PrepareTonePacket() {
            this.TonePacket = new TonePacket(this.Tones);
        }

        /// <summary>
        /// Prepares the content.
        /// </summary>
        /// <param name="givenContentType">Type of the given content.</param>
        public void PrepareContent(EditorContent givenContentType) {
            this.DisplayText = string.Empty;
            this.ToolTip = null;
            this.HasContent = false;

            var status = this.Status;
            if (!status.HasContent) {
                return;
            }

            this.ContentType = givenContentType;
            switch (givenContentType) {
                case EditorContent.Raster: {                        
                        this.DisplayText = this.Point.ToString();
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.Cell: {
                        string s = string.Empty;
                        if (status?.RhythmicFace?.ToneLevel > 0 || status.MelodicFunction != MelodicFunction.None) { //// || status?.MelodicShape != MelodicShape.None
                            s = string.Format("{0} {1}\n{2}", status.RhythmicFace?.ToneLevel, status?.MelodicFunction, status?.MelodicShape);
                        }

                        this.DisplayText = s; 
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.InstrumentAndLoudness: {
                        this.DisplayText = string.Format("{0}\n{1}", status.Instrument,  status.Loudness);
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.OctaveAndBand: {
                        this.DisplayText = string.Format("{0}\n{1}", status.Octave, status.BandType);
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.ToneAndBeatLevel: {
                        this.DisplayText = string.Format("Tone {0}\nBeat {1}", status.RhythmicFace?.ToneLevel, status.RhythmicFace?.BeatLevel);
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.MelodicFunctionAndShape: {
                        this.DisplayText = string.Format("{0}\n{1}", status.MelodicFunction, status.MelodicShape);
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.RhythmicStructure: {
                        this.DisplayText = status.RhythmicStructure != null ? status.RhythmicStructure.ElementSchema : string.Empty;
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.MelodicStructure: {
                        this.DisplayText = status.MelodicStructure != null ? status.MelodicStructure.ElementSchema : string.Empty;
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.RhythmicMotive: {
                        this.DisplayText = status.RhythmicMotive != null ? status.RhythmicMotive.Name : string.Empty;
                        this.ToolTip = status.GetTooltip;
                        break;
                    }

                case EditorContent.MelodicMotive: {
                        this.DisplayText = status.MelodicMotive != null ? status.MelodicMotive.Name : string.Empty;
                        this.ToolTip = status.GetTooltip;
                        break;
                    }
            }

            this.HasContent = true;
        }

        /// <summary>
        /// Sets the melodic pattern.
        /// </summary>
        /// <param name="givenPattern">The given pattern.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="keepOriginalArrange">if set to <c>true</c> [keep original arrange].</param>
        [UsedImplicitly]
        public void EditorSetMelodicPattern(MelodicPattern givenPattern, byte givenLine, bool keepOriginalArrange) {
            if (givenLine >= givenPattern.Voices.Count) {
                return;
            }

            var voice = givenPattern.Voices[givenLine];
            this.Tones = voice.Tones;
            this.Status.System = this.Bar.Header.System;
            this.Status.SetMelodicPatternVoice(voice, keepOriginalArrange);
        }

        /// <summary>
        /// Sets the melodic pattern.
        /// </summary>
        /// <param name="givenPattern">The given pattern.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="keepOriginalArrange">if set to <c>true</c> [keep original arrange].</param>
        [UsedImplicitly]
        public void EditorSetRhythmicPattern(RhythmicPattern givenPattern, byte givenLine, bool keepOriginalArrange) {
            if (givenLine >= givenPattern.Voices.Count) {
                return;
            }

            var voice = givenPattern.Voices[givenLine];
            this.Tones = voice.Tones;
            this.Status.System = this.Bar.Header.System;
            this.Status.SetRhythmicPatternVoice(voice, keepOriginalArrange);
        }

        /// <summary>
        /// Makes the status from tones.
        /// </summary>
        public void SetElementStatusFromTones() {
            var lineStatus = this.MusicalLine.FirstStatus;
            if (this.Status == null || lineStatus == null) {
                return;
            }

            //// Real line properties
            this.Status.BarNumber = this.Bar.BarNumber; //// 2018/12
            this.Status.MelodicVariety = lineStatus.MelodicVariety;
            //// this.Status.Channel = lineStatus.Channel;
            this.Status.Staff = lineStatus.Staff;
            this.Status.Voice = lineStatus.Voice;
            //// this.Status.HarmonicModalization = lineStatus.HarmonicModalization;

            //// Problem of repeated reading of MIF file .... ***
            if (this.Status.LineType == MusicalLineType.None && lineStatus.LineType != MusicalLineType.None) {
                this.Status.LineType = lineStatus.LineType;
            }

            if (this.Status.LocalPurpose == LinePurpose.None && lineStatus.LocalPurpose != LinePurpose.None) {
                this.Status.LocalPurpose = lineStatus.LocalPurpose;   //// 2018/12
            }
            //// ***

            var barAllTones = this.Tones;
            //// var isMelodic = this.Status.IsMelodic;
            //// var barTones = isMelodic ? this.SingleMelodicTones() : null;
            var melodicTonesInBar = this.SingleMelodicTones();

            if (barAllTones == null || barAllTones.Count == 0) {
                return;
            }

            //// *** Loudness ***
            //// 2020/10 var loudness = melodicTonesInBar.MeanLoudness;
            var loudness = barAllTones.MeanLoudness;
            this.Status.Loudness = loudness;

            //// *** Rhythmic tension *** 
            var rstruct = barAllTones.DetermineRhythmicStructure(this.Bar.Header.System.RhythmicOrder);
            this.Status.RhythmicStructure = rstruct;

            //// Melodic structure in bar - from tones
            var isMelodic = this.Status.IsMelodic;
            if (isMelodic) {
                if (melodicTonesInBar != null && melodicTonesInBar.Any()) {
                    //// lastMelodicTone
                    var mstruct = melodicTonesInBar.DetermineMelodicStructure(null, this.Bar.HarmonicBar);
                    if (mstruct == null) {
                        var system = new MelodicSystem(1, 1);
                        mstruct = new MelodicStructure(system, 1);
                    }

                    this.Status.MelodicStructure = mstruct;
                }
            }

            //// *** StaffChange *** 
            var firstTone = barAllTones.FirstOrDefault();
            if (firstTone != null) {
                this.Status.Staff = firstTone.Staff;
                this.Status.Voice = firstTone.Voice;
            }

            //// 2016/06
            //// *** Instrument *** 
            if (this.Status.Instrument.IsEmpty) {
                if (this.Status.LineType == MusicalLineType.Melodic) {
                    this.Status.Instrument = new MusicalInstrument(barAllTones.FirstMelodicInstrument);
                }

                if (this.Status.LineType == MusicalLineType.Rhythmic) {
                    this.Status.Instrument = new MusicalInstrument(barAllTones.FirstRhythmicInstrument);
                }
            }

            //// *** Octaves *** 
            this.Status.Octave = melodicTonesInBar.MeanOctave;
            this.Status.BandType = MusicalProperties.BandTypeFromOctave(this.Status.Octave);

            //// 2016/10
            if (this.Tones.Count > 0) {
                this.Status.MelodicPlan = new MelodicPlan(this.Tones);
            }

            this.Status.MelodicFunction = melodicTonesInBar.DetermineMelodicType(this.Bar.HarmonicBar);
            this.Status.MelodicShape = MelodicShape.Scales;

            //// 2020/09 Plan
            short melodicDirection = 0;
            var first = melodicTonesInBar.FirstOrDefault();
            var last = melodicTonesInBar.LastOrDefault();
            if (first != null && last != null) {
                melodicDirection = (short)(last.Pitch.SystemAltitude - first.Pitch.SystemAltitude);
            }

            if (rstruct != null) {
                this.Status.SetRhythmicFaceFromStructure(rstruct);
                this.Status.MelodicFace = new MelodicFace(melodicTonesInBar.Count, melodicDirection);
            }
        }

        /// <summary>
        /// List of tones in one bar of musical part.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalToneCollection SingleMelodicTones() {
            var tones = from mT in this.Tones
                        where mT.ToneType == MusicalToneType.Melodic && !mT.IsPause
                        orderby mT.BitFrom
                        select mT;
            var mtc = new MusicalToneCollection();
            //// Only one tone of each cluster
            var lastBitFrom = -1;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var tone in tones) {
                MusicalTone mtone = tone as MusicalTone;
                if (mtone == null) {
                    continue;
                }

                if (mtone.BitFrom == lastBitFrom) {
                    continue;
                }

                mtc.Add(mtone);
                lastBitFrom = mtone.BitFrom;
            }

            return mtc;
        }

        /// <summary>
        /// List of tones in one bar of musical part.
        /// </summary>
        /// <param name="barNumbersBack">The bar numbers back.</param>
        /// <param name="barNumbersForward">The bar numbers forward.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalToneCollection MelodicTonesAround(int barNumbersBack, int barNumbersForward) {
            var atones = new List<IMusicalTone>();
            MusicalElement element = this;
            for (int stepDown = 0; stepDown <= barNumbersBack && element != null; stepDown++) {
                var mts = this.Tones.Where(x => x != null && x.ToneType == MusicalToneType.Melodic && x.Loudness > 0);
                atones.AddRange(mts);
                element = element.PreviousElement;
            }

            element = this;
            for (int stepUp = 0; stepUp <= barNumbersForward && element != null; stepUp++) {
                var mts = this.Tones.Where(x => x != null && x.ToneType == MusicalToneType.Melodic && x.Loudness > 0);
                atones.AddRange(mts);
                element = element.NextElement;
            }

            var distinctTones = new MusicalToneCollection();
            foreach (var tone in atones) {
                var mt = tone as MusicalTone;
                if (mt?.Pitch == null) {
                    continue;
                }

                var mt1 = mt;
                var ex = from mtone in distinctTones where mtone.Pitch.Element == mt1.Pitch.Element select mtone;
                if (!ex.Any()) {
                    distinctTones.Add(mt);
                }

                if (distinctTones.Count >= 7) {
                    break;
                }
            }

            return distinctTones;
        }

        /// <summary>
        /// Musicals the tone at.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <param name="rhythmicStructure">The rhythmic structure.</param>
        /// <returns> Returns value. </returns>
        public MusicalStrike MusicalToneAt(byte tick, FiguralStructure rhythmicStructure) { //// RhythmicStructure
            if (rhythmicStructure == null) {
                return null;
            }

            var level = rhythmicStructure.LevelOfBit(tick);
            return (level < this.Tones.Count ? this.Tones[level] : null) as MusicalStrike;
        }
        #endregion

        /// <summary>
        /// Composes the tone.
        /// </summary>
        /// <param name="musicalTone">The melodic tone.</param>
        public void ComposeTone(MusicalTone musicalTone) {
            Contract.Requires(musicalTone != null);
            Contract.Requires(this.Status != null);
            //// Contract.Requires(this.Line.CurrentTone != null);

            musicalTone.SetPitch(new MusicalPitch(this.Bar.Header.System.HarmonicSystem));

            //// this.HarmonicModality = null;
            bool singleHarmony = true; //// 2018/10
            var hstruct = this.Bar.HarmonicBar?.PrevailingHarmonicStructure(musicalTone.BitRange, out singleHarmony); //// (musicalBar.Number)
            if (hstruct != null && hstruct.Level == 0) {
                hstruct = null;
            }

            var modalization = MusicalSettings.Singleton.SettingsComposition.HarmonicModalization;

            if (modalization == HarmonicModalizationType.Consecutive) { ////  != HarmonicModalizationType.Forced
                this.Status.HarmonicModality = this.DetermineHarmonicModality(hstruct); //// , this.Status.HarmonicModalization
            }

            if (modalization == HarmonicModalizationType.Forced) { ////  != HarmonicModalizationType.Forced
                this.Status.HarmonicModality = MusicalSettings.Singleton.SettingsComposition.ForcedModality;
            }

            //// harStruct.SetHarModality(hm);
            if (this.Status.HarmonicModality == null) {
                return;
            }

            this.MusicalLine.CurrentTone = musicalTone;
            var melodicVariety = this.MusicalLine.FirstStatus.MelodicVariety; //// Status.MelodicVariety;
            if (melodicVariety == null) {
                return;
            }

            if (!melodicVariety.GeneratePossibilities(
                this.MusicalLine.CurrentTone,
                this.Status.HarmonicModality,
                hstruct,
                this.Status.IsHarmonic && singleHarmony)) {
                return;
            }

            var newMelTone = melodicVariety.OptimalNextMelTone(this.MusicalLine.CurrentTone);
            if (!this.Status.IsMelodic) {
                return;
            }

            musicalTone.SetMelTone(newMelTone);
            this.CompleteMelodicTone(musicalTone);
        }

        #region Public methods - Loudness
        /// <summary>
        /// Determine loudness of the tone.
        /// </summary>
        /// <param name="rhythmicStruct">Rhythmical structure.</param>
        /// <param name="toneRange">Range of tone.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalLoudness DetermineLoudness(FiguralStructure rhythmicStruct, BitRange toneRange) {
            Contract.Requires(rhythmicStruct != null);
            Contract.Requires(toneRange != null);

            if (rhythmicStruct == null) {
                return 0;
            }

            //// if (toneRange == null) { return 0; }
            if (rhythmicStruct.IsPauseStart(toneRange.BitFrom)) {
                return 0;
            }

            var status = this.Status;
            byte loudness;

            if (MusicalSettings.Singleton.SettingsComposition.CorrectLoudness) {
                loudness = this.CorrectLoudness(toneRange, (byte)(status != null ? (byte)status.Loudness : 0));
            }
            else {
                loudness = (byte)(status != null ? (byte)status.Loudness : 0);
            }

            if (status != null && (status.IsMelodicalNature && status.MelodicFunction == MelodicFunction.MelodicMotion
                        && MusicalSettings.Singleton.SettingsComposition.HighlightMelodicVoices)) {
                loudness = (byte)Math.Min(loudness + 4, (byte)MusicalLoudness.MaxLoudness);
            }

            return (MusicalLoudness)loudness;
        }

        #endregion

        #region Public methods - Quantities
        /// <summary> Prepares current melodic interval. </summary>
        /// <returns> Returns value. </returns>
        public bool PrepareMelInterval() {
            var line = (MusicalLine)this.Line;
            if ((line.LastTone != null) && (line.CurrentTone != null) &&
                (!line.LastTone.IsEmpty && !line.CurrentTone.IsEmpty)) {
                ////201508 this.ComposedLine.TrackEngine
                this.Status.CurrentMelInterval = new MelodicInterval(this.Bar.Header.System.HarmonicSystem, line.LastTone.Pitch, line.CurrentTone.Pitch);
                return true;
                //// this._MelInterval.SetHarmonicProperties();
            }

            this.Status.CurrentMelInterval = null;
            return false;
        }
        #endregion

        #region Public methods - Rhythmic
        /// <summary>
        /// Fills the with rhythm.
        /// </summary>
        public void FillWithRhythm() {
            switch (this.Status?.LocalPurpose) {
                case LinePurpose.Composed:
                    this.FillWithRequestedRhythm();
                    break;
                case LinePurpose.Fixed:
                    this.FillWithRhythmOfTones(this.Tones);  //// 2016  .MusicalTonesInBar(this.Number)//// 2014/01
                    break;
            }
        }

        /// <summary>
        /// Creates one bar of rhythm in this musical part.
        /// </summary>
        /// <param name="rhythmicStruct">Rhythmical structure.</param>
        /// <param name="givenLoudness">Musical Loudness.</param>
        public void FillWithRhythm(RhythmicStructure rhythmicStruct, MusicalLoudness givenLoudness) {
            Contract.Requires(rhythmicStruct != null);
            Contract.Requires(this.Status != null);

            this.Status.RhythmicStructure = rhythmicStruct; //// !!!!!!!!! attention!
            if (this.Status.RhythmicStructure != null) {
                if (this.Tones.Any()) { //// 2019/02 !?!
                    this.Tones.Clear();
                }

                var binStruct = this.Status.RhythmicStructure.BinaryStructure(true);
                var binSchema = new BinarySchema(binStruct);
                if (this.Status.RhythmicStructure.IsFromPreviousBar) {
                    this.SolveToneFromPreviousBar(givenLoudness);
                }

                this.AppendTonesAccordingToRhythm(givenLoudness, binSchema);
            }
        }

        /// <summary>
        /// Compose rhythm of lines in given bar.
        /// </summary>
        public void FillWithRequestedRhythm() {
            Contract.Requires(this.Status != null);
            if (this.Status == null) {
                return;
            }

            if (this.Tones.Any()) { //// 2019/02 !?!
                this.Tones.Clear();
            }

            if (this.Status.RhythmicStructure != null || this.Status.RhythmicMotive != null) {
                var rhyStruct = this.DetermineRhythmicStructure(false);
                var loudness = this.Status.Loudness;
                this.FillWithRhythm(rhyStruct, loudness);
            }

            var addEmptyRange = this.Status.RhythmicStructure == null;
            if (!addEmptyRange) {
                return;
            }

            this.FillWithPause();
        }

        /// <summary>
        /// Fills the with pause.
        /// </summary>
        public void FillWithPause() {
            var range = new BitRange(this.Bar.Header.System.RhythmicOrder, 0, this.Bar.Header.System.RhythmicOrder);
            //// 2019/02
            var musicalPause = new MusicalPause(range, this.Bar.BarNumber);

            if (this.Status != null) {
                musicalPause.InstrumentNumber = this.Status.Instrument.Number;
                musicalPause.Staff = this.Status.Staff;
                musicalPause.Voice = this.Status.Voice;
            }

            this.Tones.Add(musicalPause);
        }

        /// <summary>
        /// Fills the with rhythm of tones.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        public void FillWithRhythmOfTones(ToneCollection givenTones) {
            var rorder = this.Bar.Header.System.RhythmicOrder;
            this.Status.RhythmicStructure = new RhythmicStructure(rorder, givenTones);  //// 2016  .MusicalTonesInBar(this.Number)//// 2014/01
        }

        /// <summary>
        /// Rhythmic structure in bar.
        /// </summary>
        /// <param name="fillEmpty">If set to <c>true</c> [fill empty].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicStructure DetermineRhythmicStructure(bool fillEmpty) {
            Contract.Requires(this.Status.RhythmicMotive != null);
            if (this.Status.RhythmicStructure != null) {
                return this.Status.RhythmicStructure;
            }

            int barIndex = 0;
            //// why there is sometimes (after chuffle...) barNumber < this.Status.BarNumber ???
            if (this.Bar.BarNumber >= this.Status.RhythmicMotiveStartBar) { //// && this.ComposedLine.Status.BarNumber.HasValue 
                //// // here have to be bar-number of the rhythmical motive start 
                //// // this.BarNumber is bar number of the last change
                barIndex = this.Bar.BarNumber - this.Status.RhythmicMotiveStartBar + 1;
            }

            if (barIndex < 1) {
                return null;
            }

            //// 2016 - stop changes ? if (barIndex > this.RhythmicMotive.Length) {   return null;   } 
            var rhythmicOrder = this.Bar.Header.System.RhythmicOrder;
            var rhyStructCode = this.Status.RhythmicMotive.RhythmicStructuralCodeForBar(barIndex);
            if (string.IsNullOrEmpty(rhyStructCode)) {
                rhyStructCode = fillEmpty ? "2" : "1";
            }

            var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder);
            var rstruct = RhythmicStructure.GetNewRhythmicStructure(rsystem, rhyStructCode); //// rhyStructNumber
            return rstruct;
        }

        /// <summary>
        /// Determines the melodic structure.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MelodicStructure DetermineMelodicStructure() {
            Contract.Requires(this.Status.MelodicMotive != null);

            int barIndex = 0;
            //// why there is sometimes (after chuffle...) barNumber < this.Status.BarNumber ???
            if (this.Bar.BarNumber >= this.Status.MelodicMotiveStartBar) { //// && this.ComposedLine.Status.BarNumber.HasValue 
                //// // here have to be bar-number of the rhythmical motive start 
                //// // this.BarNumber is bar number of the last change
                barIndex = this.Bar.BarNumber - this.Status.MelodicMotiveStartBar + 1;
            }

            if (barIndex < 1) {
                return null;
            }

            //// 2016, see stop changes... if (barIndex > this.MelodicMotive.Length) { return null;  } 
            var mms = this.Status.MelodicMotive.MelodicStructureInBar(barIndex);
            return mms;
        }

        /// <summary>
        /// Completes the melodic tone.
        /// </summary>
        /// <param name="musicalTone">The melodic tone.</param>
        public void CompleteMelodicTone(MusicalStrike musicalTone) {
            Contract.Requires(musicalTone != null);

            var line = this.MusicalLine;
            MusicalStrike.CorrectBadBinding(line.LastTone, musicalTone);

            if (line.LastTone != null && !line.LastTone.IsEmpty) {
                line.PenultTone = (MusicalTone)line.LastTone.Clone();
            }

            musicalTone.BarNumber = this.Bar.BarNumber;
            if (musicalTone.BitRange != null) {
                var loudness = this.DetermineLoudness(this.Status.RhythmicStructure, musicalTone.BitRange); //// (musicalBar.Number)
                musicalTone.Loudness = loudness;
            }

            var status = this.Status;
            musicalTone.InstrumentNumber = status.Instrument.Number;
            musicalTone.Staff = status.Staff;
            musicalTone.Voice = status.Voice;
        }

        /// <summary>
        /// Swaps the melodic with.
        /// </summary>
        /// <param name="element">The element.</param>
        public void SwapMelodicWith(MusicalElement element) {
            Contract.Requires(element != null);

            var musicalOctave = element.Status.Octave;
            element.Status.Octave = this.Status.Octave;
            this.Status.Octave = musicalOctave;

            var melodicType = element.Status.MelodicFunction;
            element.Status.MelodicFunction = this.Status.MelodicFunction;
            this.Status.MelodicFunction = melodicType;

            var melodicStructure = element.Status.MelodicStructure;
            element.Status.MelodicStructure = this.Status.MelodicStructure;
            this.Status.MelodicStructure = melodicStructure;
        }
        #endregion

        #region Public methods - Modification

        /// <summary>
        /// Shifts the line octave.
        /// </summary>
        /// <param name="numberOfOctaves">The number of octaves.</param>
        public void ShiftOctave(int numberOfOctaves) {
            if (this.Status.Octave == MusicalOctave.None) {
                this.Status.Octave = MusicalOctave.OneLine;
                return;
            }

            var n = (int)this.Status.Octave + numberOfOctaves;
            n = Math.Min(n, (int)MusicalOctave.FiveLine);
            n = Math.Max(n, (int)MusicalOctave.SubContra);
            this.Status.Octave = (MusicalOctave)n;
        }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                    " Bar {0} line {1} mel:{2} rhy:{3} R:{4}", this.Point.BarNumber, this.Point.LineIndex, this.Status.IsMelodicOriginal, this.Status.IsRhythmicOriginal, this.Status.RhythmicStructure != null ? this.Status.RhythmicStructure.ElementSchema : string.Empty);
            return s.ToString();
        }

        /// <summary>
        /// To the progress string.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string ToProgressString() {
            var s = new StringBuilder();
            s.AppendFormat(
                    " Bar {0} / Line {1}", this.Point.BarNumber, this.Point.LineIndex);
            return s.ToString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Solves the tone from previous bar.
        /// </summary>
        /// <param name="givenLoudness">The given loudness.</param>
        private void SolveToneFromPreviousBar(MusicalLoudness givenLoudness) {
            var toneRange = this.Status.RhythmicStructure.OverrunRange();
            if (toneRange == null) {
                return;
            }

            var lastTone = this.Tones.LastOrDefault();
            if (lastTone == null) {
                return;
            }

            lastTone.IsGoingToNextBar = MusicalSettings.Singleton.SettingsAnalysis.LongTones; //// true;
            var tone = this.PrepareTone(givenLoudness, toneRange);
            tone.IsFromPreviousBar = MusicalSettings.Singleton.SettingsAnalysis.LongTones; //// true;
            this.Tones?.Add(tone);
        }

        /// <summary>
        /// Prepares the tone.
        /// </summary>
        /// <param name="givenLoudness">The given loudness.</param>
        /// <param name="toneRange">The tone range.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MusicalStrike PrepareTone(MusicalLoudness givenLoudness, BitRange toneRange) {
            Contract.Requires(toneRange != null);
            MusicalStrike tone;
            var loudness = this.Status.RhythmicStructure != null && this.Status.RhythmicStructure.IsPauseStart(toneRange.BitFrom) ? MusicalLoudness.None : givenLoudness;

            if (this.Status.LineType == MusicalLineType.Melodic) {
                tone = new MusicalTone(null, toneRange, loudness, this.Bar.BarNumber);
            }
            else {
                if (this.Status.RhythmicStructure != null) {
                    loudness = givenLoudness; ////  (byte)MusicalLoudness.MeanLoudness;  //// this.DetermineLoudness(musicalBar, this.Status.RhythmicStructure, toneRange);
                }

                tone = new MusicalStrike(MusicalToneType.Rhythmic, toneRange, loudness, this.Bar.BarNumber);
            }

            if (!this.Status.IsRhythmic) {
                return tone;
            }

            tone.Loudness = loudness;
            tone.InstrumentNumber = this.Status.Instrument.Number;
            tone.Staff = this.Status.Staff;
            tone.Voice = this.Status.Voice;

            return tone;
        }

        /// <summary>
        /// Appends the tones according to rhythm.
        /// </summary>
        /// <param name="givenLoudness">The given loudness.</param>
        /// <param name="binSchema">The bin schema.</param>
        private void AppendTonesAccordingToRhythm(MusicalLoudness givenLoudness, BinarySchema binSchema) {
            Contract.Requires(binSchema != null);
            var level = this.Status.RhythmicStructure.Level; //// not ToneLevel
            BitRange toneRange = null;
            for (byte i = 0; i < level; i++) {
                if (this.Status.RhythmicStructure != null) {
                    if (i >= binSchema.Level) {
                        i = 0;
                    }

                    toneRange = binSchema.RangeAtLevel(i);  // toneRange = [  MaskedBitRange];
                }

                if (toneRange == null || toneRange.Length == 0) {
                    continue;
                }

                var tone = this.PrepareTone(givenLoudness, toneRange);
                if (this.Tones != null) {
                    tone.OrdinalIndex = this.Tones.Count;
                    this.Tones.Add(tone);
                }
            }
        }

        /// <summary>
        /// Assign Harmonic Modality.
        /// </summary>
        /// <param name="prevailingHarmonicStructure">Prevailing HarmonicStructure.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private HarmonicModality DetermineHarmonicModality(HarmonicStructure prevailingHarmonicStructure) {
            Contract.Requires(this.Bar.HarmonicBar.HarmonicMotive != null);
            HarmonicModality modality = null;
            var minModalityLevel = MusicalSettings.Singleton.SettingsAnalysis.MinimalModalityLevel;
            var modalization = MusicalSettings.Singleton.SettingsComposition.HarmonicModalization;
            switch (modalization) {
                //// 0. Chromatic only 
                case HarmonicModalizationType.Chromatic: {
                        //// modality = null;
                        break;
                    }

                //// 1. Modality from structures in the bar after it disappears } (hstruct.Number & this.HarmonicModality.Number)==0)) (current settings)
                case HarmonicModalizationType.Consecutive: {
                        if (prevailingHarmonicStructure != null) {
                            modality = prevailingHarmonicStructure.HarmonicModality;
                            if (modality == null || modality.Level < minModalityLevel || ((prevailingHarmonicStructure.Number & modality.Number) == 0)) {
                                modality = ((MusicalBar)this.Bar).HarmonicModalityFromStructures(MusicalSettings.Singleton.SettingsAnalysis.MinimalModalityLevel);
                            }
                        }

                        break;
                    }

                //// 2. Forced from above
                case HarmonicModalizationType.Forced:
                    modality = MusicalSettings.Singleton.SettingsComposition.ForcedModality;
                    break;
            }

            if (modality != null) {
                return modality;
            }

            //// Chromatic modality is default value, when other assignments failed
            modality = this.Bar.Header.System.HarmonicSystem.ChromaticModality;
            return modality;
        }

        /// <summary>
        /// Corrects the loudness. According to dynamic rules.
        /// </summary>
        /// <param name="toneRange">The tone range.</param>
        /// <param name="loudness">The loudness.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private byte CorrectLoudness(BitRange toneRange, byte loudness) {
            Contract.Requires(toneRange != null);
            const byte consonanceLimit = 60;

            if (toneRange.BitFrom == 0) {
                loudness++;
            }
            else {
                if (toneRange.Length > 1) {
                    loudness++;
                }

                if (toneRange.Length > 3) {
                    loudness++;
                }
            }

            if (this.Status.LineType != MusicalLineType.Melodic || this.Bar == null) {
                return loudness;
            }

            var cluster = ((MusicalBar)this.Bar).HarmonicClusterAtTick(toneRange.BitFrom);
            if (cluster == null) {
                return loudness;
            }

            var v = cluster.RealEnergy.Consonance;  //// .FormalConsonance);
            if (v < consonanceLimit) {
                loudness++;
            }

            return loudness;
        }
        #endregion
    }
}
