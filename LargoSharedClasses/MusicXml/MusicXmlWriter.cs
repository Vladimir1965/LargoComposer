// <copyright file="MusicXmlWriter.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MusicXml
{
    /// <summary>
    /// Music Xml.
    /// </summary>
    public sealed class MusicXmlWriter {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicXmlWriter class.
        /// </summary>
        /// <param name="givenFile">The given file.</param>
        public MusicXmlWriter(MusicalBundle givenFile) {
            Contract.Requires(givenFile != null);
            this.MusicalBundle = givenFile;
            this.MusicalBlock = givenFile.Blocks.FirstOrDefault();
            var musicalBlock = this.MusicalBlock;
            if (musicalBlock != null) {
                this.MusicalLines = (from mt in musicalBlock.Strip.Lines orderby mt.LineIndex select mt).ToList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicXmlWriter"/> class.
        /// </summary>
        [UsedImplicitly]
        public MusicXmlWriter() {    
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets Musical File.
        /// </summary>
        /// <value> Property description. </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private MusicalBundle MusicalBundle { get; }

        /// <summary> Gets Musical block. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        private MusicalBlock MusicalBlock { get; }

        /// <summary> Gets file name. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        private List<MusicalLine> MusicalLines { get; }

        /// <summary>
        /// Gets or sets MusicXml Header.
        /// </summary>
        private MusicXmlHeader Header { get; set; }
        #endregion

        #region Public interface
        /// <summary>
        /// Write Music Xml.
        /// </summary>
        /// <param name="streamWriter">Stream writer.</param>
        public void WriteTo(TextWriter streamWriter) {
            Contract.Requires(streamWriter != null);
            //// Document type
            //// "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>";
            var decl = new XDeclaration("1.0", "utf-8", "no");
            var comment = new XComment("Music Xml");
            //// XProcessingInstruction pi = new XProcessingInstruction();
            var doc = new XDocument(decl, comment);
            //// "<!DOCTYPE score-partwise PUBLIC \"-//Recordare//DTD MusicXml 3.0 Partwise//EN\" \"http://www.musicxml.org/dtds/partwise.dtd\">";
            var documentType = new XDocumentType("score-partwise", "-//Recordare//DTD MusicXml 3.0 Partwise//EN", "http://www.musicxml.org/dtds/partwise.dtd", null);
            doc.Add(documentType);

            //// Score Partwise -  doc.Root element name ="score-partwise"
            var scorePartwise = new XElement(
                                            "score-partwise",
                                            new XAttribute("version", "3.0"));
            this.Header = new MusicXmlHeader(scorePartwise, this.MusicalBlock);
            this.Header.WriteXmlHeader();
            var element = this.ScorePartListElement();
            this.Header.ScorePartwise.Add(element);

            //// ?????   XElement element = this.TempoElement();  scorePartwise.Add(element); 
            if (this.MusicalLines != null) {
                foreach (var partElement in this.MusicalLines.Select(this.PartElement)) {
                    scorePartwise.Add(partElement);
                }
            }

            doc.Add(scorePartwise);

            //// Saving
            var outputSettings = new XmlWriterSettings { NewLineOnAttributes = true, Indent = true };
            using (var xmlWriter = XmlWriter.Create(streamWriter, outputSettings)) {
                doc.Save(xmlWriter);
            }
        }
        #endregion

        #region ScorePartList
        /// <summary>
        /// Score Part List element.
        /// </summary>
        /// <returns> Returns value. </returns>
        private XElement ScorePartListElement() {
            var partList = new XElement("part-list");
            if (this.MusicalBlock == null) {
                return partList;
            }

            foreach (var track in this.MusicalBlock.Strip.Lines) {
                var instr = track.FirstStatus.Instrument?.MelodicInstrument ?? MidiMelodicInstrument.AcousticGrandPiano;
                if (track.FirstStatus.Instrument != null)
                {
                    var number = track.FirstStatus.Instrument.Number;     
                    var scorePartObject = new ScorePartObject {
                        Id = "P" + track.LineIndex,
                        PartName = instr.ToString(),
                        ScoreInstrumentId = "I" + number,
                        MidiInstrumentId = "I" + number,
                        InstrumentName = instr.ToString(),
                        MidiProgram = number,
                        MidiChannel = track.MainVoice.Channel
                    };

                    var scorePart = scorePartObject.ScorePart();
                    partList.Add(scorePart);
                }
            }

            return partList;
        }

        #endregion

        #region Musical Parts
   
        /// <summary>
        /// Part element.
        /// </summary>
        /// <param name="track">Musical track.</param>
        /// <returns> Returns value. </returns>
        private XElement PartElement(MusicalLine track) {
            Contract.Requires(this.MusicalBlock != null);
            Contract.Requires(track != null);
            var part = new XElement(
                    "part",
                    new XAttribute("id", "P" + track.LineIndex));
            for (var barNumber = 1; barNumber <= this.MusicalBlock.Header.NumberOfBars; barNumber++) {
                //// MusicXmlMeasure musicXmlMeasure = new MusicXmlMeasure(this.Header);
                var measure = MusicXmlMeasure.MeasureElement(barNumber, track, this.MusicalBlock.Header);
                part.Add(measure);
            }

            return part;
        }

        #endregion
    }
}
