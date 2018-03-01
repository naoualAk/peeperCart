using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SegmentationCreator
{

    public class Segmentation
    {
        [XmlArray("Segmentations")]

        [XmlIgnore]
        public Image<Gray, Byte> imThreshold;

        [XmlIgnore]
        public Image<Rgb, Byte> imAlgo;

        public enum SegmentationType {Soustraction, Seuillage, Fermeture, Ouverture};

        [XmlAttribute("segmentationType")]
        public SegmentationType segmentationType { get; set; }

        [XmlAttribute("backgroundPath")]
        public string backgroundPath { get; set; }

        [XmlAttribute("backgroundThreshold")]
        public int backgroundThreshold { get; set; }

        [XmlAttribute("tHueLow")]
        public int tHueLow { get; set; }

        [XmlAttribute("inverseHue")]
        public bool inverseHue { get; set; }

        [XmlAttribute("tHueHigh")]
        public int tHueHigh { get; set; }

        [XmlAttribute("tSaturation")]
        public int tSaturation { get; set; }

        [XmlAttribute("tHighLightness")]
        public int tHighLightness { get; set; }

        [XmlAttribute("tLowLightness")]
        public int tLowLightness { get; set; }

        [XmlAttribute("closingSize")]
        public int closingSize { get; set; }

        [XmlAttribute("openningSize")]
        public int openningSize { get; set; }

        [XmlAttribute("closingIteration")]
        public int closingIteration { get; set; }

        [XmlAttribute("openningIteration")]
        public int openningIteration { get; set; }

    }
}
