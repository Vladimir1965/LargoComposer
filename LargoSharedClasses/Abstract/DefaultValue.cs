// <copyright file="DefaultValue.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Default Value.
    /// </summary>
    public static class DefaultValue {
        #region General constanst
        /// <summary>
        /// After Zero.
        /// </summary>
        public const float AfterZero = 0.00001f;

        /// <summary>
        /// Eight Unit.
        /// </summary>
        public const float EightUnit = 0.125f;

        /// <summary>
        /// Quarter Unit.
        /// </summary>
        public const float QuarterUnit = 0.25f;

        /// <summary>
        /// Half Unit.
        /// </summary>
        public const float HalfUnit = 0.5f;

        /// <summary>
        /// Numeric Unit.
        /// </summary>
        public const float Unit = 1.0f;

        /// <summary>
        /// Number Fifty.
        /// </summary>
        public const byte Fifty = 50;

        /// <summary>
        /// Large number.
        /// </summary>
        public const int LargeNumber = 10000;
        #endregion 
        
        #region Music constanst
        /// <summary>
        /// Maximum Bar Number.
        /// </summary>
        public const int MaximumBarNumber = 10000;

        /// <summary>
        /// Default order of Harmonic System.
        /// </summary>
        public const byte HarmonicOrder = 12;

        /// <summary>
        /// The rhythmic order
        /// </summary>
        public const byte RhythmicOrder = 12;

        /// <summary>
        /// Default Rhythmic Order.
        /// </summary>
        public const byte MaximumRhythmicOrder = 240;  ////120 in Music Xml was used limit 250

        /// <summary>
        /// Default Tempo.
        /// </summary>
        public const byte DefaultTempo = 120;

        /// <summary>
        /// Mean Note Altitude.
        /// </summary>
        public const byte MeanNoteAltitude = 60;

        /// <summary>
        /// The lowest note
        /// </summary>
        public const byte LowestNote = 8;

        /// <summary>
        /// The highest note
        /// </summary>
        public const byte HighestNote = 120;

        #endregion

        #region Hexadecimal masks
        /// <summary>
        /// One byte (length in bits).
        /// </summary>
        public const byte OneByte = 8;

        /// <summary>
        /// Two bytes.
        /// </summary>
        public const byte TwoBytes = 16;

        /// <summary>
        /// Three bytes.
        /// </summary>
        public const byte ThreeBytes = 24;

        /// <summary>
        /// Mask first byte.
        /// </summary>
        public const int MaskFirstByte = 0xFF00;

        /// <summary>
        /// Mask second byte.
        /// </summary>
        public const int MaskSecondByte = 0x00FF;

        /// <summary>
        /// Mask first byte of four.
        /// </summary>
        public const long MaskByte1Of4 = 0xFF000000;

        /// <summary>
        /// Mask second byte of four.
        /// </summary>
        public const long MaskByte2Of4 = 0x00FF0000;

        /// <summary>
        /// Mask third byte of four.
        /// </summary>
        public const long MaskByte3Of4 = 0x0000FF00;

        /// <summary>
        /// Mask fourth byte of four.
        /// </summary>
        public const long MaskByte4Of4 = 0x000000FF;
        #endregion
    }
}