// <copyright file="CompactMidiStrip.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to represent an entire MIDI sequence/file.

namespace LargoSharedClasses.MidiFile
{
    using Abstract;
    using JetBrains.Annotations;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Midi;
    using Music;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a MIDI sequence containing sequence of MIDI data.
    /// </summary>
    [Serializable]
    public sealed class CompactMidiStrip : Collection<IMidiTrack>
    {
        #region Fields
        /// <summary>
        /// Instrument On Channel.
        /// </summary>
        private readonly Dictionary<MidiChannel, byte> instrumentOnChannel = new Dictionary<MidiChannel, byte>();

        /// <summary>The format of the MIDI file (0, 1, or 2).</summary>
        private int format;

        /// <summary>
        /// Musical tempo.
        /// </summary>
        private int tempo;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the CompactMidiStrip class.</summary>
        /// <param name="givenFormat">
        /// The format for the MIDI file (0, 1, or 2).
        /// 0 - a single multi-channel track
        /// 1 - one or more simultaneous tracks
        /// 2 - one or more sequentially independent single-track patterns.
        /// </param>
        /// <param name="givenDivision">The meaning of the delta-times in the file.</param>
        public CompactMidiStrip(int givenFormat, int givenDivision) {
            // Store values
            this.Header = new MusicalHeader { Division = givenDivision };
            this.Format = givenFormat;
            this.Header.Metric = new MusicalMetric(1, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiStrip"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="staffGrouping">if set to <c>true</c> [staff grouping].</param>
        public CompactMidiStrip(MusicalBlock givenBlock, bool staffGrouping) {
            Contract.Requires(givenBlock != null);
            if (givenBlock == null) {
                return;
            }
            
            this.Block = givenBlock;
            this.Format = 1; //// 2021/01
            var sortedLines = (from mt in this.Block.Strip.Lines orderby mt.LineIndex select mt).ToList();

            if (staffGrouping) {
                var musicalPart = this.WriteToPart(sortedLines);
                musicalPart.MoveObjectsToStaffTracks();
                var musicalLines = new List<MusicalLine>(musicalPart.MusicalLines);
                this.LoadLines(musicalLines);
                return; 
            }

            this.LoadLines(sortedLines);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiStrip"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="givenLines">The given lines.</param>
        public CompactMidiStrip(MusicalBlock givenBlock, IEnumerable<MusicalLine> givenLines) : this(1, givenBlock.Header.Division) {
            this.Block = givenBlock;
            this.Header = (MusicalHeader)givenBlock.Header.Clone();
            this.LoadLines(givenLines);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the internal.
        /// </summary>
        /// <value>
        /// The name of the internal.
        /// </value>
        public string InternalName { get; set; }

        /// <summary> Gets or sets the format of the sequence.
        /// The format for the MIDI file (0, 1, or 2).
        /// 0 - a single multi-channel track
        /// 1 - one or more simultaneous tracks
        /// 2 - one or more sequentially independent single-track patterns.
        /// </summary>
        /// <value> Property description. </value>
        public int Format {
            get => this.format;

            set {
                if (value < 0 || value > 2) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Format must be 0, 1, or 2.");
                }

                this.format = value;
            }
        }

        /// <summary>Gets the number of sequence in the sequence.</summary>
        /// <value> Property description. </value>/// 
        public int NumberOfLines => this.Count;

        /// <summary>
        /// Gets count of loaded parts.
        /// </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public int NumberOfMelodicLines {
            get {
                var num = (from mtrack in this
                           where !mtrack.IsEmpty && mtrack.IsMelodic
                           select 1)
                           .Count();

                return num;
            }
        }

        /// <summary>Gets or sets the rhythmical order.</summary>
        /// <value> Property description. </value>
        public int Tempo {
            get {
                if (this.tempo > 0) {
                    return this.tempo;
                }

                foreach (var mt in this.Where(mt => mt != null && mt.Events.Count > 0)) {
                    this.tempo = mt.Tempo;
                    break;
                }

                return this.tempo;
            }

            set => this.tempo = value;
        }

        /// <summary>
        /// Gets the rhythmic order.
        /// </summary>
        /// <value> Property description. </value>
        public byte RhythmicOrder {
            get {
                var order = MusicalProperties.RhythmicOrder(this.Header.Division, this.Header.Metric.MetricBeat, this.Header.Metric.MetricGround);
                return order;
            }
        }

        /// <summary>
        /// Gets count of loaded parts.
        /// </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public int TotalSizeOfTracks {
            get {
                var cnt = (from mtrack in this
                           where !mtrack.IsEmpty //// && mtrack.IsMelodic
                           select mtrack.Events.Count)
                           .Sum();

                return cnt;
            }
        }

        /// <summary>
        /// Gets the number of instruments.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int NumberOfInstruments {
            get {
                var events = this.VoiceProgramChangeEvents;
                var instr = (from VoiceProgramChange ev in events select ev.Number).Distinct();
                return instr.Count();
            }
        }

