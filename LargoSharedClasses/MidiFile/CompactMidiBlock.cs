// <copyright file="CompactMidiBlock.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Compact Midi Block.
    /// </summary>
    public class CompactMidiBlock
    {
        #region Fields
        /// <summary>
        /// Musical Block.
        /// </summary>
        private MusicalBlock musicalBlock;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiBlock" /> class.
        /// </summary>
        /// <param name="givenMusicalBlock">The given musical block.</param>
        /// <exception cref="InvalidOperationException">Tempo value is zero.</exception>
        public CompactMidiBlock(MusicalBlock givenMusicalBlock)
        {
            Contract.Requires(givenMusicalBlock != null);

            if (givenMusicalBlock == null) {
                return;
            }

            if (givenMusicalBlock.Header.Tempo == 0) {
                throw new InvalidOperationException("Tempo value is zero.");
            }

            this.MusicalBlock = givenMusicalBlock;
            var header = this.MusicalBlock.Header;
            this.RhythmicOrder = header.System.RhythmicOrder;
            this.BarDivision = MusicalProperties.BarDivision(header.Division, header.Metric.MetricBeat, header.Metric.MetricGround);
            this.BitDuration = MusicalProperties.MidiDuration(this.RhythmicOrder, 1, this.BarDivision);
            this.BarDuration = MusicalProperties.MidiDuration(this.RhythmicOrder, this.RhythmicOrder, this.BarDivision);

            //// this.CurrentTempoNumber = 0;
            this.CurrentTempoNumber = header.Tempo;
            this.MidiTimeToTicksQuotient = MusicalProperties.MidiTimeToTicksQuotient(header.Tempo, header.Division);
            this.NumberOfBars = header.NumberOfBars;

            //// Initialization, current Instrument is then updated later ...
            foreach (var musicalLine in this.MusicalBlock.Strip.Lines) {
                musicalLine.CurrentInstrument = (byte)MidiMelodicInstrument.None;
            }

            this.MidiBars = new List<CompactMidiBar>();
            foreach (var bar in this.MusicalBlock.Body.Bars)
            {
                var midiBar = new CompactMidiBar(this, bar);
                this.MidiBars.Add(midiBar);
            }

            //// 2021/01 this.CollectMidiEvents();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the rhythmic order.
        /// </summary>
        /// <value>
        /// The rhythmic order.
        /// </value>
        public byte RhythmicOrder { get; set; }

        /// <summary>
        /// Gets or sets the current tempo.
        /// </summary>
        /// <value>
        /// The current tempo.
        /// </value>
        public int CurrentTempoNumber { get; set; }

        /// <summary>
        /// Gets or sets the bar division.
        /// </summary>
        /// <value>
        /// The bar division.
        /// </value>
        public int BarDivision { get; set; }

        /// <summary>
        /// Gets or sets the duration of the bit.
        /// </summary>
        /// <value>
        /// The duration of the bit.
        /// </value>
        public int BitDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the bar.
        /// </summary>
        /// <value>
        /// The duration of the bar.
        /// </value>
        public int BarDuration { get; set; }

        /// <summary>
        /// Gets or sets the midi time to ticks quotient.
        /// </summary>
        /// <value>
        /// The midi time to ticks quotient.
        /// </value>
        public float MidiTimeToTicksQuotient { get; set; }

        /// <summary>
        /// Gets or sets the number of bars.
        /// </summary>
        /// <value>
        /// The number of bars.
        /// </value>
        public int NumberOfBars { get; set; }

        /// <summary>
        /// Gets or sets the musical block.
        /// </summary>
        /// <value>
        /// The musical block.
        /// </value>
        public MusicalBlock MusicalBlock
        {
            get
            {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.musicalBlock == null) {
                    throw new InvalidOperationException("Musical block is null.");
                }

                return this.musicalBlock;
            }

            set => this.musicalBlock = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }

        /// <summary>
        /// Gets or sets the midi bars.
        /// </summary>
        /// <value>
        /// The midi bars.
        /// </value>
        public List<CompactMidiBar> MidiBars { get; set;  }

        #endregion

        /// <summary>
        /// Sequences this instance.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public CompactMidiStrip Sequence(string givenName)
        {
            //// Initialization, current Instrument is then updated later ...
            foreach (var musicalLine in this.MusicalBlock.Strip.Lines) {
                musicalLine.CurrentInstrument = (byte)MidiMelodicInstrument.None;
            }

            //// Tempo events are written to zero track!
            MidiTrack zeroTrack = this.PrepareZeroTrack(); //// 2014/01
            var sequence = new CompactMidiStrip(1, this.MusicalBlock.Header.Division);
            sequence.AddTrack(zeroTrack);

            var header = this.MusicalBlock.Header;
            var score = new CompactMidiScore(this.MusicalBlock);
            foreach (var staff in score.Lines) {
                staff.MidiEventCollection = new MidiEventCollection();

                staff.MidiEventCollection.PutMetre(header.Metric.MetricBeat, header.Metric.MetricBase);
                this.CurrentTempoNumber = header.Tempo;
                staff.MidiEventCollection.PutTempo(0, this.CurrentTempoNumber); //// 2020/10 not necessary!?

                foreach (var bar in this.MidiBars) {
                    var musicalBar = bar.MusicalBar;
                    if (bar.MidiEvents == null) {
                        bar.MidiEvents = new MidiEventCollection();
                    }

                    //// Tempo
                    if (musicalBar.TempoNumber > 0 && (musicalBar.TempoNumber != this.CurrentTempoNumber || musicalBar.BarNumber == 1)) {
                        this.CurrentTempoNumber = musicalBar.TempoNumber;
                        this.MidiTimeToTicksQuotient = MusicalProperties.MidiTimeToTicksQuotient(this.CurrentTempoNumber, header.Division);
                        if (this.CurrentTempoNumber > 0) {
                            staff.MidiEventCollection.PutTempo(bar.BarDeltaTime, this.CurrentTempoNumber);
                        }
                    }

                    //// 2021/01 Here new MidiElement with staff.Voice (Octave,Loudness...) !!!
                    var lineElement = (from elem in musicalBar.Elements
                                       where (elem.Line?.LineIdent == staff.Line?.LineIdent && elem.Status?.OrchestraUnit == null)
                                        select elem).FirstOrDefault();

                    if (lineElement != null) {
                        var musicalLine = lineElement.Line;
                        var element = new CompactMidiElement(bar, lineElement, staff);
                        bar.MidiElements.Add(element);
                        var events = element.MidiEvents;
                        if (events != null) {
                            bar.MidiEvents.AddRange(events);
                            staff.MidiEventCollection.SetEventClones(events);
                        }
                    }
                    else {
                        var unitElements = (from elem in musicalBar.Elements
                                            where elem.Status?.OrchestraUnit == staff.OrchestraUnit
                                                && elem.Tones.Count > 0
                                            select elem).ToList();
                        var cnt = unitElements.Count;
                        if (cnt == 0) {
                            continue;
                        }

                        for (int i = 0; i < cnt; i++) {
                            var unitElement = unitElements[i];
                            if (i != (staff.Number % cnt)) {
                                continue;
                            }

                            var element = new CompactMidiElement(bar, unitElement, staff);
                            bar.MidiElements.Add(element);
                            var events = element.MidiEvents;
                            if (events != null) {
                                bar.MidiEvents.AddRange(events);
                                staff.MidiEventCollection.SetEventClones(events);
                            }
                        }
                    }
                }

                if (staff.MidiEventCollection.Count == 0) {
                    continue;        
                }

                staff.MidiEventCollection.Channel = staff.Channel;
                staff.MidiEventCollection.PutInstrument(0, staff.Voice.Instrument.Number);
                staff.MidiEventCollection.SortByStartTime();
                staff.MidiEventCollection.RecomputeDeltaTimes();

                var midiTrack = new MidiTrack(staff.MidiEventCollection) {
                    Name = staff.ToString(),
                    Channel = staff.Channel
                };

                if (staff.LineType == MusicalLineType.Melodic) {
                    /* Validation  var tones = new MidiTones(midiTrack);   
                    if (!tones.CheckTones()) { return false;} */

                    foreach (var ev in midiTrack.Events) {
                        var evpc = ev as VoiceProgramChange;
                        if (evpc != null) {
                            evpc.Channel = staff.Channel;
                            evpc.Number = staff.Voice.Instrument.Number;
                        }
                    }
                }

                sequence.AddTrack(midiTrack);
            }

            sequence.InternalName = givenName;
            return sequence;
        }

        /*
        public CompactMidiStrip Sequence_OriginalCode(string givenName) {
            //// Initialization, current Instrument is then updated later ...
            foreach (var musicalLine in this.MusicalBlock.Strip.Lines) {
                musicalLine.CurrentInstrument = (byte)MidiMelodicInstrument.None;
            }

            foreach (var bar in this.MidiBars) {
                foreach (var element in bar.MidiElements) {
                    var events = element.MidiEvents;
                    element.MusicalElement.MusicalLine.MidiEventCollection.AddRange(events);
                }
            }

            MidiTrack zeroTrack = this.PrepareZeroTrack(); //// 2014/01
            var sequence = new CompactMidiStrip(1, this.MusicalBlock.Header.Division);
            sequence.AddTrack(zeroTrack);

            //// 2021/01 to do---!?!
            //// 1/ make collection of all used instruments and make tracks according to them
            //// 2/ write line tones bar after bar to these tracks
            //// 3/ set channels acording to instruments...
            int channel = 0;
            foreach (var musicalLine in this.MusicalBlock.Strip.Lines) {
                var midiEvents = musicalLine.MidiEventCollection;
                if (midiEvents == null) {
                    continue;
                }

                //// it is written to zero track!
                //// foreach (var bar in this.MidiBars) {
                ////     var tempoEvent = bar.GetTempoEvent();
                //// if (tempoEvent != null) {  midiEvents.Add(tempoEvent);  }   } 

                midiEvents.SortByStartTime();
                midiEvents.RecomputeDeltaTimes();

                var lineVoices = musicalLine.Voices;
                foreach (var voice in lineVoices) {
                    //// midiEvents.SortByStartTime(); //// 2014/01
                    //// 2016/08 midiEvents.Add(new MetaEndOfTrack(0));
                    var midiTrack = new MidiTrack(musicalLine.MidiEventCollection) {
                        Name = musicalLine.Name,
                        Channel = musicalLine.MainVoice.Channel
                    };

                    if (musicalLine.LineType == MusicalLineType.Melodic && !this.MusicalBlock.HasInstrumentInTones) {
                        //// Validation  var tones = new MidiTones(midiTrack);   
                        //// if (!tones.CheckTones()) { return false;} 

                        foreach (var ev in midiTrack.Events) {
                            var evpc = ev as VoiceProgramChange;
                            if (evpc != null) {
                                evpc.Channel = (MidiChannel)(channel++ % 16);
                                evpc.Number = voice.Instrument.Number;
                            }
                        }
                    }

                    sequence.AddTrack(midiTrack);
                }
            }

            sequence.InternalName = givenName;
            return sequence;
        } */

        /// <summary>
        /// Collects the musical bar events.
        /// </summary>
        public void CollectMidiEvents()
        {
            foreach (var bar in this.MidiBars)
            {
                bar.CollectMidiEvents();
            }
        }

        /// <summary>
        /// Enqueues the midi events.
        /// </summary>
        public void EnqueueMidiEvents() {
            foreach (var bar in this.MidiBars) {
                bar.EnqueueMidiEvents();
            }
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat(
                    "MidiBlock R {0} Div {1} Bit {2} Dur {3} Q {4} Bars {5}", this.RhythmicOrder, this.BarDivision, this.BitDuration, this.BarDuration, this.MidiTimeToTicksQuotient, this.NumberOfBars);

            return s.ToString();
        }
        #endregion

        #region Midi - private static methods

        /// <summary>
        /// Writes the meta harmony.
        /// </summary>
        /// <param name="midiEvents">The midi events.</param>
        /// <param name="bitDuration">Duration of the bit.</param>
        /// <param name="barDeltaTime">The bar delta time.</param>
        /// <param name="musicalBar">The musical bar.</param>
        private static void WriteMetaHarmony(MidiEventCollection midiEvents, int bitDuration, long barDeltaTime, MusicalBar musicalBar)
        {
            Contract.Requires(midiEvents != null);
            Contract.Requires(musicalBar != null);
            if (musicalBar?.HarmonicBar?.HarmonicStructures == null) {
                return;
            }

            var rhythmicLevel = (byte)musicalBar.HarmonicBar.HarmonicStructures.Count;
            var rhythmicShape = musicalBar.HarmonicBar.RhythmicShape;
            for (byte rl = 0; rl < rhythmicLevel; rl++) {
                var harStrTick = (byte)(rhythmicShape != null && rl < rhythmicShape.BitPlaces.Count ? rhythmicShape.BitPlaces[rl] : 0);

                var harStruct = musicalBar.HarmonicBar.HarmonicStructureAtRhythmicLevel(rl);
                if (harStruct == null) {
                    continue;
                }

                //// long harStructNumber = GeneralSystem.ConvertStruct(harStruct.Number, harStruct.GSystem.Order, DefaultValue.HarmonicOrder); //// 2
                //// var harSystem = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
                //// HarmonicModality modality = musicalBar.HarmonicBar.HarmonicModalityFromStructures(MusicalSettings.Singleton.MinimalModalityLevel);
                //// HarmonicModality modality = harStruct.HarmonicModality;
                //// var modality = musicalBar.HarmonicBar.HarmonicBar.HarmonicModality;

                ////201508
                //// var harStr = DataLink.BridgeHarmony.CompleteHarmonicStructure(harSystem, modality, harStruct.GetStructuralCode()); //// harStructNumber

                //// HarmonicStructure harStr = null;
                //// if (harStr != null) {
                var shortcut = harStruct.Shortcut; ////  .ToneSchema;
                var metaText = shortcut + " "; //// string.Format(CultureInfo.CurrentCulture, "{0}({1})", shortcut, harStruct.ToneSchema);
                midiEvents.PutMetaText(1 + barDeltaTime + (bitDuration * harStrTick), metaText);
                //// }
            }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Prepares the zero track.
        /// </summary>
        /// <returns> Returns value. </returns>
        private MidiTrack PrepareZeroTrack()
        {
            var block = this.MusicalBlock;
            var midiEvents = new MidiEventCollection();  //// midiMaker.Track(trackNum++);
            //// midiEvents.PutMetaText(0, string.Empty);
            midiEvents.PutMetaCopyright(SettingsApplication.ApplicationInfo);
            midiEvents.PutKeySignature(0, block.TonalityKey, block.TonalityGenus);
            midiEvents.PutTempo(0, this.CurrentTempoNumber);
            midiEvents.PutMetre(block.Header.Metric.MetricBeat, block.Header.Metric.MetricBase);

            this.CurrentTempoNumber = 0;
            foreach (var bar in this.MidiBars) {
                var tempoEvent = bar.GetTempoEvent();
                if (tempoEvent != null) {
                    midiEvents.Add(tempoEvent);
                    //// midiEvents.PutTempo(this.CurrentTempoNumber);
                }

                //// 2019/02
                WriteMetaHarmony(midiEvents, this.BitDuration, bar.BarDeltaTime, bar.MusicalBar);
            }
            
            /*  var tempoEvents = block.Body.TempoEvents;
            var ready = false;
            if (tempoEvents != null) {
                var enumerable = tempoEvents as IList<IMidiEvent> ?? tempoEvents.ToList();
                if (enumerable.Any()) {
                    midiEvents.AddRange(enumerable);
                    ready = true;
                }
            }

            if (!ready) {
                if (block.Header.Tempo > 0) {
                    midiEvents.PutTempo(this.MusicalBlock.Header.Tempo);
                }
            } */

            midiEvents.SortByStartTime();
            midiEvents.RecomputeDeltaTimes();
            var midiTrack = new MidiTrack(midiEvents) { Name = "CompactMidi" };

            return midiTrack;
        }
        #endregion
    }
}