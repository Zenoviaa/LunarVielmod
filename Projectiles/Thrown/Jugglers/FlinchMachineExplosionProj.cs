using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class FlinchMachineExplosionProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        private ref float Timer => ref Projectile.ai[0];
        private float LifeTime => 60;
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.light = 0.78f;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Skull");

            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutExpo(progress);
            float alphaProgress = Easing.SpikeInOutCirc(progress);
            Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(8, 8), null, new Color(255, 255, 255, 0) * alphaProgress, Projectile.rotation, new Vector2(24, 24), easedProgress * 3, SpriteEffects.None, 0f);
            return false;
        }
    }
}
