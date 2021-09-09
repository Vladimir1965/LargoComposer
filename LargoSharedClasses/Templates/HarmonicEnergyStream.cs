// <copyright file="HarmonicEnergyStream.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    /// Energy stream.
    /// </summary>
    public class HarmonicEnergyStream {
        #region Fields
        /// <summary>
        /// The energy bars
        /// </summary>
        private List<HarmonicEnergyBar> energyBars;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicEnergyStream"/> class.
        /// </summary>
        public HarmonicEnergyStream() {
            this.energyBars = new List<HarmonicEnergyBar>();
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the energy bars.
        /// </summary>
        /// <value>
        /// The energy bars.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Energy bars are null.</exception>
        /// <exception cref="System.ArgumentException">Energy bars cannot be empty.;value</exception>
        public IList<HarmonicEnergyBar> EnergyBars
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<HarmonicEnergyBar>>() != null);
                if (this.energyBars == null) {
                    throw new InvalidOperationException("Energy bars are null.");
                }

                return this.energyBars;
            }

            set => this.energyBars = (List<HarmonicEnergyBar>)value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(" HarmonicEnergyStream (Length {0})", this.EnergyBars.Count);

            return s.ToString();
        }
        #endregion
    }
}
