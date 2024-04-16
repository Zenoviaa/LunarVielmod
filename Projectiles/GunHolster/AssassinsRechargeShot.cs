using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using ParticleLibrary;
using Stellamod.Particles;

using System;

using Terraria.Audio;

namespace Stellamod.Projectiles.GunHolster
{
    internal class AssassinsRechargeShot : ModProjectile
    {
        bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
       
        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;

            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
                Moved = true;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * 1), (int)(15f * 1), (int)(25f * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * 1), (int)(15f * 1), (int)(25f * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * 1), (int)(15f * 1), (int)(25f * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);

        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
            }
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player Player = Main.player[Projectile.owner];
            for (int j = 0; j < 20; j++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 10, ParticleManager.NewInstance<MoonTrailParticle>(), Color.DarkRed, Main.rand.NextFloat(0.2f, 0.8f));
            }

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Suckler"));
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoomKaev>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);


            float Speed = Main.rand.Next(4, 7);
            float offsetRandom = Main.rand.Next(0, 50);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2048f, 32f);

            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;


            for (int i = 0; i < 2; i++)
            {
                Player.Heal(1);
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Main.myPlayer);
                Projectile.netUpdate = true;
            }

            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire3, 180);

            Player Player = Main.player[Projectile.owner];
            for (int j = 0; j < 20; j++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 10, ParticleManager.NewInstance<MoonTrailParticle>(), Color.DarkRed, Main.rand.NextFloat(0.2f, 0.8f));
            }

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Suckler"));
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoomKaev>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);


            float Speed = Main.rand.Next(4, 7);
            float offsetRandom = Main.rand.Next(0, 50);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2048f, 32f);

            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;


            for (int i = 0; i < 2; i++)
            {
                Player.Heal(5);
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Main.myPlayer);
                Projectile.netUpdate = true;
            }

            Projectile.Kill();
            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.Blue, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PlatinumCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            return false;
        }

    }
}
