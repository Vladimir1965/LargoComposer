// <copyright file="LineSpace.cs" company="Traced-Ideas, Czech republic">
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
using System.Windows.Media;
using System.Windows.Threading;

namespace EditorPanels
{
    /// <summary>
    /// Line Space.
    /// </summary>
    /// <seealso cref="System.Windows.FrameworkElement" />
    /// <seealso cref="LargoSharedClasses.Interfaces.IEditorSpace" />
    public partial class LineSpace : IEditorSpace
    {
        #region Fields
        /// <summary> Delta Vertical Scroll. </summary>
        [UsedImplicitly] public readonly int DeltaVerticalScroll = 5;

        /// <summary> Delta Horizontal Scroll. </summary>
        [UsedImplicitly] public readonly int DeltaHorizontalScroll = 5;

        /// <summary> The left space. </summary>
        protected readonly int LeftSpace = 50;

        /// <summary> The top space. </summary>
        protected readonly int TopSpace = 10;

        /// <summary> The left margin. </summary>
        [UsedImplicitly] protected readonly int LeftMargin = 2 * SeedSize.CurrentWidth;

        /// <summary> The top margin. </summary>
        protected readonly int TopMargin = 2 * SeedSize.CurrentHeight;

        #endregion

        #region Static Fields
        /// <summary>
        /// The void handler
        /// </summary>
        private static readonly VoidHandler Handler = () => { }; //// 2016/08

        #endregion
        
        /// <summary> Visual children. </summary>
        private readonly VisualCollection children;

        #region Constructors
        /// <summary> Initializes a new instance of the <see cref="LineSpace" /> class. </summary>
        public LineSpace() {
            this.ResetCells();

            this.children = new VisualCollection(this) {
                this.CreateDrawingVisual()
            };
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Void Handler.
        /// </summary>
        private delegate void VoidHandler();
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the number of bars.
        /// </summary>
        /// <value>
        /// The number of bars.
        /// </value>
        public int NumberOfBars { get; set; }

        /*
        /// <summary> Gets or sets the number of lines. </summary>
        /// <value> The total number of lines. </value>
        public int NumberOfLines { get; set; }
        */

        /// <summary>
        /// Gets or sets a value indicating whether this instance is music editor.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is music editor; otherwise, <c>false</c>.
        /// </value>
        public bool IsMusicEditor { get; set; }

        /// <summary>
        /// Gets or sets the highlighted cell.
        /// </summary>
        /// <value>
        /// The highlighted cell.
        /// </value>
        public BaseCell SelectedCell { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public IAbstractLine Line { get; set; }
        #endregion

        #region Public properties - Music

        /// <summary> Gets the get musical header. </summary>
        /// <value> The get musical header. </value>
        public MusicalHeader GetMusicalHeader {
            get {
                var header = MusicalHeader.GetDefaultMusicalHeader;
                header.NumberOfBars = this.NumberOfBars;
                header.Metric.MetricBeat = 6;
                header.Tempo = 200;

                return header;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the voices.
        /// </summary>
        /// <param name="line">The line.</param>
        public void LoadVoices(IAbstractLine line) {
            this.VoiceCells.Clear();

            int voicesTopSpace = 0;
            int voicesLeftSpace = 0;
            int voicesTopMargin = 0;
            int voiceIdx = 0;
            int lineShiftMargin = 0;

            if (line.Voices == null) {
                return;
            }

            lineShiftMargin += 5 * SeedSize.BasicMargin;
            foreach (var voice in line.Voices) {
                var cell = new VoiceCell(this, voice) {
                    LineIndex = voiceIdx,
                    PenBrush = Brushes.Black,
                    ContentBrush = Brushes.WhiteSmoke,
                    Left = voicesLeftSpace,
                    Top = voicesTopSpace + voicesTopMargin + lineShiftMargin + (voiceIdx * SeedSize.CurrentHeight),
                    Width = (2 * SeedSize.CurrentWidth) - SeedSize.BasicMargin,
                    Height = SeedSize.CurrentHeight - SeedSize.BasicMargin,
                    Voice = voice,
                    Point = new MusicalPoint(this.VoiceCells.Count, 0)
                };

                this.VoiceCells.Add(cell);
                voiceIdx++;
            }
        }

        /// <summary>
        /// Copies the paste.
        /// </summary>
        /// <param name="code">The code.</param>
        public void CopyPaste(char code)
        {
            var c = this.SelectedCell;
            var cell = c; //// as ContentCell;
            if (cell != null)
            {
                var p = cell.Point;
                if (p.BarNumber == 0)
                {
                    return;
                }

                if (code == 'C')
                {
                    cell.Copy();
                }

                if (code == 'V')
                {
                    cell.Paste();
                }
            }
        }
        #endregion

        #region Private static

        /// <summary>
        /// Does the void events.
        /// </summary>
        private static void DoVoidEvents() {
            //// Stack Overflow Exception
            try {
                //// Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new VoidHandler(() => { }));
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, Handler);
            }
            catch { //// StackOverflowException ex
                //// return;  //// !?!?!
            }
        }
        #endregion
    }
}
