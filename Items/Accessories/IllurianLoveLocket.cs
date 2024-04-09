using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class IllurianLoveLocket : ModItem
    {
        private float _starTimer;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            _starTimer--;
            if(_starTimer <= 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<IllurianLoveLocketStarProj>(), 150, 1, player.whoAmI);
                _starTimer = 10;
            }
        }
    }
}
