// <copyright file="MusicalBlockWrap.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Musical Block Wrap.
    /// </summary>
    public class MusicalBlockWrap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBlockWrap"/> class.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public MusicalBlockWrap(MusicalBlock givenBlock) {
            this.Block = givenBlock;

            //// Harmonic Model
            this.HarmonicModel = HarmonicModel.GetNewModel(givenBlock);
            if (this.HarmonicModel == null) {
                return;
            }

            //// Rhythmic Model
            this.RhythmicModel = RhythmicModel.GetNewModel("Inner", givenBlock);

            //// Melodic Model
            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Analyzing musical lines..."), 0);
            this.MelodicModel = MelodicModel.GetNewModel("Inner", givenBlock);
            var melodicAnalyzer = new MelodicAnalyzer();
            melodicAnalyzer.AnalyzeMusicalLines(this.MelodicModel, this.RhythmicModel, givenBlock);

            //// Orchestration
            var orchestration = new MusicalOrchestration(givenBlock); //// model.SourceMusicalBlock, ObjectName = model.FullName,
            this.Orchestration = orchestration;

            //// 2020/09 var model = HarmonicModel.GetNewModel(givenBlock);
        }

        #region Public properties - Models
        /// <summary>
        /// Gets or sets the harmonic model.
        /// </summary>
        /// <value>
        /// The harmonic model.
        /// </value>
        public HarmonicModel HarmonicModel { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic model.
        /// </summary>
        /// <value>
        /// The rhythmic model.
        /// </value>
        public RhythmicModel RhythmicModel { get; set; }

        /// <summary>
        /// Gets or sets the melodic model.
        /// </summary>
        /// <value>
        /// The melodic model.
        /// </value>
        public MelodicModel MelodicModel { get; set; }
        #endregion

        #region Public properties - Others
        /// <summary>
        /// Gets or sets the orchestration.
        /// </summary>
        /// <value>
        /// The orchestration.
        /// </value>
        public MusicalOrchestration Orchestration { get; set; }

        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        /// <value>
        /// The block.
        /// </value>
        public MusicalBlock Block { get; set; }
        #endregion
    }
}
