// <copyright file="ToneStructure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Tone Structure.
    /// </summary>
    public class ToneStructure {
        #region Constructors

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public long Number { get; set; }

        /// <summary>
        /// Gets or sets the class number.
        /// </summary>
        /// <value>
        /// The class number.
        /// </value>
        public long ClassNumber { get; set; }

        /// <summary>
        /// Gets or sets the structural code.
        /// </summary>
        /// <value>
        /// The structural code.
        /// </value>
        public string StructuralCode { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the tones.
        /// </summary>
        /// <value>
        /// The tones.
        /// </value>
        public string Tones { get; set; }

        /// <summary>
        /// Gets or sets the chord step.
        /// </summary>
        /// <value>
        /// The chord step.
        /// </value>
        public byte ChordStep { get; set; }

        /// <summary>
        /// Gets or sets the chord base.
        /// </summary>
        /// <value>
        /// The chord base.
        /// </value>
        public string ChordBase { get; set; }

        /// <summary>
        /// Gets or sets the modality base.
        /// </summary>
        /// <value>
        /// The modality base.
        /// </value>
        [UsedImplicitly]
        public string ModalityBase { get; set; }

        /// <summary>
        /// Gets or sets the name of the chord.
        /// </summary>
        /// <value>
        /// The name of the chord.
        /// </value>
        public string ChordName { get; set; }

        /// <summary>
        /// Gets or sets the shortcut.
        /// </summary>
        /// <value>
        /// The shortcut.
        /// </value>
        public string Shortcut { get; set; }

        /// <summary>
        /// Gets or sets the name of the modality.
        /// </summary>
        /// <value>
        /// The name of the modality.
        /// </value>
        public string ModalityName { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(
                        "{0,6} L={1,2} R={2,2} {3,30} {4,30}", this.Number, this.Level, this.ChordBase, this.Tones, this.ChordName ?? this.ModalityName);
            return s.ToString();
        }
        #endregion
    }
}
