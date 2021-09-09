// <copyright file="EditorSpaceDraw.cs" company="Traced-Ideas, Czech republic">
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
    public partial class EditorSpace : FrameworkElement
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

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext) {
            //// Set coordinates and sizes of cells + shifts according to scrollbars   
            this.PlaceCells();

            //// Draw visible area
            this.DrawSpaceRectangle(drawingContext);
            this.DrawCells(drawingContext);
        }
        #endregion

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

        /// <summary>
        /// Draws the space rectangle.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        private void DrawSpaceRectangle(DrawingContext drawingContext) {
            var point = new Point(this.LeftSpace - 10, this.TopSpace - 10);
            var rectangle = new Rect(point, new Size(this.MaxLeft, this.MaxTop));
            Pen pen = new Pen(Brushes.Ivory, 1.0);
            var contentBrush = Brushes.Ivory;
            drawingContext.DrawRectangle(contentBrush, pen, rectangle);
        }

        #region Public methods - Draw cells on Canvas

        /// <summary>
        /// Places the cells.
        /// </summary>
        private void PlaceCells() {
            var lineTopBand = this.TopSpace + this.TopMargin;
            var lineLeftBand = this.LeftSpace + this.LeftMargin;

            var lineTopBase = lineTopBand - this.TopScroll;
            var lineLeftBase = lineLeftBand - this.LeftScroll;

            int lineTopRealShift = 0;
            foreach (var editorLine in this.EditorLines) {   
                if (!this.ShowMutedLines && editorLine.Line.Purpose == LargoSharedClasses.Music.LinePurpose.Mute) {
                    continue;
                }
                
                foreach (var cell in editorLine.ContentCells) {
                    var lineTopShift = lineTopRealShift; //// cell.Point.LineIndex * SeedSize.CurrentHeight;
                    var lineLeftShift = cell.BarIndex * SeedSize.CurrentWidth;  //// (cell.Point.BarNumber - 1)
                    
                    cell.Left = lineLeftBase + lineLeftShift;
                    cell.Top = lineTopBase + lineTopShift;
                } 
                 
                foreach (var cell in editorLine.GroupCells) {
                    var lineTopShift = lineTopRealShift; //// cell.Point.LineIndex * SeedSize.CurrentHeight;
                    var lineLeftShift = cell.BarIndex * SeedSize.CurrentWidth; //// (cell.Point.BarNumber - 1)

                    cell.Width = (cell.InnerCells.Count * SeedSize.CurrentWidth) - SeedSize.BasicMargin;
                    cell.Height = SeedSize.CurrentHeight - SeedSize.BasicMargin;

                    var previousWidth = lineLeftShift;
                    cell.Left = lineLeftBase + previousWidth;
                    if (cell.Left < lineLeftBand) {
                        cell.Width -= lineLeftBand - cell.Left;
                        cell.Left = lineLeftBand;
                    }

                    var previousHeight = lineTopShift;
                    cell.Top = lineTopBase + previousHeight;
                    if (cell.Top < lineTopBand) {
                        cell.Height -= lineTopBand - cell.Top;
                        cell.Top = lineTopBand;
                    }

                    if (cell.Left + cell.Width > this.MaxLeft) {
                        cell.Width = this.MaxLeft - cell.Left - SeedSize.BasicMargin;
                    }

                    if (cell.Top + cell.Height > this.MaxTop) {
                        cell.Height = this.MaxTop - cell.Height;
                        if (cell.Height > SeedSize.BasicHeight) {
                            cell.Height = SeedSize.BasicHeight; 
                        }
                    }
                }

                lineTopRealShift += SeedSize.CurrentHeight;
            }

            foreach (var cell in this.BarCells) {
                var lineLeftShift = cell.BarIndex * SeedSize.CurrentWidth; 

                cell.Left = lineLeftBase + lineLeftShift;
                cell.Top = this.TopSpace;
            }

            lineTopRealShift = 0;
            foreach (var cell in this.LineCells) {
                var lineTopShift = lineTopRealShift; //// cell.Point.LineIndex * SeedSize.CurrentHeight;

                cell.Left = this.LeftSpace;
                cell.Top = lineTopBase + lineTopShift;

                lineTopRealShift += SeedSize.CurrentHeight;
            }

            if (this.cornerCell != null) {
                this.cornerCell.Left = this.LeftSpace; //// - this.LeftScroll;
                this.cornerCell.Top = this.TopSpace; //// - this.TopScroll;
                this.cornerCell.Height = (2 * SeedSize.CurrentHeight) - SeedSize.BasicMargin;
                this.cornerCell.Width = (2 * SeedSize.CurrentWidth) - SeedSize.BasicMargin;
            }
        }

        /// <summary>
        /// Draws the cells.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        private void DrawCells(DrawingContext drawingContext) {
            var lineTopBand = this.TopSpace + this.TopMargin;
            var lineLeftBand = this.LeftSpace + this.LeftMargin;

            foreach (var track in this.EditorLines) {   
                foreach (var cell in track.GroupCells) {
                    if (cell.Left + cell.Width < lineLeftBand || cell.Left > this.MaxLeft) {
                        continue;
                    }

                    if (cell.Top + cell.Height < lineTopBand || cell.Top > this.MaxTop) {
                        continue;
                    } 

                    cell.DrawCell(drawingContext, false);
                }            
            }

            foreach (var cell in this.BarCells) {
                if (cell.Left + cell.Width < lineLeftBand || cell.Left + SeedSize.CurrentWidth > this.MaxLeft) {
                    continue;
                }

                cell.DrawCell(drawingContext, true);
            }

            foreach (var cell in this.LineCells) {
                if (cell.Top + cell.Height < lineTopBand || cell.Top + SeedSize.CurrentHeight > this.MaxTop) {
                    continue;
                }

                cell.DrawCell(drawingContext, true);
            }

            this.cornerCell?.DrawCell(drawingContext, true);
        }
        #endregion
    }
}
