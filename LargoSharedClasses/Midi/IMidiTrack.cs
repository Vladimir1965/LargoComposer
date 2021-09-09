// <copyright file="IMidiTrack.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// I Midi Track.
    /// </summary>
    public interface IMidiTrack
    {
        /// <summary>
        /// Gets or sets the type of the band.
        /// </summary>
        /// <value>
        /// The type of the band.
        /// </value>
        MusicalBand BandType { get; set; }

        /// <summary>
        /// Gets or sets the bar division.
        /// </summary>
        /// <value>
        /// The bar division.
        /// </value>
        int BarDivision { get; set; }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        MidiEventCollection Events { get; }

        /// <summary>
        /// Gets the first channel in events.
        /// </summary>
        /// <value>
        /// The first channel in events.
        /// </value>
        MidiChannel FirstChannelInEvents { get; }

        /// <summary>
        /// Gets the first melodic instrument in events.
        /// </summary>
        /// <value>
        /// The first melodic instrument in events.
        /// </value>
        MidiMelodicInstrument FirstMelodicInstrumentInEvents { get; }

        /// <summary>
        /// Gets the first rhythmic instrument in events.
        /// </summary>
        /// <value>
        /// The first rhythmic instrument in events.
        /// </value>
        MidiRhythmicInstrument FirstRhythmicInstrumentInEvents { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has end of track.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has end of track; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        bool HasEndOfTrack { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has tones.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has tones; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        bool HasTones { get; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        MidiChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        byte InstrumentNumber { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is melodic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is melodic; otherwise, <c>false</c>.
        /// </value>
        bool IsMelodic { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is rhythmical.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rhythmical; otherwise, <c>false</c>.
        /// </value>
        bool IsRhythmical { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the melodic instrumentation events.
        /// </summary>
        /// <value>
        /// The melodic instrumentation events.
        /// </value>
        IList<VoiceProgramChange> MelodicInstrumentationEvents { get; }

        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>
        /// The metric.
        /// </value>
        MusicalMetric Metric { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the octave.
        /// </summary>
        /// <value>
        /// The octave.
        /// </value>
        MusicalOctave Octave { get; set; }

        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        CompactMidiStrip Sequence { get; }

        /// <summary>
        /// Gets or sets the staff.
        /// </summary>
        /// <value>
        /// The staff.
        /// </value>
        byte Staff { get; set; }

        /// <summary>
        /// Gets the tempo.
        /// </summary>
        /// <value>
        /// The tempo.
        /// </value>
        int Tempo { get; }

        /// <summary>
        /// Gets or sets the track number.
        /// </summary>
        /// <value>
        /// The track number.
        /// </value>
        int TrackNumber { get; set; }

        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        /// <value>
        /// The voice.
        /// </value>
        byte Voice { get; set; }

        /// <summary>
        /// Adds the event voice clones.
        /// </summary>
        /// <param name="givenEvents">The given events.</param>
        /// <param name="givenChannel">The given channel.</param>
        [UsedImplicitly]
        void AddEventVoiceClones(IEnumerable<IMidiEvent> givenEvents, MidiChannel givenChannel);

        /// <summary>
        /// Assigns to sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        void AssignToSequence(CompactMidiStrip sequence);

        /// <summary>
        /// Exists any event voice.
        /// </summary>
        /// <param name="givenChannel">The given channel.</param>
        /// <returns>Returns value.</returns>
        bool ExistsAnyEventVoice(MidiChannel givenChannel);

        //// void PlayTrack(int givenDivision);

        /// <summary>
        /// Shifts the events to start.
        /// </summary>
        [UsedImplicitly]
        void ShiftEventsToStart();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();

        /// <summary>
        /// Trims to.
        /// </summary>
        /// <param name="newSequence">The new sequence.</param>
        /// <param name="totalTime">The total time.</param>
        void TrimTo(CompactMidiStrip newSequence, long totalTime);

        /// <summary>
        /// Writes the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        void Write(Stream outputStream);
    }
}