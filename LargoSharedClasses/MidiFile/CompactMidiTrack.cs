// <copyright file="CompactMidiTrack.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Compact Midi Track.
    /// </summary>
    public class CompactMidiTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiTrack"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="givenLine">The given line.</param>
        public CompactMidiTrack(MusicalBlock givenBlock, MusicalLine givenLine) {
            this.Block = givenBlock;
            this.Line = givenLine;
            this.Header = givenBlock.Header;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiTrack"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public CompactMidiTrack(MusicalBlock givenBlock) { //// PrepareZeroTrack
            this.Block = givenBlock; //// 2021/01 
            this.Header = givenBlock.Header; //// 2021/01 
            var musicalLine = new MusicalLine();
            var midiEvents = new MidiEventCollection();  //// midiMaker.Track(trackNum++);
            midiEvents.PutMetaText(0, musicalLine.MidiMetaText);
            midiEvents.PutMetaCopyright(SettingsApplication.ApplicationInfo);
            midiEvents.PutKeySignature(0, this.Block.TonalityKey, this.Block.TonalityGenus);
            midiEvents.PutMetre(this.Header.Metric.MetricBeat, this.Header.Metric.MetricBase);

            var tempoEvents = this.Block.Body.TempoEvents;
            var ready = false;
            if (tempoEvents != null) {
                var enumerable = tempoEvents as IList<IMidiEvent> ?? tempoEvents.ToList();
                if (enumerable.Any()) {
                    midiEvents.AddRange(enumerable);
                    ready = true;
                }
            }

            if (!ready) {
                if (this.Header.Tempo > 0) {
                    midiEvents.PutTempo(0, this.Header.Tempo);
                }
            }

            midiEvents.SortByStartTime();
            midiEvents.RecomputeDeltaTimes();
            //// 2020/10  musicalLine.SetMidiEventCollection(midiEvents);   musicalLine.Purpose = LinePurpose.Fixed;  return musicalLine;
        }

        #region Properties
        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        private MusicalBlock Block { get; }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        private MusicalLine Line { get; }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        private MusicalHeader Header { get; }

        #endregion

        #region Private methods

        /// <summary>
        /// Writes musical block to MIDI sequence.
        /// </summary>
        /// <param name="sequence">Midi Sequence.</param>
        public void FinishTrackMidiExport(CompactMidiStrip sequence) {
            Contract.Requires(sequence != null);
            if (sequence == null || this.Line == null) {
                return;
            }

            var midiEvents = this.Line.MidiEventCollection;
            if (midiEvents == null) {
                return;
            }

            //// midiEvents.SortByStartTime(); //// 2014/01
            //// 2016/08 midiEvents.Add(new MetaEndOfTrack(0));
            var midiTrack = new MidiTrack(this.Line.MidiEventCollection) { Name = this.Line.Name };
            sequence.AddTrack(midiTrack);
        }

        /// <summary>
        /// Exports the track tones.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="givenTones">The given tones.</param>
        /// <param name="skipToStartBar">if set to <c>true</c> [skip to start bar].</param>
        /// <param name="writeHarmony">if set to <c>true</c> [write harmony].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool WriteTrack(CompactMidiStrip sequence, ToneCollection givenTones, bool skipToStartBar, ref bool writeHarmony) {
            if (this.Line != null && (this.Line.Purpose == LinePurpose.Fixed || this.Line.Purpose == LinePurpose.Composed) && !this.Line.IsEmpty) {
                this.StartTrackMidiExport();
                //// if (!mt.IsImported) { mt.Status.LineType = MusicalLineType.None; } 

                //// 2016 track.Status = null;
                if (givenTones != null) {
                    this.ExportTrackToMidi(givenTones, writeHarmony, skipToStartBar);

                    //// 2015/01
                    var track = new MidiTrack(this.Line.MidiEventCollection);
                    var tones = new MidiTones(track);
                    if (!tones.CheckTones()) {
                        return false;
                    }

                    writeHarmony = false;
                }
            }

            if (this.Line != null && (this.Line.Purpose == LinePurpose.Fixed || this.Line.Purpose == LinePurpose.Composed) && !this.Line.IsEmpty) {
                this.FinishTrackMidiExport(sequence);
            }

            return true;
        }

        /// <summary>
        /// Exports one bar of the part to MIDI.
        /// </summary>
        private void StartTrackMidiExport() {
            if (this.Line == null) {
                return;
            }

            var midiEvents = new MidiEventCollection(this.Line.MainVoice.Channel);
            this.Line.SetMidiEventCollection(midiEvents);
        }

        /// <summary>
        /// Export To Midi.
        /// </summary>
        /// <param name="musicalTones">Given real tones.</param>
        /// <param name="writeHarmony">If set to <c>true</c> [write harmony].</param>
        /// <param name="skipToStartBar">if set to <c>true</c> [skip to start bar].</param>
        private void ExportTrackToMidi(IList<IMusicalTone> musicalTones, bool writeHarmony, bool skipToStartBar) {
            Contract.Requires(this.Line != null);
            Contract.Requires(musicalTones != null);
            if (musicalTones == null || this.Header.Metric.MetricBeat == 0) {
                return;
            }

            var midiEvents = this.Line.MidiEventCollection;
            midiEvents.Name = this.Header.FileName;
            byte lastInstrument = 127;
            //// byte lastChannel = 127;

            var firstTone = musicalTones.FirstOrDefault();
            var lastTone = musicalTones.LastOrDefault();
            if (firstTone == null || lastTone == null) {
                return;
            }

            var rhythmicOrder = this.Header.System.RhythmicOrder;
            var barDivision = MusicalProperties.BarDivision(this.Header.Division, this.Header.Metric.MetricBeat, this.Header.Metric.MetricGround);
            var bitDuration = MusicalProperties.MidiDuration(rhythmicOrder, 1, barDivision);
            var barDuration = MusicalProperties.MidiDuration(rhythmicOrder, rhythmicOrder, barDivision);
            //// This skip cannot be done for separate tracks, tracks can not have different skip ...
            //// Serves for playing short tone phrases only. 
            int deltaTimeShift = skipToStartBar ? (firstTone.BarNumber - 1) * barDuration : 0;
            //// StringBuilder sb = new StringBuilder(); 
            var lastBarNumber = 0;
            foreach (var mt in musicalTones) {
                if (mt.IsPause) {
                    //// 2019/11 !?!
                    if (mt.InstrumentNumber != lastInstrument) { //// channel != lastChannel || 
                        //// this.Status.LineType = MusicalLineType.None;
                        //// if (mtone.Channel != MidiChannel.DrumChannel) {
                        //// The decrement -1 at the and of row is important (becaose of event sorting), see alse SortByStartTime below!
                        //// But delta time can not be under zero.
                        var deltaTime = 1 + 0 - deltaTimeShift + (bitDuration * mt.BitFrom) - 1;
                        //// test midiEvents.PutInstrument(deltaTime, mtone.Instrument, lastChannel ?? channel);
                        midiEvents.PutInstrument(deltaTime, mt.InstrumentNumber);
                        //// }

                        //// lastChannel = channel;
                        lastInstrument = mt.InstrumentNumber;
                    }

                    mt.WriteTo(midiEvents, barDivision, bitDuration, barDuration, deltaTimeShift);
                    continue;
                }

                if (!(mt is MusicalTone mtone)) {
                    continue;
                }

                var barDeltaTime = 0;
                //// Bar change
                var bars = this.Block.Body.Bars;
                if (bars.Count > 0) {
                    if (mtone.BarNumber != lastBarNumber) { //// this.MusicalBlock.SourceMusicalBlockModel != null &&
                        if (mtone.BarNumber < 1 || mtone.BarNumber > bars.Count) {
                            break;
                        }

                        var musicalBar = bars[mtone.BarNumber - 1];
                        barDeltaTime = barDuration * (mtone.BarNumber - 1);
                        if (writeHarmony && musicalBar != null) {
                            PortMidi.WriteMetaHarmony(midiEvents, bitDuration, barDeltaTime, musicalBar);
                        }

                        //// Test of MidiController
                        //// MidiController.HoldPedalOnOff, MidiController.PortamentoOnOff, MidiController.ChorusLevel, MidiController.LegatoPedalOnOff
                        //// MidiController.TremoloLevel, MidiController.SoundVariation, MidiController.OmniModeOn
                        //// midiEvents.PutControlChange(barDeltaTime, (byte)mtone.Channel, MidiController.HoldPedalOnOff, 0);
                        //// midiEvents.PutControlChange(barDeltaTime, (byte)mtone.Channel, MidiController.HoldPedalOnOff, 1); 
                        lastBarNumber = mtone.BarNumber;
                    }
                }

                //// if (mtone.BarNumber > mtone.BarNumberTo) { continue;  } 
                if (!mtone.IsPause && mtone.ToneType != MusicalToneType.Empty) {
                    //// var channel = (byte)mtone.Channel;
                    if (mtone.InstrumentNumber != lastInstrument) { //// channel != lastChannel || 
                        //// this.Status.LineType = MusicalLineType.None;
                        //// if (mtone.Channel != MidiChannel.DrumChannel) {
                        //// The decrement -1 at the and of row is important (because of event sorting), see alse SortByStartTime below!
                        //// But delta time can not be under zero.
                        var deltaTime = 1 + barDeltaTime - deltaTimeShift + (bitDuration * mtone.BitFrom) - 1;
                        //// test midiEvents.PutInstrument(deltaTime, mtone.Instrument, lastChannel ?? channel);
                        midiEvents.PutInstrument(deltaTime, mtone.InstrumentNumber);
                        //// }

                        //// lastChannel = channel;
                        lastInstrument = mtone.InstrumentNumber;
                    }
                }

                //// var barStartTime = 1 + (barDuration * (mtone.BarNumber - 1)) - deltaTimeShift;
                mtone.WriteTo(midiEvents, barDivision, bitDuration, barDuration, deltaTimeShift);
            }

            //// string s = sb.ToString(); 
            //// Important - the tone off must be first in given time! (before tone on)
            midiEvents.SortByStartTime();
            midiEvents.RecomputeDeltaTimes();
        }
        #endregion
    }
}
