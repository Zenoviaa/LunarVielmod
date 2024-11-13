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
    internal class ElectricField : ModProjectile
    {
        private float _lightningRotOffset;
        private Vector2[] _lightningZaps;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[12];
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
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
            }

            float explosionProgress = Timer / 60f;
            float easedExplosionProgress = Easing.OutExpo(explosionProgress);
            float widthOffset = MathHelper.Lerp(32, 48, easedExplosionProgress) * VectorHelper.Osc(0.85f, 1f, offset: Projectile.whoAmI);
            Lightning.WidthMultiplier = Easing.SpikeOutCirc(Timer / 360f);
            if (Timer % 3 == 0)
            {

                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f) + _lightningRotOffset;
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 32, speed: 3);
                    offset += rot.ToRotationVector2() * widthOffset;
                    _lightningZaps[i] = Projectile.Center + offset + Main.rand.NextVector2Circular(4, 4);
                }
                Lightning.RandomPositions(_lightningZaps);
            }

            if (Timer % 30 == 0)
            {
                //This will make a prettty cool effect
                _lightningRotOffset += Main.rand.NextFloat(0f, 3.14f);
                for (int i = 0; i < Lightning.Trails.Length; i++)
                {
                    var trail = Lightning.Trails[i];
                    trail.LightningRandomExpand = Main.rand.NextFloat(16, 24);
                    trail.LightningRandomOffsetRange = Main.rand.NextFloat(3, 5);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.SetBoltDefaults();
            Lightning.Draw(spriteBatch, _lightningZaps, null);
            return false;
        }
    }
}
