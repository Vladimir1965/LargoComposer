// <copyright file="ContentCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Cells
{
    using EditorPanels;
    using EditorPanels.Abstract;
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Orchestra;
    using LargoSharedClasses.Rhythm;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interact logic for ContentCell.
    /// </summary>
    public class ContentCell : BaseCell
    {
        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="ContentCell"/> class. </summary>
        /// <param name="givenMaster"> The given master - editor space. </param>
        /// <param name="givenLineIdx"> Zero-based index of the given line. </param>
        /// <param name="givenBarIdx">  Zero-based index of the given bar. </param>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public ContentCell(EditorSpace givenMaster, int givenLineIdx, int givenBarIdx) : base(givenMaster) {
            this.LineIndex = givenLineIdx;
            this.BarIndex = givenBarIdx;

            this.Width = SeedSize.BasicWidth;
            this.Height = SeedSize.BasicHeight;
            //// this.RhythmicStructure = new RhythmicStructure(RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, 12), 0);
        }

        /// <summary> Initializes a new instance of the <see cref="ContentCell" /> class. </summary>
        /// <param name="givenMaster"> The given master - editor space. </param>
        /// <param name="givenElement">   The given element. </param>
        public ContentCell(EditorSpace givenMaster, IAbstractElement givenElement) : base(givenMaster) {
            this.Bar = givenElement.Bar;
            this.Line = givenElement.Line;
            this.Element = (MusicalElement)givenElement;
            this.BarIndex = this.Bar.BarNumber - 1;
            this.LineIndex = this.Line.LineIndex;
            this.Point = this.Element.Point;
            this.Width = SeedSize.BasicWidth;
            this.Height = SeedSize.CurrentHeight - SeedSize.BasicMargin;
            //// this.RenderSize = new System.Windows.Size(this.Width, this.Height);
            this.HasContent = this.Element.HasContent;
            this.Status = this.Element.Status;
        }

        #endregion

        #region Properties - Main
        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
        public IAbstractBar Bar { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public IAbstractLine Line { get; set; }

        /// <summary>
        /// Gets or sets the first unit.
        /// </summary>
        /// <value>
        /// The first unit.
        /// </value>
        public MusicalElement Element { get; set; }

        /// <summary>
        /// Gets the identifiers.
        /// </summary>
        /// <value>
        /// The identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers {
            get {
                var items = this.Element.Identifiers.ToList();
                return items;
            }
        }
        #endregion

        #region Properties - Status

        /// <summary> Gets or sets the status. </summary>
        /// <value> The status. </value>
        public LineStatus Status { get; set; }

        #endregion

        #region Properties - Content

        /// <summary>
        /// Gets or sets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        public bool HasContent { get; set; }

        /// <summary> Gets or sets the rhythmic structure. </summary>
        /// <value> The rhythmic structure. </value>
        public RhythmicStructure RhythmicStructure { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="givenData">The given data.</param>
        /// <returns> Returns value. </returns>
        public bool SetData(IDataObject givenData) {
            bool change = false;
            if (givenData.GetData("RhythmicFace") is RhythmicFace rhythmicFace) {
                RhythmicFace rhythmicFace1 = rhythmicFace;
                this.Status.RhythmicFace = rhythmicFace1;
                change = true;
            }

            if (givenData.GetData("MelodicFace") is MelodicFace melodicFace) {
                MelodicFace melodicFace1 = melodicFace;
                this.Status.MelodicFace = melodicFace1;
                change = true;
            }

            if (givenData.GetData("MelodicFunction") is MelodicFunction melodicFunction) {
                MelodicFunction melodicFunction1 = melodicFunction;
                this.Status.MelodicFunction = melodicFunction1;
                change = true;
            }

            if (givenData.GetData("MelodicShape") is MelodicShape melodicShape) {
                MelodicShape melodicShape1 = melodicShape;
                this.Status.MelodicShape = melodicShape1;
                change = true;
            }

            if (givenData.GetData("RhythmicStructure") is RhythmicStructure rhythmicStructure) {                
                var system = this.Bar.Header.System;
                var rs = rhythmicStructure;
                if (system.RhythmicOrder != rhythmicStructure.Order) { //// 2020/11
                    rs = rhythmicStructure.ConvertToSystem(system.RhythmicSystem);
                }

                this.Status.RhythmicStructure = rs;
                change = true;
            }

            if (givenData.GetData("MelodicStructure") is MelodicStructure melodicStructure) {
                MelodicStructure melodicStructure1 = melodicStructure;
                this.Status.MelodicStructure = melodicStructure1;
                change = true;
            }

            if (givenData.GetData("OrchestraUnit") is OrchestraUnit orchestraUnit) {
                OrchestraUnit orchestraUnit1 = orchestraUnit;
                this.Status.OrchestraUnit = orchestraUnit1;
                //// change = true;
            }

            if (change) {
                this.SetFormattedText(null);
            }

            return change;
        }

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
            //// if (this.formattedText != null) {
            //// return this.formattedText; }
            var sb = new StringBuilder();
            if (this.Master.IsMusicEditor) {
                sb.Append(this.Element.DisplayText);
            }
            else {
                var status = this.Element?.Status;
                if (status != null) {
                    sb.AppendLine(status.MelodicFace?.Name);
                    sb.Append(status.RhythmicFace?.Name);
                }
            }
            //// sb.Append(this.RhythmicStructure.ElementSchema);
            var text = AbstractText.Singleton.FormatText(sb.ToString(), (int)this.Width - SeedSize.BasicMargin);
            this.SetFormattedText(text);

            return text;
        }

        #endregion

        #region Copy-Paste
        /// <summary>
        /// Copies this instance.
        /// </summary>
        public override void Copy() {
            var sb = new StringBuilder();

            //// Cycle for all units ???
            var xstatus = this.Element.Status.GetXElement;

            var item = xstatus.ToString();
            sb.AppendFormat("{0};", item);
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

            var item = splitArray.First();
            var xstatus = System.Xml.Linq.XElement.Parse(item);

            var header = this.Master.GetMusicalHeader;
            var status = new LineStatus(xstatus, header);
            this.Status.SetStatus(status, EditorContent.Cell);

            System.Console.Beep(990, 180);
            var space = this.Master as EditorSpace;
            space?.InvalidateVisual();

            //// this.RedrawCell(false);
        }

        #endregion
    }
}