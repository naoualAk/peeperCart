using SegmentationCreator.Seed_Data;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Emgu.CV.UI;
using System.Diagnostics;
using Accord.Imaging;

namespace SegmentationCreator.Process
{
    static public class ProcessUtils
    {


        public static double ShannonEntropy(this Image<Gray, Byte> im)
        {
            double res = 0;

            double entropie = 0;

            double pk = 0;
            Image<Gray, Byte>[] ims = new Image<Gray, Byte>[1];
            ims[0] = im;
            UInt32 nb_pixels = Convert.ToUInt32(ims[0].Width * ims[0].Height);
            DenseHistogram hist = new DenseHistogram(256, new RangeF(0, 255));
            hist.Calculate(ims, false, null);
            for (int i = 0; i < 256; i++) // Pour chaque niveau de gris
            {
                pk = Convert.ToDouble(hist.MatND.ManagedArray.GetValue(i)) / nb_pixels;
                if (pk != 0) entropie += (pk * Math.Log(pk, 2));
                // entropie = - sigma sur K de (pk * log2(pk))
                // K = niveaux de gris, pk = proba du niveau de gris k
            }
            entropie = -entropie;

            return entropie;
        }



        /// <summary>
        /// </summary>
        /// <param name="im">gray image skeleton with 0 or 255</param>
        /// <returns></returns>
        public static Image<Gray, float> DispersionMap(this Image<Gray, Byte> im)
        {
            Image<Gray, float> imRes = new Image<Gray, float>(im.Size);

            for (int r = 0; r < im.Size.Height; r++)
            {
                for (int c = 0; c < im.Size.Width; c++)
                {
                    if (im[r, c].Intensity == 255)
                    {
                        for (int r1 = 0; r1 < im.Size.Height; r1++)
                        {
                            for (int c1 = 0; c1 < im.Size.Width; c1++)
                            {
                                if (im[r1, c1].Intensity == 255)
                                {
                                    imRes.Data[r, c, 0] = distanceEuclidienne(r, c, r1, c1);
                                }
                            }
                        }
                    }
                }
            }
            return imRes;
        }

        private static float distanceEuclidienne(int x, int y, int x1, int y1)
        {
            return (float)Math.Sqrt(Math.Pow(x - y, 2) + Math.Pow(x1 - y1, 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imSrc">skeleton image</param>
        /// <param name="mean"></param>
        /// <param name="std"></param>
        public static Image<Gray, Byte> DispersionIndice(this Image<Gray, Byte> imSrc, out float mean, out float std)
        {
            float zoom = 0.05F;
            //resize
            Image<Gray, Byte> im = imSrc.Resize(zoom, Emgu.CV.CvEnum.INTER.CV_INTER_LANCZOS4);
            //ImageViewer.Show(im, "resize laczos");
            im = im.Resize(1 / zoom, Emgu.CV.CvEnum.INTER.CV_INTER_NN);
            //ImageViewer.Show(im, "resize laczos");

            //calc the mean value, std dev
            List<float> imList = im.Data.OfType<float>().ToList();
            try
            {
                mean = imList.Average();
                std = imList.StdDev();
            }
            catch
            {
                mean = 0;
                std = 0;
            }

            return im;
        }


        //mode 1 = HS
        //mode 2 = SV
        //mode 3 = HV
        public static Image<Gray, Byte> DrawHistoHSV(Image<Hsv, Byte> im, int mode = 1)
        {
            if (mode > 3 && mode < 1) return null;

            Image<Gray, Byte> res = new Image<Gray, byte>(256, 256);
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int cl = 0; cl < im.Cols; cl++)
                {
                    if (mode == 1) res[(int)im[rw, cl].Hue, (int)im[rw, cl].Satuation] = new Gray(255); // h,s
                    else if (mode == 2) res[(int)im[rw, cl].Satuation, (int)im[rw, cl].Value] = new Gray(255); // s,v
                    else res[(int)im[rw, cl].Hue, (int)im[rw, cl].Value] = new Gray(255); // h,v
                }
            }
            return res;
        }

        /// <summary>
        /// Mask une image avec un mask 0 ou 255
        /// </summary>
        /// <param name="im"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        static public Image<Gray, float> Mask(Image<Gray, float> im, Image<Gray, Byte> mask)
        {
            Image<Gray, float> maskF = mask.Convert<Gray, float>();
            maskF._Mul(1 / 255.0);

            return im.Mul(maskF).Copy();
        }




        //mode 1 = HS
        //mode 2 = SV
        //mode 3 = HV
        public static Image<Hsv, Byte> DrawHistoHSVColor(Image<Hsv, Byte> im, int mode = 1)
        {
            if (mode > 3 && mode < 1) return null;

            Image<Hsv, Byte> res = new Image<Hsv, byte>(256, 256);
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int cl = 0; cl < im.Cols; cl++)
                {
                    if (mode == 1) res[(int)im[rw, cl].Hue, (int)im[rw, cl].Satuation] = new Hsv(im[rw, cl].Hue, im[rw, cl].Satuation, im[rw, cl].Value);
                    else if (mode == 2) res[(int)im[rw, cl].Satuation, (int)im[rw, cl].Value] = new Hsv(im[rw, cl].Hue, im[rw, cl].Satuation, im[rw, cl].Value);
                    else res[(int)im[rw, cl].Hue, (int)im[rw, cl].Value] = new Hsv(im[rw, cl].Hue, im[rw, cl].Satuation, im[rw, cl].Value);
                }
            }
            return res;
        }


