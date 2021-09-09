// <copyright file="ControlHarmonicSystem.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace ConductorControls {
    using System.Collections.Generic;
    using LargoSharedClasses.Abstract;

    /// <summary>
    /// ControlHarmonic System. //// sealed
    /// </summary>
    public partial class ControlHarmonicSystem { 
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlHarmonicSystem"/> class.
        /// </summary>
        public ControlHarmonicSystem() {
            this.InitializeComponent();
            this.Combo = this.ComboObject;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="systems">The systems.</param>
        public void LoadData(IEnumerable<KeyValuePair> systems) {
            this.ComboObject.ItemsSource = systems; //// ListHarmonicSystemOutline;
            this.SelectItemKey("12");
        }
    }
}