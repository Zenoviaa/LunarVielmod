using System.Collections.Generic;

namespace Stellamod.Core.Rendering.RTs;

public class RenderTargetHandle
{
    public static readonly List<RenderTarget2D> InUseTargets = new List<RenderTarget2D>();
    public RenderTargetHandle(RenderTargetParameters parameters, RenderTarget2D target, RenderTargetPool pool)
    {
        Parameters = parameters;
        Pool = pool;
        Target = target;
        InUseTargets.Add(Target);
    }

    public readonly RenderTargetParameters Parameters;
    public readonly RenderTargetPool Pool;
    public RenderTarget2D Target;
    public int Width => Target.Width;
    public int Height => Target.Height;
    public Vector2 Size() => new Vector2(Width, Height);
    public bool hasReleased;

    /// <summary>
    /// Returns the render target back to the pool it came from
    /// </summary>
    public void Release()
    {
        if (hasReleased)
            return;
        hasReleased = true;
        Pool.Return(this);
        InUseTargets.Remove(Target);
        //the api would be this
        //RenderTargetHandle handle = RenderTargets.ScreenTarget;
        //using (new RenderTargetContext(handle))
        //{
        //..draw to target
        //}


    }

    public static implicit operator RenderTarget2D(RenderTargetHandle handle) => handle.Target;
}
