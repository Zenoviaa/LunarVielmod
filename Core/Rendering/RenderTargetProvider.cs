using System;

namespace Stellamod.Core.Rendering;

public class RenderTargetProvider(Func<RenderTargetParameters> RenderTargetCreationFunc)
{
    private PooledRenderTarget _rentedTarget = null!;
    public PooledRenderTarget GetTarget()
    {
        var parameters = RenderTargetCreationFunc();
        if (_rentedTarget == null ||
            _rentedTarget.Parameters != parameters ||
            _rentedTarget.wasDisposed)
        {
            _rentedTarget = RenderTargetRequestManager.Request(parameters);
        }
        return _rentedTarget;
    }

    public int Width
    {
        get
        {
            if (_rentedTarget == null)
                return 1;
            return _rentedTarget.Parameters.Width;
        }
    }
    public int Height
    {
        get
        {
            if (_rentedTarget == null)
                return 1;
            return _rentedTarget.Parameters.Height;
        }
    }
    public Vector2 Size
    {
        get
        {
            return new Vector2(Width, Height);
        }
    }
    public static implicit operator RenderTarget2D(RenderTargetProvider provider)
    {
        return provider.GetTarget();
    }

}
