// <copyright file="MainManager.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;

namespace LargoPlayer
{
    /// <summary> Manager for single instances. This class cannot be inherited. </summary>
    /// <remarks> SingleInstanceManager written by 'Jig Neshon' (India). </remarks>
    public sealed class MainManager : WindowsFormsApplicationBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainManager"/> class.
        /// Main single - instance manager.
        /// </summary>
        public MainManager() {
            this.IsSingleInstance = true;
        }

        #endregion

        #region Properties

        /// <summary> Gets the application. </summary>
        /// <value> The application. </value>
        public MainApplication App { get; private set; }

        #endregion

        #region Methods
        /// <summary> Main entry-point for this application. </summary>
        /// <param name="args"> An array of command-line argument strings. </param>
        [STAThread]
        public static void Main(string[] args) {
            (new MainManager()).Run(args);
        }

        /// <summary>
        /// When overridden in a derived class, allows for code to run when the application starts.
        /// </summary>
        /// <param name="e"> <see cref="T:Microsoft.VisualBasic.ApplicationServices.StartupEventArgs" />.
        ///                  Contains the command-line arguments of the application and indicates whether
        ///                  the application startup should be canceled. </param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> that indicates if the application should continue starting up. 
        /// </returns>
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e) {
            this.App = new MainApplication();
            this.App.Run();
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, allows for code to run when a subsequent instance of a
        /// single-instance application starts.
        /// </summary>
        /// <param name="eventArgs"> <see cref="T:Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs" />.
        /// Contains the command-line arguments of the subsequent application
        /// instance and indicates whether the first application instance should
        /// be brought to the foreground upon exiting the exception handler. </param>
        protected override void OnStartupNextInstance(
            StartupNextInstanceEventArgs eventArgs) {
            base.OnStartupNextInstance(eventArgs);
            this.App.Window.Activate();
            this.App.ProcessArgs(eventArgs.CommandLine.ToArray(), false);
            this.App.Window.LoadFiles();
        }

        #endregion
    }
}
