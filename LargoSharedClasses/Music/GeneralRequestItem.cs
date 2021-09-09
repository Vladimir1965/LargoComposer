// <copyright file="GeneralRequestItem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary>  Item of general musical request. </summary>
    /// <remarks>
    /// Gen Request Item.  </remarks>
    [Serializable]
    [XmlRoot]
    public struct GeneralRequestItem {
        /// <summary> Initializes a new instance of the GeneralRequestItem struct.  Serializable. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="weight">Weight of the request.</param>
        /// <param name="value">Requested value.</param>
        public GeneralRequestItem(GenProperty property, float weight, float? value) : this() {
            this.Property = property;
            this.Weight = weight;
            this.Value = value;
        }

        /// <summary>  Gets musical property. </summary>
        /// <value> Property description. </value>
        public GenProperty Property { get; }

        /// <summary> Gets name. </summary>
        /// <value> Property description. </value>
        public string Name => this.Property.ToString();

        /// <summary>  Gets weight. </summary>
        /// <value> Property description. </value>
        public float Weight { get; }

        /// <summary> Gets value. </summary>
        /// <value> Property description. </value>
        public float? Value { get; }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Item {0,12}: Weight={1,5:F1}, Value={2,5:F1}", this.Property, this.Weight, this.Value);
            return s.ToString();
        }
        #endregion
    }
}
