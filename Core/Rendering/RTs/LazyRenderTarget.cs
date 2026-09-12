using System;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

/// <summary>
/// Provides a lazyly loaded render target that will not be reused, useful for things that don't render every frame or need to persist between frames
/// </summary>
public class LazyRenderTarget
{
    private RenderTarget2D _targetBackingField;
    public LazyRenderTarget(Func<RenderTargetParameters> targetFactory)
    {
        TargetFactory = targetFactory;
    }
    public readonly Func<RenderTargetParameters> TargetFactory;
    public RenderTarget2D Target
    {
        get
        {
            if (_targetBackingField == null)
                return RenderTargets.FailsafeTarget;
            return _targetBackingField;
        }
    }

    public int Width => Target.Width;
    public int Height => Target.Height;
    public void FlushAndResize()
    {
        RenderTargetParameters parameters = TargetFactory();
        if (_targetBackingField == null || !parameters.Matches(Target))
        {
            _targetBackingField?.Dispose();
            var Parameters = TargetFactory();
            _targetBackingField = new RenderTarget2D(Main.graphics.GraphicsDevice,
                 Parameters.Width,
                 Parameters.Height,
                 Parameters.MipMap,
                 Parameters.SurfaceFormat,
                 Parameters.DepthFormat, 0,
                 Parameters.Usage);
        }
    }
}
