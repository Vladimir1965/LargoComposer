// <copyright file="WinMainAbstract.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using System.Windows.Media;

namespace LargoSharedClasses.Support
{
    //// using System.Windows.Xps.Packaging;

    /// <summary>
    /// WinMain Abstract.
    /// </summary>
    public class WinMainAbstract : WinAbstract
    {
        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("Window {0}", this.Name);

            return s.ToString();
        }

        #endregion

        /// <summary>
        /// Prints the visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        public void PrintVisual(Visual visual)
        {
        }
    }
}