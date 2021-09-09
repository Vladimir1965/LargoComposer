// <copyright file="BitRange.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Music
{
    /// <summary> Bit Range. </summary>
    /// <remarks>
    /// BitRange represents interval of bits inside of binary structure. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class BitRange : ICloneable, IComparable {
        #region Constructors
        /// <summary> Initializes a new instance of the BitRange class.  Serializable. </summary>
        public BitRange()
        {
        }

        /// <summary> Initializes a new instance of the BitRange class. </summary>
        /// <param name="order">Order of system.</param>
        /// <param name="givenBitFrom">Starting bit.</param>
        /// <param name="length">Length of the range.</param>
        public BitRange(byte order, byte givenBitFrom, byte length) {
            //// Contract.Requires(order > 0);
            //// This all code is bit slow and very often used - any guess how to improve it?
            if (order == 0) {
                throw new ArgumentException("Order of bit range must not be 0.");
            }

            this.Order = order;
            this.BitFrom = givenBitFrom;
            if (length > 0) {
                int bitTo;
                checked {
                    bitTo = this.BitFrom + length - 1;
                    if (bitTo < 0) {
                        bitTo = 0;
                    }

                    if (bitTo > order - 1) {
                        bitTo = order - 1;
                    }
                }

                this.BitTo = (byte)bitTo;
                //// this.BitTo = (byte)Math.Max(this.BitFrom + length - 1, 0);
                //// this.BitTo = (byte)Math.Min(this.BitTo, this.Order - 1);
                this.Length = (byte)(this.BitTo - this.BitFrom + 1);
            }
            else {
                this.BitTo = 0;
                this.Length = 0;
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                //// Contract.Requires(this.BitRange != null);
                var xmlBitRange = new XElement(
                           "B",
                           new XAttribute("Order", this.Order),
                           new XAttribute("Start", this.BitFrom),
                           new XAttribute("Length", this.Length));
                return xmlBitRange;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets order of system. </summary>
        /// <value> Property description. </value>
        public byte Order { get; set; }

        /// <summary> Gets or sets the first bit of the range. </summary>
        /// <value> Property description. </value>
        public byte BitFrom { get; set; }

        /// <summary> Gets or sets length of the range. </summary>
        /// <value> Property description. </value>
        public byte Length { get; set; }

        /// <summary> Gets a value indicating whether IsEmpty. </summary>
        /// <value> Property description. </value>
        public bool IsEmpty => this.Length <= 0;

        /// <summary> Gets or sets the last bit of the range. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public byte BitTo { get; set; }
        #endregion

        #region Static operators

        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(BitRange range1, BitRange range2) { return object.Equals(range1, range2);  }
        //// public static bool operator !=(BitRange range1, BitRange range2) { return !object.Equals(range1, range2); }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(BitRange object1, BitRange object2) {
            if (object1 != null && object2 != null) {
                return object1.BitFrom < object2.BitFrom;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >(BitRange object1, BitRange object2) {
            if (object1 != null && object2 != null) {
                return object1.BitFrom > object2.BitFrom;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <=(BitRange object1, BitRange object2) {
            if (object1 != null && object2 != null) {
                return object1.BitFrom <= object2.BitFrom;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="object1">The object1.</param>
        /// <param name="object2">The object2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >=(BitRange object1, BitRange object2) {
            if (object1 != null && object2 != null) {
                return object1.BitFrom >= object2.BitFrom;
            }

            return false;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the BitRange object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            return new BitRange(this.Order, this.BitFrom, this.Length);
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Sets Xml representation.
        /// </summary>
        /// <param name="element">Xml Element.</param>
        public void SetXElement(XElement element) {
            Contract.Requires(element != null);
            //// if (element == null) { return; }

            this.Order = XmlSupport.ReadByteAttribute(element.Attribute("Order"));
            this.BitFrom = XmlSupport.ReadByteAttribute(element.Attribute("Start"));
            this.Length = XmlSupport.ReadByteAttribute(element.Attribute("Length"));
        }
        #endregion

        #region Comparison
        /// <summary> Support for potential comparing of bit ranges. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public int CompareTo(object obj) {
            return obj is BitRange br ? this.BitFrom.CompareTo(br.BitFrom) : 0;
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a BitRange");
        }

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

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode() {
            return this.BitFrom.GetHashCode();
        }
        #endregion

        #region Operations
        /// <summary> Returns intersection with the given Range. </summary>
        /// <param name="range">Given range.</param>
        /// <returns>Resulting range.</returns>
        public bool Intersect(BitRange range) {
            if (range == null) {
                return false;
            }

            var b1 = Math.Max(range.BitFrom, this.BitFrom);
            var b2 = Math.Min(range.BitTo, this.BitTo);
            return b1 <= b2;
        }

        /// <summary> Returns intersection with the given Range. </summary>
        /// <param name="range">Given range.</param>
        /// <returns>Resulting range.</returns>
        public BitRange IntersectionWith(BitRange range) {
            if (range == null) {
                return null;
            }

            var b1 = Math.Max(range.BitFrom, this.BitFrom);
            var b2 = Math.Min(range.BitTo, this.BitTo);
            var br = b1 <= b2 ? new BitRange(this.Order, b1, (byte)(b2 - b1 + 1)) : new BitRange(this.Order, 0, 0);

            return br;
        }

        /// <summary>
        /// Bit to stops at.
        /// </summary>
        /// <param name="bitStop">The bit stop.</param>
        /// <returns> Returns value. </returns>
        public BitRange StopAt(byte bitStop) {
            var b1 = this.BitFrom;
            var b2 = Math.Min(bitStop, this.BitTo);
            var br = b1 <= b2 ? new BitRange(this.Order, b1, (byte)(b2 - b1 + 1)) : new BitRange(this.Order, 0, 0);

            return br;
        }

        /// <summary> Gets a value indicating whether the range contains the given bit. </summary>
        /// <param name="givenBit">Given bit.</param>
        /// <returns>Logical value.</returns>
        public bool ContainsBit(byte givenBit) {
            return (this.BitFrom <= givenBit) && (givenBit <= this.BitTo);
        }

        /// <summary> Checks if the Range Contains the given range. </summary>
        /// <param name="range">Given range.</param>
        /// <returns> Logical value. </returns>
        public bool CoverRange(BitRange range) {
            if (range == null) {
                return false;
            }

            return (this.BitFrom <= range.BitFrom) && (range.BitTo <= this.BitTo);
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.BitFrom.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            s.Append("-");
            s.Append(this.BitTo.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            s.Append("(");
            s.Append(this.Length.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            s.Append(")");
            return s.ToString();
        }
        #endregion
    }
}
