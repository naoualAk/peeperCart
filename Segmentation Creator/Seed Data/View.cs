using System;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;
using System.Numerics;
using SegmentationCreator.Process;

namespace SegmentationCreator.Seed_Data
{
    public class View
    {
        Point hatPoint, catchPoint;
        int idCatchPoint, idHatPoint;

        PointF centroidPoint;

        double lengthMax, lengthMin;

        System.Numerics.Vector2 length;

        Contour<Point> approxContour;

        Seq<Point> convexHull;
        Rectangle roi;
        Contour<Point> contour;
        MCvBox2D rectMinArea;
        Ellipse ellipse;

        int id;

        public View()
        {
            approxContour = null;
            convexHull = null;
            contour = null;
            idCatchPoint = -1;
            idHatPoint = -1;
        }

        /* return Distance between Point pt and Segment [p1, p2]
         * point on segment nearest of point = closest 
         */
        private double DistancePointToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
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

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }


            return Math.Sqrt(dx * dx + dy * dy);
        }

        // return angle between two vectors
        private double angle(Vector2 v1, Vector2 v2)
        {
            double angle = (Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X)) * 180.0 / Math.PI;
            return angle < 0 ? angle + 360 : angle;
        }

        private double angleRad(Vector2 v1, Vector2 v2)
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

        //return angle beetween (idCorner-1, idCorner, idCorner+1) vertices of ApproxContour
        public double angleApproxContour(int idCorner)
        {
            int modulo = approxContour.Total;

            int id1 = (idCorner - 1) % modulo;
            id1 = id1 < 0 ? (modulo + id1) : id1;
            int id2 = (idCorner + 1) % modulo;
            idCorner = idCorner % modulo;

            Vector2 v1 = new Vector2(approxContour[id1].X - approxContour[idCorner].X, approxContour[id1].Y - approxContour[idCorner].Y);
            Vector2 v2 = new Vector2(approxContour[id2].X - approxContour[idCorner].X, approxContour[id2].Y - approxContour[idCorner].Y);

            return angle(v1, v2);
        }

        [Obsolete("Use Method Width")]
        public Vector2 WidthApproxContour()
        {
            if (approxContour.Total > 2)
            {
                int id1 = (idCatchPoint - 1) % approxContour.Total;
                id1 = id1 < 0 ? (approxContour.Total + id1) : id1;
                int id2 = (idCatchPoint + 1) % approxContour.Total;
                idCatchPoint = idCatchPoint % approxContour.Total;
                return new Vector2(approxContour[id1].X - approxContour[id2].X, approxContour[id1].Y - approxContour[id2].Y);
            }
            else return new Vector2(0, 0);
        }

        //return mean distance between [idCorner,idCorner+1] and [idCorner,idCorner-1] vertices of ApproxContour
        public float MeanDistanceBeetweenConsecutivePoints(int idCorner)
        {
            int modulo = approxContour.Total;

            int id1 = (idCorner - 1) % modulo;
            id1 = id1 < 0 ? (modulo + id1) : id1;
            int id2 = (idCorner + 1) % modulo;
            idCorner = idCorner % modulo;

            Vector2 v1 = new Vector2(approxContour[id1].X - approxContour[idCorner].X, approxContour[id1].Y - approxContour[idCorner].Y);
            Vector2 v2 = new Vector2(approxContour[id2].X - approxContour[idCorner].X, approxContour[id2].Y - approxContour[idCorner].Y);

            return (v1.Length() + v2.Length()) / 2;
        }

        public bool calcCatchPointbyApproxContour(Point refPoint)
        {
            if (approxContour == null) return false;
            if (CentroidPoint == new PointF(0, 0)) return false;

            double[] angles = new double[approxContour.Total];
            int idx = -1;
            for (int i = 0; i < approxContour.Total; i++)
            {
                angles[i] = angleApproxContour(i);
                if (Math.Abs(approxContour[i].Y - refPoint.Y) < 30)
                    if (angles[i] < 100)
                    {
                        idx = i;
                    }
            }

            if (idx == -1) return false;

            return true;
        }

        public bool MaxLengthContour()
        {
            Point p1, p2;
            return MaxLengthContour(out p1, out p2);
        }

        public bool MaxLengthContour(out Point p1, out Point p2)
        {
            p1 = new Point();
            p2 = new Point();
            if (contour == null) return false;

            lengthMax = 0;
            int jMax = 0, iMax = 0;
            for (int i = 0; i < contour.Total; i++)
            {
                for (int j = 0; j < contour.Total; j++)
                {
                    if (i != j)
                    {
                        double lengthTemp = _2D.length2P(contour[i], contour[j]);
                        if (lengthTemp > lengthMax)
                        {
                            lengthMax = lengthTemp;
                            iMax = i;
                            jMax = j;
                        }
                    }
                }
            }

            hatPoint = p1 = contour[iMax];
            catchPoint = p2 = contour[jMax];

            idCatchPoint = idNearestPointAndApproxContour(catchPoint);
            idHatPoint = idNearestPointAndApproxContour(hatPoint);

            return true;
        }

        public double MaxLengthApproxContour()
        {
            double lengthMax = 0;
            int jMax = 0, iMax = 0;
            for (int i = 0; i < approxContour.Total; i++)
            {
                for (int j = 0; j < approxContour.Total; j++)
                {
                    if (i != j)
                    {
                        double lengthTemp = _2D.length2P(approxContour[i], approxContour[j]);
                        if (lengthTemp > lengthMax)
                        {
                            lengthMax = lengthTemp;
                            iMax = i;
                            jMax = j;
                        }
                    }
                }
            }

            catchPoint = approxContour[iMax];
            idCatchPoint = iMax;
            hatPoint = approxContour[jMax];
            idHatPoint = jMax;
            this.lengthMax = lengthMax;

            length = new Vector2(CatchPoint.X - HatPoint.X, CatchPoint.Y - HatPoint.Y);

            return lengthMax;
        }

        private bool LineSegmentsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
    out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vector2();

            Vector2 r = p2 - p;
            Vector2 s = q2 - q;
            float rxs = Cross(r, s);
            float qpxr = Cross((q - p), r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs == 0 && qpxr == 0)
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                //          if (considerCollinearOverlapAsIntersect)
                //          if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                //              return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs == 0 && qpxr != 0)
                return false;

            // t = (q - p) x s / (r x s)
            float t = Cross((q - p), s) / rxs;

            // u = (q - p) x r / (r x s)

            float u = Cross((q - p), r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (rxs != 0 && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        private double AngleRad(Vector2 v1, Vector2 v2)
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


        private int[] idExtremePoint(Seq<Point> convexHull)
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


        //a utiliser!
        private Vector2[] RotatingCaliper(Seq<Point> convexHull)
        {
            int[] idExtremePoints = idExtremePoint(convexHull);
            int pA = idExtremePoints[0], pB = idExtremePoints[1];
            double rotatedAngle = 0;
            double minLength = double.PositiveInfinity;
            double maxLength = 0;


            Vector2[] vectors = new Vector2[2];

            Point pTemp1 = new Point(), pTemp2 = new Point();

            Vector2 caliperA = new Vector2(1, 0);
            Vector2 caliperB = new Vector2(-1, 0);
            caliperA = Vector2.Normalize(caliperA);
            caliperB = Vector2.Normalize(caliperB);

            while (rotatedAngle < Math.PI)
            {
                Vector2 egdeA = new Vector2(convexHull[(pA + 1) % convexHull.Total].X - convexHull[pA].X, convexHull[(pA + 1) % convexHull.Total].Y - convexHull[pA].Y);
                Vector2 egdeB = new Vector2(convexHull[(pB + 1) % convexHull.Total].X - convexHull[pB].X, convexHull[(pB + 1) % convexHull.Total].Y - convexHull[pB].Y);

                double angleA = AngleRad(egdeA, caliperA);
                double angleB = AngleRad(egdeB, caliperB);
                double length = 0;

                caliperA = _2D.rotate(caliperA, Math.Min(angleA, angleB));
                caliperB = _2D.rotate(caliperB, Math.Min(angleA, angleB));
                caliperA = Vector2.Normalize(caliperA);
                caliperB = Vector2.Normalize(caliperB);

                if (angleA <= angleB)
                {
                    pA = (pA + 1) % convexHull.Total;
                    if (pB != pA)
                    {
                        length = _2D.DistancePointToVector(convexHull[pB], convexHull[pA], caliperA, out pTemp2);
                        pTemp1 = convexHull[pB];
                    }
                }
                else
                {
                    pB = (pB + 1) % convexHull.Total;
                    if (pB != pA)
                    {
                        length = _2D.DistancePointToVector(convexHull[pA], convexHull[pB], caliperB, out pTemp1);
                        pTemp2 = convexHull[pA];
                    }
                }

                rotatedAngle = rotatedAngle + Math.Min(angleA, angleB);

                if (length < minLength && pB != pA)
                {
                    minLength = length;
                    vectors[0].Y = pTemp2.Y - pTemp1.Y;
                    vectors[0].X = pTemp2.X - pTemp1.X;
                }
                if (length > maxLength && pB != pA)
                {
                    maxLength = length;
                    vectors[1].Y = pTemp2.Y - pTemp1.Y;
                    vectors[1].X = pTemp2.X - pTemp1.X;
                }
            }

            lengthCalip = maxLength;
            widthCalip = minLength;

            return vectors;
        }

        private double lengthCalip = 0;

        public double LengthCalip
        {
            get
            {
                if (lengthCalip == 0)
                    RotatingCaliper(convexHull);
                return lengthCalip;
            }
            set { lengthCalip = value; }
        }

        private double widthCalip = 0;

        public double WidthCalip
        {
            get
            {
                if (widthCalip == 0)
                    RotatingCaliper(convexHull);
                return widthCalip;
            }
            set { widthCalip = value; }
        }





        public double rotCaliperMinWidth(out Point p1, out Point p2, out Vector2 direction)
        {

            int[] idExtremePoints = idExtremePoint();
            int pA = idExtremePoints[0], pB = idExtremePoints[1];
            double rotatedAngle = 0;
            double minWidth = double.PositiveInfinity;

            p1 = new Point();
            p2 = new Point();
            direction = new Vector2();

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

                if (width < minWidth && pB != pA)
                {
                    minWidth = width;
                    p1 = pTemp1;
                    p2 = pTemp2;
                    direction = caliperA;
                }
            }
            return minWidth;
        }

        public double rotCaliperMaxLength(out Point p1, out Point p2, out Vector2 direction)
        {

            int[] idExtremePoints = idExtremePoint();
            int pA = idExtremePoints[0], pB = idExtremePoints[1];
            double rotatedAngle = 0;
            double maxLength = 0;

            p1 = new Point();
            p2 = new Point();
            direction = new Vector2();

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

                if (width > maxLength && pB != pA)
                {
                    maxLength = width;
                    p1 = pTemp1;
                    p2 = pTemp2;
                    direction = caliperA;
                }
            }
            return maxLength;
        }

        /* public ShadeDescriptor RotatingCaliper()
         {
             int[] idExtremePoints = idExtremePoint();
             int pA = idExtremePoints[0], pB = idExtremePoints[1];
             double rotatedAngle = 0;
             double minLength = double.PositiveInfinity;
             double maxLength = 0;


             ShadeDescriptor desc = new ShadeDescriptor();

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
                 double length = 0;

                 caliperA = _2D.rotate(caliperA, Math.Min(angleA, angleB));
                 caliperB = _2D.rotate(caliperB, Math.Min(angleA, angleB));
                 caliperA = Vector2.Normalize(caliperA);
                 caliperB = Vector2.Normalize(caliperB);

                 if (angleA <= angleB)
                 {
                     pA = (pA + 1) % convexHull.Total;
                     if (pB != pA)
                     {
                         length = _2D.DistancePointToVector(convexHull[pB], convexHull[pA], caliperA, out pTemp2);
                         pTemp1 = convexHull[pB];
                     }
                 }
                 else
                 {
                     pB = (pB + 1) % convexHull.Total;
                     if (pB != pA)
                     {
                         length = _2D.DistancePointToVector(convexHull[pA], convexHull[pB], caliperB, out pTemp1);
                         pTemp2 = convexHull[pA];
                     }
                 }

                 rotatedAngle = rotatedAngle + Math.Min(angleA, angleB);

                 if (length < minLength && pB != pA)
                 {
                     minLength = length;
                     desc.width.length = (float)length;
                     desc.width.Pt1 = pTemp1;
                     desc.width.Pt2 = pTemp2;
                 }
                 if (length > maxLength && pB != pA)
                 {
                     maxLength = length;
                     desc.height.length = (float)length;
                     desc.height.Pt1 = pTemp1;
                     desc.height.Pt2 = pTemp2;
                 }
             }

             desc.width.X = Math.Abs(desc.width.Pt1.X - desc.width.Pt2.X);
             desc.width.Y = Math.Abs(desc.width.Pt1.Y - desc.width.Pt2.Y);
             desc.height.X = Math.Abs(desc.height.Pt1.X - desc.height.Pt2.X);
             desc.height.Y = Math.Abs(desc.height.Pt1.Y - desc.height.Pt2.Y);

             return desc;
         }*/



        public Vector2[] RotatingCaliper()
        {
            int[] idExtremePoints = idExtremePoint();
            int pA = idExtremePoints[0], pB = idExtremePoints[1];
            double rotatedAngle = 0;
            double minLength = double.PositiveInfinity;
            double maxLength = 0;


            Vector2[] vectors = new Vector2[2];

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
                double length = 0;

                caliperA = _2D.rotate(caliperA, Math.Min(angleA, angleB));
                caliperB = _2D.rotate(caliperB, Math.Min(angleA, angleB));
                caliperA = Vector2.Normalize(caliperA);
                caliperB = Vector2.Normalize(caliperB);

                if (angleA <= angleB)
                {
                    pA = (pA + 1) % convexHull.Total;
                    if (pB != pA)
                    {
                        length = _2D.DistancePointToVector(convexHull[pB], convexHull[pA], caliperA, out pTemp2);
                        pTemp1 = convexHull[pB];
                    }
                }
                else
                {
                    pB = (pB + 1) % convexHull.Total;
                    if (pB != pA)
                    {
                        length = _2D.DistancePointToVector(convexHull[pA], convexHull[pB], caliperB, out pTemp1);
                        pTemp2 = convexHull[pA];
                    }
                }

                rotatedAngle = rotatedAngle + Math.Min(angleA, angleB);

                if (length < minLength && pB != pA)
                {
                    minLength = length;
                    vectors[0].Y = pTemp2.Y - pTemp1.Y;
                    vectors[0].X = pTemp2.X - pTemp1.X;
                    /* desc.width.length = (float)length;
                     desc.width.Pt1 = pTemp1;
                     desc.width.Pt2 = pTemp2;*/
                }
                if (length > maxLength && pB != pA)
                {
                    maxLength = length;
                    vectors[1].Y = pTemp2.Y - pTemp1.Y;
                    vectors[1].X = pTemp2.X - pTemp1.X;
                    /*  desc.height.length = (float)length;
                      desc.height.Pt1 = pTemp1;
                      desc.height.Pt2 = pTemp2;*/
                }
            }

            /* desc.width.X = Math.Abs(desc.width.Pt1.X - desc.width.Pt2.X);
             desc.width.Y = Math.Abs(desc.width.Pt1.Y - desc.width.Pt2.Y);
             desc.height.X = Math.Abs(desc.height.Pt1.X - desc.height.Pt2.X);
             desc.height.Y = Math.Abs(desc.height.Pt1.Y - desc.height.Pt2.Y);*/

            // vectors 0 = width
            // vectors 1 = height
            return vectors;
        }

        public double rotCaliperMinWidth()
        {
            int[] idExtremePoints = idExtremePoint();
            int pA = idExtremePoints[0], pB = idExtremePoints[1];
            double rotatedAngle = 0;
            double minWidth = double.PositiveInfinity;

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

                if (width < minWidth && pB != pA)
                {
                    minWidth = width;
                }
            }
            return minWidth;
        }

        public void rotCaliper(out double _height, out double _width)
        {
            int[] idExtremePoints = idExtremePoint();
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
        }



        public int[] idExtremePoint()
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

        public int[] idExtremePoint(Point[] pts)
        {

            if (pts.Length < 0) return null;

            int ymin = pts[0].Y, ymax = pts[0].Y;
            int[] idx = new int[2];

            for (int i = 0; i < pts.Length; i++)
            {
                if (ymin > pts[i].Y)
                {
                    ymin = pts[i].Y;
                    idx[0] = i;
                }
                if (ymax < pts[i].Y)
                {
                    ymax = pts[i].Y;
                    idx[1] = i;
                }
            }

            return idx;
        }

        public int[] GetXExtremePoints()
        {

            if (contour.Total < 0) return null;

            int ymin = contour[0].Y, ymax = contour[0].Y;
            int[] idx = new int[2];
            idx[0] = contour[0].X;
            idx[1] = contour[0].X;

            for (int i = 0; i < contour.Total; i++)
            {
                if (ymin > contour[i].Y)
                {
                    ymin = contour[i].Y;
                    idx[0] = contour[i].X;
                }
                if (ymax < contour[i].Y)
                {
                    ymax = contour[i].Y;
                    idx[1] = contour[i].X;
                }
            }

            return idx;
        }

        private float Cross(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public void HatPointbyApproxContour(Point hatPointRef)
        {
            if (approxContour == null) return;
            if (approxContour.Total < 1) return;

            idCatchPoint = idNearestPointAndApproxContour(catchPoint);

            if (approxContour.Total == 3)
            {
                idHatPoint = -1;
                hatPoint = intersection(approxContour[(idCatchPoint + 1) % 3], approxContour[(idCatchPoint + 2) % 3], hatPointRef);
            }
            else
            {
                double maxLength = 0;
                int maxIdx = 0;
                for (int i = 0; i < approxContour.Total; i++)
                {
                    double tempLength = _2D.length2P(approxContour[i], approxContour[idCatchPoint]);
                    if (maxLength < tempLength)
                    {
                        maxLength = tempLength;
                        maxIdx = i;
                    }
                }
                idHatPoint = maxIdx;
                hatPoint = approxContour[maxIdx];
            }
        }


        //return farthest id points of ApproxContour and point ref
        private int idFarthestPointAndApproxContour(Point ptRef)
        {
            return idFarthestPointAndApproxContour(new PointF(ptRef.X, ptRef.Y));
        }

        private int idFarthestPointAndApproxContour(PointF ptRef)
        {
            if (approxContour == null) return -1;
            if (approxContour.Total < 1) return -1;

            double maxDist = _2D.length2P(ptRef, approxContour[0]);
            int maxIdx = 0;
            for (int i = 1; i < approxContour.Total; i++)
            {
                double tempLength = _2D.length2P(ptRef, approxContour[i]);
                if (maxDist < tempLength)
                {
                    maxDist = tempLength;
                    maxIdx = i;
                }
            }
            return maxIdx;
        }

        public bool MaxWidth(out Point p3, out Point p4)
        {
            return MaxWidth(CatchPoint, HatPoint, out p3, out p4);
        }

        private bool MaxWidth(Point p1, Point p2, out Point p3, out Point p4)
        {
            p3 = new Point();
            p4 = new Point();
            Vector2 vLength = new Vector2(p2.X - p1.X, p2.Y - p1.Y);
            Vector2 perpLength = new Vector2(-vLength.Y, vLength.X);


            Point pIntersect1 = new Point(), pIntersect2 = new Point();
            int div = 10;
            float maxLength = 0;

            for (int i = 1; i < div; i++)
            {
                Point pOrigin = fragSegment(p1, p2, (i / (float)div));
                float length;
                bool correctLength1 = false, correctLength2 = false;
                for (int j = (idCatchPoint) % approxContour.Total; (j % approxContour.Total) != idHatPoint; j++)
                {
                    Point pointTemp;
                    if (intersection(approxContour[j], approxContour[j + 1], pOrigin, perpLength, out pointTemp))
                    {
                        pIntersect1 = pointTemp;
                        correctLength1 = true;
                    }
                }
                for (int j = idHatPoint; j % approxContour.Total != idCatchPoint; j++)
                {
                    Point pointTemp;
                    if (intersection(approxContour[j % approxContour.Total], approxContour[(j + 1) % approxContour.Total], pOrigin, perpLength, out pointTemp))
                    {
                        pIntersect2 = pointTemp;
                        correctLength2 = true;
                    }
                }
                length = (float)_2D.length2P(pIntersect1, pIntersect2);
                if (correctLength1 && correctLength2 && length > maxLength)
                {
                    maxLength = length;
                    p3 = pIntersect1;
                    p4 = pIntersect2;
                }

            }
            return true;
        }

        private bool intersection(Point p1, Point p2, Point p3, Vector2 v3, out Point pIntersect)
        {

            pIntersect = new Point();


            float a1 = (p2.Y - p1.Y) / (float)(p2.X - p1.X);
            float b1 = p1.Y - a1 * p1.X;

            float a2 = v3.Y / v3.X;
            float b2 = p3.Y - a2 * p3.X;

            float x, y;
            if (a1 != a2 && p2.X != p1.X)
            {
                x = (b2 - b1) / (a1 - a2);
                y = a1 * x + b1;

                if ((p1.X < p2.X && (x < p1.X || x > p2.X))
                    || (p1.X > p2.X && (x > p1.X || x < p2.X))
                    || (p1.Y < p2.Y && (y < p1.Y || y > p2.Y))
                    || (p1.Y > p2.Y && (y > p1.Y || y < p2.Y))
                    )
                    return false;
                else
                {
                    pIntersect = new Point((int)x, (int)y);
                    return true;
                }
            }
            else return false;
        }


        public Point fragSegment(Point p1, Point p2, float div)
        {
            if (div > 1 || div < 0) return new Point();
            Vector2 vLength = new Vector2(p2.X - p1.X, p2.Y - p1.Y);
            float coefX = div * vLength.X;
            float coefY = div * vLength.Y;

            return new Point(p1.X + (int)coefX, p1.Y + (int)coefY);
        }


        // return nearest id point of ApproxContour and point ref 
        private int idNearestPointAndApproxContour(Point ptRef)
        {
            if (approxContour == null) return -1;
            if (approxContour.Total < 1) return -1;

            double maxDist = _2D.length2P(ptRef, approxContour[0]);
            int maxIdx = 0;
            for (int i = 1; i < approxContour.Total; i++)
            {
                double tempLength = _2D.length2P(ptRef, approxContour[i]);
                if (maxDist > tempLength)
                {
                    maxDist = tempLength;
                    maxIdx = i;
                }
            }
            return maxIdx;
        }

        Point intersection(Point p1, Point p2, Point p3_horizontal)
        {

            float a = (p2.Y - p1.Y) / (float)(p2.X - p1.X);
            float b = p1.Y - a * p1.X;
            float x, y;
            if (a != (float)0.0)
            {
                x = (p3_horizontal.Y - b) / a;
                y = p3_horizontal.Y;
                if (p1.X < p2.X && (x < p1.X || x > p2.X) || p1.X > p2.X && (x > p1.X || x < p2.X)) // if x is out of segment
                {
                    x = (p2.X + p1.X) / 2;
                }
                if (p1.Y < p2.Y && (y < p1.Y || y > p2.Y) || p1.Y > p2.Y && (y > p1.Y || y < p2.Y)) // if y is out of segment
                {
                    float i1 = Math.Abs(p1.Y - y);
                    float i2 = Math.Abs(p2.Y - y);

                    if (i1 < i2)
                        y = p1.Y;
                    else y = p2.Y;
                }
                return new Point((int)x, (int)y);
            }
            else return midSegment(p1, p2);
        }




        Point midSegment(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }



        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Rectangle Roi
        {
            get { return roi; }
            set { roi = value; }
        }


        public Contour<Point> Contour
        {
            get { return contour; }
            set { contour = value; }
        }


        public MCvBox2D RectMinArea
        {
            get { return rectMinArea; }
            set { rectMinArea = value; }
        }


        public Ellipse Ellipse
        {
            get { return ellipse; }
            set { ellipse = value; }
        }

        public Contour<Point> ApproxContour
        {
            get
            {
                return approxContour;
            }

            set
            {
                approxContour = value;
            }
        }

        public Point HatPoint
        {
            get
            {
                return hatPoint;
            }

            set
            {
                hatPoint = value;
            }
        }

        public Point CatchPoint
        {
            get
            {
                return catchPoint;
            }

            set
            {
                catchPoint = value;
            }
        }

        public PointF CentroidPoint
        {
            get
            {
                if (centroidPoint == new PointF(0, 0))
                {
                    if (contour == null)
                    {
                        return new PointF(0, 0);
                        Debug.WriteLine("Contour is null. Impossible to calculate CentroidPoint");
                    }

                    MCvMoments m = contour.GetMoments();
                    centroidPoint = new PointF((float)(m.m10 / m.m00), (float)(m.m01 / m.m00));
                }
                return centroidPoint;
            }

            set
            {
                centroidPoint = value;
            }
        }


        public double LengthMax
        {
            get
            {
                if (lengthMax == 0) MaxLengthContour();
                return lengthMax;
            }

            set
            {
                lengthMax = value;
            }
        }

        public double LengthMin
        {
            get
            {
                return lengthMin;
            }

            set
            {
                lengthMin = value;
            }
        }

        public Seq<Point> ConvexHull
        {
            get
            {
                return convexHull;
            }

            set
            {
                convexHull = value;
            }
        }

        public int IdHatPoint
        {
            get
            {
                return idHatPoint;
            }

            set
            {
                idHatPoint = value;
            }
        }

        public int IdCatchPoint
        {
            get
            {
                return idCatchPoint;
            }

            set
            {
                idCatchPoint = value;
            }
        }
    }
}
