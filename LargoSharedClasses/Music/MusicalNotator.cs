// <copyright file="MusicalNotator.cs" company="Traced-Ideas, Czech republic">
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
    /// Musical Notator.
    /// </summary>
    public class MusicalNotator {
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
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [midi files].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [midi files]; otherwise, <c>false</c>.
        /// </value>
        public bool MidiFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether special files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if special files otherwise, <c>false</c>.
        /// </value>
        public bool MxlFiles { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation - not used, so marked as static. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0,15} {1,30}", this.Name, this.Path);
            return s.ToString();
        }
        #endregion
    }
}
