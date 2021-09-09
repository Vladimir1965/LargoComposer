// <copyright file="IMidiEvent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.IO;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// I Midi Event.
    /// </summary>
    public interface IMidiEvent
    {
        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        int BarNumber { get; set; }

        /// <summary>
        /// Gets or sets the delta time.
        /// </summary>
        /// <value>
        /// The delta time.
        /// </value>
        long DeltaTime { get; set; }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        string EventType { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is meta event.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is meta event; otherwise, <c>false</c>.
        /// </value>
        bool IsMetaEvent { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is voice note event.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is voice note event; otherwise, <c>false</c>.
        /// </value>
        bool IsVoiceNoteEvent { get; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        long StartTime { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns value.</returns>
        IMidiEvent Clone();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();

        /// <summary>
        /// Writes the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        void Write(Stream outputStream);
    }
}