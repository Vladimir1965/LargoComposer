// <copyright file="MusicalFormalTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Formal Tone.
    /// </summary>
    [XmlRoot]
    public sealed class MusicalFormalTone {
        #region Properties
        /// <summary>
        /// Gets or sets Tone.
        /// </summary>
        /// <value> Property description. </value>
        public string Tone { get; set; }

        /// <summary>
        /// Gets or sets Potential.
        /// </summary>
        /// <value> Property description. </value>
        public float Potential { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Tone={0,6} Potential={1,6:F1} ", this.Tone, this.Potential);
            return s.ToString();
        }
        #endregion
    }
}