        static public void Mask(this Image<Hsv, Byte> im, Image<Gray, Byte> mask)
        {
            Parallel.For(0, im.Rows, r =>
            {
                Parallel.For(0, im.Cols, c =>
                {
                    if (mask[r, c].Intensity == 0)
                        im[r, c] = new Hsv(0, 0, 0);
                });
            });
        }

        static private void Mask(this Image<Lab, float> im, Image<Gray, Byte> mask)
        {
            Parallel.For(0, im.Rows, r =>
            {
                Parallel.For(0, im.Cols, c =>
                {
                    if (mask[r, c].Intensity == 0)
                        im[r, c] = new Lab(0, 0, 0);
                });
            });
        }

        static private List<float> DistManhattan(this Image<Lab, float> im, Lab colorRef)
        {
            List<float> res = new List<float>();
            Object lockMe = new Object();

            Parallel.For(0, im.Rows, r =>
            {
                Parallel.For(0, im.Cols, c =>
                {
                    if (im[r, c].X != 0)
                    {
                        float value = (float)Math.Abs(im[r, c].X - colorRef.X) + (float)Math.Abs(im[r, c].Y - colorRef.Y) + (float)Math.Abs(im[r, c].Z - colorRef.Z);
                        lock (lockMe)
                            res.Add(value);
                    }
                });
            });

            return res;
        }

        static private List<float> DistDeltaE76(this Image<Lab, float> im, Lab colorRef)
        {
            List<float> res = new List<float>();
            Object lockMe = new Object();

            Parallel.For(0, im.Rows, r =>
            {
                Parallel.For(0, im.Cols, c =>
                {
                    if (im[r, c].X != 0)
                    {
                        float value = (float)Math.Sqrt(Math.Pow((double)(im[r, c].X - colorRef.X), 2) + Math.Pow((double)Math.Abs(im[r, c].Y - colorRef.Y), 2) + Math.Pow((double)Math.Abs(im[r, c].Z - colorRef.Z), 2));
                        lock (lockMe)
                            res.Add(value);
                    }
                });
            });

            return res;
        }

        static private List<float> DistDeltaE2000(this Image<Lab, float> im, Lab colorRef)
        {
            List<float> res = new List<float>();
            Object lockMe = new Object();

            Parallel.For(0, im.Rows, r =>
            {
                Parallel.For(0, im.Cols, c =>
                {
                    if (im[r, c].X != 0)
                    {
                        float value = (float)CIEDE2000(im[r, c], colorRef);
                        lock (lockMe)
                            res.Add(value);
                    }
                });
            });

            return res;
        }


        static public double deg2Rad(double deg)
        {
            return (deg * (Math.PI / 180.0));
        }

        static public double rad2Deg(double rad)
        {
            return ((180.0 / Math.PI) * rad);
        }


