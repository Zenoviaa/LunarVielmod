using System;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

public readonly record struct RenderTargetContext : IDisposable
{
    private readonly RenderTargetBinding[] _oldBindings;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly RenderTargetHandle _handle;
    public RenderTargetContext(RenderTargetHandle handle, Color? clearColor = null, GraphicsDevice graphicsDevice = null) : 
        this(new RenderTargetBinding[] { handle.Target }, clearColor, graphicsDevice)
    {
        _handle = handle;
    }

    public RenderTargetContext(RenderTargetBinding[] bindings, Color? clearColor = null, GraphicsDevice graphicsDevice = null)
    {
        graphicsDevice ??= Main.graphics.graphicsDevice;
        clearColor ??= Color.Transparent;
        _graphicsDevice = graphicsDevice;
        _oldBindings = graphicsDevice.GetRenderTargets();

        //Preserve the contents since for osme reason terraria uses discard contents
        //This prevents the need of a separate render target to draw to and means fewer draws and rt switches
        for(int i = 0; i < _oldBindings.Length; i++)
        {
            if (_oldBindings[i].RenderTarget is RenderTarget2D target)
            {
                target.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            }
        }
        graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        graphicsDevice.SetRenderTargets(bindings);
        graphicsDevice.Clear(clearColor.Value);
    }

    public void Dispose()
    {
        _graphicsDevice.SetRenderTargets(_oldBindings);
        _handle?.Release();
    }
}
