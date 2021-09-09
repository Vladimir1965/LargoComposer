// <copyright file="MusicalPoint.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Editor Point.
    /// </summary>
    public struct MusicalPoint {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalPoint" /> struct.
        /// </summary>
        /// <param name="givenLineIndex">Index of the given line.</param>
        /// <param name="givenBarNumber">The given bar number.</param>
        public MusicalPoint(int givenLineIndex, int givenBarNumber)
            : this()
        {
            this.LineIndex = givenLineIndex;
            this.BarNumber = givenBarNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        public int BarNumber { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineIndex { get; }

        /// <summary>
        /// Gets Elements the identifier.
        /// </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public string Identifier {
            get
            {
                var ident = string.Format(CultureInfo.CurrentCulture, "{0}#{1}", this.LineIndex, this.BarNumber);
                return ident;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefined => this.BarNumber > 0 && this.LineIndex >= 0;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => this.BarNumber == 0 || this.LineIndex < 0;

        #endregion

        #region Public Static
        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="bar">The bar.</param>
        /// <returns> Returns value. </returns>
        public static MusicalPoint GetPoint(int line, int bar) {
            return new MusicalPoint(line, bar);
            //// var p = this.IsMainArea(line, bar) ? this.points[line, bar] : new MusicalPoint(line, bar);
            //// return p; 
        }
        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return this.Identifier;
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public string ToDisplayString() {
            var s = new StringBuilder();
            s.AppendFormat(
                    " Bar {0} line {1}", this.BarNumber, this.LineIndex);
            return s.ToString();
        }
        #endregion
    }
}
