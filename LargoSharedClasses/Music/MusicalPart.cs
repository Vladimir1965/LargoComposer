// <copyright file="MusicalPart.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Musical Part.
    /// </summary>
    public sealed class MusicalPart {
        #region Fields
        /// <summary>
        /// Musical Block.
        /// </summary>
        private MusicalBlock musicalBlock;

        /// <summary>
        /// Musical Track.
        /// </summary>
        private IList<MusicalLine> musicalLines;

        /// <summary>
        /// Musical Objects.
        /// </summary>
        private Collection<IMusicalLocation> musicalObjects;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalPart class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public MusicalPart(MusicalBlock givenBlock)
        {
            this.MusicalBlock = givenBlock;
            this.MusicalLines = new List<MusicalLine>();
            this.MusicalObjects = new Collection<IMusicalLocation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalPart"/> class.
        /// </summary>
        [UsedImplicitly]
        public MusicalPart() {         
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets Musical Lines.
        /// </summary>
        /// <value> Property description. </value>
        public IList<MusicalLine> MusicalLines {
            get
            {
                Contract.Ensures(Contract.Result<IList<MusicalLine>>() != null);
                return this.musicalLines ?? (this.musicalLines = new List<MusicalLine>());
            }
            //// Remove private set - DevExpress
            private set => this.musicalLines = value;
        }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        /// <value> Property description. </value>
        public string PartId { get; set; }

        /// <summary>
        /// Gets or sets Instrument.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalInstrument Instrument { get; set; }

        /// <summary>
        /// Gets or sets Channel.
        /// </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; set; }

        /// <summary> Gets or sets purpose of the line. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public LinePurpose Purpose { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>
        /// The type of the line.
        /// </value>
        public MusicalLineType LineType { get; set; }

        /// <summary>
        /// Gets or sets composed file.
        /// </summary>
        /// <value> Property description. </value>
        /// <summary>
        /// Gets or sets musical block.
        /// </summary>
        /// <value> Property description. </value>
        private MusicalBlock MusicalBlock {
            get {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.musicalBlock == null) {
                    throw new InvalidOperationException("Musical block is null.");
                }

                return this.musicalBlock;
            }

            set => this.musicalBlock = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be null."), nameof(value));
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets or sets Part Number.
        /// </summary>
        /// <value> Property description. </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private int PartNumber { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets Musical Tones.
        /// </summary>
        /// <value> Property description. </value>
        private Collection<IMusicalLocation> MusicalObjects {
            get
            {
                Contract.Ensures(Contract.Result<Collection<IMusicalLocation>>() != null);
                return this.musicalObjects ?? (this.musicalObjects = new Collection<IMusicalLocation>());
            }

            set => this.musicalObjects = value;
        }
        #endregion

        #region Static factory methods
        /// <summary>
        /// Gets new musical part.
        /// </summary>
        /// <param name="musicalBlock">The musical block.</param>
        /// <param name="partNumber">Number of part.</param>
        /// <param name="givenChannel">Midi Channel.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MusicalPart GetNewMusicalPart(MusicalBlock musicalBlock, int partNumber, MidiChannel givenChannel) {
            var part = GetNewMusicalPart(musicalBlock);
            part.PartNumber = partNumber;
            part.Channel = givenChannel;
            return part;
        }
        #endregion

        #region Public methods
        /// <summary> Add on musical tone to the end of part. </summary>
        /// <param name="musicalObject">Musical object - tone or shift.</param>
        public void AddMusicalObject(IMusicalLocation musicalObject) {
            Contract.Requires(musicalObject != null);
            
            //// if (musicalObject == null) { return; }
            this.MusicalObjects.Add(musicalObject);
        }

        /// <summary>
        /// Moves the objects to staff tracks.
        /// </summary>
        public void MoveObjectsToStaffTracks() {
            //// MusicalTones group by staff and voice
            var trackGroups = (from mt in this.MusicalObjects
                               select new { mt.Staff, mt.InstrumentNumber }).Distinct().ToList(); //// mt.Channel
            this.MusicalLines = new List<MusicalLine>();
            foreach (var tg in trackGroups) {
                var tg1 = tg;
                var voiceObjects = (from mt in this.MusicalObjects
                                    where mt.Staff == tg1.Staff && mt.InstrumentNumber == tg1.InstrumentNumber //// && mt.Channel == tg1.Channel 
                                    orderby mt.BarNumber
                                    select mt).ToList();
                if (!voiceObjects.Any()) {
                    continue;
                }

                var line = MusicalLine.GetNewMusicalLine(MusicalLineType.Melodic, this.MusicalBlock);
                line.FirstStatus.Instrument = new MusicalInstrument((MidiMelodicInstrument)tg.InstrumentNumber);
                //// line.FirstStatus.Channel = this.Channel;
                //// line.FirstStatus.GChannel = new GeneralChannel(InstrumentGenus.Melodical, tg.Instrument, this.Channel);
                line.Purpose = this.Purpose;
                //// line.Purpose = this.Purpose; //// 2019/01
                this.MusicalLines.Add(line);

                voiceObjects.ForAll(musicalObject => {
                    if (musicalObject is IMusicalTone tone)
                    {
                        line.AddMusicalTone(tone);
                    }
                });
            }
        }

        /// <summary>
        /// Lays the objects to voice tracks.
        /// </summary>
        public void LayObjectsToVoiceTracks() {
            //// MusicalTones group by staff and voice
            var trackGroups = (from mt in this.MusicalObjects
                               select new { mt.Staff, mt.Voice }).Distinct().ToList();
            this.MusicalLines = new List<MusicalLine>();
            foreach (var tg in trackGroups) {
                var tg1 = tg;
                var voiceObjects = (from mt in this.MusicalObjects
                                    where mt.Staff == tg1.Staff && mt.Voice == tg1.Voice
                                    select mt).ToList();
                if (!voiceObjects.Any()) {
                    continue;
                }

                var line = MusicalLine.GetNewMusicalLine(0, this.MusicalBlock);
                line.FirstStatus.Instrument = this.Instrument;
                line.MainVoice.Channel = this.Channel;

                //// line.Status.GChannel = new GeneralChannel(InstrumentGenus.Melodical, this.Instrument, this.Channel);
                line.Purpose = this.Purpose;
                //// line.Purpose = this.Purpose; //// 2019/01
                this.MusicalLines.Add(line);
                //// Values of BitFrom must be determined here !!!
                var bitFrom = 0;
                var lastBarNumber = -1;
                voiceObjects.ForAll(musicalObject => {
                    if (musicalObject.BarNumber != lastBarNumber) {
                        bitFrom = 0;
                        lastBarNumber = musicalObject.BarNumber;
                    }

                    if (musicalObject is MusicalShift shift)
                    {
                        bitFrom = bitFrom + shift.Value;
                    }

                    // ReSharper disable once InvertIf
                    if (musicalObject is MusicalStrike tone && bitFrom >= 0)
                    {
                        tone.BitFrom = (byte)bitFrom;
                        line.AddMusicalTone(tone);
                        bitFrom = bitFrom + tone.Duration;
                    }

                    if (musicalObject is MusicalPause pause && bitFrom >= 0) {
                        pause.BitFrom = (byte)bitFrom;
                        line.AddMusicalTone(pause);
                        bitFrom = bitFrom + pause.Duration;
                    }
                });
                //// line.MusicalOctave = line.MusicalTones. 
            }
        }
        #endregion 

        #region Private methods
        /// <summary>
        /// Gets new musical line.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static MusicalPart GetNewMusicalPart(MusicalBlock givenBlock) {
            var part = new MusicalPart(givenBlock);
            return part;
        }
        #endregion
    }
}
