using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity
{
    public partial class SanguineSingularity
    {
        private Color _haloColor;
        private Color TargetHaloColor;
        private float _stepDistance;
        private void CreateFootsteps()
        {
            float traveledDistance = Vector2.Distance(NPC.position, NPC.oldPosition);
            _stepDistance += traveledDistance;
            if (_stepDistance >= 100)
            {
                Vector2 pos = NPC.Bottom + new Vector2(Main.rand.NextFloat(-32f, 32f), 0);
                pos.Y += 40;
                var circleStep = Particle.NewParticle<CircleStepParticle>(pos, Vector2.UnitY);
                circleStep.color = Color.Red;
                circleStep.Rotation = NPC.rotation;
                _stepDistance = 0;
            }
        }

        private void UpdateDraw()
        {
            //Update stun halo color
            _haloColor = Color.Lerp(_haloColor, TargetHaloColor, 0.1f);

            //Update outline color
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);

            //Update animation frames for the singularity
            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);

            _draw.flashAlpha *= 0.99f;
            HandleGores();
            if (_draw.headless)
            {
                _draw.singularityScale = Vector2.Lerp(_draw.singularityScale, Vector2.One, 0.1f);
            }
            else
            {
                _draw.singularityScale = Vector2.Lerp(_draw.singularityScale, Vector2.Zero, 0.1f);
            }

            if (Timer % 5 == 0)
            {
                Vector2 upVelocity = -Vector2.UnitY;
                upVelocity *= 5;
                upVelocity = upVelocity.RotateRandom(0.5f);
                var d = Dust.NewDustPerfect(NPC.Center, DustID.Blood, upVelocity, Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = false;
            }

            NPC.spriteDirection = NPC.direction;
            CreateFootsteps();
        }

        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle frame = NPC.frame;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - screenPos;
                Vector2 drawOrigin = NPC.frame.Size() / 2f;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Lerp(Color.Red, Color.Blue, interpolant), Color.Transparent, interpolant);
                fadeColor *= _draw.afterImageAlpha;
                oldDrawPos += NPC.Size / 2f;
                fadeColor *= 0.5f;

                SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects, 0f);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawDashLine(spriteBatch, screenPos, drawColor);
            DrawWalkingTrail(spriteBatch, screenPos, drawColor);
            DrawFlamingTrail(spriteBatch, screenPos, drawColor);
            DrawChainTrail(spriteBatch, screenPos, drawColor);
            DrawAfterImage(spriteBatch, screenPos);
            DrawSingularity(spriteBatch, screenPos, drawColor);
            Draw(spriteBatch, screenPos, drawColor);
            DrawRedFlash(spriteBatch, screenPos, drawColor);
            GoreManager.Draw(spriteBatch, screenPos, drawColor);
            return false;
        }

        private Color GetWalkingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _draw.afterImageAlpha;
        }

        private float GetWalkingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0f, 20f, EasingFunction.QuadraticBump(completionRatio));
        }

        private void DrawDashLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float rotation = _dashLineRotation;
            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(lineTexture.Width / 2, 0);
            Vector2 drawCenter = NPC.Center - Main.screenPosition;
            drawColor = Color.Red;
            drawColor.A = 0;
            drawColor *= 0.5f;
            drawColor *= Timer / 30f;
            drawColor *= ExtraMath.Osc(0f, 1f, speed: 12);
            if (State != AIState.BloodyMegaCharge_Start)
                return;

            Vector2 scale = Vector2.One;
            scale.Y = 3;
            spriteBatch.Draw(lineTexture, drawCenter, null, drawColor, rotation - MathHelper.ToRadians(90), drawOrigin, scale, SpriteEffects.None, 0);
        }
        private void DrawWalkingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.Red;
            shader.OuterColor = Color.Transparent;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetWalkingTrailColor, GetWalkingTrailWidth, shader, offset: new Vector2(0, NPC.frame.Height / 2)); ;
        }


        private Color GetFlamingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _draw.afterImageAlpha * _draw.flamingTrailAlpha;
        }

        private float GetFlamingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(180, 180, completionRatio);
        }


        private void DrawFlamingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BlackFireShader.Instance;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            shader.InnerColor = Color.Red;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetFlamingTrailColor, GetFlamingTrailWidth, shader, offset: NPC.Size / 2f);
        }
        private void DrawRedFlash(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (_rotatingWalk)
                spriteEffects = SpriteEffects.None;
            Color glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= _draw.flashAlpha;
            glowColor *= 0.5f;
            spriteBatch.Draw(texture, drawCenter, frame, glowColor, NPC.rotation, drawOrigin, _draw.scale, spriteEffects, 0);

        }
        private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyBigTexture).Value;

            Vector2 center = _singularityDrawOverridePosition != Vector2.Zero ? _singularityDrawOverridePosition : NPC.Center + GetDrawOffset();
            Vector2 drawPosition = center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One;
            drawScale *= _draw.singularityScale;
            drawScale *= ExtraMath.Osc(0.9f, 1f, speed: 18f);

            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);

            Color redSingularity = Color.Red;
            redSingularity *= _draw.alpha;
            spriteBatch.Draw(texture, drawPosition, null, redSingularity, NPC.rotation, drawOrigin, drawScale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Red, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor = diskDrawColor.MultiplyRGB(Color.Red);
            diskDrawColor.A = 0;
            diskDrawColor *= _draw.alpha;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * scaleOsc * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * scaleOsc * 0.45f, SpriteEffects.None, 0);


            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2").Value;
            float rotOffset = MathHelper.ToRadians(-30f + ExtraMath.Osc(5f, 10f, speed: 2));
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(1.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(3.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(7.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);

            rotOffset = MathHelper.ToRadians(25f + ExtraMath.Osc(-10f, -5f, speed: 2, offset: 2));
            //Inverse rings
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(1.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(3.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(7.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            DrawIncresionDiskBottom(spriteBatch, screenPos, color);
            DrawIncresionDiskTop(spriteBatch, screenPos, color);
        }
        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;
            incresionDiskDrawColor *= _draw.alpha;

            Vector2 center = _singularityDrawOverridePosition != Vector2.Zero ? _singularityDrawOverridePosition : NPC.Center + GetDrawOffset();
            Vector2 drawPos = center - screenPos;

            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = NPC.scale * _draw.singularityScale.X * 1.75f;
            drawScale *= 0.4f;
            float drawRotation = NPC.rotation;
            drawRotation -= MathHelper.ToRadians(30);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;
            incresionDiskDrawColor *= _draw.alpha;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Blue;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;
            incresionDiskDrawColor *= _draw.alpha;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale * 2, SpriteEffects.None, 0);



            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation - MathHelper.ToRadians(90), drawOrigin, drawScale * 2, SpriteEffects.None, 0);
        }


        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;
            incresionDiskDrawColor *= _draw.alpha;

            Vector2 center = _singularityDrawOverridePosition != Vector2.Zero ? _singularityDrawOverridePosition : NPC.Center + GetDrawOffset();

            Vector2 drawPos = center - screenPos;

            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = NPC.scale * 3 * _draw.singularityScale.X;
            drawScale *= 0.4f;
            float drawRotation = NPC.rotation;
            drawRotation -= MathHelper.ToRadians(30);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            drawRotation -= MathHelper.ToRadians(90);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (_animation)
            {
                case AnimationState.Idle:
                    _frame = 0;
                    break;
                case AnimationState.Walk:
                    if (_frame < 2)
                    {
                        _frame = 2;
                    }
                    else if (_frame >= 10)
                    {
                        _frame = 2;
                    }
                    break;
                case AnimationState.Run:
                    if (_frame < 18)
                    {
                        _frame = 18;
                    }
                    else if (_frame >= 23)
                    {
                        _frame = 18;
                    }
                    break;
                case AnimationState.Prepare:
                    if (_frame < 28)
                    {
                        _frame = 28;
                    }
                    else if (_frame >= 33)
                    {
                        _frame = 28;
                    }
                    break;
            }
            int frame = _frame;
            if (_draw.headless)
            {
                switch (_animation)
                {
                    case AnimationState.Idle:
                        frame += 1;
                        break;
                    case AnimationState.Walk:
                        frame += 8;
                        break;
                    case AnimationState.Run:
                        frame += 5;
                        break;
                }
            }
            NPC.frame.Y = frameHeight * frame;
        }

        private Vector2 GetDrawOffset()
        {
            Vector2 drawOffset = Vector2.Zero;
            drawOffset.X += 60;
            if (!_rotatingWalk)
            {
                drawOffset.X *= NPC.spriteDirection;
            }
            drawOffset.Y -= 35;
            if (_rotatingWalk)
            {
                drawOffset.Y *= NPC.spriteDirection;
            }
            drawOffset = drawOffset.RotatedBy(NPC.rotation);
            return drawOffset;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (_rotatingWalk)
            {
                spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            }
             
            spriteBatch.Draw(texture, drawCenter, frame, color * _draw.alpha, NPC.rotation, drawOrigin, _draw.scale, spriteEffects, 0);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawSprite(spriteBatch, screenPos, Color.White.MultiplyRGB(lightColor));
        }

        private void DrawOutline(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            DrawSprite(spriteBatch, screenPos, _draw.outlineColor);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawOutline(spriteBatch, screenPos + v);
            DrawOutline(spriteBatch, screenPos - v);
            DrawOutline(spriteBatch, screenPos + h);
            DrawOutline(spriteBatch, screenPos - h);
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
            DrawHelper.DrawHalo(NPC.Center - new Vector2(0, 72), _haloColor, 3);
        }

    }
}
