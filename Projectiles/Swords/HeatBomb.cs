using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    internal class HeatBomb : ModProjectile
    {
        public bool Dead;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Bomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 12;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.width = 20;
            Projectile.height = 35;
            Projectile.hostile = false;
            Projectile.friendly = true;
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override void AI()
        {

            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 160)
            {
                {
                    if (Main.rand.NextBool(2))
                    {
                        var entitySource = Projectile.GetSource_FromThis();
                        NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<HeatBombLighting>());
                    }
                }
                if (Projectile.ai[0] >= 50)
                {
                    {
                        if (Main.rand.NextBool(6))
                        {
                            var entitySource = Projectile.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<HeatBombRayLight>());
                        }
                    }
                }
            }
            else
            {
                if (!Dead)
                {
                    Projectile.timeLeft = 1;
                    Dead = true;
                }

            }



        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.75f * Main.essScale);

        }
    }
}

