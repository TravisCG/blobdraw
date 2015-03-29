using System;
using System.Collections.Generic;
using System.Text;

namespace blobdraw
{
    class Circle
    {
        private int x;
        private int y;
        private int r;

        public Circle(int x, int y, int r = 0)
        {
            this.x = x;
            this.y = y;
            this.r = r;
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public int getR()
        {
            return r;
        }

        public void setR(int newr)
        {
            r = newr;
        }

        public void update(int newx, int newy)
        {
            float nx = newx;
            float ny = newy;

            r = (int)Math.Sqrt((nx - x) * (nx - x) + (ny - y) * (ny - y));
        }

        public void setCenter(int newx, int newy)
        {
            x = newx;
            y = newy;
        }
    }
}
