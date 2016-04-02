using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SphereGen.Graphics
{
    /// <summary>
    /// Vertex type with position, colour and normal vector. Filling in a gap in MonoGame's basic
    /// vertex types.
    /// </summary>
    struct VertexPositionColorNormal:IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }
        private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
        }
    }
}
