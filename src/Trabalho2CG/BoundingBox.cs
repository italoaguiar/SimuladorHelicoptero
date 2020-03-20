using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho2CG
{
    public struct BoundingBox
    {
        public Vector3 Min;
        public Vector3 Max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }
        public bool Intersects(BoundingBox box)
        {
            bool result;
            Intersects(ref box, out result);
            return result;
        }
        public void Intersects(ref BoundingBox box, out bool result)
        {
            if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
            {
                if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }

                result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
                return;
            }

            result = false;
            return;
        }
        public bool IntersectsWithTop(BoundingBox box)
        {
            return  (this.Min.Y <= box.Max.Y && this.Max.Y > box.Max.Y) && 
                    ((this.Min.X <= box.Max.X && this.Min.X >= box.Min.X)|| (this.Max.X <= box.Max.X && this.Max.X >= box.Min.X)) &&
                    ((this.Min.Z >= box.Min.Z && this.Min.Z <= box.Max.Z)|| (this.Max.Z >= box.Min.Z && this.Max.Z <= box.Max.Z));
        }
        public bool Intersects(BoundingSphere sphere)
        {
            if (sphere.Center.X - Min.X > sphere.Radius
                && sphere.Center.Y - Min.Y > sphere.Radius
                && sphere.Center.Z - Min.Z > sphere.Radius
                && Max.X - sphere.Center.X > sphere.Radius
                && Max.Y - sphere.Center.Y > sphere.Radius
                && Max.Z - sphere.Center.Z > sphere.Radius)
                return true;

            double dmin = 0;

            if (sphere.Center.X - Min.X <= sphere.Radius)
                dmin += (sphere.Center.X - Min.X) * (sphere.Center.X - Min.X);
            else if (Max.X - sphere.Center.X <= sphere.Radius)
                dmin += (sphere.Center.X - Max.X) * (sphere.Center.X - Max.X);

            if (sphere.Center.Y - Min.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Min.Y) * (sphere.Center.Y - Min.Y);
            else if (Max.Y - sphere.Center.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Max.Y) * (sphere.Center.Y - Max.Y);

            if (sphere.Center.Z - Min.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Min.Z) * (sphere.Center.Z - Min.Z);
            else if (Max.Z - sphere.Center.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Max.Z) * (sphere.Center.Z - Max.Z);

            if (dmin <= sphere.Radius * sphere.Radius)
                return true;

            return false;
        }
        public void Draw()
        {
            GL.PointSize(4);           
            GL.Begin(PrimitiveType.Points);
            for (float i = Math.Min(Min.X, Max.X); i < Math.Max(Min.X, Max.X); i += 0.01f)
            {
                
                GL.Vertex3(i, Min.Y, Min.Z);
                GL.Vertex3(i, Max.Y, Min.Z);
                GL.Vertex3(i, Min.Y, Max.Z);
                GL.Vertex3(i, Max.Y, Max.Z);
            }

            for (float i = Math.Min(Min.Y, Max.Y); i < Math.Max(Min.Y, Max.Y); i += 0.01f)
            {
                GL.Vertex3(Min.X, i, Min.Z);
                GL.Vertex3(Max.X, i, Min.Z);
                GL.Vertex3(Min.X, i, Max.Z);
                GL.Vertex3(Max.X, i, Max.Z);
            }

            for (float i = Math.Min(Min.Z, Max.Z); i < Math.Max(Min.Z, Max.Z); i += 0.01f)
            {
                GL.Vertex3(Min.X, Min.Y, i);
                GL.Vertex3(Min.X, Max.Y, i);
                GL.Vertex3(Max.X, Min.Y, i);
                GL.Vertex3(Max.X, Max.Y, i);
            }
            GL.End();
        }
    }
}
