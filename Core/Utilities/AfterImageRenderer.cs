using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities
{
    public class AfterImage
    {
        public string texture;
        public Rectangle frame;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public Vector2 scale;
        public Vector2 origin;
        public Color color;
        public float time;
        public SpriteEffects spriteEffects;
    }

    [Autoload(Side = ModSide.Client)]
    public class AfterImageRenderer : ModSystem
    {
        private List<AfterImage> _afterImages;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _afterImages = new();
            On_Main.DrawNPCs += DrawAfterImages;

        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawNPCs -= DrawAfterImages;
        }

        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            foreach (var item in _afterImages)
            {
                item.position += item.velocity;
                item.time++;
            }
            _afterImages.RemoveAll(x => x.time >= 60);
        }

        private void DrawAfterImages(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (_afterImages.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                for (int i = 0; i < _afterImages.Count; i++)
                {
                    AfterImage afterImage = _afterImages[i];
                    Texture2D afterImageTexture = ModContent.Request<Texture2D>(afterImage.texture).Value;
                    Vector2 drawOrigin = afterImage.origin;
                    Vector2 drawPosition = afterImage.position - Main.screenPosition;

                    float fadeTime = 30f;
                    float fadeRatio = afterImage.time / fadeTime;
                    float alpha = MathHelper.Lerp(1f, 0f, fadeRatio);

                    Color afterImageColor = afterImage.color;
                    afterImageColor *= alpha;
                    spriteBatch.Draw(afterImageTexture, drawPosition, afterImage.frame, afterImageColor, afterImage.rotation, afterImage.origin, afterImage.scale, afterImage.spriteEffects, 0);
                }
            }


            orig(self, behindTiles);
        }

        public static void New(string texture, Rectangle frame, Vector2 position, Vector2 velocity, float rotation, Vector2 scale, Vector2 origin, Color color, SpriteEffects spriteEffects)
        {
            AfterImage afterImage = new AfterImage
            {
                texture = texture,
                rotation = rotation,
                position = position,
                velocity = velocity,
                frame = frame,
                spriteEffects = spriteEffects,
                origin = origin,
                color = color,
                scale = scale
            };
            AfterImageRenderer renderer = ModContent.GetInstance<AfterImageRenderer>();
            renderer._afterImages.Add(afterImage);
        }
    }
}
