using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class BerserkingInsourcePlayer : ModPlayer
    {
        public int stacks;
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!Player.HasBuff<BerserkingInsourceBuff>())
            {
                stacks = 0;
            }
            else
            {
                if (Main.rand.NextBool(60))
                {
                    FXUtil.GlowStretch(Player.Center, Main.rand.NextVector2Circular(1, 1));
                }
            }
        }
    }
    public class BerserkingInsourceBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            BerserkingInsourcePlayer berserkPlayer = player.GetModPlayer<BerserkingInsourcePlayer>();
            player.GetAttackSpeed(DamageClass.Generic) += 2 * berserkPlayer.stacks;
        }
    }

    public class BerserkingInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 20;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            Player player = flaskPlayer.Player;
            BerserkingInsourcePlayer berserkingPlayer = player.GetModPlayer<BerserkingInsourcePlayer>();
            berserkingPlayer.stacks++;
            player.AddBuff(ModContent.BuffType<BerserkingInsourceBuff>(), 300);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankBrooch>();
        }
    }
}
