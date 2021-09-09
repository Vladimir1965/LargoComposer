// <copyright file="DataEnumsLocalization.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using Abstract;
    using Localization;

    /// <summary>
    /// Data enumerations.
    /// </summary>
    public static class DataEnumsLocalization {
        #region Fields
        /// <summary>Used for synchronization of all dictionary-related operations.</summary>
        private static readonly object DictLock = new object();

        /// <summary> Private Dictionary of enumerations. </summary>
        private static Dictionary<string, object> dictionary;
        #endregion

        #region Enumerations
        /// <summary>
        /// List of values from given enumeration type. 
        /// </summary>
        /// <param name="enumType">Type of enumeration.</param>
        /// <param name="localizedPrefix">Localized prefix.</param>
        /// <param name="includingZero">Including Zero.</param>
        /// <returns> Returns value. </returns>
        public static Collection<KeyValuePair> ListEnum(Type enumType, string localizedPrefix, bool includingZero) {
            Contract.Requires(enumType != null);
            Collection<KeyValuePair> obj;
            if (enumType == null) {
                return null;
            }

            lock (DictLock) {
                if (dictionary == null) {
                    dictionary = new Dictionary<string, object>();
                }

                var key = enumType.ToString();
                //// if (dictionary.ContainsKey(key)) {  obj = (Collection<KeyValuePair>)dictionary[key];  } 
                //// if (obj == null) {
                obj = new Collection<KeyValuePair>();
                var items = SupportCommon.GetEnumValues(enumType);
                var firstNumber = includingZero ? 0 : 1;
                for (var number = firstNumber; number < items.Count; number++) { //// 0
                    var i = items[number];
                    var si = !string.IsNullOrEmpty(localizedPrefix) ? 
                        LocalizedMusic.String(localizedPrefix + i.ToString("D", CultureInfo.CurrentCulture.NumberFormat)) 
                        : i.ToString("D", CultureInfo.CurrentCulture.NumberFormat);

                    obj.Add(new KeyValuePair(i, si));
                }

                dictionary[key] = obj;
                //// }
            }

            return obj;
        }

        /// <summary>
        /// Lists the limited enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="localizedPrefix">The localized prefix.</param>
        /// <param name="lowestValue">The lowest value.</param>
        /// <param name="highestValue">The highest value.</param>
        /// <returns> Returns value.</returns>
        public static Collection<KeyValuePair> ListLimitedEnum(Type enumType, string localizedPrefix, int lowestValue, int highestValue) {
            Contract.Requires(enumType != null);
            Collection<KeyValuePair> obj;
            if (enumType == null) {
                return null;
            }

            lock (DictLock) {
                if (dictionary == null) {
                    dictionary = new Dictionary<string, object>();
                }

                var key = enumType.ToString();
                //// if (dictionary.ContainsKey(key)) {  obj = (Collection<KeyValuePair>)dictionary[key];  } 
                //// if (obj == null) {
                obj = new Collection<KeyValuePair>();
                var items = SupportCommon.GetEnumValues(enumType);
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var number = 0; number < items.Count; number++) { //// 0
                    var i = items[number];
                    if (i < lowestValue || i > highestValue) {
                        continue;
                    }

                    var si = !string.IsNullOrEmpty(localizedPrefix) ?
                        LocalizedMusic.String(localizedPrefix + i.ToString("D", CultureInfo.CurrentCulture.NumberFormat))
                        : i.ToString("D", CultureInfo.CurrentCulture.NumberFormat);

                    obj.Add(new KeyValuePair(i, si));
                }

                dictionary[key] = obj;
                //// }
            }

            return obj;
        }

        /// <summary>
        /// List of values from given enumeration type.
        /// </summary>
        /// <param name="enumType">Type of enumeration.</param>
        /// <param name="localizedPrefix">Localized prefix.</param>
        /// <param name="includingZero">Including Zero.</param>
        /// <returns> Returns value. </returns>
        [Pure]
        public static IEnumerable<KeyValuePair> ListEnumSortedByText(Type enumType, string localizedPrefix, bool includingZero) {
            Contract.Requires(enumType != null);
            var coll = ListEnum(enumType, localizedPrefix, includingZero);
            var orderedColl = (from c in coll orderby c.Value select c).ToList();
            return new Collection<KeyValuePair>(orderedColl);
        }

        /// <summary>
        /// Reverse list of values from given enumeration type.
        /// </summary>
        /// <param name="enumType">Type of enumeration.</param>
        /// <param name="localizedPrefix">Localized prefix.</param>
        /// <param name="includingZero">Including Zero.</param>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> ReverseListEnum(Type enumType, string localizedPrefix, bool includingZero) {
            Contract.Requires(enumType != null);
            Collection<KeyValuePair> obj = null;
            if (enumType == null) {
                return null;
            }

            lock (DictLock) {
                if (dictionary == null) {
                    dictionary = new Dictionary<string, object>();
                }

                var key = enumType.ToString();
                if (dictionary.ContainsKey(key)) {
                    obj = (Collection<KeyValuePair>)dictionary[key];
                }

                if (obj != null) {
                    return obj;
                }

                //// // // Object page = null;
                obj = new Collection<KeyValuePair>();
                var items = SupportCommon.GetEnumValues(enumType);
                var firstNumber = includingZero ? 0 : 1;
                for (var number = items.Count - 1; number >= firstNumber; number--) {
                    var i = items[number];
                    var si = !string.IsNullOrEmpty(localizedPrefix) ? LocalizedMusic.String(localizedPrefix + i.ToString("D", CultureInfo.CurrentCulture.NumberFormat)) : i.ToString("D", CultureInfo.CurrentCulture.NumberFormat);

                    obj.Add(new KeyValuePair(i, si));
                }

                dictionary[key] = obj;
            }

            return obj;
        }
        #endregion
    }
}
