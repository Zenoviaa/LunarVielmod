using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering.Meshes;

//Renderers should be able to control when they render
//Probably implementing the hooks in onload/unload
public abstract class MeshRenderer<VertexType> : ModType
    where VertexType : struct, IVertexType
{
    protected readonly List<VertexType> _verticesToRenderer = new();
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected sealed override void Register()
    {
        ModTypeLookup<MeshRenderer<VertexType>>.Register(this);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public virtual void PrepareForRendering(IEnumerable<VertexType> vertices)
    {
        _verticesToRenderer.AddRange(vertices);
    }
}
