using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

using Accord.MachineLearning;

//using processPerformance;
using System.Diagnostics;
using Emgu.CV.UI;
using Accord.Math;
using Accord.Imaging.Converters;
using System.Drawing;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics;
using Accord.Math.Decompositions;
using Accord.Math.Comparers;
using Accord.Controls;
using Accord.Statistics.Analysis;
using System.Numerics;

namespace SegmentationCreator.Process
{
    /// <summary>
    /// "HaploidUtils" is a static class where utils function were developped to make Haploid image processing function easier to read.
    /// </summary>
    public static class HaploidUtils
    {
        /* Histogram processing code:*/
        /// <summary>
        /// This function computes the image histogramm, accordingly to its mask. 
        /// </summary>
        /// <param name="im"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static DenseHistogram imhist(Image<Gray, Byte> im, Image<Gray, Byte> mask)
        {
            DenseHistogram hist = new DenseHistogram(256, new RangeF(0, 255));
            hist.Calculate<Byte>(new Image<Gray, Byte>[] { im }, false, mask);

            return hist;
        }
        /// <summary>
        /// This function smouthes the histogram based on AR linear filter.
        /// </summary>
        /// <param name="hist"></param>
        /// <param name="SmouthFilterOrder"></param>
        /// <returns></returns>
        public static DenseHistogram smouthHistogram(DenseHistogram hist, uint SmouthFilterOrder)
        {

            float[] t_hist = new float[256];

            if (SmouthFilterOrder < 2)
                return hist;

            // Histogram smouthing:
            for (int i = 0; i < SmouthFilterOrder - 1; i++)
            {
                t_hist[i] = (float)hist.MatND.ManagedArray.GetValue(i);
            }

            for (int i = (int)SmouthFilterOrder - 1; i < 256; i++)
            {
                for (int j = 0; j < SmouthFilterOrder; j++)
                {
                    t_hist[i] = t_hist[i] + (float)hist.MatND.ManagedArray.GetValue(i - j) / (float)SmouthFilterOrder;
                }
                t_hist[i] = (float)Math.Floor(t_hist[i]);
            }

            DenseHistogram histm = new DenseHistogram(256, new RangeF(0, 255));
            t_hist.CopyTo(histm.MatND.ManagedArray, 0);

            return histm;
        }


        /// <summary>
        /// This function return a list with each value of a histogram
        /// </summary>
        /// <param name="hist"></param>
        /// <returns></returns>
        public static List<float> HistToList(DenseHistogram hist)
        {
            List<float> l = new List<float>();

            for (int i = 0; i < hist.MatND.ManagedArray.Length; i++)
            {
                l.Add((float)hist.MatND.ManagedArray.GetValue(i)) ;
            }

            return l;
        }


