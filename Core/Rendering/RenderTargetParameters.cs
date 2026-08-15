using System;
using Terraria;

namespace Stellamod.Core.Rendering;

public record struct RenderTargetParameters(int Width, int Height, bool MipMap, SurfaceFormat SurfaceFormat, DepthFormat DepthFormat, RenderTargetUsage Usage)
{
    public static RenderTargetParameters DefaultScreenTarget
    {
        get
        {
            return new RenderTargetParameters(Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, RenderTargetUsage.PlatformContents);
        }
    }

    public readonly static Func<RenderTargetParameters> DefaultScreenTargetCreationFunc = () => DefaultScreenTarget;
    public static Func<RenderTargetParameters> DownsizedFunc(int downSamples)
    {
        var func = () =>
        {
            return DefaultScreenTarget with
            {
                Width = Main.screenWidth / downSamples,
                Height = Main.screenHeight / downSamples
            };
        };
       return func;
    }
    public static Func<RenderTargetParameters> DownsizedFunc(Func<Point> getSize, int downSamples)
    {
        var func = () =>
        {
            Point size = getSize();
            return DefaultScreenTarget with
            {
                Width = size.X / downSamples,
                Height = size.Y / downSamples
            };
        };
        return func;
    }
    public bool Matches(RenderTarget2D renderTarget)
    {
        if (renderTarget.Width != Width)
            return false;
        if (renderTarget.Height != Height)
            return false;
        if (renderTarget.Format != SurfaceFormat)
            return false;
        if (renderTarget.DepthStencilFormat != DepthFormat)
            return false;
        if (renderTarget.RenderTargetUsage != Usage)
            return false;
        return true;
    }
}
