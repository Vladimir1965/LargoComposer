// <copyright file="MainApplication.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoManager
{
    using System.Windows;

    /// <summary> A main application. </summary>
    public class MainApplication : Application
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplication" /> class.
        /// Main single - instance application.
        /// </summary>
        public MainApplication() {
        }
        #endregion

        #region Public static properties
        /// <summary>
        /// Gets or sets Parameter0.
        /// </summary>
        /// <value> Property description. </value>
        public static string Parameter0 { get; set; }

        /// <summary>
        /// Gets or sets Parameter1.
        /// </summary>
        /// <value> Property description. </value>
        public static string Parameter1 { get; set; }
        #endregion

        #region Public properties

        /// <summary> Gets the window. </summary>
        /// <value> my window. </value>
        public MainWindow Window { get; private set; }

        #endregion

        #region Dispatcher for Unhandled Exceptions
        /// <summary>
        /// Gets or sets a value indicating whether Boolean property to determine if we need to handle the exception or not.
        /// </summary>
        /// <value> Property description. </value>
        public bool DoHandle { get; set; }
        #endregion

        #region Methods

        /// <summary> Process the arguments. </summary>
        /// <param name="args"> The arguments. </param>
        /// <param name="firstInstance"> True to first instance. </param>
        public void ProcessArgs(string[] args, bool firstInstance) {
            //// Process Command Line Arguments Here
            if (args.Length > 0) {
                Parameter0 = args[0];
            }

            if (args.Length > 1) {
                Parameter1 = args[1];
            }
        }

        /// <summary> Raises the <see cref="E:System.Windows.Application.Startup" /> event. </summary>
        /// <param name="e"> A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event
        /// data. </param>
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            this.Window = new MainWindow();
            this.ProcessArgs(e.Args, true);
            //// this.Window.LoadFiles();
            this.Window.Show();

            // define application exception handler
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
        }

        #endregion

        #region Dispatcher for Unhandled Exceptions

        /// <summary>
        /// CurrentDomain UnhandledException.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Exception exception = e.ExceptionObject as Exception;
            var w = this.Window;
            if (w != null) {
                //// !?!? If you use a separate thread, it needs to be in a STA (single-threaded apartment), which is not the case for background worker threads. 
                //// You have to create the thread yourself, like this:
                ////     Thread t = new Thread(ThreadProc);
                ////     t.SetApartmentState(ApartmentState.STA);
                ////     t.Start();
                //// w.OpenMainException(exception);
            }

            if (exception != null) {
                MessageBox.Show(exception.Message, "Thread Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// The DispatcherUnhandledException is called whenever the UI thread of the application 
        /// generated an unhandled exception.
        /// </summary>
        /// <param name="sender">Method sender.</param>
        /// <param name="e">Caught Exception.</param>
        [UsedImplicitly]
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            this.DoHandle = true;
            if (this.DoHandle) {
                //// Handling the exception within the UnhandledException handler.
                //// Exception exception = e.Exception;
                /* UnhandledException !?
                 2016/10
                MainWindow w = App.LargoMainWindow;
                if (w != null) {
                    w.OpenMainException(exception);
                }
                else {
                    MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                */
                e.Handled = true;
            }
            else {
                //// If you do not set e.Handled to true, the application will close due to crash.
                MessageBox.Show("Application is going to close! ", "Uncaught Exception");
                e.Handled = false;
            }
        }

        #endregion
    }
}
