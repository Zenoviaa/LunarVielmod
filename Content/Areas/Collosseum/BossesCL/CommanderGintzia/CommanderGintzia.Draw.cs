using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Trails;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia
{
    public partial class CommanderGintzia : 
        IDrawOutlines
    {
        private Color _outlineColor;
        private Color TargetOutlineColor;
        private WindStorm _windStorm;
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            string texturePath = Texture;
            if (State == AIState.Slam || State == AIState.Land)
                texturePath += "_Slam";
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = _outlineColor;

            spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }

        private void DrawGustStorm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            List<Vector2> gustpos = new List<Vector2>();
            Vector2 start = NPC.Center - Vector2.UnitX * 128;
            Vector2 end = NPC.Center + Vector2.UnitX * 128;
            float numPoints = 80f;
            for (float f = 0; f < numPoints; f++)
            {
                float lerpValue = f / numPoints;
                Vector2 gustPoint = Vector2.Lerp(end, start, lerpValue);
                gustpos.Add(gustPoint);
            }

            Vector2[] arr = gustpos.ToArray();
            float[] rot = new float[arr.Length];
            TrailDrawer.Draw(Main.spriteBatch, arr, rot, StripColors, StripWidth, shader);
        }
        private Color StripColors(float progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return Color.Lerp(Color.Transparent, Color.LightGray, EasingFunction.QuadraticBump(progressOnStrip)) * 0.5f;
        }

        private float StripWidth(float progressOnStrip)
        {
            float baseWidth = 80;
            return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawGustStorm(spriteBatch, screenPos, drawColor);
            string texturePath = Texture;
            if (State == AIState.Slam || State == AIState.Land)
                texturePath += "_Slam";
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);

            if (TransitionColorProgress > 0)
            {
                spriteBatch.Restart(blendState: BlendState.Additive);
                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor * TransitionColorProgress, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
                }
                spriteBatch.RestartDefaults();
            }
            _windStorm?.Draw();
            return false;
        }

    }
}
