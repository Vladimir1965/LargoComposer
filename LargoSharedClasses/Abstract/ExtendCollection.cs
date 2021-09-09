// <copyright file="ExtendCollection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Abstract
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    
    /// <summary>
    /// Collection Extender (static 11/2010).
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    [UsedImplicitly]
    public static class ExtendCollection<T> where T : new() {
        #region Public static methods
        /// <summary>
        /// Get Random object.
        /// </summary>
        /// <param name="collection">Collection of objects.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        [System.Diagnostics.Contracts.Pure]
        public static T GetRandomObject(Collection<T> collection) {
            Contract.Requires(collection != null);
            if (collection == null) {
                return default(T);
            }

            if (!collection.Any()) {
                return default(T);
            }

            var index = MathSupport.RandomNatural(collection.Count - 1);
            return index < 0 ? default(T) : collection.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Get Random object.
        /// </summary>
        /// <param name="enumerable">Enumerable of objects.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        [System.Diagnostics.Contracts.Pure]
        public static T GetRandomObject(IEnumerable<T> enumerable) {
            Contract.Requires(enumerable != null);
            if (enumerable == null) {
                return default(T);
            }

            var enumerable1 = enumerable as IList<T> ?? enumerable.ToList();
            if (!enumerable1.Any()) {
                return default(T);
            }

            var index = MathSupport.RandomNatural(enumerable1.Count - 1);
            return index < 0 ? default(T) : enumerable1.ElementAtOrDefault(index);
        }

        #endregion
    }
}
