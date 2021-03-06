﻿using Microsoft.Xna.Framework;

namespace SphereGen
{
    /// <summary>
    /// Represents a triangular face of a 3D shape.
    /// </summary>
    struct Triangle
    {
        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;

        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;
        }

        public Vector3 GetNormal()
        {
            return new Vector3(
                (Vertex1.X + Vertex2.X + Vertex3.X) / 3.0f,
                (Vertex1.Y + Vertex2.Y + Vertex3.Y) / 3.0f,
                (Vertex1.Z + Vertex2.Z + Vertex3.Z) / 3.0f
            );
        }
    }
}
