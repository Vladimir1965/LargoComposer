// <copyright file="MusicalOrchestration.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Orchestra
{
    using Abstract;
    using JetBrains.Annotations;
    using LargoSharedClasses.Models;
    using LargoSharedClasses.Music;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Harmonic Streams Port.
    /// </summary>
    public class MusicalOrchestration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalOrchestration" /> class.
        /// </summary>
        public MusicalOrchestration() {
            this.OrchestraBlocks = new List<OrchestraBlock>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalOrchestration"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public MusicalOrchestration(MusicalBlock givenBlock) {
            this.ObjectName = givenBlock.Header.FullName;
            this.Header = givenBlock.Header;
            this.OrchestraBlocks = new List<OrchestraBlock>();

            var body = givenBlock.Body;
            //// ?!?!?! body.SetHarmonicBasis(this.BlockChanges); //// true
            var simpleChanges = body.ExtractSimpleChanges(MusicalChangeType.Instrument);
            //// var simpleChanges = this.BlockChanges.InstrumentChanges;
            var changedBarNumbers = (from ic in simpleChanges select ic.BarNumber).Distinct().ToList();
            OrchestraBlock lastOrchestraBlock = null;
            var changedBars = from b in body.Bars where changedBarNumbers.Contains(b.BarNumber) select b;

            foreach (var bar in changedBars) {
                if (lastOrchestraBlock != null) {
                    lastOrchestraBlock.BarNumberTo = bar.BarNumber - 1;
                }

                var strip = bar.OrchestraStrip();
                var orchestraBlock = new OrchestraBlock(givenBlock.Header, bar.BarNumber, strip);
                this.OrchestraBlocks.Add(orchestraBlock);
                lastOrchestraBlock = orchestraBlock;
            }

            if (lastOrchestraBlock != null) {
                lastOrchestraBlock.BarNumberTo = givenBlock.Header.NumberOfBars;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>
        /// The name of the set.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        public bool IsValid => this.OrchestraBlocks.Any();

        /// <summary>
        /// Gets or sets the instrumentation of bars.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public IList<OrchestraBlock> OrchestraBlocks { get; set; }

        #endregion

        #region Public static factory methods

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Orchestra {0}", this.ObjectName);

            return s.ToString();
        }
        #endregion

        #region Public methods - Orchestration

        /// <summary>
        /// Gets the orchestra block for.
        /// </summary>
        /// <param name="numberOfMelodicTracks">The number of melodic tracks.</param>
        /// <param name="numberOfRhythmicTracks">The number of rhythmic tracks.</param>
        /// <param name="previousBlock">Orchestra block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public OrchestraBlock GetOrchestraBlockFor(byte numberOfMelodicTracks, byte numberOfRhythmicTracks, OrchestraBlock previousBlock) {
            var trackCount = numberOfMelodicTracks + numberOfRhythmicTracks;
            var blockCount = this.OrchestraBlocks.Count;
            if (blockCount == 0) {
                return null;
            }

            OrchestraBlock optimalBlock = null;
            var maxvalue = 0;
            foreach (var block in this.OrchestraBlocks) {
                var value = 100 - Math.Abs(block.TrackCount - trackCount);
                if (block.TrackCount < trackCount) {
                    value -= 20;
                }

                if (previousBlock != null && block.FileName == previousBlock.FileName) {
                    value -= 10;
                }

                value += MathSupport.RandomNatural(10);

                if (value > maxvalue) {
                    maxvalue = value;
                    optimalBlock = block;
                }
            }

            return optimalBlock;
            ////var idx = MathSupport.RandomNatural(blockCount);
            //// return this.OrchestraBlocks[idx];
        }

        /// <summary>
        /// Gets the orchestra block for - in simple (one block) styles.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public OrchestraBlock GetOrchestraBlockFor(int givenBarNumber) {
            if (!this.OrchestraBlocks.Any()) {
                return null;
            }

            var orchestraBlock = (from ob in this.OrchestraBlocks
                                  where ob.BarNumberFrom <= givenBarNumber && ob.BarNumberTo >= givenBarNumber
                                  select ob).FirstOrDefault();
            return orchestraBlock;
        }

        #endregion
    }
}