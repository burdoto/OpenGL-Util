using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;

namespace OpenGL_Util.Shape
{
    public class Cuboid : IRenderObject
    {
        private Vertex[][] _vertices;
        private Vertex _pos;
        private Vertex _scale;

        public Cuboid(Vector3 position, Vector3 scale) : this(position, scale, Color.AliceBlue)
        {
        }

        public Cuboid(Vector3 position, Vector3 scale, Color color)
        {
            Position = position;
            Color = color;
            Scale = scale / 2;
        }

        public Vector3 Position
        {
            get => _pos.Vector();
            set => _pos = value.Vertex();
        }

        public Color Color { get; }

        public Vector3 Scale
        {
            get => _scale.Vector();
            set => _scale = value.Vertex();
        }

        public Quaternion Rotation => Quaternion.Identity;

        public unsafe void Draw(OpenGL gl, Vector3 offset)
        {
            gl.Color(Color);

            var position = _pos + offset.Vertex();
            var scale = _scale;
            float[] pa = position + scale;
            float[] pb = position - scale;
            fixed (float* ptA = pa)
            fixed (float* ptB = pb)
                foreach (Vertex[] quad in CalcVertices(ptA, ptB))
                {
                    gl.Begin(BeginMode.Quads);
                    gl.TexCoord(1, 1);
                    gl.Vertex(quad[0]);
                    gl.TexCoord(0, 1);
                    gl.Vertex(quad[1]);
                    gl.TexCoord(0, 0);
                    gl.Vertex(quad[2]);
                    gl.TexCoord(1, 0);
                    gl.Vertex(quad[3]);
                    gl.End();
                }
        }

        private unsafe Vertex[][] CalcVertices(float* pa, float* pb)
        {
            float* pax = pa + 0, pay = pa + 1, paz = pa + 2;
            float* pbx = pb + 0, pby = pb + 1, pbz = pb + 2;
            return new[]
            {
                new[]
                {
                    new Vertex(*pax, *pay, *pbz),
                    new Vertex(*pbx, *pay, *pbz),
                    new Vertex(*pbx, *pay, *paz),
                    new Vertex(*pax, *pay, *paz)
                },
                new[]
                {
                    new Vertex(*pax, *pby, *paz),
                    new Vertex(*pbx, *pby, *paz),
                    new Vertex(*pbx, *pby, *pbz),
                    new Vertex(*pax, *pby, *pbz)
                },
                new[]
                {
                    new Vertex(*pbx, *pay, *paz),
                    new Vertex(*pbx, *pay, *pbz),
                    new Vertex(*pbx, *pby, *pbz),
                    new Vertex(*pbx, *pby, *paz)
                },
                new[]
                {
                    new Vertex(*pax, *pay, *pbz),
                    new Vertex(*pax, *pay, *paz),
                    new Vertex(*pax, *pby, *paz),
                    new Vertex(*pax, *pby, *pbz)
                },
                new[]
                {
                    new Vertex(*pax, *pay, *paz),
                    new Vertex(*pbx, *pay, *paz),
                    new Vertex(*pbx, *pby, *paz),
                    new Vertex(*pax, *pby, *paz)
                },
                new[]
                {
                    new Vertex(*pax, *pby, *pbz),
                    new Vertex(*pbx, *pby, *pbz),
                    new Vertex(*pbx, *pay, *pbz),
                    new Vertex(*pax, *pay, *pbz)
                }
            };
        }
    }
}
