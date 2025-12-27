using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class PrimeMegaSaw : ModProjectile,
        IDrawOutlines
    {
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                Projectile.scale = 0.0001f;
                //Cool sound
                SoundStyle revUpSound = AssetRegistry.Sounds.SteamPunking.MechSawRevLoop;
                revUpSound.Pitch = 0.5f;
                SoundEngine.PlaySound(revUpSound, Projectile.position);
            }

            ShakeModSystem.Shake = 2;

            if (Timer % 5 == 0)
            {
                //Cool little sparks and dust
                var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red);
                Vector2 upVelocity = -Vector2.UnitY * 15;
                Main.dust[d].velocity += upVelocity;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red);
            }

            if (Timer % 10 == 0)
            {
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                LegacyParticle.NewParticle<ZapParticle>(spawnPoint, Main.rand.NextVector2CircularEdge(2, 2), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            if (Timer % 20 == 0)
            {
                Vector2 spawnPosition = Projectile.Top;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            //Changing the scale after set defaults will not affect the projectile size thankfully
            float inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));
            float outScale = MathHelper.Lerp(0f, 1f, EasingFunction.Anticipation((float)Projectile.timeLeft / 30f));
            float scale = inScale * outScale;
            Projectile.scale = scale;
            Projectile.Center = Parent.Bottom;
            Projectile.rotation += MathHelper.Lerp(0f, 0.05f, EasingFunction.Anticipation(Timer / 30f));
            DrawHelper.AnimateTopToBottom(Projectile, 2);
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D drawTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            spriteBatch.Draw(drawTexture, drawCenter, frame, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        private void DrawFlare(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D drawTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            drawColor.A = 0;
            drawColor *= ExtraMath.Osc(0f, 1f, speed: 32);
            spriteBatch.Draw(drawTexture, drawCenter, frame, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSprite(Main.spriteBatch, Main.screenPosition, Color.White.MultiplyRGB(lightColor));
            DrawFlare(Main.spriteBatch, Main.screenPosition, Color.Red);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            DrawSprite(spriteBatch, Main.screenPosition + h, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition - h, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition + v, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition - v, Color.Red);
        }
    }
}
