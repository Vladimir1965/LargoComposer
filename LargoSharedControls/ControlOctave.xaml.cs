// <copyright file="ControlOctave.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls {
    using LargoSharedClasses.Music;

    /// <summary>
    /// Control Octave.
    /// </summary>
    public sealed partial class ControlOctave {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlOctave"/> class.
        /// </summary>
        public ControlOctave() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
            this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public void LoadData() {
            this.ComboObject.ItemsSource = DataEnums.ListMusicalOctave;
        }
    }
}