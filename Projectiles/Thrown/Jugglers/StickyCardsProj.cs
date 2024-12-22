using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class StickyCardsProj : BaseJugglerProjectile
    {
        private Vector2[] BungeeGumPos;
        private Vector2[] BungeeGumAuraPos;
        public PrimDrawer BungeeGumTrailDrawer { get; private set; } = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            BungeeGumPos = new Vector2[4];
            BungeeGumAuraPos = new Vector2[24];
            HomingStrength = 5;
        }

        public override void AI_Catch()
        {
            base.AI_Catch();
            if (Projectile.velocity.Y < 0)
            {
                Projectile.velocity.Y += 0.1f;
            }
            else
            {
                Projectile.velocity.Y += 0.02f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Vector2.Distance(Owner.Center, Projectile.Center) > 512)
            {
                Vector2 directionToOwner = Projectile.Center.DirectionTo(Owner.Center);
                Vector2 targetVelocity = directionToOwner * 16;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.3f);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }

            BungeeGumPos[0] = Projectile.position;
            BungeeGumPos[1] = Projectile.position;
            BungeeGumPos[2] = Owner.Center;
            BungeeGumPos[3] = Owner.Center;

            for (int i = 0; i < BungeeGumAuraPos.Length; i++)
            {
                float f = i;
                float length = BungeeGumAuraPos.Length;
                float progress = f / length;
                float offset = progress * MathHelper.TwoPi;
                Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + (Timer / 20f)).RotatedByRandom(MathHelper.PiOver4 / 24f);
                Vector2 rotatedVector = (rotatedOffset * 48 * VectorHelper.Osc(0.9f, 1f, 9));
                if (i % 2 == 0)
                {
                    BungeeGumAuraPos[i] = rotatedVector * 0.5f + Projectile.position;
                }
                else
                {
                    BungeeGumAuraPos[i] = rotatedVector + Projectile.position;
                }
            }

            //Don't take too long or else you lose your combo
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.0f * Main.essScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo <= 5)
                return;

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.LightPink,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.Purple);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            FXUtil.GlowCircleBoom(target.Center,
                 innerColor: Color.LightPink,
                 glowColor: Color.Pink,
                 outerGlowColor: Color.Purple, duration: 25, baseSize: 0.18f);
        }

        public override float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * VectorHelper.Osc(0.5f, 1f, 3);
        }

        public float WidthFunctionAura(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * 0.2f;
        }

        public override Color ColorFunction(float completionRatio)
        {
            return Color.Pink * VectorHelper.Osc(0.5f, 1f, 3) * 0.3f;
        }

        public Color ColorFunctionAura(float completionRatio)
        {
            return Color.Pink * VectorHelper.Osc(0.5f, 1f, 3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>(TextureRegistry.BoreParticleWhite).Value;
            Color drawColor = new Color(Color.Pink.R, Color.Pink.G, Color.Pink.B, 0);
            Color drawColor2 = new Color(Color.LightPink.R, Color.LightPink.G, Color.LightPink.B, 0);
            Color auraColor = Color.Lerp(drawColor, drawColor2, VectorHelper.Osc(0f, 1f, 3));
            auraColor *= 0.3f;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null,
                auraColor, Projectile.rotation,
                new Vector2(256, 256), 0.2f, SpriteEffects.None, 0f);


            BungeeGumTrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            Vector2 textureSize = new Vector2(16, 22);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StarTrail);
            BungeeGumTrailDrawer.WidthFunc = WidthFunction;
            BungeeGumTrailDrawer.ColorFunc = ColorFunction;
            BungeeGumTrailDrawer.DrawPrims(BungeeGumPos, textureSize * 0.5f - Main.screenPosition, 155);

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            BungeeGumTrailDrawer.WidthFunc = WidthFunctionAura;
            BungeeGumTrailDrawer.ColorFunc = ColorFunctionAura;
            BungeeGumTrailDrawer.DrawPrims(BungeeGumAuraPos, textureSize * 0.5f - Main.screenPosition, 155);

            if (Timer == 0)
            {
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            }

            return true;
        }
    }
}
