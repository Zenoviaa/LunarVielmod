using System;
using Terraria;

namespace Stellamod.Core.Rendering;

public class RenderTargetPrepper :
    IDisposable
{
    public RenderTargetPrepper(in RenderTargetParameters parameters)
    {
        Parameters = parameters;
        target = null!;
        lastAccessedTime = DateTime.Now;
    }

    public readonly RenderTargetParameters Parameters;
    public RenderTarget2D target;
    public DateTime lastAccessedTime;
    public bool wasInitialized;
    public bool wasDisposed;
    private void TryInitializePooledRenderTarget()
    {
        if (wasInitialized)
            return;

        Main.QueueMainThreadAction(() =>
        {
            target = new RenderTarget2D(Main.graphics.GraphicsDevice,
                Parameters.Width,
                Parameters.Height,
                Parameters.MipMap,
                Parameters.SurfaceFormat,
                Parameters.DepthFormat, 0,
                Parameters.Usage);
        });

        wasInitialized = true;
    }


    /// <summary>
    /// Returns whether this render target has not been accessed in a long time
    /// </summary>
    /// <returns></returns>
    public bool ImOld()
    {
        DateTime now = DateTime.Now;
        TimeSpan diff = now.Subtract(lastAccessedTime);
        if (diff.TotalSeconds > 5)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns whether the render target is so ancient it should probably just be disposed of.
    /// </summary>
    /// <returns></returns>
    public bool ImTooOld()
    {
        DateTime now = DateTime.Now;
        TimeSpan diff = now.Subtract(lastAccessedTime);
        if (diff.TotalSeconds > 10)
        {
            return true;
        }
        return false;
    }

    public static implicit operator RenderTarget2D(RenderTargetPrepper pooledRenderTarget)
    {
        pooledRenderTarget.lastAccessedTime = DateTime.Now;
        if (pooledRenderTarget.target == null)
        {
            if (!pooledRenderTarget.wasInitialized)
            {
                pooledRenderTarget.TryInitializePooledRenderTarget();
            }
            return RenderTargetRequestManager.UselessTarget;
        }

        return pooledRenderTarget.target;
    }

    public void Dispose()
    {
        if (wasDisposed)
            return;

        Main.QueueMainThreadAction(() =>
        {
            target?.Dispose();
            target = null!;
        });
        wasDisposed = true;
    }
}
