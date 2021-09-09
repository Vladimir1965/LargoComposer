// <copyright file="MelodicInstrument.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Melodic Instrument.
    /// </summary>
    public class MelodicInstrument
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the midi section.
        /// </summary>
        /// <value>
        /// The midi section.
        /// </value>
        public int MidiSection { get; set; }

        /// <summary>
        /// Gets or sets the instrument group.
        /// </summary>
        /// <value>
        /// The instrument group.
        /// </value>
        public int InstrumentGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MelodicInstrument"/> is bass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bass; otherwise, <c>false</c>.
        /// </value>
        public bool Bass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MelodicInstrument"/> is middle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if middle; otherwise, <c>false</c>.
        /// </value>
        public bool Middle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MelodicInstrument"/> is high.
        /// </summary>
        /// <value>
        ///   <c>true</c> if high; otherwise, <c>false</c>.
        /// </value>
        public bool High { get; set; }

        /// <summary>
        /// Gets or sets the minimum tone.
        /// </summary>
        /// <value>
        /// The minimum tone.
        /// </value>
        public int MinTone { get; set; }

        /// <summary>
        /// Gets or sets the minimum tone symbol.
        /// </summary>
        /// <value>
        /// The minimum tone symbol.
        /// </value>
        public string MinToneSymbol { get; set; }

        /// <summary>
        /// Gets or sets the maximum tone.
        /// </summary>
        /// <value>
        /// The maximum tone.
        /// </value>
        public int MaxTone { get; set; }

        /// <summary>
        /// Gets or sets the maximum tone symbol.
        /// </summary>
        /// <value>
        /// The maximum tone symbol.
        /// </value>
        public string MaxToneSymbol { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0,6} {1,30} {2,3} {3,3}-{4,3}", this.Id, this.Name, this.MidiSection, this.MinToneSymbol, this.MaxToneSymbol);
            return s.ToString();
        }
        #endregion
    }
}
