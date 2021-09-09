// <copyright file="ExtendStrings.cs" company="J.K.R.">
// Copyright (c) 2012 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Provides some essential extension methods for the string class.
    /// </summary>
    public static class ExtendStrings {
        //// FULL DISCLOSURE - I found this method on StackOverflow, but I neglected to make 
        //// note of the person that posted it. I only changed it to match my formatting style.

        #region Strings
        /// <summary>
        /// Right givenNumber characters of given string.
        /// </summary>
        /// <param name="value">Given Value.</param>
        /// <param name="givenNumber">Given Number.</param>
        /// <returns>Returns string value.</returns>
        [Pure]
        public static string Right(this string value, int givenNumber) {
            if (givenNumber <= 0 || value == null) {
                return string.Empty;
            }

            return givenNumber >= value.Length ? value : value.Substring(value.Length - givenNumber);
        }

        /// <summary>
        /// Lefts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="givenNumber">The given number.</param>
        /// <returns>Returns string value.</returns>
        public static string Left(this string value, int givenNumber) {
            if (givenNumber <= 0 || value == null) {
                return string.Empty;
            }

            return givenNumber >= value.Length ? value : value.Substring(0, givenNumber);
        }

        /// <summary>
        /// Clear Special Chars.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns> Returns value. </returns>
        [Pure]
        public static string ClearSpecialChars(this string value) {
            var sb = new StringBuilder(value);
            sb.Replace("?", string.Empty);
            sb.Replace('(', '_');
            sb.Replace(')', '_');
            sb.Replace('/', '_');
            sb.Replace('*', '_');
            sb.Replace('#', '_');
            sb.Replace("*", "_");
            sb.Replace(".", "_");
            sb.Replace(",", "_");
            sb.Replace("-", "_");
            sb.Replace(" ", string.Empty);
            sb.Replace("__", "_");
            return sb.ToString();
        }
        #endregion
    }
}
