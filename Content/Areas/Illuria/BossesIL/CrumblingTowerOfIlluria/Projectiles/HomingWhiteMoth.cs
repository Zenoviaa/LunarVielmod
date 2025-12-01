using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class HomingWhiteMoth : ModProjectile,
        IDrawOutlines
    {
        private Color _outlineColor;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer < 60)
            {
                _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
                Projectile.hostile = false;
                Projectile.extraUpdates = 1;
            }
            else
            {
                _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
                Projectile.hostile = true;
                Projectile.extraUpdates = 0;
            }

                Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
            if(closest != null)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center);
                Projectile.velocity = homingVelocity;
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, Main.rand.NextVector2Circular(1, 1));
            }
            if(Projectile.timeLeft == 2)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.DarkBlue);
                float numDust = 12;
                for(float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), velocity, newColor: Color.Cyan, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }

            float inTime = 30f;
            float inRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(inRatio);
            float inScale = MathHelper.Lerp(0f, 1f, ease);
            Projectile.scale = inScale;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(8, 0, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            for(int i = 0;  i < Projectile.oldPos.Length; i++)
            {
                float f = i;
                float completionRatio = f / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f;
                Vector2 drawOrigin = frame.Size() / 2f;
                drawColor *= 0.1f;
                spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, drawColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            DrawSprite(spriteBatch, Main.screenPosition, lightColor);
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = Projectile.Frame();
            spriteBatch.Draw(texture, Projectile.Center - screenPos, frame, drawColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }


        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowingBallTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = glowingBallTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.White;
            glowColor.A = 0;
            glowColor *= 0.5f;
            glowColor *= ExtraMath.Osc(0f, 1f, speed: 8);
            spriteBatch.Draw(glowingBallTexture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, screenPos + v, _outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _outlineColor);
        }
    }
}
