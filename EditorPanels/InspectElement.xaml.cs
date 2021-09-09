// <copyright file="InspectElement.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedControls.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace EditorPanels
{
    /// <summary>
    /// Panel Musical Score.
    /// </summary>
    public sealed partial class InspectElement {
        #region Fields

        /// <summary>
        /// Musical element.
        /// </summary>
        private MusicalElement element;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectElement"/> class. 
        /// </summary>
        public InspectElement() {
            this.InitializeComponent();
            //// MenuEventSender.Singleton.FileModelLoaded += this.FileModelLoaded;
            EditorEventSender.Singleton.EditorChanged += this.EditorChanged;
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the editor line.
        /// </summary>
        /// <value>
        /// The editor line.
        /// </value>
        public IAbstractLine Line { get; set; }

        /// <summary>
        /// Gets the main identifiers.
        /// </summary>
        /// <value>
        /// The main identifiers.
        /// </value>
        public IList<KeyValuePair> Identifiers { get; private set; }

        #endregion

        #region Private properties
        /// <summary>
        /// Gets or sets the musical element.
        /// </summary>
        /// <value>
        /// The musical element.
        /// </value>
        private MusicalElement Element {
            [UsedImplicitly] get => this.element;

            set {
                this.element = value;
                if (this.element == null) {
                    this.ResetEditorElement();
                    return;
                }

                this.DisplayEditorElement();
            }
        }

        /// <summary>
        /// Gets or sets the musical element.
        /// </summary>
        /// <value>
        /// The musical element.
        /// </value>
        private IAbstractBar Bar { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            this.Localize();
        }

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

        #endregion

        #region Private methods

        /// <summary>
        /// Displays the editor element.
        /// </summary>
        private void DisplayEditorElement() {
            if (this.element == null) {
                return;
            }

            var status = this.element.Status;

            if (this.element.Status.Instrument == null) {
                this.element.Status.Instrument = new MusicalInstrument(MidiMelodicInstrument.None);
            }

            bool isMelodic = status.LineType == MusicalLineType.Melodic;
        }

        /// <summary>
        /// Reset the editor element.
        /// </summary>
        private void ResetEditorElement() {
            this.DataGridIdentification.ItemsSource = null;
        }
        #endregion

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
        private void RefreshTopTitle()
        {
            this.TextTopTitle.Text = string.Format(
                "Line: {0,2} Bar: {1,4}", this.Line?.LineNumber, this.Bar?.BarNumber);
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
            this.Element = args.Element;
            this.Line = args.Line;
            this.Bar = args.Bar;

            this.RefreshTopTitle();
        }
        #endregion
    }
}