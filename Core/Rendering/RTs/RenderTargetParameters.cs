using System;
using Terraria;

namespace Stellamod.Core.Rendering.RTs;

file static class RTSizeHelper
{
    public static int ScreenWidth => Main.graphics.PreferredBackBufferWidth;
    public static int ScreenHeight => Main.graphics.PreferredBackBufferHeight;
}
public record struct RenderTargetParameters(int Width, int Height, bool MipMap, SurfaceFormat SurfaceFormat, DepthFormat DepthFormat, RenderTargetUsage Usage)
{
    public static RenderTargetParameters DefaultScreenTarget
    {
        get
        {
            return new RenderTargetParameters(RTSizeHelper.ScreenWidth, RTSizeHelper.ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, RenderTargetUsage.PlatformContents);
        }
    }

    public readonly static Func<RenderTargetParameters> DefaultScreenTargetCreationFunc = () => DefaultScreenTarget;


public static RenderTargetParameters FromRenderTarget(RenderTarget2D target)
    {
        return new RenderTargetParameters(target.Width, target.Height, target.LevelCount != 0, target.Format, target.DepthStencilFormat, target.RenderTargetUsage);
    }
    public static Func<RenderTargetParameters> DownsizedFunc(int downSamples)
    {
        var func = () =>
        {
            return DefaultScreenTarget with
            {
                Width = RTSizeHelper.ScreenWidth / downSamples,
                Height = RTSizeHelper.ScreenHeight / downSamples
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
