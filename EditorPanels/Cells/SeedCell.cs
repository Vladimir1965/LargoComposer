// <copyright file="SeedCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Windows;
using System.Windows.Media;

namespace EditorPanels.Cells
{
    /// <summary> A seed cell. </summary>
    public class SeedCell
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedCell"/> class.
        /// </summary>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public SeedCell() {
            //// SeedSize.HighlightPenBrush = Brushes.DarkBlue;
            this.HighlightPenBrush = Brushes.DarkRed;
        }

        #endregion 

        #region Properties - Coordinates

        /// <summary> Gets or sets the left. </summary>
        /// <value> The left. </value>
        public double Left { get; set; }

        /// <summary> Gets or sets the top. </summary>
        /// <value> The top. </value>
        public double Top { get; set; }

        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public double Height { get; set; }

        /// <summary> Gets or sets the top. </summary>
        /// <value> The top. </value>
        public double Width { get; set; }

        #endregion

        #region Properties - Main raster 
        /// <summary> Gets or sets the zero-based index of the line. </summary>
        /// <value> The line index. </value>
        public int LineIndex { get; set; }

        /// <summary> Gets or sets the zero-based index of the bar. </summary>
        /// <value> The bar index. </value>
        public int BarIndex { get; set; }
        #endregion

        #region Properties - Drawing
        /// <summary> Gets or sets the rectangle. </summary>
        /// <value> The rectangle. </value>
        public Rect Rectangle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseCell"/> is highlighted.
        /// </summary>
        /// <value> <c>true</c> if highlighted; otherwise, <c>false</c>. </value>
        public bool IsHighlighted { get; set; }

        #endregion

        #region Properties - Brushes

        /// <summary> Gets or sets the pen brush. </summary>
        /// <value> The pen brush. </value>
        public Brush PenBrush { get; set; }

        /// <summary> Gets or sets the highlight pen brush. </summary>
        /// <value> The highlight pen brush. </value>
        public Brush HighlightPenBrush { get; set; }

        /// <summary> Gets or sets the content brush. </summary>
        /// <value> The content brush. </value>
        public Brush ContentBrush { get; set; }
        #endregion

        #region Public methods - virtual

        /// <summary>
        /// Copies this instance.
        /// </summary>
        public virtual void Copy() {
        }

        /// <summary>
        /// Pastes this instance.
        /// </summary>
        public virtual void Paste() {
        }
        #endregion

        #region Public methods 

        /// <summary> Query if 'p' contains point. </summary>
        /// <param name="p"> A Point to process. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool ContainsPoint(Point p) {
            return this.Left <= p.X && this.Left + this.Width >= p.X && this.Top <= p.Y && this.Top + this.Height >= p.Y;
        }
        #endregion
    }
}
