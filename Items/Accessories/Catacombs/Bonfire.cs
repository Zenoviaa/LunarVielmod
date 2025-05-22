using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class BonfirePlayer : ModPlayer
    {
        public bool hasBonfire;
        public override void ResetEffects()
        {
            hasBonfire = false;
        }

        public override void PostUpdateEquips()
        {
            if (hasBonfire)
            {
                if(Player.ownedProjectileCounts[ModContent.ProjectileType<BonfireProj>()] == 0)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<BonfireProj>(), 1, 1, Player.whoAmI);
                }
            }
        }
    }

    internal class Bonfire : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BonfirePlayer>().hasBonfire = true;
            if(player.velocity == Vector2.Zero)
            {
                player.lifeRegen += 12;
            }
            else
            {
                player.lifeRegen += 3;
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<AlcadizScrap>());
        }
    }
}