        static double CIEDE2000(
              Lab lab1,
              Lab lab2)
        {
            /* 
             * "For these and all other numerical/graphical 􏰀delta E00 values
             * reported in this article, we set the parametric weighting factors
             * to unity(i.e., k_L = k_C = k_H = 1.0)." (Page 27).
             */
            double k_L = 1.0, k_C = 1.0, k_H = 1.0;
            double deg360InRad = deg2Rad(360.0);
            double deg180InRad = deg2Rad(180.0);
            double pow25To7 = 6103515625.0; /* pow(25, 7) */

            /*
             * Step 1 
             */
            /* Equation 2 */
            double C1 = Math.Sqrt((lab1.Y * lab1.Y) + (lab1.Z * lab1.Z));
            double C2 = Math.Sqrt((lab2.Y * lab2.Y) + (lab2.Z * lab2.Z));
            /* Equation 3 */
            double barC = (C1 + C2) / 2.0;
            /* Equation 4 */
            double G = 0.5 * (1 - Math.Sqrt(Math.Pow(barC, 7) / (Math.Pow(barC, 7) + pow25To7)));
            /* Equation 5 */
            double a1Prime = (1.0 + G) * lab1.Y;
            double a2Prime = (1.0 + G) * lab2.Y;
            /* Equation 6 */
            double CPrime1 = Math.Sqrt((a1Prime * a1Prime) + (lab1.Z * lab1.Z));
            double CPrime2 = Math.Sqrt((a2Prime * a2Prime) + (lab2.Z * lab2.Z));
            /* Equation 7 */
            double hPrime1;
            if (lab1.Z == 0 && a1Prime == 0)
                hPrime1 = 0.0;
            else
            {
                hPrime1 = Math.Atan2(lab1.Z, a1Prime);
                /* 
                 * This must be converted to a hue angle in degrees between 0 
                 * and 360 by addition of 2􏰏 to negative hue angles.
                 */
                if (hPrime1 < 0)

                    hPrime1 += deg360InRad;
            }
            double hPrime2;
            if (lab2.Z == 0 && a2Prime == 0)
                hPrime2 = 0.0;
            else
            {
                hPrime2 = Math.Atan2(lab2.Z, a2Prime);
                /* 
                 * This must be converted to a hue angle in degrees between 0 
                 * and 360 by addition of 2􏰏 to negative hue angles.
                 */
                if (hPrime2 < 0)

                    hPrime2 += deg360InRad;
            }

            /*
             * Step 2
             */
            /* Equation 8 */
            double deltaLPrime = lab2.X - lab1.X;
            /* Equation 9 */
            double deltaCPrime = CPrime2 - CPrime1;
            /* Equation 10 */
            double deltahPrime;
            double CPrimeProduct = CPrime1 * CPrime2;
            if (CPrimeProduct == 0)
                deltahPrime = 0;
            else
            {
                /* Avoid the fabs() call */
                deltahPrime = hPrime2 - hPrime1;
                if (deltahPrime < -deg180InRad)

                    deltahPrime += deg360InRad;
                else if (deltahPrime > deg180InRad)
                    deltahPrime -= deg360InRad;
            }
            /* Equation 11 */
            double deltaHPrime = 2.0 * Math.Sqrt(CPrimeProduct) *
                Math.Sin(deltahPrime / 2.0);

            /*
             * Step 3
             */
            /* Equation 12 */
            double barLPrime = (lab1.X + lab2.X) / 2.0;
            /* Equation 13 */
            double barCPrime = (CPrime1 + CPrime2) / 2.0;
            /* Equation 14 */
            double barhPrime, hPrimeSum = hPrime1 + hPrime2;
            if (CPrime1 * CPrime2 == 0)
            {
                barhPrime = hPrimeSum;
            }
            else
            {
                if (Math.Abs(hPrime1 - hPrime2) <= deg180InRad)
                    barhPrime = hPrimeSum / 2.0;
                else
                {
                    if (hPrimeSum < deg360InRad)

                        barhPrime = (hPrimeSum + deg360InRad) / 2.0;
                    else
                        barhPrime = (hPrimeSum - deg360InRad) / 2.0;
                }
            }
            /* Equation 15 */
            double T = 1.0 - (0.17 * Math.Cos(barhPrime - deg2Rad(30.0))) +
                (0.24 * Math.Cos(2.0 * barhPrime)) +
                (0.32 * Math.Cos((3.0 * barhPrime) + deg2Rad(6.0))) -
                (0.20 * Math.Cos((4.0 * barhPrime) - deg2Rad(63.0)));
            /* Equation 16 */
            double deltaTheta = deg2Rad(30.0) *
                Math.Exp(-Math.Pow((barhPrime - deg2Rad(275.0)) / deg2Rad(25.0), 2.0));
            /* Equation 17 */
            double R_C = 2.0 * Math.Sqrt(Math.Pow(barCPrime, 7.0) /
                (Math.Pow(barCPrime, 7.0) + pow25To7));
            /* Equation 18 */
            double S_L = 1 + ((0.015 * Math.Pow(barLPrime - 50.0, 2.0)) /
                Math.Sqrt(20 + Math.Pow(barLPrime - 50.0, 2.0)));
            /* Equation 19 */
            double S_C = 1 + (0.045 * barCPrime);
            /* Equation 20 */
            double S_H = 1 + (0.015 * barCPrime * T);
            /* Equation 21 */
            double R_T = (-Math.Sin(2.0 * deltaTheta)) * R_C;

            /* Equation 22 */
            double deltaE = Math.Sqrt(
                Math.Pow(deltaLPrime / (k_L * S_L), 2.0) +
                Math.Pow(deltaCPrime / (k_C * S_C), 2.0) +
                Math.Pow(deltaHPrime / (k_H * S_H), 2.0) +
                (R_T * (deltaCPrime / (k_C * S_C)) * (deltaHPrime / (k_H * S_H))));

            return (deltaE);
        }


        public enum DistMode { Manhattan, DeltaE76, DeltaE2000 };

        public static double[] ColorFidelity(Image<Lab, byte> im, Lab colorRef, Image<Gray, byte> mask = null, DistMode d = DistMode.Manhattan)
        {

            Image<Lab, float> imRes = im.Convert<Lab, float>();


            if (mask != null && mask.Size == imRes.Size)
                imRes.Mask(mask);

            //ImageViewer.Show(imRes, "imres");
            List<float> list;
            switch (d)
            {
                case DistMode.Manhattan:
                    list = imRes.DistManhattan(colorRef);
                    break;
                case DistMode.DeltaE76:
                    list = imRes.DistDeltaE76(colorRef);
                    break;
                case DistMode.DeltaE2000:
                    list = imRes.DistDeltaE2000(colorRef);
                    break;
                default:
                    list = imRes.DistManhattan(colorRef);
                    break;
            }


            // List<float> list = imRes.Data.OfType<float>().ToList();

            double[] res = new double[2];
            res[0] = list.Average();
            res[1] = list.StdDev();

            Debug.WriteLine("moy={0};ET={1}", res[0], res[1]);

            return res;
        }

        public static Rgb ConvertToRgb(this Lab color)
        {
            Image<Lab, Byte> im = new Image<Lab, byte>(1, 1, color);
            return im.Convert<Rgb, byte>()[0, 0];
        }


        /// <summary>
        /// return 
        /// </summary>
        /// <param name="im"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int[] CreateThreshold(Image<Gray, Byte> im, Image<Gray, Byte> mask = null)
        {
            List<double> features = HaploidUtils.GMMFeaturesExtraction(im, mask, 1);

            int isRotated = 0;
            Image<Gray, Byte> imRotate = new Image<Gray, byte>(im.Size);
            if (Math.Floor(features[0] - 2 * features[1]) < 0)
            {
                for (int r = 0; r < imRotate.Rows; r++)
                {
                    for (int c = 0; c < imRotate.Cols; c++)
                    {
                        imRotate[r, c] = new Gray((im[r, c].Intensity + 90) % 180);
                    }
                }
                features = HaploidUtils.GMMFeaturesExtraction(imRotate, mask, 1);
                isRotated = 1;
            }
            Debug.WriteLine("moy={0}; ET={1}", features[0], features[1]);
            int[] thresholds = new int[] { (int)(Math.Floor(features[0] - 90 * isRotated - 3 * features[1])), (int)Math.Floor(features[0] - 90 * isRotated + 3 * features[1] + 1) };

            return thresholds;
        }

