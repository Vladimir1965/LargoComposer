// <copyright file="MidiTrack.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to represent an entire track in a MIDI file.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>Represents a single MIDI track in a MIDI file.</summary>
    [Serializable]
    public sealed class MidiTrack : IMidiTrack
    {
        /// <summary>Collection of MIDI event added to this track.</summary>
        private readonly MidiEventCollection events;

        #region Fields
        /// <summary>
        /// Musical metric.
        /// </summary>
        [NonSerialized]
        private MusicalMetric metric;

        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiTrack class.</summary>
        public MidiTrack() {
            this.Metric = new MusicalMetric(1, 0);
            //// Create the buffer to store all event information
            this.events = new MidiEventCollection();
            //// We don't yet have an end of track marker, but we want one eventually.
            this.RequireEndOfTrack = true;
        }

        /// <summary>
        /// Initializes a new instance of the MidiTrack class.
        /// </summary>
        /// <param name="collection">Collection of midi events.</param>
        public MidiTrack(MidiEventCollection collection) {
            Contract.Requires(collection != null);
            this.Metric = new MusicalMetric(1, 0);
            this.events = collection;
            //// We don't yet have an end of track marker, but we want one eventually.
            this.RequireEndOfTrack = true;
            if (!this.HasEndOfTrack) {
                this.events.Add(new MetaEndOfTrack(0));
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>
        /// The metric.
        /// </value>
        public MusicalMetric Metric {
            get {
                Contract.Ensures(Contract.Result<MusicalMetric>() != null);
                if (this.metric == null) {
                    throw new InvalidOperationException("Metric is null.");
                }

                return this.metric;
            }

            set => this.metric = value ?? throw new ArgumentException("Metric cannot be set null.", nameof(value));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets name of the collection.
        /// </summary>
        /// <value> Property description. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets BarDivision.
        /// </summary>
        /// <value> Property description. </value>
        public int BarDivision { get; set; }

        /// <summary>
        /// Gets or sets instrument.
        /// </summary>
        /// <value> Property description. </value>
        public byte InstrumentNumber { get; set; }

        /// <summary>
        /// Gets or sets Channel.
        /// </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets Channel.
        /// </summary>
        /// <value> Property description. </value>
        public byte Staff { get; set; }

        /// <summary>
        /// Gets or sets Channel.
        /// </summary>
        /// <value> Property description. </value>
        public byte Voice { get; set; }

        /// <summary>
        /// Gets or sets instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalOctave Octave { get; set; }

        /// <summary>
        /// Gets or sets instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalBand BandType { get; set; }

        /// <summary>Gets a value indicating whether an end of track event has been added.</summary>
        /// <value> Property description. </value>
        public bool HasEndOfTrack {
            get {
                // Determine whether the last event is an end of track event
                if (this.Events.Count >= 1) {
                    return this.events.ElementAt(this.Events.Count - 1) is MetaEndOfTrack; //// LastOrDefault()
                }

                return false;
            }
        }

        /// <summary>
        /// Gets Sequence.
        /// </summary>
        /// <value> Property description. </value>
        public CompactMidiStrip Sequence { get; private set; }

        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        /// <value> Property description. </value>
        public int TrackNumber { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets a value indicating whether IsEmpty.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsEmpty => this.Events.Count <= 2;

        /// <summary>
        /// Gets a value indicating whether this instance has tones.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this instance has tones; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool HasTones {
            get {
                if (this.Events.Count == 0) {
                    return false;
                }

                return (from ev in this.Events
                        where ev != null
                        let eventType = ev.EventType
                        where eventType == "VoiceNoteOn"
                        select (VoiceNoteOn)ev).Any();
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsMelodic.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsMelodic {
            get {
                if (this.Events.Count == 0) {
                    return false;
                }

                return (from ev in this.Events
                        where ev != null
                        let eventType = ev.EventType
                        where eventType == "VoiceNoteOn"
                        select (VoiceNoteOn)ev).Any(eventOn => eventOn.Channel != MidiChannel.DrumChannel);
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsRhythmical.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsRhythmical {
            get {
                //// Contract.Requires(this.Events != null);

                if (this.Channel == MidiChannel.DrumChannel) {
                    return true;
                }

                if (this.Events.Count == 0) {
                    return false;
                }

                return (from ev in this.Events
                        where ev != null
                        let eventType = ev.EventType
                        where eventType == "VoiceNoteOn"
                        select (VoiceNoteOn)ev).Select(eventOn => eventOn.Channel == MidiChannel.DrumChannel).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets MusicalTempo.
        /// </summary>
        /// <value> Property description. </value>
        public int Tempo {
            get {
                if (this.events == null || this.events.Count == 0) {
                    return 0;
                }

                foreach (var tempo in from ev in this.events
                                      where ev != null
                                      let eventType = ev.EventType
                                      where eventType == "MetaTempo"
                                      select ((MetaTempo)ev).Tempo
                                          into tempo
                                          where tempo > 0
                                          select tempo) {
                    return tempo;
                }

                return (int)MusicalTempo.Tempo120;
            }
        }
        #endregion

        #region Event properties
        /// <summary> Gets properties and their values.</summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public MidiEventCollection Events {
            get {
                Contract.Ensures(Contract.Result<MidiEventCollection>() != null);
                if (this.events == null) {
                    throw new InvalidOperationException("Collection of events is null.");
                }

                return this.events;
            }
        }

        /// <summary>
        /// Gets the melodic instrumentation events.
        /// </summary>
        /// <value> Property description. </value>
        public IList<VoiceProgramChange> MelodicInstrumentationEvents {
            get {
                if (this.Events.Count == 0) {
                    return null;
                }

                var list = new List<VoiceProgramChange>();
                foreach (var ev in this.Events) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "VoiceProgramChange":
                            list.Add(ev as VoiceProgramChange);
                            break;
                        //// resharper default: break;
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets MelodicInstrumentNumber.
        /// </summary>
        /// <value> Property description. </value>
        public MidiMelodicInstrument FirstMelodicInstrumentInEvents {
            get {
                if (this.Events.Count == 0) {
                    return 0;
                }

                byte melInstrNum = 0;
                //// long deltaTime = 0L; int channel = 0;
                foreach (var ev in this.Events) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "VoiceProgramChange":
                            melInstrNum = ((VoiceProgramChange)ev).Number;
                            //// // deltaTime = ev.StartTime;
                            //// // channel = ((ProgramChange)ev).Channel;
                            break;
                        //// resharper default: break;
                    }

                    if (melInstrNum > 0) {
                        break;
                    }
                    //// sometimes Program Changes more time before beginning, according to channels
                    //// midi format 0 not supported
                    //// if (melInstrumentNum > 0 && (deltaTime > 0 || channel == this.Number)) { return melInstrumentNum; }
                }

                return (melInstrNum > 0) ? (MidiMelodicInstrument)melInstrNum : (MidiMelodicInstrument)1;
            }
        }

        /// <summary>
        /// Gets MelodicInstrumentNumber.
        /// </summary>
        /// <value> Property description. </value>
        public MidiRhythmicInstrument FirstRhythmicInstrumentInEvents {
            get {
                if (this.Events.Count == 0) {
                    return 0;
                }

                byte instrNum = 0;
                //// long deltaTime = 0L; int channel = 0;
                foreach (var ev in this.Events) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "VoiceNoteOn":
                            if (ev is VoiceNoteOn eventOn && eventOn.Channel == MidiChannel.DrumChannel) {
                                instrNum = eventOn.Note;
                            }

                            break;
                        //// resharper default: break;
                    }

                    if (instrNum > 0) {
                        break;
                    }
                }

                return (instrNum > 0) ? (MidiRhythmicInstrument)instrNum : (MidiRhythmicInstrument)1;
            }
        }

        /// <summary>
        /// Gets MelodicInstrumentNumber.
        /// </summary>
        /// <value> Property description. </value>
        public MidiChannel FirstChannelInEvents {
            get {
                if (this.Events.Count == 0) {
                    return 0;
                }

                var melChannel = (MidiChannel)16;
                //// long deltaTime = 0L; int channel = 0;
                foreach (var ev in this.Events) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "VoiceProgramChange":
                            melChannel = ((VoiceProgramChange)ev).Channel;
                            break;
                        case "VoiceNoteOn":
                            melChannel = ((VoiceNoteOn)ev).Channel;
                            break;
                        //// resharper default: break;
                    }

                    if ((byte)melChannel < 16) {
                        break;
                    }

                    //// sometimes Program Changes more time before beginning, according to channels (midi format 0 not supported)
                    //// if (melInstrumentNum > 0 && (deltaTime > 0 || channel == this.Number)) {  return melInstrumentNum;  } 
                }

                if ((byte)melChannel == 16) {
                    melChannel = MidiChannel.C15;
                }

                return melChannel;
            }
        }

        #endregion

        #region Private properties
        /// <summary>Gets a value indicating whether end of track marker is required for writing out the entire track.</summary>
        /// <remarks>
        /// Note that MIDI files require an end of track marker at the end of every track.
        /// Setting this to false could have negative consequences.
        /// </remarks>
        /// <value> Property description. </value>
        private bool RequireEndOfTrack { get; }
        #endregion

        /// <summary>
        /// Assign To Sequence.
        /// </summary>
        /// <param name="sequence">Midi sequence.</param>
        public void AssignToSequence(CompactMidiStrip sequence) {
            this.Sequence = sequence;
        }

        #region Events

        /// <summary>
        /// Exists any event voice.
        /// </summary>
        /// <param name="givenChannel">The given channel.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool ExistsAnyEventVoice(MidiChannel givenChannel) {
            return (from ev in this.Events
                    let vev = ev as VoiceEvent
                    where vev != null && vev.Channel == givenChannel
                    select 1).Any();
        }

        /// <summary>
        /// Adds the event voice clones.
        /// </summary>
        /// <param name="givenEvents">The given events.</param>
        /// <param name="givenChannel">The given channel.</param>
        public void AddEventVoiceClones(IEnumerable<IMidiEvent> givenEvents, MidiChannel givenChannel) {
            Contract.Requires(givenEvents != null);
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ev in from ev in givenEvents
                               let vev = ev as VoiceEvent
                               where (vev == null) || vev.Channel == givenChannel
                               orderby ev.StartTime //// DeltaTime
                               select ev) {
                if (ev == null) {
                    continue;
                }

                this.Events.Add(ev.Clone());
            }
        }

        /// <summary> Shift Events To Start. </summary>
        [UsedImplicitly]
        public void ShiftEventsToStart() {
            Contract.Requires(this.Events != null && this.Events.Count > 0);
            if (this.Events.Count == 0) {
                return;
            }

            var firstToneEvent = (from ev in this.Events
                                  where ev != null
                                  let eventType = ev.EventType
                                  where eventType == "VoiceNoteOn"
                                  select ev).FirstOrDefault();

            if (firstToneEvent == null) {
                return;
            }

            if (this.Events.Count > 0) {
                this.Events.RecomputeAbsoluteTimes();
            }

            if (firstToneEvent.StartTime <= 0) {
                return;
            }

            if (this.Events.Count > 0) {
                this.Events.AddTimeToTotals(-firstToneEvent.StartTime);
            }

            //// 2013/10
            //// if (this.Events.Count > 0) { this.Events.RecomputeDeltaTimes(); } 
        }

        /// <summary>
        /// Trim given sequence to given total time.
        /// </summary>
        /// <param name="newSequence">Midi sequence.</param>
        /// <param name="totalTime">Total time.</param>
        public void TrimTo(CompactMidiStrip newSequence, long totalTime) {
            Contract.Requires(newSequence != null);
            Contract.Requires(this.Events.Count > 0);

            if (newSequence == null || this.Events == null || this.Events.Count == 0) {
                return;
            }
            //// Create a new track in the new sequence to match the old track in the old sequence
            var newTrack = new MidiTrack();
            newSequence.AddTrack(newTrack);
            //// Convert all times in the old track to deltas
            if (this.Events.Count > 0) {
                this.Events.RecomputeAbsoluteTimes();
            }

            //// Copy over all events that fell before the specified time
            for (var i = 0; i < this.Events.Count; i++) {
                var evi = this.Events.ElementAt(i);
                if (evi == null) {
                    continue;
                }

                if (evi.StartTime > totalTime) {
                    break;
                }

                newTrack.Events.Add(evi.Clone());
            }

            //// 2013/10
            ///// Convert all times back (on both new and old sequence; 
            //// the new one inherited the totals)
            //// this.Events.RecomputeDeltaTimes();
            //// newTrack.Events.RecomputeDeltaTimes();

            // If the new track lacks an end of track, add one
            if (!newTrack.HasEndOfTrack) {
                newTrack.Events.Add(new MetaEndOfTrack(0));
            }
        }

        #endregion

        #region Saving the Track
        /// <summary>Write the track to the output stream.</summary>
        /// <param name="outputStream">The output stream to which the track should be written.</param>
        public void Write(Stream outputStream) {
            // Validate the stream
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new MidiParserException("Cannot write to stream.", 0);
            }

            //// Make sure we have an end of track marker if we need one
            if (!this.HasEndOfTrack && this.RequireEndOfTrack) {
                this.events.Add(new MetaEndOfTrack(0));
                //// throw new MidiParserException("The track cannot be written until it has an end of track marker.",0);
            }

            //// Get the event data and write it out
            using (var memStream = new MemoryStream()) {
                for (var i = 0; i < this.events.Count; i++) {
                    var me = this.events.ElementAt(i);
                    me?.Write(memStream);
                }
                //// Tack on the header and write the whole thing out to the main stream
                var header = new MidiTrackChunkHeader(memStream.ToArray());
                header.Write(outputStream);
            }
            //// memStream.Dispose();
        }
        #endregion

        #region To String
        /// <summary>Writes the track to a string in human-readable form.</summary>
        /// <returns>A human-readable representation of the events in the track.</returns>
        public override string ToString() {
            string s;
            //// Create a writer, dump to it, return the string
            using (var writer = new StringWriter(CultureInfo.InvariantCulture)) {
                this.ToString(writer);
                s = writer.ToString();
            }
            //// writer.Dispose();
            return s;
        }

        /// <summary>Dumps the MIDI track to the writer in human-readable form.</summary>
        /// <param name="writer">The writer to which the track should be written.</param>
        private void ToString(TextWriter writer) {
            if (this.Events == null || this.Events.Count == 0) {
                return;
            }

            //// Validate the writer
            if (writer == null) {
                return;
            }

            //// Print out each event
            this.Events.ForAll(midiEvent => writer.WriteLine(midiEvent.ToString()));
        }
        #endregion
    }
}