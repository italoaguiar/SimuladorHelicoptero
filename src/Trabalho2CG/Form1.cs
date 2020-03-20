using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho2CG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SharpGL.OpenGL gl = this.openGLControl1.OpenGL;

            grass = new Texture();
            grass.Create(gl, "Textures/grass.bmp");

            
        }

        Texture grass;
        

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            SharpGL.OpenGL gl = this.openGLControl1.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.LookAt(0, 20, 30, 0, 0, 0, 0, 1, 0);

            //desenha o piso
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            grass.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-100.0f, 0.0f, -100.0f);
            gl.TexCoord(100.0f, 0.0f); gl.Vertex(100.0f, 0.0f, -100.0f);
            gl.TexCoord(100.0f, 100.0f); gl.Vertex(100.0f, 0.0f, 100.0f);
            gl.TexCoord(0.0f, 100.0f); gl.Vertex(-100.0f, 0.0f, 100.0f);
            gl.End();
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            foreach (Polygon polygon in polygons)
            {
                polygon.PushObjectSpace(gl);
                polygon.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                polygon.PopObjectSpace(gl);
            }

            gl.Flush();
        }
        //  A set of polygons to draw.
        List<Polygon> polygons = new List<Polygon>();

        //  The camera.
        SharpGL.SceneGraph.Cameras.PerspectiveCamera camera = new SharpGL.SceneGraph.Cameras.PerspectiveCamera();

        /// <summary>
        /// Handles the Click event of the importPolygonToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void importPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //  Show a file open dialog.
            OpenFileDialog openDialog = new OpenFileDialog();
            //openDialog.Filter = SerializationEngine.Instance.Filter;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                
                
                var file = FileFormatWavefront.FileFormatObj.Load(openDialog.FileName, false);

                file.Model.
                                
                Scene scene = SerializationEngine.Instance.LoadScene(openDialog.FileName);
                if (scene != null)
                {
                    foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                    {
                        //  Get the bounds of the polygon.
                        BoundingVolume boundingVolume = polygon.BoundingVolume;
                        float[] extent = new float[3];
                        polygon.BoundingVolume.GetBoundDimensions(out extent[0], out extent[1], out extent[2]);

                        //  Get the max extent.
                        float maxExtent = extent.Max();

                        //  Scale so that we are at most 10 units in size.
                        float scaleFactor = maxExtent > 10 ? 10.0f / maxExtent : 1;
                        polygon.Transformation.ScaleX = scaleFactor;
                        polygon.Transformation.ScaleY = scaleFactor;
                        polygon.Transformation.ScaleZ = scaleFactor;
                        polygon.Freeze(openGLControl1.OpenGL);
                        polygons.Add(polygon);
                    }
                }
            }
        }
    }

}
