using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingNodeTriggeringBeam : ScarletProjectile,
        IDrawPixelated
    {
        private ref float Timer => ref Projectile.ai[0];
        private int TargetNPCIndex => (int)Projectile.ai[1];
        private NPC Target => Main.npc[TargetNPCIndex];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle shootSound = AssetRegistry.Sounds.SteamPunking.DescendingRetinaBeam;
                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
            }

            float degreeToRotate = 15f;
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Target.Center, degreeToRotate);
        }

        public void DrawPixelated()
        {
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            Texture2D drawTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_56").Value;
            Vector2 drawOrigin = drawTexture.Size() / 2f;
            float numAfterImages = TrailCacheLength;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 centerPos = OldCenterPos[i] - Main.screenPosition;
                float f = i;
                float completionRatio = f / numAfterImages;

                Color drawColor = Color.Lerp(Color.White, Color.Red, completionRatio);
                drawColor.A = 0;
                drawColor *= MathHelper.Lerp(1f, 0f, completionRatio);
                drawColor *= 0.3f;
                float scale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                scale *= outScaleEase;
                scale *= 0.25f;
                spriteBatch.Draw(drawTexture, centerPos, null, drawColor, OldCenterRot[i], drawOrigin, scale, SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
        }
    }
}
