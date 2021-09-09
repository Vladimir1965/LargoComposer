// <copyright file="BaseCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using System.Windows;
using System.Windows.Media;

namespace EditorPanels.Cells
{
    /// <summary>
    /// Interact logic for BaseCell - Cell prototype.
    /// </summary>
    public class BaseCell : SeedCell //// : FrameworkElement
    {
        /// <summary>
        /// The formatted text
        /// </summary>
        private FormattedText formattedText;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCell" /> class.
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public BaseCell(IEditorSpace givenMaster) : base() {
            this.Master = givenMaster;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public IEditorSpace Master { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseCell"/> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }

        /// <summary> Gets or sets the point. </summary>
        /// <value> The point. </value>
        public MusicalPoint Point { get; set; }

        /// <summary>
        /// Gets the type of the cell.
        /// </summary>
        /// <value>
        /// The type of the cell.
        /// </value>
        public CellType CellType {
            get {
                CellType cellType;
                if (this.GetType() == typeof(GroupCell)) {
                    cellType = CellType.GroupCell;
                }
                else {
                    if (this.GetType() == typeof(BarCell)) {
                        cellType = CellType.BarCell;
                    }
                    else {
                        if (this.GetType() == typeof(LineCell)) {
                            cellType = CellType.LineCell;
                        }
                        else {
                            cellType = this.GetType() == typeof(ContentCell) ? CellType.ContentCell : CellType.None;
                        }
                    }
                } 

                return cellType;
            }
        }
        #endregion

        #region Draw

        /// <summary>
        /// Draws the cell.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="improvedText">if set to <c>true</c> [improved text].</param>
        public void DrawCell(DrawingContext drawingContext, bool improvedText) {
            if (this.Width <= 0 || this.Height <= 0) {
                return;
            }

            var point = new Point(this.Left, this.Top);
            this.Rectangle = new Rect(point, new Size(this.Width, this.Height));
            //// var pen = new Pen(Brushes.Black, highlight ? 3 : 1);

            //// var pen = this.IsHighlighted ? new Pen(Brushes.Blue, 3.0) : new Pen(Brushes.Black, 1.0);
            var pen = this.IsHighlighted ? new Pen(Brushes.Blue, 3.0) : new Pen(Brushes.Black, 1.0);
            pen = this.IsSelected ? new Pen(Brushes.Red, 5.0) : pen;

            drawingContext.DrawRectangle(this.ContentBrush, pen, this.Rectangle);
            var textPoint = new Point(this.Left + SeedSize.TextMargin, this.Top);
            this.formattedText = this.FormattedText();

            if (this.formattedText != null) {
                if (improvedText) {
                    this.formattedText.SetFontWeight(FontWeights.Bold);
                    drawingContext.DrawText(this.formattedText, textPoint);
                }
                else {
                    drawingContext.DrawText(this.formattedText, textPoint);
                }
            }
        }

        #endregion

        /// <summary>
        /// Sets the formatted cell.
        /// </summary>
        /// <param name="givenText">The given text.</param>
        public void SetFormattedText(FormattedText givenText) {
            this.formattedText = givenText;
        }

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public virtual FormattedText FormattedText() {
            return null;
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString() {
            return string.Format("{0} L{1} W{2} T{3} H{4}", this.GetType(), this.Left, this.Width, this.Top, this.Height);
        }
    }
}
