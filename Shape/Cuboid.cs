using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [Flags]
        public enum Side
        {
            TOP = 0x1,
            BOTTOM = -0x1,

            LEFT = 0x2,
            RIGHT = -0x2,

            FRONT = 0x4,
            BACK = -0x4
        }

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
            //Debug.WriteLine("Drawing " + Color + " Cube at " + position);
            var scale = _scale;
            float[] pa = position + scale;
            float[] pb = position - scale;
            fixed (float* pap = pa)
            fixed (float* pbp = pb)
            {
                SideVertices(gl, Side.TOP, pap, pbp);
                SideVertices(gl, Side.BOTTOM, pap, pbp);

                SideVertices(gl, Side.LEFT, pap, pbp);
                SideVertices(gl, Side.RIGHT, pap, pbp);

                SideVertices(gl, Side.FRONT, pap, pbp);
                SideVertices(gl, Side.BACK, pap, pbp);
            }
        }

        private unsafe void SideVertices(OpenGL gl, Side side, float* pa, float* pb)
        {
            if (!NeedRenderSide(side))
                return;

            gl.Begin(BeginMode.Quads);
            float* pax = pa + 0, pay = pa + 1, paz = pa + 2;
            float* pbx = pb + 0, pby = pb + 1, pbz = pb + 2;
            switch (side)
            {
                case Side.TOP:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pax, *pay, *pbz); // Top Right Of The Quad (Top)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pbx, *pay, *pbz); // Top Left Of The Quad (Top)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pbx, *pay, *paz); // Bottom Left Of The Quad (Top)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pax, *pay, *paz); // Bottom Right Of The Quad (Top)
                    break;
                case Side.BOTTOM:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pax, *pby, *paz); // Top Right Of The Quad (Bottom)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pbx, *pby, *paz); // Top Left Of The Quad (Bottom)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pbx, *pby, *pbz); // Bottom Left Of The Quad (Bottom)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pax, *pby, *pbz); // Bottom Right Of The Quad (Bottom)
                    break;
                case Side.LEFT:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pbx, *pay, *paz); // Top Right Of The Quad (Left)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pbx, *pay, *pbz); // Top Left Of The Quad (Left)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pbx, *pby, *pbz); // Bottom Left Of The Quad (Left)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pbx, *pby, *paz); // Bottom Right Of The Quad (Left)
                    break;
                case Side.RIGHT:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pax, *pay, *pbz); // Top Right Of The Quad (Right)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pax, *pay, *paz); // Top Left Of The Quad (Right)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pax, *pby, *paz); // Bottom Left Of The Quad (Right)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pax, *pby, *pbz); // Bottom Right Of The Quad (Right)
                    break;
                case Side.FRONT:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pax, *pay, *paz); // Top Right Of The Quad (Front)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pbx, *pay, *paz); // Top Left Of The Quad (Front)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pbx, *pby, *paz); // Bottom Left Of The Quad (Front)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pax, *pby, *paz); // Bottom Right Of The Quad (Front)
                    break;
                case Side.BACK:
                    //gl.TexCoord(1, 1);
                    gl.Vertex(*pax, *pby, *pbz); // Bottom Left Of The Quad (Back)
                    //gl.TexCoord(0, 1);
                    gl.Vertex(*pbx, *pby, *pbz); // Bottom Right Of The Quad (Back)
                    //gl.TexCoord(0, 0);
                    gl.Vertex(*pbx, *pay, *pbz); // Top Right Of The Quad (Back)
                    //gl.TexCoord(1, 0);
                    gl.Vertex(*pax, *pay, *pbz); // Top Left Of The Quad (Back)
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, "Invalid Side");
            }
            gl.End();
        }

        private bool NeedRenderSide(Side side)
        {
            return true;
        }
    }
}
