using LargoSharedClasses.Music;
using System.Collections.Generic;

namespace LargoModeler
{
    public class StaffElement
    {
        #region Constructors
        public StaffElement(StaffZone givenStaffZone, StaffBar givenStaffBar, BeatValues givenBeat)
        {
            this.StaffZone = givenStaffZone;
            this.StaffBar = givenStaffBar;
            this.Beat = givenBeat;
        }
        #endregion

        #region Properties
        public StaffZone StaffZone { get; set; }

        public StaffBar StaffBar { get; set; }

        public BeatValues Beat { get; set; }

        public RhythmicStructure RhythmicStructure { get; set; }
        #endregion

        public void DetermineRhythm(RhythmicSystem givenRhythmicSystem)
        {
            var rsystem = givenRhythmicSystem;
            string code1 = string.Empty;
            switch (this.Beat) {
                case BeatValues.Beat: {
                        code1 = "1,0,0,0,0,0,1,0,0,0,0,0";
                        break;
                    }
                case BeatValues.Empty: {
                        code1 = "2,0,0,0,0,0,0,0,0,0,0,0";
                        break;
                    }
                case BeatValues.Light: {
                        code1 = "2,0,0,1,0,0,2,0,0,1,0,0";
                        break;
                    }
                case BeatValues.Complement: {
                        code1 = "1,0,0,1,1,1,1,0,0,1,0,0";
                        break;
                    }
            }

            var r1 = new RhythmicStructure(rsystem, code1);
            r1.DetermineBehavior();
            this.RhythmicStructure = r1;
        }
    }
}

