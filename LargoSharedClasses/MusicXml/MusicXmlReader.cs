// <copyright file="MusicXmlReader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.MusicXml
{
    /// <summary>
    /// Music Xml.
    /// </summary>
    public sealed class MusicXmlReader
    {
        #region Fields
        /// <summary>
        /// Musical Block.
        /// </summary>
        private MusicalBlock musicalBlock;

        /// <summary>
        /// The linearizer
        /// </summary>
        private MusicalLinearizer linearizer;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets Musical Block.
        /// </summary>
        /// <value> Property description. </value>
        /// <summary>
        /// Gets musical block.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalBlock Block
        {
            get
            {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.musicalBlock == null) {
                    throw new InvalidOperationException("Musical block is null.");
                }

                return this.musicalBlock;
            }

            private set => this.musicalBlock = value ?? throw new ArgumentException("Argument cannot be null.", nameof(value));
        }

        /// <summary>
        /// Gets Common Division.
        /// </summary>
        /// <value> Property description. </value>
        public int CommonDivision { get; private set; }

        /// <summary>
        /// Gets Local Division.
        /// </summary>
        /// <value> Property description. </value>
        public int LocalDivision { get; private set; }

        /// <summary>
        /// Gets or sets Local Pitch Shift.
        /// </summary>
        /// <value> Property description. </value>
        public int LocalPitchShift { get; set; }

        /// <summary>
        /// Gets or sets Header.
        /// </summary>
        private MusicXmlHeader Header { get; set; }

        /// <summary>
        /// Gets or sets Musical File.
        /// </summary>
        /// <value> Property description. </value>
        private MusicalBundle MusicalBundle { get; set; }
        #endregion

        #region Private methods
        /// <summary>
        /// Extract Musical File from the Music Xml.
        /// </summary>
        /// <param name="musicXmlDocument">Xml document.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="settingImport">The setting import.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalBundle ExtractMusicalFile(XDocument musicXmlDocument, string internalName, SettingsImport settingImport)
        {
            //// const int rhythmicDivisionLimit = 250;
            Contract.Requires(musicXmlDocument != null);
            if (musicXmlDocument == null) {
                //// exception ...
                return null;
            }

            //// var justPath = Path.GetDirectoryName(path);
            this.Block = new MusicalBlock
            {
                Header = new MusicalHeader()
                {
                    //// FilePath = Path.Combine(justPath ?? throw new InvalidOperationException(), name),
                    FileName = internalName,
                    Specification = string.Empty,
                    System = { HarmonicOrder = DefaultValue.HarmonicOrder },
                    Tempo = DefaultValue.DefaultTempo,
                }
            };

            this.MusicalBundle = MusicalBundle.GetEnvelopeOfBlock(this.Block, string.Empty);
            this.MusicalBundle.OriginType = MusicalOriginType.Original;

            //// Score Partwise
            var scorePartwise = musicXmlDocument.Root;  //// 
            if (scorePartwise.Name != "score-partwise") {
                //// exception ...
                return null;
            }

            this.Header = new MusicXmlHeader(scorePartwise, this.Block);
            this.Header.ReadXmlHeader();

            //// ProcessLogger.Singleton.SendMessageEvent(LocalizedMusic.String("Preparing file..."), 0);
            this.CommonDivision = 1;
            if (musicXmlDocument.Root != null) {
                var parts = musicXmlDocument.Root.Elements("part");
                var plist = parts as IList<XElement> ?? parts.ToList(); //// resharper
                foreach (var part in plist) {
                    this.ReadMusicalDivision(part);
                }

                var d = MusicalProperties.DetermineRhythmicOrder(this.CommonDivision);
                this.Block.Header.System.RhythmicOrder = (byte)d; //// this.MusicalBlock.Division
                this.Block.Header.Division = this.CommonDivision;
                //// if (this.MusicalBlock.Division > rhythmicDivisionLimit) {
                //// If rhythm coded in arithmetic of 3-symbols then limit is 523
                ////    throw new ArgumentException("Rhythmical order can not exceed " + rhythmicDivisionLimit + " !!!"); } 

                this.AppendParts(plist);
            }

            var setting = MusicalSettings.Singleton;
            this.linearizer.TransferPartsToLines(settingImport.SkipNegligibleTones);

            var context = new MusicalContext(setting, this.Block.Header);
            this.Block.Strip = new MusicalStrip(context);
            foreach (var t in this.linearizer.Lines) {
                this.Block.AddContentLine(t);
            }

            this.Block.LoadFirstStatusToLines(); //// 2019/10
            this.Block.ConvertStripToBody(false);
            this.Block.Body.SetBodyStatusFromTones();

            return this.MusicalBundle;
        }

        /// <summary>
        /// Read Musical Part.
        /// </summary>
        /// <param name="part">Musical part.</param>
        private void ReadMusicalDivision(XContainer part)
        {
            Contract.Requires(part != null);
            var measures = part.Elements("measure");
            //// int barNumber = 0;
            foreach (var measure in measures) {
                //// byte numberOfStaves = 1;
                //// barNumber++;
                var attributes = measure.Element("attributes");
                if (attributes == null) {
                    continue;
                }

                var time = attributes.Element("time");
                if (time != null) {
                    var symbol = (string)time.Attribute("symbol");
                    if (string.CompareOrdinal(symbol, "cut") == 0) {
                        this.Block.Header.Metric.MetricBeat = 4;
                        this.Block.Header.Metric.MetricBase = 2;
                    }
                    else {
                        var b = time.Element("beats");
                        if (b != null && !b.IsEmpty) {
                            this.Block.Header.Metric.MetricBeat = (byte)(int)b;
                        }

                        var bt = time.Element("beat-type");
                        if (bt != null && !bt.IsEmpty) {
                            var metricGround = (byte)(int)bt;
                            this.Block.Header.Metric.MetricBase = MusicalProperties.GetMetricBase(metricGround);
                        }
                    }
                }

                var divisions = attributes.Element("divisions");
                if (divisions == null) {
                    continue;
                }

                var beatDivision = (int)divisions;
                this.LocalDivision = this.Block.Header.Metric.MetricBeat * beatDivision;
                this.CommonDivision = (int)MathSupport.LeastCommonMultiple(this.CommonDivision, this.LocalDivision);
                ////  string staves = (string)attributes.Element("staves");
                ////  if (staves != null) { numberOfStaves = byte.Parse(staves); }
            }
        }

        /// <summary>
        /// Read Musical Part.
        /// </summary>
        /// <param name="part">Musical part.</param>
        /// <param name="lineIndex">Line number.</param>
        /// <returns> Returns value. </returns> 
        private MusicalPart ReadMusicalPart(XElement part, int lineIndex)
        {
            Contract.Requires(part != null);
            var partId = (string)part.Attribute("id");
            var scorePartObject = this.Header.ScoreParts[partId];
            var musicalPart = MusicalPart.GetNewMusicalPart(this.Block, lineIndex, scorePartObject.MidiChannel);
            musicalPart.PartId = partId;
            musicalPart.Purpose = LinePurpose.Fixed;  //// 2019/01 LinePurpose.Fixed;
            musicalPart.LineType = MusicalLineType.Melodic;
            var measures = part.Elements("measure");
            var barNumber = 0;
            foreach (var measure in measures) {
                barNumber++;
                var attributes = measure.Element("attributes");
                var divisions = attributes?.Element("divisions");
                if (divisions != null) {
                    var beatDivision = (int)divisions;
                    this.LocalDivision = this.Block.Header.Metric.MetricBeat * beatDivision;
                }

                var musicXmlMeasure = new MusicXmlMeasure(this.Header);
                musicXmlMeasure.ReadMusicalBar(scorePartObject, musicalPart, measure, barNumber, this);
            }

            this.Block.Header.NumberOfBars = barNumber;
            musicalPart.LayObjectsToVoiceTracks();
            return musicalPart;
        }

        /// <summary>
        /// Appends the parts.
        /// </summary>
        /// <param name="plist">The list of parts.</param>
        private void AppendParts(IEnumerable<XElement> plist)
        {
            Contract.Requires(plist != null);
            this.linearizer = new MusicalLinearizer(null); //// block.header !?!
            byte partNumber = 0;
            foreach (var part in plist) {
                var musicalPart = this.ReadMusicalPart(part, ++partNumber);
                var scorePartObject = this.Header.ScoreParts[musicalPart.PartId];
                this.LocalPitchShift = 0;
                if (scorePartObject != null) {
                    musicalPart.Instrument = new MusicalInstrument((MidiMelodicInstrument)scorePartObject.MidiProgram);
                    musicalPart.Channel = scorePartObject.MidiChannel;
                }

                //// musicalPart.MusicalBlock = this.Block;
                musicalPart.Purpose = LinePurpose.Fixed;  //// 2019/01 LinePurpose.Fixed
                musicalPart.LineType = MusicalLineType.Melodic;
                //// track.Status.LineType = MusicalLineType.Melodic;
                this.linearizer.Parts.Add(musicalPart);
            }
        }
        #endregion
    }
}
