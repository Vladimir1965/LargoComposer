// <copyright file="BlockProperties.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedControls.Abstract;

namespace LargoSharedPanels
{
    /// <summary>
    /// Block Properties.
    /// </summary>
    public partial class BlockProperties
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockProperties" /> class.
        /// </summary>
        public BlockProperties()
        {
            this.InitializeComponent();

            //// this.Focusable = true;
            //// this.textBlock1.IsEnabled = true;
            this.Identification = new List<KeyValuePair>();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets FileModel.
        /// </summary>
        /// <value> Property description. </value>
        public IList<KeyValuePair> Identification { get; }

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        public MusicalBlock Block { get; private set; }
        #endregion

        #region Load

        /// <summary>
        /// Loads the block editor.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public void LoadBlock(MusicalBlock givenBlock) //// BlockModel givenModel
        {
            if (givenBlock == null)
            {
                return;
            }

            this.Block = givenBlock;
            //// this.textBlock1.Text = givenBlock.FullName;
            //// this.textBlock1.ToolTip = givenBlock.FullName;
            this.AddIdentification(givenBlock.Identification);

            ///// 2018/10   if (givenModel != null)  { this.AddIdentification(givenModel.Identification);  } 
            this.Localize();
        }

        #endregion

        /// <summary>
        /// Loads data..
        /// </summary>
        /// <param name="givenIdentification">The given identification.</param>
        public void AddIdentification(IEnumerable<KeyValuePair> givenIdentification)
        {
            this.DataGridIdentification.ItemsSource = null;
            foreach (var pair in givenIdentification)
            {
                var item = (from ident in this.Identification where ident.Key == pair.Key select ident).FirstOrDefault();
                if (item == null)
                {
                    this.Identification.Add(pair);
                }
                else
                {
                    item.Value = pair.Value;
                }
            }

            this.DataGridIdentification.ItemsSource = this.Identification;
        }

        #region Other private methods
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize()
        {
            CultureMaster.Localize(this);
        }

        #endregion
    }
}
