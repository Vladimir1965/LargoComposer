// <copyright file="LocalizedMusic.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Localization {
    /// <summary>
    /// Localized strings of LargoMusic library.
    /// </summary>
    public static class LocalizedMusic {
        /// <summary>
        /// Gets Space.
        /// </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [Pure]
        public static string Space => " ";

        /// <summary>
        /// Localized String.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns> Returns value. </returns>
        [Pure]
        public static string String(string value) {
            var s = BaseMusic.ResourceManager.GetString(value); //// BaseMusic
            return string.IsNullOrEmpty(s) ? value : s;
        }
    }
}
