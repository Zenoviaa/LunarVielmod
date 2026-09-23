using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;

namespace Stellamod.Content.Dialogue
{
    public class RysaGotAnythingDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Rysa.Asset;
        }
        public override int GetLength()
        {
            return 4;
        }
    }

    public class RysaLivingDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Rysa.Asset;
        }
        public override int GetLength()
        {
            return 4;
        }
    }
}
