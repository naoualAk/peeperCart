using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationCreator.Process
{
    public class ShadeDescriptor
    {
        public Vector height;
        public Vector width;

        public ShadeDescriptor()
        {
            height = new Vector();
            width = new Vector();
        }
    }

    public class Vector
    {
        public Point Pt1;
        public Point Pt2;
        public float length;
        public float X;
        public float Y;
        public float Z;

        public void Divide(float value)
        {
            length /= value;
            X /= value;
            Y /= value;
            Z /= value;
        }
    }
}
