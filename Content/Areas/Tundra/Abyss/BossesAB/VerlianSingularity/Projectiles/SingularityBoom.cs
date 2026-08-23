using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class SingularityBoom : ModProjectile
    {
        private float _scale = 2f;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);

                FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY * 2, 8, 8, 32);
                int count = 32;
                float degreesPer = 360 / (float)count;
                for (int k = 0; k < count; k++)
                {
                    float degrees = k * degreesPer;
                    Vector2 d = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                    Vector2 vel = d * 8;
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, vel.X * 0.5f, vel.Y * 0.5f);
                }
                for (float f = 0; f < 16; f++)
                {
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(90, 90);
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<TSmokeDust>(), velocity, newColor: Color.DarkBlue);
                }

                var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.2f);
                part.Scale *= 6;

                var part3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.15f);
                part3.Scale *= 4;

                var part2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple);
                part2.Scale *= 3;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb"), Projectile.position);

                for (float f = 0; f < 42; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(Projectile.Center, velocity);
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightCyan,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TrailRegistry.BeamTrail.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            var shader = RadialBlastShader.Instance;

            float prog = Timer / 30f;
            float interp = EasingFunction.OutExpo(prog);
            shader.Offset = Vector2.Lerp(Vector2.One * 0.25f, -Vector2.One * 0.25f, interp);
            shader.Tiling = Vector2.Lerp(Vector2.One * 4, Vector2.One * 32, interp);
            shader.InnerColor = Color.Lerp(Color.White, Color.Black, interp);
            shader.OuterColor = Color.Lerp(Color.Blue, Color.Black, EasingFunction.OutSine(prog));
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
