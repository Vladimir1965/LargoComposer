// <copyright file="BodyComposer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Composer
{
    /// <summary>
    /// Body Composer.
    /// </summary>
    public class BodyComposer
    {
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public MusicalBody Body { get; set; }

        /// <summary>
        /// Composes the music.
        /// </summary>
        /// <param name="givenBody">The given body.</param>
        public void ComposeMusic(MusicalBody givenBody)
        {
            this.Body = givenBody;
            ElementComposer elementComposer = new ElementComposer();
            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Preparing rhythm..."), 0);
            foreach (var bar in this.Body.Bars) {
                bar.FillLinesWithRhythm();
                bar.MakeHarmonicClusters();
            }

            //// Prepare Planned Tones, correct melodic type.
            //// int percentage;
            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Preparing planned tones..."), 0);
            foreach (var bar in this.Body.Bars) {
                //// percentage = (100 * bar.BarNumber) / this.Bars.Count;
                bar.PreparePlannedTones();
                //// Correct melodic type - eliminate parallel individualized melodic voices
                if (this.Body.Context.Settings.SettingsComposition.Rules.IndividualizeMelodicVoices) {
                    bar.EliminateParallelMelodies();
                }
            }

            var elements = (from elem in this.Body.AllElements
                            where elem.Status != null
                                && elem.Line.Purpose == LinePurpose.Composed //// 2019/12
                                //// && elem.Status.LocalPurpose == LinePurpose.Composed
                                && elem.Status.IsMelodicalNature
                            select elem).ToList();
            foreach (var elem in elements) {
                elem.IsFinished = false;
            }

            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Composing lines..."), 0);
            float cntElems;
            int numElem;
            if (!this.Body.Context.Settings.SettingsComposition.Rules.IndividualizeMelodicVoices) {
                var melodicOriginals = (from elem in elements
                                        where elem.Status.IsMelodicOriginal
                                        orderby elem.Status.Priority, elem.Bar.BarNumber
                                        select elem).ToList();
                cntElems = melodicOriginals.Count;
                numElem = 0;
                foreach (var elem in melodicOriginals) {
                    numElem++;
                    int percent = (int)Math.Floor(100 * numElem / cntElems);
                    if (percent % 2 == 0) { //// percentageChange
                        ProcessLogger.Singleton.SendMessageEvent(elem.ToProgressString(), LocalizedMusic.String("Composing lines..."), percent);
                    }

                    elementComposer.ComposeMelody(elem);
                    elem.IsFinished = true;
                }

                //// Parallel melodic motives (variant) - if are the same, then here are set the same tones
                //// substituted = element.AppendDoubledMelodicTones();
                var dependentElements =
                    (from elem in elements
                     where !elem.IsFinished && elem.Status.OriginalMelodicPoint.IsDefined //// !?! && elem.OriginalMelodicPoint.IsComposed
                     orderby elem.Status.Priority, elem.Bar.BarNumber
                     select elem).ToList();
                foreach (var elem in dependentElements) {
                    var point = elem.Status.OriginalMelodicPoint;
                    var origin = this.Body.GetElement(point);
                    if (origin != null) {
                        elem.Tones = origin.Tones.Clone(false);
                        elem.IsFinished = true;
                    }
                }
            }

            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Finishing lines..."), 0);
            var restElements = (from elem in elements
                                where !elem.IsFinished
                                orderby elem.Status.Priority, elem.Bar.BarNumber
                                select elem).ToList();
            cntElems = restElements.Count;
            numElem = 0;
            foreach (var elem in restElements) {
                numElem++;
                int percent = (int)Math.Floor(100 * numElem / cntElems);
                if (percent % 2 == 0) { //// percentageChange
                    ProcessLogger.Singleton.SendMessageEvent(elem.ToProgressString(), LocalizedMusic.String("Finishing lines..."), percent);
                }

                elementComposer.ComposeMelody(elem);
                elem.IsFinished = true;
            }

            //// Complete tone instruments from current element statuses.           
            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Orchestration..."), 0);
            foreach (var bar in this.Body.Bars) {
                bar.SendStatusToTones();
            }
        }
    }
}
