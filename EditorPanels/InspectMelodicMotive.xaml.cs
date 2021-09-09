// <copyright file="InspectMelodicMotive.xaml.cs" company="Traced-Ideas, Czech republic">
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
    /// Interact logic for InspectMelodicMotive.
    /// </summary>
    public partial class InspectMelodicMotive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectMelodicMotive"/> class.
        /// </summary>
        public InspectMelodicMotive()
        {
            this.InitializeComponent();
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
        }

        /// <summary>
        /// Rebind Melodic Motive Structures.
        /// </summary>
        /// <param name="melodicMotive">The melodic motive.</param>
        public void DisplayMotive(MelodicMotive melodicMotive)
        {
            if (melodicMotive == null) {
                this.DataGridStructures.ItemsSource = null;
                return;
            }

            this.DataGridStructures.ItemsSource = melodicMotive.MelodicStructures;
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

            this.DisplayMotive(element.Status.MelodicMotive);
        }
        #endregion
    }
}
