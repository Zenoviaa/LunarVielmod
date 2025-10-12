using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Helpers
{
    public static class SpritebatchHelpers
    {
        public static void Outline(this ModProjectile modProj, Color color, ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(modProj.Texture).Value;
            Vector2 drawPos = modProj.Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = SpriteEffects.None;
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Color outlineColor = color;
            Rectangle drawFrame = modProj.Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = modProj.Projectile.scale;
            float rotation = modProj.Projectile.rotation;
            spriteBatch.Restart(effect: whiteShader.Effect);


            spriteBatch.Draw(texture, drawPos + left, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);

            spriteBatch.RestartDefaults();
        }

        public static void DrawCentered(this ModProjectile modProj, ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(modProj.Texture).Value;
            Vector2 drawPos = modProj.Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = modProj.Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = modProj.Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = modProj.Projectile.scale;
            float rotation = modProj.Projectile.rotation;
            spriteBatch.Draw(texture, drawPos, drawFrame, Color.White.MultiplyRGB(lightColor), rotation, drawOrigin, scale, spriteEffects, 0);
        }
        public static void DrawCentered(this ModProjectile modProj, ref Color lightColor, Vector2 scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(modProj.Texture).Value;
            Vector2 drawPos = modProj.Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = modProj.Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = modProj.Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
   
            float rotation = modProj.Projectile.rotation;
            spriteBatch.Draw(texture, drawPos, drawFrame, Color.White.MultiplyRGB(lightColor), rotation, drawOrigin, scale, spriteEffects, 0);
        }
    }
}
