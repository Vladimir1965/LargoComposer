// <copyright file="GeneralSystem.cs" company="Traced-Ideas, Czech republic">
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
using System.Xml.Serialization;

namespace LargoSharedClasses.Music
{
    /// <summary> Musical system. </summary>
    /// <remarks> System is prototype of so called formal GSystem. It is defined by 
    /// its order. System Contains a simple support for converting indexes into formal indexes.
    /// It serves as a common superclass for harmonic and rhythmical GSystem.
    /// Class helps to work with binary structures. </remarks>
 [Serializable] 
    [XmlRoot]
    public class GeneralSystem {
        /// <summary> Maximum (now supported) number of the system order. </summary>
        public const byte MaxOrder = 24;

        /// <summary>
        /// System degree.
        /// </summary>
        private byte degree;

        /// <summary>
        /// System order.
        /// </summary>
        private byte order;

        #region Constructors
        /// <summary> Initializes a new instance of the GeneralSystem class.  Serializable. </summary>
        public GeneralSystem() {
        }

        /// <summary> Initializes a new instance of the GeneralSystem class. </summary>
        /// <param name="degree">Degree of system.</param>       
        /// <param name="order">Order of system.</param>
        public GeneralSystem(byte degree, byte order) {
            if (degree == 0) {
                throw new ArgumentException("Degree of system must not be 0.");
            }

            if (order == 0) {
                throw new ArgumentException("Order of system must not be 0.");
            }

            this.Degree = degree;
            this.Order = order;
            this.Median = (byte)(this.Order / 2);
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets degree of the system. </summary>
        /// <value> Degree=2 for binary systems. </value>
        public byte Degree {
            get {
                Contract.Ensures(Contract.Result<byte>() > 0);
                if (this.degree <= 0) {
                    throw new InvalidOperationException("Degree is not positive number.");
                }

                return this.degree;
            }

            set {
                if (value <= 0) {
                    throw new ArgumentException("Degree must be positive number.", nameof(value));
                }

                this.degree = value;
            }
        }

        /// <summary> Gets or sets order of the system, i.e. total number of bits). </summary>
        /// <value> Property description. </value>
        public byte Order {
            get {
                Contract.Ensures(Contract.Result<byte>() > 0);
                if (this.order <= 0) {
                    throw new InvalidOperationException("Order is not positive number.");
                }

                return this.order;
            }

            set {
                if (value <= 0) {
                    throw new ArgumentException("Order must be positive number.", nameof(value));
                }

                this.order = value;
            }
        }

        /// <summary> Gets or sets median i.e. half order of the system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public byte Median { get; set; }
        #endregion

        #region Static methods
        /// <summary>
        /// Convert Struct.
        /// </summary>
        /// <param name="structNumber">Structural number.</param>
        /// <param name="sysOrderFrom">System Order From.</param>
        /// <param name="sysOrderTo">System Order To.</param>
        /// <returns> Returns value. </returns>
        public static long ConvertStruct(long structNumber, byte sysOrderFrom, byte sysOrderTo) { //// byte sysDegree
            if (sysOrderFrom == sysOrderTo) {
                return structNumber;
            }

            var rstruct = 0L;
            for (byte indexFrom = 0; indexFrom < sysOrderFrom; indexFrom++) {
                if (!BinaryNumber.ElementIsOn(structNumber, indexFrom))
                {
                    continue;
                }

                var ito = (byte)(int)Math.Round((double)indexFrom * sysOrderTo / sysOrderFrom);
                rstruct = rstruct | BinaryNumber.BitAt(ito);
            }

            //// long rstruct = (long)struct * (long)((Math.Pow(sysDegree, sysOrderTo) - 1) / (Math.Pow(sysDegree, sysOrderFrom) - 1));
            return rstruct;
        }

        #endregion

        /// <summary> Returns symbols for given element. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public long LongSize() {
            return (long)Math.Pow(this.Degree, this.Order);
        }

        /// <summary> Returns symbols for given element. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public decimal DecimalSize() {
            return (decimal)Math.Pow(this.Degree, this.Order);
        }

        /// <summary> Converts the integer index to its formal system representative element. </summary>
        /// <param name="sysLength">Real system length.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public byte FormalLength(int sysLength) {
            return MusicalProperties.FormalLength(this.Order, sysLength);  
        }

        /// <summary> Converts the integer index to its formal system representative number. </summary>
        /// <param name="sysLength">Real system length.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public int FormalMedianLength(int sysLength) {
            int frmLength = this.FormalLength(sysLength);
            return frmLength <= this.Median ? frmLength : (frmLength - this.Order);
        }

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "${0,2}", this.Order.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            return s.ToString();
        }
        #endregion
    }
}
