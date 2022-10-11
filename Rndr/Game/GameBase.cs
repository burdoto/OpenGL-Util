using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Timers;
using Rndr.Model;
using Silk.NET.OpenGL;

namespace Rndr.Game
{
    public abstract class GameBase : Container, IDrawable
    {
        public int TickTime { get; init; } = 20;
        public bool Active { get; set; }
        public abstract Camera Camera { get; }

        public void Draw(Context ctx, Camera camera, double frameTime)
        {
            ctx.gl.Clear(16640U); // depth & color buffer
            foreach (var child in Children)
            foreach (var renders in (child as IGameObject)?.RenderObjects ?? new List<IRenderObject>())
                renders.Draw(ctx, camera, frameTime);
            ctx.gl.Flush();
        }

        public override bool Load(Context ctx)
        {
            
            if (!base.Load(ctx))
                throw new Exception("Could not load game");
            if (!Enable())
                throw new Exception("Could not enable game");
            Active = true;
            return base.Load(ctx);
        }
    }
}