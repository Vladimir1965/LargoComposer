// <copyright file="MidiFile.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Midi;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Midi File.
    /// </summary>
    public class MidiFile {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiFile" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public MidiFile(string filePath) {
            Contract.Requires(this.FilePath != null);
            //// Validate input
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentNullException(nameof(this.FilePath), "No path provided.");
            }

            this.FilePath = filePath;
            this.FileName = Path.GetFileNameWithoutExtension(this.FilePath.Trim());
            this.Import();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiFile"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="givenSequence">The given sequence.</param>
        /// <exception cref="ArgumentNullException">FilePath - No path provided.</exception>
        public MidiFile(string filePath,  CompactMidiStrip givenSequence)
        {
            Contract.Requires(this.FilePath != null);
            //// Validate input
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentNullException(nameof(this.FilePath), "No path provided.");
            }

            this.FilePath = filePath;
            this.FileName = Path.GetFileNameWithoutExtension(this.FilePath.Trim());
            this.Sequence = givenSequence;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public CompactMidiStrip Sequence { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has valid sequence.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance has valid sequence; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidSequence => this.Sequence != null && this.Sequence.Count > 0;

        #endregion 

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Path: {0}", this.FilePath);

            return s.ToString();
        }
        #endregion

        /// <summary>
        /// Creates a MIDI file at the specified path and writes the sequence to it.
        /// </summary>
        public void Save()
        {
            Contract.Requires(this.Sequence != null);

            // Create the output file and save to it
            using (var stream = new FileStream(this.FilePath, FileMode.Create)) {
                this.Save(stream);
            }
        }

        #region Private methods - Importing from MIDI file
        /// <summary>
        /// Reads a MIDI stream into a new CompactMidiStrip.
        /// </summary>
        private void Import()
        {
            //// Open the file
            //// try {
            //// using (FileStream inputStream = new FileStream(filePath, FileMode.Open)) {
            using (var inputStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                // Parse it and return the sequence
                this.Import(inputStream); //// Avoid multiple or conditional return statements.
            }
            //// }
            //// IOException
            //// UnauthorizedAccessException
        }

        /// <summary>
        /// Reads a MIDI stream into a new CompactMidiStrip.
        /// </summary>
        /// <param name="inputStream">The stream containing the MIDI data.</param>
        /// <exception cref="ArgumentNullException">Input Stream Exception.</exception>
        /// <exception cref="MidiParserException">Can't read the MIDI file. - 0
        /// or
        /// Invalid MIDI header. - 0</exception>
        private void Import(Stream inputStream)
        {
            //// Validate input
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (!inputStream.CanRead) {
                throw new MidiParserException("Can't read the MIDI file.", 0);
            }

            //// Read in the main MIDI header
            var mainHeader = MidiFileChunkHeader.Read(inputStream);
            if (mainHeader.NumberOfLines < 0) {
                throw new MidiParserException("Invalid MIDI header.", 0);
            }
            //// Read in all of the tracks
            var trackChunks = new MidiTrackChunkHeader[mainHeader.NumberOfLines];
            for (var i = 0; i < mainHeader.NumberOfLines; i++) {
                trackChunks[i] = MidiTrackChunkHeader.Read(inputStream);
            }

            // Create the MIDI sequence
            this.Sequence = new CompactMidiStrip(mainHeader.Format, mainHeader.Division)
            {
                Header =
                {
                    Specification = this.FileName,
                    //// FilePath = this.FilePath,
                    FileName = Path.GetFileNameWithoutExtension(this.FilePath)
                }
            };

            for (var i = 0; i < mainHeader.NumberOfLines; i++) {
                var data = trackChunks[i].GetData();
                if (data == null) {
                    continue;
                }

                var parser = new MidiParser(data, null);
                this.Sequence.AddTrack(parser.ParseToTrack());
            }

            this.Sequence.SetTrackNamesFromMeta();

            //// 2015/01
            if (this.Sequence.Count > 0) {
                this.Sequence.Header.Metric = this.Sequence[0].Metric;
            }

            //// sequence.SetMetricFromTracks();
            //// sequence.SetTracksMetric();

            this.Sequence.SetTrackInstrumentsFromFirstOccurrence();
        }

        #endregion

        #region Private mothods - Exporting to MIDI file
        /// <summary>
        /// Writes a MIDI file header out to the stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <remarks>This functionality is automatically performed during a Save.</remarks>
        private void WriteHeader(Stream outputStream)
        {
            // Check parameters
            if (this.Sequence == null) {
                throw new ArgumentNullException(nameof(this.Sequence));
            }

            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new MidiParserException("Can't write to stream.", 0);
            }

            if (this.Sequence.Count < 1) {
                throw new ArgumentOutOfRangeException(nameof(this.Sequence), this.Sequence.Count, "Sequences require at least 1 track.");
            }

            // Write out the main header for the sequence
            var mainHeader = new MidiFileChunkHeader(this.Sequence.Format, this.Sequence.Count, this.Sequence.Header.Division);
            mainHeader.Write(outputStream);
        }

        /// <summary>
        /// Writes the MIDI sequence to the output stream.
        /// </summary>
        /// <param name="outputStream">The stream to which the MIDI sequence should be written.</param>
        private void Save(Stream outputStream)
        {
            //// Contract.Requires(sequence != null);
            //// Contract.Requires(outputStream != null);

            //// Check valid state (as best we can check)
            if (this.Sequence == null || this.Sequence.Count < 1) {
                throw new InvalidOperationException("Empty sequence (no musical tracks have been added).");
            }

            //// Check parameters
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new ArgumentException("Can't write to stream.", nameof(outputStream));
            }

            // Write out the main header for the sequence
            this.WriteHeader(outputStream);

            // Write out each track in the order it was added to the sequence
            foreach (var track in this.Sequence.Where(track => track != null)) {
                //// 2020/10 add or not here
                //// track.Events.SortByStartTime();
                //// track.Events.RecomputeDeltaTimes();

                track.Write(outputStream);
            }

            //// DoNotCatchGeneralExceptionTypes   try { catch (Exception exc) .. "Unable to save the sequence to a local file."
        }

        #endregion
    }
}
