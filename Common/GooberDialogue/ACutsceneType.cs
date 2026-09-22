using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Common.GooberDialogue;

public abstract class ACutsceneType : ModType
{
    public int Type
    {
        get;
        set;
    }

    public readonly List<ACutsceneAction> Actions = new();
    public abstract void Create();

    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }
    protected override void Register()
    {
        ModTypeLookup<ACutsceneType>.Register(this);
    }
    public void Clear() => Actions.Clear();
    public void Add(ACutsceneAction cutsceneAction) => Actions.Add(cutsceneAction);
}

