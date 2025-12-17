using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Animations;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public partial class E : IDrawOutlines
    {
        private const string Anim_Idle = "idle";
        private const string Anim_HeadTurn = "headturn";
        private const string Anim_HandOut = "handout";

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
        private void SetupAnimator()
        {
            _animatorBackingField = new Animator();
            var idle = new SpriteAnimation(0, 0, isLooping: true);
            _animatorBackingField.AddAnimation(Anim_Idle, idle);

            _animatorBackingField.AddAnimation(Anim_HeadTurn, idle);

            _animatorBackingField.AddAnimation(Anim_HandOut, idle);
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
            spriteBatch.Draw(eTexture, drawCenter, frame, drawColor, rotation, drawOrigin, _drawScale, spriteEffects, 0f);
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
