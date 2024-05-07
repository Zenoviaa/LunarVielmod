using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class BarryPlayer : ModPlayer
    {
        public bool hasBarry;
        public override void ResetEffects()
        {
            hasBarry = false;
        }

        public override void PostUpdateEquips()
        {
            if (hasBarry && Player.ownedProjectileCounts[ModContent.ProjectileType<BarrySpike>()] == 0)
            {
                float count = 9;
                for(float i = 0; i < count; i++)
                {
                    Projectile barryProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<BarrySpike>(), 90, 20, Player.whoAmI);
                    BarrySpike barrySpikeProj = barryProj.ModProjectile as BarrySpike;

                    float degreesBetween = 360 /  count;
                    barrySpikeProj.DegreesOffset = degreesBetween * i;
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
    }
}
