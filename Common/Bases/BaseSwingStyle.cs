using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Common.Bases
{
    public abstract class BaseSwingStyle
    {
        private bool _playSound;
        public BaseSwingStyle()
        {

        }

        public BaseSwingProjectile SwingProjectile { get; set; }
        public Projectile Projectile => SwingProjectile.Projectile;
        public Player Owner => SwingProjectile.Owner;
        public float swingTime;
        public SoundStyle? swingSound;
        public float swingSoundLerpValue;
        public Func<float, float> easingFunc;
        public abstract void AI();
        protected void PlaySwingSound(float smoothedLerpValue)
        {
            if (_playSound)
                return;
            if(smoothedLerpValue >= swingSoundLerpValue)
            {
                SoundEngine.PlaySound(swingSound, SwingProjectile.Projectile.position);
                _playSound = true;
            }
        }
    }
}
