using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue;

public class BulbtrifierHiDialogue : BaseDialogue
{
    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Bulbtrifier.Asset;
    }
    public override int GetLength()
    {
        return 3;
    }
}

public class BulbtrifierWhoDialogue : BaseDialogue
{
    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Bulbtrifier.Asset;
    }
    public override int GetLength()
    {
        return 4;
    }
}
public class BulbtrifierHowMuchDialogue : BaseDialogue
{
    public override Asset<Texture2D> GetPortrait(int lineNumber)
    {
        return AssetReferences.Core.DialogueSystem.Bulbtrifier.Asset;
    }
    public override int GetLength()
    {
        return 4;
    }
}