        /// <summary>
        /// This function makes a list of data generated from a histogram.
        /// </summary>
        /// <param name="hist"></param>
        /// <returns></returns>
        public static List<float> DatafromHistogram(DenseHistogram hist)
        {
            List<float> l = new List<float>();
            List<float> t_l = new List<float>();

            for (int i = 0; i < hist.MatND.ManagedArray.Length; i++)
            {
                for (int j = 0; j < (float)hist.MatND.ManagedArray.GetValue(i); j++)
                {
                    t_l.Add((float)i);
                }
                l.AddRange(t_l);
                t_l.Clear();
            }

            return l;
        }
        /// <summary>
        /// This function makes a list of data generated from a histogram. The return format makes it to be used by Gaussian Mixture Model functions in "Accord.MachineLearning".
        /// </summary>
        /// <param name="hist"></param>
        /// <returns></returns>
        public static double[][] DatafromHistogram2(DenseHistogram hist)
        {
            List<float> l = DatafromHistogram(hist);
            double[][] l2 = new double[l.Count][];
            for (int i = 0; i < l.Count; i++)
            {
                l2[i] = new double[1];
                l2[i][0] = l[i];
            }

            return l2;

        }
        /// <summary>
        /// This function sorts from lower values to higher values and returns the index of each values sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int[] sort<T>(T[] data) where T : System.IComparable
        {
            int size = data.Length;
            if (size <= 1)
                return (new int[] { 0 });


            T transfert = data[0];
            int tempInd = 0;
            int[] ind = new int[size];

            for (int i = 0; i < size; i++)
            {
                ind[i] = i;
            }

            for (int i = 0; i < size - 1; i++)
            {
                if (data[i].CompareTo(data[i + 1]) > 0)
                {
                    transfert = data[i];
                    tempInd = ind[i];

                    ind[i] = ind[i + 1];
                    data[i] = data[i + 1];

                    data[i + 1] = transfert;
                    ind[i + 1] = tempInd;

                    if (i > 0)
                    {
                        i = i - 2;
                    }
                }
            }

            return ind;
        }
        /// <summary>
        /// Germe detection based on diploid seeds detection and their kernel. It returns the germe center.
        /// </summary>
        /// <param name="im"></param>
        /// <param name="mask"></param>
        /// <param name="epsilon_conv"></param>
        /// <param name="Kg"></param>
        /// <param name="filterOrder"></param>
        /// <returns></returns>
        public static int[] GermeDetection(Image<Gray, Byte> im, Image<Gray, Byte> mask, float epsilon_conv = (float)1e-2, int Kg = 3, uint filterOrder = 3)
        {
            DenseHistogram hist = imhist(im, mask);
            DenseHistogram histm = smouthHistogram(hist, filterOrder);
            double[][] data = DatafromHistogram2(histm);

            GaussianMixtureModel GMM = new GaussianMixtureModel(Kg);
            GMM.Compute(data, epsilon_conv);
            // mean, std and alpha
            List<double> mean = new List<double>();
            List<double> std = new List<double>();
            List<double> alpha = new List<double>();
            for (int i = 0; i < GMM.Gaussians.Count; i++)
            {
                mean.Add(GMM.Gaussians[i].Mean[0]);
                std.Add(Math.Sqrt(GMM.Gaussians[i].Covariance[0, 0]));
                alpha.Add(GMM.Gaussians[i].Proportion);
            }

            int[] ind = sort<double>(mean.ToArray());
            double[] thres = new double[Kg - 1];

            // Looking for Optimal thresholds:
            for (int k = 0; k < Kg - 1; k++)
            {
                double a = Math.Pow(1 / std[ind[k]], 2) - Math.Pow(1 / std[ind[k + 1]], 2);
                double b = mean[ind[k + 1]] / Math.Pow(std[ind[k + 1]], 2) - mean[ind[k]] / Math.Pow(std[ind[k]], 2);
                double c = Math.Pow(mean[ind[k]] / std[ind[k]], 2) - Math.Pow(mean[ind[k + 1]] / std[ind[k + 1]], 2) - 2 * Math.Log(alpha[ind[k]] * std[ind[k + 1]] / (alpha[ind[k + 1]] * std[ind[k]]));
                double delta = b * b - a * c;
                if (delta < 0)
                    delta = 0;

                double thres1 = (-b + Math.Sqrt(delta)) / a;
                double thres2 = (-b - Math.Sqrt(delta)) / a;

                if (thres1 > 255 || thres1 < 0)
                    thres[k] = thres2;
                else
                    thres[k] = thres1;
            }
            Array.Sort(thres);

            // Center of gravity computing:
            double n = 0, x = 0, y = 0;
            for (int i = 0; i < im.Size.Height; i++)
            {
                for (int j = 0; j < im.Size.Width; j++)
                {
                    if (im[i, j].Intensity < thres[0])
                    {
                        x += i;
                        y += j;
                        n++;
                    }
                }
            }
            x = Math.Floor(x / n);
            y = Math.Floor(y / n);

            return (new int[] { (int)x, (int)y });
        }
        /// <summary>
        /// This function extracts features from Gaussian Mixture Model.
        /// </summary>
        /// <param name="im"></param>
        /// <param name="mask"></param>
        /// <param name="epsilon_conv"></param>
        /// <param name="Kmog"></param>
        /// <param name="filterOrder"></param>
        /// <returns></returns>
        public static List<double> GMMFeaturesExtraction(Image<Gray, Byte> im, Image<Gray, Byte> mask = null, int Kmog = 2, float epsilon_conv = (float)1e-6, uint filterOrder = 3)
        {

            DenseHistogram hist = imhist(im, mask);

            DenseHistogram histm = smouthHistogram(hist, filterOrder);
            double[][] data = DatafromHistogram2(histm);
            
           // HistogramViewer.Show(histm, "histo");
            List<double> feat = new List<double>();


            GaussianMixtureModel GMM = new GaussianMixtureModel(Kmog);

            //GMM.Compute(data, epsilon_conv);

            var clusters = GMM.Learn(data);


            for (int i = 0; i < GMM.Gaussians.Count; i++)
            {
                feat.Add(GMM.Gaussians[i].Mean[0]);
                feat.Add(Math.Sqrt(GMM.Gaussians[i].Covariance[0, 0]));
                feat.Add(GMM.Gaussians[i].Proportion);
            }

            return feat;
        }

