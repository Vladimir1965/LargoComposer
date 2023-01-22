// <copyright file="TectonicModel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Musical Block Model.
    /// </summary>
    [Serializable]
    public sealed class TectonicModel : AbstractModel, ICloneable
    {
        #region Fields
        #endregion

        #region Constructors

        #endregion

        #region Public Properties
        #endregion

        #region Other public properties

        /// <summary>
        /// Gets the first melodic bar.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int FirstMelodicBar {
            get {
                var barNumber = (from c in this.BlockChanges.Changes where c.IsMelodicalNature orderby c.BarNumber select c.BarNumber).FirstOrDefault();
                return barNumber;
            }
        }

        #endregion

        #region Static factory methods
        /// <summary>
        /// Extracts the musical block model.
        /// </summary>
        /// <param name="musicalBlock">The musical block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static TectonicModel GetNewModel(MusicalBlock musicalBlock) {
            musicalBlock.RefreshHeader(); //// ????
            //// musicalBlock.Header.NumberOfLines = (byte)musicalBlock.Strip.Lines.Count;

            var model = GetNewModel(musicalBlock.Header.FileName, musicalBlock);
            model.Number = musicalBlock.Header.Number;
            model.SourceMusicalBlock = musicalBlock;
            //// model.Core = new MusicalCore(musicalBlock.Header);
            model.Header = musicalBlock.Header;

            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Analyzing tempo..."), 0); //// musicalBlock.Name
            var tempoChanges = musicalBlock.Body.AnalyzeTempoChanges();
            tempoChanges.ForAll(change => model.BlockChanges.Changes.Add(change));

            //// 2016 this.Body.MakeBars(model.Header.NumberOfBars, this.Header);    model.SourceMusicalBlock.NumberOfBars
            musicalBlock.Body.SetHarmonicBasis(model.BlockChanges);         //// true
            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Analyzing musical lines..."), 0);

            var simpleChanges = musicalBlock.Body.ExtractSimpleChanges(MusicalChangeType.All);
            simpleChanges.ForAll(change => model.BlockChanges.Changes.Add(change));

            return model;
        }

        /// <summary>
        /// Gets the new musical model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="musicalBlock">The musical block.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Null Exception.</exception>
        public static TectonicModel GetNewModel(string modelName, MusicalBlock musicalBlock) {
            Contract.Requires(musicalBlock != null);
            var model = new TectonicModel {
                Name = modelName,
                IsSelected = false
            };
            if (model == null) {
                throw new ArgumentNullException(nameof(modelName));
            }

            model.Header = musicalBlock.Header;
            model.SourceMusicalBlock = musicalBlock;
            return model;
        }

        #endregion

        #region Public methods

        #endregion

        /// <summary> Makes a deep copy of the BlockModel object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var model = new TectonicModel {
                Name = this.Name,
                Number = this.Number,
                //// Core = this.Core,
                Header = (MusicalHeader)this.Header.Clone(),
                BlockChanges = new MusicalChanges()
            };
            model.BlockChanges.AppendCopyOfChanges(this.BlockChanges.Changes); //// model
            model.SourceMusicalBlock = this.SourceMusicalBlock;
            //// model.MakeBars(model.NumberOfBars);

            return model;
        }
    }
}
