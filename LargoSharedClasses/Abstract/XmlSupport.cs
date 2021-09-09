// <copyright file="XmlSupport.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract {
    //// using System.Diagnostics.Contracts;

    /// <summary>
    /// Library Support.
    /// </summary>
    public static class XmlSupport {
        #region Public static methods
        /// <summary>
        /// Gets the x document root.
        /// </summary>
        /// <param name="filepath">The file path.</param>
        /// <returns> Returns value. </returns>
        public static XElement GetXDocRoot(string filepath) {
            if (!File.Exists(filepath)) {
                return null;
            }

            var xdoc = XDocument.Load(filepath);
            //// if (xdoc == null) { return null; }
            var root = xdoc.Root;
            return root;
        }
        #endregion

        #region Simple attributes
        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static byte ReadByteAttribute(XAttribute attribute) {
            if (attribute == null) {
                return 0;
            }

            return (byte)(int)attribute;
        }

        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        [UsedImplicitly]
        public static DateTime? ReadDateTimeAttribute(XAttribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }

            return (DateTime)attribute;
        }

        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        [System.Diagnostics.Contracts.Pure]
        public static short ReadShortIntegerAttribute(XAttribute attribute) {
            if (attribute == null) {
                return 0;
            }

            return (short)(int)attribute;
        }

        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static string ReadStringAttribute(XAttribute attribute) {
            if (attribute == null) {
                return string.Empty;
            }

            return (string)attribute;
        }

        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static int ReadIntegerAttribute(XAttribute attribute) {
            if (attribute == null) {
                return 0;
            }

            return (int)attribute;
        }

        /// <summary>
        /// Reads the double attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        public static double ReadDoubleAttribute(XAttribute attribute) {
            if (attribute == null) {
                return 0;
            }

            return (double)attribute;
        }

        /// <summary>
        /// Reads the float attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public static float ReadFloatAttribute(XAttribute attribute) {
            if (attribute == null) {
                return 0;
            }

            return (float)attribute;
        }

        /// <summary>
        /// Read Attribute.
        /// </summary>
        /// <param name="attribute">Given Attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static bool ReadBooleanAttribute(XAttribute attribute) {
            if (attribute == null) {
                return false;
            }

            return (bool)attribute;
        }

        #endregion
    }
}
