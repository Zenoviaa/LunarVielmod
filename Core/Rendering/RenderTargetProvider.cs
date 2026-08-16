using System;
using System.Reflection;
using Terraria;

namespace Stellamod.Core.Rendering;




public class RenderTargetProvider(Func<RenderTargetParameters> RenderTargetCreationFunc)
{
    private RenderTargetPrepper _rentedTarget = null!;
    public RenderTargetPrepper GetTarget()
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

    public Func<RenderTargetParameters> RenderTargetParametersCreationFunc => RenderTargetCreationFunc;
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
