// <copyright file="ControlMelodicShape.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls {
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;

    /// <summary>
    /// Control Octave.
    /// </summary>
    public sealed partial class ControlMelodicShape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMelodicShape"/> class.
        /// </summary>
        public ControlMelodicShape() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
            this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        [UsedImplicitly]
        public void LoadData() {
            this.ComboObject.ItemsSource = DataEnums.ListMelodicShape;
        }
    }
}