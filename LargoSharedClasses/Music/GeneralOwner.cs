// <copyright file="GeneralOwner.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Abstract;

    /// <summary> General owner. </summary>
    /// <remarks>
    /// GeneralOwner is prototype of objects that have some properties (e.g. all musical modalities and structures,
    /// and also requests looking for them,...). It provides method for summation properties according
    /// to requested weights and method supporting search of objects with given properties. </remarks>
    [Serializable]
    [XmlRoot]
    public class GeneralOwner : ICloneable {
        /// <summary> Properties and their values.</summary>
        private readonly Dictionary<GenProperty, float> properties;

        /// <summary> Initializes a new instance of the GeneralOwner class.  Serializable. </summary>
        public GeneralOwner() {
            // Properties = new Hashtable(); 
            this.properties = new Dictionary<GenProperty, float>();
        }

        /// <summary> Gets properties and their values.</summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Dictionary<GenProperty, float> Properties {
            get {
                Contract.Ensures(Contract.Result<Dictionary<GenProperty, float>>() != null);
                if (this.properties == null) {
                    throw new InvalidOperationException("Empty properties.");
                }

                return this.properties;
            }
        }

        #region Static operators

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(GeneralOwner owner1, GeneralOwner owner2) { return object.Equals(owner1, owner2); }
        //// public static bool operator !=(GeneralOwner owner1, GeneralOwner owner2) { return !object.Equals(owner1, owner2);  }
        //// but TICS rule 7@531: Class GeneralOwner implements operator + but not operator ==.

        /// <summary>
        /// Operator + to join sets of properties.
        /// </summary>
        /// <param name="owner1">General owner 1.</param>
        /// <param name="owner2">General owner 2.</param>
        /// <returns> Returns value. </returns>
        public static GeneralOwner operator +(GeneralOwner owner1, GeneralOwner owner2) {
            if (owner1 == null) {
                return null;
            }

            var owner = (GeneralOwner)owner1.Clone();
            if (owner2?.Properties != null) {
                owner.AppendProperties(owner2.Properties);
            }

            return owner;
        }

        /// <summary>
        /// Add to join sets of properties.
        /// </summary>
        /// <param name="owner1">General owner 1.</param>
        /// <param name="owner2">General owner 2.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static GeneralOwner Add(GeneralOwner owner1, GeneralOwner owner2) {
            if (owner1 == null) {
                return null;
            }

            var owner = (GeneralOwner)owner1.Clone();
            if (owner2?.Properties != null) {
                owner.AppendProperties(owner2.Properties);
            }

            return owner;
        }

        #endregion

        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="value">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        /// DA0011: .*CompareTo(object) = 7,64;
        /// CompareTo functions should be cheap and not allocate any memory. 
        /// Reduce complexity of CompareTo function if possible.
        [JetBrains.Annotations.PureAttribute]
        public virtual int CompareTo(object value) {
            return value is GeneralOwner go ? this.Properties.Count.CompareTo(go.Properties.Count) : 0;
            //// This kills the DataGrid                 
            //// throw new ArgumentException("Object is not a GeneralOwner");
        }

        /// <summary> Test of equality. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override bool Equals(object obj) {
            //// check null (this pointer is never null in C# methods)
            if (object.ReferenceEquals(obj, null)) {
                return false;
            }

            if (object.ReferenceEquals(this, obj)) {
                return true;
            }

            if (this.GetType() != obj.GetType()) {
                return false;
            }

            return this.CompareTo(obj) == 0;
        }

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override int GetHashCode() {
            return this.Properties == null ? 0 : this.Properties.Count.GetHashCode();
        }

        /// <summary> Makes a deep copy of the GeneralOwner object. </summary>
        /// <returns> Returns object. </returns>
        [JetBrains.Annotations.PureAttribute]
        public virtual object Clone() {
            var owner = new GeneralOwner();
            owner.AppendProperties(this.properties);
            return owner;
        }

        /// <summary> Property settings. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="value">Value of the property.</param>
        public void SetProperty(GenProperty property, float value) {
            this.Properties[property] = value;
        }

        /// <summary> Returns float value of given property (or zero). </summary>
        /// <param name="property">General musical property.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public float GetProperty(GenProperty property) {
            return this.Properties.ContainsKey(property) ? this.Properties[property] : 0f;
        }

        /// <summary> Property settings. </summary>
        /// <param name="givenProperties">Values of musical properties.</param>
        public void CopyProperties(Dictionary<GenProperty, float> givenProperties) {
            if (givenProperties == null) {
                return;
            }
            //// this.properties = new Dictionary<GenProperty, float>();
            this.Properties.Clear();
            foreach (var rde in givenProperties) {
                this.Properties[rde.Key] = rde.Value;
            }
        }

        /// <summary> Property settings. </summary>
        /// <param name="givenProperties">Values of musical properties.</param>
        public void AppendProperties(Dictionary<GenProperty, float> givenProperties) {
            if (givenProperties == null) {
                return;
            }

            foreach (var rde in givenProperties) {
                this.Properties[rde.Key] = rde.Value;
            }
        }

        /// <summary>
        /// Writes the behavior to properties.
        /// </summary>
        public virtual void WriteBehaviorToProperties() {
        }

        /// <summary> Sum of property values multiplied by requested weights. </summary>
        /// <param name="request">General musical request.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public float SumForRequest(GeneralRequest request) {
            const float epsilon = 0.001f;
            if (request?.Items == null) {
                return 0;
            }

            float sum = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var rde in request.Items) {
                var p = rde.Key;
                var item = rde.Value;
                if (item.Value == null) { //// 2019/05
                    continue;
                }

                if (Math.Abs(item.Weight - 0) < epsilon || !this.Properties.ContainsKey(p)) { //// item == null ||
                    continue;
                }

                var propValue = this.properties[p];
                var value = item.Value != null ? -Math.Abs((float)(propValue - item.Value)) : propValue; // else Value determine quality of the sounding    
                sum += (Math.Abs(item.Weight - 1) > epsilon) ? item.Weight * value : value;
            }

            if (request.RandomEffect > 0) {
                sum += MathSupport.RandomCorrection(request.RandomEffect);
            }

            return sum;
        }

        #region String representation
        /// <summary> Returns string with key and value of all the properties. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public string StringOfProperties() {
            const byte numberOfPropertiesInRow = 5;
            //// if (this.Properties == null) {  return string.Empty;  }

            var s = new StringBuilder();
            var n = 0;
            foreach (var key in this.properties.Keys) {
                s.AppendFormat(
                        "{0,12}={1,5:F1},",
                        key, 
                        this.properties[key]); // System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                // ReSharper disable once InvertIf
                if (n++ != numberOfPropertiesInRow) {
                    continue;
                }

                s.AppendFormat(CultureInfo.CurrentCulture, Environment.NewLine);
                n = 0;
            }

            return s.ToString();
        }

        /// <summary> String representation of the object.. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override string ToString() {
            return this.StringOfProperties();
        }
        #endregion
    }
}