using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva.Projectiles
{
    public class LeafBoomerang : ScarletProjectile
    {
        public SlashTrailer Trailer { get; set; }
        private ref float Timer => ref Projectile.ai[0];
        private ref float StartX => ref Projectile.ai[1];
        private ref float EndX => ref Projectile.ai[2];
        private Vector2 ShootVelocity;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                StartX = Projectile.velocity.X;
                EndX = -Projectile.velocity.X;
                Projectile.netUpdate = true;

                SoundStyle voice21 = AssetRegistry.Sounds.Minerva.MinervaSpin;
                voice21.PitchVariance = 0.5f;
                voice21.Pitch = 0.5f;
                SoundEngine.PlaySound(voice21, Projectile.position);
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
            }
            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
            }
            if(Timer % 5 == 0 && Timer < 60)
            {
                var p =LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                p.fadeToColor = Color.DarkGreen;
                p.shrink = true;
                p.color *= 0.8f;
                p.Scale *= 0.6f;
            }

            float interpolant = Timer / 120f;
            float ease = EasingFunction.InOutSine(interpolant);
            Projectile.velocity.X = MathHelper.Lerp(StartX, EndX, ease);
            Projectile.velocity.Y = 2;
            Projectile.rotation += 0.5f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 oldPos = OldCenterPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;

                float f = i;
                float interpolant = f / (float)TrailCacheLength;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant);
                fadeColor *= 0.05f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, OldCenterRot[i], drawOrigin, 1, SpriteEffects.None, 0f);
            }
            this.Outline(Color.Red, ref lightColor);
            this.DrawCentered(ref lightColor);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
