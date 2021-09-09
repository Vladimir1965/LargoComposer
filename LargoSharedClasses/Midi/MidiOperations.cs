// <copyright file="MidiOperations.cs" company="Traced-Ideas, Czech republic">
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
using JetBrains.Annotations;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi {
    /// <summary>
    /// Raw Midi Operations.
    /// </summary>
    [UsedImplicitly]
    public static class MidiOperations {
        #region Format conversion
        /// <summary>Converts a MIDI sequence from its current format to the specified format.</summary>
        /// <param name="sequence">The sequence to be converted.</param>
        /// <param name="givenFormat">The format to which we want to convert the sequence.</param>
        /// <param name="options">Options used when doing the conversion.</param>
        /// <returns>The converted sequence.</returns>
        /// <remarks>
        /// This may or may not return the same sequence as passed in.
        /// Regardless, the reference passed in should not be used after this call as the old
        /// sequence could be unusable if a different reference was returned.
        /// </remarks>
        [UsedImplicitly]
        public static CompactMidiStrip Convert(CompactMidiStrip sequence, int givenFormat, FormatConversionOptions options = FormatConversionOptions.None) {
            // Validate the parameters
            if (sequence == null) {
                throw new ArgumentNullException(nameof(sequence));
            }

            if (givenFormat < 0 || givenFormat > 2) {
                throw new ArgumentOutOfRangeException(nameof(givenFormat), givenFormat, "The format must be 0, 1, or 2.");
            }

            // Handle the simple cases
            if (sequence.Format == givenFormat) {
                return sequence;
            } // already in requested format

            if (givenFormat != 0 || sequence.NumberOfLines == 1) { // only requires change in format #
                // Change the format and return the same sequence
                sequence.Format = givenFormat;
                return sequence;
            }

            // Now the hard one, converting to format 0.
            // We need to combine all tracks into 1.
            var newSequence = new CompactMidiStrip(givenFormat, sequence.Header.Division);

            // Iterate through all events in all tracks and change deltaTimes to actual times.
            // We'll then be able to sort based on time and change them back to deltas later
            foreach (var track in sequence.Where(track => track != null && track.Events.Count > 0)) {
                track.Events.RecomputeAbsoluteTimes(); //// ConvertDeltasToTotals();
            }

            var newTrack = NewTrackFromSequence(sequence, options);

            newSequence.AddTrack(newTrack);
            //// Return the new sequence
            return newSequence;
        }

        #endregion

        #region Transposition

        /// <summary>Transposes a MIDI sequence up/down the specified number of half-steps.</summary>
        /// <param name="sequence">The sequence to be transposed.</param>
        /// <param name="steps">The number of steps up(+) or down(-) to transpose the sequence.</param>
        /// <param name="includeDrums">Whether drum sequence should also be transposed.</param>
        /// <remarks>If the step value is too large or too small, notes may wrap.</remarks>
        [UsedImplicitly]
        public static void Transpose(IEnumerable<MidiTrack> sequence, int steps, bool includeDrums = false) {
            // If the sequence is null, throw an exception.
            if (sequence == null) {
                throw new ArgumentNullException(nameof(sequence));
            }

            // Modify each track  // Modify each event
            ////  If the event is not a voice MIDI event but the channel is the
            ////  drum channel and the user has chosen not to include drums in the
            ////  transposition (which makes sense), skip this event.
            ////  If the event is a VoiceNoteOn, VoiceNoteOff, or Aftertouch, shift the note
            ////  according to the supplied number of steps.
            foreach (var voiceEvent in from track in sequence
                                               where track != null
                                               from ev in track.Events
                                               select ev as VoiceAbstractNote
                                                   into voiceEvent
                                                   where voiceEvent != null && (includeDrums || voiceEvent.Channel != MidiChannel.DrumChannel)
                                                   select voiceEvent) {
                voiceEvent.Note = (byte)((voiceEvent.Note + steps) % 128);
            }
        }

        #endregion

        #region Trimming
        /// <summary>Trims a MIDI file to a specified length.</summary>
        /// <param name="sequence">The sequence to be copied and trimmed.</param>
        /// <param name="totalTime">The requested time length of the new MIDI sequence.</param>
        /// <returns>A MIDI sequence with only those events that fell before the requested time limit.</returns>
        [UsedImplicitly]
        public static CompactMidiStrip Trim(CompactMidiStrip sequence, long totalTime) {
            Contract.Requires(sequence != null);
            if (sequence == null) {
                return null;
            }
            //// Create a new sequence to mimic the old
            var newSequence = (CompactMidiStrip)sequence.Clone(); //// new CompactMidiStrip(sequence.Format, sequence.Header.Division);

            //// Copy each track up to the specified time limit
            foreach (var track in sequence.Where(track => track != null && track.Events.Count > 0)) {
                track.TrimTo(newSequence, totalTime);
            }

            //// Return the new sequence
            return newSequence;
        }
        #endregion

        /// <summary>
        /// New track from sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="options">The options.</param>
        /// <returns>New midi track.</returns>
        private static MidiTrack NewTrackFromSequence(CompactMidiStrip sequence, FormatConversionOptions options) {
            Contract.Requires(sequence != null);

            var newTrack = new MidiTrack();
            //// Add all events to new track (except for end of track markers!)
            ////  If this event has a channel, and if we're storing lines as channels, copy to it
            ////  Add all events, except for end of track markers (we'll add our own)
            var trackNumber = 0;
            foreach (var track in sequence.Where(track => track != null)) {
                foreach (var midiEvent in track.Events) {
                    if ((options & FormatConversionOptions.CopyTrackToChannel) > 0 
                                && (midiEvent is VoiceEvent vev)  && trackNumber <= 0xF) { //// && trackNumber >= 0
                        vev.Channel = (MidiChannel)(byte)trackNumber;
                    }

                    if (!(midiEvent is MetaEndOfTrack)) {
                        newTrack.Events.Add(midiEvent);
                    }
                }

                trackNumber++;
            }

            // Sort the events
            newTrack.Events.SortByStartTime();

            // Now go back through all of the events and update the times to be deltas
            newTrack.Events.RecomputeDeltaTimes();

            // Put an end of track on for good measure as we've already taken out
            // all of the ones that were in the original sequence.
            newTrack.Events.Add(new MetaEndOfTrack(0));
            return newTrack;
        }
    }
}
