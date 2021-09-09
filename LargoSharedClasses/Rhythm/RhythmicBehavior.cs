// <copyright file="RhythmicBehavior.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;

namespace LargoSharedClasses.Rhythm
{
    /// <summary>
    /// Rhythmic Behavior.
    /// </summary>
    public class RhythmicBehavior {
        #region Properties
        /// <summary>
        /// Gets or sets the filling.
        /// </summary>
        /// <value>
        /// The filling.
        /// </value>
        public float Filling { get; set; }

        /// <summary>
        /// Gets or sets the filling.
        /// </summary>
        /// <value>
        /// The filling.
        /// </value>
        public float Tension { get; set; }

        /// <summary>
        /// Gets or sets the complexity.
        /// </summary>
        /// <value>
        /// The complexity.
        /// </value>
        public float Complexity { get; set; }

        /// <summary>
        /// Gets or sets the beat.
        /// </summary>
        /// <value>
        /// The beat.
        /// </value>
        public float Beat { get; set; }

        /// <summary>
        /// Gets or sets the mobility.
        /// </summary>
        /// <value>
        /// The mobility.
        /// </value>
        public float Mobility { get; set; }

        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                    "Filling={0,6:F1} Tension={1,6:F1} Complexity={2,6:F1} Beat={3,6:F1} Mobility={4,6:F1}", this.Filling, this.Tension, this.Complexity, this.Beat, this.Mobility);
            return s.ToString();
        }
        #endregion
    }
}
