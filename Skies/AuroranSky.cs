using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static Terraria.Main;

namespace Stellamod.Skies
{
    public class AuroranSky : CustomSky
    {
        public bool isActive;
        public float Intensity;

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override bool IsActive()
        {
            return Intensity > 0f;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Texture2D Tex = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/SkyGradient1").Value;
                Texture2D Tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/SkyGradient2").Value;

                spriteBatch.Draw(Tex, new Rectangle(0, 0 - (int)screenPosition.Y, screenWidth, 3000), null, Color.LightPink * Intensity * 0.4f, 0, Vector2.Zero, SpriteEffects.None, 0);
                for (int i = 0; i < 2; i++)
                    spriteBatch.Draw(Tex2, new Rectangle(0, -30, screenWidth, screenHeight), null, Color.BlueViolet * Intensity * 0.6f, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            else
            {
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
        }

        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.6f));
        }
    }
}