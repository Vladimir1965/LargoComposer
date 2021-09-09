// <copyright file="InspectorWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;

namespace EditorWindows
{
    /// <summary>
    /// Interaction logic for Inspector Window.
    /// </summary>
    public partial class InspectorWindow 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorWindow"/> class.
        /// </summary>
        public InspectorWindow() {
            this.InitializeComponent();
            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
        }
    }
}
