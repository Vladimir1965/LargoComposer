// <copyright file="MidiEventCollection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>A collection of MIDI events.</summary>
    [Serializable]
    public sealed class MidiEventCollection : ICollection<IMidiEvent> {   
        #region Fields
        /// <summary>
        /// Event List.
        /// </summary>
        private readonly List<IMidiEvent> eventList;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiEventCollection class.</summary>
        public MidiEventCollection() {
            this.eventList = new List<IMidiEvent>();
        }

        /// <summary>
        /// Initializes a new instance of the MidiEventCollection class.
        /// </summary>
        /// <param name="givenChannel">Midi channel.</param>
        public MidiEventCollection(MidiChannel givenChannel) {
            this.eventList = new List<IMidiEvent>();
            this.Channel = givenChannel;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets name of the collection.
        /// </summary>
        /// <value> Property description. </value>
        public string Name { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets a value indicating whether IsReadOnly.
        /// </summary>
        /// <value> General musical property.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets Count of events.
        /// </summary>
        /// <value> General musical property.</value>
        public int Count => this.EventList.Count;

        /// <summary>
        /// Gets or sets Midi Channel.
        /// </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; set; }

        /// <summary>
        /// Gets the break events.
        /// </summary>
        /// <returns> Returns value. </returns>
        public MidiEventCollection GetBreakEvents {
            get {
                var c = new MidiEventCollection();
                foreach (var ev in this.EventList) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "MetaTempo": {
                                c.Add(ev);
                                break;
                            }

                        case "MetaKeySignature": {
                                c.Add(ev);
                                break;
                            }

                        case "MetaTimeSignature": {
                                c.Add(ev);
                                break;
                            }
                    }
                }

                return c;
            }
        }

        /// <summary>
        /// Gets the tempo events.
        /// </summary>
        /// <returns> Returns value. </returns>
        public MidiEventCollection GetTempoEvents {
            get {
                var c = new MidiEventCollection();
                foreach (var ev in this.EventList) {
                    if (ev == null) {
                        continue;
                    }

                    var eventType = ev.EventType;
                    switch (eventType) {
                        case "MetaTempo": {
                                c.Add(ev);
                                break;
                            }
                    }
                }

                return c;
            }
        }
        #endregion

        #region Private Properties

        /// <summary> Gets properties and their values.</summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private List<IMidiEvent> EventList {
            get {
                Contract.Ensures(Contract.Result<List<MidiEvent>>() != null);
                if (this.eventList == null) {
                    throw new InvalidOperationException("Null event list.");
                }

                return this.eventList;
            }
        }
        #endregion

        /// <summary>
        /// Sets the events.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void SetEventClones(IEnumerable<IMidiEvent> collection) {
            if (collection != null) {
                foreach (var ev1 in collection) {
                    var ev2 = ev1.Clone();
                    this.EventList.Add(ev2);
                }
            }
        }
        #region ICollection
        /// <summary>
        /// Add Midi Event.
        /// </summary>
        /// <param name="item">Midi Event.</param>
        public void Add(IMidiEvent item) {
            this.EventList.Add(item);
        }

        /// <summary>
        /// Add Range of Midi Events.
        /// </summary>
        /// <param name="collection">Collection of Midi events.</param>
        public void AddRange(IEnumerable<IMidiEvent> collection) {
            if (collection != null) {
                this.EventList.AddRange(collection);
            }
        }

        /// <summary>
        /// Add Range.
        /// </summary>
        /// <param name="collection">Collection of Midi Events.</param>
        public void AddRange(MidiEventCollection collection) {
            if (collection != null) {
                this.EventList.AddRange(collection);
            }
        }

        /// <summary>
        /// Clear collection.
        /// </summary>
        public void Clear() {
            if (this.Count == 0 || this.EventList.Count == 0) {
                return;
            }

            this.EventList.Clear();
        }

        /// <summary>
        /// Contains Midi Event.
        /// </summary>
        /// <param name="item">Midi Event.</param>
        /// <returns> Returns value. </returns>
        public bool Contains(IMidiEvent item) {
            return this.EventList.Contains(item);
        }

        /// <summary>
        /// Copy To array.
        /// </summary>
        /// <param name="array">Array of Midi events.</param>
        /// <param name="arrayIndex">Array Index.</param>
        public void CopyTo(IMidiEvent[] array, int arrayIndex) {
            if (arrayIndex >= array.Length || arrayIndex + this.Count > array.Length) {
                return;
            }

            this.EventList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copy To array.
        /// </summary>
        /// <param name="array">Array of Midi events.</param>
        /// <param name="arrayIndex">Array Index.</param>
        [UsedImplicitly]
        public void CopyTo(Array array, int arrayIndex) {
            if (array == null) {
                return;
            }

            if (arrayIndex < 0) {
                return;
            }

            for (var i = arrayIndex; i < this.EventList.Count; i++) {
                if (i <= array.GetUpperBound(0)) {
                    array.SetValue(this.EventList[i], i);
                }
            }
        }

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns> Returns value. </returns>
        IEnumerator<IMidiEvent> IEnumerable<IMidiEvent>.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns> Returns value. </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Remove item.
        /// </summary>
        /// <param name="item">Midi Event.</param>
        /// <returns> Returns value. </returns>
        public bool Remove(IMidiEvent item) {
            return this.EventList.Remove(item);
        }

        /// <summary>
        /// Event At index.
        /// </summary>
        /// <param name="index">Index of event.</param>
        /// <returns> Returns value. </returns>
        public IMidiEvent ElementAt(int index) {
            Contract.Requires(index >= 0);
            return this.EventList.ElementAt(index);
        }
        #endregion

        /// <summary>
        /// Get Note On.
        /// </summary>
        /// <param name="note">MIDI instrument.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="loudness">Note loudness.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public VoiceNoteOn GetNoteOn(byte note, int startTime, byte loudness) {
            VoiceNoteOn eventOn;
            if (note > 0) {
                var velocity = (byte)(note > 0 ? MusicalProperties.VelocityOfLoudness(loudness) : 0);
                eventOn = new VoiceNoteOn(startTime, this.Channel, note, velocity);
            }
            else {
                const byte velocity = 0;
                eventOn = new VoiceNoteOn(startTime, this.Channel, note, velocity);
            }

            return eventOn;
        }

        /// <summary>
        /// Get Note Off.
        /// </summary>
        /// <param name="note">MIDI instrument.</param>
        /// <param name="midiTime">The midi time.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public VoiceNoteOff GetNoteOff(byte note, int midiTime) {
            //// byte pitchBend, <param name="pitchBend">MIDI pitch bend.</param>
            //// ...Pitch bend value, 0..16383, 8192 is centered.
            const byte velocity = 0;
            var eventOff = new VoiceNoteOff(midiTime, this.Channel, note, velocity);

            return eventOff;
        }

        #region Older Interface
        /// <summary> Inserts value of metre. </summary>
        /// <param name="rhythmicBeat">Rhythmical beat. Numerator of the time signature.</param>
        /// <param name="rhythmicBase">Rhythmical base. Negative power of two, denominator of time signature.</param>
        public void PutMetre(byte rhythmicBeat, byte rhythmicBase) { //// byte order
            //// number of MIDI clock ticks per metronome click (0x18 = 24 ticks/click)
            const byte ticks = 48;
            //// number of metronome clicks per 1/4 note (The number of notated 32nd notes per MIDI quarter note?)
            const byte clicks = 1;
            var ts = new MetaTimeSignature(0, rhythmicBeat, rhythmicBase, ticks, clicks);
            this.Add(ts);
        }

        /// <summary>
        /// Inserts tempo.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="tempo">MIDI tempo = metronome clicks per minute.</param>
        public void PutTempo(long deltaTime, int tempo) { 
            Contract.Requires(tempo != 0);
            var tv = MetaTempo.MidiTempoBaseNumber / tempo;    //// microseconds per metronome clicks 0
            var evt = new MetaTempo(deltaTime, tv);
            this.Add(evt);
        }

        /*
        /// <summary> Inserts tempo. </summary>
        /// <param name="rhythmicBeat">Metre numerator.</param>
        /// <param name="rhythmicBase">Metre base.</param>
        /// <param name="tempo">MIDI tempo.</param>
        [UsedImplicitly]
        public void PutMetreAndTempo(byte rhythmicBeat, byte rhythmicBase, int tempo) { 
            Contract.Requires(tempo != 0);
            this.PutMetre(rhythmicBeat, rhythmicBase); 
            this.PutTempo(tempo);
        }*/

        /// <summary> Selects instrument and channel of the track. </summary>
        /// <param name="deltaTime">Delta Time.</param>/// 
        /// <param name="givenInstrument">MIDI instrument.</param>
        public void PutInstrument(long deltaTime, byte givenInstrument) {
            var pch = new VoiceProgramChange(deltaTime, this.Channel, (MidiMelodicInstrument)givenInstrument);
            this.Add(pch);
        }

        /// <summary>
        /// Puts the key signature.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="givenKey">The given key.</param>
        /// <param name="givenTonalGenus">The given tonal genus.</param>
        public void PutKeySignature(long deltaTime, TonalityKey givenKey, TonalityGenus givenTonalGenus) {
            var signature = new MetaKeySignature(deltaTime, givenKey, givenTonalGenus);
            this.Add(signature);
        }

        #region Meta Messages
        /// <summary>
        /// Write meta message.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="text">Meta message text.</param>
        public void PutMetaText(long deltaTime, string text) {
            var ev = new MetaText(deltaTime, text);
            this.Add(ev);
        }

        /// <summary> Write meta message. </summary>
        /// <param name="text">Meta message text.</param>
        public void PutMetaCopyright(string text) {
            var ev = new MetaCopyright(0, text);
            this.Add(ev);
        }

        #endregion

        /// <summary>
        /// Inserts one note into data.
        /// </summary>
        /// <param name="startTime">The start delta time.</param>
        /// <param name="note">MIDI note.</param>
        /// <param name="stopTime">The stop delta time.</param>
        /// <param name="loudness">Note loudness.</param>
        /// <param name="isFromPreviousBar">If set to <c>true</c> [is from previous bar].</param>
        /// <param name="isGoingToNextBar">If set to <c>true</c> [is going to next bar].</param>
        public void PutNote(
                        long startTime,
                        byte note,
                        long stopTime,
                        byte loudness,
                        bool isFromPreviousBar,
                        bool isGoingToNextBar) { //// byte pitchBend, <param name="pitchBend">MIDI pitch bend.</param>
            byte velocity = 0;
            if (note > 0) {
                velocity = MusicalProperties.VelocityOfLoudness(loudness);
            }

            if (!isFromPreviousBar) {
                var eventOn = new VoiceNoteOn(startTime, this.Channel, note, velocity);
                this.Add(eventOn);
            }

            //// Staccato, Legato, ... !?!
            if (isGoingToNextBar) {
                return;
            }

            //// 2013/11 test, was stopTime only (stopTime - startTime + 1)
            var eventOff = new VoiceNoteOff(stopTime, this.Channel, note, velocity); //// velocity 0?
            this.Add(eventOff);
        }

        #endregion

        #region Adding and Removing Events

        /// <summary>Adds a collection of MIDI event messages to the collection.</summary>
        /// <param name="messages">The events to be added.</param>
        /// <returns>The position at which the first event was added.</returns>
        [UsedImplicitly]
        public int AddRange(Collection<MidiEvent> messages) { //// virtual
            // Validate the input
            if (messages == null) {
                throw new ArgumentNullException(nameof(messages));
            }

            if (messages.Count == 0) {
                return -1;
            }

            // Store the count of the list (the inserted position of the first new element).
            var insertionPos = this.Count;

            // Add the events
            this.EventList.AddRange(messages);

            // Return the position of the first
            return insertionPos;
        }
        #endregion

        #region Global operations

        /// <summary>
        /// Increase all the delta times with given value.
        /// </summary>
        /// <param name="givenTime">Given Time.</param>
        public void AddTimeToTotals(long givenTime) {
            for (var i = 0; i < this.Count; i++) {
                if (i >= this.EventList.Count) {
                    continue;
                }

                if (this.EventList[i] == null) {
                    continue;
                }

                if (this.EventList[i].StartTime + givenTime >= 0) {
                    this.EventList[i].StartTime += givenTime;
                }
            }
        }

        /// <summary>Converts the delta times on all events to from delta times to total times.</summary>
        public void RecomputeAbsoluteTimes() { ////  ConvertDeltasToTotals //// [VL], was internal
            Contract.Requires(this.Count > 0);
            if (this.EventList == null || this.EventList.Count == 0) {
                return;
            }
            //// Update all delta times to be total times
            var ev0 = this.EventList[0];
            if (this.Count <= 0 || ev0 == null) {
                return;
            }

            var total = ev0.DeltaTime;
            ev0.StartTime = total;
            for (var i = 1; i < this.Count; i++) {
                if (i >= this.EventList.Count) {
                    continue;
                }

                var ev = this.EventList[i];
                if (ev == null) {
                    continue;
                }

                total += ev.DeltaTime;
                //// ev.DeltaTime = total;
                ev.StartTime = total;
            }
        }

        /// <summary>Converts the delta times on all events from total times back to delta times.</summary>
        public void RecomputeDeltaTimes() { //// ConvertTotalsToDeltas // [VL], was internal
            if (this.Count == 0) {
                return;
            }
            //// Update all total times to be deltas
            long lastStartTime = 0;
            for (var i = 0; i < this.Count; i++) {
                if (i >= this.EventList.Count) {
                    continue;
                }

                var evi = this.EventList[i];
                if (evi == null) {
                    continue;
                }

                var time = evi.StartTime - lastStartTime;
                evi.DeltaTime = time >= 0 ? time : 0;

                lastStartTime = evi.StartTime;
            }
        }
        #endregion

        #region Sorting
        /// <summary>Sorts the events based on deltaTime.</summary>
        public void SortByStartTime() { // [VL], was internal
            // Sort by delta time
            this.EventList.Sort(new EventComparer());
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns> Returns value. </returns>
        private IEnumerator<IMidiEvent> GetEnumerator() {
            return this.EventList.GetEnumerator();
        }
        #endregion

        #region Sorting (internal only)
        /// <summary>Enables comparison of two events based on delta times.</summary>
        private sealed class EventComparer : IComparer<IMidiEvent> {
            #region Implementation of IComparer
            /// <summary>Compares two MidiEvents based on delta times.</summary>
            /// <param name="x">The first MidiEvent to compare.</param>
            /// <param name="y">The second MidiEvent to compare.</param>
            /// <returns> Returns -1 if x.StartTime is larger, 0 if they're the same, 1 otherwise.</returns>
            public int Compare(IMidiEvent x, IMidiEvent y) {
                // Get the MidiEvents
                var eventX = x;
                var eventY = y;

                // Make sure they're valid
                if (eventX == null) {
                    throw new ArgumentNullException(nameof(x));
                }

                if (eventY == null) {
                    throw new ArgumentNullException(nameof(y));
                }

                // Compare the delta times
                return eventX.StartTime.CompareTo(eventY.StartTime);
            }
            #endregion
        }
        #endregion
    }
}
