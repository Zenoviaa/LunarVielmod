using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Dialogue
{
    public class ZuiComeQuickDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Zui.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 4;
        }
    }

    public class ZuiWhoAreYouDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Zui.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 2;
        }
    }

    public class ZuiTalkingToYouDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Zui.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 3;
        }
    }

    public class EFoundYouDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.E.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 1;
        }
    }

    public class ZuiGetOuttaHereDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.Zui.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 2;
        }
    }
    public class EEndingDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            switch (lineNumber)
            {
                case 5:
                case 6:
                case 9:
                case 10:
                case 11:
                case 12:
                case 0:
                    return AssetReferences.Core.DialogueSystem.EreshDark.Asset;
                default:
                    return AssetReferences.Core.DialogueSystem.E.Asset;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 14;
        }
    }
    public class EFearDialogue : BaseDialogue
    {
        public override Asset<Texture2D> GetPortrait(int lineNumber)
        {
            return AssetReferences.Core.DialogueSystem.E.Asset;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }
        public override int GetLength()
        {
            return 7;
        }
    }
}
