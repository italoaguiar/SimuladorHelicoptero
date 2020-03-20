using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho2CG
{
    public class Helicopter
    {
        TexturedObject helicopter, helice;

        private const float RUN_SPEED = 0.8f;
        private const float ROTATION_SPEED = 0.5f;

        private float currentSpeed = 0;

        public Helicopter()
        {
            helicopter = new TexturedObject("helicopter.obj", "Textures/fuse.png", new OpenTK.Vector3());
            helice = new TexturedObject("helice.obj", "Textures/fuse.png", new OpenTK.Vector3());
            //helice.Pivot = new Vector3(-0.034f, 0, 2.732f);
            helicopter.Pivot = new Vector3(-0.8f, 0, -9f);
            RotationAxix = new Vector3(0,0,-1.5f);
        }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 RotationAxix { get; set; }

        public BoundingBox Box { get; set; }

        public bool Collision { get; set; }
        public bool ColideTop { get; set; }

        float rotation = 0f;
        float altitude = 0f;
        public void Draw()
        {
            GL.PushMatrix();


            //Matrix4 t = Matrix4.CreateTranslation(nextPosition);
            //Matrix4 r = Matrix4.CreateRotationY((float)(Math.PI / 180) * Rotation.Y);
            //Matrix4 s = Matrix4.CreateScale(3, 3, 3);


            
            if(Game.DrawBoundingBox)
                Box.Draw();

            GL.Translate(Position.X, Position.Y, Position.Z);
            GL.Scale(3, 3, 3);
            GL.Rotate(180, 0, 1, 0);

            //Translação do Helicóptero para um ponto xyz
           


            //Rotação do Helicóptero

            GL.Translate(-RotationAxix.X, -RotationAxix.Y, -RotationAxix.Z);
            GL.Rotate(Rotation.X, 1, 0, 0);
            GL.Rotate(Rotation.Y, 0, 1, 0);
            GL.Rotate(Rotation.Z, 0, 0, 1);
            GL.Translate(RotationAxix.X, RotationAxix.Y, RotationAxix.Z);

            

            helicopter.Draw();

            
            //Rotação da Hélice
            GL.Translate(-0.034f, 0, 2.732f);
            GL.Rotate((rotation = (rotation + 15f)%360), 0, 1, 0); //evita overflow da rotation           
            GL.Translate(0.034f, 0, -2.732f);

            helice.Draw();



            GL.PopMatrix();
        }
        bool up, down, left, right, space;
        public void HandleInput()
        {
            Vector3 max = new Vector3(5f, 8f, 0f);
            Vector3 min = new Vector3(-5f, 0f, -10f);

            Box = new BoundingBox(min + Position, max + Position);

            HandleController();


            if (Position.Y > 0.1 && !Controller.IsConnected)
            { //bloquia a interação se o helicoptero estiver no chão

                if (up)
                    currentSpeed = RUN_SPEED;
                else if (down)
                    currentSpeed = -RUN_SPEED;
                else currentSpeed = 0;

                if (left)
                    Rotation = new Vector3(0, (Rotation.Y + ROTATION_SPEED), 0);
                else if (right)
                    Rotation = new Vector3(0, (Rotation.Y - ROTATION_SPEED), 0);

            }

            if (!Controller.IsConnected)
            {

                if (space && altitude < 1)
                    altitude += 0.01f;
                else if (!space && altitude > -1)
                    altitude -= 0.01f;
            }


            if (Position.Y < 0)//colisão com o chão
            {
                altitude = 0f;
                Position = new OpenTK.Vector3(Position.X, 0, Position.Z);
            }

            //colisão com o topo dos objetos
            if (ColideTop && altitude < 0) altitude = 0;

            //não permite que o helicóptero ande para frente se colidir com uma parede
            if (Collision && currentSpeed > 0) currentSpeed = 0;

            var h = Position.Y + (0.3f * altitude);

            if(h< 0)
            {
                h = 0;
            }

             Position = new Vector3(Position.X,h, Position.Z);

            Move();
        }

        private void Move()
        {
            float dx = currentSpeed * (float)Math.Sin((Math.PI / 180) * Rotation.Y);
            float dz = currentSpeed * (float)Math.Cos((Math.PI / 180) * Rotation.Y);

            Position = new Vector3(Position.X + dx, Position.Y, Position.Z + dz);
        }
        SharpDX.XInput.Controller Controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.One);
        private void HandleController()
        {
            if (Controller.IsConnected)
            {
                var s = Controller.GetState();
                float i = s.Gamepad.RightTrigger;
                altitude = (i / 127) - 1f; ;

                float x = s.Gamepad.LeftThumbX;
                float y = s.Gamepad.LeftThumbY;

                currentSpeed = RUN_SPEED * (y / 32768);

                Rotation = new Vector3(0, (Rotation.Y - x/42768), 0);

                left = s.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadLeft;
                right = s.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadRight;
                down = s.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadDown;
                up = s.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadUp;

                return;
            }

            KeyboardState state = Keyboard.GetState();

            up = state.IsKeyDown(Key.Up);
            left = state.IsKeyDown(Key.Left);
            right = state.IsKeyDown(Key.Right);
            down = state.IsKeyDown(Key.Down);
            space = state.IsKeyDown(Key.Space);
        }
    }
}
