using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Visual.Particles
{

    public struct ShockOvalSpawnParams
    {
        public ShockOvalSpawnParams()
        {
            innerColor = Color.White;
            outerColor = Color.Yellow;
            scaleRange = new Vector2(0.5f, 1);
            gravity = 0;
        }
        public Color innerColor;
        public Color outerColor;
        public Vector2 scaleRange;
        public float gravity;
        public static ShockOvalSpawnParams Default = new ShockOvalSpawnParams();
    }

    public class ShockOvalParticle : Particle<ShockOvalParticle>
    {
        public int FrameWidth = 90;
        public int FrameHeight = 108;
        public int MaxFrameCount = 5;
        public float gravity;
        public Color innerColor;
        public Color outerColor;
        public Vector2 stretchScale;
        public float dampening;
        public bool fast;
        public float animTimer;
        public int frame;
        public static ShockOvalParticle Spawn(Vector2 position, Vector2 velocity, ShockOvalSpawnParams? spawnParams = null)
        {
            if (!spawnParams.HasValue)
                spawnParams = new ShockOvalSpawnParams();
            ShockOvalSpawnParams settings = spawnParams.Value;
            float scale = Main.rand.NextFloat(settings.scaleRange.X, settings.scaleRange.Y);
            ShockOvalParticle dp = Spawn(position, velocity, Color.White, scale);
            dp.innerColor = settings.innerColor;
            dp.outerColor = settings.outerColor;
            dp.gravity = settings.gravity;
            return dp;
        }

        public override void OnSpawn()
        {
            stretchScale = Vector2.One;
            frame = 0;
            animTimer = 0;
            gravity = 0;
            innerColor = Color.White;
            outerColor = Color.Red;
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            customShader = DustShader.Instance;
        }

        public override void Update()
        {
            animTimer++;
            if(animTimer >= 4 && frame < MaxFrameCount)
            {
                animTimer = 0;
                frame++;
                Rectangle spriteFrame = new Rectangle(0, frame * FrameHeight, FrameWidth, FrameHeight);
                Frame = spriteFrame;    
            }
           // Velocity.Y += gravity;
            Velocity *= 1.0f - dampening;
            Rotation = Velocity.ToRotation() - MathHelper.Pi;
            Scale *= 0.97f;
            if (fast)
                Scale *= 0.98f;
            color *= 0.99f;

            fadeIn++;
            if (fadeIn > 180 || Scale < 0.1f)
                active = false;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            DustShader shader = DustShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;
            shader.Apply();

            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }
}
