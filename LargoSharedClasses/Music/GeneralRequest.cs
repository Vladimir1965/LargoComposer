// <copyright file="GeneralRequest.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> General musical request. </summary>
    /// <remarks> Musical object. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class GeneralRequest : ICloneable {
        /// <summary> Properties and their values.</summary>
        private Dictionary<GenProperty, GeneralRequestItem> items;

        #region Constructors
        /// <summary> Initializes a new instance of the GeneralRequest class.  Serializable. </summary>
        public GeneralRequest() {
            this.items = new Dictionary<GenProperty, GeneralRequestItem>();
            this.RandomEffect = 0;
        }
        #endregion

        /// <summary> Gets properties and their values.</summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Dictionary<GenProperty, GeneralRequestItem> Items {
            get {
                Contract.Ensures(Contract.Result<Dictionary<GenProperty, GeneralRequestItem>>() != null);
                if (this.items == null) {
                    throw new InvalidOperationException("Null items.");
                }

                return this.items;
            }
        }

        /// <summary>  Gets or sets random effect. </summary>
        /// <value> Property description. </value>
        public float RandomEffect { get; set; }

        #region Static operators
        //// TICS rule 7@531: Class GeneralOwner implements operator + but not operator ==.

        /// <summary> Operator + to join sets of properties. </summary>
        /// <param name="request1">General musical request 1.</param>
        /// <param name="request2">General musical request 2.</param>
        /// <returns> Returns value. </returns>
        public static GeneralRequest operator +(GeneralRequest request1, GeneralRequest request2) {
            Contract.Requires(request1 != null && request2 != null);
            if (request1 == null) {
                return null;
            }

            if (request2 == null) {
                return null;
            }

            var request = (GeneralRequest)request1.Clone();
            request.AppendItems(request2.Items);

            return request;
        }

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(GeneralRequest request1, GeneralRequest request2) {  return object.Equals(request1, request2);  }
        //// public static bool operator !=(GeneralRequest request1, GeneralRequest request2) {  return !object.Equals(request1, request2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Add request.
        /// </summary>
        /// <param name="request1">Musical request1.</param>
        /// <param name="request2">Musical request2.</param>
        /// <returns> Returns value. </returns>
        public static GeneralRequest Add(GeneralRequest request1, GeneralRequest request2) {
            Contract.Requires(request1 != null && request2 != null);
            if (request1 == null || request2 == null) {
                return null;
            }

            var request = (GeneralRequest)request1.Clone();
            request.AppendItems(request2.Items);

            return request;
        }

        #endregion

        /// <summary> Test of equality. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
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

        /// <summary> Support sorting according to level and number. </summary>
        /// <param name="value">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public int CompareTo(object value) {
            return value is GeneralRequest gr ? this.Items.Count.CompareTo(gr.Items.Count) : 0;
            //// This kills the DataGrid                 
            //// throw new ArgumentException("Object is not a GeneralOwner");
        }

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode()
        {
            return this.Items == null ? 0 : this.Items.Count.GetHashCode();
        }

        /// <summary> Makes a deep copy of the GeneralRequest object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var request = new GeneralRequest();
            request.AppendItems(this.items);
            return request;
        }

        #region Items
        /// <summary> Property settings. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="weight">Weight of the request.</param>
        /// <param name="value">Requested value.</param>
        public void SetItem(GenProperty property, float weight, float? value) {
            //// if (this.Items == null) {  return; }
            var ri = new GeneralRequestItem(property, weight, value);
            this.items[property] = ri;
        }

        /// <summary> Returns float value of given property (or zero). </summary>
        /// <param name="property">General musical property.</param>
        /// <returns> Returns value. </returns>
        public bool HasItem(GenProperty property)
        {
            return this.Items != null && this.items.ContainsKey(property);
        }

        /// <summary> Returns float value of given property (or zero). </summary>
        /// <param name="property">General musical property.</param>
        /// <returns> Returns value. </returns>
        public GeneralRequestItem GetItem(GenProperty property) {
            return this.Items[property];
            //// return this.items.ContainsKey(property) ? this.Items[property] : null;
        }

        /// <summary> Property settings. </summary>
        /// <param name="givenItems">Items of musical properties.</param>
        public void CopyItems(Dictionary<GenProperty, GeneralRequestItem> givenItems) {
            this.items = new Dictionary<GenProperty, GeneralRequestItem>();
            if (givenItems == null)
            {
                return;
            }

            foreach (var rde in givenItems) {
                this.items[rde.Key] = rde.Value;
            }
        }

        /// <summary> Property settings. </summary>
        /// <param name="givenItems">Items of musical properties.</param>
        public void AppendItems(Dictionary<GenProperty, GeneralRequestItem> givenItems)
        {
            if (givenItems == null || this.items == null)
            {
                return;
            }

            foreach (var rde in givenItems) {
                this.items.Add(rde.Key, rde.Value);
            }
        }

        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            //// if (this.Items == null) {  return "Empty request"; }
            var s = new StringBuilder();
            // ReSharper disable once LoopCanBePartlyConvertedToQuery 
            //// .Where(key => this.Items[key] != null) .Where(key => this.Items[key] != null)
            foreach (var key in this.Items.Keys)
            {
                var item = this.Items[key];
                //// if (item != null) {
                s.AppendLine(item.ToString());
                //// }
            }

            return s.ToString();
        }
        #endregion
    }
}

//// unused  public GeneralRequest RhythmicalRequest(int barNumber) {
//// CourseMobility, CourseFilling, CourseVariability