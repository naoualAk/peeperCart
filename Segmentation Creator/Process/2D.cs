using System;
using System.Drawing;
using System.Numerics;

namespace SegmentationCreator.Process
{
    public static class _2D
    {
        public static double length2P(Point p1, Point p2)
        {
            return length2P(new PointF(p1.X, p1.Y), new PointF(p2.X, p2.Y));
        }

        public static double length2P(PointF p1, PointF p2)
        {
            return Math.Sqrt((double)Math.Abs(p2.X - p1.X) * Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y) * Math.Abs(p2.Y - p1.Y));
        }

        public static double Scalar(Vector v1, Vector v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z) / (v1.length * v2.length);
        }

        public static Vector2 rotate(Vector2 v, double angleRad)
        {

            float X = v.X;
            float Y = v.Y;
            float cos = (float)Math.Cos(angleRad);
            float sin= (float)Math.Sin(angleRad);

            return new Vector2(X*cos-Y*sin,X*sin+Y*cos);
        }

        public static double DistancePointToSegment(PointF pt, PointF p1, Vector2 vect, out PointF closest)
        {

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

        public static double DistancePointToVector(PointF pt, PointF p1, Vector2 vect, out Point pClosest)
        {
            PointF closest=new PointF();

            float dx = vect.X;
            float dy = vect.Y;

            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                pClosest = new Point((int)closest.X, (int)closest.Y);
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
            dx = pt.X - closest.X;
            dy = pt.Y - closest.Y;

            pClosest = new Point((int)closest.X,(int) closest.Y);

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
