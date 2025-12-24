using Stellamod.Core.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Content.Dialogue
{
    public class ZuiComeQuickDialogue : BaseDialogue
    {
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
