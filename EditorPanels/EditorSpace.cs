// <copyright file="EditorSpace.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Cells;
using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using System;
using System.Windows;
using System.Windows.Media;

namespace EditorPanels
{
    /// <summary> A base chart master. </summary>
    public partial class EditorSpace : FrameworkElement, IEditorSpace
    {
        #region Fields - readonly
        /// <summary> The left margin. </summary>
        public readonly int LeftMargin = 2 * SeedSize.CurrentWidth;

        /// <summary> The top margin. </summary>
        public readonly int TopMargin = 2 * SeedSize.CurrentHeight;

        /// <summary>
        /// The delta vertical scroll
        /// </summary>
        public readonly int DeltaVerticalScroll = 10;

        /// <summary>
        /// The delta horizontal scroll
        /// </summary>
        public readonly int DeltaHorizontalScroll = 20;

        /// <summary> The left space. </summary>
        public readonly int LeftSpace = 20;

        /// <summary> The top space. </summary>
        public readonly int TopSpace = 20; //// 10
        #endregion

        #region Fields
        /// <summary> Margin on the right. </summary>
        public int MaxLeft; ////  = 45 vs 50 LeftSpace + 14 * SeedSize.CurrentWidth; //// 1400

        /// <summary> Margin on the bottom. </summary>
        public int MaxTop;  //// = 10 TopSpace + 20 * SeedSize.CurrentHeight;  //// 700;

        /// <summary>
        /// The children
        /// </summary>
        private readonly VisualCollection children;

