// <copyright file="EditorInspector.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using EditorPanels.Abstract;
using System.Diagnostics.Contracts;
using System.Windows;

namespace EditorPanels
{
    /// <summary>
    /// Interact logic for EditorInspector.
    /// </summary>
    public partial class EditorInspector
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static EditorInspector singleton;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorInspector"/> class.
        /// </summary>
        public EditorInspector()
        {
            this.InitializeComponent();
            singleton = this;
            this.InspectBar = new InspectBar();
            this.ContentGrid.Children.Add(this.InspectBar);

            this.InspectLine = new InspectLine();
            this.ContentGrid.Children.Add(this.InspectLine);

            this.InspectElement = new InspectElement();
            this.ContentGrid.Children.Add(this.InspectElement);

            this.InspectTones = new InspectTones();
            this.ContentGrid.Children.Add(this.InspectTones);

            this.InspectRhythmicMotive = new InspectRhythmicMotive();
            this.ContentGrid.Children.Add(this.InspectRhythmicMotive);

            this.InspectMelodicMotive = new InspectMelodicMotive();
            this.ContentGrid.Children.Add(this.InspectMelodicMotive);

            this.EnablePanel(0);
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the InspectorHeader Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static EditorInspector Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<EditorInspector>() != null);
                if (singleton == null) {
                    singleton = new EditorInspector();
                    //// throw new InvalidOperationException("Singleton EditorInspector is null.");
                }

                return singleton;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the inspect bar.
        /// </summary>
        /// <value>
        /// The inspect bar.
        /// </value>
        public InspectBar InspectBar { get; set; }

        /// <summary>
        /// Gets or sets the inspect line.
        /// </summary>
        /// <value>
        /// The inspect line.
        /// </value>
        public InspectLine InspectLine { get; set; }

        /// <summary>
        /// Gets or sets the inspect element.
        /// </summary>
        /// <value>
        /// The inspect element.
        /// </value>
        public InspectElement InspectElement { get; set; }

        /// <summary>
        /// Gets or sets the inspect tones.
        /// </summary>
        /// <value>
        /// The inspect tones.
        /// </value>
        public InspectTones InspectTones { get; set; }

        /// <summary>
        /// Gets or sets the inspect rhythmic motive.
        /// </summary>
        /// <value>
        /// The inspect rhythmic motive.
        /// </value>
        public InspectRhythmicMotive InspectRhythmicMotive { get; set; }

        /// <summary>
        /// Gets or sets the inspect melodic motive.
        /// </summary>
        /// <value>
        /// The inspect melodic motive.
        /// </value>
        public InspectMelodicMotive InspectMelodicMotive { get; set; }
        #endregion

        /// <summary>
        /// Enables the panel.
        /// </summary>
        /// <param name="givenInspector">The given inspector.</param>
        public void EnablePanel(InspectorType givenInspector)
        {
            this.InspectBar.IsEnabled = false;
            this.InspectBar.Visibility = Visibility.Hidden;
            this.InspectLine.IsEnabled = false;
            this.InspectLine.Visibility = Visibility.Hidden;
            this.InspectElement.IsEnabled = false;
            this.InspectElement.Visibility = Visibility.Hidden;
            this.InspectTones.IsEnabled = false;
            this.InspectTones.Visibility = Visibility.Hidden;
            this.InspectRhythmicMotive.IsEnabled = false;
            this.InspectRhythmicMotive.Visibility = Visibility.Hidden;
            this.InspectMelodicMotive.IsEnabled = false;
            this.InspectMelodicMotive.Visibility = Visibility.Hidden;

            if (givenInspector == InspectorType.Bar)
            {
                this.InspectBar.IsEnabled = true;
                this.InspectBar.Visibility = Visibility.Visible;
            }

            if (givenInspector == InspectorType.Line) {
                this.InspectLine.IsEnabled = true;
                this.InspectLine.Visibility = Visibility.Visible;
            }

            if (givenInspector == InspectorType.Element) {
                this.InspectElement.IsEnabled = true;
                this.InspectElement.Visibility = Visibility.Visible;
            }

            if (givenInspector == InspectorType.Tones) {
                this.InspectTones.IsEnabled = true;
                this.InspectTones.Visibility = Visibility.Visible;
            }

            if (givenInspector == InspectorType.RhythmicMotive) {
                this.InspectRhythmicMotive.IsEnabled = true;
                this.InspectRhythmicMotive.Visibility = Visibility.Visible;
            }

            if (givenInspector == InspectorType.MelodicMotive) {
                this.InspectMelodicMotive.IsEnabled = true;
                this.InspectMelodicMotive.Visibility = Visibility.Visible;
            }
        }
    }
}
