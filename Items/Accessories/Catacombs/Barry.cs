using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class BarryPlayer : ModPlayer
    {
        public bool hasBarry;
        public float regenTimer;
        public override void ResetEffects()
        {
            hasBarry = false;
        }

        public override void PostUpdateEquips()
        {
            regenTimer--;
            if (regenTimer > 0)
                return;

            if (hasBarry && Player.ownedProjectileCounts[ModContent.ProjectileType<BarrySpike>()] == 0)
            {
                float count = 9;
                for (float i = 0; i < count; i++)
                {
                    float progress = i / count;
                    float rot = progress * 360;
                    Projectile barryProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<BarrySpike>(), 40, 20, Player.whoAmI, ai0: rot);
                    BarrySpike barrySpikeProj = barryProj.ModProjectile as BarrySpike;
                }
            }
        }
    }

    internal class Barry : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BarryPlayer>().hasBarry = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }
}
