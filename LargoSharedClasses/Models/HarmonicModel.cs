// <copyright file="HarmonicModel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Models
{
    using Abstract;
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Settings;
    using Localization;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Musical Block Model.
    /// </summary>
    [Serializable]
    public sealed class HarmonicModel : AbstractModel, ICloneable
    {
        #region Fields
        /// <summary>
        /// Harmonic motives.
        /// </summary>
        [NonSerialized]
        private IEnumerable<HarmonicMotive> harmonicMotives;

        /// <summary>
        /// The harmonic bars
        /// </summary>
        private IEnumerable<HarmonicBar> harmonicBars;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicModel"/> class.
        /// </summary>
        public HarmonicModel() {
            this.HarmonicMotives = new List<HarmonicMotive>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the harmonic streams.
        /// </summary>
        /// <value>
        /// The harmonic streams.
        /// </value>
        [UsedImplicitly]
        public IList<HarmonicStream> HarmonicStreams {
            get {
                var list = new List<HarmonicStream>();
                var motives = this.HarmonicMotives;
                foreach (var motive in motives) {
                    if (motive.Length <= 8) {
                        continue;
                    }

                    list.Add(motive.HarmonicStream);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets or sets the harmonic motives.
        /// </summary>
        /// <value>
        /// The harmonic motives.
        /// </value>
        public IEnumerable<HarmonicMotive> HarmonicMotives {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<HarmonicMotive>>() != null);
                if (this.harmonicMotives == null) {
                    throw new InvalidOperationException("Harmonic motives are null.");
                }

                return this.harmonicMotives;
            }

            set => this.harmonicMotives = value ?? throw new ArgumentException(LocalizedMusic.String("Argument cannot be empty."), nameof(value));
        }

        /// <summary>
        /// Gets or sets the rhythmic structures.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<HarmonicBar> HarmonicBars {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<HarmonicBar>>() != null);
                if (this.harmonicBars != null) {
                    return this.harmonicBars;
                }

                var rs = new List<HarmonicBar>();
                foreach (var m in this.HarmonicMotives) {
                    rs.AddRange(m.HarmonicStream.HarmonicBars);
                }

                this.harmonicBars = rs;
                //// var rsd = (from r in rs select r).Distinct().OrderBy(x => x.Length).ToList(); ////.  x.ElementSchema);
                //// return rsd;
                return rs;
            }

            set => this.harmonicBars = value;
        }

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
        public static HarmonicModel GetNewModel(MusicalBlock musicalBlock) {
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

            ProcessLogger.Singleton.SendMessageEvent(null, LocalizedMusic.String("Analyzing harmony..."), 0); //// musicalBlock.Name
            var harmonicAnalysis = MusicalSettings.Singleton.SettingsAnalysis.HarmonicAnalysis; //// HarmonicAnalyzeType.DivisionByTicks
            var harmonicAnalyzer = new HarmonicAnalyzer(musicalBlock.Body);
            var harmonicChanges = harmonicAnalyzer.AnalyzeHarmony(model, harmonicAnalysis);

            harmonicChanges.ForAll(change => model.BlockChanges.Changes.Add(change));

            //// 2016 this.Body.MakeBars(model.Header.NumberOfBars, this.Header);    model.SourceMusicalBlock.NumberOfBars
            //// 2019/12 !?  musicalBlock.Body.SetHarmonicBasis(model.BlockChanges);         //// true

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
        public static HarmonicModel GetNewModel(string modelName, MusicalBlock musicalBlock) {
            Contract.Requires(musicalBlock != null);
            var model = new HarmonicModel {
                Name = modelName,
                IsSelected = false
            };
            if (model == null) {
                throw new ArgumentNullException(nameof(modelName));
            }

            model.Header = musicalBlock.Header;
            model.SourceMusicalBlock = musicalBlock;

            var list = new List<HarmonicBar>();
            foreach (var bar in musicalBlock.Body.Bars) {
                list.Add(bar.HarmonicBar);
            }

            model.HarmonicBars = list;

            return model;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Rhythmic of harmony.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public IList<RhythmicStructure> RhythmicOfHarmony() {
            var structs = new List<RhythmicStructure>();
            var bars = this.HarmonicBars;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var harBar in bars) {
                var rstruct = harBar.RhythmicStructure;  //// Clone?
                structs.Add(rstruct);
            }

            return structs;
        }

        /// <summary>
        /// Gets the harmonic motive.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public HarmonicMotive GetHarmonicMotive(int number) {
            var cnt = this.HarmonicMotives.Count();
            if (cnt == 0) {
                return null;
            }

            var localNumber = number;
            if (localNumber > cnt) {
                checked {
                    localNumber = ((number - 1) % cnt) + 1;
                }
            }

            //// HarmonicMotive motive = this.HarmonicMotives.ElementAt(localNumber);   
            var motive = (from m in this.HarmonicMotives where m.Number == localNumber select m).FirstOrDefault();
            return motive;
        }

        /// <summary>
        /// Adds the motive.
        /// </summary>
        /// <param name="motive">The motive.</param>
        public void AddMotive(HarmonicMotive motive) {
            Contract.Requires(motive != null);
            //// motive.Core = this;
            motive.Recompute();
            ((List<HarmonicMotive>)this.HarmonicMotives).Add(motive);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [UsedImplicitly]
        public override string ToString() {
            var system = this.Header.System;
            return system.HarmonicOrder.ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        /// <summary> Makes a deep copy of the BlockModel object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var model = new HarmonicModel {
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

        /// <summary>
        /// Gets grouped rhythmic structures.
        /// </summary>
        /// <returns>
        /// The grouped rhythmic structures.
        /// </returns>
        public IList<HarmonicBar> GetGroupedHarmonicBars() {
            var list = new List<HarmonicBar>();

            //// Structures from lines
            if (this.SourceMusicalBlock != null) {
                var tempList = new List<HarmonicBar>();
                foreach (var bar in this.SourceMusicalBlock.Body.Bars) {
                    tempList.Add(bar.HarmonicBar);
                }

                list.AddRange(tempList);
            }

            //// Grouping
            IList<HarmonicBar> structures = new List<HarmonicBar>();
            var groupList = (from ms in list
                             group ms by ms.StructuralOutline into g
                             select g).ToList();
            foreach (var g in groupList) {
                var s = g.FirstOrDefault();
                if (s == null) {
                    continue;
                }

                var ms = s;  //// Clone? //// new RhythmicStructure(s.RhythmicSystem, s.GetStructuralCode())
                ms.Occurrence = g.Count();
                structures.Add(ms);
            }

            return structures;
        }

        #region Material Extractor
        /// <summary>
        /// Extract harmonic material.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicMaterial ExtractHarmonicMaterial() {
            var system = this.Header.System;
            var thm = new HarmonicMaterial { Name = this.Name, HarmonicOrder = system.HarmonicOrder };
            //// dcm.AddToTHarmonicMaterial(thm);
            var list = this.ListOfStructures();

            var groupList = (from ms in list
                             group ms by ms.GetStructuralCode into g
                             select g).ToList();
            groupList.ForEach(g => {
                var hs = g.FirstOrDefault();
                if (hs == null) {
                    return;
                }

                var mhs = hs; //// clone?
                var chs = hs.GetClassStructure;
                hs.ClassCode = chs.GetStructuralCode;
                hs.Occurrence = g.Count();
                thm.Structures.Add(mhs);
            });

            //// dcm.SaveChanges();
            return thm;
        }

        #endregion

        /// <summary>
        /// Lists the of structures.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        private IEnumerable<HarmonicStructure> ListOfStructures() {
            var list = new Collection<HarmonicStructure>();
            foreach (var structure in from motive in this.HarmonicMotives
                                      from bar in motive.HarmonicStream.HarmonicBars
                                      from structure in bar.HarmonicStructures
                                      let sc = structure.GetStructuralCode
                                      where !string.IsNullOrEmpty(sc)
                                      select structure) {
                list.Add(structure);
            }

            return list;
        }
    }
}
