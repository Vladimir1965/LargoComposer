// <copyright file="StructuralVariety.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Variety of structures.
    /// </summary>
    /// <typeparam name="T">Structure - the generic type parameter.</typeparam>
    /// <remarks>
    /// Variety represents variety of structures, modalities or other patterns inside
    /// of a given GSystem. It is used as a common superclass for harmonic and rhythmical system varieties.
    /// </remarks>
    [Serializable]
    [XmlInclude(typeof(HarmonicSystem))]
    [XmlInclude(typeof(RhythmicSystem))]
    [XmlInclude(typeof(MelodicSystem))]
    [XmlInclude(typeof(HarmonicStructure))]
    [XmlInclude(typeof(RhythmicStructure))]
    [XmlInclude(typeof(MelodicStructure))]
    public class StructuralVariety<T> where T : class, IGeneralStruct, IModalStruct, new() {
        #region Fields
        /// <summary>  Array of structures. </summary>
        private List<T> structList; //// readonly

        /// <summary>
        /// General system.
        /// </summary>
        private GeneralSystem gsystem; //// readonly

        /// <summary>
        /// Special numbers.
        /// </summary>
        private byte n2, n3, n4, n6;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.  Serializable.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public StructuralVariety(GeneralSystem givenSystem) { // : base()
            this.structList = new List<T>();
            this.GSystem = givenSystem;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="StructuralVariety&lt;T&gt;"/> class from being created.
        /// </summary>
        private StructuralVariety() {
        }

        #endregion

        #region Properties
        /// <summary> Gets or sets modality of structures. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public BinaryStructure Modality { get; set; }

        /// <summary>  Gets array of structures. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<T> StructList
        {
            get
            {
                Contract.Ensures(Contract.Result<Collection<T>>() != null);
                if (this.structList == null) {
                    throw new InvalidOperationException("List of structures is null.");
                }

                return new Collection<T>(this.structList);
            }

            //// set { this.structList = value; }
        }

        /// <summary>  Gets or sets qualifier. </summary>
        /// <value> Property description. </value>
        public GeneralQualifier Qualifier { private get; set; }

        /// <summary>  Gets or sets maximum number of structures. </summary>
        /// <value> Property description. </value>
        public int LimitCount { private get; set; }

        /// <summary>  Gets or sets type of variety. </summary>
        /// <value> Property description. </value>
        public StructuralVarietyType VarType { private get; set; }

        /// <summary> Gets or sets abstract G-System. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private GeneralSystem GSystem
        {
            get
            {
                Contract.Ensures(Contract.Result<GeneralSystem>() != null);
                return this.gsystem ?? new GeneralSystem();
            }

            set => this.gsystem = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            if (this.Qualifier != null) {
                s.Append(this.Qualifier);
            }

            //// if (this.StructList == null) { return s.ToString();  }
            foreach (var bs in this.StructList) {
                s.Append(bs);
            }

            return s.ToString();
        }
        #endregion

        /// <summary>
        /// Sets the struct list.
        /// </summary>
        /// <param name="list">The list.</param>
        [UsedImplicitly]
        public void SetStructList(IEnumerable<T> list) {
            Contract.Requires(list != null);

            this.structList = list.ToList();
        }

        /// <summary> Returns the next optimal harmonic structure. </summary>
        /// <param name="givenRequest">General musical request.</param>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        public T OptimalNextStructForRequest(GeneralRequest givenRequest) {
            var optimalStruct = default(T);
            var extremeTotal = -10000000f;
            foreach (var str in this.StructList) {
                str.WriteBehaviorToProperties();
                var total = str.SumForRequest(givenRequest);
                if (total <= extremeTotal) {
                    continue;
                }

                extremeTotal = total;
                optimalStruct = str;
            }

            if ((optimalStruct == null) && (this.StructList.Count > 0)) {
                optimalStruct = this.StructList[0];
            }

            return optimalStruct;
        }

        /// <summary> Generate all instances of structures. </summary>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public bool GenerateInstances(byte givenLevel) {
            var r = (decimal)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            for (decimal num = 1; num <= r; num++) {
                var level = FiguralStructure.DetermineLevel(this.GSystem.Order, num);
                if (level == givenLevel) {
                    this.AddStructure(num);
                }

                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }
            }

            return this.StructList.Count > 0;
        }

        /// <summary> Generate all classes, i.e. representatives of structures (without transpositions). </summary>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public bool GenerateClasses(byte givenLevel) {
            const int maxSize = 1000000;
            var rsize = (decimal)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            var r = (int)Math.Min(rsize, maxSize);
            if (r < 0) {
                return false;
            }

            var marked = new BitArray(r + 1, false); // bool[] marked = new bool[r+1];
            //// for (g=this.GSystem.Degree; g<= r; g+=this.GSystem.Degree) marked.Set(g,true);
            var g = 1;
            while (g <= r) {
                if (g < marked.Count && !marked.Get(g)) { // !marked[g]
                    if (this.StructList.Count >= this.LimitCount) {
                        return true; //// Avoid multiple or conditional return statements.
                    }

                    decimal gl = g;
                    var level = FiguralStructure.DetermineLevel(this.GSystem.Order, gl);
                    if (givenLevel == level) {
                        this.AddStructure(gl);
                    }

                    this.MarkInstancesOfClass(rsize, r, marked, g);
                }

                g = g + 1;
            }

            return this.StructList.Count > 0;
        }

        /// <summary> Generate all classes, i.e. representatives of structures (without transpositions). </summary>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        public bool GenerateBinaryClasses(byte givenLevel) {
            var r = (long)Math.Pow(this.GSystem.Degree, this.GSystem.Order - 1) - 1;
            long num;
            byte level;
            for (num = 1; num <= r; num += 2) {
                level = BinaryNumber.DetermineLevel(this.GSystem.Order, num);
                if (level == givenLevel) {
                    var classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, num);
                    if (num == classNumber) {
                        this.AddStructure(num);
                    }
                }

                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }
            }

            num = (long)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            level = BinaryNumber.DetermineLevel(this.GSystem.Order, num);
            if (level == givenLevel) {
                this.AddStructure(num);
            }

            return this.StructList.Count > 0;
        }

        /// <summary> Generate variety. </summary>
        public void Generate() {
            switch (this.VarType) {
                case StructuralVarietyType.Classes:
                    this.GenerateClasses();
                    break;
                case StructuralVarietyType.Instances:
                    this.GenerateInstances();
                    break;
                case StructuralVarietyType.BinaryClasses:
                    this.GenerateBinaryClasses();
                    break;
                case StructuralVarietyType.BinarySubstructuresOfModality:
                    this.GenerateBinarySubstructures(this.Modality);
                    break;
                case StructuralVarietyType.FiguralSubstructuresOfModality:
                    this.GenerateFiguralSubstructures(this.Modality);
                    break;
                case StructuralVarietyType.RhythmicModalityClasses:
                    this.GenerateRhythmicModalityClasses();
                    break;
                case StructuralVarietyType.RhythmicMetricClasses:
                    this.GenerateMetricClasses();
                    break;
                case StructuralVarietyType.None:
                    break;
                case StructuralVarietyType.MelodicStructuresOfModality:
                    break;
                //// resharper default: break;
            }

            this.structList?.Sort();
        }

        /// <summary> Sort array of structures with regard to given item. </summary>
        /// <param name="property">General musical property.</param>
        /// <param name="givenDirection">Sort direction.</param>
        public void SortStructList(GenProperty property, GenSortDirection givenDirection) {
            var comparer = new ComparerGenStruct<T>(property, givenDirection);
            this.structList?.Sort(0, this.StructList.Count, comparer);
        }

        #region Private static
        /// <summary>
        /// Is Valid Composed Schema.
        /// </summary>
        /// <param name="schema">Schema of the structure.</param>
        /// <returns> Returns value. </returns>
        private static bool IsValidComposedSchema(IList<byte> schema) {
            Contract.Requires(schema != null);
            var status = schema != null;
            return status;
        }
        #endregion

        #region Private
        /// <summary>
        /// Marks the instances of class.
        /// </summary>
        /// <param name="rsize">The r-system size.</param>
        /// <param name="r">The r.</param>
        /// <param name="marked">The marked.</param>
        /// <param name="g">The g.</param>
        private void MarkInstancesOfClass(decimal rsize, int r, BitArray marked, int g) {
            var u = g;
            while (u != 0 && u < r && u > 0 && u < marked.Count && !marked.Get(u)) { // marked[u]
                if (u < marked.Count) {
                    marked.Set(u, true);   // marked[u]=true;
                }
                ////  time optimization of u = (int)((decimal)(u*this.GSystem.Degree) % rsize);
                decimal ul = u;
                //// if (this.GSystem.Degree == 2) {  ((long)ul) <<= 1; }  else {
                ul *= this.GSystem.Degree;
                //// }
                while (ul > rsize) {
                    ul -= rsize;
                }

                u = (int)ul;
            }
        }

        /// <summary> Creates structures Y as substructures of X. </summary>
        /// <param name="structureX">Number of master structure.</param>
        /// <param name="fromBit">Bit to start with.</param>
        /// <param name="restBits">Number of not finished bits.</param>
        /// <param name="structureY">Resulting structure.</param>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        private bool RecursiveAdd(FiguralNumber structureX, byte fromBit, byte restBits, FiguralNumber structureY) { //// cyclomatic complexity 10:11
            Contract.Requires(structureX != null);
            if (structureX == null) {
                return false;
            }

            var fsX = new FiguralNumber(structureX);
            if (this.StructList.Count >= this.LimitCount || structureY == null) {
                return false;
            }

            if (restBits == 0) {
                this.AppendStructureFromNumber(structureY);
            }
            else { // Yet > 0
                for (var j = fromBit; j < this.GSystem.Order; j++) {
                    if (!structureX.IsOn(j)) {
                        continue; // j in aNumX
                    }

                    structureX.SetElement(j, 0);  // aNumX - [j]
                    for (byte d = 1; d < this.GSystem.Degree; d++) {
                        structureY.SetElement(j, d);  // aNumY + [j]
                        byte nextRestBits;
                        checked {
                            nextRestBits = (byte)(restBits - 1);
                        }

                        if (!this.RecursiveAdd(fsX, j, nextRestBits, structureY)) {
                            return false; //// Avoid multiple or conditional return statements.
                        }

                        structureY.SetElement(j, 0);  // aNumY - [j]
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Appends the structure from number.
        /// </summary>
        /// <param name="structureY">The structure Y.</param>
        private void AppendStructureFromNumber(FiguralNumber structureY) {
            Contract.Requires(structureY != null);

            structureY.DetermineINumber();
            if (structureY.DecimalNumber <= 0) {
                return;
            }

            var figStruct = this.CreateStructure(structureY.DecimalNumber); ////  typeof(FiguralStructure)
            var ok1 = !figStruct.IsEmptyStruct();
            if (!ok1) {
                return;
            }

            var ok2 = this.Convenient(figStruct);
            if (ok2) {
                this.StructList.Add(figStruct);
            }
        }
        #endregion

        #region Generation of substructures

        /// <summary> Returns if Qualifier allows adding of this structure. </summary>
        /// <param name="generalStructure">General musical structure.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private bool Convenient(IGeneralStruct generalStructure) {
            if (generalStructure == null || generalStructure.IsEmptyStruct()) {
                return false;
            }

            if (!generalStructure.IsValidStruct()) {
                return false;
            }

            return this.Qualifier == null || this.Qualifier.Convenient(generalStructure);
        }

        /// <summary> Generate all substructures of the given modality. </summary>
        /// <param name="modality">Abstract modality.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private bool GenerateBinarySubstructures(IGeneralStruct modality) {
            if (modality == null) {
                return false;
            }

            byte level = modality.Level, lev;
            var emptyNum = 0L; // 0x00000000; BinaryStructure.BitAt(0);
            for (lev = 0; lev <= level; lev++) {
                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }

                this.RecursiveAdd((long)modality.DecimalNumber, 0, lev, ref emptyNum);
            }

            return this.StructList.Count > 0;
        }
        #endregion

        #region Generation of figural substructures

        /// <summary>
        /// Generate all substructures of the given modality.
        /// </summary>
        /// <param name="modality">Abstract modality.</param>
        private void GenerateFiguralSubstructures(BinaryStructure modality) {
            if (modality == null) {
                return;
            }

            byte level = modality.Level, lev;
            var emptyStruct = new FiguralNumber(this.GSystem, 0);
            for (lev = 0; lev <= level; lev++) {
                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }

                var figuralModality = new FiguralStructure(this.GSystem, 0);
                for (byte j = 0; j < this.GSystem.Order; j++) {
                    figuralModality.SetElement(j, (byte)(modality.IsOn(j) ? 1 : 0));
                }
                //// RecursiveAdd(fsModality,0,lev,emptyStruct);

                this.RecursiveAdd(modality, 0, lev, emptyStruct);
            }
        }
        #endregion

        #region Generation of instances

        /// <summary>
        /// Generate all instances of structures.
        /// </summary>
        private void GenerateInstances() {
            var r = (decimal)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            for (decimal num = 1; num <= r; num++) {
                this.AddStructure(num);
                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }
            }
        }
        #endregion

        #region Generation of classes

        /// <summary>
        /// Generate all classes, i.e. representatives of structures (without transpositions).
        /// </summary>
        private void GenerateClasses() {
            const int maxSize = 1000000;
            var rsize = (decimal)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            var r = (int)Math.Min(rsize, maxSize);
            if (r < 0) {
                return;
            }

            var marked = new BitArray(r + 1, false); // bool[] marked = new bool[r+1];
            ///// for (g=this.GSystem.Degree; g<= r; g+=this.GSystem.Degree) marked.Set(g,true);
            var g = 1;
            while (g <= r) {
                if (g < marked.Count && !marked.Get(g)) { // !marked[g]
                    if (this.StructList.Count >= this.LimitCount) {
                        return;
                    }

                    decimal gl = g;
                    //// // // byte level = BinaryStructure.DetermineLevel(this.GSystem.Order, gl);
                    this.AddStructure(gl);
                    this.MarkInstancesOfClass(g, r, marked);
                }

                g = g + 1;
            }
        }

        /// <summary>
        /// Marks the instances of class.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="r">The r.</param>
        /// <param name="marked">The marked.</param>
        private void MarkInstancesOfClass(int g, int r, BitArray marked) {
            var u = g;
            while (u != 0 && u < r && u > 0 && u < marked.Count && !marked.Get(u)) { // marked[u]
                if (u < marked.Count) {
                    marked.Set(u, true);   // marked[u]=true;
                }
                //// //  time optimization of u = (int)((decimal)(u*this.GSystem.Degree) % rsize);
                decimal ul = u;
                ////  if (this.GSystem.Degree == 2) { ((long)ul) <<= 1; } else { 
                ul *= this.GSystem.Degree;
                //// } 
                while (ul > r) {
                    ul -= r;
                }

                u = (int)ul;
            }
        }

        /// <summary>
        /// Generate all classes, i.e. representatives of structures (without transpositions).
        /// </summary>
        private void GenerateBinaryClasses() {
            var r = (decimal)Math.Pow(this.GSystem.Degree, this.GSystem.Order - 1) - 1;
            long num;
            for (num = 1; num <= r; num += 2) {
                decimal classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, num);
                if (num == classNumber) {
                    this.AddStructure(num);
                }

                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }
            }

            num = (long)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            this.AddStructure(num);
        }

        /// <summary>
        /// Generate all classes, i.e. representatives of structures (without transpositions).
        /// </summary>
        [ContractVerification(false)]
        private void GenerateRhythmicModalityClasses() {
            const byte divisor2 = 2;
            const byte divisor3 = 3;
            const byte divisor4 = 4;
            const byte divisor6 = 6;
            this.n2 = (byte)(this.GSystem.Order / divisor2);
            this.n3 = (byte)(this.GSystem.Order / divisor3);
            this.n4 = (byte)(this.GSystem.Order / divisor4);
            this.n6 = (byte)(this.GSystem.Order / divisor6);
            var lastIndex = (byte)(this.GSystem.Order - 1);  // for k=36: 24  
            var r = (long)Math.Pow(this.GSystem.Degree, lastIndex) - 1;
            long num;
            for (num = 1; num <= r; num += 1) { // for k=36: += 2
                if (this.GSystem.Order > DefaultValue.RhythmicOrder) { //// 8, 12
                    if (!this.IsSeparableStructure(num)) {
                        continue;
                    }
                }

                var classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, num);
                if (num == classNumber) {
                    this.AddValidRhythmicModalitySchema(num);
                    continue;
                    //// return; 2020/11
                }

                if (this.StructList.Count >= this.LimitCount) {
                    break;
                }
            }

            num = (long)Math.Pow(this.GSystem.Degree, this.GSystem.Order) - 1;
            this.AddStructure(num);
        }

        /// <summary>
        /// Adds the valid rhythmic modality schema.
        /// </summary>
        /// <param name="num">The number.</param>
        private void AddValidRhythmicModalitySchema(long num) {
            var schema = BinaryNumber.DistSchema(this.GSystem.Order, num);
            var lev = (byte)schema.Count;
            if ((this.GSystem.Order >= (byte)RhythmicOrder.R24) && (lev > 8)) {
                return;
            }

            var lastIndex = (byte)(lev - 1);
            var lastElement = schema[lastIndex];
            var status = this.GSystem.Order <= DefaultValue.HarmonicOrder || IsValidComposedSchema(schema);

            //// Simplicity condition
            if (status) {
                status = this.IsSimpleSchema(lastIndex, schema, lev, lastElement);
            }

            if (!status) {
                return;
            }

            this.AddStructure(num);
        }

        #endregion

        /// <summary> Creates on structure. </summary>
        /// <param name="number">Number of instance.</param>
        /// <returns> Returns value. </returns>
        private T CreateStructure(decimal number) { //// Type type
            //// Contract.Requires(type != null);
            var obj = (T)Activator.CreateInstance(typeof(T), this.GSystem, number);
            //// T obj = new T { GSystem = this.GSystem, Modality = this.Modality, DecimalNumber = number };
            obj.DetermineBehavior();
            return obj; // default(T)
        }

        /// <summary> Creates structures Y as substructures of X. </summary>
        /// <param name="structureX">Master structure.</param>
        /// <param name="fromBit">Bit to start with.</param>
        /// <param name="restBits">Number of not finished bits.</param>
        /// <param name="structureY">Resulting structure.</param>
        /// <returns> Returns value.</returns>
        private bool RecursiveAdd(BinaryStructure structureX, byte fromBit, byte restBits, FiguralNumber structureY) {
            Contract.Requires(structureX != null);
            var fsX = new BinaryStructure(structureX);
            if (this.StructList.Count >= this.LimitCount || structureY == null) {
                return false;
            }

            if (restBits == 0) {
                structureY.DetermineINumber();
                if (structureY.DecimalNumber <= 0) {
                    return true;
                }

                var figStruct = this.CreateStructure(structureY.DecimalNumber); //// typeof(FiguralStructure)
                var ok1 = !figStruct.IsEmptyStruct() && figStruct.IsValidStruct();
                if (!ok1) {
                    return true;
                }

                var ok2 = this.Convenient(figStruct);
                if (ok2) {
                    this.StructList.Add(figStruct);
                }
            }
            else { // Yet > 0
                for (var j = fromBit; j < this.GSystem.Order; j++) {
                    if (!fsX.IsOn(j)) {
                        continue; // j in aNumX
                    }

                    fsX.Off(j);  // aNumX - [j]
                    for (byte d = 1; d < this.GSystem.Degree; d++) {
                        structureY.SetElement(j, d);  // aNumY + [j]
                        byte nextRestBits;
                        checked {
                            nextRestBits = (byte)(restBits - 1);
                        }

                        if (!this.RecursiveAdd(fsX, j, nextRestBits, structureY)) {
                            return false; //// Avoid multiple or conditional return statements.
                        }

                        structureY.SetElement(j, 0);  // aNumY - [j]
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates structures Y as substructures of X.
        /// </summary>
        /// <param name="numberX">Number of master structure.</param>
        /// <param name="fromBit">Bit to start with.</param>
        /// <param name="restBits">Number of not finished bits.</param>
        /// <param name="numberY">Resulting structure.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private bool RecursiveAdd(long numberX, byte fromBit, byte restBits, ref long numberY) {
            if (this.StructList.Count >= this.LimitCount) {
                return false;
            }

            if (restBits == 0) {
                if (numberY <= 0) {
                    return true;
                }

                var str = this.CreateStructure(numberY); //// typeof(BinaryStructure)
                var ok1 = !str.IsEmptyStruct();
                if (!ok1) {
                    return true;
                }

                var ok2 = this.Convenient(str);
                if (ok2) {
                    this.StructList.Add(str);
                }
            }
            else { // Yet > 0
                var order = this.GSystem.Order;
                for (var j = fromBit; j < order; j++) {
                    if ((numberX & BinaryNumber.BitAt(j)) == (decimal)0) {
                        continue; // j in aNumX
                    }

                    numberX = numberX & BinaryNumber.NotBitAt(j); // aNumX - [j]
                    numberY = numberY | BinaryNumber.BitAt(j); // aNumY + [j]
                    if (!this.RecursiveAdd(numberX, j, (byte)(restBits - 1), ref numberY)) {
                        return false; //// Avoid multiple or conditional return statements.
                    }

                    numberY = numberY & BinaryNumber.NotBitAt(j); // aNumY - [j];
                }
            }

            return true;
        }

        /// <summary>
        /// Generate all metric classes, i.e. symmetric representatives of structures.
        /// </summary>
        private void GenerateMetricClasses() {
            var order = this.GSystem.Order;
            for (byte d = 1; d <= order; d++) {
                if (order % d != 0) {
                    continue;
                }

                var bs = new BinaryNumber(this.GSystem, 0);
                for (byte e = 0; e < order; e += d) {
                    bs.On(e);
                }

                this.AddStructure(bs.Number);
            }
        }

        /// <summary>
        /// Is Separable Structure.
        /// </summary>
        /// <param name="num">Structural Number.</param>
        /// <returns> Returns value. </returns>
        private bool IsSeparableStructure(long num) {
            var level = BinaryNumber.DetermineLevel(this.GSystem.Order, num);
            if (level <= 4) {
                return true;
            }

            if (!BinaryNumber.ElementIsOn(num, this.n2)) {
                return false;
            }

            if (this.GSystem.Order == 16) {
                if (!BinaryNumber.ElementIsOn(num, this.n4)) {
                    return false; //// Avoid multiple or conditional return statements.
                }
            }
            else {
                if (!(BinaryNumber.ElementIsOn(num, this.n3)
                    || BinaryNumber.ElementIsOn(num, this.n4)
                    || BinaryNumber.ElementIsOn(num, this.n6))) {
                    return false; //// Avoid multiple or conditional return statements.
                }
            }

            return true;
        }

        /// <summary> Add modality with given number to array of structures. </summary>
        /// <param name="number">Number of instance.</param>
        private void AddStructure(decimal number) {
            var obj = this.CreateStructure(number); //// typeof(T)
            var ok = this.Convenient(obj);
            if (ok) {
                this.StructList.Add(obj);
            }
        }

        /// <summary>
        /// Determines whether [is simple schema] [the specified last index].
        /// </summary>
        /// <param name="lastIndex">The last index.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="level">The level.</param>
        /// <param name="lastElement">The last element.</param>
        /// <returns>
        ///   <c>True</c> if [is simple schema] [the specified last index]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSimpleSchema(byte lastIndex, IList<byte> schema, byte level, byte lastElement) {
            Contract.Requires(schema != null);
            if (this.GSystem.Order <= 16 || level <= 3) {
                return true; //// originally 8  but failed 313144 !? in 16 
            }

            var status = true;
            for (byte j = 0; j < lastIndex; j++) {
                var elem = schema[j];
                if ((lastElement % elem == 0) || MathSupport.GreatestCommonDivisor(lastElement, elem) != 1) {
                    continue;
                }

                status = false;
                break;
            }

            return status;
        }
    }
}