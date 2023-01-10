// <copyright file="MusicalBlock.cs" company="Traced-Ideas, Czech republic">
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
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Composer;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Models;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Templates;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical file.
    /// </summary>
    [Serializable]
    public sealed partial class MusicalBlock : MusicalContent
    {
        #region Fields
        /// <summary>
        /// Musical file.
        /// </summary>
        private MusicalBundle musicalBundle;

        /// <summary>
        /// Strip - the list of block lines.
        /// </summary>
        [NonSerialized]
        private MusicalStrip strip;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalBlock class.
        /// </summary>
        public MusicalBlock() {
            var context = new MusicalContext(MusicalSettings.Singleton, this.Header);
            this.Body = new MusicalBody(context);
            this.Strip = new MusicalStrip(context);
            this.ContainsMusic = false;
            this.FileHeading = new FileHeading();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBlock" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        public MusicalBlock(MusicalHeader givenHeader) {
            //// this.BlockMidi = new MusicalBlockMidi();
            this.Header = givenHeader;
            var context = new MusicalContext(MusicalSettings.Singleton, this.Header);
            this.Body = new MusicalBody(context);
            this.Strip = new MusicalStrip(context);
            this.ContainsMusic = false;
            this.FileHeading = new FileHeading();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBlock" /> class.
        /// </summary>
        /// <param name="markBlock">The mark block.</param>
        public MusicalBlock(XElement markBlock) {
            Contract.Requires(markBlock != null);
            if (markBlock == null) {
                return;
            }

            var xheading = markBlock.Element("Heading");
            this.FileHeading = new FileHeading(xheading);

            var xheader = markBlock.Element("Header");
            this.Header = new MusicalHeader(xheader, true);

            var context = new MusicalContext(MusicalSettings.Singleton, this.Header);

            XElement xbody = markBlock.Element("Body");
            if (xbody == null) {
                return;
            }

            var xstrip = markBlock.Element("Strip");
            if (xstrip == null) {
                return;
            }

            this.Strip = new MusicalStrip(xstrip, context);
            this.ContainsMusic = true;
            this.ConvertStripToBody(true);

            var xelements = xbody.Elements();
            foreach (XElement xbar in xelements) {
                var barNumber = (int)xbar.Attribute("Number");
                MusicalBar bar = this.Body.GetBar(barNumber);
                if (bar == null) {  //// 2020/10
                    continue;
                }

                var tempo = (int)xbar.Attribute("Tempo");
                bar.TempoNumber = tempo;

                var xharmony = xbar.Element("Harmony");
                var harmonicBar = new HarmonicBar(this.Header, xharmony) {
                    BarNumber = barNumber
                };

                bar.HarmonicBar = harmonicBar;
            }

            this.LoadFirstStatusToLines(); //// 2019/10
            //// this.FileHeading = new FileHeading();
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xblock = new XElement("Block");

                var xheading = this.FileHeading.GetXElement;
                xblock.Add(xheading);

                XElement xheader = this.Header.GetXElement;
                xblock.Add(xheader);

                XElement xbody = new XElement("Body");
                foreach (var bar in this.Body.Bars) {
                    if (bar?.HarmonicBar == null) {
                        continue;
                    }

                    var xbar = new XElement("Bar");
                    xbar.Add(new XAttribute("Number", bar.BarNumber));
                    var xtempo = new XAttribute("Tempo", bar.TempoNumber);
                    xbar.Add(xtempo);
                    var xharmony = bar.HarmonicBar.GetXElement;
                    xbar.Add(xharmony);

                    xbody.Add(xbar);
                }

                xblock.Add(xbody);

                //// Prepare list for usage by MusicalLine.GetXElement,
                //// status will be written as lists of bars to tracks.      
                if (!this.IsStripStatusOk) {
                    this.ConvertBodyStatusToStrip();
                }

                XElement xstrip = this.Strip.GetXElement;
                xblock.Add(xstrip);

                return xblock;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the file heading.
        /// </summary>
        /// <value>
        /// The file heading.
        /// </value>
        public FileHeading FileHeading { get; set; }

        /// <summary>
        /// Gets or sets the musical file.
        /// </summary>
        /// <value>
        /// The musical file.
        /// </value>
        public MusicalBundle MusicalBundle {
            get => this.musicalBundle;

            set => this.musicalBundle = value;
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public MusicalBody Body { get; set; }

        /// <summary>
        /// Gets or sets the strip - list of musical lines.
        /// </summary>
        /// <value>
        /// The strip.
        /// </value>
        /// <exception cref="InvalidOperationException">Strip is null.</exception>
        /// <exception cref="ArgumentException">Argument Exception value</exception>
        public MusicalStrip Strip {
            get {
                Contract.Ensures(Contract.Result<MusicalStrip>() != null);
                if (this.strip == null) {
                    throw new InvalidOperationException("Strip is null.");
                }

                return this.strip;
            }

            set => this.strip = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }

        /// <summary>
        /// Gets or sets a value indicating whether [had instrument in tones].
        /// Default value is NO, i.e. instrument are taken from voices.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [had instrument in tones]; otherwise, <c>false</c>.
        /// </value>
        public bool HasInstrumentInTones { get; set; }

        /// <summary>
        /// Gets Musical Identification.
        /// </summary>
        /// <value> Property description. </value>
        public IList<KeyValuePair> Identification {
            get {
                var items = new List<KeyValuePair>();
                var item = new KeyValuePair(LocalizedMusic.String("Number of bars"), this.Header.NumberOfBars.ToString(CultureInfo.InvariantCulture));
                items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Number of lines"), this.Strip.Lines.Count.ToString(CultureInfo.InvariantCulture));
                items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Harmonic order"), this.Header.System.HarmonicOrder.ToString(CultureInfo.InvariantCulture));
                items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Rhythmic order"), this.Header.System.RhythmicOrder.ToString(CultureInfo.InvariantCulture));
                items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Metric"), string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.Header.Metric.MetricBeat, this.Header.Metric.MetricGround));
                items.Add(item);

                var musicalTempo = RhythmicSystem.ApproximateMusicalTempo(this.Header.Tempo);
                item = new KeyValuePair(LocalizedMusic.String("Tempo"), string.Format(CultureInfo.InvariantCulture, "{0}({1})", this.Header.Tempo, LocalizedMusic.String("Tempo" + ((byte)musicalTempo))));
                items.Add(item);

                //// item = new KeyValuePair("Original file name", this.FileName);
                //// items.Add(item);

                item = new KeyValuePair(LocalizedMusic.String("Original division"), this.Header.Division.ToString(CultureInfo.InvariantCulture));
                items.Add(item);

                return items;
            }
        }

        /// <summary>
        /// Gets the identification string.
        /// </summary>
        /// <value>
        /// The identification string.
        /// </value>
        public string IdentificationString {
            get {
                StringBuilder sb = new StringBuilder();
                var idents = this.Identification;
                foreach (var ident in idents) {
                    sb.AppendFormat("{0}: {1}\n", ident.Key, ident.Value);
                }

                return sb.ToString();
            }
        }

        #endregion

        /// <summary> Gets or sets the lines. </summary>
        /// <value> The lines. </value>
        public override List<IAbstractLine> ContentLines {
            get {
                var ilines = (from line in this.Strip.Lines select line as IAbstractLine).ToList();
                return ilines;
            }
        }

        /// <summary> Gets or sets the harmony. </summary>
        /// <value> The harmony. </value>
        public override List<IAbstractBar> ContentBars {
            get {
                var ibars = (from bar in this.Body.Bars select bar as IAbstractBar).ToList();
                return ibars;
            }
        }

        /// <summary>
        /// Gets the content elements.
        /// </summary>
        /// <value>
        /// The content elements.
        /// </value>
        public new IList<MusicalElement> ContentElements {
            get {
                var elements = this.Body.AllElements.ToList();
                return elements;
            }
        }

        #region Other properties

        /// <summary> Gets string with bar details. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public string BarDetailsToString {
            get {
                var s = new StringBuilder();
                s.Append(" Bar details                                          \n");
                s.Append("------------------------------------------------------\n");
                var bars = from b in this.Body.Bars orderby b.BarNumber select b;
                var lines = (from p in this.Strip.Lines orderby p.LineIndex descending select p).ToList();
                foreach (var bar in bars.Where(bar => bar != null && !bar.IsEmpty)) {
                    s.AppendFormat(CultureInfo.CurrentCulture, "* Bar {0,2}: *\r\n", bar.BarNumber);
                    foreach (var line in
                        lines.Where(ml => ml?.Tones != null && !ml.IsEmpty)) {
                        s.AppendFormat(CultureInfo.CurrentCulture, "Part {0,2}: ", line.LineIndex);
                        var tones = line.MusicalTonesInBar(bar.BarNumber);
                        foreach (var mt in tones.Where(mt => mt != null)) {
                            s.Append(mt);
                        }

                        s.AppendLine(string.Empty);
                    }

                    s.AppendLine(string.Empty);
                }

                return s.ToString();
            }
        }

        #endregion

        #region Static factory
        /// <summary>
        /// Imports Midi File.
        /// </summary>
        /// <param name="midiBlock">The midi block.</param>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns> Returns value. </returns>
        public static MusicalBlock NewMusicalBlock(MidiBlock midiBlock, byte harmonicOrder, byte rhythmicOrder) {
            Contract.Requires(midiBlock != null);
            Contract.Requires(midiBlock.Area != null);
            Contract.Requires(midiBlock.Sequence != null);

            if (midiBlock.Header.Clone() is MusicalHeader musicHeader) {
                musicHeader.System.HarmonicOrder = harmonicOrder;
                musicHeader.System.RhythmicOrder = rhythmicOrder;
                var musicalBlock = new MusicalBlock {
                    Header = musicHeader
                };

                var context = new MusicalContext(MusicalSettings.Singleton, musicalBlock.Header);
                musicalBlock.Body = new MusicalBody(context); //// 2019/02 again used!?
                musicalBlock.Strip = new MusicalStrip(context);

                var header = musicalBlock.Header;
                foreach (var midiTrack in midiBlock.Sequence) {
                    var mtrack = (MidiTrack)midiTrack;
                    mtrack.Metric = header.Metric;
                    mtrack.BarDivision = MusicalProperties.BarDivision(header.Division, header.Metric.MetricBeat, header.Metric.MetricGround);
                }

                musicalBlock.Strip.ResetLines();
                musicalBlock.AppendMidiTracks(midiBlock);

                var cmt = (byte)(from t in midiBlock.Sequence where t.IsMelodic select 1).Count();
                var crt = (byte)(from t in midiBlock.Sequence where t.IsRhythmical select 1).Count();
                musicalBlock.Header.NumberOfMelodicLines = cmt;
                musicalBlock.Header.NumberOfRhythmicLines = crt;
                musicalBlock.Header.NumberOfLines = cmt + crt;

                return musicalBlock;
            }

            return null;
        }

        /*
        /// <summary>
        /// Prepares the block.
        /// </summary>
        /// <param name="givenNumberOfBars">The given number of bars.</param>
        /// <param name="givenStripPrototype">The given strip prototype.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static MusicalBlock PrepareBlock(int givenNumberOfBars, TemplateBlock givenStripPrototype) {
            var header = MusicalHeader.GetDefaultMusicalHeader;
            header.NumberOfBars = givenNumberOfBars;
            header.Specification = "Test";
            header.FileName = "Test-File";
            //// header.System.RhythmicOrder = firstHarBar.RhythmicStructure.Order;
            var context = new MusicalContext(MusicalSettings.Singleton, header);
            var strip = new MusicalStrip(context);

            foreach (var prototype in givenStripPrototype.Lines) {
                var channel = MusicalProperties.ChannelForPartNumber(prototype.LineIndex);
                var status = new LineStatus(
                                            1,
                                            prototype.Status.LineType,
                                            prototype.Status.Instrument,
                                            LinePurpose.Composed,
                                            channel);
                //// firstStatus.MelodicVariety = null; //// 2018/09 new MusicalVariety(MusicalSettings.Singleton);
                var newLine = new MusicalLine(status) { FirstStatus = { LocalPurpose = LinePurpose.Composed } };
                strip.AddLine(newLine, true);
            }

            var block = new MusicalBlock {
                Header = header,
                Strip = strip
            };

            block.Header.Number = 1;
            //// 2018/10 block.ConvertStripToBody(true);

            return block;
        } */

        /// <summary>
        /// Defaults the block.
        /// </summary>
        /// <param name="tectonicTemplate">The tectonic template.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalBlock DefaultBlock(TemplateBlock tectonicTemplate) {
            if (tectonicTemplate == null) {
                return null;
            }

            var block = new MusicalBlock();

            foreach (var tline in tectonicTemplate.Lines) {
                var lineStatus = tline.Status;
                var newline = block.AddContentLine(lineStatus);
                var voice = new MusicalVoice {
                    Instrument = lineStatus.Instrument,
                    Octave = lineStatus.Octave,
                    Loudness = lineStatus.Loudness,
                    Line = newline
                };

                newline.Voices = new List<IAbstractVoice> { voice };
                newline.MainVoice = voice;
            }

            for (int barNumber = 1; barNumber < tectonicTemplate.Header.NumberOfBars; barNumber++) {
                block.AddContentBar(barNumber, null);
            }

            //// Convert current body back to strip!?! (Status is in elements)
            block.ConvertBodyToStrip(false, true);

            block.Header.NumberOfLines = block.ContentLines.Count;
            block.Header.NumberOfMelodicLines = (byte)block.Header.NumberOfLines;
            block.Header.NumberOfRhythmicLines = 0;
            block.Header.NumberOfBars = block.ContentBars.Count;

            return block;
        }
        #endregion

        #region Add content
        /// <summary>
        /// Adds the content bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <param name="lastBar">The last bar.</param>
        /// <returns> Returns musical bar.</returns>
        public IAbstractBar AddContentBar(int barNumber, MusicalBar lastBar) {
            var bar = new MusicalBar(barNumber++, this.Body);
            bar.HarmonicBar.Header = bar.Header;
            var chord = new HarmonicStructure(this.Header.System.HarmonicSystem, "0,4,7");
            bar.HarmonicBar.SetStructure(chord);
            //// bar.MakeCommonRhythmicShape();
            /* var rhyStruct = bar.HarmonicBar.RhythmicStructure;
            bar.HarmonicBar.RhythmicShape = new RhythmicShape(rhyStruct.RhythmicSystem.Order, rhyStruct);
            bar.HarmonicBar.SetBarMetricCode(bar.HarmonicBar.RhythmicShape.GetStructuralCode); */

            this.Body.Bars.Add(bar);
            this.Header.NumberOfBars = this.Body.Bars.Count;

            MusicalElement lastElement = null;
            foreach (var line in this.Strip.Lines) {
                if (lastBar != null) { //// 2020/10 lastElement == null && 
                    lastElement = lastBar.GetElement(line.LineIdent);
                }

                var musicalLine = line as MusicalLine;
                LineStatus status;
                if (lastElement != null) {
                    status = (LineStatus)lastElement.Status.Clone();
                }
                else {
                    status = (LineStatus)musicalLine.FirstStatus.Clone();
                }

                status.LocalPurpose = LinePurpose.Composed;
                status.BarNumber = barNumber; //// 2020/08

                var element = new MusicalElement(status, lastElement) {
                    Bar = bar,
                    Line = line
                };

                bar.Elements.Add(element);
                //// lastElement = element;
            }

            return bar;
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="givenStatus">The given line status.</param>
        /// <returns> Returns value. </returns>
        public override IAbstractLine AddContentLine(LineStatus givenStatus) {
            //// var lines = (List<MusicalLine>)this.Strip.Lines;
            var lineIndex = this.Header.NumberOfLines; 
            var newLine = new MusicalLine(givenStatus) {
                LineIndex = lineIndex,
                Purpose = LinePurpose.Composed
            };

            this.AddContentLine(newLine);
            return newLine;
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        public void AddContentLine(MusicalLine givenLine) {
            var lines = (List<MusicalLine>)this.Strip.Lines;
            if (givenLine.LineType == MusicalLineType.Melodic) {
                var channel = this.Strip.FindFreeChannel(givenLine.LineIndex);
                givenLine.MainVoice.Channel = channel;
            }
            else {
                givenLine.MainVoice.Channel = MidiChannel.DrumChannel;
            }

            //// givenLine.LineType = givenLine.FirstStatus.LineType;

            lines.Add(givenLine); //// Model have to be assigned before loading
            this.AddElementsForLine(givenLine);

            this.Header.NumberOfLines = lines.Count;
            if (givenLine.LineType == MusicalLineType.Melodic) {
                this.Header.NumberOfMelodicLines++;
            }
            else {
                this.Header.NumberOfRhythmicLines++;
            }

            this.Strip.Context.Header.NumberOfLines = this.Header.NumberOfLines; //// ?!
        }

        /// <summary>
        /// Clones the specified include tones.
        /// </summary>
        /// <param name="cloneTracks">If set to <c>true</c> [clone tracks].</param>
        /// <param name="includeTones">If set to <c>true</c> [include tones].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalBlock Clone(bool cloneTracks, bool includeTones) {
            var block = (MusicalBlock)this.Clone();
            //// block.Strip.SetTracks(new List<MusicalLine>());
            if (!cloneTracks) {
                return block;
            }

            block.strip = this.Strip.Clone(includeTones);
            return block;
        }

        /// <summary>
        /// Deletes the line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        public void DeleteLine(Guid lineIdent) {
            this.Body.DeleteLine(lineIdent);
            this.Strip.DeleteLine(lineIdent);
            this.Header.NumberOfLines--;
        }

        /// <summary>
        /// Append CompactMidiStrip.
        /// </summary>
        /// <param name="midiBlock">The midi block.</param>
        public void AppendMidiTracks(MidiBlock midiBlock) /*   CompactMidiStrip midiTracks int barNumberFrom, int barNumberTo, long timeFrom, long timeTo */
        {
            if (midiBlock.Sequence == null) {
                return;
            }

            var lineIndex = this.Strip.Lines.Count;
            this.Header.NumberOfBars = midiBlock.Area.BarTo - midiBlock.Area.BarFrom + 1;
            var orderedToneTracks = (from mt in midiBlock.Sequence orderby mt.Octave where mt.Sequence != null select mt).ToList();
            orderedToneTracks.ForEach(midiTrack => this.AppendMidiTrack(midiTrack, lineIndex++)); //// determineBarNumbers
        }

        /// <summary>
        /// Appends the midi line.
        /// </summary>
        /// <param name="midiTrack">The midi line.</param>
        /// <param name="lineIndex">The line number.</param>
        public void AppendMidiTrack(IMidiTrack midiTrack, int lineIndex) //// 2019/01 MusicalSection givenArea, 
        {
            Contract.Requires(midiTrack != null);
            var line = new MusicalLine(midiTrack, this.Strip) //// 2019/01 givenArea, 
            {
                LineIndex = lineIndex
            };

            var lines = (List<MusicalLine>)this.Strip.Lines;
            //// 2020/10 this.AddContentLine(line); 
            lines.Add(line); //// Model have to be assigned before loading
        }
        #endregion

        #region Actions - Content changes

        /// <summary>
        /// Harmonizes the specified area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="harmonicStream">The harmonic stream.</param>
        public void Harmonize(MusicalSection area, HarmonicStream harmonicStream) {
            for (var bar = 1; bar <= this.Header.NumberOfBars; bar++) {
                var idx = (bar - 1) % harmonicStream.HarmonicBars.Count;
                var harmonicBar = harmonicStream.HarmonicBars[idx];
                this.Body.Bars[idx].SetHarmonicBar(harmonicBar);
            }
        }

        /// <summary>
        /// Set rhythm to the specified area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="rhythmicStructure">The rhythmic structure.</param>
        public void Rhythmize(MusicalSection area, RhythmicStructure rhythmicStructure) {
            this.Body.Rhythmize(area, rhythmicStructure);
        }

        /// <summary>
        /// Modulates the specified area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="harmonicModality">The harmonic modality.</param>
        public void Modulate(MusicalSection area, HarmonicModality harmonicModality) {
            this.Body.Modulate(area, harmonicModality);
        }
        
        /// <summary>
        /// Changes the rhythmic.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="rhythmicMaterial">The rhythmic material.</param>
        public void ChangeRhythmic(MusicalSection area, RhythmicMaterial rhythmicMaterial) {
            this.Body.ChangeRhythmic(area, rhythmicMaterial);
        }

        /// <summary>
        /// Orchestrates the specified given block.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="orchestraBlock">The orchestra block.</param>
        public void Orchestrate(MusicalSection area, OrchestraBlock orchestraBlock) {
            int lineNumber = -1;
            foreach (var line in this.Strip.Lines) {
                lineNumber++;
                if (line == null) {
                    continue;
                }

                //// if (line.MainVoice != null) { line.MainVoice.Instrument = orchestraVoice.Instrument; }
                foreach (var voice in line.Voices) {
                    var orchestraVoice = orchestraBlock.Strip.OptimalOrchestraVoice(line, voice);
                    if (orchestraVoice == null) {
                        continue;
                    }

                    voice.Instrument = orchestraVoice.Instrument;
                    orchestraVoice.IsUsed = true;
                }

                //// if (line.FirstStatus != null) { line.FirstStatus.CurrentOrchestraVoice = orchestraVoice; } 
                if (line.FirstStatus != null) {
                    line.FirstStatus.Instrument = line.MainVoice.Instrument;
                }

                var tones = line.MusicalTonesInArea(area);
                tones.SetInstrument(line.MainVoice.Instrument.Number, line.MainVoice.Octave);

                for (var bar = 1; bar <= this.Header.NumberOfBars; bar++) {
                    var point = MusicalPoint.GetPoint(lineNumber, bar);  //// 2017/03 !?!?
                    //// var point = MusicalPoint.GetPoint(line.LineIndex, bar); //// line
                    var me = this.Body.GetElement(point);
                    if (me == null) {
                        continue;
                    }

                    me.Status.Instrument = line.MainVoice.Instrument;
                }
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            //// if (this.Name == null) { return string.Empty; }

            var s = new StringBuilder();
            s.Append("\t" + this.Header.FileName.ToString(CultureInfo.CurrentCulture));
            return s.ToString();
        }
        #endregion

        #region Strip-Body conversion

        /// <summary>
        /// Converts the strip to body.
        /// </summary>
        /// <param name="keepStatus">if set to <c>true</c> [keep status].</param>
        public void ConvertStripToBody(bool keepStatus) {
            //// Keep original status (tempo, harmony) !?
            var newBody = new MusicalBody(this.Strip);
            //// 2019/10 - added missing if !!?!?!?!!!!!!!
            //// if (keepStatus) {
            if (this.Body == null || !this.Body.Bars.Any()) { //// 2020/10 during load body exists with no bars
                this.Body = newBody;
            }
            else { 
                foreach (var bar in this.Body.Bars) {
                    var newBar = newBody.GetBar(bar.BarNumber);
                    if (newBar != null) {
                        newBar.TempoNumber = bar.TempoNumber;
                        newBar.HarmonicBar = (HarmonicBar)bar.HarmonicBar.Clone();
                        //// newBar.Status = bar.Status;
                    }
                }
            }
            //// }

            this.ConvertStripStatusToBody(); //// 2019/10  !!!!!!!

            this.Header.NumberOfLines = this.Strip.Lines.Count;
            this.Strip.Context.Header.NumberOfLines = this.Header.NumberOfLines;
            this.Strip.Context.Header.NumberOfBars = this.Body.Bars.Count;  //// 2020/10
            
            //// Tones are directed by status!?? - When opening a mif-file?! - Temporary!?
            if (keepStatus) {
                this.ConvertStripStatusToBody();
            }
        }

        /// <summary>
        /// Converts the body to strip.
        /// </summary>
        /// <param name="checkTones">if set to <c>true</c> [check tones].</param>
        /// <param name="includingStatus">if set to <c>true</c> [including status].</param>
        public void ConvertBodyToStrip(bool checkTones, bool includingStatus) {
            this.Strip.ResetTones();
            this.Strip.WriteBody(this.Body);

            if (checkTones) {
                this.Strip.CorrectOctaves();
            }

            if (includingStatus) {
                this.ConvertBodyStatusToStrip();
            }
        }
        #endregion

        #region Private methods
        /// <summary> Makes a deep copy of the MusicalBlock object. </summary>
        /// <returns> Returns object. </returns>
        private object Clone() {
            var header = (MusicalHeader)this.Header.Clone();
            var block = new MusicalBlock {
                Header = header
            };

            return block;
        }

        #endregion
    }
}

/*
/// <summary>
/// Sets the tempo events.
/// </summary>
/// <param name="givenTempoChanges">The given tempo changes.</param>
[UsedImplicitly]
public void SetTempoEvents(IEnumerable<TempoChange> givenTempoChanges) {
    Contract.Requires(givenTempoChanges != null);

    var list = new List<MetaTempo>();
    var barDivision = MusicalProperties.BarDivision(this.Header.Division, this.Header.Metric.MetricBeat, this.Header.Metric.MetricGround);
    var barDuration = MusicalProperties.MidiDuration(this.Header.System.RhythmicOrder, this.Header.System.RhythmicOrder, barDivision);
    // ReSharper disable once LoopCanBePartlyConvertedToQuery
    foreach (var change in givenTempoChanges) {
        var barDeltaTime = barDuration * (change.BarNumber - 1);
        var ev = new MetaTempo(0, 100) {
            StartTime = barDeltaTime,
            Tempo = change.TempoNumber
        };  //// 100 is arbitrary number here
        //// Tempo have to change just before start of bar
        list.Add(ev);
    }

    this.Body.TempoEvents = list;
}*/