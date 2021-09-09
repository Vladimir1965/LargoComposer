// <copyright file="ControlLoudness.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;

namespace LargoSharedControls {
    /// <summary>
    /// Control Loudness.
    /// </summary>
    public sealed partial class ControlLoudness {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlLoudness"/> class.
        /// </summary>
        public ControlLoudness() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
            this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public void LoadData() {
            this.ComboObject.ItemsSource = DataEnums.ListMusicalLoudness;
        }
    }
}