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

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia
{
    public partial class CommanderGintzia : 
        IDrawOutlines
    {
        private Color _outlineColor;
        private Color TargetOutlineColor;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

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
            return false;
        }

    }
}
