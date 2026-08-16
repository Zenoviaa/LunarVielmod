using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

[Autoload(Side = ModSide.Client)]
public class RenderTargetRequestManager : ModSystem
{
    private static readonly HashSet<RenderTargetPrepper> _activeTargets = new();
    public static RenderTarget2D UselessTarget=null!;
    public override void OnModLoad()
    {
        base.OnModLoad();
        Main.QueueMainThreadAction(() =>
        {
            UselessTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 1, 1);
        });
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        Main.QueueMainThreadAction(() =>
        {
            UselessTarget?.Dispose();
            UselessTarget = null;
        });
        Main.QueueMainThreadAction(() =>
        {
            foreach (var target in _activeTargets)
            {
                Main.QueueMainThreadAction(() =>
                {
                    target.Dispose();
                });
            }
            _activeTargets.Clear();
        });
    }


    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        IEnumerable<RenderTargetPrepper> oldTargets = _activeTargets.Where(x => x.ImTooOld());
        foreach (var target in oldTargets)
        {
            _activeTargets.Remove(target);
            Main.QueueMainThreadAction(() =>
            {
                target.Dispose();
            });
        }
    }

    public static RenderTargetPrepper Request(RenderTargetParameters parameters)
    {
        RenderTargetPrepper newPooledTarget = new RenderTargetPrepper(parameters);
        _activeTargets.Add(newPooledTarget);
        return newPooledTarget;
    }
}