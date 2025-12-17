using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Threading;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.RenderTargetSystem;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{

    [Autoload(Side = ModSide.Client)]
    public class SmearDrawManager : ModSystem
    {
        //Represents a single screen smear
        //What we're going to do is basically have it animate between the start and end positions
        //Then linger for a bit while shrinking
        private class SmearParticle
        {
            public Vector2 startPosition;
            public Vector2 endPosition;
            public float time;
            public float travelTime;
            public float lingerTime;
        }
        private ManagedRenderTarget _smearMaskRT;
        private List<SmearParticle> _particles;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _particles = new();
            _smearMaskRT = ManagedRenderTarget.New(GetScreenSize);
            On_Main.CheckMonoliths += RenderSmearRT;
            On_Main.DoDraw += DrawToScreen;
       
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderSmearRT;
            On_Main.DoDraw -= DrawToScreen;
        }

        private void DrawToScreen(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (Main.gameMenu)
                return;
            return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(_smearMaskRT, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            UpdateSmears();
        }

        private void UpdateSmears()
        {
            if(_particles.Count > 0)
            {
                for (int i = 0; i < _particles.Count; i++)
                {
                    var smear = _particles[i];
                    smear.time++;
                }
                _particles.RemoveAll(x => x.time >= x.travelTime + x.lingerTime);
            }

        }
        private void RenderSmearRT(On_Main.orig_CheckMonoliths orig)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(_smearMaskRT);
            graphicsDevice.Clear(Color.Black);
            if(_particles.Count > 0)
            {
                string path = this.GetType().DirectoryHere() + "/Smear";
                Texture2D smearTexture = ModContent.Request<Texture2D>(path).Value;
                Vector2 drawOrigin = new Vector2(0, smearTexture.Height / 2);

                DarkSmearWriteShader writeShader = DarkSmearWriteShader.Instance;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, 
                    Main.Rasterizer, writeShader.Effect);

                //Loop through all of our smears and draw them to the mask
                for (int i = 0; i < _particles.Count; i++)
                {
                    var smear = _particles[i];
                    Vector2 velocity = smear.endPosition - smear.startPosition;
                    Vector2 normalVelocity = velocity.SafeNormalize(Vector2.Zero);
                    float angle = MathF.Atan2(-velocity.Y, -velocity.X);

                    //Normalize the angle between 0-1
                    float normalAngle = angle / MathHelper.Pi * 0.5f + 0.5f;

                    Vector2 drawPosition = smear.startPosition - Main.screenPosition;


                    float lerp = smear.time / smear.travelTime;
                    float strength = MathHelper.SmoothStep(0f, 1f, lerp);
                    Color smearColor = new Color(strength, normalAngle, velocity.Y);
                    float rotation = velocity.ToRotation();
   
                    Vector2 scale = Vector2.One;

                    float distance = Vector2.Distance(smear.startPosition, smear.endPosition);
                    scale.X = distance / (float)smearTexture.Width;
                    scale.Y = MathHelper.SmoothStep(2f, 0f, lerp); ;
                    spriteBatch.Draw(smearTexture, drawPosition, null, smearColor, velocity.ToRotation(), drawOrigin, scale, SpriteEffects.None, 0);
                }
                spriteBatch.End();
                //Apply to the screne shader
                DarkSmear s = ScreenShader.GetInstance<DarkSmear>();
                s.maskTexture = _smearMaskRT;
                s.strength = 1000;
                s.alpha = 1;
            }
          
            graphicsDevice.SetRenderTarget(null);


            orig();
        }


        public void NewParticle(Vector2 position, Vector2 direction, float length, float time)
        {
            SmearParticle particle = new SmearParticle
            {
                startPosition = position,
                endPosition = position + direction * length,
                time = 0,
                travelTime = time,
                lingerTime = time / 2f
            };
            _particles.Add(particle);
        }
        private Point GetScreenSize()
        {
            return new Point(Main.screenTarget.Width, Main.screenTarget.Height);
        }
    }

    public interface IDrawBlackStar
    {
        void DrawBlackStar(SpriteBatch spriteBatch);
    }

    /// <summary>
    /// Manages particles for the black star texture
    /// </summary>
    public class BlackStarParticleManager
    {
        public struct BlackStarParticle
        {
            public Vector2 position;
            public float time;
        }


        public BlackStarParticleManager(int particleCount, float duration)
        {
            MaxParticleCount = particleCount;
            Particles = new BlackStarParticle[particleCount];
            Duration = duration;
        }

        public readonly BlackStarParticle[] Particles;
        public readonly int MaxParticleCount;
        public readonly float Duration;
        public float time;
        public void Update(Vector2 spawnBounds)
        {
            time++;
            FastParallel.For(0, MaxParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    ref BlackStarParticle particle = ref Particles[i];
                    particle.time = (time + i) % Duration;
                    if (particle.time == 1)
                    {
                        //Reinitialize the particle
                        Vector2 newPosition = new Vector2();
                        newPosition.X = Main.rand.NextFloat(0f, spawnBounds.X);
                        newPosition.Y = Main.rand.NextFloat(0f, spawnBounds.Y);
                        particle.position = newPosition;
                    }
                }
            });
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class BlackStarRenderer : ModSystem
    {
        private List<IDrawBlackStar> _blackStarDraws;
        private ManagedRenderTarget _maskTarget;
        private ManagedRenderTarget _blackStarTarget;
        private BlackStarParticleManager _particleManager;
        //Manage particles

        public override void OnModLoad()
        {
            base.OnModLoad();
            _blackStarDraws = new List<IDrawBlackStar>();
            _blackStarTarget = ManagedRenderTarget.New(GetScreenSize);
            _maskTarget = ManagedRenderTarget.New(GetScreenSize);
            _particleManager = new BlackStarParticleManager(200, 30);
            On_Main.CheckMonoliths += Render;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawBlackStarToScreen;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= Render;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawBlackStarToScreen;
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            _particleManager.Update(new Vector2(Main.screenWidth, Main.screenHeight));
        }

        private void Render(On_Main.orig_CheckMonoliths orig)
        {
            RenderBlackStarMask();
            RenderBlackStar();
            orig();
        }

        private void RenderBlackStarMask()
        {
            _blackStarDraws.Clear();
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is IDrawBlackStar draw)
                {
                    _blackStarDraws.Add(draw);
                }
            }

            if (_blackStarDraws.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_maskTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin();
                foreach (IDrawBlackStar draw in _blackStarDraws)
                {
                    draw.DrawBlackStar(spriteBatch);
                }
                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);
            }
        }

        private void RenderBlackStar()
        {
            if (_blackStarDraws.Count <= 0)
                return;
            if (InputHelper.KeyDown(Microsoft.Xna.Framework.Input.Keys.L))
            {
                _particleManager = new BlackStarParticleManager(200, 30);
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_blackStarTarget);
            graphicsDevice.Clear(Color.Transparent);

            Texture2D starTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_62").Value;
            Vector2 drawOrigin = starTexture.Size() / 2f;

            spriteBatch.Begin();
            for (int i = 0; i < _particleManager.MaxParticleCount; i++)
            {
                ref var particle = ref _particleManager.Particles[i];
                Color drawColor = Color.White;
                drawColor.A = 0;

                float ratio = particle.time / _particleManager.Duration;
                float ease = EasingFunction.QuadraticBump(ratio);
                drawColor *= ease;

                Vector2 scale = Vector2.One;
                scale *= 0.5f;
                scale *= ExtraMath.Osc(0f, 2f, offset: i);
                spriteBatch.Draw(starTexture, particle.position, null, drawColor, 0, drawOrigin, scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void DrawBlackStarToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (_blackStarDraws.Count <= 0)
                return;
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;


            Vector2 v = Vector2.UnitX * 2;
            Vector2 h = Vector2.UnitY * 2;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
   null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(_maskTarget, v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, -v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, -h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();

            //Setup the shader
            MaskCombineShader maskCombine = MaskCombineShader.Instance;
            maskCombine.MixTexture = _blackStarTarget;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
               maskCombine.Effect, Main.GameViewMatrix.TransformationMatrix);



            spriteBatch.Draw(_maskTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();

            /*
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
   maskCombine.Effect, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(_blackStarTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();*/
        }

        private Point GetScreenSize()
        {
            return new Point(Main.screenWidth, Main.screenHeight);
        }
    }
    public class EBuster : ScarletProjectile,
        IDrawBlackStar
    {
        private float _scale;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();

            TrailCacheLength = 8;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Projectile.velocity.Length() < 20)
            {
                Projectile.velocity *= 1.065f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer < 60)
            {
                Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
                if (player != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 0.5f);
                }
            }


            float inTime = 15;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _scale = MathHelper.Lerp(0f, 1f, ease);
        }

        private void DrawSprite(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;
            Vector2 drawScale = new Vector2(1f, MathHelper.Lerp(0.8f, 1f, _scale)) * _scale * 0.85f;
            spriteBatch.Draw(texture, drawCenter, null, Color.White, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;

                Vector2 drawCenter = OldCenterPos[i] - Main.screenPosition;
                float rotation = OldCenterRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.3f;
                Vector2 drawScale = new Vector2(1f, MathHelper.Lerp(0.8f, 1f, _scale)) * _scale * 0.85f;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);
            DrawSprite(spriteBatch);
        }
    }
    public partial class E
    {
        private Vector2 _forwardVector;
        private int ForwardSlashDamage => 20;
        private void ForwardSlashStartupMovement(float moveTime)
        {
            //Find a position to move to
            float startTime = moveTime;
            float completionRatio = Timer / startTime;
            float easeIn = EasingFunction.InOutExpo7(completionRatio);

            float distanceToBeAway = MathHelper.Lerp(300, 400, EasingFunction.InOutSine(completionRatio));
            Vector2 directionFromTarget = (NPC.Center - MyTarget.Center);
            directionFromTarget = directionFromTarget.SafeNormalize(Vector2.Zero);

            Vector2 positionToMoveTo = MyTarget.Center + directionFromTarget * distanceToBeAway;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 smoothVelocity = Vector2.Lerp(TargetVector, targetVelocity, easeIn);
            NPC.velocity = smoothVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
        }

        private void AI_ForwardSlashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            float startTime = 150;
            ForwardSlashStartupMovement(startTime);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startTime)
            {
                SwitchState(AIState.ForwardSlash);
            }
        }

        private void AI_ForwardSlashQuickStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }


            float lerp = _attackNumber / 10f;
            float ease = EasingFunction.InOutSine(lerp);
            float startTime = MathHelper.Lerp(50, 5, ease);
            ForwardSlashStartupMovement(startTime);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startTime)
            {
                SwitchState(AIState.ForwardSlash);
            }
        }

        private void Slash()
        {
            ShakeModSystem.Shake = 16;
            FXUtil.ShakeCamera(NPC.position, 1024, 4);


            SmearDrawManager smearDrawManager = ModContent.GetInstance<SmearDrawManager>();
            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 startPosition = NPC.Center - direction * 1200;
            smearDrawManager.NewParticle(startPosition, direction, 2400, 45);

            for (float i = 0; i < 3; i++)
            {
                var donutParticle = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -direction * MathHelper.Lerp(15, 1f, i / 3f));
                donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

            }
            var strike = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
            strike.xMult = 6;
            strike.rotOffset += MathHelper.PiOver2;
            if (MultiplayerHelper.IsHost)
            {
                Vector2 shootVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                shootVelocity *= 0.5f;
                int projType = ModContent.ProjectileType<EBuster>();
                Projectile.NewProjectile(SourceFromThis, NPC.Center, shootVelocity, projType, ForwardSlashDamage, 1, Main.myPlayer);
            }
            var strike2 = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
            strike2.xMult = 32;
            strike2.rotOffset += MathHelper.PiOver2;
            SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
            hurriSlash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hurriSlash, NPC.position);
        }

        private void AI_ForwardSlash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);

                SoundStyle newSlashSound = new SoundStyle("Stellamod/Assets/Sounds/SwordSlice");
                newSlashSound.PitchVariance = 0.2f;
                newSlashSound.Volume = 0.5f;
                SoundEngine.PlaySound(newSlashSound, NPC.position);
                Slash();
            }


            float forwardSlashTime = 5;



            float completionRatio = Timer / forwardSlashTime;
            float ease = EasingFunction.OutSine(completionRatio);

            float maxRadians = MathHelper.PiOver4;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 recoilStartVector = TargetVector;
            Vector2 recoilEndVector = recoilStartVector + forwardVector * 100;
          
            Vector2 recoilPosition = Vector2.Lerp(recoilStartVector, recoilEndVector, ease);
            Vector2 targetVelocity = recoilPosition - NPC.Center;
            NPC.velocity = targetVelocity;

            TargetOutlineColor = Color.Red;
            if (Timer >= forwardSlashTime)
            {


                SwitchState(AIState.ForwardSlash_RePosition);
            }
        }

        private void AI_ForwardSlashReposition()
        {
            Timer++;
            if(Timer == 1)
            {
                _forwardVector = (NPC.Center - MyTarget.Center);
                TargetVector = NPC.velocity;
            }

            float rotateTime = 15;
            float completionRatio = Timer / rotateTime;
            float maxRadians = MathHelper.PiOver4;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 targetPosition = MyTarget.Center + forwardVector;
            Vector2 targetVelocity = targetPosition - NPC.Center;

            float ease = EasingFunction.InOutSine(completionRatio);
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            if(Timer >= rotateTime)
            {
                SwitchState(AIState.ForwardSlash_End);
            }
        }
        private void AI_ForwardSlashEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 1)
            {
                _attackNumber++;
                if (_attackNumber >= 30)
                {
                    SwitchState(AIState.Idle);
                }
                else
                {
                    SwitchState(AIState.ForwardSlash_QuickStart);
                }
            }
        }
    }
}
