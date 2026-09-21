using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering.RTs;

[Autoload(Side = ModSide.Client)]
public class RenderTargets : ModSystem
{
    /// <summary>
    /// A render target that matches the screen size
    /// </summary>
    public static readonly RenderTargetPool ScreenTarget = new RenderTargetPool(RenderTargetParameters.DefaultScreenTargetCreationFunc);


    /// <summary>
    /// A render target that matches the screen size with mipmaps
    /// </summary>
    public static readonly RenderTargetPool ScreenTargetMipMapped = new RenderTargetPool(() =>
    {
        var parameters = RenderTargetParameters.DefaultScreenTargetCreationFunc();
        parameters.MipMap = true;
        return parameters;
    });

    /// <summary>
    /// A render target that's pretty small
    /// </summary>
    public static readonly RenderTargetPool SpeechBubbleTarget = new RenderTargetPool(() =>
    {
        return RenderTargetParameters.DefaultScreenTarget with { Width = 256, Height = 256 };
    });

    /// <summary>
    /// A render target that matches half the screen size
    /// </summary>
    public static readonly RenderTargetPool HalfScreenTarget = new RenderTargetPool(RenderTargetParameters.DownsizedFunc(2));

    /// <summary>
    /// A render target that matches a fourth the screen size
    /// </summary>
    public static readonly RenderTargetPool QuarterScreenTarget = new RenderTargetPool(RenderTargetParameters.DownsizedFunc(4));

    /// <summary>
    /// A render target that matches a either the screen size
    /// </summary>
    public static readonly RenderTargetPool EigthScreenTarget = new RenderTargetPool(RenderTargetParameters.DownsizedFunc(8));

    /// <summary>
    /// A render target that matches the tile target size
    /// </summary>
    public static readonly RenderTargetPool TileTarget = new RenderTargetPool(() =>
    {
        return RenderTargetParameters.DefaultScreenTarget with { Width = Main.instance.tileTarget.Width, Height = Main.instance.tileTarget.Height };
    });

    public static readonly RenderTargetPool WaterTarget = new RenderTargetPool(() =>
    {
        return RenderTargetParameters.DefaultScreenTarget with { Width = Main.waterTarget.Width, Height = Main.waterTarget.Height };
    });
    public static readonly RenderTargetPool HalfWaterTargets = new RenderTargetPool(() =>
    {
        return RenderTargetParameters.DefaultScreenTarget with { Width = Main.waterTarget.Width / 2, Height = Main.waterTarget.Height / 2 };
    });

    /// <summary>
    /// All the available pools that can be chosen from
    /// </summary>
    public static readonly List<RenderTargetPool> TargetPools = new List<RenderTargetPool> { 
        ScreenTarget, 
        HalfScreenTarget, 
        ScreenTargetMipMapped, 
        QuarterScreenTarget, 
        TileTarget,
        EigthScreenTarget,
        WaterTarget,
        HalfWaterTargets,
        SpeechBubbleTarget
    };

    /// <summary>
    /// Targets that are not pooled and need to live forever,
    /// </summary>
    public static readonly List<LazyRenderTarget> PersistentTargets = new List<LazyRenderTarget>();

    /// <summary>
    /// This is returned if there's somehow no available targets to use while it waits for the pool to be expanded, to prevent an error, may cause a screen flicker if this happens in some cases
    /// </summary>
    public static RenderTarget2D FailsafeTarget;

    public static LazyRenderTarget RequirePersistentTarget(Func<RenderTargetParameters> factory)
    {
        LazyRenderTarget persistentTarget = new LazyRenderTarget(factory);
        Main.QueueMainThreadAction(() =>
        {
            persistentTarget.FlushAndResize();
        });
        PersistentTargets.Add(persistentTarget);
        return persistentTarget;
    }

    public override void Load()
    {
        base.Load();

        foreach(var pool in TargetPools)
        {
            pool.OnLoad();
        }

        On_Main.InitTargets_int_int += ResizeTargets;
        Main.QueueMainThreadAction(() =>
        {
            FailsafeTarget = new RenderTarget2D(Main.graphics.graphicsDevice, 1, 1);
        });
    }

    private void ResizeTargets(On_Main.orig_InitTargets_int_int orig, Main self, int width, int height)
    {
        orig(self, width, height);
        if (Main.dedServ)
            return;

        foreach (var targetPool in TargetPools)
        {
            targetPool.FlushAndResizeTargets();
        }

        foreach(var persistentTarget in PersistentTargets)
        {
            persistentTarget.FlushAndResize();
        }
    }

    public override void Unload()
    {
        base.Unload();
        foreach (var pool in TargetPools)
        {
            pool.OnUnload();
        }

        Main.QueueMainThreadAction(() =>
        {
            foreach (var target in PersistentTargets)
            {
                target.Target?.Dispose();
            }
            PersistentTargets.Clear();
        });

        Main.QueueMainThreadAction(() =>
        {
            FailsafeTarget?.Dispose();
        });
    }
}
