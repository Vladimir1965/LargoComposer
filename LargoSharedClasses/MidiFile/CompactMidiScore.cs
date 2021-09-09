// <copyright file="CompactMidiScore.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using System.Collections.Generic;
using System.Linq;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Compact Midi Score.
    /// </summary>
    public class CompactMidiScore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiScore"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public CompactMidiScore(MusicalBlock givenBlock) {
            this.MusicalBlock = givenBlock;
            this.Lines = new List<CompactMidiStaff>();

            this.PrepareLines();
            this.Instruments = this.Lines
                                  .GroupBy(line => line.Voice.Instrument.Number)
                                  .Select(g => g.First().Voice.Instrument)
                                  .ToList();

            this.AssignChannels();
        }

        /// <summary>
        /// Gets or sets the musical block.
        /// </summary>
        /// <value>
        /// The musical block.
        /// </value>
        public MusicalBlock MusicalBlock { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public List<CompactMidiStaff> Lines { get; set; }

        /// <summary>
        /// Gets or sets the instruments.
        /// </summary>
        /// <value>
        /// The instruments.
        /// </value>
        public List<MusicalInstrument> Instruments { get; set; }

        /// <summary>
        /// Assigns the channels.
        /// </summary>
        private void AssignChannels() {
            for (byte index = 0; index < this.Instruments.Count; index++) {
                var instrument = this.Instruments[index];
                var channel = MusicalProperties.ChannelForPartNumber(index + 1); 
                foreach (var line in this.Lines) {
                    if (line.Voice.Instrument.Number == instrument.Number) {
                        line.Channel = channel;
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the lines.
        /// </summary>
        private void PrepareLines() {
            byte number = 0;
            foreach (var musicalLine in this.MusicalBlock.Strip.Lines) {
                bool hasOrchestra = false;
                foreach (var bar in this.MusicalBlock.Body.Bars) {
                    var element = bar.GetElement(musicalLine.LineIdent);
                    var unit = element.Status.OrchestraUnit;
                    hasOrchestra = hasOrchestra || (unit != null);
                    if (unit != null && !this.ExistsUnit(unit)) {
                        foreach (var voice in unit.ListVoices) {
                            var staff = new CompactMidiStaff(number++, musicalLine.LineType, null, voice, unit);
                            this.Lines.Add(staff);
                            hasOrchestra = true;
                        }
                    }
                }
                
                //// Here is not considered variant with orchestra only in selected lines.
                if (!hasOrchestra) {
                    var lineVoices = musicalLine.Voices;
                    foreach (var voice in lineVoices) {
                        var staff = new CompactMidiStaff(number++, musicalLine.LineType, musicalLine, voice, null);
                        this.Lines.Add(staff);
                    }
                }
            }
        }

        /// <summary>
        /// Exists the unit.
        /// </summary>
        /// <param name="givenUnit">The given unit.</param>
        /// <returns> Returns value. </returns>
        private bool ExistsUnit(OrchestraUnit givenUnit) {
            var flag = (from line in this.Lines
                        where line.OrchestraUnit == givenUnit
                        select 1).Any();
            return flag;
        }
    }
}
