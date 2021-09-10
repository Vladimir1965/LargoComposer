// <copyright file="SidePanels.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings Import.
    /// </summary>
    public class SidePanels
    {

        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SidePanels internalSingleton = new SidePanels();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SidePanels"/> class.
        /// </summary>
        public SidePanels() {
            this.OpenPanels = new StringDictionary {
                { "SideInstrument", "1" },
                { "SideHarmony", "0" },
                { "SideModality", "0" },
                { "SideRhythm", "0" },
                { "SideMelody", "0" },
                { "SideTempo", "0" },
                { "SideVoices", "0" }
            };
        }

        #region Static properties
        /// <summary>
        /// Gets or sets the ProcessLogger Singleton.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Singleton MusicalSettings is null.</exception>
        public static SidePanels Singleton {
            get {
                Contract.Ensures(Contract.Result<SidePanels>() != null);
                if (internalSingleton == null) {
                    throw new InvalidOperationException("Singleton SidePanels is null.");
                }

                return internalSingleton;
            }

            set => internalSingleton = value;
        }
        #endregion


        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement markSettings = new XElement("Editor");
                foreach (var key in this.OpenPanels.Keys) {
                    markSettings.Add(new XAttribute((string)key, this.OpenPanels[(string)key]));
                }

                return markSettings;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the open panels.
        /// </summary>
        /// <value>
        /// The open panels.
        /// </value>
        public StringDictionary OpenPanels { get; set; }

        /// <summary>
        /// Determines whether the specified given name is open.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        /// <returns>
        ///   <c>true</c> if the specified given name is open; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpen(string givenName) {
            return this.OpenPanels[givenName] == "1";
        }

        /// <summary>
        /// Panels the open.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        public void PanelOpen(string givenName) {
            this.OpenPanels[givenName] = "1";
        }

        /// <summary>
        /// Panels the close.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        public void PanelClose(string givenName) {
            this.OpenPanels[givenName] = "0";
        }

        /// <summary>
        /// Sets the x element.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void SetXElement(XElement markSettings) {
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            var keys = new List<string>();
            foreach (var item in this.OpenPanels.Keys) {
                keys.Add((string)item);
            }

            foreach (var key in keys) {
                var value = (string)markSettings.Attribute(key);
                if (this.OpenPanels.ContainsKey(key)) {
                    this.OpenPanels[key] = value;
                }
            }
        }
    }
}
