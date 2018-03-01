using SegmentationCreator.Process;
using SegmentationCreator.Seed_Data;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SegmentationCreator.Process
{
    static public class PRoot
    {
        static public ProcessData Run(string path, bool AFF = false)
        {
            
            ProcessData p = new ProcessData();
            //string path = @"D:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne 1\21-03\A1.bmp";
            //string path = @"P:\Ali\Projet Biostimulant\BDD\Feuillage\08-02-2017\A_1_LG30.600_MA_10.bmp";
            FileInfo fi = new FileInfo(path);
            p.Name = fi.Name;

            Image<Lab, Byte> root = new Image<Lab, byte>(path);
            //for javiera batch campagne0
            //root.ROI = new Rectangle(0, 0, root.Width, 2200);

            //for campagne 1
             //root.ROI = new Rectangle(0, 0, root.Width, 1980);

            //for campagne2
            root.ROI = new Rectangle(136, 188, 3340 - 136, 2256 - 188);

            //for campagne3
            //  root.ROI = new Rectangle(180, 320, 3208 - 180, 2300 - 320);


            p.Im = root.Convert<Bgr, byte>().Copy();

            Image<Hsv, Byte> rootHSV = root.Convert<Hsv, byte>().Copy();

            // HistogramViewer.Show(root);
           // if (AFF) ImageViewer.Show(root.Convert<Hsv, byte>(),"Racine Origine");
            if (AFF) CvInvoke.cvShowImage("Racine Origine", root.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());


            //for campagne1
           // int dirtThreshold = 20, dirtThreshold2 = 30, dirtThreshold3 = 40;   

            //for campagne2 and campagne3
            int dirtThreshold = 70, dirtThreshold2 = 80, dirtThreshold3 = 90;

            Image<Gray, Byte> rootGray = new Image<Gray, byte>(root.ROI.Size);
            Image<Gray, Byte> rootGray2 = new Image<Gray, byte>(root.ROI.Size);
            Image<Gray, Byte> rootGray3 = new Image<Gray, byte>(root.ROI.Size);

            //root = HaploidUtils.Kmeans(root, 5);
            int pxRacine = 0, pxRacine2 = 0, pxRacine3 = 0;

            // delete dirt

            Parallel.For(0, root.Rows, r =>
            {
                Parallel.For(0, root.Cols, c =>
                {
                    bool isGreen = false;
                    //for campagn 3
                    //if (rootHSV[r, c].Hue >= 50 &&  rootHSV[r, c].Hue <= 90 && rootHSV[r, c].Satuation >= 90)
                      //    isGreen = true;
                    if (root[r, c].X < dirtThreshold || isGreen)
                    {
                        root[r, c] = new Lab(0, 128, 128);
                    }
                    else
                    {
                        rootGray[r, c] = new Gray(255);
                        if (root[r, c].X > dirtThreshold2)
                        {
                            rootGray2[r, c] = new Gray(255);
                            if (root[r, c].X > dirtThreshold3)
                            {
                                rootGray3[r, c] = new Gray(255);
                            }
                        }
                    }
                });
            });



              ImageViewer.Show(rootGray.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("rootGray"));

            ImageViewer.Show(rootGray2.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("rootGray"));
            ImageViewer.Show(rootGray3.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("rootGray"));

            //delete paper
            /*  List<View> views = ProcessUtils.FindContour(rootGray);
              rootGray = deleteViewsTouchBorder(views, rootGray);

              views = ProcessUtils.FindContour(rootGray2);
              rootGray2 = deleteViewsTouchBorder(views, rootGray2);


              views = ProcessUtils.FindContour(rootGray3);
              rootGray3 = deleteViewsTouchBorder(views, rootGray3);

              */


            #region Haralick
            //    List<List<double>> haraList = 

            p.haraRootList = rootGray3.Haralick();

            #endregion

            #region Distance Map for dirt
            Image<Gray, Byte> dirtGray = rootGray3.Not();
            Image<Gray, float> distFDirt = new Image<Gray, float>(rootGray3.Size);
            IntPtr lb = new IntPtr();
            CvInvoke.cvDistTransform(dirtGray, distFDirt, Emgu.CV.CvEnum.DIST_TYPE.CV_DIST_L2, 5, null, lb);
            Image<Gray, byte> distDirtAFF = distFDirt.Convert<Gray, Byte>();
            p.haraDistMapDirtList = distDirtAFF.Haralick();


            List<float> distMapDirtData = distFDirt.Data.OfType<float>().ToList();

            distMapDirtData.RemoveAll(i => i == 0);

            p.DistMapDirtMin = distMapDirtData.Min();
            p.DistMapDirtMax = distMapDirtData.Max();
            p.DistMapDirtMoy = distMapDirtData.Average();
            p.DistMapDirtSTD = distMapDirtData.StdDev();


           if(AFF) ImageViewer.Show(distDirtAFF, "distDirtAFF");

        /*    Point[] pMax, pMin;
            double[] min, max;

            for (int i = 0; i < 10; i++)
            {
                distFDirt.MinMax(out min, out max, out pMin, out pMax);
                distFDirt.Draw(new CircleF(new PointF(pMax[0].X, pMax[0].Y), 1), new Gray(0), (int)max[0]);
                distDirtAFF.Draw(new CircleF(new PointF(pMax[0].X, pMax[0].Y), 1), new Gray(120), (int)max[0]);
                Debug.WriteLine(pMax[0].ToString() + ":" + max[0]);
                //distDirtAFF.Draw(new CircleF(new PointF(pMax[i].X, pMax[i].Y), 1), new Gray(120), 60);
            }

            ImageViewer.Show(distDirtAFF, "distDirtAFF min max");
            */

            #endregion

            #region Distance Map
            Image<Gray, float> distT = new Image<Gray, float>(rootGray3.Size);
            
            CvInvoke.cvDistTransform(rootGray3, distT, Emgu.CV.CvEnum.DIST_TYPE.CV_DIST_L2, 5, null, lb);
            Image<Gray, byte> distAFF = distT.Convert<Gray, Byte>();

            p.haraDistMapRootList = distAFF.Haralick();


            //distance map data
            List<float> distMapData = distT.Data.OfType<float>().ToList();

            distMapData.RemoveAll(i => i == 0);

            p.DistMapMin = distMapData.Min();
            p.DistMapMax = distMapData.Max();
            p.DistMapMoy = distMapData.Average();
            p.DistMapSTD = distMapData.StdDev();

            List<float> hist = HaploidUtils.HistToList(HaploidUtils.imhist(distAFF, distAFF));

            /*   for (int i = 0; i < hist.Count; i++)
               {
                   Debug.Write(hist[i] + ";");
                   if (i > 0 && hist[i] == 0)
                       break;
               }
               Debug.WriteLine("");*/
            if (AFF) CvInvoke.cvShowImage("distAFF", distAFF.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());
            if (AFF) HistogramViewer.Show(distAFF);
            //if (AFF) ImageViewer.Show(distAFF.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("distAFF"));
            #endregion

            //  ImageViewer.Show(rootGrayClear.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("rootGrayClear"));
            //  Image<Gray, Byte> rootGrayClear = deleteViews(views, rootGray);

            Parallel.For(0, root.Rows, r =>
            {
                Parallel.For(0, root.Cols, c =>
                {
                    if (rootGray[r, c].Intensity > 254)
                    {
                        Interlocked.Increment(ref pxRacine);
                    }
                    if (rootGray2[r, c].Intensity > 254)
                    {
                        Interlocked.Increment(ref pxRacine2);
                    }
                    if (rootGray3[r, c].Intensity > 254)
                    {
                        Interlocked.Increment(ref pxRacine3);
                    }
                });
            });


            float densityRoot = (float)pxRacine * 100 / (root.Rows * root.Cols);
            float densityRoot2 = (float)pxRacine2 * 100 / (root.Rows * root.Cols);
            float densityRoot3 = (float)pxRacine3 * 100 / (root.Rows * root.Cols);

            p.NbPxRoot = pxRacine;
            p.NbPxRoot2 = pxRacine2;
            p.NbPxRoot3 = pxRacine3;
            p.DensityRoot = densityRoot;
            p.DensityRoot2 = densityRoot2;
            p.DensityRoot3 = densityRoot3;

            Debug.WriteLine("Density Root:" + densityRoot);

            if (AFF) CvInvoke.cvShowImage("rootGray", rootGray.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("rootGray2", rootGray2.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("rootGray3", rootGray3.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            p.ImAlgo = rootGray3.Convert<Bgr, byte>().Copy();

            #region ZhangSuen Skeleton !

            //    Image<Gray, byte> SuenSkelet = ZhangSuen.Skeleton(rootGray3);
            // if (AFF) ImageViewer.Show(imG, String.Format("imG"));
            // if (AFF) ImageViewer.Show(SuenSkelet, "SuenSkelet");



            #endregion



            #region SkeThread Part
            Thread SkeTread1, SkeTread2;

            SkeTread1 = new Thread(() => { rootGray = ZhangSuen.Skeleton(rootGray); });
            SkeTread1.Start();

            SkeTread2 = new Thread(() => { rootGray2 = ZhangSuen.Skeleton(rootGray2); });
            SkeTread2.Start();

            rootGray3 = ZhangSuen.Skeleton(rootGray3);

            SkeTread1.Join();
            SkeTread2.Join();
            #endregion

            float mean, std;
            //Image<Gray, float> im = rootGray3.DispersionMap();
            //ImageViewer.Show(im, "dispersion map");

            Image<Gray, byte> imDispersion = rootGray3.DispersionIndice(out mean, out std);
            //p.ImAlgo = imDispersion.Convert<Bgr,Byte>();

            Image<Gray, float> imEp = distT.Copy();
            imEp = Mask(imEp, rootGray3);
            if (AFF) ImageViewer.Show(imEp, "imEp SuenSkelet");

            List<float> SuenDistMap = imEp.Data.OfType<float>().ToList();

            SuenDistMap.RemoveAll(i => i == 0);

            p.SuenDistMapMin = SuenDistMap.Min();
            p.SuenDistMapMax = SuenDistMap.Max();
            p.SuenDistMapMoy = SuenDistMap.Average();
            p.SuenDistMapSTD = SuenDistMap.StdDev();

            //p.ImAlgo = rootGray3.Convert<Bgr, byte>().Copy();

            int pxSkelet = 0, pxSkelet2 = 0, pxSkelet3 = 0, pxSuenSke = 0;


            Parallel.For(0, root.Rows, r =>
            {
                Parallel.For(0, root.Cols, c =>
                {
                    if (rootGray[r, c].Intensity > 200)
                        Interlocked.Increment(ref pxSkelet);
                    if (rootGray2[r, c].Intensity > 200)
                        Interlocked.Increment(ref pxSkelet2);
                    if (rootGray3[r, c].Intensity > 200)
                        Interlocked.Increment(ref pxSkelet3);
                    // if (SuenSkelet[r, c].Intensity > 200)
                    //    Interlocked.Increment(ref pxSuenSke);
                });
            });


            p.PxSkelet = pxSkelet;
            p.PxSkelet2 = pxSkelet2;
            p.PxSkelet3 = pxSkelet3;
            // p.SuenSke = pxSuenSke;

            if (AFF) CvInvoke.cvShowImage("Skeleton rootGray", rootGray.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            distAFF = Mask(distAFF, rootGray2);

            List<float> hist2 = HaploidUtils.HistToList(HaploidUtils.imhist(distAFF, distAFF));

            /*for (int i = 0; i < hist2.Count; i++)
            {
                Debug.Write(hist2[i] + ";");
            }*/
            Debug.WriteLine("end process");

            if (AFF) ImageViewer.Show(distAFF.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("distAFF SKE"));


            //rootGray.Convert<Bgr, byte>().Save(@"D:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne 1\test\A_1_skeleton.bmp");
            if (AFF) ImageViewer.Show(root.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("BackGround"));
            p.ProcessIsEnded = true;
            return p;
        }

        static private Image<Gray, Byte> Mask(Image<Gray, Byte> im, Image<Gray, Byte> mask)
        {
            Image<Gray, float> imF = im.Convert<Gray, float>();
            Image<Gray, float> maskF = mask.Convert<Gray, float>();
            maskF._Mul(1 / 255.0);

            imF = imF.Mul(maskF);

            return imF.Convert<Gray, byte>();
        }

        static private Image<Gray, float> Mask(Image<Gray, float> im, Image<Gray, Byte> mask)
        {
            Image<Gray, float> maskF = mask.Convert<Gray, float>();
            maskF._Mul(1 / 255.0);

            return im.Mul(maskF).Copy();
        }


        // public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values)


        static private Image<Gray, Byte> deleteViewsTouchBorder(List<View> views, Image<Gray, Byte> imG)
        {

            Image<Gray, Byte> im = imG.Copy();
            for (int i = 0; i < views.Count; i++)
            {
                if (views[i].Roi.Bottom > imG.ROI.Bottom - 5)
                {
                    im.FillConvexPoly(views[i].Contour.ToArray(), new Gray(0));
                    //  Debug.WriteLine("views:"  +views[i].Roi.ToString());
                    //Debug.WriteLine("roi:" + imG.ROI.ToString());
                }

            }
            return im;
        }

    }
}
