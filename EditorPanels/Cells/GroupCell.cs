// <copyright file="GroupCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace EditorPanels.Cells
{
    /// <summary>
    /// Group Cell.
    /// </summary>
    /// <seealso cref="BaseCell" />
    public class GroupCell : BaseCell
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCell" /> class.
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        /// <param name="givenCell">The given cell.</param>
        public GroupCell(EditorSpace givenMaster, ContentCell givenCell) : base(givenMaster) {
            this.PenBrush = Brushes.DarkGray;
            this.ContentBrush = Brushes.GhostWhite; 
            this.BarIndex = givenCell.BarIndex;
            this.LineIndex = givenCell.LineIndex;
            this.Point = givenCell.Point;
            this.InnerCells = new List<ContentCell>();
            this.FirstCell = givenCell;
            this.InnerCells.Add(givenCell);
        }
        #endregion 

        #region Properties
        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>
        /// The units.
        /// </value>
        public List<ContentCell> InnerCells { get; set; }

        /// <summary>
        /// Gets or sets the first unit.
        /// </summary>
        /// <value>
        /// The first unit.
        /// </value>
        public ContentCell FirstCell { get; set; }

        /// <summary>
        /// Gets the first unit.
        /// </summary>
        /// <value>
        /// The first unit.
        /// </value>
        public MusicalElement FirstElement => this.FirstCell.Element;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is single.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is single; otherwise, <c>false</c>.
        /// </value>
        public bool IsSingle { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; set; }

        #endregion

        #region Public methods
        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
            //// sb.Append(this.RhythmicStructure.ElementSchema);
            return this.FirstCell.FormattedText();
            //// var ft = AbstractText.Singleton.FormatText(this.FirstElement.DisplayText, (int)this.Width - SeedSize.BasicMargin);
            //// return ft;
        }

        /// <summary> Adds a musical element. </summary>
        /// <param name="givenCell"> The given cell. </param>
        public void AddInnerCell(ContentCell givenCell) {
            this.InnerCells.Add(givenCell);
            this.Length = this.InnerCells.Count;
            this.IsSingle = this.Length == 1;

            this.Width = (SeedSize.CurrentWidth * this.Length) - SeedSize.BasicMargin;
            ////   this.Margin = new System.Windows.Thickness(  CurrentWidth + (this.Point.BarNumber * CurrentWidth),
            ////   CurrentHeight + (this.Point.LineIndex * CurrentHeight),   0,  0); 
            this.Refresh();
        }

        /// <summary>
        /// Sets the inner cells.
        /// </summary>
        /// <param name="givenCells">The given cells.</param>
        public void SetInnerCellsStatus(List<ContentCell> givenCells) {
            //// this.InnerCells = givenCells;
            //// this.Length = this.InnerCells.Count;
            //// this.IsSingle = this.Length == 1;
            this.SetFormattedText(null);
            int i = 0;
            foreach (var cell in this.InnerCells) {
                var newStatus = givenCells[i].Status;
                cell.Element.Status = newStatus;
                cell.Status = newStatus;
                if (i < givenCells.Count - 1) {
                    i++;
                }

                //// this.FirstCell.BarIndex = this.BarIndex; //// 2020/11
                ////this.FirstCell.Element.Bar.BarIndex = this.BarIndex;
                //// this.FirstCell.Element.Status.BarNumber = this.FirstCell.Element.Bar.BarNumber;
                //// this.Point = this.FirstCell.Point;
            }

            //// this.Width = (SeedSize.CurrentWidth * this.Length) - SeedSize.BasicMargin;

            ////   this.Margin = new System.Windows.Thickness(  CurrentWidth + (this.Point.BarNumber * CurrentWidth),
            ////   CurrentHeight + (this.Point.LineIndex * CurrentHeight),   0,  0); 
            this.Refresh();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh() {
            this.FirstElement.PrepareContent(this.FirstElement.ContentType);
            //// this.ToolTip = this.FirstElement.ToolTip;
        }

        #endregion

        #region Copy-Paste
        /// <summary>
        /// Copies this instance.
        /// </summary>
        public override void Copy() {
            StringBuilder sb = new StringBuilder();

            //// Cycle for all units ???
            foreach (var cell in this.InnerCells) {
                var element = cell.Element;
                var xstatus = element.Status.GetXElement;

                var item = xstatus.ToString();
                sb.AppendFormat("{0};", item);
            }

            Clipboard.SetText(sb.ToString());
            //// Clipboard.SetDataObject(xstatus); 
            System.Console.Beep(880, 180);
        }

        /// <summary>
        /// Pastes this instance.
        /// </summary>
        public override void Paste() {
            var s = Clipboard.GetText();
            if (string.IsNullOrEmpty(s)) {
                return;
            }

            var splitArray = s.Split(';');
            if (!splitArray.Any()) {
                return;
            }

            var contentType = this.FirstElement.ContentType;
            int i = 0;
            foreach (var item in splitArray) {
                if (string.IsNullOrWhiteSpace(item)) {
                    continue;
                }

                if (i >= this.InnerCells.Count) {
                    break;
                }
                
                var cell = this.InnerCells[i++];
                var xstatus = XElement.Parse(item);
                var header = this.Master.GetMusicalHeader;
                var status = new LineStatus(xstatus, header);
                cell.Element.Status.SetStatus(status, contentType);
            }

            if (i > 0) {
                System.Console.Beep(990, 180);
                var space = this.Master as EditorSpace;
                space?.InvalidateVisual();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="givenData">The given data.</param>
        /// <returns> Returns value. </returns>
        public bool SetData(IDataObject givenData) {
            //// var space = this.Master as EditorSpace;
            //// if (space != null) {
            bool change = false;
            foreach (var cell in this.InnerCells) {
                if (cell.SetData(givenData)) {
                    change = true;
                }
            }

            if (change) {
                this.SetFormattedText(null);
            }

            return true;
        }

        #endregion
    }
}
