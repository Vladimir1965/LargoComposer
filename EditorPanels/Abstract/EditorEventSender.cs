// <copyright file="EditorEventSender.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Abstract
{
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Music;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Editor Event Sender.
    /// </summary>
    public class EditorEventSender {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly EditorEventSender InternalSingleton = new EditorEventSender();

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="EditorEventSender" /> class from being created.
        /// </summary>
        private EditorEventSender() {
        }
        #endregion

        #region Model Events

        /// <summary>
        /// Occurs when [editor element changed].
        /// </summary>
        public event EventHandler<EditorEventArgs> EditorChanged;

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static EditorEventSender Singleton {
            get {
                Contract.Ensures(Contract.Result<EditorEventSender>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Editor Event Sender is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "EditorEventSender";
        }
        #endregion

        #region Send Events
        /// <summary>
        /// Sends the editor changed event.
        /// </summary>
        /// <param name="givenElement">The given element.</param>
        public void SendEditorChangedEvent(MusicalElement givenElement) {
            var command = this.EditorChanged;
            command?.Invoke(this, new EditorEventArgs(givenElement));
        }

        /// <summary>
        /// Sends the editor changed event.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public void SendEditorChangedEvent(IAbstractBar givenBar) {
            var command = this.EditorChanged;
            command?.Invoke(this, new EditorEventArgs(givenBar));
        }

        /// <summary>
        /// Sends the editor changed event.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        public void SendEditorChangedEvent(IAbstractLine givenLine) {
            var command = this.EditorChanged;
            command?.Invoke(this, new EditorEventArgs(givenLine));
        }

        #endregion
    }
}
