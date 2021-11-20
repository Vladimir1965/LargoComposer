using System.Collections.Generic;

namespace LargoModeler
{
    public class StaffElement
    {
        public StaffElement(StaffZone givenStaffZone, StaffBar givenStaffBar)
        {
            this.StaffZone = givenStaffZone;
            this.StaffBar = givenStaffBar;
            this.Beat = BeatValues.Beat;
        }

        public StaffZone StaffZone { get; set; }
        public StaffBar StaffBar { get; set; }
        public BeatValues Beat { get; set; }
    }
}
