using Stellamod.Common.ScorpionMountSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Scorpions.RoyalPalace
{
    internal class RoyalPalaceScorpion : BaseScorpionItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 12;
            Item.knockBack = 4;
            Item.width = 20;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.mountType = ModContent.MountType<RoyalPalaceScorpionMount>();
            gunType = ModContent.ProjectileType<RoyalPalaceScorpionGun>();
        }

        public override int GetLeftHandedCount()
        {
            return 2;
        }

        public override int GetRightHandedCount()
        {
            return 2;
        }
    }
}
