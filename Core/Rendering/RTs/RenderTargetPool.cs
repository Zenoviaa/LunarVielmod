using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

public class RenderTargetPool
{
    public RenderTargetPool(Func<RenderTargetParameters> targetFactory)
    {
        TargetFactory = targetFactory;
        TargetPool = new();
    }
    public readonly Func<RenderTargetParameters> TargetFactory;
    public readonly Queue<RenderTarget2D> TargetPool;
    public void OnLoad()
    {
        Main.QueueMainThreadAction(() =>
        {
            var Parameters = TargetFactory();
            var target = new RenderTarget2D(Main.graphics.GraphicsDevice,
                 Parameters.Width,
                 Parameters.Height,
                 Parameters.MipMap,
                 Parameters.SurfaceFormat,
                 Parameters.DepthFormat, 0,
                 Parameters.Usage);
            TargetPool.Enqueue(target);
        });
    }

    public void OnUnload()
    {
        Main.QueueMainThreadAction(() =>
        {
            while (TargetPool.Count > 0)
            {
                var target = TargetPool.Dequeue();
                target.Dispose();
            }
        });
    }

    /// <summary>
    /// Call this function at the end of a frame
    /// </summary>
    public void FlushAndResizeTargets()
    {
        //Check if all 
        if (TargetPool.Count <= 0)
            return;
        RenderTargetParameters parameters = TargetFactory();
        if(!parameters.Matches(TargetPool.Peek()))
        {
            int numToRecreate = TargetPool.Count;
            //Dispose and resize all
            while (TargetPool.Count > 0)
            {
                var target = TargetPool.Dequeue();
                target.Dispose();
            }

            for (int i = 0; i < numToRecreate; i++)
            {
                var Parameters = TargetFactory();
                var target = new RenderTarget2D(Main.graphics.GraphicsDevice,
                     Parameters.Width,
                     Parameters.Height,
                     Parameters.MipMap,
                     Parameters.SurfaceFormat,
                     Parameters.DepthFormat, 0,
                     Parameters.Usage);
                TargetPool.Enqueue(target);
            }
        }
    }

    public RenderTargetHandle Request()
    {
        if(TargetPool.Count <= 0)
        {
            //Expand pool
            //Main thread actions happen at the end of the frame btw
            Main.QueueMainThreadAction(() =>
            {
                var Parameters = TargetFactory();
                var target = new RenderTarget2D(Main.graphics.GraphicsDevice,
                     Parameters.Width,
                     Parameters.Height,
                     Parameters.MipMap,
                     Parameters.SurfaceFormat,
                     Parameters.DepthFormat, 0,
                     Parameters.Usage);
                TargetPool.Enqueue(target);
            });
            return new RenderTargetHandle(TargetFactory(), RenderTargets.FailsafeTarget, this);
        }
        var target = TargetPool.Dequeue();
        return new RenderTargetHandle(TargetFactory(), target, this);
    }

    public void Return(RenderTargetHandle handle)
    {
        //Don't add temp targets to the pool
        if (handle.Target == RenderTargets.FailsafeTarget)
            return;
        TargetPool.Enqueue(handle.Target);
    }

    public static implicit operator RenderTargetHandle(RenderTargetPool pool) => pool.Request();
}
