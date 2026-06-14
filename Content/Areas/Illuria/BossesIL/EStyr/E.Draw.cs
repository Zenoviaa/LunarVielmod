using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackStarTrail : ScarletProjectile,
        IDrawBlackStar
    {
        private NPC Parent => Main.npc[(int)Projectile.ai[1]];
        private bool Quick => Projectile.ai[2] == 1;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 220;
        }
        public override void AI()
        {
            base.AI();
            Projectile.Center = Parent.Center;
            if (Quick)
            {
                Projectile.extraUpdates = 3;
            }
        }
                
        private float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.SmoothStep(32, 24, completionRatio);
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            w *= outScale;
            return w;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, completionRatio);
        }
        public override bool PreDraw(ref Color lightColor)
        {
           //  DrawTrail();
            return base.PreDraw(ref lightColor);
        }


        private void DrawTrail()
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BeamTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 2;
            shader.Repeats = 1f;

            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, shader);

            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, shader);

        }
        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawTrail();
        }
    }
    public partial class E : IDrawOutlines,
        IDrawBlackStar
    {
        private const string Anim_Idle = "_Idle";
        private const string Anim_SwordHold = "_SwordHold";
        private const string Anim_HandOut = "_HandOut";
        private const string Anim_LookOver = "_LookOver";
        private const string Anim_Morph = "_Morph";
        private const string Anim_Swimming = "_Swimming";
        private const string Anim_BattleIdle = "_BattleIdle";
        private const string Anim_ForwardSlash = "_ForwardSlash";
        private const string Anim_BackSlash = "_BackSlash";
        private const string Anim_FoundYou = "_FoundYou";
        private const string Anim_Holding = "_Holding";
        private const string Anim_BigSlash = "_BigSlash";
        private const string Anim_Running = "_Running";

        private float _telegraphLineAlpha;
        private float _telegraphLineRot;
        private float _afterImageAlpha;
        private float _extraAfterImageAlpha;
        private Vector2 _drawScale = Vector2.One;
        private Color _outlineColor;
        private Color TargetOutlineColor;
        private Animator _animatorBackingField;
        private Animator Animator
        {
            get
            {
                if (_animatorBackingField == null)
                    SetupAnimator();
                return _animatorBackingField;
            }
        }

        private Rectangle[] _oldFrameBackingField;
        private Rectangle[] OldFrame
        {
            get
            {
                if (_oldFrameBackingField == null)
                    _oldFrameBackingField = new Rectangle[NPC.oldPos.Length];
                return _oldFrameBackingField;
            }
        }

        private string[] _oldTexture;
        private string[] OldTexture
        {
            get
            {
                if (_oldTexture == null)
                {
                    _oldTexture = new string[NPC.oldPos.Length];
                    for(int i = 0; i < _oldTexture.Length; i++)
                    {
                        _oldTexture[i] = Texture;
                    }
                }
                    
                return _oldTexture;
            }
        }

        private TailSimulation _tailSimulation;
        private TailSimulation TailSimulation
        {
            get
            {
                if (_tailSimulation == null)
                {
                    _tailSimulation = new TailSimulation(32, 190);
                }
                return _tailSimulation;
            }
        }

        private VerletChain _hairVerlet;
        private VerletChain HairSimulation
        {
            get
            {
                _hairVerlet ??= new VerletChain(NPC.Center, NPC.Center + Vector2.UnitY * 512, 4);
                return _hairVerlet;
            }
        }
        private Vector2[] _tendrilPoints;
        private Vector2[] TendrilPoints
        {
            get
            {
                if (_tendrilPoints == null)
                {
                    _tendrilPoints = new Vector2[HairSimulation.points.Length];
                }

                HairSimulation.FillArr(_tendrilPoints);
                return _tendrilPoints;
            }
        }


        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0, completionRatio);
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, completionRatio);
        }


        private void SetupAnimator()
        {
            _animatorBackingField = new Animator();
            Vector2 drawOrigin = new Vector2(187, 160);
            var idle = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_Idle, idle);

            var swordHold = new SpriteAnimation(0, 6, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_SwordHold, swordHold);

            var handOut = new SpriteAnimation(0, 6, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_HandOut, handOut);

            var lookOver = new SpriteAnimation(0, 4, isLooping: false, drawOrigin, frameSpeed: 0.05f);
            _animatorBackingField.AddAnimation(Anim_LookOver, lookOver);

            var morph = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_Morph, morph);

            var swimming = new SpriteAnimation(0, 8, isLooping: true, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_Swimming, swimming);

            var battle = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_BattleIdle, battle);

            var forwardSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin, frameSpeed: 0.5f);
            _animatorBackingField.AddAnimation(Anim_ForwardSlash, forwardSlash);

            var backSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin, frameSpeed: 0.5f);
            _animatorBackingField.AddAnimation(Anim_BackSlash, backSlash);

            var foundYou = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_FoundYou, foundYou);

            var holding = new SpriteAnimation(0, 1, isLooping: true, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_Holding, holding);

            var bigSlash = new SpriteAnimation(0, 6, isLooping: false, drawOrigin, frameSpeed: 0.5f);
            _animatorBackingField.AddAnimation(Anim_BigSlash, bigSlash);

            var running = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.35f);
            _animatorBackingField.AddAnimation(Anim_Running, running);

        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            Animator.Update();
            NPC.frame.Y = Animator.GetFrameY(frameHeight);
          
        }

        private Vector2 GetDrawOrigin()
        {
            var drawOrigin = Animator.GetDrawOrigin();
            if (drawOrigin.HasValue)
            {
                return (Vector2)drawOrigin.Value;
            }
        
            return NPC.frame.Size() / 2f;
        }

        #region Hair Drawing
        private Color GetHairColor(float completionRatio)
        {
            return Color.Lerp(Color.LightGray, Color.Transparent, completionRatio);
        }

        private float GetHairWidth(float completionRatio)
        {
            float width = MathHelper.SmoothStep(64, 64, completionRatio);
            return width;
        }

        private void DrawHair(GraphicsDevice graphicsDevice)
        {
            BasicLaserAlphaShader shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            TrailDrawer.Draw(Main.spriteBatch, TendrilPoints, GetHairColor, GetHairWidth, shader, offset: NPC.Center);
        }
        #endregion

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

           // PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
            DrawTelegraphLine(spriteBatch, screenPos);
            DrawAfterImages(spriteBatch, screenPos, Color.White);
            DrawSprite(spriteBatch, screenPos, Color.White);
            BlackStarRenderer.QueueBlackStarDraw(this);
            return false;
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D bloomLinTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLinTexture.Width / 2f, 0f);
            float rotation = _telegraphLineRot - MathHelper.PiOver2;
            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= _telegraphLineAlpha;
            Vector2 scale = Vector2.One;
            scale.Y *= 2;
            spriteBatch.Draw(bloomLinTexture, NPC.Center - screenPos, null, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0); ;
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            string texture = Texture + Animator.GetAnimation();

            Texture2D eTexture = ModContent.Request<Texture2D>(texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Vector2 drawOrigin = GetDrawOrigin();
            float rotation = NPC.rotation;
            Rectangle frame = NPC.frame;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (NPC.spriteDirection == -1)
                drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
            spriteBatch.Draw(eTexture, drawCenter, frame, drawColor, rotation, drawOrigin, _drawScale , spriteEffects, 0f);
        }
        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawOrigin = GetDrawOrigin();
            if (NPC.spriteDirection == -1)
                drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Texture2D eTexture = ModContent.Request<Texture2D>(OldTexture[i]).Value;
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * (0.3f + _extraAfterImageAlpha);
                oldDrawPos += NPC.Size / 2f;
          
                spriteBatch.Draw(eTexture, oldDrawPos, OldFrame[i], fadeColor, NPC.oldRot[i], drawOrigin, _drawScale, spriteEffects, 0f);
            }
        }


        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 6;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;

            DrawSprite(spriteBatch, screenPos + v, _outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _outlineColor);

            outlineOffset = 4;
            v = Vector2.UnitY * outlineOffset;
            h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, screenPos + v , Color.Black);
            DrawSprite(spriteBatch, screenPos - v , Color.Black);
            DrawSprite(spriteBatch, screenPos + h , Color.Black);
            DrawSprite(spriteBatch, screenPos - h , Color.Black);

            outlineOffset = 2;
            v = Vector2.UnitY * outlineOffset;
            h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, screenPos + v, _outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _outlineColor);


        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BeamTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 2;
            shader.Repeats = 1f;

            TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, shader, NPC.Size / 2f);

            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, shader, NPC.Size / 2f);
        }
    }
}
