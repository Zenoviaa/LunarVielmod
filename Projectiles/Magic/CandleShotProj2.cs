using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Magic
{
    internal class CandleShotProj2 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.scale = 0;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private float counterAdd = 0.01f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }

        private float alphaCounter = 0;
        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 randOffset = Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 spawnPos = Projectile.Center + randOffset;
                    Vector2 velocity = spawnPos.DirectionTo(Projectile.Center) * 4;
                    Dust d = Dust.NewDustPerfect(spawnPos, DustID.Torch, velocity, Scale: 2);
                        d.noGravity = true;
                }
            }

            counterAdd *= 1.06f;
            if (Projectile.scale <= 1.3)
            {
                Projectile.scale += counterAdd;
            }

            Projectile.velocity *= 0.16f;
            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 2)
            {
                Projectile.scale = 0;
            }
            if (alphaCounter <= 5)
            {
                alphaCounter += counterAdd;
            }
            else
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<CandleShotBoom>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);

                }
                Projectile.active = false;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.08f;
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 0.78f * Main.essScale);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            for(int i = 0; i < 8; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            }
        }
    }
}