        /// <summary>
        /// Gets the main instrument.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public MidiMelodicInstrument MainInstrument {
            get {
                var events = this.VoiceProgramChangeEvents;
                var instr = (from VoiceProgramChange ev in events orderby ev.Number descending select ev.Number).FirstOrDefault();
                return (MidiMelodicInstrument)instr;
            }
        }

        #endregion

        #region Bar properties
        /// <summary>
        /// Gets or sets the duration of the bar midi.
        /// </summary>
        /// <value>
        /// The duration of the bar midi.
        /// </value>
        public int BarMidiDuration { get; set; }

        /// <summary>
        /// Gets or sets the number of bars.
        /// </summary>
        /// <value>
        /// The number of bars.
        /// </value>
        public int NumberOfBars { get; set; }

        #endregion

        #region Private properties
        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        private MusicalBlock Block { get; set; }

        /// <summary>
        /// Gets the voice program change events.
        /// </summary>
        [UsedImplicitly]
        private List<VoiceProgramChange> VoiceProgramChangeEvents {
            get {
                var instrEvents = new List<VoiceProgramChange>();
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var track in this) {
                    var trackEvents = track?.MelodicInstrumentationEvents;
                    if (trackEvents != null && trackEvents.Count > 0) {
                        instrEvents.AddRange(trackEvents);
                    }
                }

                return instrEvents;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                            "MidiStrip {0} Bars {1} Bar {2} Tempo {3}", this.Header, this.NumberOfBars, this.BarMidiDuration, this.tempo);

