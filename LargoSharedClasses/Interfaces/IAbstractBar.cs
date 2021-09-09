// <copyright file="IAbstractBar.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LargoSharedClasses.Interfaces
{
    /// <summary> Interface for abstract bar. </summary>
    public interface IAbstractBar
    {
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        MusicalHeader Header { get; }

        /// <summary> Gets or sets the zero-based index of the bar. </summary>
        /// <value> The bar index. </value>
        int BarIndex { get; set; }

        /// <summary> Gets or sets the bar number. </summary>
        /// <value> The bar number. </value>
        int BarNumber { get; set; }

        /// <summary> Gets or sets the harmonic structure. </summary>
        /// <value> The harmonic structure. </value>
        HarmonicStructure HarmonicStructure { get; set; }

        /// <summary> Gets or sets a value indicating whether this object is empty. </summary>
        /// <value> True if this object is empty, false if not. </value>
        bool IsEmpty { get; set; }

        /// <summary> Gets or sets the structural code. </summary>
        /// <value> The structural code. </value>
        string StructuralCode { get; set; }

        /// <summary> Gets or sets the system length. </summary>
        /// <value> The length of the system. </value>
        int SystemLength { get; set; }

        /// <summary>
        /// Gets the harmonic status.
        /// </summary>
        /// <value>
        /// The harmonic status.
        /// </value>
        HarmonicChange HarmonicStatus { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has harmonic motive.
        /// </summary>
        /// <value>
        /// Is <c>true</c> if this instance has harmonic motive; otherwise, <c>false</c>.
        /// </value>
        bool HasHarmonicMotive { get; }

        /// <summary>
        /// Gets or sets current harmonical bar.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [XmlIgnore]
        HarmonicBar HarmonicBar { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        int TempoNumber { get; set; }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        IList<MusicalElement> Elements { get; }

        /// <summary> Gets a list of identifiers. </summary>
        /// <value> A list of identifiers. </value>
        IList<KeyValuePair> Identifiers { get; }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns> Returns value. </returns>
        MusicalElement GetElement(Guid lineIdent);

        /// <summary>
        /// Sets the harmonic bar.
        /// </summary>
        /// <param name="value">The value.</param>
        void SetHarmonicBar(HarmonicBar value);
    }
}