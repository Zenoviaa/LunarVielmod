using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles
{
    internal class JiitasBombString : ScarletProjectile
    {
        private SilhouetteShader _silhouetteShader;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Dropped => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 4;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            base.AI();
            Projectile.tileCollide = Dropped == 1;
            Timer++;
            if (Timer % 8 == 0)
            {
                Vector2 muzzlePosition = Projectile.Center;
                muzzlePosition += Main.rand.NextVector2Circular(8, 8);
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(muzzlePosition,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.025f, 0.035f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            if (Timer >= 120)
            {
                Dropped = 1;
            }

            if (Dropped == 1)
            {

                Projectile.velocity.Y += 0.15f;
                Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            }
            else
            {
                if (Projectile.velocity.Length() <= 5)
                    Projectile.velocity *= 1.03f;

                Player p = PlayerHelper.FindClosestPlayer(Projectile.Center, 10024);
                if (p != null)
                {
                    Vector2 velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, p.Center, degreesToRotate: 2);
                    Projectile.velocity = velocity;
                }

            }


        }

        private void DrawStrings(Color drawColor)
        {
            Asset<Texture2D> stringTextureAsset = ModContent.Request<Texture2D>(Texture + "_String");
            Vector2[] stringDrawPoints = new Vector2[]
            {
                Projectile.Center + new Vector2(-16, 8),
                Projectile.Center + new Vector2(16, 8),
                Projectile.Center + new Vector2(0, -8)
            };

            //Draw Strings
            //I LOVE STRINGS
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < stringDrawPoints.Length; i++)
            {
                Vector2 drawPoint = stringDrawPoints[i];
                drawPoint -= Main.screenPosition;
                drawPoint.Y -= ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                Vector2 drawOrigin = new Vector2(0, stringTextureAsset.Height());
                float drawRotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4));
                float drawAlpha = ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                spriteBatch.Draw(stringTextureAsset.Value, drawPoint, null, drawColor * drawAlpha, drawRotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            _silhouetteShader ??= new SilhouetteShader();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            if (Dropped == 0)
                DrawStrings(lightColor);

            //draw after image trail
            spriteBatch.Restart(effect: _silhouetteShader.Effect);
            for (int i = 0; i < OldCenterPos.Length; i++)
            {
                Vector2 centerPos = OldCenterPos[i];
                Vector2 drawPos = centerPos - Main.screenPosition;
                float interpolant = (float)i / (float)OldCenterPos.Length;
                Color drawColor = Color.Lerp(Color.Red, Color.Yellow, interpolant);
                drawColor *= MathHelper.SmoothStep(1.0f, 0f, interpolant);
                drawColor = drawColor.MultiplyRGB(lightColor);
                spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, layerDepth: 0);
            }

            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, layerDepth: 0);
            return false; ;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<JiitasBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
