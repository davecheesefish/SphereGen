using Microsoft.Xna.Framework;

namespace SphereGen
{
    class Camera
    {
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                dirtyLookAtMatrix = true;
            }
        }
        private Vector3 position;

        public Vector3 Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                dirtyLookAtMatrix = true;
            }
        }
        private Vector3 target;

        public Matrix LookAtMatrix
        {
            get
            {
                if (dirtyLookAtMatrix)
                {
                    RecalculateLookAtMatrix();
                }
                return lookAtMatrix;
            }
        }
        private Matrix lookAtMatrix;
        private bool dirtyLookAtMatrix = true;

        public Matrix ProjectionMatrix { get { return projectionMatrix; } }
        private Matrix projectionMatrix;


        public Camera(Vector3 position, Vector3 target, float fieldOfView, float aspectRatio)
        {
            this.position = position;
            this.target = target;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, 0.001f, 5.0f);
        }

        public void RecalculateLookAtMatrix()
        {
            lookAtMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            dirtyLookAtMatrix = false;
        }
    }
}
