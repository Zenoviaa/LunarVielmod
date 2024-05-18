using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviLaserBlastProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public List<Vector2> BeamPoints;
        internal PrimitiveTrail BeamDrawer;

        ref float Size => ref Projectile.ai[0];
        ref float BeamLength => ref Projectile.ai[1];
        ref float Timer => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            BeamPoints = new List<Vector2>();
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast1, Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast2, Projectile.position);
                        break;
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(explosionCenter, 2048, 64);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero,
                    ModContent.ProjectileType<NiiviLaserBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < Main.rand.Next(12, 20); i++)
                {
                    float speed = 18;
                    Vector2 velocity = -Vector2.UnitY * speed;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 3f);
                    velocity *= Main.rand.NextFloat(0.5f, 1f);
                    int type = ModContent.ProjectileType<NiiviIcicleProj>();
                    int damage = Projectile.damage / 12;
                    float knockback = Projectile.knockBack / 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, velocity,
                        type, damage, knockback, Projectile.owner);
                }

                //This gonna be crazy
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.2f, 0.2f), blend: 0.15f, timer: 30);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * Size;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);

            float width = Projectile.timeLeft / 60f;
            float easedWidth = Easing.InExpo(width);
            return (Projectile.width * Projectile.scale) * osc * easedWidth * Size;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.LightCyan, Color.White, VectorHelper.Osc(0, 1));
            return color;
        }

        public override bool PreDraw(ref Color lightColor) => false;
        public override bool ShouldUpdatePosition() => false;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.White);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i <= 8; i++)
            {
                BeamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, i / 8f));
            }

            BeamDrawer.DrawPixelated(BeamPoints, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
