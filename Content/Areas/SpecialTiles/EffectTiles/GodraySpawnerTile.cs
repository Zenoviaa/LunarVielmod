using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Foggy;
using Stellamod.Helpers;
using Stellamod.Tiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{


    public class GodraySpawnerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GodraySpawnerWall>();
        }
    }



    public class GodraySpawnerWall : FogSpawnerWall
    {
        public override string Texture => this.PathHere() + "/FogSpawnerWall";
        private float DayProgress
        {
            get
            {
                if (Main.dayTime)
                {
                    float dayProgress = (float)Main.time / (float)Main.dayLength;
                    return dayProgress;
                }
                else if (!Main.dayTime)
                {
                    float dayProgress = (float)Main.time / (float)Main.nightLength;
                    return dayProgress;
                }
                else
                {
                    return 0f;
                }

            }
        }
        private Color LightColor
        {
            get
            {

                Color lightColor;
                if (Main.dayTime)
                {
                    Color startColor = Color.Lerp(Color.Purple, Color.White, DayProgress * 2);
                    Color endColor = Color.Lerp(Color.White, Color.OrangeRed, DayProgress);
                    lightColor = Color.Lerp(startColor, endColor, DayProgress);
                    lightColor *= 0.5f;
                }
                else
                {
                    Color startColor = Color.Lerp(Color.White, Color.White, DayProgress);
                    Color endColor = Color.Lerp(Color.White, Color.White, DayProgress);
                    lightColor = Color.Lerp(startColor, endColor, DayProgress);
                    lightColor *= 0.24f;
                }

                return lightColor;
            }
        }
        public override void FogCreateFunction(Fog fog)
        {
            fog.disableWithFocus = true;
            fog.blendState = BlendState.AlphaBlend;
            fog.texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Godray");
            fog.startColor = Color.Transparent;
            fog.startScale = new Vector2(Main.rand.NextFloat(0.35f, 1.0f), Main.rand.NextFloat(0.3f, 0.9f)) * 3.5f;
            fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
            fog.rotation = Main.rand.NextFloat(-0.05f, 0.05f);
            fog.offset = Main.rand.NextVector2Circular(16, 16);
        }

        public override void FogUpdateFunction(Fog fog)
        {
            base.FogUpdateFunction(fog);
            float offset = fog.position.X + fog.position.Y;
            offset *= 10;
            Color color = Color.Lerp(Color.Transparent, LightColor, VectorHelper.Osc(0.6f, 1f, offset: offset)) * 0.5f;
            color *= MathF.Sin(offset + Main.GlobalTimeWrappedHourly);
            fog.startColor = Color.Lerp(fog.startColor, color, 0.1f);
            fog.rotation = MathHelper.Lerp(0, MathHelper.ToRadians(165), DayProgress) - MathHelper.ToRadians(45);


            if (DayProgress > 0.95f)
            {
                float fadeProgress = (DayProgress - 0.95f) / 0.05f;
                fadeProgress = 1f - fadeProgress;
                fog.startColor *= fadeProgress;
            }
            else if (DayProgress < 0.05f)
            {
                float fadeProgress = DayProgress / 0.05f;
                fog.startColor *= fadeProgress;
            }
        }

        public override BaseShader FogShaderFunction()
        {
            var shader = GodrayShader.Instance;
            shader.GlowColor = LightColor;
            shader.Apply();
            return shader;
        }
    }
}