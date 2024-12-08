using Stellamod.Common.ScorpionMountSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Scorpions.OreKingdom
{
    internal class OreKingdomScorpion : BaseScorpionItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 10;
            Item.knockBack = 4;
            Item.width = 20;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.mountType = ModContent.MountType<OreKingdomScorpionMount>();
            gunType = ModContent.ProjectileType<OreKingdomScorpionGun>();
        }

        public override int GetLeftHandedCount()
        {
            return 3;
        }

        public override int GetRightHandedCount()
        {
            return 1;
        }
    }
}
