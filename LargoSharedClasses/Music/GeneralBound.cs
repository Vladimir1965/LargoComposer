// <copyright file="GeneralBound.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Boundary of values. </summary>
    /// <remarks> AbstractBound represents an interval of values defined by its lower and upper Limit. </remarks>
    [Serializable]
    [XmlRoot]
    public struct GeneralBound {
        /// <summary> Initializes a new instance of the GeneralBound struct. </summary>
        /// <param name="givenMin">Minimal value.</param>
        /// <param name="givenMax">Maximal value.</param>
        public GeneralBound(float givenMin, float givenMax) : this() {
            this.Min = givenMin;
            this.Max = givenMax;
        }

        /// <summary> Gets minimum value. </summary>
        /// <value> Property description. </value>
        public float Min { get; private set; }

        /// <summary> Gets maximum value. </summary>
        /// <value> Property description. </value>
        public float Max { get; private set; }

        /// <summary> Sets the given boundary values. </summary>
        /// <param name="givenMin">Minimal value.</param>
        /// <param name="givenMax">Maximal value.</param>
        public void SetBound(float givenMin, float givenMax) {
            this.Min = givenMin;
            this.Max = givenMax;
        }

        /// <summary> Sets the given boundary values. </summary>
        /// <param name="quotient">Given quotient for maximum.</param>
        public void MultiplyMax(float quotient) {
            this.Max = this.Max * quotient;
        }

        /// <summary> Checks if the boundary Contains the given number. </summary>
        /// <param name="number">Given number.</param>
        /// <returns> Returns value. </returns>
        public bool Contains(float number) {
            return (this.Min <= number) && (number <= this.Max);
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", this.Min.ToString("D", CultureInfo.CurrentCulture.NumberFormat), this.Max.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
        }
        #endregion
    }
}
