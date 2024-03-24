
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System.Diagnostics.Metrics;
using System.IO;
using Terraria;
using Terraria.IO;
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
        public Vector2[] SegmentPos = new Vector2[TotalSegments];
        public Vector2 SegmentCorrection;
        public float[] SegmentRot = new float[TotalSegments];

        public float SegmentRotation;
        public float SegmentRotationOsc;
        public Vector2 SegmentPosOsc;
        public float SegmentTurnRotation;
        public float HeadRotation;


        // Current frame
        public int FrameCounter;
        // Current frame's progress
        public int FrameTick;

        public const int TotalSegments = NeckSegments + BodySegments + TailSegments + 4;
        public const int NeckSegments = 4;
        public const int BodySegments = 2;
        public const int TailSegments = 3;

        public Texture2D AzzuriaHead => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaHead").Value;
        public Vector2 AzzuriaHeadSize => new Vector2(172, 108);

        public Texture2D AzzuriaArmBack => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaArmBack").Value;
        public Vector2 AzzuriaArmBackSize => new Vector2(38, 32);

        public Texture2D AzzuriaArmFront => ModContent.Request<Texture2D>($"{BaseTexturePath}AzzuriaArmFront").Value;
        public Vector2 AzzuriaArmFrontSize => new Vector2(58, 64);

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

     


        private void SetSegmentPosition(Vector2 segmentSize)
        {
            float segmentWidth = (segmentSize.X / 2) + 8;
            if(_segmentIndex > 0)
            {
                //Set the position of the segment
                SegmentPos[_segmentIndex] = NextSegmentPos;
                SegmentRot[_segmentIndex] = NextSegmentRot;

                //Set the position and rotation of the next one
                Vector2 segmentDirection = (SegmentPos[_segmentIndex] - SegmentPos[_segmentIndex - 1])
                    .SafeNormalize(Vector2.Zero)
                    .RotatedBy(NextSegmentRot);

                NextSegmentPos += segmentDirection * segmentWidth;
                NextSegmentRot += SegmentTurnRotation;
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
            DrawSegment(spriteBatch, AzzuriaTailBack, AzzuriaTailBackSize, drawColor);

            //Draw Tail
            for (int i = 0; i < TailSegments; i++)
            {
                DrawSegment(spriteBatch, AzzuriaTailFront, AzzuriaTailFrontSize, drawColor);
            }

            //Draw Body Back
            DrawSegment(spriteBatch, AzzuriaBodyBack, AzzuriaBodyBackSize, drawColor);
            Vector2 wingDrawOrigin = new Vector2(226, 190);

            int wingDrawIndex = _segmentIndex - 2;
            Rectangle drawRectangle = AzzuriaWingBack.AnimationFrame(ref FrameCounter, ref FrameTick, 4, 9, true);
            Vector2 wingDrawOffset = Vector2.UnitY.RotatedBy(-SegmentRot[wingDrawIndex] - MathHelper.PiOver2) * -24;
            DrawSegment(spriteBatch, AzzuriaWingBack, AzzuriaWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: wingDrawIndex);

            //Draw Body
            for (int i = 0; i < BodySegments; i++)
            {
                DrawSegment(spriteBatch, AzzuriaBodyMiddle, AzzuriaBodyMiddleSize, drawColor);
            }

            //Draw Back Arm
            int armDrawIndex = _segmentIndex;
            Vector2 backArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * AzzuriaArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, AzzuriaArmBack, AzzuriaArmBackSize, drawColor,
                drawOffset: backArmOffset,
                overrideIndex: armDrawIndex - 1);

            //Draw Body Front
            DrawSegment(spriteBatch, AzzuriaBodyFront, AzzuriaBodyFrontSize, drawColor);
            for (int i = 0; i < NeckSegments; i++)
            {
                //Draw Neck
                DrawSegment(spriteBatch, AzzuriaNeck, AzzuriaNeckSize, drawColor);
            }

            //Draw Front Arm
            Vector2 frontArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * AzzuriaArmBackSize.Y;
            DrawSegment(spriteBatch, AzzuriaArmFront, AzzuriaArmFrontSize, drawColor,
                drawOffset: frontArmOffset,
                overrideIndex: armDrawIndex);

            DrawSegment(spriteBatch, AzzuriaWingFront, AzzuriaWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: wingDrawIndex);

            //Draw4 Head
            DrawSegment(spriteBatch, AzzuriaHead, AzzuriaHeadSize, drawColor);
        }

        private void DrawSegment(SpriteBatch spriteBatch, Texture2D segmentTexture, Vector2 segmentSize, Color drawColor, 
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
            rotatedColor = rotatedColor.MultiplyAlpha(0.5f);
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
                    Vector2 rotatedPos = drawPos + posOsc + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[overrideIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
                }

       
                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[overrideIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
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
                    Vector2 rotatedPos = drawPos + posOsc + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[_segmentIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
                }

                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[_segmentIndex] + rotationOsc, origin, 1, SpriteEffects.None, 0);
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
            for (int i = 0; i < NeckSegments; i++)
            {
                SetSegmentPosition(AzzuriaNeckSize);
            }

            //Draw Body
            SetSegmentPosition(AzzuriaBodyFrontSize);
            for (int i = 0; i < BodySegments; i++)
            {
                SetSegmentPosition(AzzuriaBodyMiddleSize);
            }

            SetSegmentPosition(AzzuriaBodyBackSize);
            //Draw Tail
            for (int i = 0; i < TailSegments; i++)
            {
                SetSegmentPosition(AzzuriaTailFrontSize);
            }
            SetSegmentPosition(AzzuriaTailBackSize);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SetSegmentPositions(screenPos);
            DrawAllSegments(spriteBatch, screenPos, drawColor);
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 1.75f * Main.essScale);
            return false;
        }

        /*
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, Color.LightSkyBlue.ToVector3() * 1.75f * Main.essScale);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
            spriteBatch.Draw(
                GlowTexture,
                new Vector2(NPC.position.X - screenPos.X + (NPC.width / 2) - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                NPC.frame,
                Color.White,
                NPC.rotation,
                halfSize,
                NPC.scale,
                spriteEffects,
            0);
        }*/
    }
}
