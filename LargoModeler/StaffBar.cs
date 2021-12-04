using LargoSharedClasses.Music;
using System.Collections.Generic;

namespace LargoModeler
{
    public class StaffBar
    {
        #region Constructors
        public StaffBar(int givenNumber)
        {
            this.Number = givenNumber;
            this.Length = 1;
        }
        #endregion

        public int Number { get; set; }

        public string Harmony {
            get {
                if (this.HarmonicBar == null) {
                    return string.Empty;
                }

                return this.HarmonicBar.ChordsToString;
            }
        }

        public HarmonicBar HarmonicBar { get; set; }

        public string Shape {
            get {
                if (this.HarmonicBar?.RhythmicShape == null) {
                    return string.Empty;
                }

                return this.HarmonicBar.RhythmicShape.DistanceSchema;
            }
        }

        public byte Length { get; set; }
    }
}
