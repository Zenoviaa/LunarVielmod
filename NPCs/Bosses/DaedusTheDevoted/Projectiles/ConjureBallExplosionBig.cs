using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ConjureBallExplosionBig : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        private Vector2[] _lightningZaps;
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[12];
            Projectile.width = 512;
            Projectile.height = 512;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle explosionSound;
                if (Main.rand.NextBool(2))
                {
                    explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_StormSpike");
                    explosionSound.PitchVariance = 0.15f;
                }
                else
                {
                    explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_StormSpike2");
                    explosionSound.PitchVariance = 0.15f;
                }

                SoundEngine.PlaySound(explosionSound, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048, 500);
            }

            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = (float)i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = MathHelper.Lerp(32, 8, progress) * MathHelper.Lerp(2f, 0, Timer / 30f);
                trail.LightningRandomExpand = MathHelper.Lerp(64, 16, progress);
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Yellow, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Yellow, progress);
            }

            float explosionProgress = Timer / 60f;
            float easedExplosionProgress = Easing.OutCirc(explosionProgress);
            float widthOffset = MathHelper.Lerp(0, 384 * Main.rand.NextFloat(0.75f, 1), easedExplosionProgress);
            Lightning.WidthMultiplier = Easing.SpikeOutCirc(explosionProgress) * Main.rand.NextFloat(0, 16);
            if (Timer % 3 == 0)
            {

                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 32, speed: 3);
                    offset += rot.ToRotationVector2() * widthOffset;
                    _lightningZaps[i] = Projectile.Center + offset + Main.rand.NextVector2Circular(4, 4);
                }
                Lightning.RandomPositions(_lightningZaps);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, _lightningZaps, Projectile.oldRot);
            return false;
        }
    }
}
