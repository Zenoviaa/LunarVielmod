using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private float _shiftAlpha;
        private float _telegraphLineAlpha;
        private float _telegraphLineRot;


        private float _afterImageAlpha;
        private Vector2 _scale;

        private Color _outlineColor;
        private Color TargetOutlineColor;
        private void UpdateDraw()
        {
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLineTexture.Width / 2f, 0f);
            Vector2 drawScale = Vector2.One;
            drawScale.Y *= 2f;
            drawScale.X *= 0.5f;

            Color telegraphLineColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            telegraphLineColor.A = 0;
            telegraphLineColor *= _telegraphLineAlpha;
            spriteBatch.Draw(bloomLineTexture, NPC.Center - screenPos, null, telegraphLineColor, _telegraphLineRot - MathHelper.PiOver2, drawOrigin, drawScale, SpriteEffects.None, 0);
        }


        private Texture2D GetTwinTexture()
        {
            if (_phaseShift)
                return GetTwinTextureCharged();
            if (Variant == TwinVariant.Spazz)
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture + "_Spazz").Value;
                return twinTexture;
            }
            else
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture).Value;
                return twinTexture;
            }
        }
        private Texture2D GetTwinTextureCharged()
        {
            if (Variant == TwinVariant.Spazz)
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture + "_SpazzCharged").Value;
                return twinTexture;
            }
            else
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture + "Charged").Value;
                return twinTexture;
            }
        }


        private Color GetFlamingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _afterImageAlpha;
        }

        private float GetFlamingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(222, 222, completionRatio);
        }


        private void DrawFlamingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BlackFireShader.Instance;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            shader.InnerColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            shader.OuterColor = Variant == TwinVariant.Spazz ? Color.DarkGreen : Color.DarkRed;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetFlamingTrailColor, GetFlamingTrailWidth, shader, offset: NPC.Size / 2f);
        }
        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float trailLength = NPC.oldPos.Length;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawCenter = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                float f = i;
                float completionRatio = f / trailLength;

                //After image
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= _afterImageAlpha;

                drawColor *= 0.5f;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                }
                spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.oldRot[i], drawOrigin, _scale, spriteEffects, 0f);
            }
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0f);
        }
        private void DrawSpriteV2(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = GetTwinTextureCharged();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            drawColor *= _shiftAlpha;
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAfterImages(spriteBatch, screenPos);
            DrawFlamingTrail(spriteBatch, screenPos, drawColor);
            DrawTelegraphLine(spriteBatch, screenPos);
            DrawSprite(spriteBatch, screenPos, drawColor);

            //This is just to create a nice little glowy effect
            drawColor *= ExtraMath.Osc(0f, 0.5f, speed: 3f);
            drawColor.A = 0;
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            DrawSprite(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
        }
    }
}
