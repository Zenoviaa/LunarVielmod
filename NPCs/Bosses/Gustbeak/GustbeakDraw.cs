using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak
{
    internal partial class Gustbeak
    {
        internal abstract class BaseGustbeakSegment
        {
            public Vector2 position;
            public float rotation;
            public float globalRotation;
            public float length;
            public Vector2? drawOriginOverride;

            public float frameCounter;
            public int frame;
            public SpriteEffects spriteEffects;
            public BaseGustbeakSegment[] children;
            public bool drawArmored;
            public virtual void AI()
            {
                if (children != null)
                {
                    //Update everything attached to this segment
                    for (int i = 0; i < children.Length; i++)
                    {
                        var child = children[i];
                        child.position = position;
                        child.spriteEffects = spriteEffects;
                        child.rotation = rotation;
                        child.AI();
                    }
                }
            }

            public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {

            }

            protected virtual Texture2D GetTexture(string name)
            {
                string path = $"Stellamod/NPCs/Bosses/Gustbeak/Gustbeak_{name}";
                Texture2D texture = ModContent.Request<Texture2D>(path).Value;
                return texture;
            }
            protected virtual Texture2D GetArmoredTexture(string name)
            {
                string path = $"Stellamod/NPCs/Bosses/Gustbeak/Gustbeak_{name}_Armored";
                if(ModContent.RequestIfExists<Texture2D>(path, out var asset))
                {
                    return asset.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        internal abstract class BaseGustbeakBodySegment : BaseGustbeakSegment
        {
            public virtual string Texture { get; }
            public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                base.Draw(spriteBatch, screenPos, drawColor);
                Color colorToDrawIn = Color.White.MultiplyRGB(drawColor);
                Texture2D texture = GetTexture(Texture);
                Vector2 drawPos = position - screenPos;
                Vector2 drawOrigin = texture.Size() / 2;
                if (drawOriginOverride.HasValue)
                {
                    drawOrigin = drawOriginOverride.Value;
                    if (spriteEffects == SpriteEffects.FlipVertically)
                    {
                        drawOrigin.Y = texture.Size().Y - drawOrigin.Y;
                    }
                }
                float drawScale = 1f;
                float drawRotation = rotation + globalRotation;
                spriteBatch.Draw(texture, drawPos, null, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);


                texture = GetArmoredTexture(Texture);
                if (!drawArmored || texture == null)
                    return;

                MiscShaderData miscShaderData = GameShaders.Misc["LunarVeil:GustArmor"];
                miscShaderData.Shader.Parameters["time"].SetValue(MathF.Sin(Main.GlobalTimeWrappedHourly * 0.02f ) * 24);
                miscShaderData.Shader.Parameters["noiseTexture"].SetValue(TrailRegistry.CrystalNoise.Value);
                miscShaderData.Shader.Parameters["noiseTextureSize"].SetValue(TrailRegistry.CrystalNoise.Value.Size());
                miscShaderData.Apply();

                spriteBatch.Restart(effect: miscShaderData.Shader, blendState: BlendState.Additive);
                spriteBatch.Draw(texture, drawPos, null, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
                spriteBatch.Draw(texture, drawPos, null, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
                spriteBatch.RestartDefaults();
            }
        }
        internal abstract class BaseGustbeakWingSegment : BaseGustbeakSegment
        {
            public virtual string Texture { get; }
            internal enum AnimationState
            {
                Flap,
                Hold_Up,
                Hold_Out,
                Rest_Down,
                Rest,
            }

            private AnimationState _animationState;
            public AnimationState Animation
            {
                get
                {
                    return _animationState;
                }
                set
                {
                    switch (value)
                    {
                        case AnimationState.Hold_Up:
                            if (CheckCurrentAnimation(AnimationState.Hold_Up, AnimationState.Hold_Out))
                                return;
                            break;
                        case AnimationState.Rest_Down:
                            if (CheckCurrentAnimation(AnimationState.Rest_Down, AnimationState.Rest))
                                return;
                            break;
                    }
                    _animationState = value;
                }
            }


            private bool CheckCurrentAnimation(params AnimationState[] animations)
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    AnimationState animation = animations[i];
                    if (Animation == animation)
                        return true;
                }
                return false;
            }

            public float WingAnimationSpeedMult { get; set; } = 1f;
            public override void AI()
            {
                base.AI();
                switch (Animation)
                {
                    default:
                    case AnimationState.Flap:
                        frameCounter += 0.25f * WingAnimationSpeedMult;
                        if (frameCounter >= 1f)
                        {
                            frameCounter = 0;
                            frame++;
                        }

                        if (frame >= 9)
                        {
                            frame = 0;
                        }
                        break;
                    case AnimationState.Hold_Up:
                        frameCounter += 0.25f * WingAnimationSpeedMult;
                        if (frameCounter >= 1f)
                        {
                            frameCounter = 0;
                            frame++;
                        }

                        if (frame == 1)
                        {
                            frame = 1;
                            Animation = AnimationState.Hold_Out;
                        }
                        break;
                    case AnimationState.Hold_Out:
                        frame = 1;
                        break;
                    case AnimationState.Rest_Down:
                        frameCounter += 0.25f * WingAnimationSpeedMult;
                        if (frameCounter >= 1f)
                        {
                            frameCounter = 0;
                            frame++;
                        }

                        if (frame == 8)
                        {
                            frame = 8;
                            Animation = AnimationState.Rest;
                        }
                        break;
                    case AnimationState.Rest:
                        frame = 8;
                        break;
                }
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                base.Draw(spriteBatch, screenPos, drawColor);
                Color colorToDrawIn = Color.White.MultiplyRGB(drawColor);
                Texture2D texture = GetTexture(Texture);
                Vector2 drawPos = position - screenPos;
                Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 9);
                Vector2 drawOrigin = new Vector2(132, 90);
                float drawRotation = rotation + MathHelper.ToRadians(45) + globalRotation;
                float drawScale = 1f;


                if (drawOriginOverride.HasValue)
                {
                    drawOrigin = drawOriginOverride.Value;
                    if (spriteEffects == SpriteEffects.FlipVertically)
                    {
                        drawOrigin.Y = animationFrame.Size().Y - drawOrigin.Y;
                        drawRotation -= MathHelper.ToRadians(90);
                    }
                }
    
                spriteBatch.Draw(texture, drawPos, animationFrame, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }
        }


        internal class GustbeakBodyFront : BaseGustbeakBodySegment
        {
            public override string Texture => "BodyFront";
            public override void AI()
            {
                base.AI();
            }
        }

        internal class GustbeakBodyMiddle : BaseGustbeakBodySegment
        {
            public override string Texture => "BodyMiddle";
            public override void AI()
            {
                base.AI();
            }
        }

        internal class GustbeakBodyBack : BaseGustbeakBodySegment
        {
            public override string Texture => "BodyBack";
            public override void AI()
            {
                base.AI();
            }
        }

        internal class GustbeakFrontLegFront : BaseGustbeakBodySegment
        {
            public override string Texture => "FrontLegFront";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(20, -8);
            }
        }

        internal class GustbeakFrontLegBack : BaseGustbeakBodySegment
        {
            public override string Texture => "FrontLegBack";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(-10, 0);
            }
        }

        internal class GustbeakBackLegFront : BaseGustbeakBodySegment
        {
            public override string Texture => "BackLegFront";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(8, -8);
            }
        }

        internal class GustbeakBackLegBack : BaseGustbeakBodySegment
        {
            public override string Texture => "BackLegBack";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(-8, 0);
            }
        }

        internal class GustbeakTail : BaseGustbeakBodySegment
        {
            public override string Texture => "Tail";
            public override void AI()
            {
                base.AI();
            }
        }


        internal class GustbeakWingFront : BaseGustbeakWingSegment
        {
            public override string Texture => "WingsFront";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(132, 90);
            }
        }

        internal class GustbeakWingBack : BaseGustbeakWingSegment
        {
            public override string Texture => "WingsBack";
            public override void AI()
            {
                base.AI();
                drawOriginOverride = new Vector2(132, 128);
            }
        }

        internal class GustbeakHead : BaseGustbeakSegment
        {
            internal enum AnimationState
            {
                Idle,
                Open_Mouth,
                Keep_Open,
                Close_Mouth
            }

            private AnimationState _animationState;
            public AnimationState Animation
            {
                get
                {
                    return _animationState;
                }
                set
                {
                    switch (value)
                    {
                        case AnimationState.Open_Mouth:
                            if (CheckCurrentAnimation(AnimationState.Open_Mouth, AnimationState.Keep_Open))
                                return;
                            break;
                        case AnimationState.Close_Mouth:
                            if (CheckCurrentAnimation(AnimationState.Close_Mouth, AnimationState.Idle))
                                return;
                            break;
                    }
                    _animationState = value;
                }
            }


            private bool CheckCurrentAnimation(params AnimationState[] animations)
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    AnimationState animation = animations[i];
                    if (Animation == animation)
                        return true;
                }
                return false;
            }

            public override void AI()
            {
                base.AI();
                switch (Animation)
                {
                    default:
                    case AnimationState.Open_Mouth:
                        frameCounter += 0.35f;
                        if (frameCounter >= 1f)
                        {
                            frameCounter = 0;
                            frame++;
                        }

                        if (frame >= 3)
                        {
                            Animation = AnimationState.Keep_Open;
                        }
                        break;
                    case AnimationState.Keep_Open:
                        frame = 3;
                        break;
                    case AnimationState.Close_Mouth:
                        frameCounter += 0.35f;
                        if (frameCounter >= 1f)
                        {
                            frameCounter = 0;
                            frame--;
                        }
                        if (frame <= 0)
                        {
                            frame = 0;
                            Animation = AnimationState.Idle;
                        }
                        break;
                    case AnimationState.Idle:
                        frame = 0;
                        break;
                }
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
            {
                base.Draw(spriteBatch, screenPos, drawColor);
                Color colorToDrawIn = Color.White.MultiplyRGB(drawColor);
                Texture2D texture = GetTexture("Head");
                Vector2 drawPos = position - screenPos;
                Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 4);
                Vector2 drawOrigin = animationFrame.Size() / 2;
                float drawScale = 1f;
                float drawRotation = rotation + globalRotation;
                spriteBatch.Draw(texture, drawPos, animationFrame, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }
        }
    }
}
