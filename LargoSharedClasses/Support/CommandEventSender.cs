// <copyright file="CommandEventSender.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System;
using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Command Event Sender.
    /// </summary>
    public class CommandEventSender {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly CommandEventSender InternalSingleton = new CommandEventSender();

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CommandEventSender" /> class from being created.
        /// </summary>
        private CommandEventSender() {
        }
        #endregion

        #region File Events
        /// <summary>
        /// Occurs when [block selected].
        /// </summary>
        public event EventHandler<BundleEventArgs> BundleLoaded;

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static CommandEventSender Singleton {
            get {
                Contract.Ensures(Contract.Result<CommandEventSender>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Command Event Sender is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Properties
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "CommandEventSender";
        }
        #endregion

        #region Send Events

        /// <summary>
        /// Sends the bundle event.
        /// </summary>
        /// <param name="givenBundle">The given bundle.</param>
        /// <param name="givenOperation">The given operation.</param>
        public void SendBundleEvent(MusicalBundle givenBundle, ObjectOperation givenOperation)
        {
            if (givenBundle == null)
            {
                return;
            }
            
            var command = givenOperation == ObjectOperation.ObjectLoaded ? this.BundleLoaded : null;
            command?.Invoke(this, new BundleEventArgs(givenBundle, givenOperation, false));
        }

        #endregion
    }
}
