// <copyright file="SettingsApplication.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings Application.
    /// </summary>
    public class SettingsApplication
    {
        /// <summary>
        /// The manufacturer name
        /// </summary>
        public const string ManufacturerName = "Indefinite Software";

        /// <summary>
        /// The application name
        /// </summary>
        public const string ApplicationName = "Largo 2022";

        /// <summary>
        /// The application web
        /// </summary>
        public const string ApplicationWeb = "largo-composer.eu";

        /// <summary>
        /// The application information
        /// </summary>
        public const string ApplicationInfo = "Largo 2022 (largo-composer.eu)";

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xsettings = new XElement("Application");
                //// xbar.Add(new XAttribute("Number", this.BarNumber));

                return xsettings;
            }
        }
        #endregion
    }
}
