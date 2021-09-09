// <copyright file="MidiTones.cs" company="Traced-Ideas, Czech republic">
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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Midi Tone Collection. 
    /// </summary>
    [Serializable]
    public sealed class MidiTones : IMidiTones
    {
        #region Fields
        /// <summary>
        /// Musical metric.
        /// </summary>
        [NonSerialized]
        private MusicalMetric metric;

        /// <summary>
        /// Last Bar Number.
        /// </summary>
        private int lastBarNumber;

        /// <summary>
        /// Current Delta Time.
        /// </summary>
        private long currentStartTime;

        /// <summary>
        /// Current Note.
        /// </summary>
        private byte currentNoteNumber;

        /// <summary>
        /// The list.
        /// </summary>
        private List<IMidiTone> list;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTones"/> class.
        /// </summary>
        /// <param name="givenMidiTrack">The given midi track.</param>
        public MidiTones(IMidiTrack givenMidiTrack) {
            Contract.Requires(givenMidiTrack != null);
            this.Metric = givenMidiTrack.Metric;
            this.List = new List<IMidiTone>();
            this.MidiTrack = givenMidiTrack;
            this.Name = givenMidiTrack.Name;
            this.BarDivision = givenMidiTrack.BarDivision;
            this.InstrumentNumber = givenMidiTrack.InstrumentNumber;
            this.Channel = (byte)givenMidiTrack.Channel;
            this.Octave = givenMidiTrack.Octave;
            this.BandType = givenMidiTrack.BandType;
            this.Tempo = givenMidiTrack.Tempo;
            this.IsRhythmical = givenMidiTrack.IsRhythmical;
            if (this.BarDivision != 0) {
                this.ReadMidiTones(givenMidiTrack);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTones"/> class.
        /// </summary>
        /// <param name="givenMidiTrack">The given midi track.</param>
        /// <param name="givenArea">The given area.</param>
        public MidiTones(IMidiTrack givenMidiTrack, MusicalSection givenArea)
            : this(givenMidiTrack) {
            var tones = (from tone in this.List
                         where tone.BarNumberFrom >= givenArea.BarFrom && tone.BarNumberFrom <= givenArea.BarTo //// && tone.StartTime > 18432 && tone.StartTime < 20000 
                         orderby tone.BarNumberFrom, tone.StartTime ////, tone.Duration  
                         select tone).ToList();
            this.SetTones(tones);
            this.SetFirstBarNumber(givenArea.BarFrom);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiTones"/> class.
        /// </summary>
        /// <param name="list">The list of tones.</param>
        [UsedImplicitly]
        public MidiTones(IList<IMidiTone> list) {
            Contract.Requires(list != null);
            this.Metric = new MusicalMetric(1, 0);
            this.List = list.ToList();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MidiTones"/> class from being created.
        /// </summary>
        private MidiTones() {
            this.Metric = new MusicalMetric(1, 0);
            this.List = new List<IMidiTone>();
        }

        /// <summary>
        /// Initializes a new instance of the MidiTones class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="givenList">Given list.</param>
        private MidiTones(MidiTones prototype, IEnumerable<IMidiTone> givenList) {
            Contract.Requires(prototype != null);
            this.MidiTrack = prototype.MidiTrack;
            this.Name = prototype.Name;
            this.Number = prototype.Number;
            this.BarDivision = prototype.BarDivision;
            this.InstrumentNumber = prototype.InstrumentNumber;
            this.Channel = prototype.Channel;
            this.Octave = prototype.Octave;
            this.BandType = prototype.BandType;
            this.IsRhythmical = prototype.IsRhythmical;
            this.Tempo = prototype.Tempo;
            this.Metric = prototype.Metric;
            this.List = givenList.ToList();
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>
        /// The metric.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Metric is null.</exception>
        /// <exception cref="System.ArgumentException">Metric cannot be set null.;value</exception>
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
        /// Gets the list.
        /// </summary>
        /// <value> Property description. </value>
        public IList<IMidiTone> List {
            get => this.list;

            private set => this.list = (List<IMidiTone>)value;
        }

        /// <summary>
        /// Gets name of the collection.
        /// </summary>
        /// <value> Property description. </value>
        public string Name { get; }

        /// <summary>
        ///  Gets the FirstBarNumber (because of division to blocks).
        /// </summary>
        /// <value> Property description. </value>
        public int FirstBarNumber { get; private set; }

        /// <summary>
        /// Gets BarDivision.
        /// </summary>
        /// <value> Property description. </value>
        public int BarDivision { get; }

        /// <summary>
        /// Gets instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalOctave Octave { get; }

        /// <summary>
        /// Gets instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalBand BandType { get; }

        /// <summary>
        /// Gets a value indicating whether Is Rhythmical.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public bool IsRhythmical { get; }

        /// <summary> Gets musical tempo. </summary>
        /// <value> Property description. </value>
        private int Tempo { get; }

        /// <summary>
        /// Gets BarDivision.
        /// </summary>
        /// <value> Property description. </value>
        private int Number { get; }

        /// <summary>
        /// Gets instrument.
        /// </summary>
        /// <value> Property description. </value>
        private byte InstrumentNumber { get; }

        /// <summary>
        /// Gets Channel.
        /// </summary>
        /// <value> Property description. </value>
        private byte Channel { get; }

        /// <summary>
        /// Gets Tones Having Duration.
        /// </summary>
        /// <value> Property description. </value>
        private MidiTones TonesHavingDuration {
            get {
                IList<IMidiTone> listTones = this.List.Where(t => (t.Duration > 0)).OrderBy(t => t.StartTime).ToList();
                var c = new MidiTones(this, listTones);
                return c;
            }
        }

        /// <summary>Gets or sets Collection of MIDI event added to this track.</summary>
        /// <value> Property description. </value>
        private Collection<IMidiEvent> OtherEventList { get; set; }

        /// <summary>
        /// Gets the MidiTrack.
        /// </summary>
        /// <value> Property description. </value>
        private IMidiTrack MidiTrack { get; }
        #endregion

        /// <summary>
        /// Checks the tones - because of problem with sounding over end of tone
        /// </summary>
        /// <returns> Returns value. </returns>
        public bool CheckTones() {
            //// var status = true;
            var tones = (from tone in this.List
                         where tone.Loudness > 0 && tone.BarNumberTo - tone.BarNumberFrom > 10
                         orderby tone.BarNumberFrom, tone.StartTime
                         select tone).ToList();
            if (tones.Count <= 0) {
                return true;
            }

            //// status = false;
            //// var tone1 = list.FirstOrDefault();
            ////201508 MessageBox.Show(string.Format("Tone too long! \n {0} ", tone1));
            ////  foreach (var tone in list) { MessageBox.Show(string.Format("Tone too long! \n {0} ", tone)); }

            return false;
        }

        /// <summary>
        /// Check Rests.
        /// </summary>
        public void CheckRests() {
            var tones = (from tone in this.List
                         where tone.Loudness == 0 && tone.BarNumberTo - tone.BarNumberFrom > 10
                         orderby tone.BarNumberFrom, tone.StartTime
                         select tone).ToList();
            if (tones.Count <= 0) {
            }

            ////201508  foreach (var tone in list) {   MessageBox.Show(string.Format("Rest too long! \n {0} ", tone)); }
        }

        /// <summary>
        /// Sets the tones.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        public void SetTones(IEnumerable<IMidiTone> givenList) {
            Contract.Requires(givenList != null);
            this.list.Clear();
            this.list.AddRange(givenList);
        }

        /// <summary>
        /// Sets the first bar number.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        public void SetFirstBarNumber(int barNumber) {
            this.FirstBarNumber = barNumber;
            this.list.ForEach(mtone => {
                checked {
                    mtone.BarNumberFrom -= barNumber - 1;
                    mtone.BarNumberTo -= barNumber - 1;
                }
            });
        }

        #region MidiTones
        /// <summary>
        /// Completes all tones.
        /// </summary>
        /// <param name="midiTrack">The midi track.</param>
        /// <param name="tones">The tones.</param>
        /// <param name="allTones">All tones.</param>
        private static void CompleteAllTones(IMidiTrack midiTrack, MidiTones tones, MidiTones allTones) {
            Contract.Requires(tones != null);
            Contract.Requires(midiTrack != null);
            Contract.Requires(allTones != null);

            long lastTime = 0;
            tones.list.ForEach(rmt => {
                if (rmt.StartTime > lastTime) {
                    var mt = new MidiTone(lastTime, 0, 0, rmt.InstrumentNumber, rmt.Channel) {
                        Duration = rmt.StartTime - lastTime,
                        BarNumberFrom = (int)Math.Floor((double)lastTime / midiTrack.BarDivision) + 1,
                        BarNumberTo = (int)Math.Floor((double)rmt.StartTime / midiTrack.BarDivision) + 1
                    };
                    allTones.List.Add(mt);
                }

                lastTime = rmt.StartTime + rmt.Duration;
            });
        }

        /// <summary>
        /// Read Midi Tones.
        /// </summary>
        /// <param name="midiTrack">Midi Track.</param>
        [ContractVerification(false)]
        private void ReadMidiTones(IMidiTrack midiTrack) { //// cyclomatic complexity 10:11
            Contract.Requires(midiTrack != null);

            if (midiTrack.BarDivision == 0) {
                throw new InvalidOperationException("Bar division is zero.");
            }

            var tones = new MidiTones();
            this.lastBarNumber = -1;
            //// !!!!!! 2013/11 midiTrack.Events.RecomputeAbsoluteTimes();
            //// this.eventList.SortByStartTime(); 
            var allTones = new MidiTones();
            this.OtherEventList = new Collection<IMidiEvent>();
            this.currentStartTime = 0;
            this.currentNoteNumber = 0;
            const bool fullLength = true;
            this.PrepareMidiTones(midiTrack, tones, allTones, fullLength);

            allTones.AddRange(tones);
            //// when note off events are missing 
            allTones.CompleteDurations(fullLength, midiTrack.BarDivision);
            midiTrack.Events.RecomputeDeltaTimes();
            //// Where(t => (t.Velocity>0 && t.Duration==0)
            tones = allTones.TonesHavingDuration;
            CompleteAllTones(midiTrack, tones, allTones);
            //// 2014/12 Time optimization
            allTones.list.RemoveAll(mtone => mtone.Duration <= 0);
            var orderedTones = allTones.List.OrderBy(t => t.StartTime);
            //// var midiTones = allTones.List.Where(t => (t.Duration > 0)).OrderBy(t => t.StartTime).ToList();
            this.list.AddRange(orderedTones);
        }

        /// <summary>
        /// Prepares the midi tones.
        /// </summary>
        /// <param name="midiTrack">The midi track.</param>
        /// <param name="tones">The tones.</param>
        /// <param name="allTones">All tones.</param>
        /// <param name="fullLength">If set to <c>true</c> [full length].</param>
        private void PrepareMidiTones(IMidiTrack midiTrack, MidiTones tones, MidiTones allTones, bool fullLength) {
            Contract.Requires(midiTrack != null);
            Contract.Requires(midiTrack.Events != null);
            Contract.Requires(tones != null);
            Contract.Requires(allTones != null);
            var instrument = midiTrack.InstrumentNumber;
            var channel = midiTrack.Channel;
            foreach (var ev in midiTrack.Events) {
                bool noteOn = false, noteOff = false;
                VoiceNoteOn eventOn = null;
                var eventClass = ev.GetType().ToString();
                var eventType = Path.GetExtension(eventClass);
                switch (eventType) {
                    case ".VoiceProgramChange": {
                            var programChange = (VoiceProgramChange)ev;
                            instrument = programChange.Number;
                            channel = programChange.Channel;
                            continue;
                        }

                    case ".VoiceNoteOn": {
                            eventOn = (VoiceNoteOn)ev;
                            noteOn = eventOn.Velocity > 0;
                            noteOff = eventOn.Velocity == 0;
                            this.currentStartTime = eventOn.StartTime;
                            this.currentNoteNumber = eventOn.Note;
                            channel = eventOn.Channel;
                            break;
                        }

                    case ".VoiceNoteOff": {
                            var levOff = (VoiceNoteOff)ev;
                            noteOff = true;
                            this.currentStartTime = levOff.StartTime;
                            this.currentNoteNumber = levOff.Note;
                            break;
                        }

                    default: {
                            if (!ev.IsMetaEvent) {
                                this.OtherEventList.Add(ev);
                            }

                            break;
                        }
                }

                if (noteOn) {
                    this.AddNoteOnToTones(midiTrack, tones, instrument, channel, eventOn);
                }

                // ReSharper disable once InvertIf
                if (noteOff) {
                    var mt = this.CompleteCurrentNote(midiTrack, tones, fullLength);
                    if (mt == null) {
                        continue;
                    }

                    allTones.List.Add(mt);
                    tones.List.Remove(mt);
                }
            }
        }

        /// <summary>
        /// Completes the current note.
        /// </summary>
        /// <param name="midiTrack">The midi track.</param>
        /// <param name="tones">The tones.</param>
        /// <param name="fullLength">If set to <c>true</c> [full length].</param>
        /// <returns> Returns value. </returns>
        private IMidiTone CompleteCurrentNote(IMidiTrack midiTrack, MidiTones tones, bool fullLength) {
            Contract.Requires(tones != null);
            Contract.Requires(midiTrack != null);

            var noteNumber = this.currentNoteNumber;
            var toff = (from t in tones.List where t.NoteNumber == noteNumber select t).ToList();
            if (!toff.Any()) {
                return null;
            }

            var mt = toff.First();
            if (mt == null) {
                return null;
            }

            mt.CompleteDuration(this.currentStartTime, fullLength, midiTrack.BarDivision);

            var lastReadTone = tones.List.LastOrDefault();
            //// MidiTone lastCompleteTone = allTones.LastOrDefault();
            if (lastReadTone != null) {
                mt.SoundThrough = mt.StartTime < lastReadTone.StartTime;
            }

            return mt;
        }

        /// <summary>
        /// Adds the note on to tones.
        /// </summary>
        /// <param name="midiTrack">The midi track.</param>
        /// <param name="tones">The tones.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="eventOn">The voice note-on.</param>
        private void AddNoteOnToTones(IMidiTrack midiTrack, MidiTones tones, byte instrument, MidiChannel channel, VoiceNoteOn eventOn) {
            Contract.Requires(midiTrack != null);
            Contract.Requires(eventOn != null);
            Contract.Requires(tones != null);
            var mt = new MidiTone(this.currentStartTime, this.currentNoteNumber, eventOn.Velocity, instrument, channel) {
                BarNumberFrom = (int)Math.Floor((double)this.currentStartTime / midiTrack.BarDivision) + 1
            };

            if (mt.BarNumberFrom != this.lastBarNumber) {
                this.lastBarNumber = mt.BarNumberFrom;
                mt.FirstInBar = true;
            }

            tones.List.Add(mt);
        }
        #endregion

        /// <summary>
        /// Complete Durations.
        /// </summary>
        /// <param name="fullLength">Full length.</param>
        /// <param name="givenDivision">Given division.</param>
        private void CompleteDurations(bool fullLength, int givenDivision) {
            //// when note off events are missing 
            for (var i = 0; i < this.List.Count - 1; i++) {
                var mt0 = this.List[i];
                if (mt0 == null || mt0.Duration != 0) {
                    continue;
                }

                var mt1 = this.List[i + 1];
                if (mt1 != null) {
                    mt0.CompleteDuration(mt1.StartTime, fullLength, givenDivision);
                }
            }
        }

        /// <summary>
        /// Add Collection.
        /// </summary>
        /// <param name="givenTones">Given tones.</param>
        private void AddRange(MidiTones givenTones) {
            Contract.Requires(givenTones != null);
            //// if (givenTone == null) { return false;  }

            this.list.AddRange(givenTones.List);
        }
    }
}
