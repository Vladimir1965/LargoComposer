// <copyright file="IMusicalLocation.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// Musical Location Interface.
    /// </summary>
    public interface IMusicalLocation {
        #region Properties
        /// <summary> Gets or sets the bar number. </summary>
        /// <value> Property description. </value>
        int BarNumber { get; set; }

        /// <summary> Gets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        byte Staff { get; }

        /// <summary> Gets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        byte Voice { get; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        byte InstrumentNumber { get; set; }

        //// <summary> Gets voice number (MusicXml). </summary>
        //// <value> Property description. </value>
        //// MidiChannel Channel { get; }
        #endregion
    }
}
