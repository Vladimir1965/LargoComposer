// <copyright file="InspectRhythmicMotive.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
namespace EditorPanels
{
    using EditorPanels.Abstract;
    using LargoSharedClasses.Models;

    /// <summary>
    /// Interact logic for InspectRhythmicMotive.
    /// </summary>
    public partial class InspectRhythmicMotive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectRhythmicMotive"/> class.
        /// </summary>
        public InspectRhythmicMotive()
        {
            this.InitializeComponent();
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
        }

        /// <summary>
        /// Rebind Melodic Motive Structures.
        /// </summary>
        /// <param name="rhythmicMotive">The rhythmic motive.</param>
        private void DisplayMotive(RhythmicMotive rhythmicMotive)
        {
            if (rhythmicMotive == null) {
                this.DataGridStructures.ItemsSource = null;
                return;
            }

            this.DataGridStructures.ItemsSource = rhythmicMotive.RhythmicStructures;
            this.DataGridStructures.Items.Refresh();
        }

        #region Listeners

        /// <summary>
        /// Musicals the element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args)
        {
            var element = args.Element;
            if (element == null) {
                return;
            }

            this.DisplayMotive(element.Status.RhythmicMotive);
        }
        #endregion
    }
}
