using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;

namespace OpenGL_Util.Shape
{
    public class Cuboid : IDrawable
    {
        private Vertex[][] _vertices;
        private Vertex _pos;
        private Vertex _scale;

        public Cuboid(Vector3 position, Vector3 scale)
        {
            Position = position;
            Scale = scale / 2;
        }

        public Vector3 Position
        {
            get => _pos.Vector();
            set
            {
                _pos = value.Vertex();
            }
        }

        public Vector3 Scale { get => _scale.Vector(); set => _scale = value.Vertex(); }
        public Quaternion Rotation => Quaternion.Identity;

        public unsafe void Draw(OpenGL gl, Vector3 offset)
        {
            gl.Color(0.7, 0.7, 0.7);

            var position = _pos + offset.Vertex();
            var scale = _scale;
            float[] pa = position + scale;
            float[] pb = position - scale;
            fixed (float* ptA = pa)
            fixed (float* ptB = pb)
                _vertices = CalcAllVertices(GetSidesOrder(), ptA, ptB);
            for (var i = 0; i < _vertices.Length; i++)
            {
                var side = SideOrder[i];
                var quad = _vertices[i];
                var texture = GetTexture(side);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureName);
                gl.ActiveTexture(texture.TextureName);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
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

        private unsafe Vertex[][] CalcAllVertices(Block.Side[] sides, float* pa, float* pb)
        {
            List<Vertex[]> list = new List<Vertex[]>();
            for (var i = 0; i < sides.Length; i++)
                list.Add(CalcVertices(sides[i], pa, pb));
            return list.ToArray();
        }

        private unsafe Vertex[] CalcVertices(Block.Side side, float* pa, float* pb)
        {
            float* pax = pa + 0, pay = pa + 1, paz = pa + 2;
            float* pbx = pb + 0, pby = pb + 1, pbz = pb + 2;
            switch (side)
            {
                case Block.Side.TOP:
                    return new[]
                    {
                        new Vertex(*pax, *pay, *pbz),
                        new Vertex(*pbx, *pay, *pbz),
                        new Vertex(*pbx, *pay, *paz),
                        new Vertex(*pax, *pay, *paz)
                    };
                case Block.Side.BOTTOM:
                    return new[]
                    {
                        new Vertex(*pax, *pby, *paz),
                        new Vertex(*pbx, *pby, *paz),
                        new Vertex(*pbx, *pby, *pbz),
                        new Vertex(*pax, *pby, *pbz)
                    };
                case Block.Side.LEFT:
                    return new[]
                    {
                        new Vertex(*pbx, *pay, *paz),
                        new Vertex(*pbx, *pay, *pbz),
                        new Vertex(*pbx, *pby, *pbz),
                        new Vertex(*pbx, *pby, *paz)
                    };
                case Block.Side.RIGHT:
                    return new[]
                    {
                        new Vertex(*pax, *pay, *pbz),
                        new Vertex(*pax, *pay, *paz),
                        new Vertex(*pax, *pby, *paz),
                        new Vertex(*pax, *pby, *pbz)
                    };
                case Block.Side.FRONT:
                    return new[]
                    {
                        new Vertex(*pax, *pay, *paz),
                        new Vertex(*pbx, *pay, *paz),
                        new Vertex(*pbx, *pby, *paz),
                        new Vertex(*pax, *pby, *paz)
                    };
                case Block.Side.BACK:
                    return new[]
                    {
                        new Vertex(*pax, *pby, *pbz),
                        new Vertex(*pbx, *pby, *pbz),
                        new Vertex(*pbx, *pay, *pbz),
                        new Vertex(*pax, *pay, *pbz)
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, "Invalid Side");
            }
        }
    }
}
