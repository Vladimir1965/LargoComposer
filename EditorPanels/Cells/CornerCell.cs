// <copyright file="CornerCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using System.Text;
using System.Windows.Media;

namespace EditorPanels.Cells
{
    /// <summary>
    /// Interact logic for Corner Cell.
    /// </summary>
    public class CornerCell : BaseCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CornerCell" /> class.
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        public CornerCell(EditorSpace givenMaster) : base(givenMaster) {
        }

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}\n", "Corner");

            var ft = AbstractText.Singleton.FormatText(sb.ToString(), (int)this.Width - SeedSize.BasicMargin);
            return ft;
        }
    }
}