using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BisinineMissile : ModProjectile,
        IDrawOutlines
    {
        private Player _target;
        private Vector2 _scale;
        private Vector2 _targetVelocity;
        private Vector2 _targetCenter;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
     
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_targetVelocity);
            writer.WriteVector2(_targetCenter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetVelocity = reader.ReadVector2();
            _targetCenter = reader.ReadVector2();
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle cometSummonSound = AssetRegistry.Sounds.Stars.Starsingle3;
                cometSummonSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(cometSummonSound, Projectile.position);
      
            }
            _scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InOutSine(Timer / 60f));
            if(Timer == 1)
            {
                _target = PlayerHelper.FindClosestPlayer(Projectile.position, 3000);   
            }

            if(Timer % 10 == 0)
            {
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 0.5f));
                d.velocity *= 0;
            }
            if(Main.rand.NextBool(6))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.GemDiamond, newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 0.5f));

            }
            if (_target == null || _target.dead)
                return;

            if(Timer < 240f)
            {
                Projectile.velocity.Y *= 0.99f;
                Projectile.velocity.X *= 0.999f;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
            } 
            else if (Timer >= 240f)
            {
                if(Timer == 240 && this.OwnedByLocalClient())
                {
                    //Yes there's no randomness here, but because of how players are I think it's safe to net update
                    _targetCenter = _target.Center;
                   
                    Projectile.netUpdate = true;
                }
                if(Timer < 300)
                    _targetVelocity = (_targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                float angle = _targetVelocity.ToRotation();
                float myAngle = Projectile.velocity.ToRotation();
                float newAngle = MathHelper.Lerp(myAngle, angle, 0.25f);
                Vector2 newVelocity = newAngle.ToRotationVector2();
                newVelocity *= Projectile.velocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, 0.1f);
                if(Projectile.velocity.Length() < 7f)
                    Projectile.velocity *= 1.02f;
                if(Timer >= 420)
                {
                    Projectile.tileCollide = true;
                }
            }
                Projectile.rotation = Projectile.velocity.ToRotation();
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Gray, Color.LightCyan, completionRatio) * EasingFunction.QuadraticBump(completionRatio) * 0.5f;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(30, 0, completionRatio) * _scale.X;
        }

        private void DrawTrail()
        {
            BlackFireShader shader = BlackFireShader.Instance;
            shader.InnerColor = Color.Gray;
            shader.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            string texturePath = TextureRegistry.ZuiEffect;
            Texture2D starTexture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Lerp(Color.Gray, Color.Lerp(Color.Gray, Color.Blue, 0.5f), ExtraMath.Osc(0f, 1f, speed: 12));
            drawColor *= 0.5f;
            drawColor.A = 0;
            Vector2 drawOrigin = starTexture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 scale = Vector2.One * 0.5f * ExtraMath.Osc(0.9f, 1f, speed: 64) * _scale;
            spriteBatch.Draw(starTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin,  scale, SpriteEffects.None, 0);

            starTexture = ModContent.Request<Texture2D>(Texture).Value;
            drawOrigin = starTexture.Size() / 2f;

            Color innerColor = Timer < 240 ? Color.Lerp(Color.Yellow, Color.Black, ExtraMath.Osc(0f, 1f, speed: 32)) : Color.White;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.GhostWhite, Color.Lerp(Color.Gray, Color.Blue, 0.8f) * 0.2f, interpolant) * 0.05f;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor,0, drawOrigin, scale * 1.5f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(starTexture, drawPosition, null, innerColor, 0, drawOrigin, scale * 1.5f, SpriteEffects.None, 0);


            starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            drawOrigin = starTexture.Size() / 2f;
            innerColor *= 0.125f;
            innerColor.A = 0;
            spriteBatch.Draw(starTexture, drawPosition, null, innerColor, 0, drawOrigin, scale * 2, SpriteEffects.None, 0);

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.Scale *= 0.5f;
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;

            for (float f = 0; f <16; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var d = Dust.NewDustPerfect(Projectile.Center,
           ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 1f), Velocity: vel);
      
            }
            SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.LightBlue,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 scale = Vector2.One * 0.5f * ExtraMath.Osc(0.9f, 1f, speed: 64) * _scale;
            float rot = Projectile.rotation;
            Projectile.rotation = 0;
            this.OutlineNoRestart(Color.Red, ref lightColor, scale  * 1.5f);
            Projectile.rotation = rot;
        }
    }
}
