// <copyright file="ComparerGenStruct.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using System.Collections.Generic;

namespace LargoSharedClasses.Music
{
    /// <summary> Comparer of owners . </summary>
    /// <remarks>
    /// Enables comparing of Owners according to one given property.
    /// </remarks>
    /// <typeparam name="T">Structure - the generic type parameter.</typeparam>
    public sealed class ComparerGenStruct<T> : IComparer<T>
                                                where T : IGeneralStruct {
        #region Constructors
        /// <summary> Initializes a new instance of the ComparerGenStruct class. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="givenDirection">Sort direction.</param>
        public ComparerGenStruct(GenProperty property, GenSortDirection givenDirection) {
            this.Property = property;
            this.Direction = givenDirection;
        }
        #endregion

        /// <summary> Gets property to be ordered.</summary>
        /// <value> Property description. </value>
        private GenProperty Property { get;  }

        /// <summary> Gets direction of ordered set.</summary>
        /// <value> Property description. </value>
        private GenSortDirection Direction { get; }

        /// <summary> Compare property values of two given objects. </summary>
        /// <param name="x">First object.</param>
        /// <param name="y">Second object.</param>
        /// <returns> Returns value. </returns>
        int IComparer<T>.Compare(T x, T y) {
            if (x != null)
            {
                var fx = x.GetProperty(this.Property);
                if (y != null)
                {
                    var fy = y.GetProperty(this.Property);
                    if (this.Direction == GenSortDirection.Descending) {
                        if (fx > fy) {
                            return -1;
                        }

                        return fx < fy ? 1 : 0;
                    }

                    if (fx > fy) {
                        return 1;
                    }

                    if (fx < fy) {
                        return -1;
                    }
                }
            }

            return 0;
        }
    }
}
