// <copyright file="HarmonicBand.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Harmony
{
    /// <summary>
    /// Musical Band.
    /// </summary>
    public sealed class HarmonicBand {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HarmonicBand class.
        /// </summary>
        /// <param name="bandNumber">Band number.</param>
        /// <param name="bandValue">Band value.</param>
        public HarmonicBand(int bandNumber, float bandValue) {
            this.Number = bandNumber;
            this.Value = bandValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicBand"/> class.
        /// </summary>
        [UsedImplicitly]
        public HarmonicBand() {    
        }  
        #endregion

        #region Properties
        /// <summary>
        /// Gets value.
        /// </summary>
        /// <value> Property description. </value>
        public int Number { get; }

        /// <summary>
        /// Gets or sets a value indicating whether.
        /// </summary>
        /// <value> Property description. </value>
        public bool Modal { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        /// <value> Property description. </value>
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the consonance bonus - measured to main sounding tone.
        /// </summary>
        /// <value> Property description. </value>
        public float SonanceBonus { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}\t", 
                    string.Format(CultureInfo.CurrentCulture.NumberFormat, "Band {0,6}: ", this.Number), 
                    string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F1} ", this.Value)));
            s.Append(this.Modal ? "modal" : "-");
            s.Append(string.Format(CultureInfo.CurrentCulture.NumberFormat, " Sonance {0,6:F1} ", this.SonanceBonus));
            //// s.Append(this.StringOfProperties());
            return s.ToString();
        }
        #endregion
    }
}
