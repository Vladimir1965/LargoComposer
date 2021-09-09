// <copyright file="ControlCulture.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls {
    using LargoSharedClasses.Music;

    /// <summary>
    /// UC Culture.
    /// </summary>
    public sealed partial class ControlCulture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlCulture"/> class.
        /// </summary>
        public ControlCulture() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
            this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData() {
            var list = DataEnums.ListUiCulture;
            this.ComboObject.ItemsSource = list;
            if (list.Count > 0 && this.Combo.SelectedIndex < 0) {
                this.Combo.SelectedIndex = 0; //// 1
            }
        }
    }
}