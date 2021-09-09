// <copyright file="CompactMidiElement.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Compact Midi Element.
    /// </summary>
    public class CompactMidiElement
    {
        #region Fields
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiElement" /> class.
        /// </summary>
        /// <param name="givenMidiBar">The given midi bar.</param>
        /// <param name="givenMusicalElement">The given musical element.</param>
        /// <param name="voice">The voice.</param>
        public CompactMidiElement(CompactMidiBar givenMidiBar, MusicalElement givenMusicalElement, IAbstractVoice voice) {
            this.MidiBar = givenMidiBar;
            this.MusicalElement = givenMusicalElement;
            this.MidiEvents = new MidiEventCollection(givenMusicalElement.Line.MainVoice.Channel);
            var line = this.MusicalElement.MusicalLine;
            var instrumentInTones = givenMidiBar.MidiBlock.MusicalBlock.HasInstrumentInTones;

            foreach (var mtone in this.MusicalElement.Tones) {
                //// 2019/02, also pauses have instrument...
                //// if (!mtone.IsPause && mtone.ToneType != MusicalToneType.Empty) { //// var channel = (byte)mtone.Channel;
                //// var mtone = mt as MusicalStrike;  if (mtone == null) { continue; }

                var bitDuration = this.MidiBar.MidiBlock.BitDuration;
                /*  //// channel != lastChannel || //// this.Status.LineType = MusicalLineType.None;
                    //// if (mtone.Channel != MidiChannel.DrumChannel) {
                    //// The decrement -1 at the and of row is important (because of event sorting), see also SortByStartTime below!
                    //// But delta time can not be under zero.  //// 1 +   +  - 1; //// - deltaTimeShift
                    //// test midiEvents.PutInstrument(deltaTime, mtone.Instrument, lastChannel ?? channel); //// }
                    //// lastChannel = channel;  */

                var instrument = this.DetermineInstrument(instrumentInTones, mtone, voice);
                var deltaTime = this.MidiBar.BarDeltaTime + (bitDuration * mtone.BitFrom);
                this.MidiEvents.PutInstrument(deltaTime, instrument);  //// FixedInstrument

                //// 2019/02, also pauses have instrument...
                //// 2020/10, ?! suspicious
                if (mtone.IsPause) {
                    continue;
                } 

                var barDivision = this.MidiBar.MidiBlock.BarDivision;
                var barDuration = this.MidiBar.MidiBlock.BarDuration;
                var deltaTimeShift = 0;

                mtone.WriteTo(this.MidiEvents, barDivision, bitDuration, barDuration, deltaTimeShift);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiElement"/> class.
        /// </summary>
        /// <param name="givenMidiBar">The given midi bar.</param>
        /// <param name="givenMusicalElement">The given musical element.</param>
        /// <param name="staff">The staff.</param>
        public CompactMidiElement(CompactMidiBar givenMidiBar, MusicalElement givenMusicalElement, CompactMidiStaff staff) {
            this.MidiBar = givenMidiBar;
            this.MusicalElement = givenMusicalElement;
            this.MidiEvents = new MidiEventCollection(staff.Channel);

            foreach (var mtone in this.MusicalElement.Tones) {
                var bitDuration = this.MidiBar.MidiBlock.BitDuration;
                var instrument = staff.Voice.Instrument;
                var deltaTime = this.MidiBar.BarDeltaTime + (bitDuration * mtone.BitFrom);
                this.MidiEvents.PutInstrument(deltaTime, instrument.Number);  //// FixedInstrument
                if (mtone.IsPause) {
                    continue;
                }

                var barDivision = this.MidiBar.MidiBlock.BarDivision;
                var barDuration = this.MidiBar.MidiBlock.BarDuration;
                var deltaTimeShift = 0;
                
                if (staff.OrchestraUnit != null && mtone.ToneType == MusicalToneType.Melodic) {
                    var tone = mtone as MusicalTone;
                    tone.Pitch.SetOctave((int)staff.Voice.Octave);                    
                }

                mtone.WriteTo(this.MidiEvents, barDivision, bitDuration, barDuration, deltaTimeShift);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets The musical element
        /// </summary>
        public MusicalElement MusicalElement { get; set; }

        /// <summary>
        /// Gets The midi bar
        /// </summary>
        public CompactMidiBar MidiBar { get; }

        /// <summary>
        /// Gets or sets the midi events.
        /// </summary>
        /// <value>
        /// The midi events.
        /// </value>
        public MidiEventCollection MidiEvents { get; set; }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("MidiElement {0} Events {1} ", this.MusicalElement.Point, this.MidiEvents.Count);

            return s.ToString();
        }
        #endregion

        /// <summary>
        /// Determines the instrument.
        /// </summary>
        /// <param name="instrumentInTones">if set to <c>true</c> [instrument in tones].</param>
        /// <param name="mtone">The musical tone.</param>
        /// <param name="voice">The voice.</param>
        /// <returns> Returns value. </returns>
        private byte DetermineInstrument(bool instrumentInTones, IMusicalTone mtone, IAbstractVoice voice) {
            byte instrumentNumber;
            if (instrumentInTones) {
                var line = this.MusicalElement.MusicalLine;
                if (!line.FirstStatus.Instrument.IsEmpty) { //// FixedInstrument
                    instrumentNumber = line.FirstStatus.Instrument.Number;  //// FixedInstrument
                }
                else {
                    var lastInstrument = line.CurrentInstrument;
                    if (mtone.InstrumentNumber != lastInstrument && mtone.InstrumentNumber != (byte)MidiMelodicInstrument.None) {
                        line.CurrentInstrument = mtone.InstrumentNumber;
                    }

                    instrumentNumber = line.CurrentInstrument;
                }
            }
            else {
                instrumentNumber = voice.Instrument.Number;
            }

            return instrumentNumber;
        }
    }
}