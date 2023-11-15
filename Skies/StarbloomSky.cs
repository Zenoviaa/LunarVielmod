using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Skies
{
    public class StarbloomSky : CustomSky
    {
        private struct LightPillar
        {
            public Vector2 Position;

            public float Depth;
        }

        private struct BackgroundPillar
        {
            public Vector2 Position;

            public float Depth;

            //public int TextureIndex;

            public float SinOffset;

            public float AlphaFrequency;

            public float AlphaAmplitude;
        }

        private LightPillar[] lightPillar;
        private BackgroundPillar[] backgroundPillar;
        private UnifiedRandom randomValue = new UnifiedRandom();

        private bool skyActive;

        private float opacity;

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.1f;
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            return Main.DiscoColor * 0.5f * opacity;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            float fade = Main.GameUpdateCount % 60 / 60f;
            int index = (int)(Main.GameUpdateCount / 60 % 3);

            int minPillarDepth = -1;
            int maxPillarDepth = 0;
            for (int i = 0; i < lightPillar.Length; i++)
            {
                float depth = lightPillar[i].Depth;
                if (minPillarDepth == -1 && depth < maxDepth)
                {
                    minPillarDepth = i;
                }
                if (depth <= minDepth)
                {
                    break;
                }

                maxPillarDepth = i;
            }

            //draw giant beams behind the background
            if (minPillarDepth != -1)
            {
                Vector2 ScreenPos = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
                Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);

                float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);

                for (int i = minPillarDepth; i < maxPillarDepth; i++)
                {
                    Vector2 Depth = new Vector2(1f / lightPillar[i].Depth, 0.8f / lightPillar[i].Depth);
                    Vector2 PillarPosition = lightPillar[i].Position;
                    PillarPosition = (PillarPosition - ScreenPos) * Depth + ScreenPos - Main.screenPosition;

                    if (rectangle.Contains((int)PillarPosition.X, (int)PillarPosition.Y))
                    {
                        float realDepth = Depth.X * 500f;

                        float Rotation = Main.GlobalTimeWrappedHourly * 0.1f;

                        Color[] BeamColors = new Color[]
                        {
                            new Color(148, 80, 0),
                            new Color(80, 0, 148),
                            new Color(18, 148, 0)
                        };

                        Texture2D BeamTexture = ModContent.Request<Texture2D>("Stellamod/Textures/StarbloomSkyBeam").Value;

                        spriteBatch.Draw(BeamTexture, PillarPosition + new Vector2(0, 600), null, Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.75f * scale * opacity,
                        i % 2 == 0 ? MathF.Sin(-Rotation) : MathF.Sin(Rotation), new Vector2(0, BeamTexture.Height), new Vector2(realDepth / 70f, realDepth / 45f), SpriteEffects.None, 0f);
                    }
                }
            }

            int minBGPillarDepth = -1;
            int maxBGPillarDepth = 0;
            for (int i = 1; i < backgroundPillar.Length; i++)
            {
                float depth = backgroundPillar[i].Depth;
                if (minBGPillarDepth == -1 && depth < maxDepth && depth > minDepth)
                {
                    minBGPillarDepth = i;
                }
                if (depth <= minDepth)
                {
                    break;
                }

                maxBGPillarDepth = i;
            }

            if (minBGPillarDepth != -1)
            {
                //draw giant beams in the background
                Vector2 ScreenPos = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Rectangle rectangle = new Rectangle(-1000, -2000, 4000, 4000);

                for (int i = minBGPillarDepth + 1; i < maxBGPillarDepth; i++)
                {
                    Vector2 Depth = new Vector2(1f / backgroundPillar[i].Depth, 1f / backgroundPillar[i].Depth);
                    Vector2 position = (backgroundPillar[i].Position - ScreenPos) * Depth + ScreenPos - Main.screenPosition;

                    //only draw beams if they are on screen and they arent too high up
                    if (rectangle.Contains((int)position.X, (int)position.Y) && position.Y > -750)
                    {
                        float PillarIntensity = (float)Math.Sin(backgroundPillar[i].AlphaFrequency * Main.GlobalTimeWrappedHourly + backgroundPillar[i].SinOffset) * backgroundPillar[i].AlphaAmplitude + backgroundPillar[i].AlphaAmplitude;
                        PillarIntensity = MathHelper.Clamp(PillarIntensity, 0f, 1f);

                        //two color lists for pillar color variance
                        Color[] BeamColors = new Color[]
                        {
                            new Color(18, 148, 0),
                            new Color(148, 80, 0),
                            new Color(80, 0, 148)
                        };
                        Color[] BeamColors2 = new Color[]
                        {
                            new Color(80, 0, 148),
                            new Color(18, 148, 0),
                            new Color(148, 80, 0)
                        };

                        float Rotation = Main.GlobalTimeWrappedHourly * 0.4f;

                        Texture2D BeamTextureTop = ModContent.Request<Texture2D>("Stellamod/Textures/StarbloomSkyBeam").Value;
                        Texture2D BeamTextureBottom = ModContent.Request<Texture2D>("Stellamod/Textures/StarbloomSkyBeam2").Value;

                        spriteBatch.Draw(BeamTextureTop, position + new Vector2(0, 1550), null,
                        i % 2 == 0 ? Color.Lerp(BeamColors2[index], BeamColors2[(index + 1) % 3], fade) * 0.3f * opacity : Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.3f * opacity,
                        i % 2 == 0 ? MathF.Sin(Rotation) : MathF.Sin(-Rotation), new Vector2(BeamTextureTop.Width / 2, BeamTextureTop.Height), (Depth.X * 0.5f + 0.5f) * PillarIntensity, SpriteEffects.None, 0f);

                        spriteBatch.Draw(BeamTextureBottom, position + new Vector2(0, 1550), null,
                        i % 2 == 0 ? Color.Lerp(BeamColors2[index], BeamColors2[(index + 1) % 3], fade) * 0.3f * opacity : Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade) * 0.3f * opacity,
                        i % 2 == 0 ? MathF.Sin(Rotation) : MathF.Sin(-Rotation), new Vector2(BeamTextureBottom.Width / 2, 0), (Depth.X * 0.5f + 0.5f) * PillarIntensity, SpriteEffects.None, 0f);
                    }
                }
            }

            //draw the sky itself
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Color[] SkyColors = new Color[]
                {
                    new Color(35, 42, 217),
                    new Color(96, 47, 135),
                    new Color(158, 0, 145)
                };

                Texture2D SkyTexture = ModContent.Request<Texture2D>("Stellamod/Textures/StarbloomSkyBeam").Value;

                spriteBatch.Draw(SkyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2000) * 0.1f)), Main.screenWidth, Main.screenHeight),
                Color.Lerp(SkyColors[index], SkyColors[(index + 1) % 3], fade) * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * opacity));
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.3f + 0.7f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            opacity = 0.002f;
            skyActive = true;
            lightPillar = new LightPillar[40];
            for (int i = 0; i < lightPillar.Length; i++)
            {
                lightPillar[i].Position.X = (float)i / lightPillar.Length * (Main.maxTilesX * 16f + 20000f) + randomValue.NextFloat() * 40f - 20f - 20000f;
                lightPillar[i].Position.Y = randomValue.NextFloat() * 200f - 800f;
                lightPillar[i].Depth = randomValue.NextFloat() * 8f + 7f;
            }

            Array.Sort(lightPillar, SortPillarMethod);

            int amount = 120;
            int amountMult = 10;
            int currentPillar = 0;

            backgroundPillar = new BackgroundPillar[amount * amountMult];

            for (int i = 0; i < amount; i++)
            {
                float XAmount = i / (float)amount;
                for (int j = 0; j < amountMult; j++)
                {
                    float YAmount = j / (float)amountMult;
                    backgroundPillar[currentPillar].Position.X = XAmount * Main.maxTilesX * 16f;
                    backgroundPillar[currentPillar].Position.Y = YAmount * ((float)Main.worldSurface * 16f + 3000f) - 1000f;
                    backgroundPillar[currentPillar].Depth = randomValue.NextFloat() * 8f + 2f;
                    backgroundPillar[currentPillar].SinOffset = randomValue.NextFloat() * 6.28f;
                    backgroundPillar[currentPillar].AlphaAmplitude = randomValue.NextFloat() * 5f;
                    backgroundPillar[currentPillar].AlphaFrequency = randomValue.NextFloat() + 1f;
                    currentPillar++;
                }
            }

            Array.Sort(backgroundPillar, SortBgPillarMethod);
        }

        private int SortPillarMethod(LightPillar pillar1, LightPillar pillar2)
        {
            return pillar2.Depth.CompareTo(pillar1.Depth);
        }

        private int SortBgPillarMethod(BackgroundPillar pillar1, BackgroundPillar pillar2)
        {
            return pillar2.Depth.CompareTo(pillar1.Depth);
        }

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return (skyActive || opacity > 0.001f) && !Main.gameMenu;
        }
    }
}