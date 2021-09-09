// <copyright file="MathSupport.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract {
    /// <summary> Mathematical utilities. </summary>
    /// was static
    public static class MathSupport {
        #region Private properties - Random values
        /// <summary>
        /// Gets System.GUID. System.GUID has a very low probability of being duplicated 
        /// and provides more unique seed values. 
        /// </summary>
        /// <value> General musical property.</value>
        private static int GuidSeed {
            get {
                const byte seedLength = 5;
                var guid = Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture);
                guid = guid.Replace("a", string.Empty).Replace("b", string.Empty).Replace("c", string.Empty)
                            .Replace("d", string.Empty).Replace("e", string.Empty).Replace("f", string.Empty);
                var seed = guid.Length > seedLength ? int.Parse(guid.Substring(0, seedLength), CultureInfo.CurrentCulture.NumberFormat) : 0;

                return seed;
            }
        }

        /// <summary> Gets or sets object for random numbers. </summary>
        /// <value> Property description. </value>
        private static Random RandObj { get; set; }
        #endregion

        #region Public static methods - Equality
        /// <summary>
        /// Equality test of the given inaccurate numbers by difference.
        /// </summary>
        /// <param name="number0">Given value 0.</param>
        /// <param name="number1">Given value 1.</param>
        /// <param name="givenDelta">Given Delta.</param>
        /// <returns> Returns value. </returns>
        public static bool EqualNumbers(float number0, float number1, float givenDelta) {
            return Math.Abs(number0 - number1) <= givenDelta;
        }

        /// <summary>
        /// Equality test of the given inaccurate numbers by ratio.
        /// </summary>
        /// <param name="number0">Given value 0.</param>
        /// <param name="number1">Given value 1.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public static bool EqualNumbersRational(float number0, float number1, float limit) {
            //// bool b0 = Math.Abs(aR0) < limit; 
            //// bool b1 = Math.Abs(aR1) < limit;
            if (float.IsNaN(number0) || float.IsNaN(number1)) {
                return false;
            }

            //// int s0 = Math.Sign(number0);
            //// int s1 = Math.Sign(number1);
            //// if (s0 != s1) {  return false;  }

            if (number0 < 0) {
                number0 = -number0;
            }

            if (number1 < 0) {
                number1 = -number1;
            }

            float min, max;
            if (number0 < number1) {
                min = number0;
                max = number1;
            }
            else {
                min = number1;
                max = number0;
            }
            //// float min = Math.Min(number0, number1);
            //// float max = Math.Max(number0, number1);
            const float afterZero = 0.0001f;
            const float largeNumber = 10000f;
            var ratio = min >= afterZero && min <= largeNumber ? max / min : 0;

            return ratio <= limit;
        }
        #endregion

        #region Public static methods - Algebra
        /// <summary>
        /// Greatest common divisor.
        /// </summary>
        /// <param name="value1">Given value1.</param>
        /// <param name="value2">Given value2.</param>
        /// <returns>Returns value.</returns>
        public static decimal GreatestCommonDivisor(decimal value1, decimal value2) {
            if (value1 == 0 || value2 == 0) {
                return 0;
            }

            var n1 = Math.Max(value1, value2);
            var n2 = Math.Min(value1, value2);
            decimal rest = -1;
            while (rest != 0) {
                rest = n1 % n2;
                n1 = n2;
                n2 = rest;
            }

            return n1;
        }

        /// <summary> Least common multiple. </summary>
        /// <param name="number1">First number.</param>
        /// <param name="number2">Second number.</param>
        /// <returns> Returns value. </returns>
        public static decimal LeastCommonMultiple(decimal number1, decimal number2) {
            var result = GreatestCommonDivisor(number1, number2);
            result = (number1 * number2) / result;
            return result;
        }

        #endregion

        #region Public static methods - Random values
        /// <summary>
        /// Returns random correction, aRandomEffect 0-1.
        /// </summary>
        /// <param name="randomEffect">Random Effect.</param>
        /// <returns> Returns value. </returns>
        public static float RandomCorrection(float randomEffect) {
            // ReSharper disable once InvertIf
            if (RandObj == null) {
                var theSeed = GuidSeed;
                //// int theSeed = (int)DateTime.Now.Ticks; //// or DateTime.Now.Second
                RandObj = new Random(theSeed);
            }

            return (float)(randomEffect * RandObj.NextDouble()); // /100     
        }

        /// <summary>
        /// Returns random correction, aRandomEffect 0-1. 
        /// </summary>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public static int RandomNatural(int limit) {
            // ReSharper disable once InvertIf
            if (RandObj == null) {
                var theSeed = GuidSeed;
                //// int theSeed = (int)DateTime.Now.Ticks; 
                RandObj = new Random(theSeed);
            }

            var x = (limit - 0.001) * RandObj.NextDouble();
            return (int)Math.Floor(x); //// Round
        }

        /// <summary>
        /// Random natural round.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static int RandomNaturalRound(int limit) {
            // ReSharper disable once InvertIf
            if (RandObj == null) {
                var theSeed = GuidSeed;
                //// int theSeed = (int)DateTime.Now.Ticks; 
                RandObj = new Random(theSeed);
            }

            var x = limit * RandObj.NextDouble();
            return (int)Math.Round(x); //// Round
        }

        #endregion
    }
}
