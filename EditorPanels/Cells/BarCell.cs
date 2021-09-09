// <copyright file="BarCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using EditorPanels.Abstract;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;

namespace EditorPanels.Cells
{
    /// <summary>
    /// Interact logic for BarCell.
    /// </summary>
    public class BarCell : BaseCell
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCell" /> class.
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        /// <param name="givenBar">The given bar.</param>
        /// ###
        public BarCell(EditorSpace givenMaster, IAbstractBar givenBar) : base(givenMaster) {
            this.Bar = givenBar;
            this.Point = new MusicalPoint(-1, this.Bar.BarIndex + 1);
            this.BarIndex = this.Bar.BarIndex;
            this.HarmonicStructure = new HarmonicStructure(HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder), "0,4,7");
        }

        #endregion

        #region Main Properties

        /// <summary> Gets or sets the harmonic structure. </summary>
        /// <value> The harmonic structure. </value>
        public HarmonicStructure HarmonicStructure { get; set; }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is single.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is single; otherwise, <c>false</c>.
        /// </value>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public bool IsSingle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public bool HasContent { get; set; }

        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
        public IAbstractBar Bar { get; set; }

        #endregion

        #region Public methods 

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="givenData">The given data.</param>
        /// <returns> Returns value. </returns>
        public bool SetData(IDataObject givenData) {
            if (givenData.GetData("HarmonicStructure") is HarmonicStructure harmonicStructure) {
                HarmonicStructure harmonicStructure1 = harmonicStructure;
                this.HarmonicStructure = harmonicStructure1;
                if (this.Bar?.HarmonicBar != null) {
                    this.Bar.HarmonicBar.SetStructure(this.HarmonicStructure);
                    return true;
                }
            }

            if (givenData.GetData("MusicalTempo") is MusicalTempo tempo) {
                //// var rs = new BarStatus { TempoNumber = int.Parse(tempo.Key) };
                var abstractBar = this.Bar;
                if (abstractBar != null) {
                    abstractBar.TempoNumber = (int)tempo; //// int.Parse(tempo.Key)
                }

                return true;
            }

            return false;
        }

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
                var sb = new StringBuilder();
                sb.AppendFormat("{0}\n", this.Bar.BarNumber);
                if (this.Bar.HarmonicBar != null) {
                    var outline = this.Bar.HarmonicBar.SimpleStructuralOutline;
                    if (outline.Length > 30) {
                        outline = outline.Left(30) + "...";
                    }

                    sb.AppendFormat("{0}\n", outline);
                }

                ////  if (this.HarmonicStructure != null) { sb.AppendFormat("{0} ({1})\n", this.HarmonicStructure.Shortcut, this.HarmonicStructure.ToneSchema); } */
                var ft = AbstractText.Singleton.FormatText(sb.ToString(), (int)this.Width - SeedSize.BasicMargin);
                return ft;
            }
        #endregion

        #region Copy-Paste
        /// <summary>
        /// Copies this instance.
        /// </summary>
        public override void Copy() {
                StringBuilder sb = new StringBuilder();
                var xharmony = this.Bar.HarmonicBar.GetXElement;
                var item = xharmony.ToString();
                sb.AppendFormat("{0};", item);
                Clipboard.SetText(sb.ToString());
                //// Clipboard.SetDataObject(xstatus); 
                Console.Beep(880, 180);
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
            var xharmony = XElement.Parse(item);

            var header = this.Master.GetMusicalHeader;
            HarmonicBar harmonicBar = new HarmonicBar(header, xharmony);
            this.Bar.SetHarmonicBar(harmonicBar);

            Console.Beep(990, 180);
            var space = this.Master as EditorSpace;
            space?.InvalidateVisual();
            //// this.RedrawCell(false);
        }

        #endregion
    }
}
