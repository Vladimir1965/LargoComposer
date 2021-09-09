// <copyright file="MetaAbstractText.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary>Represents a text meta event message.</summary>
    [Serializable]
    public abstract class MetaAbstractText : MetaEvent {
        #region Fields
        /// <summary>The text associated with the event.</summary>
        private string text;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaAbstractText class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenMetaEventId">The ID of the meta event.</param>
        /// <param name="text">The text associated with the event.</param>
        protected MetaAbstractText(long deltaTime, byte givenMetaEventId, string text) :
            base(deltaTime, givenMetaEventId) {
            this.text = text;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the text associated with this event.</summary>
        /// <value> General musical property.</value>
        public string Text {
            get => this.text;

            [UsedImplicitly]
            set => this.text = value ?? throw new ArgumentNullException(nameof(value));
        }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            if (this.Text != null) {
                sb.Append(this.Text.ToString(CultureInfo.CurrentCulture));
            }

            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>Write the event to the output stream.</summary>
        /// <param name="outputStream">The stream to which the event should be written.</param>
        public override void Write(Stream outputStream) {
            if (outputStream == null) {
                return;
            } 
            //// Write out the base event information
            base.Write(outputStream);
            if (this.text == null)
            {
                return;
            }

            //// Special meta event marker and the id of the event
            var asciiBytes = Encoding.ASCII.GetBytes(this.text);
            MidiEvent.WriteVariableLength(outputStream, asciiBytes.Length);
            outputStream.Write(asciiBytes, 0, asciiBytes.Length);
        }
        #endregion
    }
}