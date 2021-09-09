// <copyright file="HarmonicBehavior.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Text;

namespace LargoSharedClasses.Harmony
{
    /// <summary>
    /// Harmonic Behavior.
    /// </summary>
    /// <seealso cref="BindingBehavior" />
    public class HarmonicBehavior : BindingBehavior {
        #region Properties
        /// <summary>
        /// Gets or sets inner measure of dissonance.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public float Consonance { get; set; }

        /// <summary>
        /// Gets or sets the formal genus.
        /// </summary>
        /// <value>
        /// The formal genus.
        /// </value>
        public float Genus { get; set; }

        /// <summary>
        /// Gets or sets the potential.
        /// </summary>
        /// <value>
        /// The potential.
        /// </value>
        public float Potential { get; set; }

        /// <summary>
        /// Gets or sets inner balance.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public float Balance { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Consonance={0,6:F1} Genus={1,6:F1} Potential={2,6:F1} Balance={3,6:F1} ", this.Consonance, this.Genus, this.Potential, this.Balance);
            return s.ToString();
        }
        #endregion
    }
}
