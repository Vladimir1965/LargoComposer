// <copyright file="ControlMusicalNote.xaml.cs" company="Traced-Ideas, Czech republic">
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
    public sealed partial class ControlMusicalNote
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMusicalNote"/> class.
        /// </summary>
        public ControlMusicalNote() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
            //// this.LoadData();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="variant">The variant.</param>
        public void LoadData(byte variant) {
            switch (variant) {
                case 1: {
                    this.ComboObject.ItemsSource = DataEnums.ListNotes(0, 23);
                    }

                    break;
                case 2: {
                    this.ComboObject.ItemsSource = DataEnums.ListNotes(104, 127);
                    }

                    break;
                default: {
                    this.ComboObject.ItemsSource = DataEnums.ListNotes(0, 127);
                    }

                    break;
            }
        }
    }
}