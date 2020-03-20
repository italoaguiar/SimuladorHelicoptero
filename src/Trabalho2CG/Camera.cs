using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho2CG
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        private float pitch = 20f;
        private float yaw = 0f;
        private float distanceFromHelicopter = 20f;
        private float angleAroundHelicopter = 0f;

        private Vector3 thirdPersonReference = new Vector3(0, 10, -35);

        private Helicopter helicopter;

        public Camera(Helicopter helicopter)
        {
            this.helicopter = helicopter;
        }

        public void Move()
        {
            //float hDistance = calculateHorizontalDistance();
            //float vDistance = calculateVerticalDistance();
            //calculateCameraPosition(hDistance, vDistance);

            Matrix4 rotationMatrix = Matrix4.CreateRotationY((float)(Math.PI / 180) * helicopter.Rotation.Y);
            Vector3 transformedReference = Vector3.Transform(thirdPersonReference, rotationMatrix);

            Position = helicopter.Position + transformedReference;
        }

        private void calculateCameraPosition(float hDistance, float vDistance)
        {
            float theta = helicopter.Rotation.Y + angleAroundHelicopter;
            float offsetX = hDistance * (float)Math.Sin((Math.PI / 180) * theta);
            float offsetZ = hDistance * (float)Math.Cos((Math.PI / 180) * theta);
            Position = new Vector3(helicopter.Position.X - offsetX, helicopter.Position.Y + vDistance, helicopter.Position.Z - offsetZ);
            yaw = helicopter.Rotation.Y + angleAroundHelicopter;
        }

        private float calculateHorizontalDistance()
        {
            return 25;
        }
        private float calculateVerticalDistance()
        {
            return 10;
        }
    }
}
