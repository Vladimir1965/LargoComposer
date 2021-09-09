// <copyright file="BindingBehavior.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Binding Behavior.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class BindingBehavior {
        #region Properties
        /// <summary>
        /// Gets or sets inner continuity.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public float? Continuity { get; set; }

        /// <summary>
        /// Gets or sets inner impulse.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public float? Impulse { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Continuity={0,6:F1} Impulse={1,6:F1} ", this.Continuity, this.Impulse); 
            return s.ToString();
        }
        #endregion
    }
}
