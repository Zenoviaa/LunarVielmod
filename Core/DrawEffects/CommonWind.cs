using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Core.DrawEffects
{
    public class CoreWind
    {
        public class CoreWindSlash
        {
            public CoreWindSlash()
            {
                timer = 0;
                duration = 60;
                swingXRadius = 64;
                swingYRadius = 16;
                swingRange = MathHelper.ToRadians(360);
                rotation = MathHelper.ToRadians(45);
                trailStartOffset = 0.35f;
                oldPos = new Vector2[12];
            }

            public float timer;
            public float duration;
            public float swingXRadius;
            public float swingYRadius;
            public float swingRange;
            public float rotation;
            public float trailStartOffset;
            public float expandMultiplier;
            public float rotOffset;
            public Func<float, float> easingFunc;
            public Vector2 position;
            public Vector2 offset;
            public Vector2[] oldPos;

            public void AI()
            {

                timer++;
                float lerpValue = timer / duration;
                lerpValue = MathHelper.Clamp(lerpValue, 0, 1);
                for (int i = 0; i < oldPos.Length; i++)
                {
                    float l = oldPos.Length;
                    //Lerp between the points
                    float progressOnTrail = i / l;

                    //Calculate starting lerp value
                    float startTrailLerpValue = MathHelper.Clamp(lerpValue - trailStartOffset, 0, 1);
                    float startTrailProgress = startTrailLerpValue;
                    startTrailProgress = easingFunc(startTrailLerpValue);


                    //Calculate ending lerp value
                    float endTrailLerpValue = lerpValue;
                    float endTrailProgress = endTrailLerpValue;
                    endTrailProgress = easingFunc(endTrailLerpValue);

                    //Lerp in between points
                    float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                    float xOffset2 = (swingXRadius * expandMultiplier) * MathF.Sin(smoothedTrailProgress * swingRange + swingRange + rotOffset);
                    float yOffset2 = (swingYRadius * expandMultiplier) * MathF.Cos(smoothedTrailProgress * swingRange + swingRange + rotOffset);
                    Vector2 pos = (position + offset) + new Vector2(xOffset2, yOffset2).RotatedBy(rotation);
                    oldPos[i] = pos;
                }
            }
        }

        private List<CoreWindSlash> _windSlashes;
        public CoreWind()
        {
            _windSlashes = new List<CoreWindSlash>();
            ColorFunc = DefaultColorFunction;
            WidthFunc = DefaultWidthFunction;
            EasingFunc = DefaultEasingFunction;
            ExpandMultiplier = 1f;
            WidthMultiplier = 1f;
            TrailTexture = TrailRegistry.SimpleTrail;
        }

        public Func<float, Color> ColorFunc { get; set; }
        public Func<float, float> WidthFunc { get; set; }
        public Func<float, float> EasingFunc { get; set; }
        public float ExpandMultiplier { get; set; }
        public float WidthMultiplier { get; set; }
        public Asset<Texture2D> TrailTexture { get; set; }
        private Color DefaultColorFunction(float progress)
        {
            float easedProgress = Easing.SpikeOutCirc(progress);
            return Color.Lerp(Color.Transparent, Color.White, easedProgress);
        }

        private float DefaultWidthFunction(float progress)
        {
            return EasingFunction.QuadraticBump(progress) * 16;
        }

        private float DefaultEasingFunction(float progress)
        {
            return progress;
        }
        private Color TrueColorFunction(float progress)
        {
            return ColorFunc(progress) * 0.4f;
        }

        private float TrueWidthFunction(float progress)
        {
            return WidthFunc(progress);
        }
        public void AI(Vector2 position)
        {
            _windSlashes = _windSlashes.Where(x => x.timer < x.duration).ToList();
            for (int i = 0; i < _windSlashes.Count; i++)
            {
                CoreWindSlash slash = _windSlashes[i];
                slash.position = position;
                slash.expandMultiplier = ExpandMultiplier;
                slash.AI();
            }
        }

        public CoreWindSlash NewSlash(Vector2 offset)
        {
            CoreWindSlash slash = new CoreWindSlash();
            slash.offset = offset;
            slash.easingFunc = DefaultEasingFunction;
            slash.rotation = offset.ToRotation();

            slash.swingXRadius = Main.rand.NextFloat(32, 80);
            slash.swingYRadius = Main.rand.NextFloat(8, 24);
            slash.rotOffset = Main.rand.NextFloat(1f, 5f);
            slash.duration = Main.rand.NextFloat(5, 12);
            _windSlashes.Add(slash);
            return slash;
        }
        public CoreWindSlash NewSlash(Vector2 offset, float rotation)
        {
            CoreWindSlash slash = new CoreWindSlash();
            slash.offset = offset;
            slash.easingFunc = DefaultEasingFunction;
            slash.rotation = rotation;

            slash.swingXRadius = Main.rand.NextFloat(32, 80);
            slash.swingYRadius = Main.rand.NextFloat(8, 24);
            slash.rotOffset = Main.rand.NextFloat(1f, 5f);
            slash.duration = Main.rand.NextFloat(5, 12);
            _windSlashes.Add(slash);
            return slash;
        }

        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes
            for (int i = 0; i < _windSlashes.Count; i++)
            {
                CoreWindSlash slash = _windSlashes[i];
                TrailDrawer.Draw(Main.spriteBatch, slash.oldPos, TrueColorFunction, TrueWidthFunction, shader);
            }
        }
    }
}
