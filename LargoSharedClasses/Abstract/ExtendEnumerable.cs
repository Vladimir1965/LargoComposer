// <copyright file="ExtendEnumerable.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Abstract {
    /// <summary> Static Extensions. </summary>
    /// <remarks> Extensions of library classes.
    /// </remarks> 
    public static class ExtendEnumerable {
        #region Enumerables
        /// <summary>
        /// ForAll. usage:  foo.ForAll((item) => Console.WriteLine(item.ToString(CultureInfo.CurrentCulture))).
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="enumerable">Enumerable collection.</param>
        /// <param name="action">Abstract action.</param>
        public static void ForAll<T>(this IEnumerable<T> enumerable, Action<T> action) {
            Contract.Requires(enumerable != null);
            Contract.Requires(action != null);
            if (action == null || enumerable == null) {
                return;
            }

            foreach (var item in enumerable) {
                action(item);
            }
        }
        #endregion
    }
}
