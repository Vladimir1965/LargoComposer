// <copyright file="BlockComposer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Composer
{
    using System.IO;
    using Abstract;
    using LargoSharedClasses.Models;
    using LargoSharedClasses.Settings;
    using Localization;
    using Music;

    /// <summary>
    /// Block Composer.
    /// </summary>
    public class BlockComposer
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the source block.
        /// </summary>
        /// <value>
        /// The source block.
        /// </value>
        public MusicalBlock SourceBlock { get; set; }

        /// <summary>
        /// Gets or sets the composed block.
        /// </summary>
        /// <value>
        /// The composed block.
        /// </value>
        public MusicalBlock ComposedBlock { get; set; }

        #endregion

        #region Public methods - Composition

        /// <summary>
        /// Prepares the lines for composition.
        /// </summary>
        public void PrepareLinesForComposition()
        {
                foreach (var line in this.ComposedBlock.Strip.Lines)
                {
                    if (line == null) {
                        continue;
                    }

                    if (line.Purpose == LinePurpose.Composed) {
                        line.Reset();
                    }

                    line.FirstStatus.MelodicVariety = new MusicalVariety(MusicalSettings.Singleton);
                }
        }

        /// <summary>
        /// Composes the music.
        /// </summary>
        /// <param name="givenSourceBlock">The given source block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalBundle ComposeMusic(MusicalBlock givenSourceBlock)
        {
            this.SourceBlock = givenSourceBlock;
            this.ComposedBlock = this.SourceBlock;

            //// var model = 
            HarmonicModel.GetNewModel(this.SourceBlock);

            this.PrepareLinesForComposition();

            ProcessLogger.Singleton.SendMessageEvent(this.SourceBlock.Header.Specification, LocalizedMusic.String("Initialization..."), 0);
            BodyComposer bodyComposer = new BodyComposer();

            //// Main compositional method.
            bodyComposer.ComposeMusic(this.ComposedBlock.Body);

            //// 2019/01
            //// When tones are saved without instrument or loudness ... 
            foreach (var bar in this.ComposedBlock.Body.Bars) {
                bar.SendStatusToTones();
            }

            this.ComposedBlock.ConvertBodyToStrip(true, true);

            ProcessLogger.Singleton.SendMessageEvent(this.SourceBlock.Header.Specification, LocalizedMusic.String("Finalization..."), 0);
            var filename = this.SourceBlock.Header.FullName;
            int variant = 0;
            while (File.Exists(filename))
            {
                variant++;
                filename = this.SourceBlock.Header.FullName + variant.ToString();
            }

            var composedFile = MusicalBundle.GetEnvelopeOfBlock(this.ComposedBlock, filename);
            return composedFile;
        }

        #endregion
    }
}
