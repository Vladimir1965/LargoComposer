// <copyright file="IAbstractLine.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LargoSharedClasses.Interfaces
{
    /// <summary> Interface for abstract line. </summary>
    public interface IAbstractLine
    {
        /// <summary> Gets or sets the type of the line. </summary>
        /// <value> The type of the line. </value>
        MusicalLineType LineType { get; set; }

        /// <summary> Gets or sets purpose of the line. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        LinePurpose Purpose { get; set; }

        /// <summary> Gets the get x coordinate element. </summary>
        /// <value> The get x coordinate element. </value>
        XElement GetXElement { get; }

        /// <summary> Gets or sets the zero-based index of the line. </summary>
        /// <value> The line index. </value>
        int LineIndex { get; set; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        byte LineNumber { get; }

        /// <summary> Gets or sets the main voice. </summary>
        /// <value> The main voice. </value>
        IAbstractVoice MainVoice { get; set; }

        /// <summary> Gets or sets the system length. </summary>
        /// <value> The length of the system. </value>
        int SystemLength { get; set; }

        /// <summary> Gets or sets the identifier of the line. </summary>
        /// <value> The identifier of the line. </value>
        Guid LineIdent { get; set; }

        /// <summary> Gets or sets the voices. </summary>
        /// <value> The voices. </value>
        List<IAbstractVoice> Voices { get; set; }

        /// <summary> Gets a list of identifiers. </summary>
        /// <value> A list of identifiers. </value>
        IList<KeyValuePair> Identifiers { get; }

        /// <summary>
        /// Gets or sets the first status.
        /// </summary>
        /// <value>
        /// The first status.
        /// </value>
        LineStatus FirstStatus { get; set; }
    }
    }