// <copyright file="NoteLength.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Notation
{
    /// <summary>
    /// Note Length.
    /// </summary>
    public class NoteLength {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteLength"/> class.
        /// </summary>
        /// <param name="givenTupleNumber">The given tuple number.</param>
        /// <param name="givenDenominator">The given denominator.</param>
        /// <param name="givenNumberOfDots">The given number of dots.</param>
        /// <param name="givenTuplePosition">The given tuple position.</param>
        public NoteLength(TupleNumber givenTupleNumber, MusicalDenominator givenDenominator, int givenNumberOfDots, TuplePosition givenTuplePosition) {
            this.TupleNumber = givenTupleNumber;
            this.Denominator = givenDenominator;
            this.NumberOfDots = givenNumberOfDots;
            this.TuplePosition = givenTuplePosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteLength"/> class.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <param name="wholeNoteTicks">The whole note ticks.</param>
        public NoteLength(int ticks, int wholeNoteTicks) {
            this.TupleNumber = TupleNumber.Single;
            this.NumberOfDots = 0;
            if (ticks == 0) {
                return;
            }

            //// var smallestLength = (float)wholeNoteTicks / 128;
            //// var denominator = 1;
            double baseTicks;
            if (ticks > wholeNoteTicks) {
                this.Denominator = MusicalDenominator.Whole;
                //// baseTicks = wholeNoteTicks;
                return;
            }
            else {
                var s = wholeNoteTicks / ticks;
                var r = Math.Log(s) / Math.Log(2);
                var cr = Math.Ceiling(r);
                if (cr > 7) {
                    cr = 7;
                }

                double dr = Math.Pow(2, cr);
                this.Denominator = (MusicalDenominator)dr;
                baseTicks = Math.Round(wholeNoteTicks / dr);
            }

            var q = ticks / baseTicks;
            if (q > 1.4) {
                this.NumberOfDots = 1;
            }

            if (q > 1.7) {
                this.NumberOfDots = 2;
            }

            //// Temporarily - the aim is not register very short pauses ...  
            if (this.Denominator == MusicalDenominator.D128Th) {
                this.Denominator = MusicalDenominator.None;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Denominator.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalDenominator Denominator { get; }

        /// <summary>
        /// Gets the number of dots.
        /// </summary>
        /// <value>
        /// The number of dots.
        /// </value>
        public int NumberOfDots { get;  }

        /// <summary>
        /// Gets the tuple.
        /// </summary>
        /// <value>
        /// The tuple.
        /// </value>
        public TuplePosition TuplePosition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [include pause]. Staccato...
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include pause]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludePause { get; set; }

        /// <summary>
        /// Gets the tuple.
        /// </summary>
        /// <value>
        /// The tuple.
        /// </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private TupleNumber TupleNumber { [UsedImplicitly] get;  }

        #endregion

        #region Factory methods
        /// <summary>
        /// Gets the length of the note.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <param name="pauseTicks">The pause ticks.</param>
        /// <param name="wholeNoteTicks">The whole note ticks.</param>
        /// <returns> Returns value. </returns>
        public static NoteLength GetNoteLength(int ticks, int pauseTicks, int wholeNoteTicks) {
            //// if (ticks == 53 && pauseTicks == 120) { pauseTicks++; pauseTicks--;  } 

            if (pauseTicks == 0) {
                return new NoteLength(ticks, wholeNoteTicks);
            }

            var pauseLength = new NoteLength(pauseTicks, wholeNoteTicks);
            bool auxiliaryPause = (pauseLength.Denominator == MusicalDenominator.None)
                                    || ((pauseLength.Denominator >= MusicalDenominator.Sixteenth) && pauseTicks < 1.3 * ticks);

            NoteLength nl;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (auxiliaryPause) {
                nl = new NoteLength(ticks + pauseTicks, wholeNoteTicks) { IncludePause = true };
            }
            else {
                //// if (ticks == 1) { pauseTicks++; pauseTicks--; }
                nl = new NoteLength(ticks, wholeNoteTicks) { IncludePause = false };
            }

            return nl;
        }

        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Denominator);
            sb.AppendFormat(" {0} dots ", this.NumberOfDots);
            sb.Append(this.TuplePosition);
            return sb.ToString();
        }
        #endregion
    }
}