        /// <summary>
        /// The musical header
        /// </summary>
        private MusicalHeader musicalHeader;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the <see cref="EditorSpace" /> class. </summary>
        public EditorSpace()
        {
            this.SelectionRectangle = new System.Windows.Shapes.Rectangle {
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            //// rectangle.Fill = System.Windows.Media.Brushes.SkyBlue;
            this.IsDraggingAllowed = true;

            this.ResetCells();

            //// TimedPlayer.Singleton.SkipToBar += this.EditorSkipToBar;
            this.children = new VisualCollection(this) {
                this.CreateDrawingVisual()
            };

            this.ContentType = EditorContent.Cell;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the number of bars.
        /// </summary>
        /// <value>
        /// The number of bars.
        /// </value>
        public int NumberOfBars { get; set; }

        /// <summary> Gets or sets the number of lines. </summary>
        /// <value> The total number of lines. </value>
        public int NumberOfLines { get; set; }

        /// <summary>
        /// Gets or sets the content of the musical.
        /// </summary>
        /// <value>
        /// The content of the musical.
        /// </value>
        public IMusicalContent MusicalContent { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public EditorContent ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is music editor.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is music editor; otherwise, <c>false</c>.
        /// </value>
        public bool IsMusicEditor { get; set; }

        /// <summary>
        /// Gets or sets the left scroll.
        /// </summary>
        /// <value>
        /// The left scroll.
        /// </value>
        public double LeftScroll { get; set; }

        /// <summary>
        /// Gets or sets the top scroll.
        /// </summary>
        /// <value>
        /// The top scroll.
        /// </value>
        public double TopScroll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show muted lines].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show muted lines]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMutedLines { get; set; }
        #endregion

        #region Public properties - Selection
        /// <summary>
        /// Gets or sets the selection rectangle.
        /// </summary>
        /// <value>
        /// The selection rectangle.
        /// </value>
        public System.Windows.Shapes.Rectangle SelectionRectangle { get; set; }

        /// <summary>
        /// Gets or sets the current bar number.
        /// </summary>
        /// <value>
        /// The current bar number.
        /// </value>
        [UsedImplicitly]
        public int CurrentBarNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selection in progress.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selection in progress; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectionInProgress { get; set; }

        /// <summary>
        /// Gets or sets the highlighted cell.
        /// </summary>
        /// <value>
        /// The highlighted cell.
        /// </value>
        public BaseCell SelectedCell { get; set; }

        #endregion

        #region Public properties - Dragging

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dragging allowed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDraggingAllowed { get; set; }
        #endregion

        #region Public properties - Music

        /// <summary> Gets or sets the get musical header. </summary>
        /// <value> The get musical header. </value>
        public MusicalHeader GetMusicalHeader {
            get {
                if (this.musicalHeader == null) {
                    this.musicalHeader = MusicalHeader.GetDefaultMusicalHeader;
                    this.musicalHeader.NumberOfBars = this.NumberOfBars;
                    this.musicalHeader.Metric.MetricBeat = 6;
                    this.musicalHeader.Tempo = 200;
                }

                return this.musicalHeader;
            }

            set => this.musicalHeader = value;
        }

        /// <summary> Gets the get harmonic stream. </summary>
        /// <value> The get harmonic stream. </value>
        [UsedImplicitly]
        public HarmonicStream GetHarmonicStream {
            get {
                var header = this.GetMusicalHeader;
                var stream = new HarmonicStream(header);

                foreach (var cell in this.BarCells) {
                    var harmonicBar = new HarmonicBar(header, cell.HarmonicStructure) {
                        BarNumber = cell.BarIndex + 1
                    };
                    stream.HarmonicBars.Add(harmonicBar);
                }

                return stream;
            }
        }
        #endregion

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="givenContent">Content of the given.</param>
        /// <param name="givenIsMusic">if set to <c>true</c> [given is music].</param>
        public void LoadContent(IMusicalContent givenContent, bool givenIsMusic)
        {
            this.MusicalContent = givenContent;
            this.musicalHeader = this.MusicalContent.Header;
            this.IsMusicEditor = givenIsMusic;
            this.MakeCellsFromContent(givenContent, this.ContentType, givenIsMusic);
        }

        /// <summary>
        /// Copies the paste.
        /// </summary>
        /// <param name="code">The code.</param>
        public void CopyPaste(char code)
        {
            var c = this.SelectedCell;
            var cell = c; //// as ContentCell;
            if (cell != null) {
                var p = cell.Point;
                if (p.BarNumber == 0) {
                    return;
                }

                if (code == 'C') {
                    cell.Copy();
                }

                if (code == 'V') {
                    cell.Paste();
                    this.MakeCellsFromContent(this.MusicalContent, this.ContentType, this.IsMusicEditor);
                }
            }
        }

        #region Private methods - Selection
        /// <summary>
        /// Selects all.
        /// </summary>
        [UsedImplicitly]
        public void SelectAll()
        {
            MusicalPoint point0 = MusicalPoint.GetPoint(0, 1);
            MusicalPoint point1 = MusicalPoint.GetPoint(this.NumberOfLines, this.NumberOfBars);
            this.MarkSelectedArea(point0, point1, true);
        }

        /// <summary>
        /// Marks the selected area.
        /// </summary>
        /// <param name="givenPoint0">The given point0.</param>
        /// <param name="givenPoint1">The given point1.</param>
        /// <param name="isCellSelected">if set to <c>true</c> [is cell selected].</param>
        private void MarkSelectedArea(MusicalPoint givenPoint0, MusicalPoint givenPoint1, bool isCellSelected)
        { //// Brush color
            int minLine = Math.Min(givenPoint0.LineIndex, givenPoint1.LineIndex);
            int maxLine = Math.Max(givenPoint0.LineIndex, givenPoint1.LineIndex);
            var minBar = Math.Min(givenPoint0.BarNumber, givenPoint1.BarNumber);
            var maxBar = Math.Max(givenPoint0.BarNumber, givenPoint1.BarNumber);
            var startPoint = MusicalPoint.GetPoint((byte)minLine, minBar);
            var endPoint = MusicalPoint.GetPoint((byte)maxLine, maxBar);
            var area = new MusicalArea(startPoint, endPoint);

            foreach (var cell in this.ContentCells) {
                if (area.ContainsPoint(cell.Element.Point)) {
                    cell.IsSelected = isCellSelected;
                }
            }
        }

        #endregion
    }
}
