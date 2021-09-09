// <copyright file="MidiBlocksSolver.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.MidiFile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    using LargoSharedClasses.Midi;
    using Music;

    /// <summary>
    /// Midi Blocks Solver.
    /// </summary>
    public sealed class MidiBlocksSolver
    {
        #region Fields
        /// <summary>
        /// Tempo Serious Change Tolerance.
        /// </summary>
        private const int TempoSeriousChangeTolerance = 25; //// 20- 30
        //// private const int tempoChangeTolerance = 10; //// 10 

        /// <summary>
        /// Tempo Block Change Tolerance.
        /// </summary>
        private const int TempoBlockChangeLimit = 60;

        /// <summary>
        /// Minimum Block Length.
        /// </summary>
        private const int MinimumBlockLength = 20;

        /// <summary>
        /// Break events.
        /// </summary>
        private readonly IEnumerable<MidiEvent> breakEvents;

        /// <summary>
        /// Block Record.
        /// </summary>
        private BlockRecord currentRecord;

        /// <summary>
        /// Block Record.
        /// </summary>
        private BlockRecord lastRecord;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiBlocksSolver" /> class.
        /// </summary>
        /// <param name="givenSequence">The given sequence.</param>
        /// <param name="givenBreakEvents">The given break events.</param>
        public MidiBlocksSolver(CompactMidiStrip givenSequence, IEnumerable<MidiEvent> givenBreakEvents)
        {
            this.Sequence = givenSequence;
            this.breakEvents = givenBreakEvents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiBlocksSolver"/> class.
        /// </summary>
        [UsedImplicitly]
        public MidiBlocksSolver()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CompactMidiStrip Sequence { get; set; }

        /// <summary>
        /// Gets the main events.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        private IEnumerable<MidiEvent> MainEvents
        {
            get
            {
                var selectedEvents = new List<MidiEvent>();
                MidiEvent lastEvent = null;
                var lastTempo = 0;
                foreach (var ev in this.breakEvents.Where(ev => ev != null))
                {
                    this.ReadCurrentValues(ev);

                    if (string.CompareOrdinal(this.currentRecord.EventType, "MetaTempo") == 0)
                    {
                        if (lastEvent == null)
                        {
                            selectedEvents.Add(ev);
                            lastEvent = ev;
                            lastTempo = this.currentRecord.Tempo;
                            continue;
                        }

                        var tempoChange = Math.Abs(lastTempo - this.currentRecord.Tempo);
                        if (tempoChange <= 0)
                        {
                            continue;
                        }

                        if (tempoChange <= TempoSeriousChangeTolerance)
                        {
                            continue;
                        }

                        selectedEvents.Add(ev);
                        lastTempo = this.currentRecord.Tempo;
                    }
                    else
                    {
                        selectedEvents.Add(ev);
                    }
                }

                return selectedEvents;
            }
        }
        #endregion

        /// <summary>
        /// Determines the blocks.
        /// </summary>
        /// <param name="finalEvent">The final event.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IEnumerable<MidiBlock> DetermineBlocks(IMidiEvent finalEvent)
        {
            var mainEvents = this.MainEvents;
            var midiBlocks = new Collection<MidiBlock>();
            this.currentRecord = new BlockRecord(string.Empty, 4, 2, TonalityKey.None, 0);
            this.lastRecord = new BlockRecord(string.Empty, 4, 2, TonalityKey.None, 0);
            var lastBarNumber = 1;
            long lastStartTime = 0;
            MidiBlock midiBlock = null;
            int barNumber, barDifference;

            foreach (var ev in mainEvents.Where(ev => ev != null))
            {
                this.ReadCurrentValues(ev);
                barNumber = this.DetermineBarNumber(this.Sequence.Header.Division, ev);
                barDifference = barNumber - lastBarNumber;
                var streamChange = this.DetermineTypeOfChange(this.lastRecord.Tempo, this.lastRecord.MetricBeat, this.lastRecord.MetricBase); //// this.lastRecord.TonalityKey

                if (streamChange != MidiStreamChange.Serious)
                { //// || (streamChange == MidiStreamChange.Common && (barDifference > 31)
                    continue;
                }

                if (barDifference > 0)
                {
                    //// Check tempo in the previous block 
                    midiBlock?.CheckTempo();
                    if (this.Sequence.Header.Clone() is MusicalHeader header)
                    {
                        header.Metric.MetricBase = this.lastRecord.MetricBase;
                        header.Metric.MetricBeat = this.lastRecord.MetricBeat;
                        midiBlock = new MidiBlock(
                            lastBarNumber,
                            header, 
                            this.lastRecord.TonalityKey, 
                            this.lastRecord.Tempo)
                        {
                            MidiTimeFrom = lastStartTime,
                            MidiTimeTo = ev.StartTime,
                            Area = { BarTo = barNumber - 1 }
                        };
                    }

                    midiBlocks.Add(midiBlock);
                    if (midiBlock?.Header != null)
                    {
                        midiBlock.Header.Number = midiBlocks.Count;
                    }

                    lastBarNumber = barNumber; //// 2013/03
                    lastStartTime = ev.StartTime;
                }

                //// Properties of the last block
                this.lastRecord.GetValuesFrom(this.currentRecord);
            }

            //// Check tempo in the previous block 
            midiBlock?.CheckTempo();

            // ReSharper disable once InvertIf
            if (finalEvent != null)
            {
                this.ReadCurrentValues(finalEvent);
                barNumber = this.DetermineBarNumber(this.Sequence.Header.Division, finalEvent);
                barDifference = barNumber - lastBarNumber;

                // ReSharper disable once InvertIf
                if (barDifference > 0)
                {
                    if (this.Sequence.Header.Clone() is MusicalHeader musicHeader)
                    {
                        musicHeader.Metric.MetricBase = this.lastRecord.MetricBase;
                        musicHeader.Metric.MetricBeat = this.lastRecord.MetricBeat;
                        midiBlock = new MidiBlock(
                            lastBarNumber,
                            musicHeader, 
                            this.lastRecord.TonalityKey, 
                            this.lastRecord.Tempo)
                        {
                            MidiTimeFrom = lastStartTime,
                            MidiTimeTo = finalEvent.StartTime,
                            Area = { BarTo = barNumber }
                        };
                    }

                    if (midiBlock != null)
                    {
                        midiBlock.CheckTempo();
                        midiBlocks.Add(midiBlock);
                        midiBlock.Header.Number = midiBlocks.Count;
                    }
                }
            }

            var blocks = OptimizeBlocks(midiBlocks);

            var determineBlocks = blocks.ToList();
            foreach (var block in determineBlocks)
            {
                block.Sequence = new CompactMidiStrip(this.Sequence.Format, this.Sequence.Header.Division);
                foreach (var originTrack in this.Sequence)
                {
                    var track = new MidiTrack();

                    //// 2020/02 E.g. instruments can be defined in any previous block 
                    var zeroEvents = (from ev in originTrack.Events
                                          where ev.StartTime < block.MidiTimeFrom  && ev.EventType == "VoiceProgramChange"
                                          orderby ev.StartTime   
                                          select ev).ToList();
                    foreach (var ev in zeroEvents) { //// 2019/02
                        ev.StartTime = 0;
                    }

                    track.Events.AddRange(zeroEvents);

                    var events = (from ev in originTrack.Events
                        where ev.StartTime >= block.MidiTimeFrom && ev.StartTime <= block.MidiTimeTo
                        orderby ev.StartTime ////, tone.Duration  
                        select ev).ToList();
                    foreach (var ev in events) { //// 2019/02
                        ev.StartTime = ev.StartTime - block.MidiTimeFrom;
                    }

                    track.Events.AddRange(events);
                    track.Events.RecomputeDeltaTimes(); //// 2019/02
                    block.Sequence.AddTrack(track);
                }
            }

            return determineBlocks;
        }

        #region Private Static methods
        /// <summary>
        /// Merges the blocks.
        /// </summary>
        /// <param name="midiBlocks">The midi blocks.</param>
        /// <returns> Returns value.</returns>
        private static IEnumerable<MidiBlock> OptimizeBlocks(IEnumerable<MidiBlock> midiBlocks)
        {
            Contract.Requires(midiBlocks != null);

            var blocks = midiBlocks.Where(block => block != null).ToList();
            if (blocks.Count <= 1)
            {
                return blocks;
            }

            var mergedBlocks = new List<MidiBlock>();
            MidiBlock lastBlock = null;
            foreach (var block in blocks)
            {
                if (lastBlock == null)
                {
                    mergedBlocks.Add(block);
                    block.Header.Number = mergedBlocks.Count;
                    lastBlock = block;
                    continue;
                }

                if (block.Header.Metric.MetricBase == lastBlock.Header.Metric.MetricBase && block.Header.Metric.MetricBeat == lastBlock.Header.Metric.MetricBeat
                    && block.TonalityKey == lastBlock.TonalityKey
                    && Math.Abs(block.Tempo - lastBlock.Tempo) < TempoBlockChangeLimit)
                {
                    if (lastBlock.Area.Length < MinimumBlockLength || block.Area.Length < MinimumBlockLength || lastBlock.Tempo == block.Tempo)
                    {
                        lastBlock.Area.BarTo = block.Area.BarTo;
                        lastBlock.MidiTimeTo = block.MidiTimeTo;
                        continue;
                    }
                }

                mergedBlocks.Add(block);
                block.Header.Number = mergedBlocks.Count;
                lastBlock = block;
            }

            return mergedBlocks;
        }
        #endregion

        /// <summary>
        /// Determines the bar number.
        /// </summary>
        /// <param name="givenDivision">The given division.</param>
        /// <param name="givenEvent">The given event.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private int DetermineBarNumber(int givenDivision, IMidiEvent givenEvent)
        { //// out byte metricGround, out int barDivision, out int barNumber
            var metricGround = MusicalProperties.GetMetricGround(this.currentRecord.MetricBase);
            var barDivision = MusicalProperties.BarDivision(givenDivision, this.currentRecord.MetricBeat, metricGround);
            var barNumber = (int)Math.Floor((double)givenEvent.StartTime / barDivision) + 1;
            return barNumber;
        }

        /// <summary>
        /// Reads the current values.
        /// </summary>
        /// <param name="ev">The Midi Event.</param>
        private void ReadCurrentValues(IMidiEvent ev)
        {
            Contract.Requires(ev != null);
            var eventType = ev.EventType;
            this.currentRecord.EventType = eventType;
            switch (eventType)
            {
                case "MetaTempo":
                    {
                        this.currentRecord.Tempo = ((MetaTempo)ev).Tempo;
                        break;
                    }

                case "MetaKeySignature":
                    {
                        this.currentRecord.TonalityKey = ((MetaKeySignature)ev).Key;
                        break;
                    }

                case "MetaTimeSignature":
                    {
                        var ts = (MetaTimeSignature)ev;
                        this.currentRecord.MetricBeat = ts.Numerator;
                        this.currentRecord.MetricBase = ts.Denominator;
                        break;
                    }
                //// resharper default: { break; }
            }
        }

        /// <summary>
        /// Determines the type of change.
        /// </summary>
        /// <param name="lastTempo">The last tempo.</param>
        /// <param name="lastMetricBeat">The last metric beat.</param>
        /// <param name="lastMetricBase">The last metric base.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MidiStreamChange DetermineTypeOfChange(int lastTempo, byte lastMetricBeat, byte lastMetricBase)
        { //// TonalityKey lastTonalityKey, 
            //// 2014 !!?!?  //// 2015/01  || (lastTonalityKey != this.currentRecord.TonalityKey)
            if ((lastMetricBeat != this.currentRecord.MetricBeat)
                || (lastMetricBase != this.currentRecord.MetricBase))
            {
                //// || (lastTempo != this.currentRecord.Tempo)) {
                return MidiStreamChange.Serious;
            }

            if (lastTempo == 0 && this.currentRecord.Tempo > 0)
            {
                return MidiStreamChange.Serious;
            }

            return MidiStreamChange.None;
        }
    }
}
