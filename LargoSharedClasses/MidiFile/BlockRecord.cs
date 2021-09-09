// <copyright file="BlockRecord.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Block Record.
    /// </summary>
    public struct BlockRecord {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockRecord"/> struct.
        /// </summary>
        /// <param name="givenEventType">Type of the given event.</param>
        /// <param name="givenMetricBeat">The given metric beat.</param>
        /// <param name="givenMetricBase">The given metric base.</param>
        /// <param name="givenTonalityKey">The given tonality key.</param>
        /// <param name="givenTempo">The given tempo.</param>
        public BlockRecord(string givenEventType, byte givenMetricBeat, byte givenMetricBase, TonalityKey givenTonalityKey, int givenTempo) : this() {
            this.EventType = givenEventType;
            this.MetricBeat = givenMetricBeat;
            this.MetricBase = givenMetricBase;
            this.TonalityKey = givenTonalityKey;
            this.Tempo = givenTempo;
        }

        /// <summary>
        /// Gets or sets Current EventType.
        /// </summary>
        /// <value> Property description. </value>
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets Current MetricBeat.
        /// </summary>
        /// <value> Property description. </value>
        public byte MetricBeat { get; set; }

        /// <summary>
        /// Gets or sets Current MetricBase.
        /// </summary>
        /// <value> Property description. </value>
        public byte MetricBase { get; set; }

        /// <summary>
        /// Gets or sets Current TonalityKey.
        /// </summary>
        /// <value> Property description. </value>
        public TonalityKey TonalityKey { get; set; }

        /// <summary>
        /// Gets or sets Current Tempo.
        /// </summary>
        /// <value> Property description. </value>
        public int Tempo { get; set; }

        /// <summary>
        /// Gets the values from.
        /// </summary>
        /// <param name="givenRecord">The given record.</param>
        public void GetValuesFrom(BlockRecord givenRecord) {
            this.MetricBeat = givenRecord.MetricBeat;
            this.MetricBase = givenRecord.MetricBase;
            this.TonalityKey = givenRecord.TonalityKey;
            this.Tempo = givenRecord.Tempo;
        }
    }
}