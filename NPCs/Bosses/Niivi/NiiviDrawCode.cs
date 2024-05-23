
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal class NiiviCrystalDraw
    {
        public NiiviCrystalDraw(NPC npc, Texture2D texture)
        {
            DrawPosition = npc.Center;
            Npc = npc;
            Texture = texture;
        }

        private bool _draw;
        public bool Draw
        {
            get
            {
                return _draw;
            }
            set
            {
                if(value == false && _draw != false)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromThis(), DrawPosition, Vector2.Zero,
                            ModContent.ProjectileType<NiiviCrystalWarpExplosionProj>(), 0, 0, Main.myPlayer);
                    }
                }

                _draw = value;
            }
        }
        public Vector2 DrawPosition;
        public Texture2D Texture;
        public NPC Npc;
    }

    internal partial class Niivi 
    {
        private int _segmentIndex;
        public const string BaseTexturePath = "Stellamod/NPCs/Bosses/Niivi/";
        private string BaseProjectileTexturePath => $"{BaseTexturePath}Projectiles/";

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

        //Crystals
        public NiiviCrystalDraw[] Crystals;

        // Wing Animation Frame Counters
        public int WingFrameCounter;
        public int WingFrameTick;
        public int WingDrawIndex;
        public int FlightDirection;
        public float CurrentFlightDirection;
        public SpriteEffects Effects => CurrentFlightDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        public const int Total_Segments = Neck_Segments + Body_Segments + Tail_Segments + 4;
        public const int Neck_Segments = 3;
        public const int Body_Segments = 4;
        public const int Tail_Segments = 7;
        public const float Tail_Min_Scale = 0.2f;
        public const float Body_Min_Scale = 0.5f;
        public const float Glow_Distance = 2;

        public Texture2D NiiviHead => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviHead").Value;
        public Vector2 NiiviHeadSize => new Vector2(172, 108);

        public Texture2D NiiviArmBack => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviArmBack").Value;
        public Vector2 NiiviArmBackSize => new Vector2(38, 32);

        public Texture2D NiiviArmFront => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviArmFront").Value;
        public Vector2 NiiviArmFrontSize => new Vector2(58, 64);


        public Texture2D NiiviLegBack => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviLegBack").Value;
        public Vector2 NiiviLegBackSize => new Vector2(24, 42);

        public Texture2D NiiviLegFront => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviLegFront").Value;
        public Vector2 NiiviLegFrontSize => new Vector2(56, 64);

        public Texture2D NiiviBodyBack => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviBodyBack").Value;
        public Vector2 NiiviBodyBackSize => new Vector2(38, 54);

        public Texture2D NiiviBodyFront => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviBodyFront").Value;
        public Vector2 NiiviBodyFrontSize => new Vector2(36, 64);

        public Texture2D NiiviBodyMiddle => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviBodyMiddle").Value;
        public Vector2 NiiviBodyMiddleSize => new Vector2(36, 66);

        public Texture2D NiiviNeck => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviNeck").Value;
        public Vector2 NiiviNeckSize => new Vector2(34, 46);

        public Texture2D NiiviTailBack => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviTailBack").Value;
        public Vector2 NiiviTailBackSize => new Vector2(48, 38);

        public Texture2D NiiviTailFront => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviTailFront").Value;
        public Vector2 NiiviTailFrontSize => new Vector2(22, 38);

        public Texture2D NiiviWingFront => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviWingFront").Value;
        public Texture2D NiiviWingBack => ModContent.Request<Texture2D>($"{BaseTexturePath}NiiviWingBack").Value;
        public Vector2 NiiviWingSize => new Vector2(336, 464);

        private bool ChargeCrystals;
        private float ChargeCrystalTimer;
        private bool Black;

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

                NextSegmentPos += segmentDirection * segmentWidth * Math.Abs(StartSegmentDirection.X);
                NextSegmentRot += SegmentTurnRotation * rotMultiplier;
            }
            else
            {
                SegmentPos[_segmentIndex] = NextSegmentPos;
                SegmentRot[_segmentIndex] = NextSegmentRot;

                NextSegmentPos += StartSegmentDirection * segmentWidth * Math.Abs(StartSegmentDirection.X);
                NextSegmentRot += SegmentTurnRotation;
            }

            _segmentIndex++;
        }

        private void DrawAllSegments(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var shader = ShaderRegistry.MiscSilPixelShader;

            //The color to lerp to
            shader.UseColor(Main.DiscoColor);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            shader.UseSaturation(ChargeCrystalTimer);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            DrawSegments(spriteBatch, screenPos, drawColor, true);

     
            spriteBatch.End();
            spriteBatch.Begin();

            //Draw Normal Niivi
            DrawSegments(spriteBatch, screenPos, drawColor, false);


            if (Black)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                shader = ShaderRegistry.MiscSilPixelShader;

                //The color to lerp to
                shader.UseColor(Color.Black);

                //Should be between 0-1
                //1 being fully opaque
                //0 being the original color
                shader.UseSaturation(ChargeCrystalTimer);

                // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
                shader.Apply(null);

                DrawSegments(spriteBatch, screenPos, drawColor, true);

                spriteBatch.End();
                spriteBatch.Begin();
            }

            if (SpecialTimer >= 2500 || State == ActionState.Laser_Blast_V2 || State == ActionState.Star_Wrath_V2 || State == ActionState.Space_Circle)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                shader = ShaderRegistry.MiscFireWhitePixelShader;

                shader.UseOpacity(1f);

                //How intense the colors are
                //Should be between 0-1
                shader.UseIntensity(1f);

                //How fast the extra texture animates
                float speed = 5f;
                shader.UseSaturation(speed);

                //Color
                shader.UseColor(Main.DiscoColor);

                //Texture itself
                shader.UseImage1(TextureRegistry.CloudTexture);

                // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
                shader.Apply(null);

                DrawSegments(spriteBatch, screenPos, drawColor, false);

                spriteBatch.End();
                spriteBatch.Begin();
            }

            if(State == ActionState.Transition_P2)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                shader = ShaderRegistry.MiscSilPixelShader;

                //The color to lerp to
                shader.UseColor(Color.Black);

                //Should be between 0-1
                //1 being fully opaque
                //0 being the original color
                shader.UseSaturation(0.5f);

                // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
                shader.Apply(null);

                DrawSegments(spriteBatch, screenPos, drawColor, true);

                spriteBatch.End();
                spriteBatch.Begin();
            }
        }


        private void DrawSegments(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, bool animate)
        {
            _segmentIndex = SegmentPos.Length - 1;
            SegmentCorrection = (SegmentPos[1] - SegmentPos[0]);
            //Draw Tail Back
            DrawSegment(spriteBatch, NiiviTailBack, NiiviTailBackSize, drawColor, rot: MathHelper.Pi);

            //Draw Tail
            for (int i = 0; i < Tail_Segments; i++)
            {
                float scaleProgress = i / (float)Tail_Segments;
                DrawSegment(spriteBatch, NiiviTailFront, NiiviTailFrontSize, drawColor, scaleProgress + Tail_Min_Scale);
            }

            //Draw Body Back
            DrawSegment(spriteBatch, NiiviBodyBack, NiiviBodyBackSize, drawColor, Body_Min_Scale);
            Vector2 rightWingDrawOrigin = new Vector2(226, 190);
            Vector2 flippedWingDrawOrigin = new Vector2(110, 190);
            Vector2 wingDrawOrigin = CurrentFlightDirection < 0 ? flippedWingDrawOrigin : rightWingDrawOrigin;

            int animationSpeed = State == ActionState.Spare_Me ? 12 : 4;
            WingDrawIndex = _segmentIndex - Body_Segments - 1;
            Rectangle drawRectangle;
            switch (State)
            {
                default:
                    drawRectangle = NiiviWingBack.AnimationFrame(ref WingFrameCounter, ref WingFrameTick, animationSpeed, 9, animate);
                    break;
            }

            Vector2 wingDrawOffset = Vector2.UnitY.RotatedBy(-SegmentRot[WingDrawIndex] - MathHelper.PiOver2) * -24;
            DrawSegment(spriteBatch, NiiviWingBack, NiiviWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: WingDrawIndex);

            //Draw Back Leg
            int legDrawIndex = _segmentIndex - 2;
            Vector2 backLegDrawOffset = Vector2.UnitY.RotatedBy(SegmentRot[legDrawIndex]) * NiiviArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, NiiviLegBack, NiiviLegBackSize, drawColor,
                    drawOffset: backLegDrawOffset,
                    overrideIndex: legDrawIndex - 1);

            //Draw Body
            for (int i = 0; i < Body_Segments; i++)
            {
                float scaleProgress = i / (float)Body_Segments;
                DrawSegment(spriteBatch, NiiviBodyMiddle, NiiviBodyMiddleSize, drawColor, scaleProgress + Body_Min_Scale);
            }

            //Draw Back Arm
            int armDrawIndex = _segmentIndex;
            Vector2 backArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * NiiviArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, NiiviArmBack, NiiviArmBackSize, drawColor,
                drawOffset: backArmOffset,
                overrideIndex: armDrawIndex - 1);


            //Draw Body Front
            DrawSegment(spriteBatch, NiiviBodyFront, NiiviBodyFrontSize, drawColor, 1f + Body_Min_Scale);
            for (int i = 0; i < Neck_Segments; i++)
            {
                //Draw Neck
                DrawSegment(spriteBatch, NiiviNeck, NiiviNeckSize, drawColor, 1.2f);
            }

            //Draw Front Arm
            Vector2 frontArmOffset = Vector2.UnitY.RotatedBy(SegmentRot[armDrawIndex]) * NiiviArmBackSize.Y;
            DrawSegment(spriteBatch, NiiviArmFront, NiiviArmFrontSize, drawColor,
                drawOffset: frontArmOffset,
                overrideIndex: armDrawIndex);

            //Draw Front Leg
            Vector2 frontLegDrawOffset = Vector2.UnitY.RotatedBy(SegmentRot[legDrawIndex]) * NiiviArmBackSize.Y * 1.3f;
            DrawSegment(spriteBatch, NiiviLegFront, NiiviLegFrontSize, drawColor,
                 drawOffset: frontLegDrawOffset,
                 overrideIndex: legDrawIndex);

            DrawSegment(spriteBatch, NiiviWingFront, NiiviWingSize, drawColor,
                drawOrigin: wingDrawOrigin,
                drawOffset: wingDrawOffset,
                sourceRectangle: drawRectangle,
                overrideIndex: WingDrawIndex);

            //Draw4 Head
            DrawSegment(spriteBatch, NiiviHead, NiiviHeadSize, drawColor);
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
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[overrideIndex] + rot + rotationOsc, origin, scale, Effects, 0);
                }

       
                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[overrideIndex] + rot + rotationOsc, origin, scale, Effects, 0);
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
                    spriteBatch.Draw(segmentTexture, rotatedPos, sourceRectangle, rotatedColor, SegmentRot[_segmentIndex] + rot + rotationOsc, origin, scale, Effects, 0);
                }

                spriteBatch.Draw(segmentTexture, drawPos + posOsc, sourceRectangle, drawColor, SegmentRot[_segmentIndex] + rot + rotationOsc, origin, scale, Effects, 0);
                _segmentIndex--;
            }
        }

        private void SetSegmentPositions(Vector2 screenPos)
        {
            _segmentIndex = 0;
            NextSegmentPos = NPC.Center - screenPos;
            NextSegmentRot = 0;

            //Draw4 Head
            SetSegmentPosition(NiiviHeadSize);
            for (int i = 0; i < Neck_Segments; i++)
            {
                SetSegmentPosition(NiiviNeckSize);
            }

            //Draw Body
            SetSegmentPosition(NiiviBodyFrontSize, 1f + Body_Min_Scale);
            for (int i = 0; i < Body_Segments; i++)
            {
                float scaleProgress = i / (float)Body_Segments;
                SetSegmentPosition(NiiviBodyMiddleSize, (1f - scaleProgress) + Body_Min_Scale);
            }

            SetSegmentPosition(NiiviBodyBackSize, Body_Min_Scale);
            //Draw Tail
            for (int i = 0; i < Tail_Segments; i++)
            {
                float scaleProgress = i / (float)Tail_Segments;
                SetSegmentPosition(NiiviTailFrontSize, (1f - scaleProgress) + Tail_Min_Scale);
            }
            SetSegmentPosition(NiiviTailBackSize, 0.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (SegmentPos == null)
                return false;
            SetSegmentPositions(screenPos);
            DrawAllSegments(spriteBatch, screenPos, drawColor);
            DrawCrystals(spriteBatch, screenPos, drawColor);
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 1.75f * Main.essScale);
            return false;
        }

        private void DrawCrystals(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            float speed = ChargeCrystals ? 0.3f : 0.015f;
            for(float i = 0; i < Crystals.Length; i++)
            {
                var crystal = Crystals[(int)i];
                if (!crystal.Draw)
                    continue;

                float progress = i / (float)Crystals.Length;
                float rot = progress * MathHelper.TwoPi;
                float orbitRadius = 128;
                Vector2 drawPosition = NPC.Center + Vector2.UnitX.RotatedBy(rot + (CrystalTimer * speed)) * orbitRadius;
                drawPosition += new Vector2(0, VectorHelper.Osc(0f, 32, offset: i));
                drawPosition -= screenPos;
                crystal.DrawPosition = NPC.Center + Vector2.UnitX.RotatedBy(rot + (CrystalTimer * speed)) * orbitRadius;
                crystal.DrawPosition += new Vector2(0, VectorHelper.Osc(0f, 32f, offset: i));

                Vector2 drawOrigin = crystal.Texture.Size() / 2;
                float drawScale = 1f;
                spriteBatch.Draw(crystal.Texture, drawPosition, null, Color.White, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (ChargeCrystals || ChargeCrystalTimer > 0f)
            {
                var shader = ShaderRegistry.MiscSilPixelShader;

                //The color to lerp to
                shader.UseColor(Color.White);

                //Should be between 0-1
                //1 being fully opaque
                //0 being the original color
                shader.UseSaturation(ChargeCrystalTimer);

                // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
                shader.Apply(null);

                for (float i = 0; i < Crystals.Length; i++)
                {
                    var crystal = Crystals[(int)i];
                    if (!crystal.Draw)
                        continue;

                    float progress = i / (float)Crystals.Length;
                    float rot = progress * MathHelper.TwoPi;
                    float orbitRadius = 128;
                    Vector2 drawPosition = NPC.Center + Vector2.UnitX.RotatedBy(rot + (CrystalTimer * speed)) * orbitRadius;
                    drawPosition += new Vector2(0, VectorHelper.Osc(0f, 32, offset: i));
                    drawPosition -= screenPos;
                    crystal.DrawPosition = NPC.Center + Vector2.UnitX.RotatedBy(rot + (CrystalTimer * speed)) * orbitRadius;
                    crystal.DrawPosition += new Vector2(0, VectorHelper.Osc(0f, 32f, offset: i));

                    Vector2 drawOrigin = crystal.Texture.Size() / 2;
                    float drawScale = 1f;
                    spriteBatch.Draw(crystal.Texture, drawPosition, null, Color.White, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
    }
}
