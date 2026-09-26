using System;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

public readonly record struct RTContext : IDisposable
{
    private readonly RenderTargetHandle _handle;
    public RTContext(RenderTargetHandle handle)
    {
        RenderTargets.TargetCounts++;
        _handle = handle;
    }

    public RenderTarget2D Target => _handle.Target;
    public Vector2 Size() => Target.Size();
    public int Width => Target.Width;
    public int Height => Target.Height;
    public void Dispose()
    {

        _handle?.Release();
    }
    public static implicit operator Texture2D(RTContext context) => context._handle;
    public static implicit operator RenderTargetHandle(RTContext context) => context._handle;
}
