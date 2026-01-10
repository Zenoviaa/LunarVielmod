using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Friendzied : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("10+ Defense and Golden trail oooo :0");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public int miracleLevel;
        public int miracleTimeLeft;
        public bool hasMiracleSet;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.50f;
            player.statDefense += 10;
            Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.SilverCoin, Vector2.Zero);
        }
    }
}