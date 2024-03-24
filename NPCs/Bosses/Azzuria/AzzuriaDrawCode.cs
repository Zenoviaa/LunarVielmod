
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Azzuria
{
    internal partial class Azzuria 
    {
        private int _segmentIndex;
        public const string BaseTexturePath = "Stellamod/NPCs/Bosses/Azzuria/";

        public Vector2 NextSegmentPos;
        public float NextSegmentRot;
        public Vector2 StartSegmentDirection = new Vector2(-1, 0);
        public Vector2[] SegmentPos = new Vector2[Total_Segments];
        public Vector2 SegmentCorrection;
        public float[] SegmentRot = new float[Total_Segments];

        public float SegmentRotation;
        public float SegmentRotationOsc;
        public Vector2 SegmentPosOsc;
        public float SegmentTurnRotation;
        public float HeadRotation;

        // Wing Animation Frame Counters
        public int WingFrameCounter;
        public int WingFrameTick;
        public int WingDrawIndex;

        public const int Total_Segments = Neck_Segments + Body_Segments + Tail_Segments + 4;
        public const int Neck_Segments = 3;
        public const int Body_Segments = 4;
        public const int Tail_Segments = 7;
        public const float Tail_Min_Scale = 0.2f;
        public const float Body_Min_Scale = 0.5f;
        public const float Glow_Distance = 2;
        public Texture2D AzzuriaHead => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaHead").Value;
        public Vector2 AzzuriaHeadSize => new Vector2(172, 108);

        public Texture2D AzzuriaArmBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaArmBack").Value;
        public Vector2 AzzuriaArmBackSize => new Vector2(38, 32);

        public Texture2D AzzuriaArmFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaArmFront").Value;
        public Vector2 AzzuriaArmFrontSize => new Vector2(58, 64);


        public Texture2D AzzuriaLegBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaLegBack").Value;
        public Vector2 AzzuriaLegBackSize => new Vector2(24, 42);

        public Texture2D AzzuriaLegFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaLegFront").Value;
        public Vector2 AzzuriaLegFrontSize => new Vector2(56, 64);

        public Texture2D AzzuriaBodyBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaBodyBack").Value;
        public Vector2 AzzuriaBodyBackSize => new Vector2(38, 54);

        public Texture2D AzzuriaBodyFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaBodyFront").Value;
        public Vector2 AzzuriaBodyFrontSize => new Vector2(36, 64);

        public Texture2D AzzuriaBodyMiddle => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaBodyMiddle").Value;
        public Vector2 AzzuriaBodyMiddleSize => new Vector2(36, 66);

        public Texture2D AzzuriaNeck => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaNeck").Value;
        public Vector2 AzzuriaNeckSize => new Vector2(34, 46);

        public Texture2D AzzuriaTailBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaTailBack").Value;
        public Vector2 AzzuriaTailBackSize => new Vector2(48, 38);

        public Texture2D AzzuriaTailFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaTailFront").Value;
        public Vector2 AzzuriaTailFrontSize => new Vector2(22, 38);

        public Texture2D AzzuriaWingFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaWingFront").Value;
        public Texture2D AzzuriaWingBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaWingBack").Value;
        public Vector2 AzzuriaWingSize => new Vector2(336, 464);

     


        private void SetSegmentPosition(Vector2 segmentSize, float scale = 1f)
        {
            float segmentWidth = ((segmentSize.X / 2) + 8) * scale;
            if(_segmentIndex > 0)
            {
                //Set the position of the segment
                SegmentPos[_segmentIndex] = NextSegmentPos;
                SegmentRot[_segmentIndex] = NextSegmentRot;

                float rotMultiplier = _segmentIndex >= 1 + Neck_Segments && 
                    _segmentIndex <= 1 + Neck_Segments + Body_Segments ? 0.2f : 1f;
                //Set the position and rotation of the next one
                Vector2 segmentDirection = (SegmentPos[_segmentIndex] - SegmentPos[_segmentIndex - 1])
                    .SafeNormalize(Vector2.Zero)
                    .RotatedBy(NextSegmentRot * rotMultiplier);

                NextSegmentPos += segmentDirection * segmentWidth;
                NextSegmentRot += SegmentTurnRotation * rotMultiplier;
            }
            else
            {
                SegmentPos[_segmentIndex] = NextSegmentPos;
                SegmentRot[_segmentIndex] = NextSegmentRot;

                NextSegmentPos += StartSegmentDirection * segmentWidth;
                NextSegmentRot += SegmentTurnRotation;
            }

            _segmentIndex++;
        }

        private void DrawAllSegments(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            _segmentIndex = SegmentPos.Length - 1;
            SegmentCorrection = (SegmentPos[1] - SegmentPos[0]);
            //Draw Tail Back
            DrawSegment(spriteBatch, AzzuriaTailBack, AzzuriaTailBackSize, drawColor, rot: MathHelper.Pi);

            //Draw Tail
            for (int i = 0; i < Tail_Segments; i++)
            {
                float scaleProgress = i / (float)Tail_Segments;
                DrawSegment(spriteBatch, AzzuriaTailFront, AzzuriaTailFrontSize, drawColor, scaleProgress + Tail_Min_Scale);
            }

            //Draw Body Back
            DrawSegment(spriteBatch, AzzuriaBodyBack, AzzuriaBodyBackSize, drawColor, Body_Min_Scale);
            Vector2 wingDrawOrigin = new Vector2(226, 190);

            WingDrawIndex = _segmentIndex - Body_Segments - 1;
            Rectangle drawRectangle = AzzuriaWingBack.AnimationFrame(ref WingFrameCounter, ref WingFrameTick, 4, 9, true);
            Vector2 wingDrawOffset = Vector2.UnitY.RotatedBy(-SegmentRot[WingDrawIndex] - MathHelper.PiOver2) * -24;
            DrawSegment(spriteBatch, AzzuriaWingBack, AzzuriaWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: WingDrawIndex);

            //Draw Back Leg
            int legDrawIndex = _segmentIndex - 2;
            Vector2 backLegDrawOffset = Vector2.UnitY.RotatedBy(SegmentRot[legDrawIndex]) * AzzuriaArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, AzzuriaLegBack, AzzuriaLegBackSize, drawColor,
                    drawOffset: backLegDrawOffset,
                    overrideIndex: legDrawIndex - 1);

            //Draw Body
            for (int i = 0; i < Body_Segments; i++)
            {
                float scaleProgress = i / (float)Body_Segments;
                DrawSegment(spriteBatch, AzzuriaBodyMiddle, AzzuriaBodyMiddleSize, drawColor, scaleProgress + Body_Min_Scale);
            }

            //Draw Back Arm
            int armDrawIndex = _segmentIndex;
            Vector2 backArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * AzzuriaArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, AzzuriaArmBack, AzzuriaArmBackSize, drawColor,
                drawOffset: backArmOffset,
                overrideIndex: armDrawIndex - 1);

    
            //Draw Body Front
            DrawSegment(spriteBatch, AzzuriaBodyFront, AzzuriaBodyFrontSize, drawColor, 1f + Body_Min_Scale);
            for (int i = 0; i < Neck_Segments; i++)
            {
                //Draw Neck
                DrawSegment(spriteBatch, AzzuriaNeck, AzzuriaNeckSize, drawColor, 1.2f);
            }

            //Draw Front Arm
            Vector2 frontArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * AzzuriaArmBackSize.Y;
            DrawSegment(spriteBatch, AzzuriaArmFront, AzzuriaArmFrontSize, drawColor,
                drawOffset: frontArmOffset,
                overrideIndex: armDrawIndex);

            //Draw Front Leg
            Vector2 frontLegDrawOffset = Vector2.UnitY.RotatedBy(SegmentRot[legDrawIndex]) * AzzuriaArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, AzzuriaLegFront, AzzuriaLegFrontSize, drawColor,
                 drawOffset: frontLegDrawOffset,
                 overrideIndex: legDrawIndex);

            DrawSegment(spriteBatch, AzzuriaWingFront, AzzuriaWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: WingDrawIndex);

            //Draw4 Head
            DrawSegment(spriteBatch, AzzuriaHead, AzzuriaHeadSize, drawColor);
        }

        private void DrawSegmentGlow(SpriteBatch spriteBatch, Texture2D segmentTexture, Vector2 segmentSize, Color drawColor,
            Vector2? drawOrigin = null,
            Rectangle? sourceRectangle = null,
            Vector2? drawOffset = null,
            int overrideIndex = -1)
        {
            Vector2 origin = segmentSize / 2;
            if (drawOrigin != null)
            {
                origin = (Vector2)drawOrigin;
            }

            if (_segmentIndex == SegmentRot.Length - 1)
            {
                SegmentRot[_segmentIndex] += MathHelper.PiOver2;
            }

            Vector2 drawPos;
            if (overrideIndex != -1)
            {
                drawPos = SegmentPos[overrideIndex];
            }
            else
            {
                drawPos = SegmentPos[_segmentIndex];
            }

            if (drawOffset != null)
            {
                drawPos += (Vector2)drawOffset;
            }

            if (_segmentIndex != 0 || overrideIndex != -1)
            {
                drawPos -= SegmentCorrection;
            }


            Color rotatedColor = Color.LightSkyBlue;
            rotatedColor = rotatedColor.MultiplyAlpha(0.1f);
            float glowDistance = Glow_Distance;
            if (overrideIndex != -1)
            {
                Vector2 posOsc = SegmentPosOsc * (overrideIndex + 1);
                float rotationOsc = SegmentRotationOsc * (overrideIndex + 1);


                float time = Main.GlobalTimeWrappedHourly;
                float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
                float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
                time %= 4f;
                time /= 2f;

                if (time >= 1f)
                {
                    time = 2f - time;
                }

                time = time * 0.5f + 0.5f;
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    Vector2 rotatedPos = drawPos + posOsc + new Vector2(0f, glowDistance * rotationOffset).RotatedBy(radians) * time;
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[overrideIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
                }


                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[overrideIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
            }
        }

        private void DrawSegment(SpriteBatch spriteBatch, Texture2D segmentTexture, Vector2 segmentSize, Color drawColor,
            float scale = 1,
            float rot = 0,
            Vector2? drawOrigin = null, 
            Rectangle? sourceRectangle = null, 
            Vector2? drawOffset = null, 
            int overrideIndex = -1)
        {
            Vector2 origin = segmentSize / 2;
            if(drawOrigin != null)
            {
                origin = (Vector2)drawOrigin;
            }

            if(_segmentIndex == SegmentRot.Length - 1)
            {
                SegmentRot[_segmentIndex] += MathHelper.PiOver2;
            }

            Vector2 drawPos;
            if(overrideIndex != -1)
            {
                drawPos = SegmentPos[overrideIndex];
            }
            else
            {
                drawPos = SegmentPos[_segmentIndex];
            }

            if (drawOffset != null)
            {
                drawPos += (Vector2)drawOffset;
            }

            if(_segmentIndex != 0 || overrideIndex != -1)
            {
                drawPos -= SegmentCorrection;
            }


            Color rotatedColor = Color.LightSkyBlue;
            rotatedColor = rotatedColor.MultiplyAlpha(0.1f);
            float glowDistance = Glow_Distance;
            if(overrideIndex != -1)
            {
                Vector2 posOsc = SegmentPosOsc * (overrideIndex + 1);
                float rotationOsc = SegmentRotationOsc * (overrideIndex + 1);

    
                float time = Main.GlobalTimeWrappedHourly;
                float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
                float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
                time %= 4f;
                time /= 2f;

                if (time >= 1f)
                {
                    time = 2f - time;
                }

                time = time * 0.5f + 0.5f;
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    Vector2 rotatedPos = drawPos + posOsc + new Vector2(0f, glowDistance * rotationOffset).RotatedBy(radians) * time;
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[overrideIndex] + rot + rotationOsc, origin, scale, SpriteEffects.None, 0);
                }

       
                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[overrideIndex] + rot + rotationOsc, origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                float rotationOsc = SegmentRotationOsc * (_segmentIndex + 1);
                Vector2 posOsc = SegmentPosOsc * (_segmentIndex + 1);
                if (_segmentIndex == 0)
                {
                    SegmentRot[_segmentIndex] = HeadRotation;
                }

                //Circuling After image Thing
                float time = Main.GlobalTimeWrappedHourly;
                float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
                float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
                time %= 4f;
                time /= 2f;

                if (time >= 1f)
                {
                    time = 2f - time;
                }

                time = time * 0.5f + 0.5f;
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    Vector2 rotatedPos = drawPos + posOsc + new Vector2(0f, glowDistance * rotationOffset).RotatedBy(radians) * time;
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[_segmentIndex] + rot + rotationOsc, origin, scale, SpriteEffects.None, 0);
                }

                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[_segmentIndex] + rot + rotationOsc, origin, scale, SpriteEffects.None, 0);
                _segmentIndex--;
            }
        }

        private void SetSegmentPositions(Vector2 screenPos)
        {
            _segmentIndex = 0;
            NextSegmentPos = NPC.Center - screenPos;
            NextSegmentRot = NPC.rotation;

            //Draw4 Head
            SetSegmentPosition(AzzuriaHeadSize);
            for (int i = 0; i < Neck_Segments; i++)
            {
                SetSegmentPosition(AzzuriaNeckSize);
            }

            //Draw Body
            SetSegmentPosition(AzzuriaBodyFrontSize, 1f + Body_Min_Scale);
            for (int i = 0; i < Body_Segments; i++)
            {
                float scaleProgress = i / (float)Body_Segments;
                SetSegmentPosition(AzzuriaBodyMiddleSize, (1f - scaleProgress) + Body_Min_Scale);
            }

            SetSegmentPosition(AzzuriaBodyBackSize, Body_Min_Scale);
            //Draw Tail
            for (int i = 0; i < Tail_Segments; i++)
            {
                float scaleProgress = i / (float)Tail_Segments;
                SetSegmentPosition(AzzuriaTailFrontSize, (1f - scaleProgress) + Tail_Min_Scale);
            }
            SetSegmentPosition(AzzuriaTailBackSize, 0.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SetSegmentPositions(screenPos);
            DrawAllSegments(spriteBatch, screenPos, drawColor);
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 1.75f * Main.essScale);
            return false;
        }

    
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rectangle drawRectangle = AzzuriaWingBack.AnimationFrame(ref WingFrameCounter, ref WingFrameTick, 4, 9, false);
            Vector2 wingDrawOffset = Vector2.UnitY.RotatedBy(-SegmentRot[WingDrawIndex] - MathHelper.PiOver2) * -24;
            Vector2 wingDrawOrigin = new Vector2(226, 190);

            DrawSegmentGlow(spriteBatch, AzzuriaWingFront, AzzuriaWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: WingDrawIndex);
        }
    }
}
