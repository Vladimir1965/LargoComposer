namespace LargoModeler
{
    public class StaffZone
    {
        public StaffZone(string givenName)
        {
            this.Name = givenName;
            this.Level = 2;
            this.Beat = BeatValues.Beat;
            this.Orchestra = OrchestraValues.Piano;
            this.Voices = 2;
        }

        public BeatValues Beat { get; set; }
        public OrchestraValues Orchestra { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public byte Voices { get; set; }
    }
}
