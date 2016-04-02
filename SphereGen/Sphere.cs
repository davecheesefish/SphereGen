using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SphereGen.Graphics;

namespace SphereGen
{
    /// <summary>
    /// A sphere of unit radius, contructed of equal triangles.
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
            // Create a tetrahedron as the base shape
            // Create the 4 vertices.
            Vector3 vertex1 = new Vector3(-1, 1, -1);
            Vector3 vertex2 = new Vector3(1, 1, 1);
            Vector3 vertex3 = new Vector3(-1, -1, 1);
            Vector3 vertex4 = new Vector3(1, -1, -1);

            // Normalise the vertex vectors.
            vertex1.Normalize();
            vertex2.Normalize();
            vertex3.Normalize();
            vertex4.Normalize();

            // Create the triangular faces.
            faces = new List<Triangle>();
            faces.Add(new Triangle(vertex1, vertex3, vertex4));
            faces.Add(new Triangle(vertex1, vertex4, vertex2));
            faces.Add(new Triangle(vertex1, vertex2, vertex3));
            faces.Add(new Triangle(vertex2, vertex4, vertex3));

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
        /// Reconstructs the vertex buffer from face data.
        /// </summary>
        private void RecontructVertices(GraphicsDevice graphicsDevice)
        {
            // Create the effect if it hasn't been created already.
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.LightingEnabled = true;
                effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                effect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.3f, 0.0f);
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
