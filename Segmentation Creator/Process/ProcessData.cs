using SegmentationCreator.Seed_Data;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationCreator
{
    public class ProcessData
    {
        private bool processIsEnded = false;
        private Image<Bgr, Byte> im;
        private Image<Bgr, Byte> imAlgo;

        List<Feature> features;

        private double densityRoot;
        private double nbPxRoot;
        private string name;
        private double densityRoot2;

        public bool ProcessIsEnded
        {
            get
            {
                return processIsEnded;
            }

            set
            {
                processIsEnded = value;
            }
        }

        public Image<Bgr, byte> Im
        {
            get
            {
                return im;
            }

            set
            {
                im = value;
            }
        }

        public Image<Bgr, byte> ImAlgo
        {
            get
            {
                return imAlgo;
            }

            set
            {
                imAlgo = value;
            }
        }

        public double DensityRoot
        {
            get
            {
                return densityRoot;
            }

            set
            {
                densityRoot = value;
            }
        }

        public double NbPxRoot
        {
            get
            {
                return nbPxRoot;
            }

            set
            {
                nbPxRoot = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double DensityRoot2
        {
            get
            {
                return densityRoot2;
            }

            set
            {
                densityRoot2 = value;
            }
        }

        public int PxSkelet { get; set; }
        public double NbPxRoot2 { get; internal set; }
        public double NbPxRoot3 { get; internal set; }
        public double DensityRoot3 { get; internal set; }
        public object PxSkelet2 { get; internal set; }
        public object PxSkelet3 { get; internal set; }
        public double PxSkLeaves { get; internal set; }
        public double PxLeaves { get; internal set; }
        public double LeavesAreaMin { get; internal set; }
        public double LeavesAreaMax { get; internal set; }
        public double LeavesAreaMoy { get; internal set; }
        public double LeavesAreaStd { get; internal set; }
        public double SkeMin { get; internal set; }
        public double SkeMax { get; internal set; }
        public double SkeMoy { get; internal set; }
        public double SkeSTD { get; internal set; }
        public double LeavesLengthMin { get; internal set; }
        public double LeavesLengthMax { get; internal set; }
        public double LeavesLengthMoy { get; internal set; }
        public double LeavesLengthStd { get; internal set; }
        public double LeavesLengthCalipMin { get; internal set; }
        public double LeavesLengthCalipMax { get; internal set; }
        public double LeavesLengthCalipMoy { get; internal set; }
        public double LeavesLengthCalipStd { get; internal set; }
        public double LeavesWidthCalipMin { get; internal set; }
        public double LeavesWidthCalipMax { get; internal set; }
        public double LeavesWidthCalipMoy { get; internal set; }
        public double LeavesWidthCalipStd { get; internal set; }
        public int NbRod { get; internal set; }

        public double Couverture { get;  set; }
        public int nbPxSeed { get;  set; }
        public int nbPxTreatment { get;  set; }
        public int NbSpot { get;  set; }
        public double SpotMoy { get;  set; }
        public int SpotMin { get;  set; }
        public int SpotMax { get;  set; }
        public double SpotET { get;  set; }
        public Rgb ColorTreatment { get; set; }
        public string ColorTreatmentStr { get; set; }
        public int L { get; set; }
        public int a { get; set; }
        public int b { get; set; }
        public double LET { get; set; }
        public double aET { get; set; }
        public double bET { get; set; }
        public string ColorStr { get; set; }
        public string ColorNoTreatmentStr { get; set; }
        public double OpacityMoy { get; set; }
        public double OpacityET { get; set; }
        public int SuenSke { get; internal set; }
        public float SuenDistMapMin { get; internal set; }
        public float SuenDistMapMax { get; internal set; }
        public float SuenDistMapMoy { get; internal set; }
        public float SuenDistMapSTD { get; internal set; }
        public float DistMapMin { get; internal set; }
        public float DistMapMax { get; internal set; }
        public float DistMapMoy { get; internal set; }
        public float DistMapSTD { get; internal set; }
        public double fidelityMoy { get; set; }
        public double fidelityET { get; set; }
        public int NbView { get; set; }
        public float DistMapDirtMin { get; internal set; }
        public float DistMapDirtMax { get; internal set; }
        public float DistMapDirtMoy { get; internal set; }
        public float DistMapDirtSTD { get; internal set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<List<double>> haraDistMapDirtList { get; internal set; }
        public List<List<double>> haraRootList { get; internal set; }
        public List<List<double>> haraDistMapRootList { get; internal set; }
    }

    public class MetaProcessData
    {

        public MetaProcessData()
        {
            datas = new List<ProcessData>();
        }
        public List<ProcessData> datas;
        public bool processIsEnded = false;
    }
}
