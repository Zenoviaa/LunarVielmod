using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

[Autoload(Side = ModSide.Client)]
public class RenderTargetRequestManager : ModSystem
{
    private static readonly HashSet<PooledRenderTarget> _rentedTargets = new();
    private static readonly HashSet<PooledRenderTarget> _renderTargetPool = new();
    public static RenderTarget2D UselessTarget;
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
            foreach (var target in _rentedTargets)
            {
                Main.QueueMainThreadAction(() =>
                {
                    target.Dispose();
                });
            }
            foreach (var target in _renderTargetPool)
            {
                Main.QueueMainThreadAction(() =>
                {
                    target.Dispose();
                });
            }
            _rentedTargets.Clear();
            _renderTargetPool.Clear();
        });
    }


    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        IEnumerable<PooledRenderTarget> oldTargets = _rentedTargets.Where(x => x.ImOld());
        foreach (var target in oldTargets)
        {
            _rentedTargets.Remove(target);
            _renderTargetPool.Add(target);
            Mod.Logger.Info($"Return Rented Target");
        }

        IEnumerable<PooledRenderTarget> tooOldTargets = _renderTargetPool.Where(x => x.ImTooOld());
        foreach (var target in tooOldTargets)
        {
            Mod.Logger.Info($"Dispose Rented Target");
            _renderTargetPool.Remove(target);
            Main.QueueMainThreadAction(() =>
            {
                target.Dispose();
            });
        }
    }

    public static PooledRenderTarget Request(RenderTargetParameters parameters)
    {


        PooledRenderTarget pooledRenderTarget = null!;
        foreach (var target in _renderTargetPool)
        {
            if (parameters.Matches(target))
            {
                pooledRenderTarget = target;
                break;
            }
        }

        if (pooledRenderTarget != null)
        {
            _renderTargetPool.Remove(pooledRenderTarget);
            _rentedTargets.Add(pooledRenderTarget);
            return pooledRenderTarget;
        }
        Stellamod.Instance.Logger.Info($"New Rented Target");
        PooledRenderTarget newPooledTarget = new PooledRenderTarget(parameters);
        _rentedTargets.Add(newPooledTarget);
        return newPooledTarget;
    }
}