        public static Image<Gray, Byte> SegmentationBlue(Image<Rgb, Byte> imSrc, Image<Rgb, Byte> imBg, Rectangle[] rois)
        {

            int maxUp = imSrc.Height, maxRight = 0, maxLeft = imSrc.Width, maxBottom = 0;
            if (rois.Length > 0)
            {
                int width = imSrc.Width, height = imSrc.Height;

                Image<Gray, Byte> threshTot = new Image<Gray, Byte>(width, height);

                for (int i = 0; i < rois.Length; i++)
                {
                    string strIdx = i.ToString();
                    if (maxLeft > rois[i].X) maxLeft = rois[i].X;
                    if (maxUp > rois[i].Y) maxUp = rois[i].Y;
                    if (maxRight < rois[i].Right) maxRight = rois[i].Right;
                    if (maxBottom < rois[i].Bottom) maxBottom = rois[i].Bottom;

                    imSrc.ROI = rois[i];
                    imBg.ROI = rois[i];

                    Image<Lab, Byte> srcLab = imSrc.Convert<Lab, Byte>();
                    Image<Lab, Byte> bgLab = imBg.Convert<Lab, Byte>();

                    /// a of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> blurMask = new Image<Gray, Byte>(srcLab.Split()[1].AbsDiff(bgLab.Split()[1]).Data);
                    Image<Gray, Byte> whiteMask = new Image<Gray, Byte>(srcLab.Split()[0].AbsDiff(bgLab.Split()[0]).Data);

                    /// b of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> threshIm = new Image<Gray, Byte>(srcLab.Split()[2].AbsDiff(bgLab.Split()[2]).Data);


                    // ImageViewer.Show( whiteMask.Convert<Bgr, Byte>(),"whiteMask");
                    //carotte 15 mais 20
                    whiteMask._ThresholdToZero(new Gray(15));
                    //  ImageViewer.Show(whiteMask.Convert<Bgr, Byte>(), "whitemask thresh");
                    // ImageViewer.Show(blurMask.Convert<Bgr, Byte>(), "blurMask ");
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm ");

                    //next line for carotte 
                    // suppr two next lines for haricot noir
                    // threshIm = threshIm.AddWeighted(blurMask, 0.4, 2, 0);
                    //threshIm = threshIm.AddWeighted(whiteMask, 1, 0.2, 0);
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm thresh");

                    Image<Gray, Byte> border = new Image<Gray, Byte>(threshIm.Width + 80, threshIm.Height + 80);

                    CvInvoke.cvCopyMakeBorder(threshIm, border, new Point(40, 40), Emgu.CV.CvEnum.BORDER_TYPE.CONSTANT, new MCvScalar(0));
                    threshIm = border;

                    //pour bande bleu mettre threshold à 30 pour tapis noir mettre 10
                    CvInvoke.cvThreshold(threshIm, threshIm, 30, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

                    int size = 2;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 3);

                    size = 2;
                    /// ouverture 
                    elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 2);


                    threshIm.ROI = new Rectangle(40, 40, threshIm.Width - 80, threshIm.Height - 80);
                    threshTot.ROI = new Rectangle(rois[i].X, rois[i].Y, threshIm.Width, threshIm.Height);

                    threshIm.CopyTo(threshTot);
                }

                threshTot.ROI = new Rectangle(maxLeft, maxUp, maxRight - maxLeft, maxBottom - maxUp);
                imSrc.ROI = threshTot.ROI;
                imBg.ROI = threshTot.ROI;

                Image<Gray, Byte> res = new Image<Gray, Byte>(maxRight - maxLeft, maxBottom - maxUp);
                threshTot.CopyTo(res);

                return res;
            }
            return null;
        }

        public static Image<Gray, Byte> SegmentationBlack(Image<Rgb, Byte> imSrc, Image<Rgb, Byte> imBg, Rectangle[] rois)
        {

            int maxUp = imSrc.Height, maxRight = 0, maxLeft = imSrc.Width, maxBottom = 0;
            if (rois.Length > 0)
            {
                int width = imSrc.Width, height = imSrc.Height;

                Image<Gray, Byte> threshTot = new Image<Gray, Byte>(width, height);

                for (int i = 0; i < rois.Length; i++)
                {
                    string strIdx = i.ToString();
                    if (maxLeft > rois[i].X) maxLeft = rois[i].X;
                    if (maxUp > rois[i].Y) maxUp = rois[i].Y;
                    if (maxRight < rois[i].Right) maxRight = rois[i].Right;
                    if (maxBottom < rois[i].Bottom) maxBottom = rois[i].Bottom;

                    imSrc.ROI = rois[i];
                    imBg.ROI = rois[i];

                    Image<Lab, Byte> srcLab = imSrc.Convert<Lab, Byte>();
                    Image<Lab, Byte> bgLab = imBg.Convert<Lab, Byte>();

                    /// a of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> blurMask = new Image<Gray, Byte>(srcLab.Split()[1].AbsDiff(bgLab.Split()[1]).Data);
                    Image<Gray, Byte> whiteMask = new Image<Gray, Byte>(srcLab.Split()[0].AbsDiff(bgLab.Split()[0]).Data);

                    /// b of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> threshIm = new Image<Gray, Byte>(srcLab.Split()[2].AbsDiff(bgLab.Split()[2]).Data);


                    // ImageViewer.Show( whiteMask.Convert<Bgr, Byte>(),"whiteMask");
                    //carotte 15 mais 20
                    whiteMask._ThresholdToZero(new Gray(15));
                    //  ImageViewer.Show(whiteMask.Convert<Bgr, Byte>(), "whitemask thresh");
                    // ImageViewer.Show(blurMask.Convert<Bgr, Byte>(), "blurMask ");
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm ");

                    //next line for carotte 
                    // suppr two next lines for haricot noir
                    // threshIm = threshIm.AddWeighted(blurMask, 0.4, 2, 0);
                    //threshIm = threshIm.AddWeighted(whiteMask, 1, 0.2, 0);
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm thresh");

                    Image<Gray, Byte> border = new Image<Gray, Byte>(threshIm.Width + 80, threshIm.Height + 80);

                    CvInvoke.cvCopyMakeBorder(threshIm, border, new Point(40, 40), Emgu.CV.CvEnum.BORDER_TYPE.CONSTANT, new MCvScalar(0));
                    threshIm = border;

                    //pour bande bleu mettre threshold à 30 pour tapis noir mettre 10
                    CvInvoke.cvThreshold(threshIm, threshIm, 10, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

                    int size = 2;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 3);

                    size = 2;
                    /// ouverture 
                    elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 2);


                    threshIm.ROI = new Rectangle(40, 40, threshIm.Width - 80, threshIm.Height - 80);
                    threshTot.ROI = new Rectangle(rois[i].X, rois[i].Y, threshIm.Width, threshIm.Height);

                    threshIm.CopyTo(threshTot);
                }

                threshTot.ROI = new Rectangle(maxLeft, maxUp, maxRight - maxLeft, maxBottom - maxUp);
                imSrc.ROI = threshTot.ROI;
                imBg.ROI = threshTot.ROI;

                Image<Gray, Byte> res = new Image<Gray, Byte>(maxRight - maxLeft, maxBottom - maxUp);
                threshTot.CopyTo(res);

                return res;
            }
            return null;
        }

        public static Image<Gray, Byte> SegmentationWhite(Image<Rgb, Byte> imSrc, Image<Rgb, Byte> imBg, Rectangle[] rois)
        {

            int maxUp = imSrc.Height, maxRight = 0, maxLeft = imSrc.Width, maxBottom = 0;
            if (rois.Length > 0)
            {
                int width = imSrc.Width, height = imSrc.Height;

                Image<Gray, Byte> threshTot = new Image<Gray, Byte>(width, height);

                for (int i = 0; i < rois.Length; i++)
                {
                    string strIdx = i.ToString();
                    if (maxLeft > rois[i].X) maxLeft = rois[i].X;
                    if (maxUp > rois[i].Y) maxUp = rois[i].Y;
                    if (maxRight < rois[i].Right) maxRight = rois[i].Right;
                    if (maxBottom < rois[i].Bottom) maxBottom = rois[i].Bottom;

                    imSrc.ROI = rois[i];
                    imBg.ROI = rois[i];

                    Image<Lab, Byte> srcLab = imSrc.Convert<Lab, Byte>();
                    Image<Lab, Byte> bgLab = imBg.Convert<Lab, Byte>();

                    /// a of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> blurMask = new Image<Gray, Byte>(srcLab.Split()[1].AbsDiff(bgLab.Split()[1]).Data);
                    Image<Gray, Byte> whiteMask = new Image<Gray, Byte>(srcLab.Split()[0].AbsDiff(bgLab.Split()[0]).Data);

                    /// b of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> threshIm = new Image<Gray, Byte>(srcLab.Split()[2].AbsDiff(bgLab.Split()[2]).Data);


                    // ImageViewer.Show( whiteMask.Convert<Bgr, Byte>(),"whiteMask");
                    //carotte 15 mais 20
                    //ImageViewer.Show(whiteMask.Convert<Bgr, Byte>(), "l");
                    //whiteMask._ThresholdToZero(new Gray(70));
                    //   ImageViewer.Show(whiteMask.Convert<Bgr, Byte>(), "l");
                    //  ImageViewer.Show(blurMask.Convert<Bgr, Byte>(), "a ");
                    //  ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "b");


                    threshIm = threshIm.AddWeighted(blurMask, 0.6, 0.5, 0);
                    threshIm = threshIm.AddWeighted(whiteMask, 1, 0.08, 0);

                    //  ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm");


                    Image<Gray, Byte> border = new Image<Gray, Byte>(threshIm.Width + 80, threshIm.Height + 80);

                    CvInvoke.cvCopyMakeBorder(threshIm, border, new Point(40, 40), Emgu.CV.CvEnum.BORDER_TYPE.CONSTANT, new MCvScalar(0));
                    threshIm = border;

                    //pour bande bleu mettre threshold à 30 pour tapis noir mettre 10
                    CvInvoke.cvThreshold(threshIm, threshIm, 12, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm thresholded");
                    int size = 1;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 3);
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm close");

                    size = 2;
                    /// ouverture 
                    elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 3);

                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm open");
                    threshIm.ROI = new Rectangle(40, 40, threshIm.Width - 80, threshIm.Height - 80);
                    threshTot.ROI = new Rectangle(rois[i].X, rois[i].Y, threshIm.Width, threshIm.Height);

                    threshIm.CopyTo(threshTot);
                }

                threshTot.ROI = new Rectangle(maxLeft, maxUp, maxRight - maxLeft, maxBottom - maxUp);
                imSrc.ROI = threshTot.ROI;
                imBg.ROI = threshTot.ROI;

                Image<Gray, Byte> res = new Image<Gray, Byte>(maxRight - maxLeft, maxBottom - maxUp);
                threshTot.CopyTo(res);

                return res;
            }
            return null;
        }

        public static Image<Gray, Byte> SegmentationHSV(Image<Rgb, Byte> imSrc, Image<Rgb, Byte> imBg, Rectangle[] rois)
        {

            int maxUp = imSrc.Height, maxRight = 0, maxLeft = imSrc.Width, maxBottom = 0;
            if (rois.Length > 0)
            {
                int width = imSrc.Width, height = imSrc.Height;

                Image<Gray, Byte> threshTot = new Image<Gray, Byte>(width, height);

                for (int i = 0; i < rois.Length; i++)
                {
                    string strIdx = i.ToString();
                    if (maxLeft > rois[i].X) maxLeft = rois[i].X;
                    if (maxUp > rois[i].Y) maxUp = rois[i].Y;
                    if (maxRight < rois[i].Right) maxRight = rois[i].Right;
                    if (maxBottom < rois[i].Bottom) maxBottom = rois[i].Bottom;

                    imSrc.ROI = rois[i];
                    imBg.ROI = rois[i];

                    Image<Hsv, Byte> srcLab = imSrc.Convert<Hsv, Byte>();
                    Image<Hsv, Byte> bgLab = imBg.Convert<Hsv, Byte>();

                    /// a of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> blurMask = new Image<Gray, Byte>(srcLab.Split()[1].AbsDiff(bgLab.Split()[1]).Data);
                    Image<Gray, Byte> whiteMask = new Image<Gray, Byte>(srcLab.Split()[0].AbsDiff(bgLab.Split()[0]).Data);

                    /// b of LAB : | src - bg | x shadowMask
                    Image<Gray, Byte> threshIm = new Image<Gray, Byte>(srcLab.Split()[2].AbsDiff(bgLab.Split()[2]).Data);


                    /*       Image<Gray,float> imSobel = threshIm.Sobel(1, 0, 3);
                           imSobel = imSobel.AddWeighted(threshIm.Sobel(0, 1, 3), 0.5, 0.5,0);
                           ImageViewer.Show(imSobel, "imSobel");*/

                    // ImageViewer.Show( whiteMask.Convert<Bgr, Byte>(),"whiteMask");
                    //carotte 15 mais 20
                    // whiteMask._ThresholdToZero(new Gray(15));
                    //  ImageViewer.Show(whiteMask.Convert<Bgr, Byte>(), "whitemask thresh");
                    //  ImageViewer.Show(blurMask.Convert<Bgr, Byte>(), "blurMask ");
                    // ImageViewer.Show(threshIm.Convert<Bgr, Byte>(), "threshIm ");

                    //next line for carotte
                    threshIm = threshIm.AddWeighted(blurMask, 0.4, 0.6, 0);
                    threshIm = threshIm.AddWeighted(whiteMask, 0.8, 0.2, 0);
                    // ImageViewer.Show(threshIm, "threshIm AddWeighted");

                    Image<Gray, Byte> border = new Image<Gray, Byte>(threshIm.Width + 80, threshIm.Height + 80);

                    CvInvoke.cvCopyMakeBorder(threshIm, border, new Point(40, 40), Emgu.CV.CvEnum.BORDER_TYPE.CONSTANT, new MCvScalar(0));
                    threshIm = border;

                    CvInvoke.cvThreshold(threshIm, threshIm, 30, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

                    //  ImageViewer.Show(threshIm, "threshIm thresh");

                    int size = 1;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 2);

                    //  ImageViewer.Show(threshIm, "threshIm close");

                    size = 2;
                    /// ouverture 
                    elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    threshIm = threshIm.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 2);

                    //  ImageViewer.Show(threshIm, "threshIm thresh");

                    threshIm.ROI = new Rectangle(40, 40, threshIm.Width - 80, threshIm.Height - 80);
                    threshTot.ROI = new Rectangle(rois[i].X, rois[i].Y, threshIm.Width, threshIm.Height);

                    threshIm.CopyTo(threshTot);
                }

                threshTot.ROI = new Rectangle(maxLeft, maxUp, maxRight - maxLeft, maxBottom - maxUp);
                imSrc.ROI = threshTot.ROI;
                imBg.ROI = threshTot.ROI;

                Image<Gray, Byte> res = new Image<Gray, Byte>(maxRight - maxLeft, maxBottom - maxUp);
                threshTot.CopyTo(res);

                return res;
            }
            return null;
        }

        public static List<View> FindContour(Image<Gray, byte> im, double thresholdArea = 0, bool drawInside = false)
        {
            List<View> views = new List<View>();
            Contour<Point> contour;

            #region Finding Contours
            MemStorage storage = new MemStorage();//allocate storage for contour approximation
            for (contour = im.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL, storage); contour != null; contour = contour.HNext)
            {
                if (contour.Area > thresholdArea) //only consider contours with area greater than  1mmx1mm = 70 x 70  = 4900
                {
                    View viewTemp = new View();
                    viewTemp.Roi = CvInvoke.cvBoundingRect(contour, false);

                    viewTemp.Contour = contour;
                    viewTemp.ConvexHull = contour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_COUNTER_CLOCKWISE, storage);
                    viewTemp.RectMinArea = CvInvoke.cvMinAreaRect2(contour, storage);// contour.GetMinAreaRect();

                    viewTemp.ApproxContour = contour.ApproxPoly((contour.Perimeter / 100), storage);

                    if (contour.Total > 5)
                    {
                        MCvBox2D boxEll = CvInvoke.cvFitEllipse2(contour);
                        boxEll.angle += 90;

                        viewTemp.Ellipse = new Ellipse(boxEll);
                    }
                    if (drawInside)
                        im.FillConvexPoly(contour.ToArray(), new Gray(255));
                    views.Add(viewTemp);
                }
                else im.FillConvexPoly(contour.ToArray(), new Gray(0));

            }
            #endregion

            views.Sort((x, y) => ((double)y.Contour.Area).CompareTo((double)x.Contour.Area)); // bigger
            return views;
        }




        public static Image<Gray, Byte> DrawLine(Image<Gray, Byte> im, PointF a, Vector2 b)
        {



            PointF o = new PointF(a.X, a.Y);
            PointF f1 = new PointF(a.X + b.X * 1000, a.Y + b.Y * 1000);
            PointF f2 = new PointF(a.X - b.X * 1000, a.Y - b.Y * 1000);
            LineSegment2DF line = new LineSegment2DF(o, f1);
            LineSegment2DF line2 = new LineSegment2DF(o, f2);
            im.Draw(line, new Gray(125), 1);
            // im.Draw(line2, new Gray(125), 1);

            return im;
        }

        public static void DrawLine(this Image<Hsv, Byte> im, PointF a, Vector2 b)
        {



            PointF o = new PointF(a.X, a.Y);
            PointF f1 = new PointF(a.X + b.X * 1000, a.Y + b.Y * 1000);
            PointF f2 = new PointF(a.X - b.X * 1000, a.Y - b.Y * 1000);
            LineSegment2DF line = new LineSegment2DF(o, f1);
            LineSegment2DF line2 = new LineSegment2DF(o, f2);
            im.Draw(line, new Hsv(255, 255, 255), 1);
            // im.Draw(line2, new Gray(125), 1);

        }

        static public Image<Gray, byte> Skeleton(Image<Gray, byte> orgImg)
        {
            Image<Gray, byte> skel = new Image<Gray, byte>(orgImg.Size);
            for (int y = 0; y < skel.Height; y++)
                for (int x = 0; x < skel.Width; x++)
                    skel.Data[y, x, 0] = 0;

            //imageBoxOutputROI.Image = skel;

            Image<Gray, byte> img = skel.Copy();
            for (int y = 0; y < skel.Height; y++)
                for (int x = 0; x < skel.Width; x++)
                    img.Data[y, x, 0] = orgImg.Data[y, x, 0];

            StructuringElementEx element;
            element = new StructuringElementEx(3, 3, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_CROSS);
            Image<Gray, byte> temp;

            bool done = false;
            do
            {
                temp = img.MorphologyEx(element, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 1);
                temp = temp.Not();
                temp = temp.And(img);
                skel = skel.Or(temp);
                img = img.Erode(1);
                double[] min, max;
                Point[] pmin, pmax;
                img.MinMax(out min, out max, out pmin, out pmax);
                done = (max[0] == 0);
            } while (!done);

            /* StructuringElementEx elementOpen = new StructuringElementEx(3, 3, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
             skel = skel.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 1);*/


            return skel;
        }

        public static float StdDev(this IEnumerable<float> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return (float)ret;
        }


        public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values)
        {
            // ref: http://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
            // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/ 
            var mean = 0.0;
            var sum = 0.0;
            var stdDev = 0.0;
            var n = 0;
            foreach (var value in list.Select(values))
            {
                n++;
                var delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdDev = Math.Sqrt(sum / (n - 1));

            return stdDev;
        }

        /*      private static int[] idExtremePoint(Seq<Point> convexHull)
              {

                  if (convexHull.Total < 0) return null;

                  int ymin = convexHull[0].Y, ymax = convexHull[0].Y;
                  int[] idx = new int[2];

                  for (int i = 0; i < convexHull.Total; i++)
                  {
                      if (ymin > convexHull[i].Y)
                      {
                          ymin = convexHull[i].Y;
                          idx[0] = i;
                      }
                      if (ymax < convexHull[i].Y)
                      {
                          ymax = convexHull[i].Y;
                          idx[1] = i;
                      }
                  }

                  return idx;
              }

              public static double angleRad(Vector2 v1, Vector2 v2)
              {
                  Vector2 v = Vector2.Normalize(v1);

                  float a = Vector2.Dot(v, v2);
                  float b = v.Length();
                  float c = v2.Length();
                  float d = Vector2.Dot(v, v2) / (v.Length() * v2.Length());

                  if (d > 1) d = 1;
                  else if (d < -1) d = -1;


                  double angle = Math.Acos(d);


                  return angle > Math.PI ? angle - Math.PI : angle;
              }

              public static void RotatingCaliper(this View view, out double _height, out double _width)
              {
                  Seq<Point> convexHull = view.ConvexHull;
                  if(convexHull == null)
                  {
                      _height = 0;
                      _width = 0;
                      return;
                  }

                  int[] idExtremePoints = idExtremePoint(convexHull);
                  int pA = idExtremePoints[0], pB = idExtremePoints[1];
                  double rotatedAngle = 0;
                  double minLength = double.PositiveInfinity;
                  double maxLength = 0;

                  Point pTemp1 = new Point(), pTemp2 = new Point();

                  Vector2 caliperA = new Vector2(1, 0);
                  Vector2 caliperB = new Vector2(-1, 0);
                  caliperA = Vector2.Normalize(caliperA);
                  caliperB = Vector2.Normalize(caliperB);

                  while (rotatedAngle < Math.PI)
                  {
                      Vector2 egdeA = new Vector2(convexHull[(pA + 1) % convexHull.Total].X - convexHull[pA].X, convexHull[(pA + 1) % convexHull.Total].Y - convexHull[pA].Y);
                      Vector2 egdeB = new Vector2(convexHull[(pB + 1) % convexHull.Total].X - convexHull[pB].X, convexHull[(pB + 1) % convexHull.Total].Y - convexHull[pB].Y);

                      double angleA = angleRad(egdeA, caliperA);
                      double angleB = angleRad(egdeB, caliperB);
                      double width = 0;

                      caliperA = _2D.rotate(caliperA, Math.Min(angleA, angleB));
                      caliperB = _2D.rotate(caliperB, Math.Min(angleA, angleB));
                      caliperA = Vector2.Normalize(caliperA);
                      caliperB = Vector2.Normalize(caliperB);

                      if (angleA <= angleB)
                      {
                          pA = (pA + 1) % convexHull.Total;
                          if (pB != pA)
                          {
                              width = _2D.DistancePointToVector(convexHull[pB], convexHull[pA], caliperA, out pTemp2);
                              pTemp1 = convexHull[pB];
                          }
                      }
                      else
                      {
                          pB = (pB + 1) % convexHull.Total;
                          if (pB != pA)
                          {
                              width = _2D.DistancePointToVector(convexHull[pA], convexHull[pB], caliperB, out pTemp1);
                              pTemp2 = convexHull[pA];
                          }
                      }

                      rotatedAngle = rotatedAngle + Math.Min(angleA, angleB);

                      if (width < minLength && pB != pA)
                      {
                          minLength = width;
                      }
                      if (width > maxLength && pB != pA)
                      {
                          maxLength = width;
                      }
                  }

                  _height = maxLength;
                  _width = minLength;
              }*/

        static public List<List<double>> Haralick(this Image<Gray, Byte> im)
        {

            Bitmap bp = im.ToBitmap();


            Haralick haralick = new Haralick();


            List<double[]> haraList = haralick.ProcessImage(bp);




            List<List<double>> res = new List<List<double>>();

          
            res.Add(new List<double>());
            res.Add(new List<double>());
            res.Add(new List<double>());
            res.Add(new List<double>());

            for (int i = 0; i < 4; i++)
            {
                CooccurrenceDegree d;
                switch (i)
                {
                    case 0: d = CooccurrenceDegree.Degree0;
                        break;
                    case 1:
                        d = CooccurrenceDegree.Degree45;
                        break;
                    case 2: d = CooccurrenceDegree.Degree90; break;
                    case 3: d = CooccurrenceDegree.Degree135; break;
                    default:
                        d = CooccurrenceDegree.Degree0;
                        break; 
                }
                Debug.WriteLine("hara degree:" + d);

                res[i].Add(haralick.Descriptors[0, 0][d].F01);
                res[i].Add(haralick.Descriptors[0, 0][d].F02);
                res[i].Add(haralick.Descriptors[0, 0][d].F03);
                res[i].Add(haralick.Descriptors[0, 0][d].F04);
                res[i].Add(haralick.Descriptors[0, 0][d].F05);
                res[i].Add(haralick.Descriptors[0, 0][d].F06);
                res[i].Add(haralick.Descriptors[0, 0][d].F07);
                res[i].Add(haralick.Descriptors[0, 0][d].F08);
                res[i].Add(haralick.Descriptors[0, 0][d].F09);
                res[i].Add(haralick.Descriptors[0, 0][d].F10);
                res[i].Add(haralick.Descriptors[0, 0][d].F11);
                res[i].Add(haralick.Descriptors[0, 0][d].F12);
                res[i].Add(haralick.Descriptors[0, 0][d].F13);
            }
            





            /*  HaralickDescriptor haraDesc = new HaralickDescriptor(null);

              List<double> res = new List<double>();
              res.Add(haraDesc.F01);  //Angular Second Momentum
              res.Add(haraDesc.F02);  //Contrast
              res.Add(haraDesc.F03);  //Correlation
              res.Add(haraDesc.F04);  //Sum of Squares: Variance.
              res.Add(haraDesc.F05);  //Inverse Difference Moment. 
              res.Add(haraDesc.F06);  //Sum Average. 
              res.Add(haraDesc.F07);  // Sum Variance
              res.Add(haraDesc.F08);  // Sum Entropy.
              res.Add(haraDesc.F09);  //Entropy. 
              res.Add(haraDesc.F10);  //Difference Variance. 
              res.Add(haraDesc.F11);  //Difference Entropy. 
              res.Add(haraDesc.F12);  // First Information Measure. 
              res.Add(haraDesc.F13);  //Second Information Measure. 
              res.Add(haraDesc.F14);  //Maximal Correlation Coefficient. */

            return res;

        }
    }
}
