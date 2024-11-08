using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ThunderSlapWarn : ModProjectile
    {
        public float BeamLength;
        public Vector2[] BeamPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            float targetBeamLength = 2400;
            BeamLength = targetBeamLength;
            if (Timer == 1)
            {
                //Sound Effect Goooo
                SoundStyle lightningSoundStyle = SoundID.DD2_LightningAuraZap;
                lightningSoundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);
            }

            //Setup lightning stuff
            //Should make it scale in/out
            float lightningProgress = Timer / 30f;
            float easedLightningProgress = Easing.SpikeOutCirc(lightningProgress);
            Lightning.WidthMultiplier = MathHelper.Lerp(0f, 0.5f, easedLightningProgress);
            if (Timer % 3 == 0)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i <= 8; i++)
                {
                    float maxProgress = MathHelper.Lerp(0f, 1f, Easing.OutExpo(Timer / 15f));
                    float progress = MathHelper.Lerp(0f, maxProgress, i / 8f);

                    Vector2 start = Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, progress);
                    beamPoints.Add(start);
                }
                BeamPoints = beamPoints.ToArray();
                Lightning.RandomPositions(BeamPoints);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            Lightning.SetBoltDefaults();
            Lightning.Draw(spriteBatch, BeamPoints, Projectile.oldRot);
            return false;
        }
    }
}
