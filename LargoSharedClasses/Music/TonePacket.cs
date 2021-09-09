// <copyright file="TonePacket.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Tone Packet.
    /// </summary>
    public sealed class TonePacket
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TonePacket" /> class.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        public TonePacket(IList<IMusicalTone> givenTones) {
            this.BarTones = givenTones;
            this.IntendedTones = new List<IntendedTone>();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the tone wrappers.
        /// </summary>
        /// <value>
        /// Returns value.
        /// </value>        
        public IList<IMusicalTone> BarTones { get; }

        /// <summary>
        /// Gets the tone wrappers.
        /// </summary>
        /// <value>
        /// Returns value.
        /// </value>        
        public IList<IntendedTone> IntendedTones { get; }

        /// <summary>
        /// Marks the best tone.
        /// </summary>
        /// <param name="tone">The melodic tone.</param>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public void MarkBestTone(MusicalTone tone) {
            var wrapper = (from p in this.IntendedTones
                          where p.Note == tone.ToShortString()
                          select p).FirstOrDefault();
            if (wrapper == null)
            {
                return;
            }

            wrapper.IsSelected = true;
        }
        #endregion
    }
}