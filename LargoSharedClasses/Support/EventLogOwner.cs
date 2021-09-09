// <copyright file="EventLogOwner.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Support {
    /// <summary>
    /// Musical Director.
    /// </summary>
    public sealed class EventLogOwner {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly EventLogOwner InternalSingleton = new EventLogOwner();

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the EventLogOwner class from being created.
        /// </summary>
        private EventLogOwner() {
            this.EventLog = new EventLog { Source = "LARGO", Log = "LARGO_Log" };
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static EventLogOwner Singleton {
            get {
                Contract.Ensures(Contract.Result<EventLogOwner>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton EventLogOwner is null.");
                }

                return InternalSingleton; 
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether [play immediately].
        /// </summary>
        /// <value>
        /// <c>True</c> if [play immediately]; otherwise, <c>false</c>.
        /// </value>
        private EventLog EventLog { get;  }

        #endregion

        /// <summary>
        /// Writes the entry.
        /// </summary>
        /// <param name="givenText">The given text.</param>
        [UsedImplicitly]
        public void WriteEntry(string givenText) {
            ////201509Security 
            this.EventLog.WriteEntry(givenText);
        }

        /// <summary>
        /// Writes the exception.
        /// </summary>
        /// <param name="givenText">The given text.</param>
        /// <param name="exception">The exception.</param>
        [UsedImplicitly]
        public void WriteException(string givenText, Exception exception) {
            ////201509Security 
            this.EventLog.WriteEntry(givenText + exception.ListError());
        }
    }
}
