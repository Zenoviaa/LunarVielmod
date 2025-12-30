using Stellamod.Common.XixianFlaskSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class WoodyBuffPlayer : ModPlayer
    {
        public int stacks;
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!Player.HasBuff<WoodyInsourceBuff>())
            {
                stacks = 0;
            }
        }
    }

    public class WoodyInsourceBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            WoodyBuffPlayer woodyBuffPlayer = player.GetModPlayer<WoodyBuffPlayer>();
            player.statDefense += woodyBuffPlayer.stacks * 10;
        }
    }

    public class WoodyInsource : InsourceItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }

        public override int GetAddedTime()
        {
            return 60 * 15;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            WoodyBuffPlayer woodyBuffPlayer = flaskPlayer.Player.GetModPlayer<WoodyBuffPlayer>();
            woodyBuffPlayer.stacks++;
            flaskPlayer.Player.AddBuff(ModContent.BuffType<WoodyInsourceBuff>(), 900);
        }
    }
}
