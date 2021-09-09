// <copyright file="RhythmicEnergyStream.cs" company="Traced-Ideas, Czech republic">
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
    /// Rhythmic Energy Stream.
    /// </summary>
    public class RhythmicEnergyStream
    {
        #region Fields
        /// <summary>
        /// The energy bars
        /// </summary>
        private List<RhythmicEnergyBar> energyBars;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicEnergyStream"/> class.
        /// </summary>
        public RhythmicEnergyStream()
        {
            this.energyBars = new List<RhythmicEnergyBar>();
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
        public IList<RhythmicEnergyBar> EnergyBars
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<RhythmicEnergyBar>>() != null);
                if (this.energyBars == null)
                {
                    throw new InvalidOperationException("Energy bars are null.");
                }

                return this.energyBars;
            }

            set => this.energyBars = (List<RhythmicEnergyBar>)value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }
        #endregion 

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat(" RhythmicEnergyStream (Length {0})", this.EnergyBars.Count);

            return s.ToString();
        }
        #endregion
    }
}
