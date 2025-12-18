using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Animations;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public partial class E : IDrawOutlines
    {
        private const string Anim_Idle = "idle";
        private const string Anim_SwordHold = "swordhold";
        private const string Anim_HeadTurn = "headturn";
        private const string Anim_HandOut = "handout";
        private const string Anim_LookOver = "lookover";

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
        private void SetupAnimator()
        {
            _animatorBackingField = new Animator();
            Vector2 drawOrigin = new Vector2(60, 65);
            var idle = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_Idle, idle);

            var swordHold = new SpriteAnimation(1, 7, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_SwordHold, swordHold);

            var handOut = new SpriteAnimation(8, 14, isLooping: false, drawOrigin);
            _animatorBackingField.AddAnimation(Anim_HandOut, handOut);

            var lookOver = new SpriteAnimation(15, 19, isLooping: false, drawOrigin, frameSpeed: 0.05f);
            _animatorBackingField.AddAnimation(Anim_LookOver, lookOver);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            screenPos.Y += ExtraMath.Osc(-2f, 2f, speed: 16);
            DrawAfterImages(spriteBatch, screenPos, Color.White);
            DrawSprite(spriteBatch, screenPos, Color.White);

            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D eTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Vector2 drawOrigin = GetDrawOrigin();
            float rotation = NPC.rotation;
            Rectangle frame = NPC.frame;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (NPC.spriteDirection == -1)
                drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
            spriteBatch.Draw(eTexture, drawCenter, frame, drawColor, rotation, drawOrigin, _drawScale * 2f, spriteEffects, 0f);
        }
        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 oldDrawScale = _drawScale;
            _drawScale.Y *= ExtraMath.Osc(3f, 4);
            _drawScale.X *= 0.1f;
            float numAfterImages = 16;
            for (float f = 0; f < numAfterImages; f++)
            {
                float ratio = f / numAfterImages;
                float rot = ratio * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly * 4;
                Vector2 offset = rot.ToRotationVector2() * ExtraMath.Osc(54, 64, speed: 1);
                offset.Y *= 0.2f;
                DrawSprite(spriteBatch, screenPos + offset, drawColor * 0.2f * _afterImageAlpha);
            }
            _drawScale = oldDrawScale;
            Texture2D eTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = GetDrawOrigin();
            if (NPC.spriteDirection == -1)
                drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * (0.3f + _extraAfterImageAlpha);
                oldDrawPos += NPC.Size / 2f;
          
                spriteBatch.Draw(eTexture, oldDrawPos, OldFrame[i], fadeColor, NPC.oldRot[i], drawOrigin, _drawScale * 2f, spriteEffects, 0f);
            }


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
