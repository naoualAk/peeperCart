using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisartLib.Data;

namespace pepperSoft.Modele
{
    public class Item
    {

        public Item()
        {
            images = new ImData();
            shapes = new List<Shape>();
            statsColor = new StatsColor();
            haralick = new Haralick();
            dimension1 = new Dimension();
            dimension2 = new Dimension();
            dimension3 = new Dimension();
        }

        public ImData images { get; set; }

        public void SetImageSource(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            System.Drawing.Bitmap bitmapConversion = bitmap;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmapConversion.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }


            ImageSource = bitmapImage;
        }

        public List<Shape> shapes { get; set; }


        public StatsColor statsColor { get; set; }

        public int ID { get; set; }

        public string Name { get; set; }

        public Dimension dimension1 { get; set; }
        public Dimension dimension2 { get; set; }
        public Dimension dimension3 { get; set; }

        public float avg { get; set; }
        public float max { get; set; }
        public float min { get; set; }
        public float std { get; set; }

        public System.Windows.Media.ImageSource ImageSource { get; set; }

        public Bitmap bitmap { get; set; }

        public float avgSob { get; set; }
        public float maxSob { get; set; }
        public float minSob { get; set; }
        public float stdSob { get; set; }
        public float height { get; set; }
        public float width { get; set; }
        public float area { get; set; }

        public float roundness { get; set; }
        public float ellHeight { get; set; }
        public float ellWidth { get; set; }
        public float rugosite { get; set; }


        public Haralick haralick { get; set; }
    }
}
