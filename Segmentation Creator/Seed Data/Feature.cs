using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationCreator.Seed_Data
{
    public class Feature
    {
        string Name;
        public object value;

        Feature(string name)
        {
            this.Name = name;
        }
    }
}