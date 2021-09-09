// <copyright file="ElementMaster.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Element Master
    /// </summary>
    public class ElementMaster {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementMaster" /> class.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        public ElementMaster(IEnumerable<MusicalElement> givenList) {
            this.List = givenList;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public IEnumerable<MusicalElement> List { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "Element Master";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the random rhythm.
        /// </summary>
        /// <param name="rhythmicStructures">The rhythmic structures.</param>
        public void SetRandomRhythm(IList<RhythmicStructure> rhythmicStructures) {
            var cnt = rhythmicStructures.Count;
            foreach (var element1 in this.List) {
                if (element1 == null) {
                    continue;
                }

                var i = 0;
                RhythmicStructure rstruct;
                while (true) {
                    var idx = MathSupport.RandomNatural(cnt);
                    rstruct = rhythmicStructures[idx];
                    if (rstruct.Level > 0 || i > 10) {
                        break;
                    }

                    i++;
                }

                element1.Status.RhythmicStructure = rstruct;
            }
        }

        #endregion

        #region Convert line types
        /// <summary>
        /// Converts to melodic.
        /// </summary>
        /// <param name="givenChannel">The given channel.</param>
        public void ConvertToMelodic(MidiChannel givenChannel) {
            foreach (var elem in this.List) {
                elem.Status.LineType = MusicalLineType.Melodic;
                elem.Status.Instrument =
                    new MusicalInstrument(MidiMelodicInstrument.NylonAcousticGuitar); //// AcousticGrandPiano
                elem.Status.MelodicShape = MelodicShape.Scales;
                elem.Status.Octave = MusicalOctave.OneLine;
                elem.Status.MelodicGenus = MelodicGenus.Melodic;
                elem.Status.MelodicFunction = MelodicFunction.HarmonicMotion;
                //// elem.Status.Channel = givenChannel;
                elem.Status.LocalPurpose = LinePurpose.Composed;
                elem.Tones = new ToneCollection();
            }
        }

        /// <summary>
        /// Converts to rhythmic.
        /// </summary>
        /// <param name="givenChannel">The given channel.</param>
        public void ConvertToRhythmic(MidiChannel givenChannel) {
            foreach (var elem in this.List) {
                elem.Status.LineType = MusicalLineType.Rhythmic;
                elem.Status.Instrument =
                    new MusicalInstrument(MidiRhythmicInstrument.AcousticSnare);
                elem.Status.MelodicShape = MelodicShape.None;
                elem.Status.Octave = MusicalOctave.None;
                elem.Status.MelodicGenus = MelodicGenus.None;
                elem.Status.MelodicFunction = MelodicFunction.None;
                //// elem.Status.Channel = givenChannel;
                elem.Status.LocalPurpose = LinePurpose.Composed;
                elem.Tones = new ToneCollection();
            }
        }
        #endregion

        /// <summary>
        /// Eliminates the melodic dependencies.
        /// </summary>
        public void EliminateMelodicDependencies() {
            foreach (var element in this.List) {
                if (element.Status.OriginalMelodicPoint.IsDefined) {
                    element.Status.LocalPurpose = LinePurpose.Mute;
                    element.Status.LineType = MusicalLineType.None;
                    element.Tones = new ToneCollection();
                }
            }
        }

        /// <summary>
        /// Eliminates the rhythmic dependencies.
        /// </summary>
        public void EliminateRhythmicDependencies() {
            foreach (var element in this.List) {
                if (element.Status.OriginalRhythmicPoint.IsDefined) {
                    element.Status.LocalPurpose = LinePurpose.Mute;
                    element.Status.LineType = MusicalLineType.None;
                    element.Tones = new ToneCollection();
                }
            }
        }

        /// <summary>
        /// Haves any purpose.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool HaveAnyPurpose() {
            bool isEmpty = true;
            foreach (var element in this.List) {
                if (element.Status.LocalPurpose == LinePurpose.Mute || element.Status.LocalPurpose == LinePurpose.None) {
                    continue;
                }

                isEmpty = false;
            }

            return !isEmpty;
        }
    }
}