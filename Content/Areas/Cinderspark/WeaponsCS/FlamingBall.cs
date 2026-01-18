using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class FlamingBall : BaseChainedBallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<FlamingBallProj>();
        }
    }

    public class FlamingBallProj : BaseChainedBallProjectile
    {
        private bool _hit;
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Just having this here in case
            //Iron Ball is just gonna use default stuff htough

            //Variables
            //Easing
            EasingFunction = (float lerpValue) => Easing.InOutExpo(lerpValue, 7);

            //How far it drags behind you
            DragDistance = 126;

            //Swing Range (IT USES OVAL SWING)
            SwingRange = MathHelper.ToRadians(360);

            //Offst for theoval swing
            OvalRotOffset = MathHelper.ToRadians(-90);

            //Max X Swing Radius
            SwingXRadius = 512;

            //Y Swing  Radius
            SwingYRadius = 80;

            //How long it takes to swing
            BaseSwingTime = 48;

            //Glowing stuff
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.005f;

            //Damage multiplier for hitting the tip
            TipDamageMultiplier = 2;
        }

        protected override void SetSlingDefaults()
        {
            base.SetSlingDefaults();

            //Reset the hit
            _hit = false;
        }


        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        break;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            if (!_hit)
            {
                SoundEngine.PlaySound(spearHit2, Projectile.position);
                FXUtil.ShakeCamera(target.Center, 1024, 2);
                _hit = true;
            }
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire, 120);
        }
    }
}
