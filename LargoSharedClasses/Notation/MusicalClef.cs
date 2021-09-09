// <copyright file="MusicalClef.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;

namespace LargoSharedClasses.Notation
{
    /// <summary>
    /// Musical Clef.
    /// </summary>
    public class MusicalClef {
        #region Private fields
        /// <summary>
        /// Type Of Clef.
        /// </summary>
        private readonly ClefType typeOfClef;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalClef"/> class.
        /// </summary>
        /// <param name="clefType">Type of the clef.</param>
        /// <param name="whichLine">The which line.</param>
        public MusicalClef(ClefType clefType, int whichLine) {
            //// noteHeight = new NoteHeight("G", 0, 4);
            this.typeOfClef = clefType;
            this.Line = whichLine;
            this.ClefPitch = ToClefMidiPitch(this.typeOfClef);

            switch (this.typeOfClef)
            {
                case ClefType.GClef:
                    this.Height = new NoteHeight("G", 0, 4);
                    break;
                case ClefType.FClef:
                    this.Height = new NoteHeight("F", 0, 3);
                    break;
                case ClefType.CClef:
                    this.Height = new NoteHeight("C", 0, 4);
                    break;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the Note Height.
        /// </summary>
        /// <value> Property description. </value>
        public NoteHeight Height { get; }

        /// <summary>
        /// Gets the type of clef.
        /// </summary>
        /// <value> Property description. </value>
        public ClefType TypeOfClef => this.typeOfClef;

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value> Property description. </value>
        public int Line { get; }

        /// <summary>
        /// Gets the clef pitch.
        /// </summary>
        /// <value> Property description. </value>
        public int ClefPitch { get; }

        #endregion

        #region Public static methods
        /// <summary>
        /// Suggests the clef for A note.
        /// </summary>
        /// <param name="givenMidiPitch">The given midi pitch.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public static MusicalClef SuggestClefForMidiNote(int givenMidiPitch)
        {
            const int breakMidiPitch = 60;     //// 55
            return givenMidiPitch < breakMidiPitch ? new MusicalClef(ClefType.FClef, 4) : new MusicalClef(ClefType.GClef, 2);
        }

        /// <summary>
        /// Suggests the clef for A note.
        /// </summary>
        /// <param name="givenMidiPitch">The given midi pitch.</param>
        /// <param name="currentClef">The current clef.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public static MusicalClef SuggestClefForMidiNote(int givenMidiPitch, MusicalClef currentClef)
        {
            switch (currentClef.TypeOfClef)
            {
                case ClefType.GClef:
                    if ((currentClef.Line == 1) && (givenMidiPitch < 59)) {
                        return new MusicalClef(ClefType.FClef, 4);
                    }

                    if ((currentClef.Line == 2) && (givenMidiPitch < 55)) {
                        return new MusicalClef(ClefType.FClef, 4);
                    }

                    if ((currentClef.Line == 3) && (givenMidiPitch < 52)) {
                        return new MusicalClef(ClefType.FClef, 4);
                    }

                    if ((currentClef.Line == 4) && (givenMidiPitch < 48)) {
                        return new MusicalClef(ClefType.FClef, 4);
                    }

                    if ((currentClef.Line == 5) && (givenMidiPitch < 45)) {
                        return new MusicalClef(ClefType.FClef, 4);
                    }

                    return currentClef;

                case ClefType.FClef:
                    if ((currentClef.Line == 1) && (givenMidiPitch > 74)) {
                        return new MusicalClef(ClefType.GClef, 2);
                    }

                    if ((currentClef.Line == 2) && (givenMidiPitch > 71)) {
                        return new MusicalClef(ClefType.GClef, 2);
                    }

                    if ((currentClef.Line == 3) && (givenMidiPitch > 67)) {
                        return new MusicalClef(ClefType.GClef, 2);
                    }

                    if ((currentClef.Line == 4) && (givenMidiPitch > 64)) {
                        return new MusicalClef(ClefType.GClef, 2);
                    }

                    if ((currentClef.Line == 5) && (givenMidiPitch > 60)) {
                        return new MusicalClef(ClefType.GClef, 2);
                    }
                    
                    return currentClef;

                default:
                    return new MusicalClef(ClefType.GClef, 2);
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("Clef {0} {1} Pitch {2}", this.TypeOfClef, this.Height, this.ClefPitch);

            return s.ToString();
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Toes the clef midi pitch.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns> Returns value. </returns>
        private static int ToClefMidiPitch(ClefType type) {
            switch (type) {
                case ClefType.CClef:
                    return 60;
                case ClefType.FClef:
                    return 53;
                case ClefType.GClef:
                    return 67;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
