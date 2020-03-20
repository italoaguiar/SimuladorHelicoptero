﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho2CG
{
    public struct BoundingSphere
    {
        public Vector3 Center;
        public double Radius;

        public BoundingSphere(Vector3 center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public bool Intersects(BoundingBox box)
        {
            return box.Intersects(this);
        }

        public void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Center);
            Vertex[] SphereVertices = CalculateVertices2((float)Radius, (float)Radius, 20, 20);
            ushort[] SphereElements = CalculateElements((float)Radius, (float)Radius, 20, 20);


            GL.Begin(PrimitiveType.LineLoop);
            foreach (var element in SphereElements)
            {
                var vertex = SphereVertices[element];
                GL.TexCoord2(vertex.TexCoord);
                GL.Normal3(vertex.Normal);
                GL.Vertex3(vertex.Position);
            }

            GL.End();

            GL.PopMatrix();
        }
        private static Vertex[] CalculateVertices2(float radius, float height, byte segments, byte rings)
        {
            var data = new Vertex[segments * rings];

            int i = 0;

            for (double y = 0; y < rings; y++)
            {
                double phi = (y / (rings - 1)) * Math.PI; //was /2 
                for (double x = 0; x < segments; x++)
                {
                    double theta = (x / (segments - 1)) * 2 * Math.PI;

                    Vector3 v = new Vector3()
                    {
                        X = (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
                        Y = (float)(height * Math.Cos(phi)),
                        Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta)),
                    };
                    Vector3 n = Vector3.Normalize(v);
                    Vector2 uv = new Vector2()
                    {
                        X = (float)(x / (segments - 1)),
                        Y = (float)(y / (rings - 1))
                    };
                    // Using data[i++] causes i to be incremented multiple times in Mono 2.2 (bug #479506).
                    data[i] = new Vertex() { Position = v, Normal = n, TexCoord = uv };
                    i++;
                }

            }

            return data;
        }

        private static ushort[] CalculateElements(float radius, float height, byte segments, byte rings)
        {
            var num_vertices = segments * rings;
            var data = new ushort[num_vertices * 6];

            ushort i = 0;

            for (byte y = 0; y < rings - 1; y++)
            {
                for (byte x = 0; x < segments - 1; x++)
                {
                    data[i++] = (ushort)((y + 0) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x + 1);

                    data[i++] = (ushort)((y + 1) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x);
                }
            }

            // Verify that we don't access any vertices out of bounds:
            foreach (int index in data)
                if (index >= segments * rings)
                    throw new IndexOutOfRangeException();

            return data;
        }


        private struct Vertex
        { // mimic InterleavedArrayFormat.T2fN3fV3f
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Position;
        }
    }
}
