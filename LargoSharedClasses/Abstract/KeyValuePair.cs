// <copyright file="KeyValuePair.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Gen Key Value.
    /// </summary>
    [Serializable]
    [XmlRoot]
    public sealed class KeyValuePair {
        #region Constructors
        /// <summary> Initializes a new instance of the KeyValuePair class. </summary>
        public KeyValuePair() {
            this.Key = string.Empty;
            this.Value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the KeyValuePair class.
        /// </summary>
        /// <param name="key">Object Value.</param>
        /// <param name="value">Object Text.</param>
        public KeyValuePair(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePair"/> class.
        /// </summary>
        /// <param name="key">The given key.</param>
        /// <param name="value">The given value.</param>
        public KeyValuePair(int key, string value) {
            this.Key = key.ToString("D", CultureInfo.CurrentCulture.NumberFormat);
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePair"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyValuePair(long key, string value) {
            this.Key = key.ToString();
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePair"/> class.
        /// </summary>
        /// <param name="key">The given key.</param>
        /// <param name="value">The given value.</param>
        public KeyValuePair(decimal key, string value) {
            this.Key = key.ToString("D", CultureInfo.CurrentCulture.NumberFormat);
            this.Value = value;
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets. </summary>
        /// <value> Property description. </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets the numeric key.
        /// </summary>
        /// <value> Property description. </value>
        public long NumericKey => int.Parse(this.Key, CultureInfo.CurrentCulture.NumberFormat);

        /// <summary> Gets or sets. </summary>
        /// <value> Property description. </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <value> Property description. </value>
        public string KeyValue => $"{this.Key} {this.Value}";

        #endregion

        #region Public methods
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.Key + "\t");
            s.AppendLine(this.Value);

            return s.ToString();
        }
        #endregion
    }
}