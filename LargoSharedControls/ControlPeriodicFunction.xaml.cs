// <copyright file="ControlPeriodicFunction.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using System;

namespace LargoSharedControls
{
    /// <summary>
    /// Dialog Box Change String.
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class ControlPeriodicFunction 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPeriodicFunction" /> class.
        /// </summary>
        public ControlPeriodicFunction() {
            this.InitializeComponent();
            this.comboFunction.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets the period.
        /// </summary>
        /// <value>
        /// The period.
        /// </value>
        public int Period => (int)this.sliderPeriod.Value;

        /// <summary>
        /// Gets the phase.
        /// </summary>
        /// <value>
        /// The phase.
        /// </value>
        public int Phase => (int)this.sliderPhase.Value;

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public PeriodicFunction Function => (PeriodicFunction)this.comboFunction.SelectedIndex;

        /// <summary>
        /// Gets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public int Base => (int)this.sliderBase.Value;

        /// <summary>
        /// Gets the plan function.
        /// </summary>
        /// <value>
        /// The plan function.
        /// </value>
        public PlanFunction PlanFunction {
            get {
                var a = Math.Min(this.Base, 100 - this.Base);
                var f = new PlanFunction(this.Function, this.Period, this.Phase, this.Base, a);
                return f;
            }
        }

        /// <summary>
        /// Bases the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void BaseChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            if (this.txtBase != null) {
                this.txtBase.Text = ((int)e.NewValue).ToString();
            }
        }

        /// <summary>
        /// Periods the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void PeriodChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            if (this.txtPeriod != null) {
                this.txtPeriod.Text = ((int)e.NewValue).ToString();
            }
        }

        /// <summary>
        /// Phases the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void PhaseChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            if (this.txtPhase != null) {
                this.txtPhase.Text = ((int)e.NewValue).ToString();
            }
        }
    }
}