using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.Balls
{
    internal class IronBall : BaseChainedBallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<IronBallProj>();
        }
    }

    internal class IronBallProj : BaseChainedBallProjectile
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
        }
    }
}