            return s.ToString();
        }

        /// <summary>Writes the sequence to a string in human-readable form.</summary>
        /// <returns>A human-readable representation of the sequence and events in the sequence.</returns>
        [UsedImplicitly]
        public string ToHumanReadableString()
        {
            //// Original method of Saving the Sequence
            //// Create a writer, dump to it, return the string
            string s;
            using (var writer = new StringWriter(CultureInfo.InvariantCulture)) {
                this.ToString(writer);
                s = writer.ToString();
                //// writer.Dispose();
            }

            return s;
        } 
        #endregion

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns value. </returns>
        public object Clone() {
            var strip = new CompactMidiStrip(this.Format, this.Header.Division);
            return strip;
        }

        #region Midi Lines
        /// <summary>Adds a track to the MIDI sequence.</summary>
        /// <param name="track">The complete track to be added.</param>
        public void AddTrack(IMidiTrack track) {
            // Make sure the track is valid and that is hasn't already been added.
            if (track == null) {
                throw new ArgumentNullException(nameof(track));
            }

            if (this.Contains(track)) {
                throw new MidiParserException("This track is already part of the sequence.", 0);
            }

            // If this is format 0, we can only have 1 track
            if (this.format == 0 && this.Count >= 1) {
                throw new MidiParserException("Format 0 MIDI files can only have 1 track.", 0);
            }
            //// track.Sequence = this;
            track.AssignToSequence(this);
            track.TrackNumber = this.Count;
            //// Add the track.
            this.Add(track);
        }

        /// <summary>Removes a track that has been adding to the MIDI sequence.</summary>
        /// <param name="track">The track to be removed.</param>
        [UsedImplicitly]
        public void RemoveTrack(IMidiTrack track) {
            // Remove the track
            this.Remove(track);
        }

        /// <summary>Adds a new empty track to the MIDI sequence.</summary>
        /// <returns>The new track as added to the sequence. Modifications made to the track will be reflected.</returns>
        [UsedImplicitly]
        public IMidiTrack AddEmptyTrack() {
            // Create a new track, add it, and return it
            var track = new MidiTrack();
            this.AddTrack(track);
            return track;
        }

        /// <summary>
        /// Converts the deltas to totals.
        /// </summary>
        public void RecomputeAbsoluteTimes() {
            foreach (var track in this.Where(track => track != null)) {
                track.Events.RecomputeAbsoluteTimes();
            }
        }

        /// <summary>
        /// Converts the totals to deltas.
        /// </summary>
        [UsedImplicitly]
        public void RecomputeDeltaTimes() {
            foreach (var track in this.Where(track => track != null)) {
                track.Events.RecomputeDeltaTimes();
            }
        }

        /// <summary>
        /// Gets the midi blocks.
        /// </summary>
        /// <param name="maxBlockLength">Max length of the block (Demo verse restriction).</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IEnumerable<IMidiBlock> GetMidiBlocks(int maxBlockLength) {
            this.RecomputeAbsoluteTimes();
            var events = this.GetBreakEvents();
            var solver = new MidiBlocksSolver(this, events);
            var finalEvent = this.GetLastEvent();
            var midiBlocks = solver.DetermineBlocks(finalEvent);
            if (maxBlockLength <= 0) {
                return midiBlocks;
            }

            var blocks = midiBlocks as IList<MidiBlock> ?? midiBlocks.ToList();
            //// 2019/01 foreach (var block in blocks.Where(block => block.Area.Length > maxBlockLength))  {
            //// block.Area.BarTo = block.Area.BarFrom + maxBlockLength - 1;  } 
            //// this.RecomputeDeltaTimes();
            return blocks;
        }

        #endregion

        #region Other public methods
        /// <summary>
        /// Sets the track names from meta.
        /// </summary>
        public void SetTrackNamesFromMeta() {
            foreach (var track in this) {
                if (track == null) {
                    continue;
                }

                var metaInfo = new MidiTrackMetaInfo(track);
                track.Name = metaInfo.GuessName;
            }
        }

        /// <summary>
        /// Sets the track instruments from first occurrence.
        /// </summary>
        public void SetTrackInstrumentsFromFirstOccurrence() {
            foreach (var track in this.Where(track => track != null)) {
                track.Channel = track.FirstChannelInEvents;
                track.InstrumentNumber = track.Channel == MidiChannel.DrumChannel ? (byte)track.FirstRhythmicInstrumentInEvents
                    : (byte)track.FirstMelodicInstrumentInEvents;
            }
        }

        /// <summary>
        /// Split Lines By Instruments.
        /// </summary>
        /// <returns>Returns value.</returns>
        public CompactMidiStrip SplitTracksByInstruments() {
            //// produced MIDI files have always format 1 with multiple sequence
            var trackNumber = 1;
            this.DetermineMetric();
            this.InitializeInstrumentOnChannel();

            var newTracks = new CompactMidiStrip(1, this.Header.Division) {
                Header = this.Header,
                InternalName = this.InternalName,
                Tempo = this.Tempo,
                Format = 1
            };

            foreach (var track in this.Where(track => track != null)) {
                track.Events.RecomputeAbsoluteTimes();

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var channel in this.instrumentOnChannel.Keys) {
                    var channel1 = channel;
                    if (!track.ExistsAnyEventVoice(channel1)) {
                        continue;
                    }

                    if (this.Header.Metric.MetricGround == 0) {
                        continue;
                    }

                    var newTrack = this.NewTrackWithChannel(ref trackNumber, track, channel1);
                    if (newTrack.HasEndOfTrack) {
                        newTracks.AddTrack(newTrack);
                    }
                }
            }

            return newTracks;
        }

        /// <summary>
        /// Invert Vertically.
        /// </summary>
        [UsedImplicitly]
        public void InvertVertically() {
            //// // If the event is a VoiceNoteOn, VoiceNoteOff, or Aftertouch, shift the note
            //// // according to the supplied number of steps.
            foreach (var voiceEvent in
                (from track in this
                 where track != null
                 from ev in track.Events
                 where ev != null && ev.IsVoiceNoteEvent
                 select ev).OfType<VoiceAbstractNote>()) {
                voiceEvent.Note = (byte)(128 - voiceEvent.Note);
            }
        }

        /// <summary>
        /// Gets the tempo events.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public IEnumerable<IMidiEvent> GetTempoEvents() {
            var tempoEvents = new MidiEventCollection();
            foreach (var track in this.Where(track => track != null)) {
                tempoEvents.AddRange(track.Events.GetTempoEvents);
            }

            var events = (from MidiEvent me in tempoEvents
                          orderby me.StartTime
                          select me).ToList();
            return events;
        }

        /// <summary>
        /// Gets the tempo events.
        /// </summary>
        /// <param name="startTimeFrom">The start time from.</param>
        /// <param name="startTimeTo">The start time to.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IEnumerable<IMidiEvent> GetTempoEvents(long startTimeFrom, long startTimeTo) {
            var tempoEvents = new MidiEventCollection();
            foreach (var track in this.Where(track => track != null)) {
                tempoEvents.AddRange(track.Events.GetTempoEvents);
            }

            var events = (from MidiEvent me in tempoEvents
                          where me.StartTime >= startTimeFrom && me.StartTime <= startTimeTo
                          orderby me.StartTime
                          select me).ToList();
            //// 2013/11 All Tempo events have to have DeltaTime zero.
            foreach (var me in events.Where(me => me.DeltaTime > 0)) {
                me.DeltaTime = 0;
            }

            return events;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the break events.
        /// </summary>
        /// <returns> Returns value. </returns>
        private IMidiEvent GetLastEvent() {
            IMidiEvent lastEvent = new MidiEvent(0);

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (var track in this) {
                var ev = track?.Events.LastOrDefault();
                if (ev != null && ev.StartTime > lastEvent.StartTime) {
                    lastEvent = ev;
                }
            }

            return lastEvent;
        }

        /// <summary>
        /// Initializes the instrument on channel.
        /// </summary>
        private void DetermineMetric() {
            this.Header.Metric.Reset();

            foreach (var track in this.Where(track => track != null)) {
                if (track.Metric.MetricBeat > this.Header.Metric.MetricBeat) {
                    this.Header.Metric.MetricBeat = track.Metric.MetricBeat;
                }

                // ReSharper disable once InvertIf
                if (track.Metric.MetricBase > this.Header.Metric.MetricBase) {
                    this.Header.Metric.MetricBase = track.Metric.MetricBase;
                }
            }

            //// There is no TimeSignature in some midi files
            if (this.Header.Metric.MetricBeat == 0) {
                this.Header.Metric.MetricBeat = 4;
            }

            // ReSharper disable once InvertIf
            if (this.Header.Metric.MetricBase == 0) {
                this.Header.Metric.MetricBase = 2;
            }
        }

        /// <summary>
        /// Initialize Instrument On Channel.
        /// </summary>
        private void InitializeInstrumentOnChannel() {
            Contract.Requires(this.instrumentOnChannel != null);

            for (byte channel = 0; channel < 16; channel++) {
                this.instrumentOnChannel[(MidiChannel)channel] = 0;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var track in this) {
                if (track == null) {
                    continue;
                }

                foreach (var voiceChange in
                    (from ev in track.Events
                     let eventType = ev.EventType
                     where string.CompareOrdinal(eventType, "VoiceProgramChange") == 0
                     select ev).OfType<VoiceProgramChange>()) {
                    this.instrumentOnChannel[voiceChange.Channel] = voiceChange.Number;
                }
            }
        }

        /// <summary>
        /// Gets the break events.
        /// </summary>
        /// <returns> Returns value. </returns>
        private IEnumerable<MidiEvent> GetBreakEvents() {
            var breakEvents = new MidiEventCollection();
            foreach (var track in this.Where(track => track != null)) {
                breakEvents.AddRange(track.Events.GetBreakEvents);
            }

            var events = (from MidiEvent me in breakEvents orderby me.StartTime select me).ToList();
            return events;
        }

        /// <summary>Dumps the MIDI sequence to the writer in human-readable form.</summary>
        /// <param name="writer">The writer to which the track should be written.</param>
        [UsedImplicitly]
        private void ToString(TextWriter writer) {
            // Validate the writer
            if (writer == null) {
                return;     //// throw new ArgumentNullException("writer");
            }

            // Info on sequence
            writer.WriteLine("MIDI Sequence");
            writer.WriteLine("Format: " + this.format);
            writer.WriteLine("Lines: " + this.Count);
            writer.WriteLine("Division: " + this.Header.Division);
            writer.WriteLine(string.Empty);

            // Print out each track
            foreach (var track in this.Where(track => track != null)) {
                writer.WriteLine(track.ToString());
            }
        }

        /// <summary>
        /// News the track with channel.
        /// </summary>
        /// <param name="trackNumber">The track number.</param>
        /// <param name="track">The track.</param>
        /// <param name="channel1">The channel1.</param>
        /// <returns> Returns value. </returns>
        private MidiTrack NewTrackWithChannel(ref int trackNumber, IMidiTrack track, MidiChannel channel1) {
            Contract.Requires(this.instrumentOnChannel != null);
            Contract.Requires(track != null);

            var newTrack = new MidiTrack {
                Name = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", track.Name, channel1),
                Channel = channel1,
                InstrumentNumber = this.instrumentOnChannel[channel1],
                TrackNumber = trackNumber++,
                Metric = (MusicalMetric)this.Header.Metric.Clone(),
                BarDivision = MusicalProperties.BarDivision(this.Header.Division, this.Header.Metric.MetricBeat, this.Header.Metric.MetricGround)
            };

            //// newTrack.Events.Add(new LargoBaseMusic.Midi.ProgramChange(0, newTrack.Channel, (MidiMelodicInstrument)newTrack.Instrument));

            newTrack.AddEventVoiceClones(track.Events, channel1);

            var firstNoteEvent = (from ev1 in newTrack.Events.Select(ev => ev as VoiceNoteOn)
                                  where ev1 != null && ev1.Velocity > 0 && ev1.Note > 0
                                  select ev1).FirstOrDefault();
            if (firstNoteEvent != null) {
                newTrack.Octave = (MusicalOctave)(firstNoteEvent.Note / DefaultValue.HarmonicOrder);
                newTrack.BandType = MusicalProperties.BandTypeFromOctave(newTrack.Octave);
            }

            newTrack.Events.RecomputeDeltaTimes();
            return newTrack;
        }

        #endregion

        /// <summary>
        /// Write To Part.
        /// </summary>
        /// <param name="givenLines">The given lines.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MusicalPart WriteToPart(IList<MusicalLine> givenLines) {
            var musicalPart = new MusicalPart(this.Block);
            foreach (var line in givenLines.Where(mt => mt.Tones != null)) {
                musicalPart.Purpose = line.Purpose;
                foreach (var musicalTone in line.Tones) {
                    //// it would be assigned somewhere above
                    //// see  melodicTone.Staff = this.ComposedTrack.Status.Staff; ....
                    //// mtone.Staff = mt.Staff;  //// mtone.Voice = mt.Voice; 
                    //// mtone.Instrument = mt.Instrument; //// mtone.Channel = (MidiChannel)mt.Channel;
                    musicalPart.AddMusicalObject(musicalTone);
                }
            }

            return musicalPart;
        }

        /// <summary>
        /// Loads the lines.
        /// </summary>
        /// <param name="givenLines">The given lines.</param>
        /// <exception cref="ArgumentNullException">Null in given lines.</exception>
        /// <exception cref="InvalidOperationException">
        /// No tracks.
        /// or
        /// Tempo value is zero.
        /// </exception>
        private void LoadLines(IEnumerable<MusicalLine> givenLines) {
            if (givenLines == null) {
                throw new ArgumentNullException(nameof(givenLines));
            }

            var listLines = givenLines.ToList(); //// resharper
            if (!listLines.Any()) {
                //// return null;
                throw new InvalidOperationException("No tracks.");
            }

            if (this.Block.Header.Tempo == 0) {
                throw new InvalidOperationException("Tempo value is zero.");
            }

            var zeroTrack = new CompactMidiTrack(this.Block); //// 2014/01
            zeroTrack.FinishTrackMidiExport(this);

            var writeHarmony = true;
            //// bool writeTempo = true;
            for (var ip = 0; ip < listLines.Count; ip++) {
                var line = listLines.ElementAt(ip);
                var track = new CompactMidiTrack(this.Block, line);
                track.WriteTrack(this, line.Tones, false, ref writeHarmony);
            }
        }
    }
}