using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue;

public class VerliaFreeingDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CloseOnComplete = true;
    }

    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Verlia.Asset;
    }

    public override int GetLength()
    {
        return 10;
    }
}

public class VerliaHappenedDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Verlia.Asset;
    }
    public override int GetLength()
    {
        return 6;
    }
}

public class VerliaFamilyDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Verlia.Asset;
    }

    public override int GetLength()
    {
        return 6;
    }
}

public class VerliaWingsDialogue : BaseDialogue
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Verlia.Asset;
    }
    public override int GetLength()
    {
        return 8;
    }
}