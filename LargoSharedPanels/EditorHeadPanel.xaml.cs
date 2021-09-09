// <copyright file="EditorHeadPanel.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Support;
using LargoSharedControls.Abstract;
using System.Windows;

namespace LargoSharedPanels
{
    /// <summary>
    /// Panel Musical Schema.
    /// </summary>
    /// <seealso cref="PanelAbstract" />
    public sealed partial class EditorHeadPanel {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorHeadPanel" /> class.
        /// </summary>
        public EditorHeadPanel() {
            this.InitializeComponent();

            this.Focusable = true;
            //// this.textBlock1.IsEnabled = true;
            this.LoadData();
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        public MusicalBlock Block { get; private set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; private set; }

        #endregion

        #region Load

        /// <summary>
        /// Loads the block editor.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="givenFilePath">The given file path.</param>
        public void LoadBlock(MusicalBlock givenBlock, string givenFilePath) { //// BlockModel givenModel
            if (givenBlock == null) {
                return;
            }

            this.Block = givenBlock;
            this.FilePath = givenFilePath;

            var h = this.Block.FileHeading;
            if (h == null) {
                h = new FileHeading();
                this.Block.FileHeading = h;
            }

            if (string.IsNullOrWhiteSpace(h.WorkTitle)) {
                h.WorkTitle = this.Block.Header.FullName;
            }

            this.TbRow1.Text = h.WorkTitle;
            this.TbRow2.Text = h.Creator;
            this.TbRow3.Text = h.Composer;
            this.TbRow4.Text = h.WorkNumber;
            this.TbRow5.Text = h.Source;
            this.TbRow6.Text = h.Software;
            this.TbRow7.Text = h.Encoder;
            this.TbRow8.Text = h.EncodingDate;
            this.TbRow9.Text = h.EncodingDescription;
            this.TbRow10.Text = h.Rights;

            this.Refresh();
            //// this.textBlock1.Text = givenBlock.FullName;
            //// this.textBlock1.ToolTip = givenBlock.FullName;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();

            //// this.tbLab1.Text = LocalizedMusic.String("Work title");
            this.TbLab2.Text = LocalizedMusic.String("Creator");
            this.TbLab3.Text = LocalizedMusic.String("Composer");
            this.TbLab4.Text = LocalizedMusic.String("Work number");
            this.TbLab5.Text = LocalizedMusic.String("Source");
            this.TbLab6.Text = LocalizedMusic.String("Software");
            this.TbLab7.Text = LocalizedMusic.String("Encoder");
            this.TbLab8.Text = LocalizedMusic.String("EncodingDate");
            this.TbLab9.Text = LocalizedMusic.String("EncodingDescription");
            this.TbLab10.Text = LocalizedMusic.String("Rights");

            this.Localize();
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveChanges(object sender, RoutedEventArgs e) {
            var h = this.Block.FileHeading;
            h.WorkTitle = this.TbRow1.Text;
            h.Creator = this.TbRow2.Text;
            h.Composer = this.TbRow3.Text;
            h.WorkNumber = this.TbRow4.Text;
            h.Source = this.TbRow5.Text;
            h.Software = this.TbRow6.Text;
            h.Encoder = this.TbRow7.Text;
            h.EncodingDate = this.TbRow8.Text;
            h.EncodingDescription = this.TbRow9.Text;
            h.Rights = this.TbRow10.Text;

            //// document.FilePath = destinationFilePath;
            var port = PortAbstract.CreatePort(MusicalSourceType.MIFI);
            port.WriteMusicFile(this.Block.MusicalBundle, this.FilePath);
        }
        #endregion

        #region Other private methods
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }

        #endregion
    }
}
