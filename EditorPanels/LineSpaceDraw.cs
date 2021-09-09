// <copyright file="LineSpaceDraw.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Windows;
using System.Windows.Media;
using EditorPanels.Cells;

namespace EditorPanels
{
    /// <summary> A base chart master. </summary>
    public partial class LineSpace : FrameworkElement
    {
        #region Public properties - DrawingVisual

        /// <summary>
        /// Gets or sets the drawing visual.
        /// </summary>
        /// <value>
        /// The drawing visual.
        /// </value>
        public DrawingVisual DrawingVisual { get; set; }

        /// <summary>
        /// Gets the number of visual child elements within this element. Mandatory overrides for VisualChildrenCount property.
        /// </summary>
        protected override int VisualChildrenCount => 1;

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Argument Out Of Range Exception</exception>
        protected override Visual GetVisualChild(int index) {
            if (index != 0) {
                throw new ArgumentOutOfRangeException();
            }

            return this.children[index];
        }
        #endregion

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext) {
             //// using (DrawingContext dc = this.EditorSpace.DrawingVisual.RenderOpen()) {
             //// drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Yellow, 3), new Rect(new Point(100, 100), new Point(400, 200))); }; 

            //// Set coordinates and sizes of cells + shifts according to scrollbars   
            this.PlaceCells();

            //// Draw visible area
            this.DrawCells(drawingContext);
        }

        /// <summary>
        /// Creates the drawing visual.
        /// </summary>
        /// <returns> Returns value. </returns>
        private DrawingVisual CreateDrawingVisual() { //// int stationObjectSize, double actualWidth, double actualHeight) {
            //// initialize canvasGrid
            this.DrawingVisual = new DrawingVisual();

            //// end using clause
            return this.DrawingVisual;
        }        
        
        #region Private methods - Draw cells on Canvas

        /// <summary>
        /// Places the cells.
        /// </summary>
        private void PlaceCells() {
            var lineTopBand = this.TopSpace + this.TopMargin;
            foreach (var cell in this.VoiceCells) {
                cell.Left = this.LeftSpace;
                cell.Top = lineTopBand + (cell.Point.LineIndex * SeedSize.CurrentHeight);
            }
        }

        /// <summary>
        /// Draws the cells.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        private void DrawCells(DrawingContext drawingContext) {
            var lineTopBand = this.TopSpace + this.TopMargin;
            foreach (var cell in this.VoiceCells) {
                if (cell.Top + cell.Height < lineTopBand || cell.Top > 700) {
                    continue;
                }

                cell.DrawCell(drawingContext, true);
            }
        }
        #endregion
    }
}
