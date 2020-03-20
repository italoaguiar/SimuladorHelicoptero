using Meshomatic;
using OpenTK;

using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Trabalho2CG
{
    
    public class Game: GameWindow
    {
        public Game()
            : base(1280,768, OpenTK.Graphics.GraphicsMode.Default)
        {
            VSync = VSyncMode.On;
            Location = new System.Drawing.Point(20, 20);
            //this.WindowBorder = WindowBorder.Hidden;
            this.WindowState = WindowState.Fullscreen;
        }

        public static bool DrawBoundingBox { get; set; }
        int texture_grass, bussola, Hud;
        TexturedObject skyBox;
        TexturedObject building, blocoC;
        Helicopter helicopter;
        Camera camera;
        Midi m = new Midi();
        Font serif = new Font(FontFamily.GenericSerif, 24);
        Font sans = new Font(FontFamily.GenericSansSerif, 24);
        Font mono = new Font(FontFamily.GenericMonospace, 24);
        TextRenderer textRederer;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 10000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);


            //helicopter = new TexturedObject("helicopter.obj", "Textures/fuse.png", new OpenTK.Vector3());
            building = new TexturedObject("Building-1.obj", "Textures/Building_1.bmp", new OpenTK.Vector3(10,0,20));
            building.Pivot = new OpenTK.Vector3(-building.Size.X/2,0,-building.Size.Z/2);

            skyBox = new TexturedObject("skybox.obj", "Textures/skybox_cubemap.png", new OpenTK.Vector3());
            blocoC = new TexturedObject("BlocoC.obj", "Textures/BlocoC.png", new OpenTK.Vector3(100,0,200));
            blocoC.Pivot = new OpenTK.Vector3(-blocoC.Size.X / 2,0, -blocoC.Size.Z / 2);
            texture_grass = TexturedObject.LoadTexture("Textures/grass.bmp");
            bussola = TexturedObject.LoadTexture("Textures/bussola.png");
            Hud = TexturedObject.LoadTexture("Textures/Hud.png");

            textRederer = new TextRenderer(Width, Height);

            helicopter = new Helicopter();
            camera = new Camera(helicopter);
            m.Instrument(0, 125);
            m.NoteOn(0, 60, 127);

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 10000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0f,0f,0f,1f);
            

            DefineIluminacao();


            this.Title = string.Format("Camera Position {0} Helcóptero Position: {1} Helicóptero Rotation: {2}", camera.Position, helicopter.Position, helicopter.Rotation);

            //lookat
            Matrix4 lookAt = Matrix4.LookAt(camera.Position, helicopter.Position, new OpenTK.Vector3(0,1,0));
            //Matrix4 lookAt = LookAt(camera.Position, helicopter.Position);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            


            GL.PushMatrix();
            GL.Scale(10, 10, 10);
            GL.Translate(0, -465, 0);
            
            skyBox.Draw();
            GL.PopMatrix();

            //desenha o piso
            GL.BindTexture(TextureTarget.Texture2D, texture_grass);
            GL.Begin(PrimitiveType.Quads);
            //GL.Color3(1.0f, 1.0f, 0.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-3000.0f, 0.0f, -3000.0f);
            GL.TexCoord2(700.0f, 0.0f); GL.Vertex3(3000.0f, 0.0f, -3000.0f);
            GL.TexCoord2(700.0f, 700.0f); GL.Vertex3(3000.0f, 0.0f, 3000.0f);
            GL.TexCoord2(0.0f, 700.0f); GL.Vertex3(-3000.0f, 0.0f, 3000.0f);
            GL.End();

            if(DrawBoundingBox)
                blocoC.Box.Draw();

            blocoC.Draw();


            GL.Disable(EnableCap.Light1);
            helicopter.Draw();
            GL.Enable(EnableCap.Light1);


            
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, 160, 0, 90, -5, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            //GL.Viewport(0, 0, 1, 1);

            GL.Translate(10, 10, 0);
            GL.Rotate(-helicopter.Rotation.Y + 180, 0, 0, 1);
            GL.Translate(-10, -10, 0);

            GL.PushMatrix();
            GL.Translate(10 + helicopter.Position.X / 100, 10 + helicopter.Position.Z / 100, 0);
            GL.PointSize(5);
            GL.Begin(PrimitiveType.Points);
            GL.Color4(Color.Red);
            GL.Vertex3(-building.Position.X/10, -building.Position.Z/10, 0);
            GL.End();
            GL.Translate(-10 - helicopter.Position.X / 100, -10 - helicopter.Position.Z / 100, 0);
            GL.PopMatrix();



            GL.BindTexture(TextureTarget.Texture2D, bussola);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.White);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(20.0f, 0.0f, 0.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(20.0f, 20.0f, 0.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 20.0f, 0.0f);
            GL.End();

            

            //GL.Translate(10, 10, 0);


            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();




            SwapBuffers();

        }        

        void DefineIluminacao()
        {
            float[] luzAmbiente = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] luzDifusa = { 1.0f, 1.0f, 1.0f, 1.0f }; // "cor"
            float[] luzEspecular = { 1.0f, 1.0f, 1.0f, 1.0f }; // "brilho"
            float[] posicaoLuz = { 0.0f, 50.0f, 50.0f, 1.0f };
            // Capacidade de brilho do material
            float[] especularidade = { 1.0f, 1.0f, 1.0f, 1.0f };
            int especMaterial = 80;
            // Define a refletância do material

            GL.Material(MaterialFace.Front, MaterialParameter.Specular, especularidade);
            // Define a concentração do brilho
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, especMaterial);
            // Ativa o uso da luz ambiente
            GL.LightModel(LightModelParameter.LightModelAmbient, luzAmbiente);
            // Define os parâmetros da luz de número 0
            GL.Light(LightName.Light0, LightParameter.Ambient, luzAmbiente);
            GL.Light(LightName.Light0, LightParameter.Diffuse, luzDifusa);
            GL.Light(LightName.Light0, LightParameter.Specular, luzEspecular);
            GL.Light(LightName.Light0, LightParameter.Position, posicaoLuz);


            GL.Light(LightName.Light1, LightParameter.Ambient, new Vector4(0.5f, 0.5f, 0.5f, 1f));
            GL.Light(LightName.Light1, LightParameter.Position, new float[] { 50f, 50f, -50f, 1f });

            GL.ShadeModel(ShadingModel.Smooth);
        }
        OpenTK.Input.KeyboardState oldState;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var state = OpenTK.Input.Keyboard.GetState();

            if (state.IsKeyDown(OpenTK.Input.Key.Escape))
                this.Close();

            helicopter.Collision = helicopter.Box.Intersects(blocoC.Box);

            helicopter.ColideTop = helicopter.Box.IntersectsWithTop(blocoC.Box);

            camera.Move();
            helicopter.HandleInput();


            if (oldState.IsKeyDown(OpenTK.Input.Key.B) && state.IsKeyUp(OpenTK.Input.Key.B))
                DrawBoundingBox = !DrawBoundingBox;


            //System.Diagnostics.Debug.WriteLine(helicopter.Collision);


            //if (helicopter.Position.Y < 40)
            //{


            //    //if (state.IsKeyDown(OpenTK.Input.Key.Space))
            //    //    helicopter.Position = new OpenTK.Vector3(helicopter.Position.X, helicopter.Position.Y + (0.3f * altitude), helicopter.Position.Z);
            //    //else if(state.IsKeyUp(OpenTK.Input.Key.Space) && helicopter.Position.Y > 0)
            //    //    helicopter.Position = new OpenTK.Vector3(helicopter.Position.X, helicopter.Position.Y - (0.1f * altitude), helicopter.Position.Z);
            //}

            //(float)CubicEaseInOut(altitude,0,30,30)

            oldState = state;

        }

    }
}
