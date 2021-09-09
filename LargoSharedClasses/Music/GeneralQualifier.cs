// <copyright file="GeneralQualifier.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Qualifier of owners. </summary>
    /// <remarks>
    /// General Qualifier is designed to keep limits of properties (e.g. lower and upper bound for Level, i.e. number
    /// of sounding tones, entropy or genus,...). During generation of musical structures Musical Qualifier restricts
    /// set of all possible structures to some its subset. e.g.,if applied to structures of harmonical modality,
    /// it reduces set of chords, that will be used (triads, consonances, ...). </remarks>
    ////  [Serializable] 
    [XmlRoot]
    public sealed class GeneralQualifier : GeneralOwner {
        /// <summary> Threshold of inaccuracy. </summary>
        private const float Threshold = 0.01f;

        /// <summary> Dictionaries of limits. </summary>
        private readonly Dictionary<GenProperty, float> minvalue, maxvalue;

        /// <summary> Initializes a new instance of the GeneralQualifier class.  Serializable. </summary>
        public GeneralQualifier() { //// MusicalComponent aComponent
            //// this.component = aComponent;
            this.minvalue = new Dictionary<GenProperty, float>();
            this.maxvalue = new Dictionary<GenProperty, float>();
        }

        /// <summary>
        /// Set boundary limits for the given property.
        /// </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="givenMinValue">The given min value.</param>
        /// <param name="givenMaxValue">The given max value.</param>
        public void SetLimits(GenProperty property, float? givenMinValue, float? givenMaxValue) {
            if (givenMinValue == null || givenMaxValue == null) {
                return;
            }

            this.minvalue.Add(property, (float)givenMinValue - Threshold);
            this.maxvalue.Add(property, (float)givenMaxValue + Threshold);
        }

        /// <summary> Set boundary limits for the given property and bound selection (see MusicalValue). </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="bound">Bound of property.</param>
        /// <param name="value">Musical value.</param>
        public void SetBoundedLimits(GenProperty property, GeneralBound bound, MusicalValue value) {
            const byte boundDivision = 6;

            var step = (bound.Max - bound.Min) / boundDivision; // linear
            var lim1 = bound.Min + step; // 2*step; 
            var lim2 = bound.Min + (3 * step);
            var lim3 = bound.Min + (4 * step);
            switch (value) {
                case MusicalValue.VeryLow:
                    this.SetLimits(property, bound.Min, lim1); 
                    break;
                case MusicalValue.Lower:
                    this.SetLimits(property, bound.Min, lim2); 
                    break;
                case MusicalValue.NotHigh:
                    this.SetLimits(property, bound.Min, lim3); 
                    break;
                case MusicalValue.Middle:
                    this.SetLimits(property, lim1, lim3); 
                    break;
                case MusicalValue.NotLow:
                    this.SetLimits(property, lim1, bound.Max);
                    break;
                case MusicalValue.Higher:
                    this.SetLimits(property, lim2, bound.Max);
                    break;
                case MusicalValue.VeryHigh:
                    this.SetLimits(property, lim3, bound.Max); 
                    break;
                default:
                    this.SetLimits(property, bound.Min, bound.Max); 
                    break;
            }
        }

        /// <summary> Minimum value from condition for the given property. </summary>
        /// <param name="property">General musical property.</param>
        /// <returns> Returns value. </returns>
        public float MinValueOfProperty(GenProperty property)
        {
            return this.minvalue?[property] ?? 0;
        }

        /// <summary> Maximum value from condition for the given property. </summary>
        /// <param name="property">General musical property.</param>
        /// <returns> Returns value. </returns>
        public float MaxValueOfProperty(GenProperty property)
        {
            return this.maxvalue?[property] ?? 0;
        }

        /// <summary> Determine if given object satisfy all the boundary conditions. </summary>
        /// <param name="generalStructure">General musical structure.</param>
        /// <returns> Returns value. </returns>
        public bool Convenient(IGeneralStruct generalStructure) { //// virtual
            if (this.minvalue == null || this.maxvalue == null || generalStructure == null) {
                return false;
            }

            return !(from rde in this.minvalue
                     let minValue = rde.Value
                     let maxValue = this.maxvalue[rde.Key]
                     let v = generalStructure.GetProperty(rde.Key)
                     where minValue > v || v > maxValue
                     select minValue).Any();
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            if (this.minvalue == null || this.maxvalue == null) {
                return string.Empty;
            }

            var s = new StringBuilder();
            foreach (var key in this.minvalue.Keys) {
                var min = this.minvalue[key]; 
                var max = this.maxvalue[key];
                s.AppendFormat(
                        "{0,12}:({1,4:F1};{2,4:F1})\r",
                        key,
                        min,
                        max); // System.Globalization.CultureInfo.CurrentCulture.NumberFormat
            }

            return s.ToString();
        }
        #endregion
    }
}
