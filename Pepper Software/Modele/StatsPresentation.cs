using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisartLib;

namespace pepperSoft.Modele
{
    public class StatsPresentation : List<Stats>
    {
        public StatsPresentation(List<Item> items)
        {
            Add(new Modele.Stats()
            {
                FeaturesName = "Longueur1",
                Avg = (float)items.Average(i => i.dimension1.Height),
                Std = (float)items.StdDev(i => i.dimension1.Height),
                Max = (float)items.Max(i => i.dimension1.Height),
                Min = (float)items.Min(i => i.dimension1.Height),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Largeur1",
                Avg = (float)items.Average(i => i.dimension1.Width),
                Std = (float)items.StdDev(i => i.dimension1.Width),
                Max = (float)items.Max(i => i.dimension1.Width),
                Min = (float)items.Min(i => i.dimension1.Width),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Aire1",
                Avg = (float)items.Average(i => i.dimension1.Area),
                Std = (float)items.StdDev(i => i.dimension1.Area),
                Max = (float)items.Max(i => i.dimension1.Area),
                Min = (float)items.Min(i => i.dimension1.Area),
            });



            Add(new Modele.Stats()
            {
                FeaturesName = "Longueur2",
                Avg = (float)items.Average(i => i.dimension2.Height),
                Std = (float)items.StdDev(i => i.dimension2.Height),
                Max = (float)items.Max(i => i.dimension2.Height),
                Min = (float)items.Min(i => i.dimension2.Height),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Largeur2",
                Avg = (float)items.Average(i => i.dimension2.Width),
                Std = (float)items.StdDev(i => i.dimension2.Width),
                Max = (float)items.Max(i => i.dimension2.Width),
                Min = (float)items.Min(i => i.dimension2.Width),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Aire2",
                Avg = (float)items.Average(i => i.dimension2.Area),
                Std = (float)items.StdDev(i => i.dimension2.Area),
                Max = (float)items.Max(i => i.dimension2.Area),
                Min = (float)items.Min(i => i.dimension2.Area),
            });


            Add(new Modele.Stats()
            {
                FeaturesName = "Longueur3",
                Avg = (float)items.Average(i => i.dimension3.Height),
                Std = (float)items.StdDev(i => i.dimension3.Height),
                Max = (float)items.Max(i => i.dimension3.Height),
                Min = (float)items.Min(i => i.dimension3.Height),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Largeur3",
                Avg = (float)items.Average(i => i.dimension3.Width),
                Std = (float)items.StdDev(i => i.dimension3.Width),
                Max = (float)items.Max(i => i.dimension3.Width),
                Min = (float)items.Min(i => i.dimension3.Width),
            });

            Add(new Modele.Stats()
            {
                FeaturesName = "Aire3",
                Avg = (float)items.Average(i => i.dimension3.Area),
                Std = (float)items.StdDev(i => i.dimension3.Area),
                Max = (float)items.Max(i => i.dimension3.Area),
                Min = (float)items.Min(i => i.dimension3.Area),
            });

            foreach (Stats stat in this)
            {
                stat.CV = stat.Avg / stat.Std;
            }
        }
    }
}
