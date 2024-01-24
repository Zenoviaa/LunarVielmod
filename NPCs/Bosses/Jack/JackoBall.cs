using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Jack
{
    public class JackoBall : ModProjectile
    {
        public bool OptionallySomeCondition { get; private set; }
        public float ShakeVel;
        public float ShakeVelY;
        public Vector2 Center;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("JackoBall");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Projectile.width = 32;
            Projectile.height = 32;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Main.myPlayer == Projectile.owner)
            {
                if (Main.rand.NextBool(2))
                    target.AddBuff(BuffID.OnFire, 180);
            }
        }

        float offsetRandom;
        public override void AI()
        {
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            Projectile.ai[0]++;

            if(Projectile.ai[0] >= 30)
            {
                if (Projectile.ai[0] == 70)
                {
                    offsetRandom = Main.rand.Next(0, 50);
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                    }

                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(1, 0) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    for (int i = 0; i < 4; i++)
                    {
                        if(Main.myPlayer == Projectile.owner)
                        {
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f),
                                ModContent.ProjectileType<JackWarning2>(), 16, 0, Projectile.owner);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f),
                                ModContent.ProjectileType<JackWarning2>(), 16, 0, Projectile.owner);
                        }
                    }
                }

                if (Projectile.ai[0] <= 60)
                {
                    if (Projectile.ai[0] % 5 == 0  && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity.Y = Main.rand.NextFloat(-ShakeVelY, ShakeVelY);
                        Projectile.velocity.X = Main.rand.NextFloat(-ShakeVel, ShakeVel);
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity *= 0.98f;
                }
                else
                {
                    Projectile.velocity *= 0.90f;
                }
                ShakeVelY += 0.05f;
                ShakeVel += 0.05f;
                AIType = -1;
                alphaCounter += 0.03f;

            }
            else
            {
                Projectile.velocity.Y += 0.4f;
            }
            if(alphaCounter >= 5)
            {
                Projectile.Kill();
            }
            Player player;
            if ((player = VectorHelper.GetNearestPlayerDirect(Projectile.position, Alive: true)) != null)
            {
                if (Projectile.position.Y + 5 <= player.position.Y)
                {
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }
        }
        float alphaCounter = 0;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                Projectile.Kill();
            }
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = (height = 8);
            fallThrough = base.Projectile.position.Y <= base.Projectile.ai[1];
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.47f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.position, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
            }
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.position, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.position, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(base.Projectile.position, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(base.Projectile.position, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
            }
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            for (int i = 0; i < 4; i++)
            {
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<JackWarning>(), 16, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<JackWarning>(), 16, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<JackFire2>(), 16, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), ModContent.ProjectileType<JackFire2>(), 16, 0, Main.myPlayer);
            }
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 120f);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.position);
            var entitySource = Projectile.GetSource_FromThis();
        }

    }
}