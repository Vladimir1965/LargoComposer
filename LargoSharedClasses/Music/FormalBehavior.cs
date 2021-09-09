// <copyright file="FormalBehavior.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Formal Behavior.
    /// </summary>
    public class FormalBehavior {
        #region Properties
        /// <summary>
        /// Gets or sets the variance.
        /// </summary>
        /// <value>
        /// The variance.
        /// </value>
        public float Variance { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public float Balance { get; set; }

        /// <summary>
        /// Gets or sets the entropy.
        /// </summary>
        /// <value>
        /// The entropy.
        /// </value>
        public float Entropy { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Variance={0,6:F1} Balance={1,6:F1} Entropy={2,6:F1} ", this.Variance, this.Balance, this.Entropy);
            return s.ToString();
        }
        #endregion
    }
}
