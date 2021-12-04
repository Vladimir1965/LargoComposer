using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using System.Collections.Generic;

namespace LargoModeler
{
    public class StaffZone
    {
        public StaffZone(string givenName)
        {
            this.Name = givenName;
            this.Level = 2;
            this.OrchestraValue = OrchestraValues.Piano;
            this.Lines = 2;
            this.staffElements = new List<StaffElement>(); //// ObservableCollection
            this.Loudness = MusicalLoudness.MeanLoudness;
        }

        public OrchestraValues OrchestraValue { get; set; }
        public OrchestraUnit Orchestra { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public byte Lines { get; set; }
        public List<StaffElement> staffElements { get; set; }
        public LineStatus Status { get; set; }
        public MusicalLoudness Loudness { get; set; }
    }
}
