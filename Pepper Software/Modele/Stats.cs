using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class Stats
    {
        public string FeaturesName { get; set; }
        public float Avg { get; set; }
        public float Std { get; set; }

        public float CV { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
    }
}
