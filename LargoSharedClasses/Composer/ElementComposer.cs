// <copyright file="ElementComposer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Linq;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Composer
{
    /// <summary>
    /// Element Composer.
    /// </summary>
    public class ElementComposer
    {
        /// <summary>
        /// Gets or sets the line rules.
        /// </summary>
        /// <value>
        /// The line rules.
        /// </value>
        public LineRules LineRules { get; set; }

        /// <summary>
        /// Composes one bar of melody in this musical part.
        /// </summary>
        /// <param name="element">The element.</param>
        [ContractVerification(false)]
        public void ComposeMelody(MusicalElement element)
        {
            var line = (MusicalLine)element.Line;

            this.LineRules = LineRules.NewStandardLineRules(element.Status);
            if (this.LineRules == null) {
                return;
            }

            if (MusicalSettings.Singleton.SettingsProgram.HasTraceValues) {
                element.PrepareTonePacket();
            }

            if (element.PreviousElement != null) {
                line.LastTone = element.PreviousElement.Tones.LastOrDefault() as MusicalTone;
            }
            //// this.AssignPreviousTones();

            var tones = element.Tones; //// this.Line.MusicalTonesInBar(this.Bar.Number);
            var count = tones.Count; // c.MelPartBar.
            if (count == 0) {
                return;
            }

            var melodicVariety = line.FirstStatus.MelodicVariety; //// Status.MelodicVariety;
            if (melodicVariety == null) {
                return;
            }     
             
            melodicVariety.PrepareVariety(element, (MusicalBar)element.Bar, this.LineRules);
            //// Ordinal indexes (time optimization for figural value) 
            ////  foreach (MusicalStrike mt in tones) {
            //// IndexOf does not work, because of MusicalStrike compareTo method!? 
            ////  mt.OrdinalIndex = this.MusicalTones.IndexOf(mt);  } 

            //// foreach: the elements are traversed in increasing index order
            var tonesToPass = tones.OfType<MusicalTone>().Where(musicalTone => musicalTone.Duration != 0).ToList(); //// 2018/10
            foreach (var musicalTone in tonesToPass) {
                if (musicalTone.Loudness == 0) {
                    //// 2016/09 this is some nonsense                    
                    //// if (this.Line.LastTone != null && !this.Line.LastTone.IsEmpty) {
                    //// musicalTone.SetPitch(this.Line.LastTone.Pitch);  } 
                    if (musicalTone.IsTrueTone) {
                        line.LastTone = (MusicalTone)musicalTone.Clone();
                    }

                    continue;
                }

                if (musicalTone.IsFromPreviousBar) {
                    if (line.LastTone != null && !line.LastTone.IsEmpty) {
                        if (line.LastTone.IsGoingToNextBar) {
                            musicalTone.SetPitch(line.LastTone.Pitch);
                            if (musicalTone.IsTrueTone) {
                                line.LastTone = (MusicalTone)musicalTone.Clone();
                            }

                            continue;
                        } //// else {
                        musicalTone.IsFromPreviousBar = false;
                        //// } 
                    }
                }

                element.ComposeTone(musicalTone);

                if (line.LastTone != null && line.LastTone.IsTrueTone) {
                    line.PenultTone = (MusicalTone)line.LastTone.Clone();
                }

                if (musicalTone.IsTrueTone) {
                    line.LastTone = (MusicalTone)musicalTone.Clone();
                }
                //// this.Status.PreviousBarLastTone = prevTones.LastOrDefault() as MusicalTone;
            }
            //// this.CurrentMelodicMotiveEvaluator.LineInBarFinished();
        }
    }
}
