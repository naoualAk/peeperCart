using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class Features : List<Feature>
    {
        public void Add(string featuresName, object value)
        {
            if (value == null ||
                 value.GetType() == typeof(int) && Convert.ToInt32(value) == 0 ||
                 value.GetType() == typeof(double) && Convert.ToDouble(value) == 0 ||
                 value.GetType() == typeof(float) && Convert.ToDouble(value) == 0)
                return;

            this.Add(new Feature() { FeaturesName = featuresName, Value = value });
        }

    
    }
}
