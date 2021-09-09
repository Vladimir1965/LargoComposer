// <copyright file="InspectLine.xaml.cs" company="Traced-Ideas, Czech republic">
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
    /// Interact logic for InspectLine.
    /// </summary>
    public partial class InspectLine 
    {
        #region Fields

        /// <summary>
        /// Musical line.
        /// </summary>
        private IAbstractLine line;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectLine"/> class.
        /// </summary>
        public InspectLine()
        {
            this.InitializeComponent();
            //// this.uCMidChannel1.SelectItemNumericKey(0, false);
            //// this.uCMidChannel1.SelectItemNumericKey((int?)channel, false);
            //// this.uCMidChannel1.SelectItemNumericKey(midChannel.Count == 1 ? (int?)midChannel[0] : null, true);
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
            this.Localize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public IAbstractLine Line
        {
            get => this.line;

            set => this.line = value;
        }

        /// <summary>
        /// Gets the main identifiers.
        /// </summary>
        /// <value>
        /// The main identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers { get; private set; }
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
                "Line: {0,2}", this.Line?.LineNumber);
        }

        #endregion

        #region Private methods - Events

        /// <summary>
        /// Editors the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EditorEventArgs"/> instance containing the event data.</param>
        private void EditorChanged(object sender, EditorEventArgs args)
        {
            this.Line = args.Line;

            this.RefreshTopTitle();
            //// var channel = this.Line.Status.Channel;
            //// this.uCMidChannel1.SelectItemNumericKey((int?)channel, false); 
        }

        #endregion
    }
}
