// <copyright file="MusicalLinearizer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;

    /// <summary>
    /// Musical Linearizer.
    /// </summary>
    public class MusicalLinearizer
    {
        #region Fields
        /// <summary>
        /// Melodic tones.
        /// </summary>
        private static IList<IMusicalTone> melodicTones;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalLinearizer" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        public MusicalLinearizer(MusicalHeader givenHeader) {
            this.Header = givenHeader;
            this.Parts = new List<MusicalPart>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets Musical Lines.
        /// </summary>
        /// <value> Property description. </value>
        public IList<MusicalLine> Lines { get; set; }

        /// <summary>
        /// Gets or sets Musical Lines.
        /// </summary>
        /// <value> Property description. </value>
        public IList<MusicalPart> Parts { get; set; }

        /// <summary>
        /// Gets or sets the total duration.
        /// </summary>
        /// <value>
        /// The total duration.
        /// </value>
        private long TotalDuration { get; set; }
        #endregion

        #region Public static
        /// <summary>
        /// Line Of Tones.
        /// </summary>
        /// <param name="sourceLine">Source line.</param>
        /// <param name="voice">Voice number.</param>
        /// <returns> Returns value. </returns>
        public static MusicalLine NextRhythmicLineTrack(MusicalLine sourceLine, byte voice) {
            Contract.Requires(sourceLine != null);
            if (sourceLine == null || sourceLine.IsEmpty) {
                return null;
            }

            var line = (MusicalLine)sourceLine.Clone();

            if (!((from tone in sourceLine.Tones
                   where tone.ToneType == MusicalToneType.Rhythmic && !tone.IsPause && !tone.IsReady
                   orderby tone.BarNumber
                   select tone).FirstOrDefault() is MusicalStrike firstTone)) {
                return null;
            }

            //// Key Number represents Instrument but why is not here LineType = MusicalLineType.Rhythmic?
            //// if (line.Channel == (byte)MidiChannel.DrumChannel) {
            //// line.Status.GChannel = new GeneralChannel(InstrumentGenus.Melodical, firstTone.Instrument, MidiChannel.DrumChannel);
            line.FirstStatus.Instrument = new MusicalInstrument((MidiMelodicInstrument)firstTone.InstrumentNumber);
            line.FirstStatus.LineType = MusicalLineType.Rhythmic;

            var lastTone = firstTone;
            var optimalTone = firstTone;
            //// if (optimalTone == null) { return null; }

            optimalTone.IsReady = true;
            optimalTone.Staff = sourceLine.LineNumber;
            optimalTone.Voice = voice;
            line.Tones.Add(optimalTone);

            foreach (
                var tone in
                    sourceLine.Tones.Where(tone => tone.ToneType == MusicalToneType.Rhythmic && !tone.IsPause && !tone.IsReady)) {
                optimalTone = (tone.InstrumentNumber == lastTone.InstrumentNumber ? tone : null) as MusicalStrike;

                if (optimalTone == null) {
                    continue;
                }

                optimalTone.IsReady = true;
                optimalTone.Staff = sourceLine.LineNumber;
                optimalTone.Voice = voice;
                line.Tones.Add(optimalTone);
                lastTone = optimalTone;
            }

            line.FirstStatus.Staff = sourceLine.LineNumber;
            line.FirstStatus.Voice = voice;
            return line;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "Musical Linearizer";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Line Of Tones.
        /// </summary>
        /// <param name="sourceLine">The source line.</param>
        /// <param name="voice">Voice number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalLine NextMelodicLine(MusicalLine sourceLine, byte voice) {
            Contract.Requires(sourceLine != null);
            if (sourceLine == null || sourceLine.IsEmpty) {
                return null;
            }

            var newLine = (MusicalLine)sourceLine.Clone();

            melodicTones = sourceLine.Tones
                    .Where(tone => tone.ToneType == MusicalToneType.Melodic && !tone.IsPause && !tone.IsReady)
                    .ToList();
            if (!((from tone in melodicTones
                   orderby tone.BarNumber
                   select tone).FirstOrDefault() is MusicalTone firstTone)) {
                return null;
            }

            //// Key Number represents Instrument but why is not here LineType = MusicalLineType.Rhythmic?
            //// if (line.Channel == (byte)MidiChannel.DrumChannel) {
            //// line.Instrument = firstTone.Pitch.MidiKeyNumber(); 
            //// line.LineType = MusicalLineType.Rhythmic;  } 

            var lastTone = firstTone;
            var optimalTone = firstTone;

            optimalTone.IsReady = true;
            optimalTone.Staff = sourceLine.LineNumber;
            optimalTone.Voice = voice;
            newLine.Tones.Add(optimalTone);

            while (true) {
                optimalTone = FindOptimalTone(lastTone);
                if (optimalTone == null) {
                    break;
                }

                MusicalStrike.CorrectBadBinding(lastTone, optimalTone);
                optimalTone.IsReady = true;
                optimalTone.Staff = sourceLine.LineNumber;
                optimalTone.Voice = voice;
                newLine.Tones.Add(optimalTone);
                lastTone = optimalTone;
            }

            newLine.FirstStatus.Staff = sourceLine.LineNumber;
            newLine.FirstStatus.Voice = voice;
            newLine.FirstStatus.LocalPurpose = LinePurpose.Fixed;  //// 2020/10

            var completedTones = newLine.Tones.CollectionWithAddedMissingPauses();
            var standardizedTones = completedTones.StandardizeTones(this.Header);
            newLine.SetTones(standardizedTones);
            return newLine;
        }

        /// <summary>
        /// Transfer Parts To Lines.
        /// </summary>
        /// <param name="skipNegligibleTracks">if set to <c>true</c> [skip negligible tracks].</param>
        public void TransferPartsToLines(bool skipNegligibleTracks) { //// MusicalBlock block, block.TotalDuration
            var lines = new List<MusicalLine>();
            int lineIndex = 0;
            foreach (var part in this.Parts) {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var line in part.MusicalLines) {
                    //// this.MusicalParts.Select(part => part.MusicalLines).SelectMany(partTracks => partTracks)) {

                    //// Test 2013/12
                    //// Skip negligible tracks !?
                    if (skipNegligibleTracks) {
                        var minimumDuration = this.TotalDuration / 10;
                        if (line.DurationOfTones < minimumDuration) {
                            continue;
                        }
                    }

                    line.LineType = part.LineType; //// 2020/10
                    line.FirstStatus.Instrument = part.Instrument;
                    line.LineIndex = lineIndex++;
                    line.FirstStatus.BarNumber = 1; //// 2018/12
                    line.FirstStatus.LocalPurpose = LinePurpose.Fixed;  //// 2020/10
                    //// line.Staff = givenMidiTrack.Staff;
                    //// line.Voice = givenMidiTrack.Voice;
                    //// line.Status.GChannel = new GeneralChannel(InstrumentGenus.Melodical, part.Instrument, part.Channel);

                    //// line.MainVoice.Channel = part.Channel;
                    line.MainVoice = new MusicalVoice {
                        Octave = line.FirstStatus.Octave,
                        Instrument = line.FirstStatus.Instrument,
                        Loudness = line.FirstStatus.Loudness,
                        Line = line,
                        Channel = part.Channel
                    };

                    line.Voices = new List<IAbstractVoice> { line.MainVoice };

                    lines.Add(line);
                }
            }

            this.Lines = lines;
            //// 2016 this.NumberOfRhythmicLines = 0;
            //// 2016 this.NumberOfMelodicLines = lineIndex;
        }

        /// <summary>
        /// Split Lines To Parts.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="splitMultiVoiceLines">If set to <c>true</c> [split multi-voice tracks].</param>
        public void SplitLinesToParts(MusicalBlock block, FileSplit splitMultiVoiceLines) {
            const float minimalOccupationForSplit = 1.1f;
            const float minimalOccupationForInclusion = 0.05f; //// 2015/01
            this.TotalDuration = block.TotalDuration;
            this.Parts = new List<MusicalPart>();
            byte voice = 0;
            byte staff = 0;
            foreach (var sourceLine in this.Lines) {
                if (sourceLine.LineNumber != staff) {
                    staff = sourceLine.LineNumber;
                    voice = 0;
                }

                var part = new MusicalPart(block) {
                    Channel = sourceLine.MainVoice.Channel,
                    Instrument = sourceLine.FirstStatus.Instrument, //// 2019/10
                    LineType = sourceLine.LineType
                };

                //// Melodic line
                if (sourceLine.FirstStatus.IsMelodic) {
                    var q = sourceLine.QuotientOfOccupation();

                    if (splitMultiVoiceLines == FileSplit.Total ||
                        (splitMultiVoiceLines == FileSplit.Automatic && q > minimalOccupationForSplit)) {
                        while (true) {
                            var line = this.NextMelodicLine(sourceLine, voice++);
                            if (line == null) {
                                break;
                            }

                            q = line.QuotientOfOccupation();
                            if (q > minimalOccupationForInclusion) {
                                part.MusicalLines.Add(line);
                            }
                        }
                    }
                    else {
                        part.MusicalLines.Add(sourceLine);
                    }
                }

                //// Rhythmic line
                if (sourceLine.FirstStatus.IsRhythmic) {
                    while (true) {
                        var line = NextRhythmicLineTrack(sourceLine, voice++);
                        if (line == null) {
                            break;
                        }

                        part.MusicalLines.Add(line);
                    }
                }

                //// If there is more sequence in one part, decrement their loudness !?
                //// DecrementLoudness(part);

                this.Parts.Add(part);
            }
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Finds the optimum tone.
        /// </summary>
        /// <param name="lastTone">The last tone.</param>
        /// <returns> Returns value. </returns>
        private static MusicalTone FindOptimalTone(MusicalTone lastTone) {
            Contract.Requires(lastTone != null);
            const int maxPassTones = 12;
            MusicalTone optimalTone = null;
            var optimalValue = 0;
            var countPassed = 0;
            foreach (var musicalStrike in melodicTones) {
                var mtone = (MusicalTone)musicalStrike;
                //// .Select(tone => tone as MusicalTone)
                var continuous = true;
                if (mtone.IsReady) {
                    continue;
                }

                var value = 100;
                //// Here possibly toleration of some percent of length
                var minStartPosition = lastTone.BitPosition + (int)Math.Round(lastTone.Duration * 0.8f); //// 0.9f
                if (mtone.BitPosition >= minStartPosition) {
                    var dist = mtone.Pitch.DistanceFrom(lastTone.Pitch);

                    if (dist == 0) {
                        value += 10;
                    }
                    else {
                        value -= dist;
                        if (dist > DefaultValue.HarmonicOrder) {
                            value -= 1;
                        }
                    }

                    var bitDistance = Math.Abs(mtone.BitPosition - lastTone.BitPosition);
                    value -= bitDistance / mtone.BitRange.Order;

                    if (mtone.BitRange.Length == mtone.BitRange.Order && lastTone.BitRange.Length == lastTone.BitRange.Order) {
                        value += 1;
                    }

                    if (lastTone.BarNumber == mtone.BarNumber - 1 && lastTone.IsGoingToNextBar != mtone.IsFromPreviousBar) {
                        value -= 100;
                        continuous = false;
                    }

                    if (value > optimalValue && (!continuous || optimalTone == null || mtone.BitPosition <= optimalTone.BitPosition)) { //// || optimalValue < -500
                        optimalValue = value;
                        optimalTone = mtone;
                    }

                    countPassed++;
                }

                if (countPassed > maxPassTones) {
                    break;
                }
            }

            return optimalTone;
        }

        #endregion
    }
}
