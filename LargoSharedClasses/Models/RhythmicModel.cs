// <copyright file="RhythmicModel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Musical Block Model.
    /// </summary>
    [Serializable]
    public sealed class RhythmicModel : AbstractModel, ICloneable
    {
        #region Fields
        /// <summary>
        /// Rhythmic motives.
        /// </summary>
        [NonSerialized]
        private IEnumerable<RhythmicMotive> rhythmicMotives;

        /// <summary> Used Rhythmic Motives. </summary>
        private Dictionary<string, RhythmicMotive> usedRhythmicMotives;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicModel"/> class.
        /// </summary>
        public RhythmicModel() {
            this.RhythmicMotives = new List<RhythmicMotive>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicModel" /> class.
        /// </summary>
        /// <param name="markBlockModel">The mark block model.</param>
        public RhythmicModel(XElement markBlockModel) {
            this.RhythmicMotives = new List<RhythmicMotive>();
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public new XElement GetXElement {
            get {
                var xe = new XElement(
                    "Rhythmic",
                    new XAttribute("Name", this.Name),
                    new XAttribute("Order", this.RhythmicOrder));

                var xmotives = new XElement("Motives");
                foreach (var rstruct in this.RhythmicMotives) {
                    var xmotive = rstruct.GetXElement;
                    xmotives.Add(xmotive);
                }

                xe.Add(xmotives);
                return xe;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rhythmic order.
        /// </summary>
        /// <value>
        /// The rhythmic order.
        /// </value>
        public byte RhythmicOrder {
            get; set;
        }

        /// <summary>
        /// Gets or sets the rhythmic motives.
        /// </summary>
        /// <value>
        /// The rhythmic motives.
        /// </value>        
        public IEnumerable<RhythmicMotive> RhythmicMotives {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<RhythmicMotive>>() != null);
                if (this.rhythmicMotives == null) {
                    throw new InvalidOperationException("Rhythmic motives are null.");
                }

                return this.rhythmicMotives;
            }

            set => this.rhythmicMotives = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets the rhythmic structures by variance.
        /// </summary>
        /// <value> Property description. </value>
        public IList<RhythmicStructure> RhythmicStructuresOfMotives {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<RhythmicStructure>>() != null);

                var rs = new List<RhythmicStructure>();
                foreach (var m in this.RhythmicMotives) {
                    rs.AddRange(m.RhythmicStructures);
                }

                var structs = (from r in rs select r).Distinct().ToList();
                //// var sortedStructs = from rx in structs orderby rx.Level, rx.Variance select rx; //// rx.ToneLevel rx.ElementSchema
                //// OrderBy(x => x.Level.ToString().PadLeft(3) + x.ElementSchema).ToList(); ////.  x.ElementSchema);
                return structs;
            }
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
        [UsedImplicitly]
        public static RhythmicModel GetNewModel(MusicalBlock musicalBlock) {
            musicalBlock.RefreshHeader(); //// ????
            //// musicalBlock.Header.NumberOfLines = (byte)musicalBlock.Strip.Lines.Count;

            var model = GetNewModel(musicalBlock.Header.FileName, musicalBlock);
            model.Number = musicalBlock.Header.Number;
            model.SourceMusicalBlock = musicalBlock;
            model.Header = musicalBlock.Header;
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
        public static RhythmicModel GetNewModel(string modelName, MusicalBlock musicalBlock) {
            Contract.Requires(musicalBlock != null);
            var model = new RhythmicModel {
                Name = modelName,
                IsSelected = false
            };
            if (model == null) {
                throw new ArgumentNullException(nameof(modelName));
            }

            model.Header = musicalBlock.Header;
            model.SourceMusicalBlock = musicalBlock;
            model.RhythmicOrder = model.Header.System.RhythmicOrder;
            return model;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts to order.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        [UsedImplicitly]
        public void ConvertToSystem(RhythmicSystem givenSystem) {
            Contract.Requires(givenSystem != null);

            this.RhythmicOrder = givenSystem.Order;
            var cnt = this.RhythmicMotives.Count();
            if (cnt == 0) {
                return;
            }

            foreach (var m in this.RhythmicMotives) {
                m.ConvertToSystem(givenSystem);
            }
        }

        /// <summary>
        /// Gets the rhythmic motive.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public RhythmicMotive GetRhythmicMotive(int number) {
            var cnt = this.RhythmicMotives.Count();
            if (cnt == 0) {
                return null;
            }

            var localNumber = number;
            if (localNumber > cnt) {
                checked {
                    localNumber = ((number - 1) % cnt) + 1;
                }
            }

            //// RhythmicMotive motive = this.RhythmicMotives.ElementAt(localNumber);   
            var motive = (from m in this.RhythmicMotives where m.Number == localNumber select m).FirstOrDefault();
            return motive;
        }

        /// <summary>
        /// Adds the motive.
        /// </summary>
        /// <param name="motive">The motive.</param>
        public void AddMotive(RhythmicMotive motive) {
            Contract.Requires(motive != null);
            //// motive.Core = this;
            ((List<RhythmicMotive>)this.RhythmicMotives).Add(motive);
        }
        #endregion

        /// <summary>
        /// Appends the rhythmic motives.
        /// Main algorithm to determine motivic classes and their instances
        /// </summary>
        /// <param name="itemGroups">The item groups.</param>
        public void AppendRhythmicMotives(List<MelodicItemGroup> itemGroups) {
            var lastRhythmicIdentifier = string.Empty;

            //// All motivic items ordered by length (descending) and identifier
            var rhythmicItemGroups = (from ig in itemGroups orderby ig.Length descending, ig.RhythmicIdentifier select ig).ToList();

            RhythmicMotive rhythmicMotive = null;
            //// var numberWithinLength = 0; var lastLength = 0;
            foreach (var itemGroup in rhythmicItemGroups) {
                //// if (itemGroup.Length != lastLength) { numberWithinLength = 0; lastLength = itemGroup.Length; }
                var ident = itemGroup.RhythmicIdentifier;
                if (ident != lastRhythmicIdentifier) {  //// Step to next new motive
                    rhythmicMotive = itemGroup.RhythmicMotive(0, string.Empty);
                    if (rhythmicMotive == null) {
                        return;
                    }

                    this.AddMotive(rhythmicMotive);
                    lastRhythmicIdentifier = ident;
                }

                if (rhythmicMotive != null) {
                    rhythmicMotive.Occurrence++;
                    var area = itemGroup.GetArea();
                    this.SourceMusicalBlock.Body.MarkRhythmicMotive(rhythmicMotive, area);
                }
            }

            this.CompleteMotives();
        }

        /// <summary>
        /// Completes the motives.
        /// </summary>
        public void CompleteMotives() {
            var rhythmicMotiveNumber = 0;
            var orderedMotives = (from m in this.RhythmicMotives orderby m.Occurrence descending, m.Length descending select m).ToList();
            foreach (var motive in orderedMotives) {
                rhythmicMotiveNumber++;
                //// string motiveName = string.Format(CultureInfo.InvariantCulture,"T{0}/R{1}", ("0" + musicalLine.LineIndex.ToString(CultureInfo.CurrentCulture)).Right(2), ("0000" + this.MelodicAnalyzer.RhythmicMotiveNumber.ToString(CultureInfo.CurrentCulture)).Right(4));
                //// var motiveName = string.Format(CultureInfo.InvariantCulture, "R{0}", ("0000" + rhythmicMotiveNumber.ToString(CultureInfo.CurrentCulture)).Right(4));
                var motiveName = MusicalProperties.GetMotiveName(string.Empty, rhythmicMotiveNumber, motive.Length); //// rhythmicMotiveNumber
                motive.Number = rhythmicMotiveNumber;
                motive.Name = motiveName;
            }
        }

        #region Unique motives
        /// <summary>
        /// Get UniqueTRhythmicMotive.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique Identifier.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public RhythmicMotive GetUniqueRhythmicMotive(string uniqueIdentifier) {
            Contract.Requires(uniqueIdentifier != null);

            if (this.usedRhythmicMotives == null) {
                this.usedRhythmicMotives = new Dictionary<string, RhythmicMotive>();
            }

            var trm = this.usedRhythmicMotives.ContainsKey(uniqueIdentifier) ? this.usedRhythmicMotives[uniqueIdentifier] : null;
            if (trm != null) {
                return trm;
            }

            //// Lock needed here
            var rhythmicMotiveList = this.RhythmicMotives;
            foreach (var rhythmicMotive in
                rhythmicMotiveList.Where(rhythmicMotive => rhythmicMotive != null && string.CompareOrdinal(rhythmicMotive.UniqueIdentifier, uniqueIdentifier) == 0)) {
                this.usedRhythmicMotives[uniqueIdentifier] = rhythmicMotive;
                return rhythmicMotive; //// Avoid multiple or conditional return statements.
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Gets grouped rhythmic structures.
        /// </summary>
        /// <returns>
        /// The grouped rhythmic structures.
        /// </returns>
        public IList<RhythmicStructure> GetGroupedRhythmicStructures() {
            var rhythmicMaterial = this.ExtractRhythmicMaterial();

            var list = (from s in rhythmicMaterial.Structures
                        orderby s.ToneLevel descending, s.FormalBehavior.Variance ascending, s.Occurrence descending
                        select s).ToList();

            /* 2020/12
            //// Regular structures.
            var regular = RhythmicStructureFactory.RegularStructures(this.RhythmicOrder); //// Header.System.RhythmicOrder
            list.AddRange(regular);
            

            //// Structures from harmony.
            if (this.SourceMusicalBlock != null) {
                var tempList = new List<RhythmicStructure>();
                foreach (var bar in this.SourceMusicalBlock.Body.Bars) {
                    var structure = bar?.HarmonicBar?.RhythmicStructure;
                    if (structure == null) {
                        continue;
                    }

                    tempList.Add(structure);
                }

                list.AddRange(tempList);
            }
            */

            //// Structures from lines
            if (this.SourceMusicalBlock != null) {
                var tempList = new List<RhythmicStructure>();
                foreach (var bar in this.SourceMusicalBlock.Body.Bars) {
                    foreach (var element in bar.Elements) {
                        var structure = element?.Status?.RhythmicStructure;
                        if (structure == null) {
                            continue;
                        }

                        tempList.Add(structure);
                    }
                }

                list.AddRange(tempList);
            }

            //// Grouping
            IList<RhythmicStructure> structures = new List<RhythmicStructure>();
            var groupList = (from ms in list
                             group ms by ms.GetStructuralCode into g
                             select g).ToList();
            foreach (var g in groupList) {
                var s = g.FirstOrDefault();
                if (s == null) {
                    continue;
                }

                var ms = s;  //// Clone? //// new RhythmicStructure(s.RhythmicSystem, s.GetStructuralCode())
                ms.Occurrence = g.Count();
                ms.DetermineLevel(); //// 2019/01
                ms.DetermineBehavior(); //// 2019/01
                structures.Add(ms);
            }

            return structures;
        }

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [UsedImplicitly]
        public override string ToString() {
            return this.RhythmicOrder.ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        /// <summary> Makes a deep copy of the BlockModel object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var model = new RhythmicModel {
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
        /// Extract rhythmic material.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicMaterial ExtractRhythmicMaterial() {
            //// var dcm = DataBridgeMaterial.GetMaterialContext;
            var material = new RhythmicMaterial(this.Header) { 
                RhythmicOrder = this.RhythmicOrder 
            }; 
            //// Name = this.Name, CoreId = rhythmicCore.TRhythmicCore.Id
            //// dcm.AddToTRhythmicMaterial(trm);
            var list = new Collection<RhythmicStructure>();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var motive in this.RhythmicMotives) {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var structure in motive.RhythmicStructures) {
                    var sc = structure.GetStructuralCode;
                    if (string.IsNullOrEmpty(sc)) {
                        continue;
                    }

                    list.Add(structure);
                }
            }

            //// Grouping
            var groupList = (from ms in list
                             group ms by ms.GetStructuralCode into g
                             select g).ToList();
            foreach (var g in groupList) {
                var s = g.FirstOrDefault();
                if (s == null) {
                    continue;
                }

                var ms = s;  //// Clone? //// new RhythmicStructure(s.RhythmicSystem, s.GetStructuralCode())
                ms.Occurrence = g.Count();
                ms.DetermineLevel(); //// 2019/01
                ms.DetermineBehavior(); //// 2019/01
                material.Structures.Add(ms);
            }

            //// dcm.SaveChanges();
            return material;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Regulars the rhythmic structures.
        /// </summary>
        /// <param name="metricBase">The metric base.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        private IEnumerable<RhythmicStructure> RegularRhythmicStructures(byte metricBase) {
            Contract.Ensures(Contract.Result<IEnumerable<RhythmicStructure>>() != null);

            var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, this.RhythmicOrder);
            var structs = new List<RhythmicStructure>();

            var s1 = string.Format(CultureInfo.CurrentCulture, "1,{0}*0,", this.RhythmicOrder - 1);
            var r1 = new RhythmicStructure(rsystem, s1);
            r1.DetermineBehavior();
            structs.Add(r1);

            var s1P = string.Format(CultureInfo.CurrentCulture, "2,{0}*0,", this.RhythmicOrder - 1);
            var r1P = new RhythmicStructure(rsystem, s1P);
            r1P.DetermineBehavior();
            structs.Add(r1P);

            for (var d = 1; d < this.RhythmicOrder - 1; d++) {
                if (d % metricBase != 0) {
                    continue;
                }

                if (this.RhythmicOrder % d != 0) {
                    continue;
                }

                var sb = new StringBuilder();
                var length = this.RhythmicOrder / d;

                var s = string.Format(CultureInfo.CurrentCulture, "1,{0}*0,", length - 1);
                for (var i = 0; i < d; i++) {
                    sb.Append(s);
                }

                sb.Remove(sb.Length - 1, 1);

                var rs = new RhythmicStructure(rsystem, sb.ToString());
                rs.DetermineBehavior();

                structs.Add(rs);
            }

            return structs;
        }
        #endregion
    }
}
