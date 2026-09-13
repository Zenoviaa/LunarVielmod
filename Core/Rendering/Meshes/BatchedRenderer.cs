using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering.Meshes;

/// <summary>
/// Allows for more control over rendering, for projectiles that are spammed this provides easy batching between projectiles for example
/// </summary>
/// <typeparam name="DrawData"></typeparam>
public abstract class BatchedRenderer<DrawData> : ModType where DrawData : struct
{
    protected readonly List<DrawData> _draws = new();
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected sealed override void Register()
    {
        ModTypeLookup<BatchedRenderer<DrawData>>.Register(this);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public virtual void PrepareForRendering(DrawData drawData)
    {
        _draws.Add(drawData);
    }
}
