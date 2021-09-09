// <copyright file="IntendedTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Tone Packet.
    /// </summary>
    public sealed class IntendedTone {
        #region Public properties
        /// <summary>
        /// Gets or sets the melodic tone.
        /// </summary>
        /// <value>
        /// The melodic tone.
        /// </value>
        [UsedImplicitly]
        public MusicalTone MusicalTone { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { [UsedImplicitly] get; set; }
        #endregion

        #region Public properties - Computed Values
        /// <summary>
        /// Gets or sets the harmonic cover value.
        /// </summary>
        /// <value>
        /// The harmonic cover value.
        /// </value>
        public int HarmonicCoverValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the harmonic value.
        /// </summary>
        /// <value>
        /// The harmonic value.
        /// </value>
        public int HarmonicValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the easy sing value.
        /// </summary>
        /// <value>
        /// The easy sing value.
        /// </value>
        public int EasySingValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the free band value.
        /// </summary>
        /// <value>
        /// The free band value.
        /// </value>
        public int FreeBandValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the impulse collisions value.
        /// </summary>
        /// <value>
        /// The impulse collisions value.
        /// </value>
        public int ImpulseCollisionsValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the melodic collisions value.
        /// </summary>
        /// <value>
        /// The melodic collisions value.
        /// </value>
        public int MelodicCollisionsValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the ambit change value.
        /// </summary>
        /// <value>
        /// The ambit change value.
        /// </value>
        public int AmbitChangeValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the octave value.
        /// </summary>
        /// <value>
        /// The octave value.
        /// </value>
        public int OctaveValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the sequence value.
        /// </summary>
        /// <value>
        /// The sequence value.
        /// </value>
        public int SequenceValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the figural value.
        /// </summary>
        /// <value>
        /// The figural value.
        /// </value>
        public int FiguralValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the variability value.
        /// </summary>
        /// <value>
        /// The variability value.
        /// </value>
        public int VariabilityValue { [UsedImplicitly] get; set; }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value> Property description. </value>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets the bar number.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int BarNumber {
            get {
                Contract.Assume(this.MusicalTone != null);

                return this.MusicalTone.BarNumber;
            }
        }

        /// <summary>
        /// Gets the bit from.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public byte BitFrom {
            get {
                Contract.Requires(this.MusicalTone != null);
                return this.MusicalTone.BitFrom;
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int Duration {
            get {
                Contract.Requires(this.MusicalTone != null);
                return this.MusicalTone.Duration;
            }
        }

        /// <summary>
        /// Gets the note.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string Note {
            get {
                Contract.Requires(this.MusicalTone != null);
                return this.MusicalTone.ToShortString();
            }
        }

        /// <summary>
        /// Gets or sets the total formal value.
        /// </summary>
        /// <value> Property description. </value>
        public float TotalFormalValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the total real value.
        /// </summary>
        /// <value> Property description. </value>
        public float TotalRealValue { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets the total value.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public float TotalValue { get; set; }
        #endregion
    }
}
