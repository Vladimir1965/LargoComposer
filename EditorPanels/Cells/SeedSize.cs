// <copyright file="SeedSize.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;

namespace EditorPanels.Cells
{
    /// <summary> A seed size. </summary>
    public class SeedSize
    {
        #region Constants
        /// <summary>
        /// The basic width
        /// </summary>
        public const int BasicWidth = 100;

        /// <summary>
        /// The basic height
        /// </summary>
        public const int BasicHeight = 32;

        /// <summary>
        /// The basic margin
        /// </summary>
        public const int BasicMargin = 2;

        /// <summary>
        /// The basic font size
        /// </summary>
        public const int BasicFontSize = 12;

        /// <summary> The text margin. </summary>
        public const int TextMargin = 5;

        #endregion

        #region Constructors

        /// <summary> Initializes static members of the SeedSize class. </summary>
        static SeedSize() {
            CurrentWidth = BasicWidth;
            CurrentFontSize = BasicFontSize;
            CurrentHeight = BasicHeight;

            CultureInfo = new CultureInfo("en-US");
        }
        #endregion

        #region Properties - Size
        //// Cell after percent resize

        /// <summary> Gets or sets the current width. </summary>
        /// <value> The width of the current. </value>
        public static int CurrentWidth { get; set; }

        /// <summary> Gets or sets the size of the current font. </summary>
        /// <value> The size of the current font. </value>
        public static int CurrentFontSize { get; set; }

        /// <summary> Gets or sets the current height. </summary>
        /// <value> The height of the current. </value>
        public static int CurrentHeight { get; set; }

        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public static CultureInfo CultureInfo { get; set; }
        #endregion

        /// <summary>
        /// The size changed as percent.
        /// </summary>
        /// <param name="givenValue">The given value.</param>
        public static void PercentSizeChanged(int givenValue) {
            var q = givenValue / 100.0f;
            CurrentWidth = (int)Math.Round(BasicWidth * q);
            CurrentFontSize = (int)Math.Round(BasicFontSize * q);
            CurrentHeight = (int)Math.Round(BasicHeight * q);

            //// this.EditorSpace.RefreshMusterGrid();
        }
    }
}
