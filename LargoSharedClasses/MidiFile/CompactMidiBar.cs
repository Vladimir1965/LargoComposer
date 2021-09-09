// <copyright file="CompactMidiBar.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.MidiFile
{
    using LargoSharedClasses.Midi;
    using Localization;
    using Music;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Compact Midi Bar.
    /// </summary>
    public class CompactMidiBar
    {
        #region Fields

        /// <summary>
        /// Midi Events.
        /// </summary>
        private Queue<IMidiEvent> eventQueue;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiBar"/> class.
        /// </summary>
        /// <param name="givenMidiBlock">The given midi block.</param>
        /// <param name="givenMusicalBar">The given musical bar.</param>
        public CompactMidiBar(CompactMidiBlock givenMidiBlock, MusicalBar givenMusicalBar) : this()
        {
            this.MidiBlock = givenMidiBlock;
            this.MusicalBar = givenMusicalBar;
            this.BarDeltaTime = givenMidiBlock.BarDuration * (givenMusicalBar.BarNumber - 1);
            if (this.MusicalBar.TempoNumber > 0 && this.MusicalBar.TempoNumber != this.MidiBlock.CurrentTempoNumber) { 
                var header = this.MidiBlock.MusicalBlock.Header;
                this.MidiBlock.CurrentTempoNumber = this.MusicalBar.TempoNumber;
                this.MidiBlock.MidiTimeToTicksQuotient = MusicalProperties.MidiTimeToTicksQuotient(this.MidiBlock.CurrentTempoNumber, header.Division);
            }

            /* 2021/01
            foreach (var element in this.MusicalBar.Elements)
            {
                var line = element.Line;
                if (this.MidiBlock.MusicalBlock.HasInstrumentInTones) {
                    var midiElement = new CompactMidiElement(this, element, line.MainVoice); 
                    this.MidiElements.Add(midiElement);
                } else { //// Instrument in voices
                    var voices = line.Voices;
                    foreach (var voice in voices) {
                        var midiElement = new CompactMidiElement(this, element, voice);
                        this.MidiElements.Add(midiElement);
                    }
                }
            } */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiBar"/> class.
        /// </summary>
        public CompactMidiBar()
        {
            this.MidiElements = new List<CompactMidiElement>();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets The musical bar
        /// </summary>
        /// <value>
        /// The musical bar.
        /// </value>
        public MusicalBar MusicalBar { get; }

        /// <summary>
        /// Gets or sets The midi block
        /// </summary>
        /// <value>
        /// The midi block.
        /// </value>
        public CompactMidiBlock MidiBlock { get; set; }

        /// <summary>
        /// Gets or sets the bar delta time.
        /// </summary>
        /// <value>
        /// The bar delta time.
        /// </value>
        public long BarDeltaTime { get; set; }

        /// <summary>
        /// Gets or sets the midi elements.
        /// </summary>
        /// <value>
        /// The midi elements.
        /// </value>
        public List<CompactMidiElement> MidiElements { get; set; }

        /// <summary>
        /// Gets or sets the midi events.
        /// </summary>
        /// <value>
        /// The midi events.
        /// </value>
        public MidiEventCollection MidiEvents { get; set; }

        /// <summary>
        /// Gets or sets Queue of Midi Events.
        /// </summary>
        /// <value> Property description. </value>
        public Queue<IMidiEvent> EventQueue
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IMidiEvent>>() != null);
                return this.eventQueue ?? (this.eventQueue = new Queue<IMidiEvent>());
            }

            set => this.eventQueue = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns> Returns value. </returns>
        public CompactMidiElement GetMidiElement(Guid lineIdent) {
            var element = (from elem in this.MidiElements
                           where elem.MusicalElement.Line.LineIdent == lineIdent
                           select elem).FirstOrDefault();
            return element;
        }

        /// <summary>
        /// Collects the midi events.
        /// </summary>
        public void CollectMidiEvents()
        {
            foreach (var element in this.MusicalBar.Elements)
            {
                var line = element.Line;
                if (this.MidiBlock.MusicalBlock.HasInstrumentInTones) {
                    var midiElement = new CompactMidiElement(this, element, line.MainVoice); 
                    this.MidiElements.Add(midiElement);
                } else { //// Instrument in voices
                    var voices = line.Voices;
                    foreach (var voice in voices) {
                        var midiElement = new CompactMidiElement(this, element, voice);
                        this.MidiElements.Add(midiElement);
                    }
                }
            } 

            this.MidiEvents = new MidiEventCollection();

            var header = this.MidiBlock.MusicalBlock.Header;
            if (this.MusicalBar.BarNumber == 1) 
            {
                this.MidiEvents.PutMetre(header.Metric.MetricBeat, header.Metric.MetricBase);
                this.MidiBlock.CurrentTempoNumber = header.Tempo;
                this.MidiEvents.PutTempo(this.BarDeltaTime, this.MidiBlock.CurrentTempoNumber); //// 2020/10 not necessary!?
            }

            //// Tempo
            if (this.MusicalBar.TempoNumber > 0 && this.MusicalBar.TempoNumber != this.MidiBlock.CurrentTempoNumber) {
                this.MidiBlock.CurrentTempoNumber = this.MusicalBar.TempoNumber;
                this.MidiBlock.MidiTimeToTicksQuotient = MusicalProperties.MidiTimeToTicksQuotient(this.MidiBlock.CurrentTempoNumber, header.Division);
                if (this.MidiBlock.CurrentTempoNumber > 0) {
                    this.MidiEvents.PutTempo(this.BarDeltaTime, this.MidiBlock.CurrentTempoNumber);
                }
            } 
            
            foreach (var element in this.MidiElements) {
                if (element == null) { //// || !musicalLine.IsSelected 
                    continue;
                }

                this.MidiEvents.AddRange(element.MidiEvents);
            }

            this.EnqueueMidiEvents();
        }

        /// <summary>
        /// Enqueues the midi events.
        /// </summary>
        public void EnqueueMidiEvents() {
            if (this.MidiEvents == null) {
                return;
            }

            this.MidiEvents.SortByStartTime();
            this.MidiEvents.RecomputeDeltaTimes();
            foreach (var midiEvent in this.MidiEvents) {
                this.EventQueue.Enqueue(midiEvent);
            }
        }

        /// <summary>
        /// Gets the tempo event.
        /// </summary>
        /// <returns> Returns value. </returns>
        public MetaTempo GetTempoEvent()
        {
            MetaTempo tempoEvent = null;
            var header = this.MidiBlock.MusicalBlock.Header;
            if (this.MusicalBar.BarNumber == 1) {
                this.MidiBlock.CurrentTempoNumber = header.Tempo;
                var tv = MetaTempo.MidiTempoBaseNumber / this.MidiBlock.CurrentTempoNumber;    //// microseconds per metronome clicks 0
                tempoEvent = new MetaTempo(this.BarDeltaTime, tv);
            }

            bool writeTempo = this.MusicalBar.TempoNumber > 0 && this.MusicalBar.TempoNumber != this.MidiBlock.CurrentTempoNumber;
            if (writeTempo) {
                this.MidiBlock.CurrentTempoNumber = this.MusicalBar.TempoNumber;
                this.MidiBlock.MidiTimeToTicksQuotient = MusicalProperties.MidiTimeToTicksQuotient(this.MidiBlock.CurrentTempoNumber, header.Division);
                //// this.MidiEvents.PutTempo(this.MidiBlock.CurrentTempoNumber);
                if (this.MidiBlock.CurrentTempoNumber > 0) {
                    var tv = MetaTempo.MidiTempoBaseNumber / this.MidiBlock.CurrentTempoNumber;    //// microseconds per metronome clicks 0
                    tempoEvent = new MetaTempo(this.BarDeltaTime, tv);
                }
            }

            return tempoEvent;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("MidiBar B{0} T{1}", this.MusicalBar.BarNumber, this.BarDeltaTime);

            return s.ToString();
        }
        #endregion
    }
}
