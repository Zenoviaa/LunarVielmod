using Microsoft.Xna.Framework;
using ParticleLibrary;
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
                for (int i = 0; i < 1; i++)
                {
                    float distance = 128;
                    float particleSpeed = 8;
                    Vector2 position = player.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                    Vector2 speed = (player.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<UnderworldParticle1>(), default(Color), 0.5f);
                    p.timeLeft = 20;
                }
            }
            else
            {
                player.lifeRegen += 3;
            }
        }
    }
}
