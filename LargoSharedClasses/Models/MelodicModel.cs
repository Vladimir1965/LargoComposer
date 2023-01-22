// <copyright file="MelodicModel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
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
    public sealed class MelodicModel : AbstractModel, ICloneable
    {
        #region Fields

        /// <summary>
        /// The melodic motives
        /// </summary>
        private IEnumerable<MelodicMotive> melodicMotives;

        /// <summary>
        /// The used melodic motives
        /// </summary>
        private Dictionary<string, MelodicMotive> usedMelodicMotives;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicModel"/> class.
        /// </summary>
        public MelodicModel() {
            this.MelodicMotives = new List<MelodicMotive>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the harmonic order.
        /// </summary>
        /// <value>
        /// The harmonic order.
        /// </value>
        public byte HarmonicOrder { get; set; }

        /// <summary>
        /// Gets or sets the melodic motives.
        /// </summary>
        /// <value>
        /// The melodic motives.
        /// </value>
        public IEnumerable<MelodicMotive> MelodicMotives {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<MelodicMotive>>() != null);
                if (this.melodicMotives == null) {
                    throw new InvalidOperationException("Melodic motives are null.");
                }

                return this.melodicMotives;
            }

            set => this.melodicMotives = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be empty."), nameof(value));
        }

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
        public static MelodicModel GetNewModel(MusicalBlock musicalBlock) {
            musicalBlock.RefreshHeader(); //// ????
            //// musicalBlock.Header.NumberOfLines = (byte)musicalBlock.Strip.Lines.Count;

            var model = GetNewModel(musicalBlock.Header.FileName, musicalBlock);
            model.Number = musicalBlock.Header.Number;
            model.SourceMusicalBlock = musicalBlock;
            model.Header = musicalBlock.Header;

            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Analyzing musical lines..."), 0);
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
        public static MelodicModel GetNewModel(string modelName, MusicalBlock musicalBlock) {
            Contract.Requires(musicalBlock != null);
            var model = new MelodicModel {
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

        /// <summary>
        /// Gets the melodic motive.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MelodicMotive GetMelodicMotive(int number) {
            var cnt = this.MelodicMotives.Count();
            if (cnt == 0) {
                return null;
            }

            var localNumber = number;
            if (localNumber > cnt) {
                checked {
                    localNumber = ((number - 1) % cnt) + 1;
                }
            }

            //// MelodicMotive motive = this.MelodicMotives.ElementAt(localNumber);   
            var motive = (from m in this.MelodicMotives where m.Number == localNumber select m).FirstOrDefault();
            return motive;
        }

        /// <summary>
        /// Adds the motive.
        /// </summary>
        /// <param name="motive">The motive.</param>
        public void AddMotive(MelodicMotive motive) {
            Contract.Requires(motive != null);
            //// motive.Core = this;
            ((List<MelodicMotive>)this.MelodicMotives).Add(motive);
        }
        #endregion

        /// <summary>
        /// Appends the melodic motives.
        /// Main algorithm to determine motivic classes and their instances
        /// </summary>
        /// <param name="itemGroups">The item groups.</param>
        public void AppendMelodicMotives(List<MelodicItemGroup> itemGroups) {
            if (this.MelodicMotives == null) {
                return;
            }

            var lastMelodicIdentifier = string.Empty;

            //// All motivic items ordered by length (descending) and identifier
            var melodicItemGroups = (from ig in itemGroups orderby ig.Length descending, ig.MelodicIdentifier select ig).ToList();

            MelodicMotive melodicMotive = null;
            //// var numberWithinLength = 0; var lastLength = 0;
            foreach (var itemGroup in melodicItemGroups) {
                if (itemGroup.MusicalLine.FirstStatus.LineType != MusicalLineType.Melodic) {
                    continue;
                }

                //// if (itemGroup.Length != lastLength) { numberWithinLength = 0; lastLength = itemGroup.Length; } 
                var ident = itemGroup.MelodicIdentifier;
                if (ident != lastMelodicIdentifier) { //// Step to next new motive
                    melodicMotive = itemGroup.MelodicMotive(0, string.Empty);
                    this.AddMotive(melodicMotive);
                    lastMelodicIdentifier = ident;
                }

                if (melodicMotive != null) {
                    melodicMotive.Occurrence++;
                    var area = itemGroup.GetArea();
                    this.SourceMusicalBlock.Body.MarkMelodicMotive(melodicMotive, area);
                }
            }

            this.CompleteMotives();
        }

        /// <summary>
        /// Completes the motives.
        /// </summary>
        public void CompleteMotives() {
            var melodicMotiveNumber = 0;
            var orderedMotives = (from m in this.MelodicMotives orderby m.Occurrence descending, m.Length descending select m).ToList();
            foreach (var motive in orderedMotives) {
                melodicMotiveNumber++;
                //// string motiveName = string.Format(CultureInfo.InvariantCulture,"T{0}/R{1}", ("0" + musicalLine.LineIndex.ToString(CultureInfo.CurrentCulture)).Right(2), ("0000" + this.MelodicAnalyzer.RhythmicMotiveNumber.ToString(CultureInfo.CurrentCulture)).Right(4));
                //// var motiveName = string.Format(CultureInfo.InvariantCulture, "R{0}", ("0000" + rhythmicMotiveNumber.ToString(CultureInfo.CurrentCulture)).Right(4));
                var motiveName = MusicalProperties.GetMotiveName(string.Empty, melodicMotiveNumber, motive.Length); //// rhythmicMotiveNumber
                motive.Number = melodicMotiveNumber;
                motive.Name = motiveName;
            }
        }

        #region Unique motives

        /// <summary>
        /// Get UniqueTMelodicMotive.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique Identifier.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MelodicMotive GetUniqueMelodicMotive(string uniqueIdentifier) {
            if (this.usedMelodicMotives == null) {
                this.usedMelodicMotives = new Dictionary<string, MelodicMotive>();
            }

            var tmm = this.usedMelodicMotives.ContainsKey(uniqueIdentifier) ? this.usedMelodicMotives[uniqueIdentifier] : null;
            if (tmm != null) {
                return tmm;
            }

            //// Lock needed here
            var melodicMotiveList = this.MelodicMotives;
            foreach (var melodicMotive in
                melodicMotiveList.Where(melodicMotive => melodicMotive != null
                    && string.Compare(melodicMotive.UniqueIdentifier, uniqueIdentifier, StringComparison.Ordinal) == 0)) {
                this.usedMelodicMotives[uniqueIdentifier] = melodicMotive;
                return melodicMotive; //// Avoid multiple or conditional return statements.
            }

            return null;
        }

        #endregion

        /// <summary> Makes a deep copy of the BlockModel object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var model = new MelodicModel {
                Name = this.Name,
                Number = this.Number,
                //// Core = this.Core,
                Header = (MusicalHeader)this.Header.Clone(),
                SourceMusicalBlock = this.SourceMusicalBlock
            };

            return model;
        }

        #region Material Extractor
        /// <summary>
        /// Extract melodic material.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MelodicMaterial ExtractMelodicMaterial() {
            //// var dcm = DataBridgeMaterial.GetMaterialContext;
            var tmm = new MelodicMaterial { Name = this.Name }; //// , CoreId = melodicCore.TMelodicCore.Id
            //// dcm.AddToTMelodicMaterial(tmm);
            var list = new List<MelodicStructure>();

            /* 2020/12
            foreach (var structure in from motive in this.MelodicMotives
                                      from structure in motive.MelodicStructures
                                      let sc = structure.GetStructuralCode
                                      where !string.IsNullOrEmpty(sc)
                                      select structure) {
                list.Add(structure);
            } */

            if (this.SourceMusicalBlock != null) {
                var tempList = new List<MelodicStructure>();
                foreach (var bar in this.SourceMusicalBlock.Body.Bars) {
                    foreach (var element in bar.Elements) {
                        var structure = element?.Status?.MelodicStructure;
                        if (structure == null) {
                            continue;
                        }

                        tempList.Add(structure);
                    }
                }

                list.AddRange(tempList);
            }

            var groupList = (from ms in list
                             group ms by ms.GetStructuralCode into g
                             select g).ToList();
            groupList.ForEach(g => {
                var s = g.FirstOrDefault();
                if (s == null) {
                    return;
                }

                var ms = s; //// Clone?
                ms.Occurrence = g.Count();
                tmm.Structures.Add(ms);
            });

            //// dcm.SaveChanges();
            return tmm;
        }
        #endregion
    }
}
