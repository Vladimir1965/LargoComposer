// <copyright file="RhythmicStructureCollection.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Rhythmic Struct Collection.
    /// </summary>
    [XmlRoot]
    public sealed class RhythmicStructureCollection : Collection<RhythmicStructure> {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RhythmicStructureCollection class.
        /// </summary>
        public RhythmicStructureCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicStructureCollection class.
        /// </summary>
        /// <param name="givenList">Given list.</param>
        public RhythmicStructureCollection(IList<RhythmicStructure> givenList)
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
                    this.Where(rs => rs.GetStructuralCode != null).SelectMany(rs => rs.GetStructuralCode))
                {
                    ident.Append(sc);
                }
                //// ElementSchema, DecimalNumber, ms.StructuralCode

                return ident.ToString(); 
            }
        }

        /// <summary>
        /// Is Equal To.
        /// </summary>
        /// <param name="rhythmicStructures">Rhythmic Struct Collection.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public bool IsEqualTo(RhythmicStructureCollection rhythmicStructures) {
            if (rhythmicStructures == null) {
                return false;
            }

            return string.CompareOrdinal(this.UniqueIdentifier, rhythmicStructures.UniqueIdentifier) == 0; 
        }

        #endregion 
    }
}
