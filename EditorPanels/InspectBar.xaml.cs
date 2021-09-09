// <copyright file="InspectBar.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels
{
    using EditorPanels.Abstract;
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedControls.Abstract;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interact logic for InspectBar.
    /// </summary>
    public partial class InspectBar 
    {
        #region Fields

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectBar"/> class.
        /// </summary>
        public InspectBar()
        {
            this.InitializeComponent();
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the main identifiers.
        /// </summary>
        /// <value>
        /// The main identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers { get; private set; }

        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
        private IAbstractBar Bar { get; set; }

        #endregion

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void ResetIdentifiers() {
            this.Identifiers = new List<KeyValuePair>();
        }

        /// <summary>
        /// Adds the main identifiers.
        /// </summary>
        /// <param name="givenIdentifiers">The given identifiers.</param>
        public void AddMainIdentifiers(IEnumerable<KeyValuePair> givenIdentifiers) {
            this.DataGridIdentification.ItemsSource = null;
            foreach (var pair in givenIdentifiers) {
                var item = (from ident in this.Identifiers where ident.Key == pair.Key select ident).FirstOrDefault();
                if (item == null) {
                    this.Identifiers.Add(pair);
                }
                else {
                    item.Value = pair.Value;
                }
            }

            this.DataGridIdentification.ItemsSource = this.Identifiers;
        }

        #region Private methods
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        private void Localize() {
            CultureMaster.Localize(this);
        }

        /// <summary>
        /// Refreshes the top title.
        /// </summary>
        private void RefreshTopTitle() {
            this.TextTopTitle.Text = string.Format(
                "Bar: {0,4}", this.Bar?.BarNumber);
        }

        #endregion

        #region Listeners

        /// <summary>
        /// Musicals the element changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args)
        {
            this.Bar = args.Bar;
            this.RefreshTopTitle();
        }

        #endregion
    }
}
