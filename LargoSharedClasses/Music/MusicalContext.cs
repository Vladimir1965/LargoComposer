// <copyright file="MusicalContext.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Settings;
using System.Text;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Musical Context.
    /// </summary>
    public class MusicalContext {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalContext"/> class.
        /// </summary>
        /// <param name="givenSettings">The given settings.</param>
        /// <param name="givenHeader">The given header.</param>
        public MusicalContext(MusicalSettings givenSettings, MusicalHeader givenHeader) {
            this.Settings = givenSettings;
            this.Header = givenHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalContext"/> class.
        /// </summary>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public MusicalContext()
        {            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public MusicalSettings Settings { get; }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("MusicalContext {0}", this.Header);

            return s.ToString();
        }
        #endregion
    }
}
