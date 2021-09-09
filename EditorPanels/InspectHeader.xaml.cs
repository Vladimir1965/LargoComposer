// <copyright file="InspectHeader.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedControls.Abstract;

namespace EditorPanels
{
    /// <summary>
    /// Panel Musical Score.
    /// </summary>
    public sealed partial class InspectHeader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectHeader"/> class. 
        /// </summary>
        public InspectHeader() {
            this.InitializeComponent();
        }

        #endregion

        #region Properties
  
        #endregion

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            this.Localize();
        }

        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }
    }
}