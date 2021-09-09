// <copyright file="SupportCommon.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Abstract {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using JetBrains.Annotations;

    /// <summary> Class for other utilities.  </summary>
    public static class SupportCommon {
        #region Date Utilities
        /// <summary>
        /// Gets dates the time as file stamp.
        /// </summary>
        /// <value> Property description. </value>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static string DateTimeIdentifier {
            get {
                var now = DateTime.Now;
                return string.Format(CultureInfo.CurrentCulture, "{0:yyMMdd(HHmmss)}", now); //// yyyyMMdd-HHmmss
            }
        }

        /// <summary>
        /// Gets the date time identifier2.
        /// </summary>
        /// <value>
        /// The date time identifier2.
        /// </value>
        [UsedImplicitly]
        public static string DateTimeIdentifier2 {
            get {
                var now = DateTime.Now;
                return string.Format(CultureInfo.CurrentCulture, "{0:yyyy-MM-dd(HHmmss)}", now); //// yyyyMMdd-HHmmss
            }
        }
        #endregion

        #region Enum support
        /// <summary>
        /// Enumeration values.
        /// </summary>
        /// <param name="enumType">Type of enumeration.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static IList<int> GetEnumValues(Type enumType) {
            const int maxEnumValue = 300;
            var list = new List<int>();
            for (var i = 0; i < maxEnumValue; i++) {
                if (Enum.IsDefined(enumType, i)) {
                    list.Add(i);
                }
            }

            return list;
        }
        #endregion

        #region String support
        /// <summary>
        /// Receives string and returns the string with its letters reversed.
        /// </summary>
        /// <param name="givenValue">The given string.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        [UsedImplicitly]
        public static string ReverseString(string givenValue) {
            Contract.Requires(givenValue != null);
            var arr = givenValue.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        #endregion

        #region Hexadecimal numbers
        /// <summary>
        /// Numbers to hex.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        [UsedImplicitly]
        public static string NumberToHex(long number) {  //// ulong is not CLS-compliant
            var hexValue = string.Format(CultureInfo.CurrentCulture, "{0:x}", number);
            return hexValue;
        }

        /// <summary>
        /// Hexes to number.
        /// </summary>
        /// <param name="hexValue">The hex value.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        [UsedImplicitly]
        public static long HexToNumber(string hexValue) {  //// ulong is not CLS-compliant
            var number = (long)Convert.ToUInt64(hexValue, 16);
            //// decimal number = decimal.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            //// decimal number = Convert.ToDecimal(hexValue, 16);
            return number;
        }

        #endregion
    }
}