        public static int CountNoBlackPixel(Image<Luv, byte> im)
        {

            int nbPx = 0;
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int col = 0; col < im.Cols; col++)
                {
                    if (im[rw, col].X != 0 && im[rw, col].Y != 0 && im[rw, col].Z != 0)
                    {
                        nbPx++;
                    }
                }
            }
            return nbPx;
        }

        public static int CountNoBlackPixel(Image<Lab, byte> im)
        {

            int nbPx = 0;
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int col = 0; col < im.Cols; col++)
                {
                    if (im[rw, col].X != 0 && im[rw, col].Y != 0 && im[rw, col].Z != 0)
                    {
                        nbPx++;
                    }
                }
            }
            return nbPx;
        }


        public static void CalculatePCAofImage(Image<Luv, byte> im)
        {

            int sizeTab = CountNoBlackPixel(im);
            double[,] data = new double[sizeTab, 2];
            for (int rw = 0; rw < sizeTab; rw++)
                for (int col = 0; col < 2; col++)
                    data[rw, col] = 0;

            int idx = 0;
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int col = 0; col < im.Cols; col++)
                {
                    if (im[rw, col].X != 0 && im[rw, col].Y != 0 && im[rw, col].Z != 0)
                    {
                        data[idx, 0] = im[rw, col].Y; // u is Axis-X
                        data[idx, 1] = im[rw, col].X; // L is Axis-Y
                       // data[idx, 2] = im[rw, col].Z; // v is Axis-Z
                        idx++;
                    }
                }
            }



            double mean = data.Mean();
            double[,] dataAdjust = data.Subtract(mean);
            double[,] cov = dataAdjust.Covariance();
            var evd = new EigenvalueDecomposition(cov);
            double[] eigenvalues = evd.RealEigenvalues;
            double[,] eigenvectors = evd.Eigenvectors;

            eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));
            double[,] featureVector = eigenvectors;
            // Step 6. Deriving the new data set
            //double[,] finalData = dataAdjust.Multiply(eigenvectors);

            // Console.ReadKey();

            Debug.WriteLine("");
            Debug.WriteLine("Covariance Matrix: ");
            Debug.WriteLine(cov.ToString(" +0.0000000000;- 0.0000000000; "));
            Debug.WriteLine("");
            Debug.WriteLine("Eigenvalues: ");
            Debug.WriteLine(eigenvalues.ToString(" +0.0000000000;  - 0.0000000000; "));
            Debug.WriteLine("");
            Debug.WriteLine("Eigenvectors:");
            Debug.WriteLine(eigenvectors.ToString(" +0.0000000000;  - 0.0000000000; "));

            double[,] abs = new double[255, 2];
            double a = eigenvectors[1, 0] / eigenvectors[1, 1];
            double b = eigenvalues[1];
            Debug.WriteLine("ax+b : " + a + "x+" + b);
            //abs[0, 0] = 0;
            // abs[0, 1] = eigenvalues[1];




            for (int i = 0; i < 255; i++)
            {
                abs[i, 0] = i;
                abs[i, 1] = b + i * a;
            }



           /* var pca = new PrincipalComponentAnalysis(data); 
            // Step 3. Compute the analysis
            pca.Compute();
            double[,] finalData = pca.Transform(data);


            Debug.WriteLine("");

            Debug.WriteLine("pca : Eigenvalues: ");
            Debug.WriteLine(pca.Eigenvalues.ToString(" +0.0000000000;  - 0.0000000000; "));
            Debug.WriteLine("");
            Debug.WriteLine("pca: Eigenvectors:");
            Debug.WriteLine(pca.ComponentVectors.ToString(" +0.0000000000;  - 0.0000000000; "));

            Debug.WriteLine("pca : StandardDeviations:");
             Debug.WriteLine(pca.StandardDeviations.ToString(" +0.0000000000;  - 0.0000000000; "));

            Debug.WriteLine("pca : Means:");
            Debug.WriteLine(pca.Means.ToString(" +0.0000000000;  - 0.0000000000; "));

               PointF pt = new PointF((float)pca.Means[0], (float)pca.Means[1]);
               Vector2 v = new Vector2((float)pca.ComponentVectors[0][0], (float)pca.ComponentVectors[0][1]);

            //PointF pt = new PointF((float)173, (float)96);
           // Vector2 v = new Vector2((float)0.61, (float)0.79);


            double ellipseCov;
            double ratioEC = 0;
            //double[] sigma = { pca.StandardDeviations[0], pca.StandardDeviations[1] };
            double[] sigma = { 21.14,  16.6 };

            double[] ecart = new double[sizeTab];
            double ratio = 0;
            mean = 0;
            for (int i = 0; i < sizeTab; i++)
            {
                PointF p1 = new PointF((float)data[i, 0], (float)data[i, 1]);

               
                ecart[i] = DistancePointToVector(p1, pt, v);
                if (ecart[i] < 16)
                    ratio++;
                mean += ecart[i];

                PointF p2 = new PointF((float)data[i, 0]-pt.X, (float)data[i, 1]-pt.Y);

                ellipseCov = Math.Pow((p2.X / sigma[0] ), 2) + Math.Pow((p2.Y / sigma[1]), 2);

                if (ellipseCov < 5.991)
                    ratioEC++;
            }

            ratio /= sizeTab;
            ratio *= 100;

            ratioEC /= sizeTab;
            ratioEC *= 100;

            mean /= sizeTab;

            Debug.WriteLine("ratioEC:" + ratioEC + "%");
            Debug.WriteLine("ratio:" + ratio + "%");
            Debug.WriteLine("mean:" + mean );


            ScatterplotBox.Show("Original PCA data", data);
            ScatterplotBox.Show("Final PCA data", finalData);
           // ScatterplotBox.Show("abs PCA data", abs);
            ScatterplotBox.Show("ecart PCA data", ecart);*/

        }

        public static double DistancePointToVector(PointF pt, PointF p1, Vector2 vect)
        {
            PointF closest;
            PointF p2 = new PointF(p1.X + vect.X, p1.Y + vect.Y);

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
            dx = pt.X - closest.X;
            dy = pt.Y - closest.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static List<double> GMMFeaturesExtraction(Image<Lab, Byte> im, Image<Gray, Byte> mask = null, float epsilon_conv = (float)1e-6, int Kmog = 4, uint filterOrder = 3)
        {

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: 0, max: 65025);
            ArrayToImage arrayToImage = new ArrayToImage(im.Width, im.Height, min: 0, max: 65025);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(im.Bitmap, out pixels);

            List<double> feat = new List<double>();

            GaussianMixtureModel GMM = new GaussianMixtureModel(Kmog);

            var clusters = GMM.Learn(pixels);

            for (int i = 0; i < GMM.Gaussians.Count; i++)
            {
                feat.Add(GMM.Gaussians[i].Mean[0]);
                feat.Add(Math.Sqrt(GMM.Gaussians[i].Covariance[0, 0]));
                feat.Add(GMM.Gaussians[i].Proportion);
            }


            return feat;
        }

        public static List<double> GMMFeaturesExtraction(Image<Luv, Byte> im, Image<Gray, Byte> mask = null, float epsilon_conv = (float)1e-6, int Kmog = 4, uint filterOrder = 3)
        {


            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: 0, max: 65025);
            ArrayToImage arrayToImage = new ArrayToImage(im.Width, im.Height, min: 0, max: 65025);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(im.Bitmap, out pixels);

            List<double> feat = new List<double>();

            GaussianMixtureModel GMM = new GaussianMixtureModel(Kmog);

            var clusters = GMM.Learn(pixels);

            for (int i = 0; i < GMM.Gaussians.Count; i++)
            {
                feat.Add(GMM.Gaussians[i].Mean[0]);
                feat.Add(Math.Sqrt(GMM.Gaussians[i].Covariance[0, 0]));
                feat.Add(GMM.Gaussians[i].Proportion);
            }


            return feat;
        }

        public static Image<Lab, Byte> Kmeans(Image<Lab, Byte> im, Image<Gray, Byte> mask, out List<double> features, float epsilon_conv = (float)1e-6, int Kmog =10, uint filterOrder = 3)
        {

            int k = Kmog;
            ImageToArray imageToArray = new ImageToArray(min: 0, max: 65025);
            ArrayToImage arrayToImage = new ArrayToImage(im.Width, im.Height, min: 0, max: 65025);

            // Transform the image into an array of pixel values
            double[][] pixels = new double[255][];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new double[255];
            }

            imageToArray.Convert(im.Bitmap, out pixels);

            /*for (int r = 0; r < im.Rows; r++)
            {
                for (int c = 0; c < im.Cols; c++)
                {
                    pixels[r][c]++;
                }
            }*/

            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k, Distance.SquareEuclidean);

            // We will compute the K-Means algorithm until cluster centroids
            // change less than 0.5 between two iterations of the algorithm
            kmeans.Tolerance = 0.05;

            // Learn the clusters in the data
            var clustering = kmeans.Learn(pixels);

            // Use clusters to decide class labels
            int[] idx = clustering.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);
            features = new List<double>();


            for (int i = 0; i < kmeans.Clusters.Count; i++)
            {
                features.Add(kmeans.Clusters[i].Centroid[0]);
                features.Add(Math.Sqrt(kmeans.Clusters[i].Covariance[0][0]));
                features.Add(kmeans.Clusters[i].Proportion);
            }

            // Retrieve the resulting image in a picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);


            return new Image<Lab, Byte>(result);
        }

        

        public static Image<Lab, Byte> Kmeans(Image<Lab, Byte> im, int Kmog = 5, float epsilon_conv = (float)1e-6, uint filterOrder = 3)
        {

            int k = Kmog;
            ImageToArray imageToArray = new ImageToArray(min: 0, max: 65025);
            ArrayToImage arrayToImage = new ArrayToImage(im.Width, im.Height, min: 0, max: 65025);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(im.Bitmap, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k, Distance.SquareEuclidean);

            // We will compute the K-Means algorithm until cluster centroids
            // change less than 0.5 between two iterations of the algorithm
            kmeans.Tolerance = 0.05;

            // Learn the clusters in the data
            var clustering = kmeans.Learn(pixels);

            // Use clusters to decide class labels
            int[] idx = clustering.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);
          
            // Retrieve the resulting image in a picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);


            return new Image<Lab, Byte>(result);
        }



        public static Image<Lab, Byte> MinShift(Image<Lab, Byte> im, Image<Gray, Byte> mask, float epsilon_conv = (float)1e-6, int Kmog = 2, uint filterOrder = 3)
        {

            int pixelSize = 3;   // RGB color pixel
            double sigma = 0.06; // kernel bandwidth

            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(im.Width, im.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(im.Bitmap, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            MeanShift meanShift = new MeanShift(pixelSize, new GaussianKernel(3), sigma);

            // We will compute the mean-shift algorithm until the means
            // change less than 0.5 between two iterations of the algorithm
            meanShift.Tolerance = 0.05;
            meanShift.MaxIterations = 10;

            // Learn the clusters in the data
            var clustering = meanShift.Learn(pixels);

            // Use clusters to decide class labels
            int[] idx = clustering.Decide(pixels);

            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => meanShift.Clusters.Modes[idx[i]]);

            // Retrieve the resulting image in a picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);


            return new Image<Lab, Byte>(result);
        }



        public static double[][] StackImage3D(Image<Lab, Byte> im, Image<Gray, Byte> mask)
        {
            double[][][] value = new double[256][][];

            for (int i = 0; i < 256; i++)
            {
                value[i] = new double[256][];
                for (int j = 0; j < 256; j++)
                {
                    value[i][j] = new double[256];
                    for (int k = 0; k < 256; k++)
                    {
                        value[i][j][k] = 0;
                    }
                }
            }

            for (int i = 0; i < im.Height; i++)
            {
                for (int j = 0; j < im.Width; j++)
                {
                    if (mask[i, j].Intensity != 0)
                    {
                        value[(int)im[i, j].X][(int)im[i, j].Y][(int)im[i, j].Z]++;
                    }
                }
            }

            return Matrix.Stack(value);


        }

        public static double[][] StackImage2D(Image<Lab, Byte> im, Image<Gray, Byte> mask)
        {
            double[][] value = new double[256][];

            for (int i = 0; i < 256; i++)
            {
                value[i] = new double[256];
                for (int j = 0; j < 256; j++)
                {
                    value[i][j] = 1;
                }
            }

            for (int i = 0; i < im.Height; i++)
            {
                for (int j = 0; j < im.Width; j++)
                {
                    if (mask[i, j].Intensity != 0)
                    {
                        value[(int)im[i, j].Z][(int)im[i, j].Y]++;
                    }
                }
            }

            return value;
        }



    }
}
