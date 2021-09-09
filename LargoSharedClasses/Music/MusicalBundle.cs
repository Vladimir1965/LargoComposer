// <copyright file="MusicalBundle.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical File.
    /// </summary>
    [Serializable]
    public sealed class MusicalBundle
    {
        #region Fields
        /// <summary>
        /// Name of the model.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Musical Blocks.
        /// </summary>
        private Collection<MusicalBlock> blocks;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBundle"/> class.
        /// </summary>
        public MusicalBundle() {
            this.blocks = new Collection<MusicalBlock>();
            this.FileHeading = new FileHeading();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBundle" /> class.
        /// </summary>
        /// <param name="xbundle">The marked file.</param>
        public MusicalBundle(XElement xbundle)
            : this() {
            Contract.Requires(xbundle != null);
            if (xbundle == null) {
                return;
            }

            //// XElement xheading = xbundle.Element("Heading");
            //// this.FileHeading = new FileHeading(xheading);            
            this.FileName = (string)xbundle.Attribute("FileName");

            XElement xblocks = xbundle.Element("Blocks");
            if (xblocks == null) {
                return;
            }

            //// Blocks
            foreach (var xblock in xblocks.Elements()) {
                MusicalBlock block = new MusicalBlock(xblock) {
                    MusicalBundle = this
                    //// FileName = this.Name
                };

                this.Blocks.Add(block);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the maximum number of bars in block.
        /// </summary>
        /// <value>
        /// The maximum number of bars in block.
        /// </value>
        public static int MaxNumberOfBarsInBlock { get; set; }   //// CA1044 (FxCop)

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xbundle = new XElement(
                         "MIF",
                          new XAttribute("FileName", this.FileName));

                XElement xcopyright = new XElement(
                    "Software",
                    new XAttribute("Name", SettingsApplication.ApplicationName),
                    new XAttribute("Www", SettingsApplication.ApplicationWeb));
                xbundle.Add(xcopyright);

                //// var xheading = this.FileHeading.GetXElement;
                //// xbundle.Add(xheading);

                //// Blocks
                XElement xblocks = new XElement("Blocks");
                foreach (MusicalBlock block in this.Blocks.Where(block => block != null)) {
                    var xblock = block.GetXElement;
                    xblocks.Add(xblock);
                }

                xbundle.Add(xblocks);
                return xbundle;
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the original path.
        /// </summary>
        /// <value>
        /// The original path.
        /// </value>
        public string OriginalPath { get; set; }

        /// <summary>
        /// Gets the blocks.
        /// </summary>
        /// <value>
        /// The blocks.
        /// </value>
        public Collection<MusicalBlock> Blocks {
            get {
                Contract.Ensures(Contract.Result<Collection<MusicalBlock>>() != null);
                return this.blocks;
            }
        }

        /// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
        /// <value>
        /// The type of the model.
        /// </value>
        public MusicalOriginType OriginType { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the file heading.
        /// </summary>
        /// <value>
        /// The file heading.
        /// </value>
        public FileHeading FileHeading { get; set; }

        #endregion

        #region Public Properties - Naming

        /// <summary> Gets or sets file name. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public string FileName {
            get {
                Contract.Ensures(Contract.Result<string>() != null);
                if (this.fileName == null) {
                    throw new InvalidOperationException("File name is null.");
                }

                return this.fileName;
            }

            set {
                if (value == null) {
                    this.fileName = string.Empty;
                    //// throw new ArgumentException("Name cannot be set null.", "value");
                    return;
                }

                this.fileName = value;
            }
        }

        /// <summary>
        /// Gets the actual name.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string ActualName => string.Format(
            CultureInfo.CurrentCulture,
            "{0}_{1}{2}", 
            this.NameAndOrigin,
            SupportCommon.DateTimeIdentifier, 
            this.OrchestrationName.ClearSpecialChars() ?? string.Empty).Trim();

        /// <summary> Gets or sets ComposerId. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public int? ComposerId { get; set; }   //// CA1044 (FxCop)

        #endregion

        #region Public properties
        /// <summary>
        /// Gets Musical Identification.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public IList<KeyValuePair> Identification {
            get {
                var items = new List<KeyValuePair>();
                var item = new KeyValuePair("FileName", this.FileName);
                items.Add(item);

                //// items.AddRange(this.FileHeading.Identification);
                return items;
            }
        }

        /// <summary>
        /// Gets the identification string.
        /// </summary>
        /// <value>
        /// The identification string.
        /// </value>
        [UsedImplicitly]
        public string IdentificationString {
            get {
                StringBuilder sb = new StringBuilder();
                var idents = this.Identification;
                foreach (var ident in idents) {
                    sb.AppendFormat("{0}: {1}\n", ident.Key, ident.Value);
                }

                foreach (var block in this.blocks) {
                    sb.AppendLine(string.Empty);
                    sb.AppendFormat("{0}/ {1}\n", block.Header.Number, block.Header.Specification);
                    sb.Append(block.IdentificationString);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Selected.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public bool IsSelected { get; set; }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the name and origin.
        /// </summary>
        /// <value> Property description. </value>
        private string NameAndOrigin {
            get {
                var origin = this.OriginType != MusicalOriginType.None ? this.OriginType.ToString() : string.Empty;
                var clearName = string.Format(CultureInfo.CurrentCulture, "{0}{1}", this.FileName.ClearSpecialChars(), origin);
                return clearName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the orchestration.
        /// </summary>
        /// <value>
        /// The name of the orchestration.
        /// </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private string OrchestrationName { get; set; }
        #endregion

        #region Static factory methods
        /// <summary>
        /// Gets the new musical file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns> Returns value. </returns>
        public static MusicalBundle GetNewMusicalBundle(string fileName) {
            var musicBundle = new MusicalBundle {
                FileName = fileName,
                blocks = new Collection<MusicalBlock>(),
                FileHeading = { WorkTitle = fileName }
            };
            return musicBundle;
        }

        /// <summary>
        /// Gets the new musical file.
        /// </summary>
        /// <param name="musicalBlock">The musical block.</param>
        /// <param name="givenFileName">The file name.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalBundle GetEnvelopeOfBlock(MusicalBlock musicalBlock, string givenFileName) {
            var musicBundle = new MusicalBundle {
                FileName = givenFileName,
                
                blocks = new Collection<MusicalBlock>(),
                FileHeading = { WorkTitle = givenFileName }
            };

            if (musicalBlock != null) {
                musicBundle.Blocks.Add(musicalBlock);
                musicalBlock.MusicalBundle = musicBundle;
            }

            return musicBundle;
        }

        /// <summary>
        /// Musicals the file.
        /// </summary>
        /// <param name="givenSequence">The sequence.</param>
        /// <param name="settingImport">The setting import.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalBundle GetMusicalBundle(CompactMidiStrip givenSequence, SettingsImport settingImport) {
            Contract.Requires(givenSequence != null);

            const byte harmonicOrder = DefaultValue.HarmonicOrder;
            //// Track can be saved only after back writing of events
            var musicalBundle = GetNewMusicalBundle(givenSequence.InternalName);
            musicalBundle.OriginType = MusicalOriginType.Original;

            if (givenSequence.Format == 0) {
                givenSequence = givenSequence.SplitTracksByInstruments();
                givenSequence.SetTrackInstrumentsFromFirstOccurrence();
            }

            var midiBlocks = givenSequence.GetMidiBlocks(MaxNumberOfBarsInBlock);

            foreach (var midiBlock in midiBlocks) {
                var rhythmicOrder = MusicalProperties.RhythmicOrder(midiBlock.Header.Division, midiBlock.Header.Metric.MetricBeat, midiBlock.Header.Metric.MetricGround);
                var block = MusicalBlock.NewMusicalBlock((MidiBlock)midiBlock, harmonicOrder, rhythmicOrder); //// sequence 
                //// block.FileName = musicalBundle.Name;
                block.TonalityKey = midiBlock.TonalityKey;
                block.TonalityGenus = midiBlock.TonalityGenus;

                if (block.Header.NumberOfMelodicLines > 0) {
                    MusicalLinearizer linearizer = new MusicalLinearizer(block.Header) { Lines = block.Strip.Lines };
                    linearizer.SplitLinesToParts(block, settingImport.SplitMultiTracks);
                    linearizer.TransferPartsToLines(settingImport.SkipNegligibleTones);
                    block.Strip.SetLines(linearizer.Lines);
                    block.Strip.RebuildChannels();
                    block.LoadFirstStatusToLines(); //// 2019/10
                }

                //// Skip block having only negligible tracks!? (2019/03)
                if (!block.Strip.Lines.Any()) {
                    continue;
                }

                block.ConvertStripToBody(false);

                var toneTracks = (from mt in ((MidiBlock)midiBlock).Sequence where mt.Sequence != null select mt)
                    .ToList();
                block.Body.SetTempoEventsFrom(midiBlock.MidiTimeFrom, midiBlock.MidiTimeTo, toneTracks);

                //// Status (lists of bars in tracks) will be transformed to blockStatus.
                //// List of bar status in each given line have to be converted to Status in elements.
                //// Read from midi file needs status from tones...!?
                block.Body.SetBodyStatusFromTones();

                //// MusicMidiWriter writer = new MusicMidiWriter(block);
                //// CompactMidiStrip midiTracks = writer.WriteToSequence(false); //// staff grouping
                //// block.SplitLinesToParts(splitMultiVoiceLines);
                //// block.TransferPartsToLines();

                //// MusicMidiWriter.SaveBlockMidiToDatabase(block);
                block.MusicalBundle = musicalBundle;
                musicalBundle.Blocks.Add(block);
            }

            //// 2019/01 musicalBundle.SaveToMidi(false, splitMultiVoiceLines);
            //// MusicMidiWriter.SaveMidiToDatabase(file, originalName + " (i-split)");
            musicalBundle.FileName = givenSequence.InternalName;
            return musicalBundle;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalFile object. </summary>
        /// <returns> Returns object. </returns>
        [UsedImplicitly]
        public object Clone() {
            var file = GetNewMusicalBundle(this.FileName);
            file.ComposerId = this.ComposerId;
            file.FileHeading.Encoder = this.FileHeading.Encoder;
            file.FileHeading.EncodingDate = this.FileHeading.EncodingDate;
            file.FileHeading.EncodingDescription = this.FileHeading.EncodingDescription;
            //// file.Identification = this.Identification;
            file.FileHeading.Software = this.FileHeading.Software;
            file.FileHeading.WorkNumber = this.FileHeading.WorkNumber;
            file.FileHeading.WorkTitle = this.FileHeading.WorkTitle;

            //// file.Blocks = new Collection<MusicalBlock>();
            foreach (var newBlock in this.Blocks.Select(block => block.Clone(true, true))) {
                file.Blocks.Add(newBlock);
                newBlock.MusicalBundle = file;
            }

            return file;
        }

        /// <summary>
        /// Sorts the blocks.
        /// </summary>
        [UsedImplicitly]
        public void SortBlocks() {
            var bs = (from fb in this.Blocks orderby fb.Header.Number select fb).ToList();
            this.blocks = new Collection<MusicalBlock>(bs);
        }

        /// <summary>
        /// Saves the midi of musical file.
        /// </summary>
        /// <param name="checkTracks">If set to <c>true</c> [check tracks].</param>
        /// <param name="splitMultiVoiceLines">The split multi-voice tracks.</param>
        public void SaveToMidi(bool checkTracks, FileSplit splitMultiVoiceLines) {
            foreach (var block in this.Blocks.Where(block => block.Strip.Lines.Any())) {
                if (checkTracks) {
                    //// this primarily splits percussion line according to instruments 
                    //// bool split = splitMultiVoiceLines == FileSplit.Total || (splitMultiVoiceLines == FileSplit.Automatic && block.Strip.Lines.Count < 3);

                    MusicalLinearizer linearizer = new MusicalLinearizer(block.Header) { Lines = block.Strip.Lines };
                    linearizer.SplitLinesToParts(block, splitMultiVoiceLines);
                    linearizer.TransferPartsToLines(true);
                    block.Strip.SetLines(linearizer.Lines);
                    block.Strip.RebuildChannels();
                }
                ////201508   SaveMidiOfMusicalBlock(block, MidiSourceType.ReadBlock);
            }
        }
        #endregion
    }
}
