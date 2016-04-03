using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SphereGen.Graphics;

namespace SphereGen
{
    /// <summary>
    /// A sphere of unit radius, contructed of triangles.
    /// </summary>
    class Sphere
    {
        /// <summary>
        /// List of the triangular faces that make up this sphere.
        /// </summary>
        private List<Triangle> faces;

        /// <summary>
        /// Vertex buffer containing the vertex data for drawing.
        /// </summary>
        private VertexBuffer vertexBuffer;

        /// <summary>
        /// Effect to use when drawing geometry.
        /// </summary>
        private BasicEffect effect = null;

        /// <summary>
        /// Whether the geometry needs to be rebuilt before drawing.
        /// </summary>
        private bool dirtyGeometry = false;


        public Sphere()
        {
            // Create an icosahedron as the base shape
            // Create the 12 vertices.
            // See https://en.wikipedia.org/wiki/Regular_icosahedron
            float phi = (float)((1.0 + Math.Sqrt(5)) / 2.0); // Golden ratio
            Console.WriteLine(phi);
            Vector3[] vertices = new Vector3[12];
            
            // The "top" (not actually at the top due to rotation)
            vertices[0] = new Vector3(0, phi, -1);
            // Points surrounding the top, anticlockwise
            vertices[1] = new Vector3(0, phi, 1);
            vertices[2] = new Vector3(-phi, 1, 0);
            vertices[3] = new Vector3(-1, 0, -phi);
            vertices[4] = new Vector3(1, 0, -phi);
            vertices[5] = new Vector3(phi, 1, 0);
            // The "bottom"
            vertices[6] = new Vector3(0, -phi, 1);
            // Points surrounding the bottom, anticlockwise
            vertices[7] = new Vector3(0, -phi, -1);
            vertices[8] = new Vector3(-phi, -1, 0);
            vertices[9] = new Vector3(-1, 0, phi);
            vertices[10] = new Vector3(1, 0, phi);
            vertices[11] = new Vector3(phi, -1, 0);

            // Normalise the vertex vectors onto the unit sphere.
            for (int i = 0; i < 12; ++i)
            {
                vertices[i].Normalize();
            }

            // Create the 20 triangular faces.
            faces = new List<Triangle>();
            // Faces around the top
            faces.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
            faces.Add(new Triangle(vertices[0], vertices[2], vertices[3]));
            faces.Add(new Triangle(vertices[0], vertices[3], vertices[4]));
            faces.Add(new Triangle(vertices[0], vertices[4], vertices[5]));
            faces.Add(new Triangle(vertices[0], vertices[5], vertices[1]));
            // Faces around the bottom
            faces.Add(new Triangle(vertices[6], vertices[7], vertices[8]));
            faces.Add(new Triangle(vertices[6], vertices[8], vertices[9]));
            faces.Add(new Triangle(vertices[6], vertices[9], vertices[10]));
            faces.Add(new Triangle(vertices[6], vertices[10], vertices[11]));
            faces.Add(new Triangle(vertices[6], vertices[11], vertices[7]));
            // Faces around the middle
            faces.Add(new Triangle(vertices[1], vertices[10], vertices[9]));
            faces.Add(new Triangle(vertices[1], vertices[9], vertices[2]));
            faces.Add(new Triangle(vertices[2], vertices[9], vertices[8]));
            faces.Add(new Triangle(vertices[2], vertices[8], vertices[3]));
            faces.Add(new Triangle(vertices[3], vertices[8], vertices[7]));
            faces.Add(new Triangle(vertices[3], vertices[7], vertices[4]));
            faces.Add(new Triangle(vertices[4], vertices[7], vertices[11]));
            faces.Add(new Triangle(vertices[4], vertices[11], vertices[5]));
            faces.Add(new Triangle(vertices[5], vertices[11], vertices[10]));
            faces.Add(new Triangle(vertices[5], vertices[10], vertices[1]));

            dirtyGeometry = true;
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            // If the geometry needs rebuilding, do so.
            if (dirtyGeometry)
            {
                RecontructVertices(graphicsDevice);
            }

            // Set up the effect.
            // Align the effect's transformation matrices with the camera.
            effect.View = camera.LookAtMatrix;
            effect.Projection = camera.ProjectionMatrix;

            // Set the light to be pointing out from the camera.
            effect.DirectionalLight0.Direction = camera.Target - camera.Position;

            // Draw the shape.
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, faces.Count);
            }
        }

        /// <summary>
        /// Refines the shape to be closer to a true sphere by subdividing the current faces.
        /// </summary>
        public void Refine()
        {
            // Create a container for the new faces of the sphere.
            List<Triangle> newFaces = new List<Triangle>();

            // For each current triangular face, replace it with 4 smaller triangles in a "triforce" pattern.
            foreach (Triangle face in faces)
            {
                // Get the midpoints of each edge. These will form the new extra vertices.
                // The midpoint is merely the average of the two corner vectors.
                Vector3 midpoint1 = new Vector3(
                    0.5f * (face.Vertex1.X + face.Vertex2.X),
                    0.5f * (face.Vertex1.Y + face.Vertex2.Y),
                    0.5f * (face.Vertex1.Z + face.Vertex2.Z)
                );

                Vector3 midpoint2 = new Vector3(
                    0.5f * (face.Vertex2.X + face.Vertex3.X),
                    0.5f * (face.Vertex2.Y + face.Vertex3.Y),
                    0.5f * (face.Vertex2.Z + face.Vertex3.Z)
                );

                Vector3 midpoint3 = new Vector3(
                    0.5f * (face.Vertex3.X + face.Vertex1.X),
                    0.5f * (face.Vertex3.Y + face.Vertex1.Y),
                    0.5f * (face.Vertex3.Z + face.Vertex1.Z)
                );

                // Normalise the midpoints so that they are on the surface of the unit sphere.
                midpoint1.Normalize();
                midpoint2.Normalize();
                midpoint3.Normalize();

                // Create the new triangles.
                newFaces.Add(new Triangle(face.Vertex1, midpoint1, midpoint3));
                newFaces.Add(new Triangle(midpoint1, midpoint2, midpoint3));
                newFaces.Add(new Triangle(midpoint1, face.Vertex2, midpoint2));
                newFaces.Add(new Triangle(midpoint3, midpoint2, face.Vertex3));
            }

            // Set the shape to use the new faces.
            faces = newFaces;

            // The geometry needs rebuilding to match the new face data, so set the rebuild flag.
            dirtyGeometry = true;
        }

        /// <summary>
        /// Reconstructs the vertex buffer from face data.
        /// </summary>
        private void RecontructVertices(GraphicsDevice graphicsDevice)
        {
            // Create the effect if it hasn't been created already.
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = true;
                effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
                effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f);
                effect.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.0f);
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorNormal), faces.Count * 3, BufferUsage.WriteOnly);

            // Create the vertices
            VertexPositionColorNormal[] vertices = new VertexPositionColorNormal[faces.Count * 3];
            int i = 0;
            foreach (Triangle face in faces)
            {
                Vector3 normal = face.GetNormal();
                vertices[i++] = new VertexPositionColorNormal(face.Vertex1, Color.Orange, normal);
                vertices[i++] = new VertexPositionColorNormal(face.Vertex2, Color.Orange, normal);
                vertices[i++] = new VertexPositionColorNormal(face.Vertex3, Color.Orange, normal);
            }
            vertexBuffer.SetData<VertexPositionColorNormal>(vertices);

            // Set the geometry rebuild flag back to false.
            dirtyGeometry = false;
        }
    }
}
