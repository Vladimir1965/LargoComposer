// <copyright file="MusicalShift.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Shift.
    /// </summary>
    public sealed class MusicalShift : IMusicalLocation {
        #region Properties
        /// <summary> Gets or sets bar number. </summary>
        /// <value> Property description. </value>
        public int BarNumber { get; set; }

        /// <summary> Gets or sets staff number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte Staff { get; set; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte Voice { get; set; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        public short Value { get; set; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        public byte InstrumentNumber { get; [UsedImplicitly] set; }

        /// <summary> Gets or sets voice number (MusicXml). </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; [UsedImplicitly] set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Bar={0,4} Value={1,6:F1} ", this.BarNumber, this.Value);
            return s.ToString();
        }
        #endregion
    }
}
