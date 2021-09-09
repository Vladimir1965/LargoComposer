// <copyright file="PlanFunction.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;

namespace LargoSharedClasses.Abstract
{
    /// <summary> A plan function. </summary>
    public class PlanFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlanFunction" /> class.
        /// </summary>
        /// <param name="givenFunction">The given function.</param>
        /// <param name="givenPeriod">The given period.</param>
        /// <param name="givePhase">The give phase.</param>
        /// <param name="givenBase">The given base.</param>
        /// <param name="givenAmplitude">The given amplitude.</param>
        public PlanFunction(PeriodicFunction givenFunction, int givenPeriod, int givePhase, int givenBase, int givenAmplitude) {
            this.Function = givenFunction;
            this.Period = givenPeriod;
            this.Phase = givePhase;
            this.Base = givenBase;
            this.Amplitude = givenAmplitude;
        }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public PeriodicFunction Function { get; set; }

        /// <summary> Gets or sets the period. </summary>
        /// <value> The period. </value>
        public int Period { get; set; }

        /// <summary> Gets or sets the phase. </summary>
        /// <value> The phase. </value>
        public int Phase { get; set; }

        /// <summary>
        /// Gets or sets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public int Base { get; set; }

        /// <summary>
        /// Gets or sets the amplitude.
        /// </summary>
        /// <value>
        /// The amplitude.
        /// </value>
        public int Amplitude { get; set; }

        /// <summary> Value for bar. </summary>
        /// <param name="barNumber"> The bar number. </param>
        /// <returns> A double. </returns>
        public int ValueForBar(int barNumber) {
            int v = this.Base;
            switch (this.Function) {
                case PeriodicFunction.Sinus: {
                        v += (int)(this.Amplitude * Math.Sin(2 * Math.PI * (barNumber + this.Phase) / this.Period));
                        break;
                    }

                case PeriodicFunction.Cosinus: {
                        v += (int)(this.Amplitude * Math.Cos(2 * Math.PI * (barNumber + this.Phase) / this.Period));
                        break;
                    }

                default: {
                        break;
                    }
            }

            return v;
        }
    }
}
