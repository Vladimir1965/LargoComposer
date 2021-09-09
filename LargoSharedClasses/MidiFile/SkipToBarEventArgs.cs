// <copyright file="SkipToBarEventArgs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Editor SkipToBar EventArgs.
    /// </summary>
    public class SkipToBarEventArgs : EventArgs {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipToBarEventArgs"/> class.
        /// </summary>
        /// <param name="givenBarNumber">The given line rhythm.</param>
        public SkipToBarEventArgs(int givenBarNumber) {
            this.BarNumber = givenBarNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkipToBarEventArgs"/> class.
        /// </summary>
        [UsedImplicitly]
        public SkipToBarEventArgs() {    
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the line rhythm.
        /// </summary>
        /// <value>
        /// The line rhythm.
        /// </value>
        public int BarNumber { get; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("SkipToBar {0}", this.BarNumber);

            return s.ToString();
        }
        #endregion
    }
}
