using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.UI.DialogueTowning;
using Stellamod.NPCs.Town;

namespace Stellamod.Common.QuestSystem.Quests
{
    public class TalkToZui : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ItemID.Wood, 10);

        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override void StartQuest(Player player)
        {
            base.StartQuest(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return ModContent.GetInstance<DialogueTowningUISystem>().WhosTalking == ModContent.NPCType<Zui>();
        }
    }
}
