using Meshomatic;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho2CG
{
    public class TexturedObject
    {
        MeshData m, b1;
        uint dataBuffer;
        uint indexBuffer;
        int tex;
        int vertOffset, normOffset, texcoordOffset;
        OpenTK.Vector3d up = new OpenTK.Vector3d(0.0, 1.0, 0.0);
        OpenTK.Vector3d viewDirection = new OpenTK.Vector3d(1.0, 1.0, 1.0);
        public OpenTK.Vector3 Position { get; set; }
        public OpenTK.Vector3 Size { get; set; }

        OpenTK.Vector3 _pivot;
        public OpenTK.Vector3 Pivot
        {
            get
            {
                return _pivot;
            }
            set
            {
                _pivot = value;

                OpenTK.Vector3 centre = Size / 2;
                centre = centre + Position + Pivot;
                Box = new BoundingBox(centre - Size / 2, centre + Size / 2);
            }
        }
        
        
        public MeshData Mesh
        {
            get { return m; }            
        }
        public BoundingBox Box { get; set; }

        public TexturedObject(string modelName, string textureName, OpenTK.Vector3 position)
        {
            m = new ObjLoader().LoadFile(modelName);
            LoadBuffers(m);
            tex = LoadTexture(textureName);
            this.Position = position;

            double width, lenght, height;
            m.Dimensions(out width, out lenght, out height);
            OpenTK.Vector3 obj = new OpenTK.Vector3((float)width, (float)lenght, (float)height);
            Size = obj;
            OpenTK.Vector3 centre = obj/2;
            centre = centre + position - Pivot ;

            Box = new BoundingBox(centre - obj/2, centre + obj/2);
        }

        

        void LoadBuffers(MeshData m)
        {
            float[] verts, norms, texcoords;
            uint[] indices;
            m.OpenGLArrays(out verts, out norms, out texcoords, out indices);
            GL.GenBuffers(1, out dataBuffer);
            GL.GenBuffers(1, out indexBuffer);

            // Set up data for VBO.
            // We're going to use one VBO for all geometry, and stick it in 
            // in (VVVVNNNNCCCC) order.  Non interleaved.
            int buffersize = (verts.Length + norms.Length + texcoords.Length);
            float[] bufferdata = new float[buffersize];
            vertOffset = 0;
            normOffset = verts.Length;
            texcoordOffset = (verts.Length + norms.Length);

            verts.CopyTo(bufferdata, vertOffset);
            norms.CopyTo(bufferdata, normOffset);
            texcoords.CopyTo(bufferdata, texcoordOffset);

            bool v = false;
            for (int i = texcoordOffset; i < bufferdata.Length; i++)
            {
                if (v)
                {
                    bufferdata[i] = 1 - bufferdata[i];
                    v = false;
                }
                else
                {
                    v = true;
                }
            }

            // Load geometry data
            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(buffersize * sizeof(float)), bufferdata,
                          BufferUsageHint.StaticDraw);

            // Load index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                          (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }
        public void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Position);
            GL.Color4(Color.White);
            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            // Normal buffer
            GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr)(normOffset * sizeof(float)));

            // TexCoord buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)(texcoordOffset * sizeof(float)));

            // Vertex buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr)(vertOffset * sizeof(float)));

            // Index array
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Triangles, m.Tris.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.PopMatrix();
            // Restore the state
            GL.PopClientAttrib();
        }
        public static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);


            return id;
        }
    }
}
