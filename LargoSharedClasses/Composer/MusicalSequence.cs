// <copyright file="MusicalSequence.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Text;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Composer
{
    /// <summary>
    /// MusicPiece Sequence.
    /// </summary>
    public class MusicalSequence {
        #region Fields
        /// <summary>
        /// Melodic Variety.
        /// </summary>
        private readonly MusicalVariety melodicVariety;

        /// <summary>
        /// Melodic Shape.
        /// </summary>
        private readonly MelodicShape melodicShape;

        /// <summary>
        /// Harmonic System.
        /// </summary>
        private HarmonicSystem harmonicSystem;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalSequence" /> class.
        /// </summary>
        /// <param name="givenVariety">The given variety.</param>
        public MusicalSequence(MusicalVariety givenVariety) {
            //// Warning 4 CodeContracts: Member 'LargoObjectMusic.Engine.MelodicSequence.melodicVariety' has less visibility than the enclosing method 
            //// 'LargoObjectMusic.Engine.MelodicSequence.SequenceValue'
            Contract.Requires(givenVariety != null);

            this.melodicVariety = givenVariety;
            this.melodicShape = givenVariety.LineRules.MelodicShape;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("MusicalSequence {0} {1}", this.melodicShape, this.melodicVariety);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sequences the value.
        /// </summary>
        /// <returns> Returns value. </returns>
        public int SequenceValue() {
            //// Warning 5 CodeContracts: Member 'LargoObjectMusic.Engine.MelodicSequence.melodicVariety' 
            //// has less visibility than the enclosing method 'LargoObjectMusic.Engine.MelodicSequence.SequenceValue'.
            Contract.Requires(this.melodicVariety.Element.Status != null);
            Contract.Requires(this.melodicVariety.Element.Line != null);

            var element = this.melodicVariety.Element;
            var line = (MusicalLine)element.Line;
            if (line.LastTone == null || line.PenultTone == null) { //// || mte.CurrentTone == null
                return MusicalQuantity.NeutralValue;
            }

            this.harmonicSystem = this.melodicVariety.Element.Bar.Header.System.HarmonicSystem; ////  ?? mte.CurrentTone.Pitch.HarmonicSystem;

            var value = 0;
            //// Melodic shape rules
            switch (this.melodicShape) {
                case MelodicShape.MinimumMotion: {
                        value = this.MinimumMotionValue();
                        break;
                    }

                case MelodicShape.Scales: {
                        value = this.ScalesValue();
                        break;
                    }

                case MelodicShape.Sinusoids: {
                        value = this.SinusoidsValue();
                        break;
                    }

                case MelodicShape.BreakingLine: {
                        value = this.BreakingLineValue();
                        break;
                    }

                case MelodicShape.Original:
                    break;

                case MelodicShape.None:
                    break;
            }

            return value;
        }
        #endregion

        #region Private methods - Part related values
        /// <summary> Property that is higher for smaller changes of the musical pitch. </summary>
        /// <returns> Returns value. </returns>
        private int MinimumMotionValue() {
            var mte = this.melodicVariety.Element;

            if (!this.melodicVariety.Element.Status.IsFilling) {
                return MusicalQuantity.NeutralValue;
            }

            if (mte.Status.CurrentMelInterval == null) {
                return MusicalQuantity.NeutralValue;
            }

            var val = mte.Status.CurrentMelInterval.MinimumMotion;
            if (val < DefaultValue.HalfUnit) {
                //// 2008/12 VeryNiceValue caused tone duplicates in parallel voices
                return (int)((float)MusicalQuantity.VeryNiceValue / 2 * (1 - val));  
            } //// 1.0-r

            return MusicalQuantity.NeutralValue; // PoorValue;
        }

        /// <summary> Value of breaking line property. </summary>
        /// <returns> Returns value. </returns>
        private int BreakingLineValue() {
            var element = this.melodicVariety.Element;
            var line = (MusicalLine)element.Line;

            var p1 = line.PenultTone.Pitch;
            var p2 = line.LastTone.Pitch;
            if (p1 == null || p2 == null) {
                return MusicalQuantity.NeutralValue; 
            }

            //// not more than 2 intervals (after jumping the best move in the opposite direction)
            var i1 = line.LastTone.Pitch.IntervalFrom(p1);
            var i2 = line.CurrentTone.Pitch.IntervalFrom(p2);

            var fi1 = this.harmonicSystem.FormalMedianLength(i1);
            var fi2 = this.harmonicSystem.FormalMedianLength(i2);
            if (fi1 == 0 && fi2 == 0) { //// if (fi2 == 0) {
                return MusicalQuantity.PoorValue;
            }

            //// int sum = i1 + i2;
            //// sum = sum > -short.MinValue ? Math.Abs(sum) : 0;

            int result;
            if (i1 >= this.harmonicSystem.Median) {
                result = i2 >= this.harmonicSystem.Median ? MusicalQuantity.PoorValue
                    : Math.Sign(i1) == Math.Sign(i2) ? MusicalQuantity.GoodValue : MusicalQuantity.NiceValue;
            }
            else {
                result = Math.Sign(i1) == Math.Sign(i2) ? MusicalQuantity.NiceValue : MusicalQuantity.GoodValue;
            }

            return result;
        }

        /// <summary>
        /// Value of scales.
        /// </summary>
        /// <returns> Returns value. </returns>
        private int ScalesValue() {
            var element = this.melodicVariety.Element;
            var line = (MusicalLine)element.Line;

            if (line.LastTone?.Pitch == null) {
                return MusicalQuantity.NeutralValue;
            }

            //// small intervals in the same direction favored 
            var i1 = line.LastTone.Pitch.IntervalFrom(line.PenultTone.Pitch);
            var i2 = line.CurrentTone.Pitch.IntervalFrom(line.LastTone.Pitch);

            if (2 * Math.Abs(i2) > this.harmonicSystem.Median) {
                return MusicalQuantity.PoorValue;
            }

            var fi1 = this.harmonicSystem.FormalMedianLength(i1);
            var fi2 = this.harmonicSystem.FormalMedianLength(i2);
            if (fi1 == 0 && fi2 == 0) { //// if (fi2 == 0) {
                return MusicalQuantity.PoorValue;
            }

            if (2 * Math.Abs(i1) <= this.harmonicSystem.Median && Math.Sign(i1) == Math.Sign(i2)) {
                return MusicalQuantity.VeryNiceValue;
            }

            return MusicalQuantity.NeutralValue;
        }

        /// <summary>
        /// Value of sinusoids.
        /// </summary>
        /// <returns> Returns value. </returns>
        private int SinusoidsValue() {
            var element = this.melodicVariety.Element;
            var line = (MusicalLine)element.Line;
            ////  if (this.harmonicSystem == null) {  return MusicalQuantity.NeutralValue; } 
            //// tones at greater interval distance with regard to the tones in previous bars are favored.
            var tone1 = element.PreviousBarFirstTone;
            var tone2 = line.CurrentTone;
            if (tone1 == null || tone2 == null) {
                return MusicalQuantity.NeutralValue;
            }

            var d1 = tone2.Pitch.DistanceFrom(tone1.Pitch);
            return d1 > this.harmonicSystem.Median ? MusicalQuantity.NiceValue : MusicalQuantity.NeutralValue;
        }
        #endregion
    }
}
