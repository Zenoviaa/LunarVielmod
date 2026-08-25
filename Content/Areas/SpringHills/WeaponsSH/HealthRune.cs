using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class RuneHealing : ModBuff
    {
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
            if (Main.rand.NextBool(8))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Green,
                    outerColor = Color.Green,
                    scaleRange = new Vector2(0.2f, 0.5f)

                };
                var dp = DustParticle.Spawn(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), -Vector2.UnitY, spawnParams);
                dp.gravity = 0.05f;
            }

        }
    }

    public class HealthRune : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToRune(ModContent.ProjectileType<HealthRuneShaper>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankRune>();
        }
    }

    public class HealthRuneShaper : AbstractRuneProjectile
    {

        public override bool MatchShapeCheck(Vector2[] shapePoints)
        {
            if (shapePoints.Length <= 2)
                return false;
            //First let's estimate where the lines are at
            //We can do this with a dot product, comparing it with the last line to see if it changes position
            List<Line> lines = ShapeUtilities.GetLines(shapePoints, detectLineChangeThreshold: 0.5f);

            //There may be cases where it detects an extra line, those are fine
            //But if there's too many extra lines just say it's a fail, this will prevent scribbling from doing anything
            if (lines.Count >= 5)
                return false;

            //A square has 4 90 degree angles
            //We'll add a margin of error though so you can still mess up a little bit and get credit for it
            float marginOfError = 30;
            float targetAngle = 120;
            float numMatches = ShapeUtilities.CountAngles(lines, targetAngle, marginOfError);

            //Finally check that this is roughly a closed shape
            //Pretty sure that a simple distance check can't realistically be cheated
            float distanceToEndPointThreshold = 200;
            bool startAndEndPointsMeet = Vector2.Distance(lines[0].a, lines[lines.Count - 1].b) <= distanceToEndPointThreshold;
            return numMatches >= 3 && startAndEndPointsMeet;
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(3))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.LightGreen,
                    outerColor = Color.Green,
                    scaleRange = new Vector2(0.3f, 0.6f)
                };
                var dp = DustParticle.Spawn(DrawingPosition + Main.screenPosition, Main.rand.NextVector2Circular(3, 3), spawnParams);
                dp.gravity = 0.02f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle soundStyle = AssetRegistry.Sounds.Runes.RuneTeleport;
            soundStyle.PitchVariance = 0.2f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        public override void ApplyMagic(AbstractBellSummon minion)
        {
            minion.Heal();
        }



        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPointer);
            PixelationManager.QueuePrimitivesDrawAction(DrawRunePixelPrimitives);
            return false;
        }
        public float PreviewWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0f, 3, EasingFunction.InOutSine(EaseInRatio)) * MathF.Sin(completionRatio * 8 + Main.GlobalTimeWrappedHourly * 4);
        }

        public Color PreviewColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, EasingFunction.QuadraticBump(completionRatio));
        }

        public float WidthFunction(float completionRatio)
        {
            float osc = MathF.Sin(completionRatio * 384) * 0.5f + 0.5f;
            return MathHelper.SmoothStep(7, 2, completionRatio) * MathHelper.Lerp(1f, 0f, osc);
        }

        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 3;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGreen, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }


        private void DrawDottedLine(GraphicsDevice graphicsDevice)
        {
            List<Vector2> points = new List<Vector2>();
            Vector2 a = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            a.Y -= 400;
            Vector2 b = a + new Vector2(-100, 200);
            Vector2 c = a + new Vector2(100, 200);

            float numPoints = 12;
            for (float n = 0; n < numPoints; n++)
            {
                points.Add(Vector2.Lerp(a, b, n / numPoints));
            }
            for (float n = 0; n < numPoints; n++)
            {
                points.Add(Vector2.Lerp(b, c, n / numPoints));
            }
            for (float n = 0; n < numPoints; n++)
            {
                points.Add(Vector2.Lerp(c, a, n / numPoints));
            }

            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.White;
            TrailDrawer.Draw(Main.spriteBatch, points.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
        }

        private void DrawRunePixelPrimitives(GraphicsDevice graphicsDevice)
        {
            DrawDottedLine(graphicsDevice);
            //DrawInnerSquare();
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Green, ExtraMath.Osc(0f, 1f, speed: 32));
            shader.OuterColor = Color.Green;
            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction, shader, Main.screenPosition);


            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.LaserColor = Color.Green * 0.2f;
            shader.InnerColor = Color.Lerp(Color.GreenYellow, Color.Green, ExtraMath.Osc(0f, 1f, speed: 32)) * 0.2f;
            shader.OuterColor = Color.Green * 0.2f;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction2, shader, Main.screenPosition);
        }

        private void DrawPointer(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D pointerTexture = AssetManager.GlowMask.Shine.Value;
            Vector2 drawOrigin = pointerTexture.Size() / 2f;
            Vector2 drawCenter = DrawingPosition;

            Color glowColor2 = Color.Green;
            glowColor2 = Color.Lerp(glowColor2, Color.Black, ExtraMath.Osc(0.1f, 0.3f, 4));
            glowColor2.A = 0;
            spriteBatch.Draw(pointerTexture, drawCenter, null, glowColor2, 0, drawOrigin, ExtraMath.Osc(0.8f, 1f, speed: 32) * 0.2f, SpriteEffects.None, 0);

            Color glowColor = Color.White;
            glowColor = Color.Lerp(glowColor, Color.Black, ExtraMath.Osc(0.1f, 0.3f, 4));
            glowColor.A = 0;
            spriteBatch.Draw(pointerTexture, drawCenter, null, glowColor, 0, drawOrigin, ExtraMath.Osc(0.8f, 1f, speed: 32) * 0.12f, SpriteEffects.None, 0);

        }
        public override void OnDissipate(Vector2 trailPoint)
        {
            base.OnDissipate(trailPoint);
            if (Main.rand.NextBool(2))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.Green,
                    scaleRange = new Vector2(0.1f, 1f)
                    
                };
                var dp = DustParticle.Spawn(trailPoint + Main.screenPosition, Vector2.Zero, spawnParams);
                dp.gravity = 0;
            }
        }
    }
}

