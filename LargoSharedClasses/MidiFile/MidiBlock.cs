// <copyright file="MidiBlock.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to represent one MIDI block.

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Midi Block.
    /// </summary>
    public sealed class MidiBlock : IMidiBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiBlock" /> class.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="tonalityKey">The tonality key.</param>
        /// <param name="tempo">The tempo.</param>
        public MidiBlock(int barNumber, MusicalHeader givenHeader, TonalityKey tonalityKey, int tempo) {
            this.Area = new MusicalSection(barNumber, DefaultValue.MaximumBarNumber, null);
            this.Header = givenHeader;
            this.TonalityKey = tonalityKey;
            this.Tempo = tempo;
            this.TonalityGenus = TonalityGenus.Major;
            //// this.CheckTempo();
            this.GuessName();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public CompactMidiStrip Sequence { get; set; }

        /// <summary>
        /// Gets Musical Area.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalSection Area { get; }

        /// <summary>
        /// Gets or sets the midi time from.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public long MidiTimeFrom { get; set; }

        /// <summary>
        /// Gets or sets the midi time to.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public long MidiTimeTo { get; set; }

        /// <summary>Gets the rhythmical order.</summary>
        /// <value> Property description. </value>
        public int Tempo { get; private set; }

        /// <summary>
        /// Gets the tonality key.
        /// </summary>
        /// <value>
        /// The tonality key.
        /// </value>
        public TonalityKey TonalityKey { get; }

        /// <summary>
        /// Gets the tonality genus.
        /// </summary>
        /// <value>
        /// The tonality genus.
        /// </value>
        public TonalityGenus TonalityGenus { get; }
        #endregion

        /// <summary>
        /// Checks the tempo.
        /// </summary>
        public void CheckTempo() {
            if (this.Tempo != 0) {
                return;
            }

            this.Tempo = DefaultValue.DefaultTempo;
            //// MessageBox.Show("Default tempo assigned!");
        }

        /// <summary>
        /// Guesses the name.
        /// </summary>
        private void GuessName() {
            this.Header.Specification = MusicalProperties.GetTempoValue(this.Tempo);
            if (this.TonalityKey == TonalityKey.None) {
                return;
            }

            var t = this.TonalityKey.ToString().Replace('s', '#');
            t = t.Replace("Tonality", string.Empty);
            this.Header.Specification += " in " + t;
        }
    }
}
