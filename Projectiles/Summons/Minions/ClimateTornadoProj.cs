using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class ClimateTornadoProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        private float Scale = 0f;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation += 0.5f;

            float progress = Timer / 60f;
            float easedProgress = Easing.SpikeOutCirc(progress);
            Scale = MathHelper.Lerp(0f, 1f, easedProgress);

            Lighting.AddLight(Projectile.position, 1.5f, 0.7f, 2.5f);
            Lighting.Brightness(2, 2);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = new(100, 255, 255, 0);
            Asset<Texture2D> vortexTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/VoxTexture");
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(vortexTexture.Value, Projectile.Center - Main.screenPosition,
                          vortexTexture.Value.Bounds, drawColor, Projectile.rotation,
                          vortexTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
