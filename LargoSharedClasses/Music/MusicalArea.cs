// <copyright file="MusicalArea.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Area.
    /// </summary>
    public class MusicalArea {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalArea"/> class.
        /// </summary>
        /// <param name="givenStart">The given start.</param>
        /// <param name="givenEnd">The given end.</param>
        public MusicalArea(MusicalPoint givenStart, MusicalPoint givenEnd)
        {
            this.StartPoint = givenStart;
            this.EndPoint = givenEnd;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public MusicalPoint StartPoint { get; }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public MusicalPoint EndPoint { get; }

        /// <summary>
        /// Gets a value indicating whether multiple selection.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public bool IsSimple => this.StartPoint.Identifier == this.EndPoint.Identifier;

        #endregion

        /// <summary>
        /// Determines whether the specified given point contains point.
        /// </summary>
        /// <param name="givenPoint">The given point.</param>
        /// <returns>
        ///   <c>true</c> if the specified given point contains point; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPoint(MusicalPoint givenPoint)
        {
            bool flagLine = this.StartPoint.LineIndex <= givenPoint.LineIndex
                            && givenPoint.LineIndex <= this.EndPoint.LineIndex;
            if (!flagLine) {
                return false;
            }

            bool flagBar = this.StartPoint.BarNumber <= givenPoint.BarNumber
                            && givenPoint.BarNumber <= this.EndPoint.BarNumber;
            return flagBar;
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Area ({0}-{1})", this.StartPoint, this.EndPoint);

            return s.ToString();
        }
        #endregion
    }
}
