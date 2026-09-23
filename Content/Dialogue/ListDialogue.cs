using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue
{
    public class ListUmDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.List.Asset;
        }
        public override int GetLength()
        {
            return 5;
        }
    }
    public class ListWhyHereDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.List.Asset;
        }
        public override int GetLength()
        {
            return 3;
        }
    }
    public class ListZuiDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.List.Asset;
        }
        public override int GetLength()
        {
            return 8;
        }
    }
    public class ListAloneDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.List.Asset;
        }
        public override int GetLength()
        {
            return 8;
        }
    }
}
