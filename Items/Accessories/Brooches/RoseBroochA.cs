using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class RoseBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.buffType = ModContent.BuffType<RoseB>();
            Item.defense = 4;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RoseShield>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
                    ModContent.ProjectileType<RoseShield>(), 0, 1f, player.whoAmI);
            }
        }
    }
}