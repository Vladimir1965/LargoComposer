﻿using System.Collections.Generic;

namespace LargoModeler
{
    public class StaffBar
    {
        public StaffBar(int givenNumber)
        {
            this.Number = givenNumber;
            this.Harmony = string.Empty;
            this.Shape = string.Empty;
            this.Length = 1;
        }

        public int Number { get; set; }
        public string Harmony { get; set; }
        public string Shape { get; set; }
        public byte Length { get; set; }
    }
}
