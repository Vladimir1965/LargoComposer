// <copyright file="ToneCollection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Notation;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Strike Collection.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public sealed class ToneCollection : Collection<IMusicalTone>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ToneCollection class.
        /// </summary>
        public ToneCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ToneCollection class.
        /// </summary>
        /// <param name="givenList">Given list.</param>
        public ToneCollection(IList<IMusicalTone> givenList)
            : base(givenList)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToneCollection"/> class.
        /// </summary>
        /// <param name="xmlTones">The Xml tones.</param>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        public ToneCollection(XElement xmlTones, byte rhythmicOrder)
        {
            Contract.Requires(xmlTones != null);

            byte lastInstrument = (byte)MidiMelodicInstrument.None;
            foreach (var xtone in xmlTones.Elements()) {
                IMusicalTone mt = null;
                switch (xtone.Name.ToString()) {
                    case "Tone": {
                            mt = new MusicalTone(xtone, rhythmicOrder);
                            break;
                        }

                    case "Pause": {
                            mt = new MusicalPause(xtone, rhythmicOrder);
                            break;
                        }

                    case "Tick": {
                            mt = new MusicalStrike(xtone, rhythmicOrder);
                            break;
                        }
                }

                if (mt == null) {
                    continue;
                }

                //// 2019/02- instrument is also property of pauses ... 
                if (mt.InstrumentNumber == (byte)MidiMelodicInstrument.None) {
                    mt.InstrumentNumber = lastInstrument;
                }

                lastInstrument = mt.InstrumentNumber;
                //// mt.BarNumber = LibSupport.ReadIntegerAttribute(xtone.Attribute("BarNumber"));
                this.Add(mt);
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public XElement GetXElement {
            get
            {
                var xtones = new XElement("Tones", null);

                foreach (var xtone in this.Select(mt => mt.GetXElement).Where(xtone => xtone != null)) {
                    xtones.Add(xtone);
                }

                return xtones;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Name ( there was an Identifier in the Harmonic Stream Ground class ...).
        /// </summary>
        /// <value> Property description. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets Count Of MelTones.
        /// </summary>
        /// <value> Property description. </value>
        public int CountOfMelTones
        {
            get
            {
                if (this.Count == 0) {
                    return 0;
                }

                var num = (from mt in this where mt.ToneType == MusicalToneType.Melodic select 1).Count();
                return num;
            }
        }

        /// <summary>
        /// Gets the count of sounding tones.
        /// </summary>
        /// <value> Property description. </value>
        public int CountOfSoundingTones
        {
            get
            {
                if (!this.HasAnySoundingTone) {
                    return 0;
                }

                var num = (from mt in this
                           where mt.ToneType == MusicalToneType.Melodic && !mt.IsPause
                           select 1).Count();
                return num;
            }
        }

        /// <summary>Gets Mean Loudness. </summary>
        /// <value> Property description. </value>
        public MusicalLoudness MeanLoudness
        {
            get
            {
                const int limit = 100;
                float sumLoudness = 0;
                var numNotes = 0;
                var tones = this.Where(mt => mt != null && mt.ToneType != MusicalToneType.Empty);
                foreach (var mt in tones) {
                    if (mt is MusicalStrike mtone && mtone.Loudness > 0) {
                        sumLoudness += (short)mtone.Loudness;
                        numNotes++;
                        if (numNotes >= limit) {
                            break;
                        }
                    }
                }

                var meanLoudness = numNotes != 0 ? sumLoudness / numNotes : 0;

                return (MusicalLoudness)(byte)Math.Round(meanLoudness);
            }
        }

        /// <summary>
        /// Gets the mean midi key number.
        /// </summary>
        /// <value>
        /// The mean midi key number.
        /// </value>
        public byte MeanMidiKeyNumber
        {
            get
            {
                const int limit = 50; //// .Take(50)
                float sumMidiKeys = 0;
                var numNotes = 0;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var musicalStrike in this.Where(musicalStrike => musicalStrike != null && !musicalStrike.IsPause)) {
                    MusicalTone musicalTone = musicalStrike as MusicalTone;
                    if (musicalTone == null || musicalTone.IsEmpty) {
                        continue;
                    }

                    sumMidiKeys += musicalTone.Pitch.MidiKeyNumber;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanMidiKey = numNotes != 0 ? sumMidiKeys / numNotes : 0;
                return (byte)Math.Round(meanMidiKey);
            }
        }

        /// <summary>
        /// Gets the mean octave.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalOctave MeanOctave
        {
            get
            {
                const int limit = 50; //// .Take(50)
                float sumOctave = 0;
                var numNotes = 0;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var musicalStrike in this.Where(musicalStrike => musicalStrike != null && !musicalStrike.IsPause)) {
                    MusicalTone musicalTone = musicalStrike as MusicalTone;
                    if (musicalTone == null || musicalTone.IsEmpty) {
                        continue;
                    }

                    sumOctave += musicalTone.Pitch.Octave;
                    numNotes++;
                    if (numNotes >= limit) {
                        break;
                    }
                }

                var meanOctave = numNotes != 0 ? sumOctave / numNotes : 0;
                return (MusicalOctave)(byte)Math.Round(meanOctave);
            }
        }

        /// <summary>
        /// Gets the first instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MidiMelodicInstrument FirstMelodicInstrument
        {
            get
            {
                var instrument = MidiMelodicInstrument.None;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var musicalStrike in this.Where(musicalStrike => musicalStrike != null && !musicalStrike.IsPause)) {
                    MusicalTone musicalTone = musicalStrike as MusicalTone;
                    if (musicalTone == null || musicalTone.IsEmpty) {
                        continue;
                    }

                    instrument = (MidiMelodicInstrument)musicalTone.InstrumentNumber;
                    break;
                }

                return instrument;
            }
        }

        /// <summary>
        /// Gets the first rhythmic instrument.
        /// </summary>
        /// <value>
        /// The first rhythmic instrument.
        /// </value>
        public MidiRhythmicInstrument FirstRhythmicInstrument
        {
            get
            {
                var instrument = MidiRhythmicInstrument.None;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var musicalStrike in this.Where(musicalStrike => musicalStrike != null && !musicalStrike.IsPause)) {
                    instrument = (MidiRhythmicInstrument)musicalStrike.InstrumentNumber;
                    break;
                }

                return instrument;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [has any sounding tone].
        /// </summary>
        /// <value> Property description. </value>
        /// <returns>
        /// <c>True</c> if [has any sounding tone]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAnySoundingTone
        {
            get
            {
                return this.Count != 0 && this.Any(mt => !mt.IsPause);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has true tones.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has true tones; otherwise, <c>false</c>.
        /// </value>
        public bool HasTrueTones
        {
            get
            {
                foreach (var musicalStrike in this.Where(musicalStrike => musicalStrike != null && !musicalStrike.IsPause)) {
                    if (!(musicalStrike is MusicalTone musicalTone) || musicalTone.IsEmpty) {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the melodic pattern identifier.
        /// </summary>
        /// <value>
        /// The melodic pattern identifier.
        /// </value>
        public string MelodicPatternIdentifier
        {
            get
            {
                var sb = new StringBuilder("Idents");

                foreach (var ident in this.Select(mt => mt.MelodicIdentifier).Where(xtone => xtone != null)) {
                    sb.Append(ident);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the rhythmic pattern identifier.
        /// </summary>
        /// <value>
        /// The rhythmic pattern identifier.
        /// </value>
        public string RhythmicPatternIdentifier
        {
            get
            {
                var sb = new StringBuilder("Idents");

                foreach (var ident in this.Select(mt => mt.RhythmicIdentifier).Where(xtone => xtone != null)) {
                    sb.Append(ident);
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Public static
        /// <summary>
        /// Gets the tones from Midi tones.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="midiTones">The midi tones.</param>
        /// <param name="melodicTrack">if set to <c>true</c> [melodic line].</param>
        /// <returns>Returns value.</returns>
        public static ToneCollection GetTones(MusicalHeader givenHeader, IMidiTones midiTones, bool melodicTrack) { //// this.Strip.Context.Header
            Contract.Requires(midiTones != null);
            //// Contract.Requires(line.Sequence != null);
            Contract.Requires(givenHeader.Metric.MetricGround > 0);

            if (midiTones == null) { //// || this.MusicalBlock.Metric.MetricGround == 0
                return null;
            }

            var rhythmicOrder = givenHeader.System.RhythmicOrder;
            //// !!!!! int division = line.Sequence.Division;
            //// int division = this.MusicalBlock.Division;
            //// line.BarDivision = MusicalProperties.BarDivision(division, this.MusicalBlock.MetricBeat, this.MusicalBlock.MetricGround);

            //// rhythmicOrder must be integer divisor of barDivision
            var quotient = midiTones.BarDivision / rhythmicOrder;
            //// !!!!!  if (line.MidiTones.Count == 0 && line.Events.Count < 0) { line.ReadMidiTones(line); }

            var mtones = new List<IMusicalTone>();
            if ((midiTones.List.Count == 0) || (quotient == 0)) {
                return null;
            }

            //// int numberOfBarsInTrack = midiTones.NumberOfBars;
            //// this.MusicalBlock.NumberOfBars = Math.Max(this.MusicalBlock.NumberOfBars, numberOfBarsInTrack);
            //// int test = 0;
            foreach (var midiTone in midiTones.List) {
                for (var barNumber = midiTone.BarNumberFrom; barNumber <= midiTone.BarNumberTo; barNumber++) {
                    var realBarNumber = midiTones.FirstBarNumber + barNumber - 1;
                    //// if (midiTone.Note == "A#1" && realBarNumber >= 3) { //// && this.MusicalBlock.NumberOfBars == 32
                    //// test = 1; } 
                    //// test = 2 * test;
                    var musTone = MusicalStrike.GetNewMusicalTone(givenHeader, quotient, midiTone, barNumber, realBarNumber, melodicTrack);
                    if (musTone == null) {
                        continue;
                    }

                    mtones.Add(musTone);
                }
            }

            var selectedTones = (from tone in mtones orderby tone.BarNumber, tone.BitFrom select tone).ToList();
            var tones = new ToneCollection(selectedTones);
            //// if (tones == null) {   return null;     }
            var completedTones = tones.CollectionWithAddedMissingPauses();
            //// if (completedTones == null) {      return null;         }

            if (completedTones == null) { //// 2020/09
                return tones;      
            }

            var standardizedTones = completedTones.StandardizeTones(givenHeader);
            return standardizedTones;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Add on musical tone to the end of part.
        /// </summary>
        /// <param name="givenTone">Musical tone.</param>
        /// <param name="setOrdinalIndex">If set to <c>true</c> [set ordinal index].</param>
        public void AddTone(IMusicalTone givenTone, bool setOrdinalIndex)
        {
            Contract.Requires(givenTone != null);
            //// if (givenTone == null) { return false;  }

            if (setOrdinalIndex) {
                givenTone.OrdinalIndex = this.Count;
            }

            this.Add(givenTone);
        }

        /// <summary>
        /// Adds the collection.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        /// <param name="setOrdinalIndex">If set to <c>true</c> [set ordinal index].</param>
        public void AddCollection(ToneCollection givenTones, bool setOrdinalIndex)
        {
            Contract.Requires(givenTones != null);

            givenTones.ForAll(musicalStrike => this.AddTone(musicalStrike, setOrdinalIndex));
        }

        /// <summary>
        /// Adds the collection.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        /// <param name="givenBarNumber">The given bar number.</param>
        /// <param name="setOrdinalIndex">if set to <c>true</c> [set ordinal index].</param>
        public void AddCollection(ToneCollection givenTones, int givenBarNumber, bool setOrdinalIndex)
        {
            Contract.Requires(givenTones != null);

            foreach (var tone in givenTones) {
                tone.BarNumber = givenBarNumber;
            }

            this.AddCollection(givenTones, setOrdinalIndex);
        }

        /// <summary>
        /// Add Missing Pauses.
        /// </summary>
        /// <param name="resetBar">if set to <c>true</c> [reset bar].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public ToneCollection Clone(bool resetBar)
        {
            var newTones = new ToneCollection();
            foreach (var newTone in this.Select(tone => (IMusicalTone)tone.CloneTone())) {
                if (resetBar) {
                    newTone.BarNumber = 1;
                }

                newTones.Add(newTone);
            }

            return newTones;
        }

        /// <summary> Return number of bits with non-harmonic tones. </summary>
        /// <returns> Returns value. </returns>
        /// <param name="harmonicStruct">Harmonic structure.</param>
        /// <param name="rhythmicRange">Rhythmic range.</param>
        public byte NumberOfMelodicBits(
                        BinaryStructure harmonicStruct,
                        BitRange rhythmicRange)
        {
            Contract.Requires(harmonicStruct != null);
            Contract.Requires(rhythmicRange != null);
            //// if (harmonicStruct == null) { return 0; }

            if (rhythmicRange == null) {
                return 0;
            }

            var tones = from IMusicalTone mt in this where mt != null && !mt.IsPause select mt as MusicalTone;
            var interRanges = from MusicalTone mt in tones
                              where mt != null && !mt.IsEmpty && !harmonicStruct.IsOn(mt.Pitch.Element)
                              select mt.BitRange into toneRange //// (barNumber)
                              select rhythmicRange.IntersectionWith(toneRange) into interRange
                              where interRange != null
                              select interRange;
            var totalLength = interRanges.Aggregate<BitRange, byte>(0, (current, interRange) => (byte)(current + interRange.Length));
            return totalLength;
        }

        /// <summary>
        /// Add Missing Pauses.
        /// </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public ToneCollection CollectionWithAddedMissingPauses()
        {
            var firstTone = this.FirstOrDefault();
            if (firstTone == null) {
                return null;
            }

            var lastBarNumber = firstTone.BarNumber - 1;
            var rhythmicOrder = firstTone.RhythmicOrder;
            //// Pause for previous bars    
            var newTones = CollectionWithPauses(lastBarNumber, rhythmicOrder);

            //// Pause before the first tone  
            if (firstTone.BitFrom > 0) {
                IMusicalTone pause = MusicalPause.CreatePause(firstTone.RhythmicOrder, 0, firstTone.BitFrom, firstTone.BarNumber);
                pause.InstrumentNumber = firstTone.InstrumentNumber; //// 2019/02
                newTones.Add(pause);
            }

            newTones.Add(firstTone);
            var lastTone = firstTone;

            this.AddMissingInternalPauses(lastTone, newTones);

            return newTones;
        }

        /// <summary>
        /// Standardizes the tones.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        [JetBrains.Annotations.PureAttribute]
        public ToneCollection StandardizeTones(MusicalHeader givenHeader)
        {
            var newTones = new ToneCollection();
            var beat = givenHeader.Metric.MetricBeat;
            if (beat == 0) {
                return null;
            }

            var ground = givenHeader.Metric.MetricGround;
            int rorder = givenHeader.System.RhythmicOrder;
            var wholeNoteTicks = rorder * ground / beat;

            MusicalTone lastMelodicTone = null;
            foreach (var tone in this) {
                //// Include pause to the previous tone.
                if (tone is MusicalTone musicalTone && !musicalTone.IsEmpty) {
                    newTones.Add(musicalTone);
                    lastMelodicTone = musicalTone;
                    continue;
                }

                NoteLength noteLength;
                if (lastMelodicTone != null) {
                    var musicalPause = tone as MusicalPause ?? new MusicalPause(tone.BitRange, tone.BarNumber);

                    noteLength = NoteLength.GetNoteLength(lastMelodicTone.Duration, musicalPause.Duration, wholeNoteTicks);
                    lastMelodicTone.NoteLength = noteLength;
                    if (noteLength != null && noteLength.IncludePause && lastMelodicTone.Pause == null) { //// added lastMelodicTone.Pause == null !?!                       
                        lastMelodicTone.Pause = musicalPause;
                    }
                    else {
                        noteLength = NoteLength.GetNoteLength(musicalPause.Duration, 0, wholeNoteTicks);
                        musicalPause.NoteLength = noteLength;
                        newTones.Add(musicalPause);
                    }
                }
                else {
                    noteLength = NoteLength.GetNoteLength(tone.Duration, 0, wholeNoteTicks);
                    tone.NoteLength = noteLength;
                    newTones.Add(tone);
                }
            }

            return newTones;
        }

        /// <summary>
        /// Determine RhythmicStructure.
        /// </summary>
        /// <param name="rhythmicOrder">Rhythmical order.</param>
        /// <returns> Returns value. </returns>
        public RhythmicStructure DetermineRhythmicStructure(byte rhythmicOrder)
        {
            var tonesGroups = from mt in this group mt by mt.BitFrom;
            var tonesDistinct = new Collection<IMusicalTone>();
            foreach (var toneGroup in tonesGroups) {
                tonesDistinct.Add(toneGroup.First());
            }

            var rstruct = new RhythmicStructure(rhythmicOrder, tonesDistinct);
            rstruct.DetermineBehavior();
            return rstruct;
        }

        /// <summary>
        /// Write tones into MIDI-list.
        /// </summary>
        /// <param name="midiEvents">Midi Event List.</param>
        /// <param name="instrument">Melodic instrument.</param>
        /// <param name="barDivision">Bar division.</param>
        public void WriteTo(MidiEventCollection midiEvents, byte instrument, int barDivision)
        { //// , MidiChannel channel
            ////byte rhyOrder,
            if (midiEvents == null) {
                return;
            }

            foreach (var mt in this) {
                var mtone = mt as MusicalStrike;
                if (mtone == null) {
                    continue;
                }

                mtone.InstrumentNumber = instrument;
                //// mt.Channel = channel;
                mt.WriteTo(midiEvents, barDivision);
            }
        }

        /// <summary>
        /// Export To Midi Events.
        /// </summary>
        /// <param name="events">Midi Events.</param>
        /// <param name="instrument">Midi instrument.</param>
        /// <param name="channel">Midi channel.</param>
        /// <param name="barDivision">Bar division.</param>
        public void ExportToMidiEvents(MidiEventCollection events, byte instrument, MidiChannel channel, int barDivision)
        {
            Contract.Requires(events != null);

            if (events == null) {
                return;
            }

            if (channel != MidiChannel.DrumChannel) {
                events.PutInstrument(0, instrument);
            }

            foreach (var mt in this.Where(mt => mt != null && mt.RhythmicOrder > 0)) {
                var mtone = mt as MusicalStrike;
                if (mtone == null) {
                    continue;
                }

                mtone.InstrumentNumber = instrument;
                //// mt.Channel = channel;
                mt.WriteTo(events, barDivision);
            }
        }

        /// <summary>
        /// Sets the instrument.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="octave">The octave.</param>
        public void SetInstrument(byte instrument, MusicalOctave octave)
        {
            var firstTone = this.FirstOrDefault();
            foreach (var mt in this) {
                //// 2015/01
                // ReSharper disable once PossibleUnintendedReferenceComparison
                var mtone = mt as MusicalStrike;
                if (mtone == null) {
                    continue;
                }

                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (mtone.IsFromPreviousBar && mtone == firstTone) {
                    continue;
                }

                mtone.InstrumentNumber = instrument;
                //// mt.Channel = (MidiChannel)channel;
                if (mtone is MusicalTone melt && !melt.IsEmpty) {
                    melt.Pitch.Octave = (short)octave;
                }
            }
        }
        #endregion

        #region String representation
        /// <summary>
        /// String with tones.
        /// </summary>
        /// <returns>Returns value.</returns>
        public string TonesToString()
        {
            var s = new StringBuilder();
            var actualBarNumber = 0;
            foreach (var mt in this.Where(mt => mt != null)) {
                if (mt.BarNumber != actualBarNumber) {
                    s.Append("|| "); ////‡↕#∫$
                    actualBarNumber = mt.BarNumber;
                }

                s.Append(mt);
            }

            return s.ToString();
        }

        /// <summary>
        /// Rhythmic tones to string.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string RhythmicTonesToString()
        {
            var s = new StringBuilder();
            var actualBarNumber = 0;
            foreach (var mt in this.Where(mt => mt != null)) {
                if (mt.BarNumber != actualBarNumber) {
                    s.Append("|| "); ////‡↕#∫$
                    actualBarNumber = mt.BarNumber;
                }

                if (mt is MusicalStrike mx) {
                    s.Append(mx.RhythmicToString());
                }
            }

            return s.ToString();
        }

        /// <summary>
        /// Returns the tooltip.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string TooltipString()
        {
            var s = new StringBuilder();
            this.ForAll(musicalStrike => s.AppendLine(musicalStrike.ToString()));
            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            this.ForAll(musicalStrike => s.Append(musicalStrike));

            return s.ToString();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Collections the with pauses.
        /// </summary>
        /// <param name="lastBarNumber">The last bar number.</param>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns> Returns value. </returns>
        private static ToneCollection CollectionWithPauses(int lastBarNumber, byte rhythmicOrder)
        {
            var newTones = new ToneCollection();
            for (var barNumber = 1; barNumber <= lastBarNumber; barNumber++) {
                IMusicalTone pause = MusicalPause.CreatePause(rhythmicOrder, 0, rhythmicOrder, barNumber);
                newTones.Add(pause);
            }

            return newTones;
        }

        /// <summary>
        /// Adds the missing internal pauses.
        /// </summary>
        /// <param name="lastTone">The last tone.</param>
        /// <param name="newTones">The new tones.</param>
        private void AddMissingInternalPauses(IMusicalTone lastTone, ToneCollection newTones)
        {
            foreach (var tone in this) {
                //// Pause between the last tone and tone
                int bitFrom;
                int duration;
                if (lastTone.BarNumber == tone.BarNumber) {
                    bitFrom = lastTone.BitFrom + lastTone.Duration;
                    duration = tone.BitFrom - bitFrom;
                    //// tone.BitPosition - lastTone.BitPosition;
                    //// 2016/08 - in the property Pause is saved short pause after tone (staccato,...)
                    var mtone = lastTone as MusicalTone;
                    if (duration > 0 && (mtone?.Pause == null)) {
                        IMusicalTone pause = MusicalPause.CreatePause(lastTone.RhythmicOrder, (byte)bitFrom, (byte)duration, lastTone.BarNumber);
                        pause.InstrumentNumber = lastTone.InstrumentNumber; //// 2019/02
                        newTones.Add(pause);
                    }
                }
                else {
                    if (lastTone.BarNumber < tone.BarNumber) {
                        //// Pause after the last tone of previous bar in the previous bar
                        bitFrom = lastTone.BitFrom + lastTone.Duration;
                        duration = lastTone.RhythmicOrder - bitFrom;
                        if (duration > 0) {
                            IMusicalTone pause = MusicalPause.CreatePause(lastTone.RhythmicOrder, (byte)bitFrom, (byte)duration, lastTone.BarNumber);
                            pause.InstrumentNumber = lastTone.InstrumentNumber; //// 2019/02
                            newTones.Add(pause);
                        }

                        //// 2016/08
                        if (tone.BitFrom > 0) {
                            bitFrom = 0;
                            duration = tone.BitFrom;
                            IMusicalTone pause = MusicalPause.CreatePause(lastTone.RhythmicOrder, (byte)bitFrom, (byte)duration, lastTone.BarNumber);
                            pause.InstrumentNumber = lastTone.InstrumentNumber; //// 2019/02
                            newTones.Add(pause);
                        }
                    }
                }

                //// ReSharper disable once PossibleUnintendedReferenceComparison
                //// tone != lastTone)
                if (!object.ReferenceEquals(tone, lastTone)) {
                    newTones.Add(tone);
                }

                lastTone = tone;
            }
        }
        #endregion
    }
}
