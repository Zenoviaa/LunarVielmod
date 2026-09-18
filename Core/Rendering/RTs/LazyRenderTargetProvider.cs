using System;
using System.Reflection;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;




public class LazyRenderTargetProvider(Func<RenderTargetParameters> RenderTargetCreationFunc)
{
    private LazyRenderTarget _persistentTarget;
    public RenderTarget2D GetTarget()
    {
        if(_persistentTarget == null)
        {
            _persistentTarget = RenderTargets.RequirePersistentTarget(RenderTargetCreationFunc);
        }
        return _persistentTarget.Target;
    }

    public int Width
    {
        get
        {
            if (_persistentTarget == null)
                return 1;
            return _persistentTarget.Width;
        }
    }
    public int Height
    {
        get
        {
            if (_persistentTarget == null)
                return 1;
            return _persistentTarget.Height;
        }
    }
    public Vector2 Size
    {
        get
        {
            return new Vector2(Width, Height);
        }
    }

    public static implicit operator RenderTarget2D(LazyRenderTargetProvider provider)
    {
        return provider.GetTarget();
    }
}
