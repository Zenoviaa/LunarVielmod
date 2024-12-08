using Stellamod.Buffs.Scorpion;
using Stellamod.Common.ScorpionMountSystem;
using Stellamod.Items.Weapons.Scorpions.OreKingdom;
using Stellamod.Items.Weapons.Scorpions.RoyalPalace;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Scorpions.OreKingdom
{
    internal class OreKingdomScorpionMount : BaseScorpionMount
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            MountData.buff = ModContent.BuffType<OreKingdomScorpionMountBuff>();
            scorpionItem = (BaseScorpionItem)(new Item(ModContent.ItemType<OreKingdomScorpion>()).ModItem);
        }
    }

    
        internal class OreKingdomScorpionMountBuff : ModBuff
        {
            public override void SetStaticDefaults()
            {
                Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
                Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            }

            public override void Update(Player player, ref int buffIndex)
            {
                player.mount.SetMount(ModContent.MountType<OreKingdomScorpionMount>(), player);
                player.buffTime[buffIndex] = 10; // reset buff time
            }
        }
    
}
