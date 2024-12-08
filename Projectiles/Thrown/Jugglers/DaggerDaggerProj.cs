using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class DaggerDaggerProj : BaseJugglerProjectile
    {
        private Vector2[] BungeeGumPos;
        private Vector2[] BungeeGumAuraPos;
        public PrimDrawer BungeeGumDrawer { get; private set; } = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            BungeeGumAuraPos = new Vector2[24];
        }

        public override void AI_Catch()
        {
            base.AI_Catch();
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

            if (Projectile.velocity.Y < 0)
            {
                Projectile.velocity.Y += 0.1f;
            }
            else
            {
                Projectile.velocity.Y += 0.02f;
            }


            if (Timer > 30)
            {
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Projectile.velocity.Y += 0.1f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
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
                    glowColor: Color.Red,
                    outerGlowColor: Color.DarkOrange);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 12f);
            for (int i = 0; i < 8; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<BloodDust>());
            }

            FXUtil.GlowCircleBoom(target.Center,
                 innerColor: Color.LightPink,
                 glowColor: Color.Red,
                 outerGlowColor: Color.DarkOrange, duration: 25, baseSize: 0.18f);

            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);

            switch (Main.rand.Next(2))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj3"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj4"), Projectile.position);
                    break;
            }
        }


        public float WidthFunctionAura(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return baseWidth * 0.2f;
        }

        public Color ColorFunctionAura(float completionRatio)
        {
            return Color.Red * VectorHelper.Osc(0.5f, 1f, 3) * 0.33f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BungeeGumDrawer ??= new PrimDrawer(WidthFunctionAura, ColorFunctionAura, GameShaders.Misc["VampKnives:BasicTrail"]);
            Vector2 textureSize = new Vector2(42, 46);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            BungeeGumDrawer.WidthFunc = WidthFunctionAura;
            BungeeGumDrawer.ColorFunc = ColorFunctionAura;
            BungeeGumDrawer.DrawPrims(BungeeGumAuraPos, textureSize * 0.5f - Main.screenPosition, 155);

            if (Timer == 0)
            {
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
