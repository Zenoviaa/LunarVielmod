using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox
{
   
    public enum FoxDirection : byte
    {
        FrontNeutral,
        FrontUp,
        FrontDown,
        SideUp,
        SideDown,
        BackUp,
        BackNeutral,
        BackDown
    }
    
    public class FoxSegment
    {
        //Represents a body part
        public FoxSegment(Asset<Texture2D> textureAsset, FoxSegment parent, float length, float angle, Vector2 origin)
        {
            this.textureAsset = textureAsset;
            this.parent = parent;
            this.length = length;
            this.angle = angle;
            this.origin = origin;
            this.scale = Vector2.One;
        }

        public Asset<Texture2D> textureAsset;
        public Rectangle? frame;
        public FoxSegment parent;
        public Vector2 position;
        public Vector2 origin;
        public float length;
        public float angle;
        public Vector2 scale;
        public bool flipX;
        public FoxDirection direction;
        
        public void Update()
        {
            //Calculate frame based on the direction


            if (parent == null)
                return;
            position = parent.position + parent.length * parent.angle.ToRotationVector2();
            
        }

        public void Draw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D textureToDraw = textureAsset.Value;
            Vector2 drawPosition = position - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);

            Vector2 drawOrigin = origin;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (flipX)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                if(frame != null)
                {
                    drawOrigin.X = frame.Value.Width - origin.X;
                }
                else
                {
                    drawOrigin.X = textureAsset.Width() - origin.X;
                }
            }

            spriteBatch.Draw(textureToDraw, drawPosition, frame, drawColor, angle, drawOrigin, scale, spriteEffects, 0);
        }
    }

    public partial class RoyalFox : ScarletBoss
    {
    }
}
