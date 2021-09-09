// <copyright file="LocalizedControls.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Localization {
    /// <summary>
    /// Localized strings of LargoMusic library.
    /// </summary>
    public static class LocalizedControls {
        /// <summary>
        /// Gets Space.
        /// </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static string Space => " ";

        /// <summary>
        /// Localized String.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns> Returns value. </returns>
        public static string String(string value) {
            //// BaseControls.Name
            var s = BaseControls.ResourceManager.GetString(value);
            return string.IsNullOrEmpty(s) ? value : s;
        }

        /// <summary>
        /// Localized String.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static string StringOrDefault(string value) {
            //// BaseControls.Name
            var s = BaseControls.ResourceManager.GetString(value);
            if (string.IsNullOrEmpty(s)) {
                s = value;
            }

            return s;
        }
    }
}
