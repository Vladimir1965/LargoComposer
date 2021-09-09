// <copyright file="ControlChannel.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls {
    using LargoSharedClasses.Music;

    /// <summary>
    /// UCMid Channel.
    /// </summary>
    public sealed partial class ControlChannel {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlChannel"/> class.
        /// </summary>
        public ControlChannel() {
            this.InitializeComponent();
            this.Combo = this.ComboBoxChannel;
            this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData() {
            this.Combo.ItemsSource = DataEnums.ListMidChannel;
        }
    }
}