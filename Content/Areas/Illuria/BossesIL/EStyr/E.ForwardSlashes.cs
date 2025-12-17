using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Core;
using Stellamod.Core.RenderTargetSystem;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
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

            TrailCacheLength = 24;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Projectile.velocity.Length() < 20)
            {
                Projectile.velocity *= 1.1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if(Timer < 60)
            {
                Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
                if (player != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 1);
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
            Vector2 drawScale = new Vector2(1f, 1f) * _scale;
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
                Vector2 drawScale = new Vector2(1f, 1f) * _scale;
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

            float distanceToBeAway = 120;
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

            float startTime = 100;
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


            float startTime = MathHelper.Lerp(50, 20, _attackNumber / 10f);
            ForwardSlashStartupMovement(startTime);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startTime)
            {
                SwitchState(AIState.ForwardSlash);
            }
        }

        private void AI_ForwardSlash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
            }

            float forwardSlashTime = 20;
            float completionRatio = Timer / forwardSlashTime;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            Vector2 startVector = TargetVector;
            Vector2 endVector = TargetVector + _forwardVector * 64;
            Vector2 targetPosition = Vector2.Lerp(startVector, endVector, ease);
            Vector2 targetVelocity = targetPosition - NPC.Center;
            NPC.velocity = targetVelocity;

            TargetOutlineColor = Color.Red;
            if (Timer >= forwardSlashTime)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 shootVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    shootVelocity *= 2;
                    int projType = ModContent.ProjectileType<EBuster>();
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, shootVelocity, projType, ForwardSlashDamage, 1, Main.myPlayer);
                }
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
                if(_attackNumber >= 10)
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
