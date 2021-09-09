// <copyright file="PanelDocumentViewer.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Windows.Xps.Packaging;

namespace LargoSharedPanels.Support {
    /// <summary>
    /// PanelDocument Viewer.
    /// </summary>
    public sealed partial class PanelDocumentViewer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelDocumentViewer"/> class.
        /// </summary>
        public PanelDocumentViewer() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <param name="doc">Printable document.</param>
        public void LoadData(XpsDocument doc) {
            var fixedDocumentSequence = doc?.GetFixedDocumentSequence();
            if (fixedDocumentSequence == null)
            {
                return;
            }

            this.DocumentViewer1.Document = fixedDocumentSequence;
        }
        #endregion
    }
}