// <copyright file="HarmonicFunction.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Harmony
{
    /// <summary>
    /// Harmonic Function.
    /// </summary>
    [XmlRoot]
    public sealed class HarmonicFunction {
        #region Properties
        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        /// <value> Property description. </value>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value> Property description. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Structure.
        /// </summary>
        /// <value> Property description. </value>
        public string Structure { get; set; }

        /// <summary>
        /// Gets or sets Order.
        /// </summary>
        /// <value> Property description. </value>
        public byte Order { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0,15} {1,30} {2,30}", this.Number, this.Name, this.Structure);
            return s.ToString();
        }
        #endregion
    }
}
