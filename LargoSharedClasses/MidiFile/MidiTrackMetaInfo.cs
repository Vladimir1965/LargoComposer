// <copyright file="MidiTrackMetaInfo.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to represent an entire track in a MIDI file.

using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Midi;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>Represents a single MIDI track in a MIDI file.</summary>
    public sealed class MidiTrackMetaInfo {
        /// <summary> Collection of MIDI event added to this track.</summary>
        private readonly MidiEventCollection events;

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTrackMetaInfo"/> class.
        /// </summary>
        /// <param name="givenMidiTrack">The given midi track.</param>
        public MidiTrackMetaInfo(IMidiTrack givenMidiTrack) {
            Contract.Requires(givenMidiTrack != null);
            this.events = givenMidiTrack.Events;
            this.ReadMetaProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTrackMetaInfo"/> class.
        /// </summary>
        [UsedImplicitly]
        public MidiTrackMetaInfo() {
        }

        #region Public Meta Properties
        /// <summary>
        /// Gets MetaText.
        /// </summary>
        /// <value> Property description. </value>
        public string MetaText { get; private set; }

        /// <summary>
        /// Gets MetaSequenceTrackName.
        /// </summary>
        /// <value> Property description. </value>
        public string MetaSequenceTrackName { get; private set; }

        /// <summary>
        /// Gets MetaInstrument.
        /// <value> Property description. </value>
        /// </summary>
        /// <value> Property description. </value>
        public string MetaInstrument { get; private set; }

        /// <summary>
        /// Gets the name of the guess.
        /// </summary>
        /// <value>
        /// The name of the guess.
        /// </value>
        public string GuessName => ((this.MetaSequenceTrackName ?? this.MetaText) ?? this.MetaInstrument) ?? string.Empty;

        #endregion

        #region Support
        /// <summary> Read MetaProperties. </summary>
        [ContractVerification(false)]
        private void ReadMetaProperties()
        {
            foreach (var ev in this.events.Where(ev => ev != null))
            {
                this.AssignMetaProperty(ev);
            }
        }

        /// <summary>
        /// Assigns the meta property.
        /// </summary>
        /// <param name="midiEvent">The midi event.</param>
        private void AssignMetaProperty(IMidiEvent midiEvent) { //// cyclomatic complexity 10:13
            Contract.Requires(midiEvent != null);
            var eventType = midiEvent.EventType;
            var metaTextEvent = midiEvent as MetaAbstractText;
            var s = metaTextEvent?.Text.Trim();

            if (s == null) {
                return;
            }

            switch (eventType) {
                case "MetaText":
                    this.MetaText = s;
                    break;

                case "MetaSequenceTrackName":
                    this.MetaSequenceTrackName = s;
                    break;

                case "MetaInstrument":
                    this.MetaInstrument = s;
                    break;
                
                //// case "LargoBaseMusic.Midi.ProgramChange": this.MelodicInstrumentNumber = ((ProgramChange)ev).Number;
                ////    break;
                //// resharper default: break;
            }
        }

        #endregion
    }
}
