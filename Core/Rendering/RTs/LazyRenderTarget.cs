using System;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

/// <summary>
/// Provides a lazyly loaded render target that will not be reused, useful for things that don't render every frame or need to persist between frames
/// </summary>
public class LazyRenderTarget
{
    bool _needsTargets=true;
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
            if (_targetBackingField == null || _targetBackingField.IsDisposed || _targetBackingField.IsContentLost)
                return RenderTargets.FailsafeTarget;
            return _targetBackingField;
        }
    }

    public int Width => Target.Width;
    public int Height => Target.Height;
    public void ReleaseTarget()
    {
        _targetBackingField?.Dispose();
        _targetBackingField = null;
        _needsTargets = true;
    }

    public void CreateTarget()
    {
        if (!_needsTargets)
            return;
        _needsTargets = false;
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
