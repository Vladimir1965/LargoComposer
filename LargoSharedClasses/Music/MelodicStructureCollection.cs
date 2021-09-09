// <copyright file="MelodicStructureCollection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Melodic Struct Collection.
    /// </summary>
    [XmlRoot]
    public sealed class MelodicStructureCollection : Collection<MelodicStructure> {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MelodicStructureCollection class.
        /// </summary>
        public MelodicStructureCollection() {
        }

        /// <summary>
        /// Initializes a new instance of the MelodicStructureCollection class.
        /// </summary>
        /// <param name="givenList">Given list.</param>
        public MelodicStructureCollection(IList<MelodicStructure> givenList)
            : base(givenList) {
        }
        #endregion

        #region Properties
        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public string UniqueIdentifier {
            get {
                var ident = new StringBuilder();
                ident.Append(string.Format(CultureInfo.CurrentCulture, "#{0}#", this.Count));
                foreach (var sc in
                    this.Where(ms => ms.GetStructuralCode != null).SelectMany(ms => ms.GetStructuralCode)) {
                    ident.Append(sc); //// ElementSchema, DecimalNumber, ms.StructuralCode
                }

                return ident.ToString();
            }
        }

        /// <summary>
        /// Is Equal To.
        /// </summary>
        /// <param name="melodicStructures">Melodic structure collection.</param>
        /// <returns> Returns value. </returns>
        [Pure]
        public bool IsEqualTo(MelodicStructureCollection melodicStructures) {
            if (melodicStructures == null) {
                return false;
            }

            return string.CompareOrdinal(this.UniqueIdentifier, melodicStructures.UniqueIdentifier) == 0;
        }

        #endregion
    }
}
